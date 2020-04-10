using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
    internal static class ProfilesCalculation
    {
        private static double CalcElevAngleArgs(double H1, double H2, double D, double RE)
        {
            if (D > 100)
            {
                return (Math.PI / 2 - Math.Acos((Math.Pow(RE + H1, 2) + Math.Pow(D, 2) - Math.Pow(RE + H2, 2)) / (2 * (RE + H1) * D)));
            }
            else
            {
                return (Math.Atan((H1 - H2) / D) * 180 / Math.PI);
            }
            
        }

        public static void CalcTilts(double re_km, double ha_m, double hb_m, double d_km, in short[] Profile, int StartPosition, int IndexerCount, out double tiltA_deg, out double tiltB_deg)
        {
           //both implementations requires testing 
            double dN = d_km * 1000 / (IndexerCount - StartPosition);
            double h1_m = ha_m + Profile[StartPosition];
            double h2_m = hb_m + Profile[IndexerCount];
            double angleArgAB = CalcElevAngleArgs(h1_m, hb_m, d_km * 1000, re_km * 1000);
            double angleArgBA = CalcElevAngleArgs(h2_m, hb_m, d_km * 1000, re_km * 1000);

            double minAngleArgAB = 1;
            double minAngleArgBA = 1;


            for (int n = StartPosition; n < IndexerCount - 1; n++)
            {
                double dAN = dN * (n - StartPosition);
                double dNB = dN * (IndexerCount - n);
                
                double nAngleArgAB = CalcElevAngleArgs(h1_m, Profile[n + StartPosition], dAN, re_km * 1000);
                double nAngleArgBA = CalcElevAngleArgs(h2_m, Profile[IndexerCount - n], dNB, re_km * 1000);

                if (minAngleArgAB > nAngleArgAB)
                {
                    minAngleArgAB = nAngleArgAB;
                }
                if (minAngleArgBA > nAngleArgBA)
                {
                    minAngleArgBA = nAngleArgBA;
                }
            }


            if (d_km > 0.1)
            {
                tiltA_deg = (Math.PI / 2 - Math.Acos(angleArgAB)) * 180 / Math.PI;
                tiltB_deg = (Math.PI / 2 - Math.Acos(angleArgBA)) * 180 / Math.PI;
            }
            else
            {
                tiltA_deg = Math.Atan(angleArgAB) * 180 / Math.PI;
                tiltB_deg = Math.Atan(angleArgAB) * 180 / Math.PI;
            }
            
        }


        public static void CalcTilts(double re_km, double h1_m, double h2_m, double d_km, out double tiltA_deg, out double tiltB_deg)
        {
            double angleArgAB = CalcElevAngleArgs(h1_m, h2_m, d_km * 1000, re_km * 1000);
            double angleArgBA = CalcElevAngleArgs(h2_m, h1_m, d_km * 1000, re_km * 1000);

            if (d_km > 0.1)
            {
                tiltA_deg = (Math.PI / 2 - Math.Acos(angleArgAB)) * 180 / Math.PI;
                tiltB_deg = (Math.PI / 2 - Math.Acos(angleArgBA)) * 180 / Math.PI;
            }
            else
            {
                tiltA_deg = Math.Atan(angleArgAB) * 180 / Math.PI;
                tiltB_deg = Math.Atan(angleArgAB) * 180 / Math.PI;
            }
        }
        public static EstimationClutterObstaclesResult[] EstimationClutterObstacles(in CalcLossArgs args)
        {
            List<EstimationClutterObstaclesResult> estimationClutterObstaclesResults = new List<EstimationClutterObstaclesResult>(); // Думаю Андрей тут будет резко против Нужно будет с ним посоветоваться
            EstimationClutterObstaclesResult estimationClutterObstaclesResult = new EstimationClutterObstaclesResult()
            {
                clutterCode = 1,
                d_km = 0.12,
                endPoint = false,
                elevation_deg = 0
            };
            estimationClutterObstaclesResults.Add(estimationClutterObstaclesResult);
            return estimationClutterObstaclesResults.ToArray();// Думаю Андрей тут будет резко против Нужно будет с ним посоветоваться
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
