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
    public static class GE06CalcContoursByDistance
    {
        /// <summary>
        /// Расчет  для CalculationType == CreateContoursByDistance
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        public static void Calculation(
                                            in Ge06CalcData ge06CalcData,
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
                    message+= $"The following Assignments are not validated: {notValidBroadcastingAssignment}";
                }
                if (!string.IsNullOrEmpty(notValidBroadcastingAllotment))
                {
                    message += $"The following Alotment are not validated: {notValidBroadcastingAllotment}";
                }
                throw new Exception(message);
            }

            var affectedServices = new List<string>();
            if (broadcastingContextBase.Allotments != null)
            {
                if (broadcastingContextBase.Allotments.EmissionCharacteristics != null)
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


            var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
            {
                BroadcastingAllotment = broadcastingContextBase.Allotments,
                BroadcastingAssignments = broadcastingContextBase.Assignments
            };
            //1.Определение центра гравитации(2.1)
            var pointEarthGeometricBarycenter = new PointEarthGeometric();
            gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometricBarycenter);

            try
            {
                pointEarthGeometricsResult = pointEarthGeometricPool.Take();
                countoursPointExtendedBuffer = countoursPointExtendedPool.Take();
                contoursResultBuffer = contoursResultPool.Take();

                int indexForCountoursPointExtendedBuffer = 0;
                for (int i = 0; i < ge06CalcData.Ge06TaskParameters.Distances.Length; i++)
                {
                    // 2. Построение контуров фиксированной дистанции относительно центра гравитации.
                    // Базируемся на функции CreateContourFromPointByDistance если у нас только BroadcastingAssignment []

                    if ((broadcastingContextBase.Allotments == null) && ((broadcastingContextBase.Assignments != null) && (broadcastingContextBase.Assignments.Length > 0)))
                    {
                        var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                        {
                            Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                            Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                            PointEarthGeometricCalc = pointEarthGeometricBarycenter
                        };



                        earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);
                        for (int k = 0; k < sizeResultBuffer; k++)
                        {
                            var point = new Point()
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
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Distance = ge06CalcData.Ge06TaskParameters.Distances[i];
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].FS = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in point,
                                                                                            broadcastingTypeContext,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                            iterationHandlerFieldStrengthCalcData,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );
                            if (ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue)
                            {
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Height = ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value;
                            }

                            var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
                            {
                                Longitude_dec = point.Longitude,
                                Latitude_dec = point.Latitude
                            });

                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].administration = adm;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeContext = broadcastingTypeContext;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeCalculation = BroadcastingTypeCalculation.Distance;

                            indexForCountoursPointExtendedBuffer++;
                        }
                    }
                    //или на функции CreateContourFromContureByDistance если у нас есть BroadcastingAllotment 
                    else
                    {
                        if (broadcastingContextBase.Allotments != null)
                        {
                            if (broadcastingContextBase.Allotments.AllotmentParameters != null)
                            {
                                var areaPoints = broadcastingContextBase.Allotments.AllotmentParameters.Contur;
                                if ((areaPoints != null) && (areaPoints.Length > 0))
                                {
                                    var pointEarthGeometrics = new PointEarthGeometric[areaPoints.Length];
                                    for (int h = 0; h < areaPoints.Length; h++)
                                    {
                                        pointEarthGeometrics[h] = new PointEarthGeometric(areaPoints[h].Lon_DEC, areaPoints[h].Lat_DEC, CoordinateUnits.deg);
                                    }

                                    var contourFromContureByDistanceArgs = new ContourFromContureByDistanceArgs()
                                    {
                                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                        Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                                        PointBaryCenter = pointEarthGeometricBarycenter,
                                        ContourPoints = pointEarthGeometrics
                                    };


                                    earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                                    for (int k = 0; k < sizeResultBuffer; k++)
                                    {
                                        var point = new Point()
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
                                        countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Distance = ge06CalcData.Ge06TaskParameters.Distances[i];
                                        countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].FS = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in point,
                                                                                            broadcastingTypeContext,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                            iterationHandlerFieldStrengthCalcData,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );




                                        if (ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue)
                                        {
                                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Height = ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value;
                                        }

                                        var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                        {
                                            Longitude_dec = point.Longitude,
                                            Latitude_dec = point.Latitude
                                        });

                                        countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].administration = adm;
                                        countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeContext = broadcastingTypeContext;
                                        countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeCalculation = BroadcastingTypeCalculation.Distance;

                                        indexForCountoursPointExtendedBuffer++;
                                    }
                                }
                            }
                        }
                    }
                }
                var lstCountoursPointExtendeds = new List<CountoursPointExtended>();
                for (int f = 0; f < indexForCountoursPointExtendedBuffer; f++)
                {
                    lstCountoursPointExtendeds.Add(countoursPointExtendedBuffer[f]);
                }
                FillContoursResultOnDistance.Fill(ge06CalcData.Ge06TaskParameters.Distances, lstCountoursPointExtendeds.ToArray(), broadcastingTypeContext, ref contoursResultBuffer, out int sizeBufferContoursResult);
                if (sizeBufferContoursResult > 0)
                {
                    ge06CalcResult.ContoursResult = new ContoursResult[sizeBufferContoursResult];
                    for (int f = 0; f < sizeBufferContoursResult; f++)
                    {
                        ge06CalcResult.ContoursResult[f] = contoursResultBuffer[f];
                    }
                    ge06CalcResult.AffectedADMResult = FillAffectedADMResult.Fill(ge06CalcResult.ContoursResult, string.Join(",", affectedServices));
                }
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
            GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextBase, ref ge06CalcResult);
        }
    }
}
