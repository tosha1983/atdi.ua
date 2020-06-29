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
    public class TestGE06GetBoundaryPointsFromAllotments
    {
        public static void Test()
        {

            using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    host.Start();
                    host.Container.Register<IIdwmService, IdwmService>(ServiceLifetime.PerThread);
                    host.Container.Register<ITransformation, TransformationService>(ServiceLifetime.PerThread);
                    host.Container.Register<IEarthGeometricService, EarthGeometricService>(ServiceLifetime.PerThread);
                    host.Container.Register<IGn06Service, EstimationAssignmentsService>(ServiceLifetime.PerThread);
                    var resolver = host.Container.GetResolver<IServicesResolver>();

                    var gn06Service = resolver.Resolve<IGn06Service>();


                    BroadcastingAllotmentWithStep broadcastingAllotmentWithStep = new BroadcastingAllotmentWithStep();
                    broadcastingAllotmentWithStep.BroadcastingAllotment = new GE.BroadcastingAllotment()
                    {
                       AllotmentParameters = new GE.AllotmentParameters()
                       {
                           ContourId = 1,
                           Name = "Name",
                           Сontur = new GE.AreaPoint[4]
                             {
                                  new GE.AreaPoint()
                                  {
                                       Lon_DEC = 20,
                                       Lat_DEC = 10
                                  },
                                  new GE.AreaPoint()
                                  {
                                       Lon_DEC = 20,
                                       Lat_DEC = 30
                                  },
                                  new GE.AreaPoint()
                                  {
                                       Lon_DEC = 30,
                                       Lat_DEC = 30
                                  },
                                  new GE.AreaPoint()
                                  {
                                       Lon_DEC = 30,
                                       Lat_DEC = 10
                                  }
                             }
                       }
                    };
                    broadcastingAllotmentWithStep.step_km = 5;

                    Points points = new Points();
                    points.PointEarthGeometrics = new PointEarthGeometric[10000];

                    gn06Service.GetBoundaryPointsFromAllotments(in broadcastingAllotmentWithStep, ref points);
                    // на карту 
                    WPF.Location[] InputData = new WPF.Location[4] {
                        new WPF.Location(broadcastingAllotmentWithStep.BroadcastingAllotment.AllotmentParameters.Сontur[0].Lon_DEC, broadcastingAllotmentWithStep.BroadcastingAllotment.AllotmentParameters.Сontur[0].Lat_DEC),
                        new WPF.Location(broadcastingAllotmentWithStep.BroadcastingAllotment.AllotmentParameters.Сontur[1].Lon_DEC, broadcastingAllotmentWithStep.BroadcastingAllotment.AllotmentParameters.Сontur[1].Lat_DEC),
                        new WPF.Location(broadcastingAllotmentWithStep.BroadcastingAllotment.AllotmentParameters.Сontur[2].Lon_DEC, broadcastingAllotmentWithStep.BroadcastingAllotment.AllotmentParameters.Сontur[2].Lat_DEC),
                        new WPF.Location(broadcastingAllotmentWithStep.BroadcastingAllotment.AllotmentParameters.Сontur[3].Lon_DEC, broadcastingAllotmentWithStep.BroadcastingAllotment.AllotmentParameters.Сontur[3].Lat_DEC)
                    };
                    WPF.Location[] OutputData = new WPF.Location[points.SizeResultBuffer];
                    for (int j = 0; points.SizeResultBuffer > j; j++)
                    { OutputData[j] = new WPF.Location(points.PointEarthGeometrics[j].Longitude, points.PointEarthGeometrics[j].Latitude); }
                    WPF.RunApp.Start(WPF.TypeObject.Points, InputData, WPF.TypeObject.Points, OutputData);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.WriteLine($"Press any key to stop test DeepServices GE06 ...");
                Console.ReadLine();
            }
        }
    }
}
