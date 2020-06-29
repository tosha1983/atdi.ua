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
                        TypeGeometryObject = TypeGeometryObject.Points,

                        //Points = new PointEarthGeometric[4]
                        //{
                        //     new PointEarthGeometric()
                        //       {
                        //            Longitude = 20,
                        //            Latitude = 10,
                        //            CoordinateUnits = CoordinateUnits.deg
                        //       },
                        //       new PointEarthGeometric()
                        //       {
                        //            Longitude = 20,
                        //            Latitude = 30,
                        //            CoordinateUnits = CoordinateUnits.deg
                        //       },

                        //       new PointEarthGeometric()
                        //       {
                        //            Longitude = 30,
                        //            Latitude = 30,
                        //            CoordinateUnits = CoordinateUnits.deg
                        //       },
                        //       new PointEarthGeometric()
                        //       {
                        //            Longitude = 30,
                        //            Latitude = 10,
                        //            CoordinateUnits = CoordinateUnits.deg
                        //       }
                        //}


                        // капля
                        Points = new PointEarthGeometric[8]
                        {
                             new PointEarthGeometric()
                               {
                                    Longitude = 30.795413,
                                    Latitude = 50.560265,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = 30.897192,
                                    Latitude = 50.535627,
                                    CoordinateUnits = CoordinateUnits.deg
                               },

                               new PointEarthGeometric()
                               {
                                    Longitude = 30.955352,
                                    Latitude = 50.489397,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = 30.935965,
                                    Latitude = 50.436948,
                                    CoordinateUnits = CoordinateUnits.deg
                               } ,
                               new PointEarthGeometric()
                               {
                                    Longitude = 30.892346,
                                    Latitude = 50.390621,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                        new PointEarthGeometric()
                        {
                            Longitude = 30.8148,
                            Latitude = 50.347342,
                            CoordinateUnits = CoordinateUnits.deg
                        },

                               new PointEarthGeometric()
                               {
                                   Longitude = 30.703328,
                                   Latitude = 50.35662,
                                   CoordinateUnits = CoordinateUnits.deg
                               } ,
                               new PointEarthGeometric()
                               {
                                   Longitude = 29.641918,
                                   Latitude = 50.58489,
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
