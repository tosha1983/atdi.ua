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
    public static class InterseptionOnSphere
    {
        public static void Test()
        {
            using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    // Start input DATA 
                    PointEarthGeometric StartPoint = new PointEarthGeometric()
                    {
                        Latitude = 30, Longitude = 350, CoordinateUnits = CoordinateUnits.deg
                    };
                    PointEarthGeometric Point1 = new PointEarthGeometric()
                    {
                        Latitude = 30, Longitude = 350, CoordinateUnits = CoordinateUnits.deg
                    };
                    PointEarthGeometric Point2 = new PointEarthGeometric()
                    {
                        Latitude = 30, Longitude = 10,CoordinateUnits = CoordinateUnits.deg
                    };
                    int AzimutStart = 0;
                    int AzimutStop = 360;
                    //END input DATA 
                    WPF.Location[] outCoords = new WPF.Location[AzimutStop- AzimutStart+1];
                    
                    for (int i = AzimutStart; i <= AzimutStop; i++)
                    {
                        double Azimut = i;
                        bool inter = GeometricСalculations.GetInterseptionOnSphere(StartPoint, Azimut, Point1, Point2, out PointEarthGeometric point);
                        outCoords[i] = new WPF.Location(point.Longitude, point.Latitude);
                    }
                    
                    
                    WPF.Location[] inputCoords = new WPF.Location[3];
                    inputCoords[0] = new WPF.Location(StartPoint.Longitude, StartPoint.Latitude);
                    inputCoords[1] = new WPF.Location(Point1.Longitude, Point1.Latitude);
                    inputCoords[2] = new WPF.Location(Point2.Longitude, Point2.Latitude);



                    WPF.RunApp.Start(WPF.TypeObject.Points, inputCoords, WPF.TypeObject.Points, outCoords);

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
