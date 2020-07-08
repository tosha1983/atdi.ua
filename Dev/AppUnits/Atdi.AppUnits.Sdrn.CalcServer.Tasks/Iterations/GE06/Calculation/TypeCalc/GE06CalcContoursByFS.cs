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
                throw new Exception(message);
            }


            if (broadcastingContextBase.Allotments != null)
            {
                if ((broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC1)
                           || (broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC2)
                               || (broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC3))
                {
                    affectedServices.Add("BT");
                }
                else if ((broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC4)
                    || (broadcastingContextBase.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC5)
                        )
                {
                    affectedServices.Add("BC");
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


                    try
                    {
                        pointEarthGeometricsResult = pointEarthGeometricPool.Take();
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
                            var countoursPoints = new CountoursPoint[sizeResultBuffer];
                            for (int k = 0; k < sizeResultBuffer; k++)
                            {
                                var pointFS = new Point()
                                {
                                    Longitude = pointEarthGeometricsResult[k].Longitude,
                                    Latitude = pointEarthGeometricsResult[k].Latitude
                                };

                                countoursPoints[k] = new CountoursPoint();
                                countoursPoints[k].Lon_DEC = pointEarthGeometricsResult[k].Longitude;
                                countoursPoints[k].Lat_DEC = pointEarthGeometricsResult[k].Latitude;
                                if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                                {
                                    countoursPoints[k].PointType = PointType.Etalon;
                                }
                                if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                                {
                                    countoursPoints[k].PointType = PointType.Unknown;
                                }
                                countoursPoints[k].FS = ge06CalcData.Ge06TaskParameters.FieldStrength[i]; //(int)CalcFieldStrengthInPointGE06(ge06CalcData, in pointFS, broadcastingTypeContext);
                                if (ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue)
                                {
                                    countoursPoints[k].Height = ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value;
                                }

                                var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                {
                                    Longitude_dec = pointFS.Longitude,
                                    Latitude_dec = pointFS.Latitude
                                });

                                dicCountoursPoints.Add(countoursPoints[k], adm);
                            }
                        }
                    }

                    finally
                    {
                        if (pointEarthGeometricsResult != null)
                        {
                            pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                        }
                    }
                }
            }
            if ((dicCountoursPoints != null) && (dicCountoursPoints.Count > 0))
            {
                var fieldStrength = ge06CalcData.Ge06TaskParameters.FieldStrength;
                var lstCountoursPoints = dicCountoursPoints.ToList();
                var arrPointType = new PointType[4] { PointType.Etalon, PointType.Unknown, PointType.Affected, PointType.Correct };
                if (lstCountoursPoints != null)
                {
                    for (int n = 0; n < arrPointType.Length; n++)
                    {
                        var distinctByPointType = lstCountoursPoints.FindAll(c => c.Key.PointType == arrPointType[n]);
                        if (distinctByPointType != null)
                        {
                            for (int i = 0; i < fieldStrength.Length; i++)
                            {
                                var distinctByFieldStrength = distinctByPointType.FindAll(c => c.Key.FS == fieldStrength[i]);
                                if (distinctByFieldStrength != null)
                                {
                                    var distinctAdmByAdm = distinctByFieldStrength.Select(c => c.Value).Distinct();
                                    if (distinctAdmByAdm != null)
                                    {
                                        var arrDistinctAdmByFieldStrength = distinctAdmByAdm.ToArray();
                                        for (int k = 0; k < arrDistinctAdmByFieldStrength.Length; k++)
                                        {
                                            var listContourPoints = lstCountoursPoints.FindAll(c => c.Key.FS == fieldStrength[i] && c.Key.PointType == arrPointType[n] && c.Value == arrDistinctAdmByFieldStrength[k]);
                                            if (listContourPoints != null)
                                            {

                                                var contourType = ContourType.Unknown;
                                                if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                                                {
                                                    contourType = ContourType.Etalon;
                                                }
                                                if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                                                {
                                                    contourType = ContourType.New;
                                                }

                                                var allPoints = listContourPoints.Select(c => c.Key).ToArray();
                                                lstContoursResults.Add(new ContoursResult()
                                                {
                                                    AffectedADM = arrDistinctAdmByFieldStrength[k],
                                                    ContourType = contourType,
                                                    CountoursPoints = allPoints,
                                                    FS = fieldStrength[i],
                                                    PointsCount = allPoints.Length
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ge06CalcResult.ContoursResult = lstContoursResults.ToArray();


                var distinctAdm = lstContoursResults.Select(c => c.AffectedADM).Distinct();
                if (distinctAdm != null)
                {
                    var arrDistinctAdmByAdm = distinctAdm.ToArray();
                    if (arrDistinctAdmByAdm.Length > 0)
                    {
                        var affectedADMRes = new AffectedADMResult[arrDistinctAdmByAdm.Length];
                        for (int k = 0; k < arrDistinctAdmByAdm.Length; k++)
                        {
                            affectedADMRes[k] = new AffectedADMResult();
                            affectedADMRes[k].ADM = arrDistinctAdmByAdm[k];
                            affectedADMRes[k].AffectedServices = string.Join(",", affectedServices);
                        }
                        ge06CalcResult.AffectedADMResult = affectedADMRes;
                    }
                }
            }

            ge06CalcResult.ContoursResult = lstContoursResults.ToArray();
            GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextBase, ref ge06CalcResult);
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
