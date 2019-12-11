using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    static class CalcSDRParameters
    {
        public static double SDRGainFromFrequency(MesureTraceDeviceProperties MesureTraceDeviceProperties, double Frequency_Hz)
        {
            /*
            // Константа с файла конфигурации
            double GainByDefault = 3;
            // Конец констант

            if ((Frequency_Hz < 0.009) || (Frequency_Hz > 400000)||(MesureTraceDeviceProperties == null) || (MesureTraceDeviceProperties.StandardDeviceProperties == null) || 
                (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters == null)) { return GainByDefault; }
            if ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Freq_Hz <= Frequency_Hz)
            { return MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Gain;}
            if ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length].Freq_Hz >= Frequency_Hz)
            { return MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length].Gain; }
            for (int i = 0; i < MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length - 1; i++)
            {
                if (((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz <= Frequency_Hz) && ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i+1].Freq_Hz >= Frequency_Hz))
                {
                    double G = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Gain + 
                        (Frequency_Hz - (double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz) * 
                        (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Gain) / 
                        ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Freq_Hz - (double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz);
                    return G;
                }
            }
            return GainByDefault;
            */
            // Константа с файла конфигурации
            double GainByDefault = 0;

            if ((Frequency_Hz < 9) || (Frequency_Hz > 400000000000) || (MesureTraceDeviceProperties == null) || (MesureTraceDeviceProperties.StandardDeviceProperties == null) ||
                (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters == null)) { return GainByDefault; }
            if ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Freq_Hz >= Frequency_Hz)
            {
                double GainLoss = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].FeederLoss_dB;
                return GainLoss;
            }
            if ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length - 1].Freq_Hz <= Frequency_Hz)
            {
                double GainLoss = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length - 1].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length - 1].FeederLoss_dB;
                return GainLoss;
            }
            if (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length == 1)
            {
                double GainLoss = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[0].FeederLoss_dB;
                return GainLoss;
            }
            for (var i = 0; i < MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters.Length - 1; i++)
            {
                if (((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz <= Frequency_Hz) && ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Freq_Hz >= Frequency_Hz))
                {
                    double G = MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].FeederLoss_dB +
                        (Frequency_Hz - (double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz) *
                        (MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Gain - MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Gain -
                        MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].FeederLoss_dB + MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].FeederLoss_dB) /
                        ((double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i + 1].Freq_Hz - (double)MesureTraceDeviceProperties.StandardDeviceProperties.RadioPathParameters[i].Freq_Hz);
                    return G;
                }
            }
            return GainByDefault;
        }
    }
}
