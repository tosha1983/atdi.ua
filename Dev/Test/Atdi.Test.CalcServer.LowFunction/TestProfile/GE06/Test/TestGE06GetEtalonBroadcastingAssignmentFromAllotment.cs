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
    public class TestGE06GetEtalonBroadcastingAssignmentFromAllotment
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

                    // нужно заполнить
                    GE.BroadcastingAllotment broadcastingAllotment = new GE.BroadcastingAllotment();
                    broadcastingAllotment.EmissionCharacteristics = new GE.BroadcastingAllotmentEmissionCharacteristics()
                    {
                    };


                    // нужно заполнить
                    GE.BroadcastingAssignment broadcastingAssignment = new GE.BroadcastingAssignment();
                    broadcastingAssignment.EmissionCharacteristics = new GE.BroadcastingAssignmentEmissionCharacteristics()
                    {

                    };
                    gn06Service.GetEtalonBroadcastingAssignmentFromAllotment(broadcastingAllotment, broadcastingAssignment);
                

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
