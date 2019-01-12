using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.MonitoringProcess.ProcessSignal
{
    [Serializable]
    public class ReceivedIQStream
    {
        #region parameters
        public List<float[]> iq_samples;
        public List<int[]> triggers;
        public List<float[]> Ampl;
        public List<int> dataRemainings;
        public List<int> sampleLosses;
        public List<int> iqSeces;
        public List<int> iqNanos;
        public double MinLevel;
        public double MaxLevel;
        public DateTime TimeMeasStart; // время начала измерения потока IQ
        public double durationReceiving_sec; 
        #endregion
        public void CalcAmpl()
        {
            Ampl = new List<float[]>();
            MinLevel = 1000;
            MaxLevel = 0; 
            for (int i = 0; i < iq_samples.Count; i++)
            {
                float[] arrAmpl = new float[iq_samples[i].Length/2];
                for (int j = 0; j<iq_samples[i].Length; j=j+2)
                {
                    float _arrAmpl = (float)Math.Sqrt(iq_samples[i][j] * iq_samples[i][j] + iq_samples[i][j+1] * iq_samples[i][j+1]);
                    if (MinLevel > _arrAmpl) { MinLevel = _arrAmpl; }
                    if (MaxLevel < _arrAmpl) { MaxLevel = _arrAmpl; }
                    arrAmpl[j / 2] = _arrAmpl;
                }
                Ampl.Add(arrAmpl);
            }
        } 
    }
}
