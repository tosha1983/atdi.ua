using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeepServices.GN06
{
    public static class EstimationAssignmentsCalculation
    {
        public const double AntDiscriminationDVBT_dB =6;
        public const double AntDiscriminationTDAB_dB =12;
        public static void Calc(in BroadcastingAllotment broadcastingAllotment, in AreaPoint pointAllotment, in AreaPoint pointCalcFieldStrength, ref PointWithAzimuth[] pointResult, IEarthGeometricService earthGeometricService, out int sizeResultBuffer)
        {
            // определение параметров эталонной сети D - размер соты, d_ минимальное растояние между передатчиками
            GetNetworkParameters(in broadcastingAllotment, out double D_km, out double d_km, out double DistanceToSt_km, out int NumberStation);
            sizeResultBuffer = NumberStation;
            // переупаковачка данных
            PointEarthGeometric pointAllotmentE = new PointEarthGeometric()
            { Latitude = pointAllotment.Lat_DEC, Longitude = pointAllotment.Lon_DEC, CoordinateUnits = CoordinateUnits.deg};
            PointEarthGeometric pointCalcFieldStrengthE = new PointEarthGeometric()
            { Latitude = pointCalcFieldStrength.Lat_DEC, Longitude = pointCalcFieldStrength.Lon_DEC, CoordinateUnits = CoordinateUnits.deg };
            //1 вычисление центра алотмента
            //1a вычисляем длинну между точками
            double k = earthGeometricService.GetDistance_km(in pointAllotmentE, in pointCalcFieldStrengthE);
            //1b вычисляем азимут на центр
            double AzToCentr = earthGeometricService.GetAzimut(in pointCalcFieldStrengthE, in pointAllotmentE);
            //1c вычисляем координаты центра
            var Centr = earthGeometricService.CalculationCoordinateByLengthAndAzimuth(pointCalcFieldStrengthE, k+D_km*0.4330127, AzToCentr);
            //2 Вычисляем координаты точек
            //2a определяем близость точки к элотменту для расчета дискриминации
            bool SmallDistance = true;
            if (k > D_km * 0.4330127) { SmallDistance = false;}
            //2в определяем азимут из центра на точку алотмента 
            double Az = earthGeometricService.GetAzimut(in Centr, in pointAllotmentE);
            //2c все остальное
            GetCoordinat(earthGeometricService, broadcastingAllotment.EmissionCharacteristics.RefNetwork, ref pointResult, Centr, Az, DistanceToSt_km, SmallDistance);

        }
        private static void GetNetworkParameters(in BroadcastingAllotment broadcastingAllotment, out double D, out double d, out double DistFromCentr, out int NumberStation)
        {
            D = 0; d = 0; DistFromCentr = 0; NumberStation = 0;
            if (broadcastingAllotment.EmissionCharacteristics is null)
            {return;}
            switch (broadcastingAllotment.EmissionCharacteristics.RefNetwork)
            {
                case RefNetworkType.RN1:
                    switch (broadcastingAllotment.EmissionCharacteristics.RefNetworkConfig)
                    {
                        case RefNetworkConfigType.RPC1: d = 70; D = 161; break;
                        case RefNetworkConfigType.RPC2: d = 50; D = 115; break;
                        case RefNetworkConfigType.RPC3: d = 40; D = 92; break;
                    }
                    DistFromCentr = d;
                    NumberStation = 7;
                    break;
                case RefNetworkType.RN2:
                case RefNetworkType.RN3:
                    switch (broadcastingAllotment.EmissionCharacteristics.RefNetworkConfig)
                    {
                        case RefNetworkConfigType.RPC1: d = 40; D = 53; break;
                        case RefNetworkConfigType.RPC2: d = 25; D = 33; break;
                        case RefNetworkConfigType.RPC3: d = 25; D = 33; break;
                    }
                    DistFromCentr = d*0.57735027;
                    NumberStation = 3;
                    break;
                case RefNetworkType.RN4:
                    switch (broadcastingAllotment.EmissionCharacteristics.RefNetworkConfig)
                    {
                        case RefNetworkConfigType.RPC1: d = 40; D = 46; break;
                        case RefNetworkConfigType.RPC2: d = 25; D = 29; break;
                        case RefNetworkConfigType.RPC3: d = 25; D = 29; break;
                    }
                    DistFromCentr = D * 0.5;
                    NumberStation = 3;
                    break;
                case RefNetworkType.TDAB:
                    switch (broadcastingAllotment.EmissionCharacteristics.RefNetworkConfig)
                    {
                        case RefNetworkConfigType.RPC4: d = 60; D = 120; break;
                        case RefNetworkConfigType.RPC5: d = 60; D = 120; break;
                    }
                    DistFromCentr = D * 0.5;
                    NumberStation = 7;
                    break;
            }
        }
        private static void GetCoordinat(IEarthGeometricService earthGeometricService, RefNetworkType refNetworkType, ref PointWithAzimuth[] pointResult, PointEarthGeometric Centr, double AzimutFromCentrToAllotmentPoint_deg, double dist, bool SmallDistance)
        {
            switch (refNetworkType)
            {
                case RefNetworkType.RN1:
                case RefNetworkType.TDAB:
                    pointResult[0] = new PointWithAzimuth();
                    pointResult[0].AreaPoint = new AreaPoint() { Lat_DEC = Centr.Latitude, Lon_DEC = Centr.Longitude};
                    for (int i = 1; i <= 6; i++)
                    {
                        double Az = AzimutFromCentrToAllotmentPoint_deg - 30 + 60*i;
                        if (Az >= 360) {Az = Az - 360;}
                        var Point = earthGeometricService.CalculationCoordinateByLengthAndAzimuth(in Centr, dist, Az);
                        pointResult[i] = new PointWithAzimuth();
                        pointResult[i].AreaPoint = new AreaPoint() { Lat_DEC = Point.Latitude, Lon_DEC = Point.Longitude};  
                    }
                    break;
                case RefNetworkType.RN2:
                case RefNetworkType.RN3:
                case RefNetworkType.RN4:
                    for (int i = 0; i <= 2; i++)
                    {
                        double Az = AzimutFromCentrToAllotmentPoint_deg + 30 + 120 * i;
                        if (Az >= 360) { Az = Az - 360; }
                        var Point = earthGeometricService.CalculationCoordinateByLengthAndAzimuth(in Centr, dist, Az);
                        pointResult[i] = new PointWithAzimuth();
                        pointResult[i].AreaPoint = new AreaPoint() { Lat_DEC = Point.Latitude, Lon_DEC = Point.Longitude };
                    }
                    break;
            }
            switch (refNetworkType)
            {
                case RefNetworkType.RN1:
                    for (int i = 0; i <= 6; i++)
                    {
                        pointResult[i].AntDiscrimination_dB = 0; 
                    }
                    break;
                case RefNetworkType.RN2:
                case RefNetworkType.RN3:
                    for (int i = 0; i <= 2; i++)
                    {
                        pointResult[i].AntDiscrimination_dB = 0;
                    }
                    break;
                case RefNetworkType.RN4:
                    pointResult[0].AntDiscrimination_dB = AntDiscriminationDVBT_dB;
                    pointResult[1].AntDiscrimination_dB = 0;
                    if (SmallDistance) { pointResult[2].AntDiscrimination_dB = 0; } else { pointResult[2].AntDiscrimination_dB = AntDiscriminationDVBT_dB; }
                    break;
                case RefNetworkType.TDAB:
                    pointResult[1].AntDiscrimination_dB = AntDiscriminationTDAB_dB;
                    pointResult[6].AntDiscrimination_dB = AntDiscriminationTDAB_dB;
                    pointResult[0].AntDiscrimination_dB = 0;
                    pointResult[3].AntDiscrimination_dB = 0;
                    pointResult[4].AntDiscrimination_dB = 0;
                    if (SmallDistance)
                    { pointResult[2].AntDiscrimination_dB = 0;
                        pointResult[5].AntDiscrimination_dB = 0;}
                    else { pointResult[2].AntDiscrimination_dB = AntDiscriminationTDAB_dB;
                        pointResult[5].AntDiscrimination_dB = AntDiscriminationTDAB_dB;
                    }
                    break;
            }
        }
    }
}
