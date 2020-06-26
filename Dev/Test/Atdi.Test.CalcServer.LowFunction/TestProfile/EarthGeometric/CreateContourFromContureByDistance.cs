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
    public class CreateContourFromContureByDistance
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


                    var arrPnts = new PointEarthGeometric[9]
                        {
                                              new PointEarthGeometric()
                                              {
                                                 Longitude = 30,
                                                 Latitude = 50,
                                                 CoordinateUnits = CoordinateUnits.deg

                                              },
                                              new PointEarthGeometric()
                                              {
                                                 Longitude = 30,
                                                 Latitude = 50.001,
                                                 CoordinateUnits = CoordinateUnits.deg

                                              },
                                              new PointEarthGeometric()
                                              {
                                                 Longitude = 30,
                                                 Latitude = 50.003,
                                                 CoordinateUnits = CoordinateUnits.deg

                                              },
                                              new PointEarthGeometric()
                                              {
                                                 Longitude = 30,
                                                 Latitude = 51
                                                 ,
                                                 CoordinateUnits = CoordinateUnits.deg
                                              },
                                              new PointEarthGeometric()
                                              {
                                                 Longitude = 31,
                                                 Latitude = 51
                                                 ,
                                                 CoordinateUnits = CoordinateUnits.deg
                                              },

                                              new PointEarthGeometric()
                                              {
                                                 Longitude = 30.6,
                                                 Latitude = 50.6
                                                 ,
                                                 CoordinateUnits = CoordinateUnits.deg
                                              },

                                              new PointEarthGeometric()
                                              {
                                                 Longitude = 30.6,
                                                 Latitude = 50.4
                                                 ,
                                                 CoordinateUnits = CoordinateUnits.deg
                                              },

                                              new PointEarthGeometric()
                                              {
                                                 Longitude = 30.7,
                                                 Latitude = 50.1
                                                 ,
                                                 CoordinateUnits = CoordinateUnits.deg
                                              },

                                              new PointEarthGeometric()
                                              {
                                                 Longitude = 31,
                                                 Latitude = 50
                                                 ,
                                                 CoordinateUnits = CoordinateUnits.deg
                                              }
                        };
                    PointEarthGeometric pointEarthGeometricR = new PointEarthGeometric();
                    earthGeometricServiceServices.CalcBarycenter(new GeometryArgs() { Points = arrPnts, TypeGeometryObject = TypeGeometryObject.Points }, ref pointEarthGeometricR);
                    var arg = new ContourFromContureByDistanceArgs()
                    {
                        ContourPoints = arrPnts,
                        Distance_km = 10,
                        PointBaryCenter = pointEarthGeometricR,
                        Step_deg = 3
                    };

                    PointEarthGeometricWithAzimuth[] pointEarthGeometricPtx = new PointEarthGeometricWithAzimuth[10000];
                    earthGeometricServiceServices.CreateContourFromContureByDistance(in arg, ref pointEarthGeometricPtx, out int pointLength);

                    WPF.Location[] outPnts = new WPF.Location[pointLength];
                    for (int u = 0; u < pointLength; u++)
                    {
                        outPnts[u] = new WPF.Location(pointEarthGeometricPtx[u].PointEarthGeometric.Longitude, pointEarthGeometricPtx[u].PointEarthGeometric.Latitude);
                    }


                    WPF.Location[] inputPnts = new WPF.Location[arrPnts.Length];
                    for (int u = 0; u < arrPnts.Length; u++)
                    {
                        inputPnts[u] = new WPF.Location(arrPnts[u].Longitude, arrPnts[u].Latitude);
                    }

                    WPF.RunApp.Start(WPF.TypeObject.Polygon, inputPnts, WPF.TypeObject.Points, outPnts);






                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.WriteLine($"Press any key to stop test DeepServices IEarthGeometricService ...");
                Console.ReadLine();
            }
        }

       
        /// метод расчета напряженности по двум точкам 
        public static double CalcFieldStrength(PointEarthGeometric pointEarthGeometric1, PointEarthGeometric pointEarthGeometric2)
        {
            return -1;
        }
    }
}
