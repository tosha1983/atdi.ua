using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.SpecializedCalculation.TDOA
{
    public class GeographicLocalization
    {
        /// <summary>
        /// Calc Y (km) coordinate for line TDOA localization by x (km)
        /// </summary>
        /// <param name="l">distance between sensors, km</param>
        /// <param name="d">delta distance of signal propagation, km</param>
        /// <param name="x"> coordinate from sensor 1 to sensore 2, km</param>
        /// <returns></returns>
        private static double GetYformX(double l, double d, double x)
        { // Не тестированно 
            double a1 = (2 * l * x - l * l - d * d) * (2 * l * x - l * l - d * d) / (4 * d * d) - (l - x) * (l - x);
            if (a1 < 0) { return 99999999999999;}
            return Math.Sqrt(a1);
        }
        public static double[] GetLineLonLat(double LonMinArea, double LonMaxArea, double LatMinArea, double LatMaxArea, double LonSt1, double LatSt1, double LonSt2,  double LatSt2, double DeltaDistSt1MinusSt2_km)
        {
            //Const
            int MaxPointInLine = 100;

            // проверка корректности входных данных
            if ((LonMinArea > 180) || (LonMinArea < -180) ||
                (LonMaxArea > 180) || (LonMaxArea < -180) ||
                (LonSt1 > 180) || (LonSt1 < -180)||
                (LonSt2 > 180) || (LonSt2 < -180) ||
                (LatMinArea > 89) || (LatMinArea < -89) ||
                (LatMaxArea > 89) || (LatMaxArea < -89) ||
                (LatSt1 > 89) || (LatSt1 < -89) ||
                (LatSt2 > 89) || (LatSt2 < -89) ||
                (DeltaDistSt1MinusSt2_km > 100) || (DeltaDistSt1MinusSt2_km < -100)) { return null;}
            if (LonMinArea > LonMaxArea) { double a1 = LonMaxArea; LonMaxArea = LonMinArea; LonMinArea = a1;}
            if (LatMinArea > LatMaxArea) { double a1 = LatMaxArea; LatMaxArea = LatMinArea; LatMinArea = a1; }
            double dLat = 111.316 * (LatSt2 - LatSt1);
            double dLon = 111.316 * (LonSt2 - LonSt1)*Math.Cos(Math.PI*(LatSt1 + LatSt2)/360.0);
            double cosLat = Math.Cos(Math.PI * (LatSt1 + LatSt2) / 360.0);
            double l = Math.Sqrt(dLat* dLat+ dLon* dLon);
            if ((Math.Abs(DeltaDistSt1MinusSt2_km) > l) || (l>200)) { return null; }
            double dArLat = 111.316 * (LatMaxArea - LatMinArea);
            double dArLon = 111.316 * Math.Abs(LonMaxArea - LonMinArea) * Math.Cos(Math.PI * (LatMaxArea + LatMaxArea) / 360.0);
            // конец проверки входных данных
            double lAr = Math.Sqrt(dArLat * dArLat + dArLon * dArLon);
            double step_km = lAr/ MaxPointInLine;
            double stopd = (l-Math.Abs(DeltaDistSt1MinusSt2_km)) / (2);
            
            // создаем масив с шагами X
            double[] XArr = new double[MaxPointInLine*2];
            if ((DeltaDistSt1MinusSt2_km < 0.01)&&(DeltaDistSt1MinusSt2_km > -0.01))
            {
                for (int i = 0; i < MaxPointInLine*2; i++)
                {// пишем сюда Y
                    XArr[i] = -lAr + step_km * i;
                }
            }
            else
            {
                if (DeltaDistSt1MinusSt2_km > 0)
                {
                    for (int i = 0; i < MaxPointInLine + 1; i++)
                    {// концы 
                        XArr[i] = -lAr + step_km * i;
                    }
                    for (int i = MaxPointInLine + 1; i < 2 * MaxPointInLine - 1; i++)
                    {// внутри
                        XArr[i] = XArr[i - 1] + (stopd - XArr[i - 1]) * 0.2;
                    }
                    XArr[2 * MaxPointInLine - 1] = stopd;
                }
                else
                {
                    for (int i = 0; i < MaxPointInLine + 1; i++)
                    {// концы 
                        XArr[i] = l + lAr - step_km * i;
                    }
                    for (int i = MaxPointInLine + 1; i < 2 * MaxPointInLine - 1; i++)
                    {// внутри
                        XArr[i] = XArr[i - 1] - (XArr[i - 1] - (l - stopd)) * 0.2;
                    }
                    XArr[2 * MaxPointInLine - 1] = l - stopd;
                }
            }
            // идем по массиву X и формируем координаты
            double sinAl = dLat/l;
            double cosAl = dLon/l;
            List<double> ListResultArr = new List<double>();
            if ((DeltaDistSt1MinusSt2_km < 0.01) && (DeltaDistSt1MinusSt2_km > -0.01))
            {
                for (int i = 0; i < XArr.Length; i++)
                {
                    double x = l/2;
                    double y = XArr[i];
                    double dLon_ = x * cosAl - y * sinAl;
                    double dLat_ = x * sinAl + y * cosAl;
                    double Lon = dLon_ / (111.316 * cosLat) + LonSt1;
                    double Lat = dLat_ / (111.316) + LatSt1;
                    if ((Lon > LonMinArea) && (Lon < LonMaxArea) && (Lat > LatMinArea) && (Lat < LatMaxArea))
                    { ListResultArr.Add(Lon); ListResultArr.Add(Lat); }
                }
            }
            else
            {
                for (int i = 0; i < XArr.Length; i++)
                {
                    double x = XArr[i];
                    double y = GetYformX(l, DeltaDistSt1MinusSt2_km, x);
                    double dLon_ = x * cosAl - y * sinAl;
                    double dLat_ = x * sinAl + y * cosAl;
                    double Lon = dLon_ / (111.316 * cosLat) + LonSt1;
                    double Lat = dLat_ / (111.316) + LatSt1;
                    if ((Lon > LonMinArea) && (Lon < LonMaxArea) && (Lat > LatMinArea) && (Lat < LatMaxArea))
                    { ListResultArr.Add(Lon); ListResultArr.Add(Lat); }
                }
                for (int i = XArr.Length - 1; i >= 0; i--)
                {
                    double x = XArr[i];
                    double y = -GetYformX(l, DeltaDistSt1MinusSt2_km, x);
                    double dLon_ = x * cosAl - y * sinAl;
                    double dLat_ = x * sinAl + y * cosAl;
                    double Lon = dLon_ / (111.316 * cosLat) + LonSt1;
                    double Lat = dLat_ / (111.316) + LatSt1;
                    if ((Lon > LonMinArea) && (Lon < LonMaxArea) && (Lat > LatMinArea) && (Lat < LatMaxArea))
                    { ListResultArr.Add(Lon); ListResultArr.Add(Lat); }
                }
            }
            return ListResultArr.ToArray();
        }
    }
}
