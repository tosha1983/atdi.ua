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
        public static void CalcTilts(double re_km, double ha_m, double hb_m, double d_km, in short[] profile, int StartPosition, int IndexerCount, out double tiltA_deg, out double tiltB_deg)
        {
            tiltA_deg = 0;
            tiltB_deg = 0;
        }
        public static void CalcTilts(double re_km, double h1_m, double h2_m, double d_km, out double tiltA_deg, out double tiltB_deg)
        {
            tiltA_deg = 0;
            tiltB_deg = 0;
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
