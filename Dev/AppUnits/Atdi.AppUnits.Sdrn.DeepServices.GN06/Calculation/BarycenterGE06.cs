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
            string[] administrationsBroadcastingAssignments = null;
            string administrationAllotment = null;
            // Если присутствует BroadcastingAllotment то для определения центра гравитации используем функцию Barycenter с точками BroadcastingAllotment и признаком полигона.
            if (broadcastingCalcBarycenterGE06.BroadcastingAllotment != null)
            {
                if (broadcastingCalcBarycenterGE06.BroadcastingAllotment.AllotmentParameters != null)
                {
                    if (broadcastingCalcBarycenterGE06.BroadcastingAllotment.AllotmentParameters.Contur != null)
                    {
                        var pointsAllotmentParametersСontur = broadcastingCalcBarycenterGE06.BroadcastingAllotment.AllotmentParameters.Contur;
                        var pointEarthGeometricsAllotmentParametersСontur = new PointEarthGeometric[pointsAllotmentParametersСontur.Length];
                        for (int i = 0; i < pointsAllotmentParametersСontur.Length; i++)
                        {
                            pointEarthGeometricsAllotmentParametersСontur[i] = new PointEarthGeometric()
                            {
                                Longitude = pointsAllotmentParametersСontur[i].Lon_DEC,
                                Latitude = pointsAllotmentParametersСontur[i].Lat_DEC,
                                CoordinateUnits = CoordinateUnits.deg
                            };
                        }

                        
                        administrationAllotment = broadcastingCalcBarycenterGE06.BroadcastingAllotment.AdminData.Adm;
                        

                        var geometryArgs = new GeometryArgs()
                        {
                            TypeGeometryObject = TypeGeometryObject.Polygon,
                            Points = pointEarthGeometricsAllotmentParametersСontur
                        };

                        earthGeometricService.CalcBarycenter(in geometryArgs, ref coordBaryCenter);

                        var checkHittingArgs = new CheckHittingArgs()
                        {
                            Poligon = pointEarthGeometricsAllotmentParametersСontur,
                            Point = coordBaryCenter
                        };

                        var isInsideContour = earthGeometricService.CheckHitting(in checkHittingArgs);
                        if (!isInsideContour)
                        {
                            var putPointToContourArgs = new PutPointToContourArgs()
                            {
                                Points = pointEarthGeometricsAllotmentParametersСontur,
                                PointEarthGeometricCalc = coordBaryCenter
                            };

                            earthGeometricService.PutPointToContour(in putPointToContourArgs, ref coordBaryCenter);
                        }
                    }
                }
            }
            else if ((broadcastingCalcBarycenterGE06.BroadcastingAssignments != null) && (broadcastingCalcBarycenterGE06.BroadcastingAssignments.Length > 0))
            {
                var broadcastingAssignments = broadcastingCalcBarycenterGE06.BroadcastingAssignments;
                var pointEarthGeometricsBroadcastingAssignments = new PointEarthGeometric[broadcastingAssignments.Length];
                administrationsBroadcastingAssignments = new string[broadcastingAssignments.Length];
                for (int i = 0; i < broadcastingAssignments.Length; i++)
                {
                    pointEarthGeometricsBroadcastingAssignments[i] = new PointEarthGeometric()
                    {
                        Longitude = broadcastingAssignments[i].SiteParameters.Lon_Dec,
                        Latitude = broadcastingAssignments[i].SiteParameters.Lat_Dec,
                        CoordinateUnits = CoordinateUnits.deg
                    };
                   
                    
                    administrationsBroadcastingAssignments[i] = broadcastingAssignments[i].AdmData.Adm;
                    
                }

                var geometryArgs = new GeometryArgs()
                {
                    TypeGeometryObject = TypeGeometryObject.Points,
                    Points = pointEarthGeometricsBroadcastingAssignments
                };

                earthGeometricService.CalcBarycenter(in geometryArgs, ref coordBaryCenter);
            }
            var administrationBaryCenter = CalcAdministrationBaryCenter(idwmService, coordBaryCenter);

            if (administrationAllotment != null)
            {
                if (administrationAllotment != administrationBaryCenter)
                {
                    var nearestPoint = new Point();
                    CalcNearestPointByADM(idwmService, coordBaryCenter, administrationAllotment, ref nearestPoint);
                    coordBaryCenter.Longitude = nearestPoint.Longitude_dec.Value;
                    coordBaryCenter.Latitude = nearestPoint.Latitude_dec.Value;
                }
            }
            if (administrationsBroadcastingAssignments != null)
            {
                for (int i = 0; i < administrationsBroadcastingAssignments.Length; i++)
                {
                    if (administrationsBroadcastingAssignments[i] != administrationBaryCenter)
                    {
                        var nearestPoint = new Point();
                        CalcNearestPointByADM(idwmService, coordBaryCenter, administrationsBroadcastingAssignments[i], ref nearestPoint);
                        coordBaryCenter.Longitude = nearestPoint.Longitude_dec.Value;
                        coordBaryCenter.Latitude = nearestPoint.Latitude_dec.Value;
                        break;
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
