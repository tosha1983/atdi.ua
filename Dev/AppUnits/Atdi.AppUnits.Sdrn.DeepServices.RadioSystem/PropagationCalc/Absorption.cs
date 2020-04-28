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

        private static double InvCnDF(double p)
        {
            double t, x;
            double[] c = { 2.515517, 0.802853, 0.010328 };
            double[] d = { 1.432788, .189269, .001308 };

            if (p >= 1)
            {
                throw new Exception ("PropagationModels.ITU2109: probability should be less than 1");
            }
            else if (p <= .5)
            {
                t = Math.Sqrt(-2.0 * Math.Log(p));
                x = (c[0] + c[1] * t + c[2] * Math.Pow(t, 2)) / (1.0 + d[0] * t + d[1] * Math.Pow(t, 2) + d[2] * Math.Pow(t, 3)) - t;
                return x;
            }
            else
            {
                t = Math.Sqrt(-2.0 * Math.Log(1.0 - p));
                x = t - (c[0] + c[1] * t + c[2] * Math.Pow(t, 2)) / (1.0 + d[0] * t + d[1] * Math.Pow(t, 2) + d[2] * Math.Pow(t, 3));
                return x;
            }
            
        }

        private static double AB (double p, double sigma, double mu)
        {
            return InvCnDF(p) * sigma + mu;
        }

        private static double ITU2109(double elevation_deg, double Freq_MHz, double time_pc)
        {
            bool thermallyEfficientBuild = false;
            double r, s, t, u, v, w, x, y, z;
            double mu1, mu2, sigma1, sigma2, C;
            double f = Freq_MHz / 1000;

            if (thermallyEfficientBuild) { r = 28.19; s = -3.0; t = 8.48; u = 13.5; v = 3.8; w = 27.8; x = -2.9; y = 9.4; z = -2.1; }
            else { r = 12.64; s = 3.72; t = 0.96; u = 9.6; v = 2.0; w = 9.1; x = -3.0; y = 4.5; z = -2.0; }

            double Lh = r + s * Math.Log10(f) + t * Math.Pow(Math.Log10(f), 2);
            double Le = 0.212 * Math.Abs(elevation_deg);

            mu1 = Lh + Le;
            mu2 = w + x * Math.Log10(f);
            sigma1 = u + v * Math.Log10(f);
            sigma2 = y + z * Math.Log10(f);
            C = -3.0;

            return 10 * Math.Log10(Math.Pow(10 , 0.1 * AB(time_pc, sigma1, mu1)) + Math.Pow(10, 0.1 * AB(time_pc, sigma2, mu2)) + Math.Pow(10, 0.1 * C));
        }
    }
}
