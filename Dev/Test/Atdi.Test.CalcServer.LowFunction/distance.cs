using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;


namespace Atdi.Test.CalcServer.LowFunction
{
    class TestDistance
    {
        public void Test()
        {
            EarthGeometricService earthGeometricService = new EarthGeometricService();
            double longitude = 30;
            double latitude = 0;
            double distance = 0;
            double azimuth = 0;
            PointEarthGeometric [,] arr1 = new PointEarthGeometric [8, 1];
            PointEarthGeometric[,] arr2 = new PointEarthGeometric[8, 1];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 16; j <= 16; j++)

                {
                    distance = j*10;
                    azimuth = i*45;
                    arr1[i,0] = earthGeometricService.CalculationCoordinateByLengthAndAzimuth(longitude, latitude, distance, azimuth, true);
                    arr2[i, 0] = earthGeometricService.CalculationCoordinateByLengthAndAzimuth(longitude, latitude, distance, azimuth, false);
                }
            }
        }
        public void Test1()
        {
            EarthGeometricService earthGeometricService = new EarthGeometricService();
            PointEarthGeometric point1 = new  PointEarthGeometric()
            {
                Latitude = 50, Longitude = 30,
                CoordinateUnits = CoordinateUnits.deg
            };
            PointEarthGeometric point2 = new PointEarthGeometric()
            {
                Latitude = 50.1,
                Longitude = 30, CoordinateUnits = CoordinateUnits.deg
                
            };
            earthGeometricService.GetDistance_km(point1, point2);
        }
        
        
 
    }
}
