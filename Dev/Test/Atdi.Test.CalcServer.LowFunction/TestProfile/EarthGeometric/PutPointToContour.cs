using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
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
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Atdi.AppUnits.Sdrn.DeepServices.GN06;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using GE = Atdi.DataModels.Sdrn.DeepServices.GN06;
using WPF = Atdi.Test.DeepServices.Client.WPF;
using System;

namespace Atdi.Test.CalcServer.LowFunction
{
    public class PutPointToContour
    {
        public void Test()
        {
            using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    host.Start();
                    host.Container.Register<IIdwmService, IdwmService>(ServiceLifetime.PerThread);
                    host.Container.Register<ITransformation, TransformationService>(ServiceLifetime.PerThread);
                    host.Container.Register<IEarthGeometricService, EarthGeometricService>(ServiceLifetime.PerThread);
                    var resolver = host.Container.GetResolver<IServicesResolver>();
                    var earthGeometricServiceServices = resolver.Resolve<IEarthGeometricService>();


                    //inputPnts[inputPnts.Length - 2] = new WPF.Location(30.54, 51.0899);
                    //inputPnts[inputPnts.Length - 1] = new WPF.Location(31, 51.0908);





                    PutPointToContourArgs geometryArgs = new PutPointToContourArgs()
                    {
                        // полумесяц
                        Points = new PointEarthGeometric[18]
                        {
                            new PointEarthGeometric()
                            {
                                Longitude = -2.0543394,
                                Latitude = 54.641588,
                                CoordinateUnits = CoordinateUnits.deg
                            },
                            new PointEarthGeometric()
                                   {
                                        Longitude = -0.55678357,
                                        Latitude = 54.430394,
                                        CoordinateUnits = CoordinateUnits.deg
                                   },

                                   new PointEarthGeometric()
                                   {
                                        Longitude = 0.38398867,
                                        Latitude = 53.835212,
                                        CoordinateUnits = CoordinateUnits.deg
                                   },
                                   new PointEarthGeometric()
                                   {
                                        Longitude = 1.1327666,
                                        Latitude = 52.971237,
                                        CoordinateUnits = CoordinateUnits.deg
                                   } ,
                                   new PointEarthGeometric()
                                   {
                                        Longitude = 1.5167552,
                                        Latitude = 52.107263,
                                        CoordinateUnits = CoordinateUnits.deg
                                   },
                                new PointEarthGeometric()
                                {
                                    Longitude = 1.4591569,
                                    Latitude = 51.320086,
                                    CoordinateUnits = CoordinateUnits.deg
                                },

                                   new PointEarthGeometric()
                                   {
                                       Longitude = 1.3439603,
                                       Latitude = 50.379314,
                                       CoordinateUnits = CoordinateUnits.deg
                                   } ,
                                   new PointEarthGeometric()
                                   {
                                       Longitude = 0.74877791,
                                       Latitude = 49.899328,
                                       CoordinateUnits = CoordinateUnits.deg
                                   } ,
                                   new PointEarthGeometric()
                                   {
                                       Longitude = -0.15359547,
                                       Latitude = 49.688134,
                                       CoordinateUnits = CoordinateUnits.deg
                                   } ,
                                   new PointEarthGeometric()
                                    {
                                        Longitude = -1.1135671,
                                        Latitude = 49.688134,
                                        CoordinateUnits = CoordinateUnits.deg
                                   }
                                    ,
                                   new PointEarthGeometric()
                                    {
                                        Longitude = -2.0735388,
                                        Latitude = 49.764932,
                                        CoordinateUnits = CoordinateUnits.deg
                                   }
                                   ,
                                   new PointEarthGeometric()
                                    {
                                        Longitude = -0.61438187,
                                        Latitude = 50.379314,
                                        CoordinateUnits = CoordinateUnits.deg
                                   }
                                   ,
                                   new PointEarthGeometric()
                                    {
                                        Longitude = 0.26879207,
                                        Latitude = 50.8401,
                                        CoordinateUnits = CoordinateUnits.deg
                                   }
                                                                      ,
                                   new PointEarthGeometric()
                                    {
                                        Longitude = 0.076797734,
                                        Latitude = 51.723274,
                                        CoordinateUnits = CoordinateUnits.deg
                                   }
                                                                      ,
                                   new PointEarthGeometric()
                                    {
                                        Longitude = -0.15359547,
                                        Latitude = 52.452852,
                                        CoordinateUnits = CoordinateUnits.deg
                                   }
                                                                      ,
                                   new PointEarthGeometric()
                                    {
                                        Longitude = -0.61438187,
                                        Latitude = 53.489622,
                                        CoordinateUnits = CoordinateUnits.deg
                                   }
                                    ,
                                   new PointEarthGeometric()
                                    {
                                        Longitude = -1.4591569,
                                        Latitude = 53.950408,
                                        CoordinateUnits = CoordinateUnits.deg
                                   }
                                    ,
                                   new PointEarthGeometric()
                                    {
                                        Longitude = -1.9583422,
                                        Latitude = 54.679987,
                                        CoordinateUnits = CoordinateUnits.deg
                                   }
                               }
                    };



                    //    Points = new PointEarthGeometric[5]
                    //      {
                    //        new PointEarthGeometric()
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

                    //                           new PointEarthGeometric()
                    //                          {
                    //                             Longitude = 30.7,
                    //                             Latitude = 50.5
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

                    //      }

                    //};
                    PointEarthGeometric newBaryCenter = new PointEarthGeometric();
                    PointEarthGeometric BaryCenter = new PointEarthGeometric();
                    GeometryArgs geometryArgsBC = new GeometryArgs()
                    {
                        Points = geometryArgs.Points,
                        TypeGeometryObject = TypeGeometryObject.Polygon
                    };
                    earthGeometricServiceServices.CalcBarycenter(geometryArgsBC, ref BaryCenter);
                    geometryArgs.PointEarthGeometricCalc = BaryCenter;
                    earthGeometricServiceServices.PutPointToContour(in geometryArgs, ref newBaryCenter);
                    WPF.Location[] inputCoords = new WPF.Location[geometryArgs.Points.Length];
                    for (int u = 0; u < geometryArgs.Points.Length; u++)
                    {
                        inputCoords[u] = new WPF.Location(geometryArgs.Points[u].Longitude, geometryArgs.Points[u].Latitude);
                    }
                    WPF.Location[] outputCoords = new WPF.Location[2] 
                    {
                         new WPF.Location(BaryCenter.Longitude, BaryCenter.Latitude),
                         new WPF.Location(newBaryCenter.Longitude, newBaryCenter.Latitude)
                    };
                    WPF.RunApp.Start(WPF.TypeObject.Polygon, inputCoords, WPF.TypeObject.Points, outputCoords);


                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.WriteLine($"Press any key to stop test DeepServices IEarthGeometricService ...");
                Console.ReadLine();
            }
        }
    }
}
