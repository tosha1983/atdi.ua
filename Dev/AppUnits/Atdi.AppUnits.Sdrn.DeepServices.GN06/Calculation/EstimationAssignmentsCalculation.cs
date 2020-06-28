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


        public static void Calc(in EstimationAssignmentsPointsArgs estimationAssignmentsPointsArgs, ref PointWithAzimuthResult pointWithAzimuthResult, IEarthGeometricService earthGeometricService)
        {
            // определение параметров эталонной сети D - размер соты, d_ минимальное растояние между передатчиками
            var broadcastingAllotment = estimationAssignmentsPointsArgs.BroadcastingAllotment;
            GetNetworkParameters(in broadcastingAllotment, out double D_km, out double d_km, out double distanceToSt_km, out int numberStation);
            pointWithAzimuthResult.sizeResultBuffer = numberStation;
            // переупаковачка данных
            PointEarthGeometric pointAllotmentE = new PointEarthGeometric()
            {
                Latitude = estimationAssignmentsPointsArgs.PointAllotment.Lat_DEC, Longitude = estimationAssignmentsPointsArgs.PointAllotment.Lon_DEC, CoordinateUnits = CoordinateUnits.deg
            };
            PointEarthGeometric pointCalcFieldStrengthE = new PointEarthGeometric()
            {
                Latitude = estimationAssignmentsPointsArgs.PointCalcFieldStrength.Lat_DEC, Longitude = estimationAssignmentsPointsArgs.PointCalcFieldStrength.Lon_DEC, CoordinateUnits = CoordinateUnits.deg
            };
            //1 вычисление центра алотмента
            //1a вычисляем длинну между точками
            double k = earthGeometricService.GetDistance_km(in pointAllotmentE, in pointCalcFieldStrengthE);
            //1b вычисляем азимут на центр
            double azToCentr = earthGeometricService.GetAzimut(in pointCalcFieldStrengthE, in pointAllotmentE);
            //1c вычисляем координаты центра
            var Centr = earthGeometricService.CalculationCoordinateByLengthAndAzimuth(pointCalcFieldStrengthE, k+D_km*0.4330127, azToCentr);
            //2 Вычисляем координаты точек
            //2a определяем близость точки к элотменту для расчета дискриминации
            bool smallDistance = true;
            if (k > D_km * 0.4330127) { smallDistance = false;}
            //2в определяем азимут из центра на точку алотмента 
            double az = earthGeometricService.GetAzimut(in Centr, in pointAllotmentE);
            //2c все остальное
            GetCoordinat(earthGeometricService, broadcastingAllotment.EmissionCharacteristics.RefNetwork, ref pointWithAzimuthResult, Centr, az, distanceToSt_km, smallDistance);

        }
        private static void GetNetworkParameters(in BroadcastingAllotment broadcastingAllotment, out double D, out double d, out double distFromCentr, out int numberStation)
        {
            D = 0; d = 0; distFromCentr = 0; numberStation = 0;
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
                    distFromCentr = d;
                    numberStation = 7;
                    break;
                case RefNetworkType.RN2:
                case RefNetworkType.RN3:
                    switch (broadcastingAllotment.EmissionCharacteristics.RefNetworkConfig)
                    {
                        case RefNetworkConfigType.RPC1: d = 40; D = 53; break;
                        case RefNetworkConfigType.RPC2: d = 25; D = 33; break;
                        case RefNetworkConfigType.RPC3: d = 25; D = 33; break;
                    }
                    distFromCentr = d*0.57735027;
                    numberStation = 3;
                    break;
                case RefNetworkType.RN4:
                    switch (broadcastingAllotment.EmissionCharacteristics.RefNetworkConfig)
                    {
                        case RefNetworkConfigType.RPC1: d = 40; D = 46; break;
                        case RefNetworkConfigType.RPC2: d = 25; D = 29; break;
                        case RefNetworkConfigType.RPC3: d = 25; D = 29; break;
                    }
                    distFromCentr = D * 0.5;
                    numberStation = 3;
                    break;
                case RefNetworkType.RN5:
                case RefNetworkType.RN6:
                    switch (broadcastingAllotment.EmissionCharacteristics.RefNetworkConfig)
                    {
                        case RefNetworkConfigType.RPC4: d = 60; D = 120; break;
                        case RefNetworkConfigType.RPC5: d = 60; D = 120; break;
                    }
                    distFromCentr = D * 0.5;
                    numberStation = 7;
                    break;
            }
        }
        private static void GetCoordinat(IEarthGeometricService earthGeometricService, RefNetworkType refNetworkType, ref PointWithAzimuthResult pointResult, PointEarthGeometric centr, double azimutFromCentrToAllotmentPoint_deg, double dist, bool smallDistance)
        {
            switch (refNetworkType)
            {
                case RefNetworkType.RN1:
                case RefNetworkType.RN5:
                case RefNetworkType.RN6:
                    pointResult.PointWithAzimuth[0] = new PointWithAzimuth();
                    pointResult.PointWithAzimuth[0].AreaPoint = new AreaPoint() { Lat_DEC = centr.Latitude, Lon_DEC = centr.Longitude};
                    for (int i = 1; i <= 6; i++)
                    {
                        double Az = azimutFromCentrToAllotmentPoint_deg - 30 + 60*i;
                        if (Az >= 360) {Az = Az - 360;}
                        var Point = earthGeometricService.CalculationCoordinateByLengthAndAzimuth(in centr, dist, Az);
                        pointResult.PointWithAzimuth[i] = new PointWithAzimuth();
                        pointResult.PointWithAzimuth[i].AreaPoint = new AreaPoint() { Lat_DEC = Point.Latitude, Lon_DEC = Point.Longitude};  
                    }
                    break;
                case RefNetworkType.RN2:
                case RefNetworkType.RN3:
                case RefNetworkType.RN4:
                    for (int i = 0; i <= 2; i++)
                    {
                        double Az = azimutFromCentrToAllotmentPoint_deg + 30 + 120 * i;
                        if (Az >= 360) { Az = Az - 360; }
                        var Point = earthGeometricService.CalculationCoordinateByLengthAndAzimuth(in centr, dist, Az);
                        pointResult.PointWithAzimuth[i] = new PointWithAzimuth();
                        pointResult.PointWithAzimuth[i].AreaPoint = new AreaPoint() { Lat_DEC = Point.Latitude, Lon_DEC = Point.Longitude };
                    }
                    break;
            }
            switch (refNetworkType)
            {
                case RefNetworkType.RN1:
                    for (int i = 0; i <= 6; i++)
                    {
                        pointResult.PointWithAzimuth[i].AntDiscrimination_dB = 0; 
                    }
                    break;
                case RefNetworkType.RN2:
                case RefNetworkType.RN3:
                    for (int i = 0; i <= 2; i++)
                    {
                        pointResult.PointWithAzimuth[i].AntDiscrimination_dB = 0;
                    }
                    break;
                case RefNetworkType.RN4:
                    pointResult.PointWithAzimuth[0].AntDiscrimination_dB = AntDiscriminationDVBT_dB;
                    pointResult.PointWithAzimuth[1].AntDiscrimination_dB = 0;
                    if (smallDistance) { pointResult.PointWithAzimuth[2].AntDiscrimination_dB = 0; } else { pointResult.PointWithAzimuth[2].AntDiscrimination_dB = AntDiscriminationDVBT_dB; }
                    break;
                case RefNetworkType.RN5:
                case RefNetworkType.RN6:
                    pointResult.PointWithAzimuth[1].AntDiscrimination_dB = AntDiscriminationTDAB_dB;
                    pointResult.PointWithAzimuth[6].AntDiscrimination_dB = AntDiscriminationTDAB_dB;
                    pointResult.PointWithAzimuth[0].AntDiscrimination_dB = 0;
                    pointResult.PointWithAzimuth[3].AntDiscrimination_dB = 0;
                    pointResult.PointWithAzimuth[4].AntDiscrimination_dB = 0;
                    if (smallDistance)
                    {
                        pointResult.PointWithAzimuth[2].AntDiscrimination_dB = 0;
                        pointResult.PointWithAzimuth[5].AntDiscrimination_dB = 0;}
                    else {
                        pointResult.PointWithAzimuth[2].AntDiscrimination_dB = AntDiscriminationTDAB_dB;
                        pointResult.PointWithAzimuth[5].AntDiscrimination_dB = AntDiscriminationTDAB_dB;
                    }
                    break;
            }
        }
    }
}
