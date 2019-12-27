using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class ConvertSemplFreqToFrequencySample
    {
        public static FrequencySample[] Convert(this SemplFreq[] semplFreqs)
        {
            var fSemples = new FrequencySample[semplFreqs.Length];
            for (var i = 0; i < semplFreqs.Length; i++)
            {
                if (semplFreqs[i] != null)
                {
                    fSemples[i] = new FrequencySample
                    {
                        Freq_MHz = semplFreqs[i].Freq,
                        Level_dBm = semplFreqs[i].LeveldBm,
                        Level_dBmkVm = semplFreqs[i].LeveldBmkVm,
                        LevelMax_dBm = semplFreqs[i].LevelMaxdBm,
                        LevelMin_dBm = semplFreqs[i].LevelMindBm,
                        Occupation_Pt = semplFreqs[i].OcupationPt
                    };
                }
            }
            return fSemples;
        }

    }
}
