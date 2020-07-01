using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Workflows;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.AppUnits.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Contracts.Sdrn.DeepServices.IDWM;
using Atdi.AppUnits.Sdrn.DeepServices.IDWM;
using Atdi.DataModels.Sdrn.DeepServices.IDWM;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.AppUnits.Sdrn.DeepServices.GN06;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using GE = Atdi.DataModels.Sdrn.DeepServices.GN06;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using WPF = Atdi.Test.DeepServices.Client.WPF;
using Atdi.Common;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;

namespace Atdi.Test.Platform.SG
{


class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to start test DeepServices Gis ...");
            Console.ReadLine();

			using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    host.Start();





                    //app.Run(app.MainWindow);
                    //app.MainWindow.ShowDialog();

                    //WPF.App.Main();

                    //WPF.MainWindow.MapX.DrawingData = WPF.MapDrawingUpdateData.UpdateData(WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(30, 50) }, WPF.TypeObject.Points , new WPF.Location[] { new WPF.Location(30, 50.2) });



                    host.Container.Register<IDataLayer<EntityDataOrm>>(ServiceLifetime.PerThread);
                    host.Container.Register<ITransformation, TransformationService>(ServiceLifetime.PerThread);
                    host.Container.Register<IEarthGeometricService, EarthGeometricService>(ServiceLifetime.PerThread);
                    var resolver = host.Container.GetResolver<IServicesResolver>();
                    var transformation = resolver.Resolve<ITransformation>();
                    var earthGeometricServiceServices = resolver.Resolve<IEarthGeometricService>();


                    var dataLayer = resolver.Resolve<IDataLayer<EntityDataOrm>>();

                    //var dataLayer = resolver.Resolve<IDataLayer>();
                    //

                    var logger = resolver.Resolve<ILogger>();


                   // Test_(dataLayer);

                    //var Longitude = 36.2527;
                    //var Latitude = 49.9808;

                    //for (int i = 0; i <= 100000; i++)
                    //{
                    //    var code = transformation.ConvertProjectionToAtdiName(32635);
                    //    var code_ = transformation.ConvertProjectionToCode("4UTN35");
                    //    var epsgCoordinate = transformation.ConvertCoordinateToEpgs(new Wgs84Coordinate() { Longitude = Longitude, Latitude = Latitude }, 32635);
                    //    var epsgCoordinate2 = transformation.ConvertCoordinateToEpgs(new EpsgCoordinate() { X = epsgCoordinate.X, Y = epsgCoordinate.Y }, 32635, 4326);

                    //    var EpsgProjectionCoordinate = transformation.ConvertCoordinateToEpgsProjection(new Wgs84Coordinate() { Longitude = Longitude, Latitude = Latitude }, 32635);
                    //    var EpsgProjectionCoordinate2 = transformation.ConvertCoordinateToEpgsProjection(new EpsgProjectionCoordinate() { X = EpsgProjectionCoordinate.X, Y = EpsgProjectionCoordinate.Y, Projection = EpsgProjectionCoordinate.Projection }, 4326);

                    //    var Wgs84Coordinate = transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = EpsgProjectionCoordinate.X, Y = EpsgProjectionCoordinate.Y }, 32635);

                    //    var Wgs84Coordinate2 = transformation.ConvertCoordinateToWgs84(new EpsgProjectionCoordinate() { X = EpsgProjectionCoordinate.X, Y = EpsgProjectionCoordinate.Y, Projection = 32635 });

                    //    Console.WriteLine("X: " + Wgs84Coordinate2.Longitude + " Y: " + Wgs84Coordinate2.Latitude);
                    //    Longitude += 0.000000001;
                    //    Latitude -= 0.000000001;
                    //}



                    var idwmServices = resolver.Resolve<IIdwmService>();


                    var point = new Point() { Longitude_dec = 25.23193315, Latitude_dec = 47.98992167 };
                    var adm = idwmServices.GetADMByPoint(in point);
                    var point2 = new PointByADM() { Point = new Point() { Longitude_dec = 23.97, Latitude_dec = 50.03 }, Administration = "POL" };

                    var resultPoint = new Point();
                    idwmServices.GetNearestPointByADM(in point2, ref resultPoint);

                    //WPF.RunApp.Start(WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(point2.Point.Longitude_dec.Value, point2.Point.Latitude_dec.Value) }, WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(resultPoint.Longitude_dec.Value, resultPoint.Latitude_dec.Value) });

                    //WPF.App app = new WPF.App();
                    //var wnd = new WPF.MainWindow();
                    //wnd.MapX.DrawingData = WPF.MapDrawingUpdateData.UpdateData(WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(point2.Point.Longitude_dec.Value, point2.Point.Latitude_dec.Value) }, WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(resultPoint.Longitude_dec.Value, resultPoint.Latitude_dec.Value) });
                    //wnd.UpdateSource();
                    //app.Run(wnd);
                    //app.Shutdown();

                    var tx = new PointAndDistance()
                    {
                        Distance = 100,
                        Point = new Point()
                        {
                            Longitude_dec = 23.97,
                            Latitude_dec = 50.03
                        }
                    };
                    AdministrationsResult[] administrationsResults = new AdministrationsResult[100];
                    idwmServices.GetADMByPointAndDistance(in tx, ref administrationsResults, out int SizeBuffer);

                    WPF.Location[] zx = new WPF.Location[SizeBuffer];
                    for (int u = 0; u < SizeBuffer; u++)
                    {
                        zx[u] = new WPF.Location(administrationsResults[u].Point.Longitude_dec.Value, administrationsResults[u].Point.Latitude_dec.Value);
                    }

  

                    //WPF.App app = new WPF.App();
                    //var wnd = new WPF.MainWindow();
                    //wnd.MapX.DrawingData = WPF.MapDrawingUpdateData.UpdateData(WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(23, 97) }, WPF.TypeObject.Points, zx );
                    //wnd.UpdateSource();
                    //app.Run(wnd);
                    //app.Shutdown();

                    Console.WriteLine("Adm: " + adm);
 

                  GeometryArgs geometryArgs = new GeometryArgs()
                  {
                       TypeGeometryObject = TypeGeometryObject.Points,

                      Points = new PointEarthGeometric[8]
                      {
                           new PointEarthGeometric()
                           {
                                Longitude = 20,
                                Latitude = 10
                           },
                           new PointEarthGeometric()
                           {
                                Longitude = 16,
                                Latitude = 12
                           },
                           new PointEarthGeometric()
                           {
                                Longitude = 14,
                                Latitude = 15
                           },
                           new PointEarthGeometric()
                           {
                                Longitude = 16,
                                Latitude = 18
                           }
                           ,
                           new PointEarthGeometric()
                           {
                                Longitude = 20,
                                Latitude = 30
                           }
                           ,
                           new PointEarthGeometric()
                           {
                                Longitude = 17,
                                Latitude = 18
                           }
                           ,
                           new PointEarthGeometric()
                           {
                                Longitude = 18,
                                Latitude = 15
                           }
                           ,
                           new PointEarthGeometric()
                           {
                                Longitude = 17,
                                Latitude = 12
                           }
                      }

                      //Points = new  PointEarthGeometric[4]
                      //{
                      //     new PointEarthGeometric()
                      //     {
                      //          Longitude = 20,
                      //          Latitude = 10
                      //     },
                      //     new PointEarthGeometric()
                      //     {
                      //          Longitude = 20,
                      //          Latitude = 30
                      //     },
                      //     new PointEarthGeometric()
                      //     {
                      //          Longitude = 30,
                      //          Latitude = 30
                      //     },
                      //     new PointEarthGeometric()
                      //     {
                      //          Longitude = 30,
                      //          Latitude = 10
                      //     }
                      //}

                  };
                  PointEarthGeometric pointEarthGeometric = new PointEarthGeometric();


       
                  earthGeometricServiceServices.CalcBarycenter(in geometryArgs, ref pointEarthGeometric);
                    WPF.Location[] zx2 = new WPF.Location[geometryArgs.Points.Length];
                    for (int u = 0; u < geometryArgs.Points.Length; u++)
                    {
                        zx2[u] = new WPF.Location(geometryArgs.Points[u].Longitude, geometryArgs.Points[u].Latitude);
                    }


                    //WPF.RunApp.Start(WPF.TypeObject.Polygon, zx2, WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(pointEarthGeometric.Longitude, pointEarthGeometric.Latitude) });

                   
                    PutPointToContourArgs geometryArgs2 = new PutPointToContourArgs()
                    {
                         PointEarthGeometricCalc = new PointEarthGeometric()
                         {
                             Longitude = 32,
                             Latitude = 15,
                             CoordinateUnits = CoordinateUnits.deg
                             
                         },
                        Points = new PointEarthGeometric[4]
                         {
                               new PointEarthGeometric()
                               {
                                    Longitude = 20,
                                    Latitude = 10,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = 20,
                                    Latitude = 30,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                                 
                               new PointEarthGeometric()
                               {
                                    Longitude = 30,
                                    Latitude = 30,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = 30,
                                    Latitude = 10,
                                    CoordinateUnits = CoordinateUnits.deg
                               }
                         }

                    };
                    PointEarthGeometric pointEarthGeometric2 = new PointEarthGeometric();

                   earthGeometricServiceServices.PutPointToContour(in geometryArgs2, ref pointEarthGeometric2);
                    WPF.Location[] zx3 = new WPF.Location[geometryArgs2.Points.Length];
                    for (int u = 0; u < geometryArgs2.Points.Length; u++)
                    {
                        zx3[u] = new WPF.Location(geometryArgs2.Points[u].Longitude, geometryArgs2.Points[u].Latitude);
                    }
                    //zx3[zx3.Length - 1] = new WPF.Location(geometryArgs2.PointEarthGeometricCalc.Longitude, geometryArgs2.PointEarthGeometricCalc.Latitude);

                    //WPF.RunApp.Start(WPF.TypeObject.Polygon, zx3 , WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(pointEarthGeometric2.Longitude, pointEarthGeometric2.Latitude) });


                    
                    ContourForStationByTriggerFieldStrengthsArgs contourForStationByTriggerFieldStrengthsArgs3 = new ContourForStationByTriggerFieldStrengthsArgs()
                    {
                        PointEarthGeometricCalc = new PointEarthGeometric()
                        {
                            Longitude = 30,
                            Latitude = 50
                        },
                        Step_deg = 1,
                        TriggerFieldStrength = 1.56
                    };

                    PointEarthGeometric[] pointEarthGeometric3 = new PointEarthGeometric[3000];

                    earthGeometricServiceServices.CreateContourForStationByTriggerFieldStrengths((sourcePoint, destPoint) => CalcFieldStrength(sourcePoint, destPoint), in contourForStationByTriggerFieldStrengthsArgs3, ref pointEarthGeometric3, out int sizeBuffer);
                    WPF.Location[] zx5 = new WPF.Location[sizeBuffer];
                    for (int u = 0; u < sizeBuffer; u++)
                    {
                        zx5[u] = new WPF.Location(pointEarthGeometric3[u].Longitude, pointEarthGeometric3[u].Latitude);
                    }


                    //WPF.RunApp.Start(WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(contourForStationByTriggerFieldStrengthsArgs3.PointEarthGeometricCalc.Longitude, contourForStationByTriggerFieldStrengthsArgs3.PointEarthGeometricCalc.Latitude) }, WPF.TypeObject.Points, zx5);

                    List<PointEarthGeometric> pointEarthGeometricslst = new List<PointEarthGeometric>();
                    var str = System.IO.File.ReadAllText("C:\\Projects\\AreaTest.txt");
                    string[] a = str.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i=0; i<a.Length;i++)
                    {
                        string[] aa = a[i].Split(new char[] { '\t'}, StringSplitOptions.RemoveEmptyEntries);
                        if ((aa != null) && (aa.Length > 0))
                        {
                            for (int j = 0; j < aa.Length; j++)
                            {
                                

                                pointEarthGeometricslst.Add(new PointEarthGeometric()
                                {
                                    Longitude = Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern.Position.DmsToDec(aa[0].ConvertStringToDouble().Value),
                                    Latitude = Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern.Position.DmsToDec(aa[1].ConvertStringToDouble().Value),
                                    CoordinateUnits = CoordinateUnits.deg

                                });
                                break;
                            }
                        }
                    }
                    
                    var arrPnts = pointEarthGeometricslst.ToArray();

                    //var arrPnts = new PointEarthGeometric[4]
                    //                        {
                    //                          new PointEarthGeometric()
                    //                          {
                    //                             Longitude = 30,
                    //                             Latitude = 50,
                    //                             CoordinateUnits = CoordinateUnits.deg

                    //                          },

                    //                          new PointEarthGeometric()
                    //                          {
                    //                             Longitude = 30,
                    //                             Latitude = 51
                    //                             ,
                    //                             CoordinateUnits = CoordinateUnits.deg
                    //                          },
                    //                          new PointEarthGeometric()
                    //                          {
                    //                             Longitude = 31,
                    //                             Latitude = 51
                    //                             ,
                    //                             CoordinateUnits = CoordinateUnits.deg
                    //                          },

                    //                          new PointEarthGeometric()
                    //                          {
                    //                             Longitude = 31,
                    //                             Latitude = 50
                    //                             ,
                    //                             CoordinateUnits = CoordinateUnits.deg
                    //                          }
                    //                        };
                    PointEarthGeometric pointEarthGeometricR = new PointEarthGeometric();
                                       earthGeometricServiceServices.CalcBarycenter( new GeometryArgs() { Points = arrPnts, TypeGeometryObject = TypeGeometryObject.Points} , ref pointEarthGeometricR);
                                       var arg = new ContourFromContureByDistanceArgs()
                                       {
                                           ContourPoints = arrPnts,
                                           Distance_km = 40,
                                           PointBaryCenter = pointEarthGeometricR,
                                           Step_deg = 0.1
                                       };

                                       PointEarthGeometricWithAzimuth[] pointEarthGeometricPtx = new PointEarthGeometricWithAzimuth[2000000];
                                       earthGeometricServiceServices.CreateContourFromContureByDistance(in arg, ref pointEarthGeometricPtx, out int pointLength);

                    WPF.Location[] zx9 = new WPF.Location[pointLength];
                    for (int u = 0; u < pointLength; u++)
                    {
                        zx9[u] = new WPF.Location(pointEarthGeometricPtx[u].PointEarthGeometric.Longitude, pointEarthGeometricPtx[u].PointEarthGeometric.Latitude);
                    }


                    WPF.Location[] zx11 = new WPF.Location[arrPnts.Length+1];
                    for (int u = 0; u < arrPnts.Length; u++)
                    {
                        zx11[u] = new WPF.Location(arrPnts[u].Longitude, arrPnts[u].Latitude);
                    }
                    zx11[zx11.Length - 1] = new WPF.Location(pointEarthGeometricR.Longitude, pointEarthGeometricR.Latitude);
                    

                    //WPF.RunApp.Start(WPF.TypeObject.Polygon, new WPF.Location[] { new WPF.Location(30,50), new WPF.Location(30, 51), new WPF.Location(31, 51), new WPF.Location(30.6, 50.6), new WPF.Location(30.6, 50.4), new WPF.Location(31, 50), new WPF.Location(pointEarthGeometricR.Longitude, pointEarthGeometricR.Latitude) }, WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(0, 0) } /*zx9*/);
                    WPF.RunApp.Start(WPF.TypeObject.Polygon, zx11, WPF.TypeObject.Polygon,  zx9);











                    ContourFromPointByDistanceArgs contourContourFromPointByDistanceArgs4 = new ContourFromPointByDistanceArgs()
                                       {
                                           PointEarthGeometricCalc = new PointEarthGeometric()
                                           {
                                               Longitude = 30,
                                               Latitude = 50
                                           },
                                           Step_deg = 15,
                                           Distance_km = 20.0
                                       };

                                       PointEarthGeometric[] pointEarthGeometric4 = new PointEarthGeometric[3000];

                                       earthGeometricServiceServices.CreateContourFromPointByDistance(in contourContourFromPointByDistanceArgs4, ref pointEarthGeometric4, out int sizeBuffer4);

                    WPF.Location[] zx4 = new WPF.Location[sizeBuffer4];
                    for (int u = 0; u < sizeBuffer4; u++)
                    {
                        zx4[u] = new WPF.Location(pointEarthGeometric4[u].Longitude, pointEarthGeometric4[u].Latitude);
                    }
                    //WPF.RunApp.Start(WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(contourContourFromPointByDistanceArgs4.PointEarthGeometricCalc.Longitude, contourContourFromPointByDistanceArgs4.PointEarthGeometricCalc.Latitude) }, WPF.TypeObject.Points,  zx4);




                    var source = new PointEarthGeometric()
                    {
                        Longitude = 30,
                        Latitude = 50,
                        CoordinateUnits = CoordinateUnits.deg
                    };
                    var target = new PointEarthGeometric()
                    {
                        Longitude = 30,
                        Latitude = 50.1,
                        CoordinateUnits = CoordinateUnits.deg
                    };

                    var d = earthGeometricServiceServices.GetDistance_km(in source, in target);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.WriteLine($"Press any key to stop test DeepServices Gis ...");
                Console.ReadLine();
             
            }

            Console.WriteLine($"Server host was stopped. Press any key to exit ...");
            Console.ReadLine();
        }

        private static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public static double getVal(double x, double all)
        {
            return x / all;
        }

        public static double CalcFieldStrength(PointEarthGeometric pointEarthGeometric1, PointEarthGeometric pointEarthGeometric2)
        {
            var dLat = Deg2Rad(pointEarthGeometric2.Latitude - pointEarthGeometric1.Latitude);
            var dLon = Deg2Rad(pointEarthGeometric2.Longitude - pointEarthGeometric1.Longitude);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(Deg2Rad(pointEarthGeometric1.Latitude)) * Math.Cos(Deg2Rad(pointEarthGeometric2.Latitude)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = 6371 * c *1000;


            if (d <= 50)
            {
                return getVal(d, 50);
            }
            else if ((d >= 50) && (d <= 1000))
            {
                return 1+ getVal(d, 1000);
            }
            else if ((d >= 1000) && (d <= 5000))
            {
                return 2+ getVal(d, 5000);
            }
            else if ((d >= 5000) && (d <= 10000))
            {
                return 3+ getVal(d, 10000);
            }
            else if ((d >= 10000) && (d <= 20000))
            {
                return 4+ getVal(d, 20000);
            }
            else if ((d >= 20000) && (d <= 50000))
            {
                return 5+ getVal(d, 50000);
            }
            else if ((d >= 50000) && (d <= 100000))
            {
                return 6+ getVal(d, 100000);
            }
            else if ((d >= 100000) && (d <= 500000))
            {
                return 7+ getVal(d, 500000);
            }
            else
            {
                return 9;
            }
        }

    }
}
