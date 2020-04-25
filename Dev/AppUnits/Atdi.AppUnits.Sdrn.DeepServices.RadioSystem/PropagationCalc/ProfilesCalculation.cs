using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
    public static class ProfilesCalculation
    {
        private static double CalcElevAngleArgs(double h1, double h2, double d, double rE)
        {
            return ((h2 - h1)/d - 500 * d / rE) / 1000;
        }

        public static void CalcTilts(double re_km, double ha_m, double hb_m, double d_km, in short[] profile_m, int profileStartPosition, int profilePointsNumber, out double tiltA_deg, out double tiltB_deg)
        {
            int profileEndPosition = profileStartPosition + profilePointsNumber - 1;
            double dN = d_km / (profilePointsNumber - profileStartPosition);
            double h1_m = ha_m + profile_m[profileStartPosition];
            double h2_m = hb_m + profile_m[profileEndPosition];
            
            double maxAngleArgAB = CalcElevAngleArgs(h1_m, h2_m, d_km, re_km);
            double maxAngleArgBA = CalcElevAngleArgs(h2_m, h1_m, d_km, re_km);


            for (int n = profileStartPosition; n < profileEndPosition; n++)
            {
                double dAN = dN * (n - profileStartPosition);
                double dNB = dN * (profileEndPosition - n);
                
                double nAngleArgAB = CalcElevAngleArgs(h1_m, profile_m[n + profileStartPosition], dAN, re_km);
                double nAngleArgBA = CalcElevAngleArgs(h2_m, profile_m[profileEndPosition - n], dNB, re_km);

                if (maxAngleArgAB < nAngleArgAB)
                {
                    maxAngleArgAB = nAngleArgAB;
                }
                if (maxAngleArgBA < nAngleArgBA)
                {
                    maxAngleArgBA = nAngleArgBA;
                }
            }

            
            tiltA_deg = (Math.Atan(maxAngleArgAB)) * 180 / Math.PI;
            tiltB_deg = (Math.Atan(maxAngleArgBA)) * 180 / Math.PI;
        }


        public static void CalcTilts(double re_km, double h1_m, double h2_m, double d_km, out double tiltA_deg, out double tiltB_deg)
        {            
            tiltA_deg = (Math.Atan(CalcElevAngleArgs(h1_m, h2_m, d_km, re_km))) * 180 / Math.PI;
            tiltB_deg = (Math.Atan(CalcElevAngleArgs(h2_m, h1_m, d_km, re_km))) * 180 / Math.PI;
        }
        /// <summary>
        /// Функция производит определение препятсвий на трассе и для каждого препятсвия запускает расчет FunctionCalc. Препятсвия определяються от первой точки и до последней
        /// </summary>
        /// <param name="FunctionCalc">сама функция расчета ослабления</param>
        /// <param name="args">профили</param>
        /// <param name="tilta_deg"></param>
        /// <param name="tiltb_deg"></param>
        /// <returns></returns>
        public static double CalcLossOfObstacles(Func<double, EstimationClutterObstaclesResult, double> FunctionCalc, in CalcLossArgs args, double tilta_deg, double tiltb_deg)
        {
            
            double LossObs = 0;

            bool obstacleIntersection = false;
            bool isEndPoint = false;

            double pixelLength_km = args.D_km / (args.ProfileLength - args.ReliefStartIndex);

            int clutterHeightMin;
            int clutterHeightMax;
            
            double beamAHeight;
            double beamBHeight;
            double beamHeight;
            double beamPrevHeight;

            double alpha;
            double theta_deg = 0;
            double degToRad = Math.PI / 180.0;
            double distanceTo_km;
            double distanceInside_km = 0;

            double invRe = 1 / args.Model.Parameters.EarthRadius_km;
            double invPi = 1 / Math.PI;

            int profileEndIndex = args.ReliefStartIndex + args.ProfileLength - 1;

            var ha_m = args.Ha_m + args.ReliefProfile[args.ReliefStartIndex];
            var hb_m = args.Hb_m + args.ReliefProfile[profileEndIndex];

            for (int i = args.ReliefStartIndex + 1; i <= profileEndIndex; i++)
            {

                clutterHeightMin = args.ReliefProfile[i] + args.BuildingProfile[i];
                clutterHeightMax = args.ReliefProfile[i] + args.BuildingProfile[i] + args.ClutterProfile[i];
                
                beamAHeight = pixelLength_km * (i - args.ReliefStartIndex) * (1000 * Math.Tan(tilta_deg * degToRad) + 500 * pixelLength_km * (i - args.ReliefStartIndex) * invRe) + ha_m;
                beamBHeight = pixelLength_km * (profileEndIndex - i) * (1000 * Math.Tan(tilta_deg * degToRad) + 500 * pixelLength_km * (profileEndIndex - i) * invRe) + hb_m;

                // определение луча, который пересекает препятствие
                if (beamAHeight <= beamBHeight)
                {
                    beamHeight = beamAHeight;
                    beamPrevHeight = pixelLength_km * (i - args.ReliefStartIndex - 1) * (1000 * Math.Tan(tilta_deg) + 0.5 * invRe) + ha_m;
                    alpha = tilta_deg;
                    distanceTo_km = pixelLength_km * (i - args.ReliefStartIndex);
                }
                else
                {
                    beamHeight = beamBHeight;
                    beamPrevHeight = pixelLength_km * (profileEndIndex - i + 1) * (1000 * Math.Tan(tilta_deg) + 0.5 * invRe) + hb_m;
                    alpha = tiltb_deg;
                    distanceTo_km = pixelLength_km * (profileEndIndex - i);
                }

                // если в данной точке луч пересекает клаттер
                if (beamHeight >= clutterHeightMin && beamHeight <= clutterHeightMax)
                {
                    // если в предыдущей точке луч не пересекает клаттер, но элемент слоя клаттеров есть - тогда считаем, что луч падает сверху 
                    if (beamPrevHeight >= clutterHeightMin && beamPrevHeight <= clutterHeightMax && args.ClutterProfile[i - 1] != 0 && obstacleIntersection == false)
                    {
                        theta_deg = alpha - 180 * distanceTo_km * invPi * invRe;
                    }
                    else if (obstacleIntersection == false)
                    {
                        theta_deg = 90 - alpha + 180 * distanceTo_km * invPi * invRe;
                    }

                    if ((i == args.ReliefStartIndex + 1) || (i == profileEndIndex))
                    {
                        isEndPoint = true;
                    }
                    else if ((i > args.ReliefStartIndex || i < args.ReliefStartIndex + args.ProfileLength - 1) && (isEndPoint == true))
                    {
                        isEndPoint = false;
                    }

                    // начало определения длительности прохождения луча в клаттере
                    obstacleIntersection = true;
                    distanceInside_km += pixelLength_km;
                }
                else if ((beamHeight < clutterHeightMin || beamHeight > clutterHeightMax) && (obstacleIntersection == true))
                {
                    obstacleIntersection = false;
                }

                // когда препятствие закончилось, вызывается функция расчёта
                if (obstacleIntersection == false && distanceInside_km > 0)
                { 
                    
                    EstimationClutterObstaclesResult obs = new EstimationClutterObstaclesResult()
                    {
                        clutterCode = 7, // код клатера определяется из профиля клатеров
                        d_km = distanceInside_km, // дистанция прохождения в данном клатере
                        elevation_deg = theta_deg, // УМ вхождения в клатер
                        endPoint = isEndPoint // признак того что это препятсвие в котором (внутри) находиться точка а иди б
                    };
                    LossObs = FunctionCalc(LossObs, obs);
                    //LossObs += 1;
                    distanceInside_km = 0;
                }
                
            }
            return LossObs;
        }
    }
    public struct EstimationClutterObstaclesResult
    {
        public int clutterCode;
        public double d_km;
        public bool endPoint;
        public double elevation_deg;
    }
}
