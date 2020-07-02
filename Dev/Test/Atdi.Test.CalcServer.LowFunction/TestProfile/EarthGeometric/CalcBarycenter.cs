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
    public class CalcBarycenter
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

                    GeometryArgs geometryArgs = new GeometryArgs()
                    {
                        TypeGeometryObject = TypeGeometryObject.Polygon,


                       


                        // капля
                        Points = new PointEarthGeometric[14]
                        {
                             new PointEarthGeometric()
                               {
                                    Longitude = -2.9567128,
                                    Latitude = 52.280058,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = -1.7471485,
                                    Latitude = 52.760043,
                                    CoordinateUnits = CoordinateUnits.deg
                               },

                               new PointEarthGeometric()
                               {
                                    Longitude = -0.88317394,
                                    Latitude = 53.412824,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = -0.3455898,
                                    Latitude = 53.508821,
                                    CoordinateUnits = CoordinateUnits.deg
                               } ,
                               new PointEarthGeometric()
                               {
                                    Longitude = 0.32639037,
                                    Latitude = 53.412824,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                        new PointEarthGeometric()
                        {
                            Longitude = 0.76797734,
                            Latitude = 53.20163,
                            CoordinateUnits = CoordinateUnits.deg
                        },

                               new PointEarthGeometric()
                               {
                                   Longitude = 1.0751683,
                                   Latitude = 52.52965,
                                   CoordinateUnits = CoordinateUnits.deg
                               } ,
                               new PointEarthGeometric()
                               {
                                   Longitude = 1.0559688,
                                   Latitude = 51.85767,
                                   CoordinateUnits = CoordinateUnits.deg
                               } ,
                             new PointEarthGeometric()
                               {
                                   Longitude = 0.55678357,
                                   Latitude = 51.627277,
                                   CoordinateUnits = CoordinateUnits.deg
                               } ,
                             new PointEarthGeometric()
                               {
                                   Longitude = 0.038398867,
                                   Latitude = 51.454482,
                                   CoordinateUnits = CoordinateUnits.deg
                               } ,
                             new PointEarthGeometric()
                               {
                                   Longitude = -0.69117961,
                                   Latitude = 51.665676,
                                   CoordinateUnits = CoordinateUnits.deg
                               } ,
                             new PointEarthGeometric()
                               {
                                   Longitude = -1.5743536,
                                   Latitude = 51.819271,
                                   CoordinateUnits = CoordinateUnits.deg
                               } ,
                             new PointEarthGeometric()
                               {
                                   Longitude = -2.3999292,
                                   Latitude = 52.049664,
                                   CoordinateUnits = CoordinateUnits.deg
                               } ,
                             new PointEarthGeometric()
                               {
                                   Longitude = -2.9183139,
                                   Latitude = 52.318456,
                                   CoordinateUnits = CoordinateUnits.deg
                               } ,

                           }
                    };


     


                    //// полумесяц
                    //Points = new PointEarthGeometric[13]
                    //    {
                    //         new PointEarthGeometric()
                    //           {
                    //                Longitude = 30.160506,
                    //                Latitude = 50.517141,
                    //                CoordinateUnits = CoordinateUnits.deg
                    //           },
                    //           new PointEarthGeometric()
                    //           {
                    //                Longitude = 30.562776,
                    //                Latitude = 50.554107,
                    //                CoordinateUnits = CoordinateUnits.deg
                    //           },

                    //           new PointEarthGeometric()
                    //           {
                    //                Longitude = 30.887499,
                    //                Latitude = 50.375169,
                    //                CoordinateUnits = CoordinateUnits.deg
                    //           },
                    //           new PointEarthGeometric()
                    //           {
                    //                Longitude = 31.076517,
                    //                Latitude = 50.114817,
                    //                CoordinateUnits = CoordinateUnits.deg
                    //           } ,
                    //           new PointEarthGeometric()
                    //           {
                    //                Longitude = 30.965045,
                    //                Latitude = 49.828036,
                    //                CoordinateUnits = CoordinateUnits.deg
                    //           },
                    //    new PointEarthGeometric()
                    //    {
                    //        Longitude = 30.577316,
                    //        Latitude = 49.746679,
                    //        CoordinateUnits = CoordinateUnits.deg
                    //    },

                    //           new PointEarthGeometric()
                    //           {
                    //               Longitude = 30.145967,
                    //               Latitude = 49.759204,
                    //               CoordinateUnits = CoordinateUnits.deg
                    //           } ,
                    //           new PointEarthGeometric()
                    //           {
                    //               Longitude = 30.45615,
                    //               Latitude = 49.812401,
                    //               CoordinateUnits = CoordinateUnits.deg
                    //           } ,
                    //           new PointEarthGeometric()
                    //           {
                    //               Longitude = 30.82934,
                    //               Latitude = 49.890525,
                    //               CoordinateUnits = CoordinateUnits.deg
                    //           } ,
                    //           new PointEarthGeometric()
                    //            {
                    //                Longitude = 30.868113,
                    //                Latitude = 50.033943,
                    //                CoordinateUnits = CoordinateUnits.deg
                    //           }
                    //            ,
                    //           new PointEarthGeometric()
                    //            {
                    //                Longitude = 30.824493,
                    //                Latitude = 50.251369,
                    //                CoordinateUnits = CoordinateUnits.deg
                    //           }
                    //           ,
                    //           new PointEarthGeometric()
                    //            {
                    //                Longitude = 30.567622,
                    //                Latitude = 50.424599,
                    //                CoordinateUnits = CoordinateUnits.deg
                    //           }
                    //           ,
                    //           new PointEarthGeometric()
                    //            {
                    //                Longitude = 30.305905,
                    //                Latitude = 50.495564,
                    //                CoordinateUnits = CoordinateUnits.deg
                    //           }

                    //       }
                    //};


                    PointEarthGeometric pointEarthGeometricCalc = new PointEarthGeometric();
                    earthGeometricServiceServices.CalcBarycenter(in geometryArgs, ref pointEarthGeometricCalc);
                    WPF.Location[] inputCoords = new WPF.Location[geometryArgs.Points.Length];
                    for (int u = 0; u < geometryArgs.Points.Length; u++)
                    {
                        inputCoords[u] = new WPF.Location(geometryArgs.Points[u].Longitude, geometryArgs.Points[u].Latitude);
                    }
                    WPF.RunApp.Start(WPF.TypeObject.Polygon, inputCoords, WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(pointEarthGeometricCalc.Longitude, pointEarthGeometricCalc.Latitude ) });

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
