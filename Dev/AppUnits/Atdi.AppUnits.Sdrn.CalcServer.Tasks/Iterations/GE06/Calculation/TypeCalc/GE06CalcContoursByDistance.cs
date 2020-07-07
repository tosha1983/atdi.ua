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
                                            IIterationsPool iterationsPool,
                                            IObjectPoolSite poolSite,
                                            ITransformation transformation,
                                            ITaskContext taskContext,
                                            IGn06Service gn06Service,
                                            IEarthGeometricService earthGeometricService,
                                            Idwm.IIdwmService idwmService
                                            )
        {

            var pointEarthGeometricsResult = default(PointEarthGeometric[]);

            BroadcastingContextBase broadcastingContextBase = null;
            if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            }
            if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;
            }


            var affectedServices = new List<string>();

            if (((GE06Validation.ValidationAssignment(broadcastingContextBase.Assignments)) && (GE06Validation.ValidationAllotment(broadcastingContextBase.Allotments))) == false)
            {
                throw new Exception("Input parameters failed validation");
            }


            if (broadcastingContextBase.Allotments != null)
            {
                affectedServices.Add(broadcastingContextBase.Allotments.AdminData.StnClass);
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

            var dicCountoursPoints = new Dictionary<CountoursPoint, string>();

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


                    try
                    {
                        pointEarthGeometricsResult = pointEarthGeometricPool.Take();
                        earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);
                        var countoursPoints = new CountoursPoint[sizeResultBuffer];
                        for (int k = 0; k < sizeResultBuffer; k++)
                        {
                            var point = new Point()
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
                            countoursPoints[k].Distance = ge06CalcData.Ge06TaskParameters.Distances[i];
                            countoursPoints[k].FS = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in point,
                                                                                            broadcastingTypeContext,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationsPool,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );
                            if (ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue)
                            {
                                countoursPoints[k].Height = ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value;
                            }

                            var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
                            {
                                Longitude_dec = point.Longitude,
                                Latitude_dec = point.Latitude
                            });

                            dicCountoursPoints.Add(countoursPoints[k], adm);
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

                                try
                                {
                                    pointEarthGeometricsResult = pointEarthGeometricPool.Take();

                                    earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                                    var countoursPoints = new CountoursPoint[sizeResultBuffer];
                                    for (int k = 0; k < sizeResultBuffer; k++)
                                    {
                                        var point = new Point()
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
                                        countoursPoints[k].Distance = ge06CalcData.Ge06TaskParameters.Distances[i];
                                        countoursPoints[k].FS = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in point,
                                                                                            broadcastingTypeContext,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationsPool,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );




                                        if (ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue)
                                        {
                                            countoursPoints[k].Height = ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value;
                                        }

                                        var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                        {
                                            Longitude_dec = point.Longitude,
                                            Latitude_dec = point.Latitude
                                        });

                                        dicCountoursPoints.Add(countoursPoints[k], adm);
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
                    }
                }
            }
            var lstContoursResults = new List<ContoursResult>();
            var distances = ge06CalcData.Ge06TaskParameters.Distances;
            if ((dicCountoursPoints != null) && (dicCountoursPoints.Count > 0))
            {
                var lstCountoursPoints = dicCountoursPoints.ToList();
                var arrPointType = new PointType[4] { PointType.Etalon, PointType.Unknown, PointType.Affected, PointType.Correct };
                if (lstCountoursPoints != null)
                {
                    for (int n = 0; n < arrPointType.Length; n++)
                    {
                        var distinctByPointType = lstCountoursPoints.FindAll(c => c.Key.PointType == arrPointType[n]);
                        if (distinctByPointType != null)
                        {
                            for (int i = 0; i < distances.Length; i++)
                            {
                                var distinctByDistance = distinctByPointType.FindAll(c => c.Key.Distance == distances[i]);
                                if (distinctByDistance != null)
                                {
                                    var distinctAdmByAdm = distinctByDistance.Select(c => c.Value).Distinct();
                                    if (distinctAdmByAdm != null)
                                    {
                                        var arrDistinctAdmByDistance = distinctAdmByAdm.ToArray();
                                        for (int k = 0; k < arrDistinctAdmByDistance.Length; k++)
                                        {
                                            var listContourPoints = lstCountoursPoints.FindAll(c => c.Key.Distance == distances[i] && c.Key.PointType == arrPointType[n] && c.Value == arrDistinctAdmByDistance[k]);
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
                                                    AffectedADM = arrDistinctAdmByDistance[k],
                                                    ContourType = contourType,
                                                    CountoursPoints = allPoints,
                                                    Distance = distances[i],
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
            }
            ge06CalcResult.ContoursResult = lstContoursResults.ToArray();
            var distinctAdm = lstContoursResults.Select(c => c.AffectedADM).Distinct();
            if ((distinctAdm != null) && (distinctAdm.Count() > 0))
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
            GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextBase, ref ge06CalcResult);
        }
    }
}
