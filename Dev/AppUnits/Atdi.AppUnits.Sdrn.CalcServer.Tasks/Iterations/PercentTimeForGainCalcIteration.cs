using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis.MapService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public class PercentTimeForGainCalcIteration : IIterationHandler<PercentTimeForGainCalcData, double[]>
    {
        /// <summary>
        /// According ITU R-P.530, eq 19-23
        /// </summary>
        /// <param name="E_dB">Gain, dB</param>
        /// <param name="d_km">distance between transmitter and receiver, km</param>
        /// <param name="f_GHz">transmitter frequency, GHz</param>
        /// <param name="hTx_m">transmitter antenna height, m</param>
        /// <param name="hRx_m">receiver antenna height, m</param>
        /// <returns></returns>
        private double CalcPw(double E_dB, double d_km, double f_GHz, double hTx_m, double hRx_m)
        {
            int dN1 = -400; // taken from ITU-R-P.453 pic 12
            double K = Math.Pow(10, -4.6 - 0.0027 * dN1); 
            double ep = Math.Abs(hRx_m - hTx_m) / d_km; // angle
            double Aooi = -0.00076 * Math.Max(hRx_m, hTx_m) - Math.Log10(0.001 * Math.Pow(1 + ep, 1.03) / (K * Math.Pow(d_km, 3.4) * Math.Pow(f_GHz, 0.8))); //dB


            double pw = 0;
            if (E_dB > 10)
            {
                pw = 100 - Math.Pow(10, (-1.7 + 0.2 * Aooi - E_dB) / 3.5);
            }
            else
            {
                double pw_ = 100.0 - Math.Pow(10, (-1.7 + 0.2 * Aooi - E_dB) / 3.5);
                double qe_ = -20.0 / E_dB * Math.Log10(-Math.Log(1 - (100.0 - pw_) / 58.21));
                double qs = 2.05 * qe_ - 20.3;
                double qe = 8.0 + (1.0 + 0.3 * Math.Pow(10, -E_dB / 20.0)) * Math.Pow(10, -0.7 * E_dB / 20.0) * (qs + 12.0 * (Math.Pow(10, -E_dB / 20.0) + E_dB / 800));
                pw = 100 - 58.21 * (1 - Math.Exp(-Math.Pow(10, -qe * E_dB / 20)));
            }
            return pw;
        }
        public double[] Run(ITaskContext taskContext, PercentTimeForGainCalcData data)
        {
            double[] P_pc = new double[data.StationData.Length];
            double Pmax = -999;
            for (int i = 0; i < data.StationData.Length; i++)
            {
                if (data.StationData[i].Level_dBm > Pmax)
                {
                    Pmax = data.StationData[i].Level_dBm.Value;
                }
            }
            for (int i = 0; i < data.StationData.Length; i++)
            {
                double E = Pmax - data.StationData[i].Level_dBm.Value;
                P_pc[i] = 100 - CalcPw(E, data.StationData[i].Distance_km.Value, data.StationData[i].Frequency_Mhz.Value * 1000, data.StationData[i].AntennaHeight_m.Value, data.SensorAntennaHeight_m);
            }

            return P_pc;
        }
    }
}
