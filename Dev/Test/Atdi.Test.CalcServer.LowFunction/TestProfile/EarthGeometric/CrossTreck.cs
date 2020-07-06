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
    public static class CrossTreckOnSphere
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
                        Latitude = 0.10, Longitude = 0, CoordinateUnits = CoordinateUnits.deg
                    };
                    PointEarthGeometric StopPoint = new PointEarthGeometric()
                    {
                        Latitude = 0, Longitude = -10, CoordinateUnits = CoordinateUnits.deg
                    };
                    PointEarthGeometric Point = new PointEarthGeometric()
                    {
                        Latitude = 29, Longitude = -10,CoordinateUnits = CoordinateUnits.deg
                    };
                    int LatStart = -20;
                    int LatStop = 20;
                    //END input DATA 
                    WPF.Location[] outCoords = new WPF.Location[LatStop- LatStart+1];
                    double[,] arr = new double[LatStop - LatStart + 1, 2];
                    for (int i = LatStart; i <= LatStop; i++)
                    {
                        double Lat = i;
                        Point.Latitude = Lat;
                        double dist = GeometricСalculations.GetCrossTrackDistanceOnSphere_km(StartPoint, StopPoint, Point);
                        outCoords[i-LatStart] = new WPF.Location(Point.Longitude, Point.Latitude);
                        arr[i- LatStart, 0] = Lat;
                        arr[i- LatStart, 1] = dist;
                    }
                    


                    WPF.Location[] inputCoords = new WPF.Location[2];
                    inputCoords[0] = new WPF.Location(StartPoint.Longitude, StartPoint.Latitude);
                    inputCoords[1] = new WPF.Location(StopPoint.Longitude, StopPoint.Latitude);
                    



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
