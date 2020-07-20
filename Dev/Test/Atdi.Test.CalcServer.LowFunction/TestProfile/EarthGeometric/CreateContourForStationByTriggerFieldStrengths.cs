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
    public class CreateContourForStationByTriggerFieldStrengths
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

                    ContourForStationByTriggerFieldStrengthsArgs contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                    {
                        BaryCenter = new PointEarthGeometric()
                        {
                            Longitude = 30,
                            Latitude = 50
                        },
                        Step_deg = 1,
                        TriggerFieldStrength = 6.56
                    };

                    PointEarthGeometric[] pointEarthGeometric3 = new PointEarthGeometric[3000];

                    earthGeometricServiceServices.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrength(destinationPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometric3, out int sizeBuffer);
                    WPF.Location[] OutCoords = new WPF.Location[sizeBuffer];
                    for (int u = 0; u < sizeBuffer; u++)
                    {
                        OutCoords[u] = new WPF.Location(pointEarthGeometric3[u].Longitude, pointEarthGeometric3[u].Latitude);
                    }


                    WPF.RunApp.Start(WPF.TypeObject.Points, new WPF.Location[] { new WPF.Location(contourForStationByTriggerFieldStrengthsArgs.BaryCenter.Longitude, contourForStationByTriggerFieldStrengthsArgs.BaryCenter.Latitude) }, WPF.TypeObject.Points, OutCoords);

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
        public static double CalcFieldStrength(PointEarthGeometric destPoint)
        {
            return -1;
        }
    }
}
