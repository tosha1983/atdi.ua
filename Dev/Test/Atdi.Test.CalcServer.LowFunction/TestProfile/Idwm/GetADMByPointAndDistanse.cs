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
    public class GetADMByPointAndDistanse
    {
        public void Test()
        {
            using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    host.Start();
                    host.Container.Register<IIdwmService, IdwmService>(ServiceLifetime.PerThread);
                    host.Container.Register<IEarthGeometricService, EarthGeometricService>(ServiceLifetime.PerThread);
                    var resolver = host.Container.GetResolver<IServicesResolver>();

                    var idwmServices = resolver.Resolve<IIdwmService>();
                    var pointAndDistance = new PointAndDistance()
                    {
                        Distance = 100,
                        Point = new Point()
                        {
                            Longitude_dec = 23.97,
                            Latitude_dec = 50.03
                        }
                    };
                    AdministrationsResult[] administrationsResults = new AdministrationsResult[100];
                    idwmServices.GetADMByPointAndDistance(in pointAndDistance, ref administrationsResults, out int SizeBuffer);

                    WPF.Location[] outCoords = new WPF.Location[SizeBuffer];
                    for (int u = 0; u < SizeBuffer; u++)
                    {
                        outCoords[u] = new WPF.Location(administrationsResults[u].Point.Longitude_dec.Value, administrationsResults[u].Point.Latitude_dec.Value);
                    }


                    WPF.RunApp.Start(WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(23.97, 50.03) }, WPF.TypeObject.Points, outCoords);


                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }

                Console.WriteLine($"Press any key to stop test DeepServices IIdwmService ...");
                Console.ReadLine();
            }
        }
    }
}
