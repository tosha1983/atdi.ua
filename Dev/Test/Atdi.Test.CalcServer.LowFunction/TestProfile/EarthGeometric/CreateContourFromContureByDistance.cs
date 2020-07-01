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
using System.Collections.Generic;
using Atdi.Common;

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


                    string fileName = System.IO.Path.Combine(Environment.CurrentDirectory, "AreaTest.txt");

                    List<PointEarthGeometric> pointEarthGeometricslst = new List<PointEarthGeometric>();
                    var str = System.IO.File.ReadAllText(fileName);
                    string[] a = str.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < a.Length; i++)
                    {
                        string[] aa = a[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if ((aa != null) && (aa.Length > 0))
                        {
                            for (int j = 0; j < aa.Length; j++)
                            {
                                pointEarthGeometricslst.Add(new PointEarthGeometric()
                                {
                                    Longitude = Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern.Position.DmsToDec(aa[0].ConvertStringToDouble().Value),
                                    Latitude = Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern.Position.DmsToDec(aa[1].ConvertStringToDouble().Value),
                                    CoordinateUnits = CoordinateUnits.deg
                                });
                                break;
                            }
                        }
                    }

                    var arrPnts = pointEarthGeometricslst.ToArray();

                    PointEarthGeometric pointEarthGeometricR = new PointEarthGeometric();
                    earthGeometricServiceServices.CalcBarycenter(new GeometryArgs() { Points = arrPnts, TypeGeometryObject = TypeGeometryObject.Points }, ref pointEarthGeometricR);
                    var arg = new ContourFromContureByDistanceArgs()
                    {
                        ContourPoints = arrPnts,
                        Distance_km = 5,
                        PointBaryCenter = pointEarthGeometricR,
                        Step_deg = 1
                    };

                    PointEarthGeometric[] pointEarthGeometricPtx = new PointEarthGeometric[10000];
                    earthGeometricServiceServices.CreateContourFromContureByDistance(in arg, ref pointEarthGeometricPtx, out int pointLength);

                    WPF.Location[] outPnts = new WPF.Location[pointLength];
                    for (int u = 0; u < pointLength; u++)
                    {
                        outPnts[u] = new WPF.Location(pointEarthGeometricPtx[u].Longitude, pointEarthGeometricPtx[u].Latitude);
                    }


                    WPF.Location[] inputPnts = new WPF.Location[arrPnts.Length];
                    for (int u = 0; u < arrPnts.Length; u++)
                    {
                        inputPnts[u] = new WPF.Location(arrPnts[u].Longitude, arrPnts[u].Latitude);
                    }

                    //inputPnts[inputPnts.Length - 2] = new WPF.Location(30.54, 51.0899);
                    //inputPnts[inputPnts.Length - 1] = new WPF.Location(31, 51.0908);


                    WPF.RunApp.Start(WPF.TypeObject.Polygon, inputPnts, WPF.TypeObject.Polygon, outPnts);






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
