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

            double dN = d_km / (profilePointsNumber - profileStartPosition);
            double h1_m = ha_m + profile_m[profileStartPosition];
            double h2_m = hb_m + profile_m[profilePointsNumber];
            
            double maxAngleArgAB = CalcElevAngleArgs(h1_m, h2_m, d_km, re_km);
            double maxAngleArgBA = CalcElevAngleArgs(h2_m, h1_m, d_km, re_km);


            for (int n = profileStartPosition; n < profilePointsNumber - 1; n++)
            {
                double dAN = dN * (n - profileStartPosition);
                double dNB = dN * (profilePointsNumber - n);
                
                double nAngleArgAB = CalcElevAngleArgs(h1_m, profile_m[n + profileStartPosition], dAN, re_km);
                double nAngleArgBA = CalcElevAngleArgs(h2_m, profile_m[profilePointsNumber - n], dNB, re_km);

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
            double LossObs =0;
            for (int i = 0; i <10; i++)
            {
                EstimationClutterObstaclesResult obs = new EstimationClutterObstaclesResult()
                {
                    clutterCode = 7, // код клатера определяется из профиля клатеров
                    d_km = 0.01, // дистанция прохождения в данном клатере
                    elevation_deg =0.1, // УМ вхождения в клатер
                    endPoint = false // признак того что это препятсвие в котором (внутри) находиться точка а иди б
                };
                LossObs = FunctionCalc(LossObs, obs);
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
