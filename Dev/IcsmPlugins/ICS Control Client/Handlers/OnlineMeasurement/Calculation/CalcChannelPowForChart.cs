using System;


namespace XICSM.ICSControlClient.Handlers.OnlineMeasurement.Calculation
{
    static class CalcChannelPowForChart
    {
        /// <summary>
        /// Get Pow in Channel in dBm
        /// </summary>
        /// <param name="levels_dBm"></param>
        /// <param name="freq_MHz"></param>
        /// <param name="RBW_kHz"></param>
        /// <returns></returns>
        public static double getPow(float[] levels_dBm, double[] freq_MHz, double RBW_kHz)
        {
            if ((levels_dBm == null)||(RBW_kHz == 0))
            {return -999;}
            if (levels_dBm.Length == 1) { return levels_dBm[0];}
            double sum_mW = 0;
            for (int i = 0; levels_dBm.Length > i; i++)
            {
                sum_mW = sum_mW + Math.Pow(10, levels_dBm[i]/10.0);
            }
            sum_mW = 10 * Math.Log10(sum_mW);
            if ((freq_MHz == null) || (freq_MHz.Length<2)||(RBW_kHz <0)) {return sum_mW;}
            double delta_freq = (freq_MHz[freq_MHz.Length - 1] - freq_MHz[0])/(freq_MHz.Length - 1); if (delta_freq < 0) { delta_freq = -delta_freq;}
            if (delta_freq == 0) { return sum_mW; }
            double resizing = 10 * Math.Log10(delta_freq / RBW_kHz) + 30;
            sum_mW = sum_mW + resizing;
            return sum_mW;
        }

        public static double CalclIntensity(double power, double antennaFactor)
        {
            // E = P + AntennaFactor – 106.99 
            return power + antennaFactor + 106.99;
        }
    }
}
