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
using Atdi.Contracts.Sdrn.DeepServices.IDWM;
using Atdi.AppUnits.Sdrn.DeepServices.IDWM;
using Atdi.DataModels.Sdrn.DeepServices.IDWM;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.ServiceModel;


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



                    host.Container.Register<IIdwmService, IdwmService>(ServiceLifetime.PerThread);
                    host.Container.Register<ITransformation, TransformationService>(ServiceLifetime.PerThread);
                    host.Container.Register<IEarthGeometricService, EarthGeometricService>(ServiceLifetime.PerThread);
                    var resolver = host.Container.GetResolver<IServicesResolver>();
                    var transformation = resolver.Resolve<ITransformation>();
                    var logger = resolver.Resolve<ILogger>();

                    //var Longitude = 36.2527;
                    //var Latitude = 49.9808;

                    //for (int i = 0; i <= 100000; i++)
                    //{
                    //    var code = transformation.ConvertProjectionToAtdiName(32635);
                    //    var code_ = transformation.ConvertProjectionToCode("4UTN35");
                    //    var epsgCoordinate = transformation.ConvertCoordinateToEpgs(new Wgs84Coordinate() { Longitude = Longitude, Latitude = Latitude }, 32635);
                    //    var epsgCoordinate2 = transformation.ConvertCoordinateToEpgs(new EpsgCoordinate() { X = epsgCoordinate.X, Y = epsgCoordinate.Y }, 32635, 4326);

                    //    var EpsgProjectionCoordinate = transformation.ConvertCoordinateToEpgsProjection(new Wgs84Coordinate() { Longitude = Longitude, Latitude = Latitude }, 32635);
                    //    var EpsgProjectionCoordinate2 = transformation.ConvertCoordinateToEpgsProjection(new EpsgProjectionCoordinate() { X = EpsgProjectionCoordinate.X, Y = EpsgProjectionCoordinate.Y, Projection = EpsgProjectionCoordinate.Projection }, 4326);

                    //    var Wgs84Coordinate = transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = EpsgProjectionCoordinate.X, Y = EpsgProjectionCoordinate.Y }, 32635);

                    //    var Wgs84Coordinate2 = transformation.ConvertCoordinateToWgs84(new EpsgProjectionCoordinate() { X = EpsgProjectionCoordinate.X, Y = EpsgProjectionCoordinate.Y, Projection = 32635 });

                    //    Console.WriteLine("X: " + Wgs84Coordinate2.Longitude + " Y: " + Wgs84Coordinate2.Latitude);
                    //    Longitude += 0.000000001;
                    //    Latitude -= 0.000000001;
                    //}


                    
                    var idwmServices = resolver.Resolve<IIdwmService>();
                    var point = new Point() { Longitude = 25.23193315, Latitude = 47.98992167 };
                    var adm = idwmServices.GetADMByPoint(in point);
                    var point2 = new PointByADM () { Longitude = 23.97, Latitude = 50.03, Administration = "POL" };

                    var resultPoint = new Point();
                    idwmServices.GetNearestPointByADM(in point2, ref resultPoint);

                    var tx= new PointAndDistance()
                    {
                        Distance = 100,
                        Longitude = 23.97,
                        Latitude = 50.03
                    };
                    AdministrationsResult[] administrationsResults = new AdministrationsResult[100];
                    idwmServices.GetADMByPointAndDistance(in tx, ref administrationsResults, out int SizeBuffer);
                    Console.WriteLine("Adm: " + adm);


                    GeometryArgs geometryArgs = new GeometryArgs()
                    {
                         TypeGeometryObject = TypeGeometryObject.Points,

                        Points = new PointEarthGeometric[8]
                        {
                             new PointEarthGeometric()
                             {
                                  Longitude = 20,
                                  Latitude = 10
                             },
                             new PointEarthGeometric()
                             {
                                  Longitude = 16,
                                  Latitude = 12
                             },
                             new PointEarthGeometric()
                             {
                                  Longitude = 14,
                                  Latitude = 15
                             },
                             new PointEarthGeometric()
                             {
                                  Longitude = 16,
                                  Latitude = 18
                             }
                             ,
                             new PointEarthGeometric()
                             {
                                  Longitude = 20,
                                  Latitude = 30
                             }
                             ,
                             new PointEarthGeometric()
                             {
                                  Longitude = 17,
                                  Latitude = 18
                             }
                             ,
                             new PointEarthGeometric()
                             {
                                  Longitude = 18,
                                  Latitude = 15
                             }
                             ,
                             new PointEarthGeometric()
                             {
                                  Longitude = 17,
                                  Latitude = 12
                             }
                        }

                        //Points = new  PointEarthGeometric[4]
                        //{
                        //     new PointEarthGeometric()
                        //     {
                        //          Longitude = 20,
                        //          Latitude = 10
                        //     },
                        //     new PointEarthGeometric()
                        //     {
                        //          Longitude = 20,
                        //          Latitude = 30
                        //     },
                        //     new PointEarthGeometric()
                        //     {
                        //          Longitude = 30,
                        //          Latitude = 30
                        //     },
                        //     new PointEarthGeometric()
                        //     {
                        //          Longitude = 30,
                        //          Latitude = 10
                        //     }
                        //}

                    };
                    PointEarthGeometric pointEarthGeometric = new PointEarthGeometric();

                 
                    var earthGeometricServiceServices = resolver.Resolve<IEarthGeometricService>();
                    earthGeometricServiceServices.CalcBarycenter(in geometryArgs, ref pointEarthGeometric);


                    PutPointToContourArgs geometryArgs2 = new PutPointToContourArgs()
                    {
                         PointEarthGeometricCalc = new PointEarthGeometric()
                         {
                             Longitude = 22,
                             Latitude = 25
                         },
                        Points = new PointEarthGeometric[4]
                         {
                               new PointEarthGeometric()
                               {
                                    Longitude = 20,
                                    Latitude = 10
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = 20,
                                    Latitude = 30
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = 30,
                                    Latitude = 30
                               },
                               new PointEarthGeometric()
                               {
                                    Longitude = 30,
                                    Latitude = 10
                               }
                         }

                    };
                    PointEarthGeometric pointEarthGeometric2 = new PointEarthGeometric();

                   earthGeometricServiceServices.PutPointToContour(in geometryArgs2, ref pointEarthGeometric2);




                    ContourForStationByTriggerFieldStrengthsArgs contourForStationByTriggerFieldStrengthsArgs3 = new ContourForStationByTriggerFieldStrengthsArgs()
                    {
                         PointEarthGeometricCalc = new PointEarthGeometric()
                         {
                              Longitude = 30,
                               Latitude = 50
                         },
                          Step_deg = 1,
                          TriggerFieldStrength = 0.56
                    };

                    PointEarthGeometric[] pointEarthGeometric3 = new PointEarthGeometric[3];

                    earthGeometricServiceServices.CreateContourForStationByTriggerFieldStrengths(in contourForStationByTriggerFieldStrengthsArgs3, ref pointEarthGeometric3, out int sizeBuffer);


                    ContourFromPointByDistanceArgs contourContourFromPointByDistanceArgs4 = new ContourFromPointByDistanceArgs()
                    {
                        PointEarthGeometricCalc = new PointEarthGeometric()
                        {
                            Longitude = 30,
                            Latitude = 50
                        },
                        Step_deg = 0.5,
                        Distance_m = 2000.0
                    };

                    PointEarthGeometric[] pointEarthGeometric4 = new PointEarthGeometric[3000];

                    earthGeometricServiceServices.CreateContourFromPointByDistance(in contourContourFromPointByDistanceArgs4, ref pointEarthGeometric4, out int sizeBuffer4);
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
