﻿using Atdi.Platform;
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

                    PutPointToContourArgs geometryArgs = new PutPointToContourArgs()
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
                                    Longitude = 25,
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
                                    Longitude = 35,
                                    Latitude = 30,
                                    CoordinateUnits = CoordinateUnits.deg
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = 25,
                                    Latitude = 10,
                                    CoordinateUnits = CoordinateUnits.deg
                               }
                          }

                    };
                    PointEarthGeometric pointEarthGeometricCalc = new PointEarthGeometric();

                    earthGeometricServiceServices.PutPointToContour(in geometryArgs, ref pointEarthGeometricCalc);
                    WPF.Location[] inputCoords = new WPF.Location[geometryArgs.Points.Length + 1];
                    for (int u = 0; u < geometryArgs.Points.Length; u++)
                    {
                        inputCoords[u] = new WPF.Location(geometryArgs.Points[u].Longitude, geometryArgs.Points[u].Latitude);
                    }
                    inputCoords[inputCoords.Length - 1] = new WPF.Location(geometryArgs.PointEarthGeometricCalc.Longitude, geometryArgs.PointEarthGeometricCalc.Latitude);

                    WPF.RunApp.Start(WPF.TypeObject.Points, inputCoords, WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(pointEarthGeometricCalc.Longitude, pointEarthGeometricCalc.Latitude) });


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
