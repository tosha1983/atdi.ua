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


namespace Atdi.Test.Platform.SG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to start test DeepServices Gis ...");
            Console.ReadLine();

			using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    host.Start();

                    host.Container.Register<ITransformation, TransformationService>(ServiceLifetime.PerThread);
                    var resolver = host.Container.GetResolver<IServicesResolver>();
                    var transformation = resolver.Resolve<ITransformation>();
                    var logger = resolver.Resolve<ILogger>();

                    for (int i = 0; i <= 100000; i++)
                    {
                        var code = transformation.ConvertProjectionToAtdiName(32635);
                        var code_ = transformation.ConvertProjectionToCode("4UTN35");
                        var epsgCoordinate = transformation.ConvertCoordinateToEpgs(new Wgs84Coordinate() { Longitude = 36.2527, Latitude = 49.9808 }, 32635);
                        var epsgCoordinate2 = transformation.ConvertCoordinateToEpgs(new EpsgCoordinate() { X = epsgCoordinate.X, Y = epsgCoordinate.Y }, 32635, 4326);

                        var EpsgProjectionCoordinate = transformation.ConvertCoordinateToEpgsProjection(new Wgs84Coordinate() { Longitude = 36.2527, Latitude = 49.9808 }, 32635);
                        var EpsgProjectionCoordinate2 = transformation.ConvertCoordinateToEpgsProjection(new EpsgProjectionCoordinate() { X = EpsgProjectionCoordinate.X, Y = EpsgProjectionCoordinate.Y, Projection = EpsgProjectionCoordinate.Projection }, 4326);

                        var Wgs84Coordinate = transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = EpsgProjectionCoordinate.X, Y = EpsgProjectionCoordinate.Y }, 32635);

                        var Wgs84Coordinate2 = transformation.ConvertCoordinateToWgs84(new EpsgProjectionCoordinate() { X = EpsgProjectionCoordinate.X, Y = EpsgProjectionCoordinate.Y, Projection = 32635 });
                    }

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
    }
}
