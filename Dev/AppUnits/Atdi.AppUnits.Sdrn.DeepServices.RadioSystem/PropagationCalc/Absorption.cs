using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
    internal static class AbsorptionCalc
    {
        public static double CalcAbsorption(AbsorptionCalcBlockModelType ModelType, double Freq_MHz, double Time_pc ,double currentLoss, EstimationClutterObstaclesResult obs)
        {
            double L1 = 0;
            double L2 = 0;
            switch (ModelType)
            {
                case AbsorptionCalcBlockModelType.Flat:
                    L1 = AbsorptionCalc.Flat(obs);
                    break;
                case AbsorptionCalcBlockModelType.FlatAndLinear:
                    L1 = AbsorptionCalc.Flat(obs);
                    L2 = AbsorptionCalc.Linear(obs);
                    break;
                case AbsorptionCalcBlockModelType.ITU2109AndLinear:
                    L1 = AbsorptionCalc.ITU2109(obs.elevation_deg, Freq_MHz, Time_pc);
                    L2 = AbsorptionCalc.Linear(obs);
                    break;
                case AbsorptionCalcBlockModelType.ITU2109_2:
                    L1 = AbsorptionCalc.ITU2109(obs.elevation_deg, Freq_MHz, Time_pc);
                    break;
                case AbsorptionCalcBlockModelType.Linear:
                    L2 = AbsorptionCalc.Linear(obs);
                    break;
                default:
                    break;
            }
            var Labsorption_dB = currentLoss + L1 + L2;
            return Labsorption_dB;
        }
        public static double CalcClutter(ClutterCalcBlockModelType ModelType, double Freq_MHz, double Time_pc, double currentLoss, EstimationClutterObstaclesResult obs)
        {
            if (obs.endPoint)
            {
                double L1 = 0;
                switch (ModelType)
                {
                    case ClutterCalcBlockModelType.Flat:
                        L1 = AbsorptionCalc.Flat(obs);
                        break;
                    case ClutterCalcBlockModelType.ITU2109:
                        L1 = AbsorptionCalc.ITU2109(obs.elevation_deg, Freq_MHz, Time_pc);
                        break;
                    default:
                        break;
                }
                var Labsorption_dB = currentLoss + L1;
                return Labsorption_dB;
            }
            return currentLoss;
        }
        private static double Flat(EstimationClutterObstaclesResult estimationClutterObstaclesResult)
        {
            return 0;
        }
        private static double Linear(EstimationClutterObstaclesResult estimationClutterObstaclesResult)
        {
            return 0;
        }
        private static double ITU2109(double elevation_deg, double Freq_MHz, double time_pc)
        {
            return 0;
        }
    }
}
