using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.IDWM;
using Atdi.DataModels.Sdrn.DeepServices.IDWM;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.Platform.Logging;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Atdi.AppUnits.Sdrn.DeepServices.GN06
{
    public static class BarycenterGE06
    {
        public static void Calc(IEarthGeometricService earthGeometricService, IIdwmService idwmService, in BroadcastingCalcBarycenterGE06 broadcastingCalcBarycenterGE06, ref PointEarthGeometric coordBaryCenter)
        {
            // Если присутствует BroadcastingAllotment то для определения центра гравитации используем функцию Barycenter с точками BroadcastingAllotment и признаком полигона.
            if (broadcastingCalcBarycenterGE06.BroadcastingAllotment != null)
            {
                if (broadcastingCalcBarycenterGE06.BroadcastingAllotment.AllotmentParameters != null)
                {
                    if (broadcastingCalcBarycenterGE06.BroadcastingAllotment.AllotmentParameters.Сontur != null)
                    {
                        var points = broadcastingCalcBarycenterGE06.BroadcastingAllotment.AllotmentParameters.Сontur;
                        var pointEarthGeometrics = new PointEarthGeometric[points.Length];
                        var administrations = new string[points.Length];
                        for (int i = 0; i < points.Length; i++)
                        {
                            pointEarthGeometrics[i] = new PointEarthGeometric()
                            {
                                Longitude = points[i].Lon_DEC,
                                Latitude = points[i].Lat_DEC,
                                CoordinateUnits = CoordinateUnits.deg
                            };

                            administrations[i] = CalcAdministrationBaryCenter(idwmService, pointEarthGeometrics[i]);

                        }

                        var geometryArgs = new GeometryArgs()
                        {
                            TypeGeometryObject = TypeGeometryObject.Polygon,
                            Points = pointEarthGeometrics
                        };

                        earthGeometricService.CalcBarycenter(in geometryArgs, ref coordBaryCenter);

                        var checkHittingArgs = new CheckHittingArgs()
                        {
                            Poligon = pointEarthGeometrics,
                            Point = coordBaryCenter
                        };

                        var putPointToContourArgs = new PutPointToContourArgs()
                        {
                            Points = pointEarthGeometrics,
                            PointEarthGeometricCalc = coordBaryCenter
                        };

                        var isInsideContour = earthGeometricService.CheckHitting(in checkHittingArgs);
                        if (!isInsideContour)
                        {
                            earthGeometricService.PutPointToContour(in putPointToContourArgs, ref coordBaryCenter);
                        }

                        var administrationBaryCenter = CalcAdministrationBaryCenter(idwmService, coordBaryCenter);
                        for (int i = 0; i < administrations.Length; i++)
                        {
                            if (administrations[i] != administrationBaryCenter)
                            {
                                var nearestPoint = new Point();
                                CalcNearestPointByADM(idwmService, pointEarthGeometrics[i], administrations[i], ref nearestPoint);
                                if ((nearestPoint.Longitude_dec != null) && (nearestPoint.Latitude_dec != null))
                                {
                                    coordBaryCenter.Longitude = nearestPoint.Longitude_dec.Value;
                                    coordBaryCenter.Latitude = nearestPoint.Latitude_dec.Value;
                                    coordBaryCenter.CoordinateUnits = CoordinateUnits.deg;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if ((broadcastingCalcBarycenterGE06.BroadcastingAssignments != null) && (broadcastingCalcBarycenterGE06.BroadcastingAssignments.Length > 0))
            {
                var broadcastingAssignments = broadcastingCalcBarycenterGE06.BroadcastingAssignments;
                var pointEarthGeometrics = new PointEarthGeometric[broadcastingAssignments.Length];
                var administrations = new string[broadcastingAssignments.Length];
                for (int i = 0; i < broadcastingAssignments.Length; i++)
                {
                    pointEarthGeometrics[i] = new PointEarthGeometric()
                    {
                        Longitude = broadcastingAssignments[i].SiteParameters.Lon_Dec,
                        Latitude = broadcastingAssignments[i].SiteParameters.Lat_Dec,
                        CoordinateUnits = CoordinateUnits.deg
                    };

                    administrations[i] = CalcAdministrationBaryCenter(idwmService, pointEarthGeometrics[i]);
                }

                var geometryArgs = new GeometryArgs()
                {
                    TypeGeometryObject = TypeGeometryObject.Points,
                    Points = pointEarthGeometrics
                };

                earthGeometricService.CalcBarycenter(in geometryArgs, ref coordBaryCenter);
                var administrationBaryCenter = CalcAdministrationBaryCenter(idwmService, coordBaryCenter);
                for (int i = 0; i < administrations.Length; i++)
                {
                    if (administrations[i]!= administrationBaryCenter)
                    {
                        var nearestPoint = new Point();
                        CalcNearestPointByADM(idwmService, pointEarthGeometrics[i], administrations[i], ref nearestPoint);
                        if ((nearestPoint.Longitude_dec != null) && (nearestPoint.Latitude_dec != null))
                        {
                            coordBaryCenter.Longitude = nearestPoint.Longitude_dec.Value;
                            coordBaryCenter.Latitude = nearestPoint.Latitude_dec.Value;
                            coordBaryCenter.CoordinateUnits = CoordinateUnits.deg;
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Используем GetADMByPoint для определения администрации в которой находиться барицентр.
        /// </summary>
        /// <param name="idwmService"></param>
        /// <param name="pointEarthGeometric"></param>
        /// <returns></returns>
        public static string CalcAdministrationBaryCenter(IIdwmService idwmService, PointEarthGeometric pointEarthGeometric)
        {
            var point = new Point()
            {
                 Longitude_dec = pointEarthGeometric.Longitude,
                 Latitude_dec = pointEarthGeometric.Latitude
            };
            return  idwmService.GetADMByPoint(in point);
        }

        /// <summary>
        /// Если барицентр не совпадает с ADM BroadcastingAssignment[] (любого из) или BroadcastingAllotment, то тогда используем GetNearestPointByADM для поиска ближайшей точки заданной (это ADM BroadcastingAssignment[] (любого из) или BroadcastingAllotment) администрации
        /// </summary>
        /// <param name="idwmService"></param>
        /// <param name="pointEarthGeometric"></param>
        /// <param name="adm"></param>
        /// <param name="nearestPoint"></param>
        public static void CalcNearestPointByADM(IIdwmService idwmService, PointEarthGeometric pointEarthGeometric, string adm, ref Point nearestPoint)
        {
            var pointByADM = new PointByADM()
            {
                 Administration =  adm,
                  Point = new Point()
                  {
                      Longitude_dec = pointEarthGeometric.Longitude,
                      Latitude_dec = pointEarthGeometric.Latitude
                  }
            };
            idwmService.GetNearestPointByADM(in pointByADM, ref nearestPoint);
        }
    }
}
