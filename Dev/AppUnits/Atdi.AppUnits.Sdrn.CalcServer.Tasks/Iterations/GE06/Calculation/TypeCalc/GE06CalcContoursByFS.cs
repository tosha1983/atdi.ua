using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Idwm = Atdi.Contracts.Sdrn.DeepServices.IDWM;
using IdwmDataModel = Atdi.DataModels.Sdrn.DeepServices.IDWM;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class GE06CalcContoursByFS
    {
        /// <summary>
        /// Расчет  для  CalculationType == CreateContoursByFS
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        public static void Calculation(
                                            Ge06CalcData ge06CalcData,
                                            BroadcastingTypeContext broadcastingTypeContext,
                                            ref Ge06CalcResult ge06CalcResult,
                                            IObjectPool<PointEarthGeometric[]> pointEarthGeometricPool,
                                            IObjectPool<CountoursPointExtended[]> countoursPointExtendedPool,
                                            IObjectPool<ContoursResult[]> contoursResultPool,
                                            IIterationHandler<BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult> iterationHandlerBroadcastingFieldStrengthCalcData,
                                            IIterationHandler<FieldStrengthCalcData, FieldStrengthCalcResult> iterationHandlerFieldStrengthCalcData,
                                            IObjectPoolSite poolSite,
                                            ITransformation transformation,
                                            ITaskContext taskContext,
                                            IGn06Service gn06Service,
                                            IEarthGeometricService earthGeometricService,
                                            Idwm.IIdwmService idwmService
                                           )
        {
            var pointEarthGeometricsResult = default(PointEarthGeometric[]);
            var countoursPointExtendedBuffer = default(CountoursPointExtended[]);
            var contoursResultBuffer = default(ContoursResult[]);

            var affectedServices = new List<string>();

            BroadcastingContextBase broadcastingContextBase = null;
            if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            }
            if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;
            }

            string notValidBroadcastingAssignment = string.Empty;
            string notValidBroadcastingAllotment = string.Empty;

            if (((GE06Validation.ValidationAssignment(broadcastingContextBase.Assignments, out notValidBroadcastingAssignment)) && (GE06Validation.ValidationAllotment(broadcastingContextBase.Allotments, out notValidBroadcastingAllotment))) == false)
            {
                string message = "";
                if (!string.IsNullOrEmpty(notValidBroadcastingAssignment))
                {
                    message += $"The following Assignments are not validated: {notValidBroadcastingAssignment}";
                }
                if (!string.IsNullOrEmpty(notValidBroadcastingAllotment))
                {
                    message += $"The following Allotment are not validated: {notValidBroadcastingAllotment}";
                }
                taskContext.SendEvent(new CalcResultEvent
                {
                    Level = CalcResultEventLevel.Error,
                    Context = "Ge06CalcIteration",
                    Message = message
                });
                throw new Exception(message);
            }


            if (broadcastingContextBase.Allotments != null)
            {
                if ((broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC1)
                           || (broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC2)
                               || (broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC3))
                {
                    if (!affectedServices.Contains("BT"))
                    {
                        affectedServices.Add("BT");
                    }
                }
                else if ((broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC4)
                    || (broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC5)
                        )
                {
                    if (!affectedServices.Contains("BC"))
                    {
                        affectedServices.Add("BC");
                    }
                }
            }
            if (broadcastingContextBase.Assignments != null)
            {
                for (int i = 0; i < broadcastingContextBase.Assignments.Length; i++)
                {
                    if (!affectedServices.Contains(broadcastingContextBase.Assignments[i].AdmData.StnClass))
                    {
                        affectedServices.Add(broadcastingContextBase.Assignments[i].AdmData.StnClass);
                    }
                }
            }


            var lstContoursResults = new List<ContoursResult>();
            var dicCountoursPoints = new Dictionary<CountoursPoint, string>();


            if ((broadcastingContextBase.Allotments != null) || ((broadcastingContextBase.Assignments != null) && (broadcastingContextBase.Assignments.Length > 0)))
            {

                try
                {
                    int currPercentComplete = 0;
                    int indexForCountoursPointExtendedBuffer = 0;

                    pointEarthGeometricsResult = pointEarthGeometricPool.Take();
                    countoursPointExtendedBuffer = countoursPointExtendedPool.Take();
                    contoursResultBuffer = contoursResultPool.Take();

                    var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
                    {
                        BroadcastingAllotment = broadcastingContextBase.Allotments,
                        BroadcastingAssignments = broadcastingContextBase.Assignments
                    };


                   
                    //1.Определение центра гравитации(2.1)
                    var pointEarthGeometricBarycenter = new PointEarthGeometric();
                    gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometricBarycenter);

                    for (int i = 0; i < ge06CalcData.Ge06TaskParameters.FieldStrength.Length; i++)
                    {

                        // 2. Построение контуров фиксированной дистанции относительно центра гравитации.
                        // Базируемся на функции CreateContourFromPointByDistance если у нас только BroadcastingAssignment []

                        var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                        {
                            Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                            TriggerFieldStrength = ge06CalcData.Ge06TaskParameters.FieldStrength[i],
                            BaryCenter = pointEarthGeometricBarycenter
                        };



                        int sizeResultBuffer = 0;

                        // модель из контекста, высота абонента из формы, процент времени из формы
                        var propModel = ge06CalcData.PropagationModel;
                        GE06PropagationModel.GetPropagationModelForContoursByFS(ref propModel, 50, (float)ge06CalcData.Ge06TaskParameters.PercentageTime.Value);
                        ge06CalcData.PropagationModel = propModel;


                        if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                        {
                            earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthBRIFIC(destinationPoint,
                                                                                                                                               ge06CalcData,
                                                                                                                                               pointEarthGeometricPool,
                                                                                                                                               iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                                                                               iterationHandlerFieldStrengthCalcData,
                                                                                                                                               poolSite,
                                                                                                                                               transformation,
                                                                                                                                               taskContext,
                                                                                                                                               gn06Service), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResult, out sizeResultBuffer);
                        }
                        if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                        {
                            earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthICSM(destinationPoint,
                                                                                                                                             ge06CalcData,
                                                                                                                                             pointEarthGeometricPool,
                                                                                                                                             iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                                                                             iterationHandlerFieldStrengthCalcData,
                                                                                                                                             poolSite,
                                                                                                                                             transformation,
                                                                                                                                             taskContext,
                                                                                                                                             gn06Service), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResult, out sizeResultBuffer);
                        }
                        if (sizeResultBuffer > 0)
                        {
                          
                            for (int k = 0; k < sizeResultBuffer; k++)
                            {
                                var pointFS = new Point()
                                {
                                    Longitude = pointEarthGeometricsResult[k].Longitude,
                                    Latitude = pointEarthGeometricsResult[k].Latitude
                                };

                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer] = new CountoursPointExtended();
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lon_DEC = pointEarthGeometricsResult[k].Longitude;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lat_DEC = pointEarthGeometricsResult[k].Latitude;
                                if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                                {
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].PointType = PointType.Etalon;
                                }
                                if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                                {
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].PointType = PointType.Unknown;
                                }
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].FS = ge06CalcData.Ge06TaskParameters.FieldStrength[i]; 
                                if (ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue)
                                {
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Height = ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value;
                                }

                                var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                {
                                    Longitude_dec = pointFS.Longitude,
                                    Latitude_dec = pointFS.Latitude
                                });

                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].administration = adm;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeContext = broadcastingTypeContext;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeCalculation = BroadcastingTypeCalculation.FieldStrength;

                                indexForCountoursPointExtendedBuffer++;

                                UpdateProgress.UpdatePercentComplete100(ge06CalcData.Ge06TaskParameters.FieldStrength.Length, sizeResultBuffer, i, k, ref currPercentComplete, "ContoursByFS", taskContext);
                            }
                        }
                    }
                    var lstCountoursPointExtendeds = new List<CountoursPointExtended>();
                    for (int f = 0; f < indexForCountoursPointExtendedBuffer; f++)
                    {
                        lstCountoursPointExtendeds.Add(countoursPointExtendedBuffer[f]);
                    }

                    FillContoursResultOnFS.Fill(ge06CalcData.Ge06TaskParameters.FieldStrength, lstCountoursPointExtendeds.ToArray(), broadcastingTypeContext, ref contoursResultBuffer, out int sizeBufferContoursResult);
                    if (sizeBufferContoursResult > 0)
                    {
                        ge06CalcResult.ContoursResult = new ContoursResult[sizeBufferContoursResult];
                        for (int f = 0; f < sizeBufferContoursResult; f++)
                        {
                            ge06CalcResult.ContoursResult[f] = contoursResultBuffer[f];
                        }
                        ge06CalcResult.AffectedADMResult = FillAffectedADMResult.Fill(ge06CalcResult.ContoursResult, string.Join(",", affectedServices));
                    }
                    UpdateProgress.UpdatePercentComplete100(ref currPercentComplete, "ContoursByFS", taskContext);
                }
                finally
                {
                    if (countoursPointExtendedBuffer != null)
                    {
                        countoursPointExtendedPool.Put(countoursPointExtendedBuffer);
                    }
                    if (contoursResultBuffer != null)
                    {
                        contoursResultPool.Put(contoursResultBuffer);
                    }
                    if (pointEarthGeometricsResult != null)
                    {
                        pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                    }
                }
                GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextBase, broadcastingTypeContext, ref ge06CalcResult);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationPoint"></param>
        /// <returns></returns>
        public static double CalcFieldStrengthBRIFIC(PointEarthGeometric destinationPoint,
                                            Ge06CalcData ge06CalcData,
                                            IObjectPool<PointEarthGeometric[]> pointEarthGeometricPool,
                                            IIterationHandler<BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult> iterationHandlerBroadcastingFieldStrengthCalcData,
                                            IIterationHandler<FieldStrengthCalcData, FieldStrengthCalcResult> iterationHandlerFieldStrengthCalcData,
                                            IObjectPoolSite poolSite,
                                            ITransformation transformation,
                                            ITaskContext taskContext,
                                            IGn06Service gn06Service
                                           
             )
        {
            var point = new Point()
            {
                Longitude = destinationPoint.Longitude,
                Latitude = destinationPoint.Latitude,
                Height_m = ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue ? ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value : 0
            };

            return CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                     in point,
                                                     BroadcastingTypeContext.Brific,
                                                     pointEarthGeometricPool,
                                                     iterationHandlerBroadcastingFieldStrengthCalcData,
                                                     iterationHandlerFieldStrengthCalcData,
                                                     poolSite,
                                                     transformation,
                                                     taskContext,
                                                     gn06Service
                                                     );

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationPoint"></param>
        /// <returns></returns>
        public static double CalcFieldStrengthICSM(PointEarthGeometric destinationPoint, 
                                            Ge06CalcData ge06CalcData,
                                            IObjectPool<PointEarthGeometric[]> pointEarthGeometricPool,
                                            IIterationHandler<BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult> iterationHandlerBroadcastingFieldStrengthCalcData,
                                            IIterationHandler<FieldStrengthCalcData, FieldStrengthCalcResult> iterationHandlerFieldStrengthCalcData,
                                            IObjectPoolSite poolSite,
                                            ITransformation transformation,
                                            ITaskContext taskContext,
                                            IGn06Service gn06Service)
        {
           
            var point = new Point()
            {
                Longitude = destinationPoint.Longitude,
                Latitude = destinationPoint.Latitude,
                Height_m = ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue ? ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value : 0
            };
            return CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                  in point,
                                                  BroadcastingTypeContext.Icsm,
                                                  pointEarthGeometricPool,
                                                  iterationHandlerBroadcastingFieldStrengthCalcData,
                                                  iterationHandlerFieldStrengthCalcData,
                                                  poolSite,
                                                  transformation,
                                                  taskContext,
                                                  gn06Service
                                                  );
        }
    }
}
