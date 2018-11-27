using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.SDR.Server.MeasurementProcessing.SingleHound.ProcessSignal
{
    [Serializable]
    public class ReceiveIQStream
    {
        #region parameters
        public List<float[]> iq_samples;
        public List<int[]> triggers;
        public List<float[]> Ampl;
        public Double MinLevel;
        public Double MaxLevel;  
        #endregion 
        public ReceiveIQStream(int id_dev, int return_len, int samples_per_sec, Double TimeReceivingSec)
        {
            Double NumberPass = 0;
            NumberPass = TimeReceivingSec * samples_per_sec / return_len;
            iq_samples = new List<float[]>();
            triggers = new List<int[]>();
            for (int i = 0; i < NumberPass; i++)
            {
                float[] iq_sample = new float[return_len * 2];
                int[] trigger = new int[80];
                bb_api.bbFetchRaw(id_dev, iq_sample, trigger);
                iq_samples.Add(iq_sample);
                triggers.Add(trigger);
            }
        }

        public ReceiveIQStream()
        {

        }
        
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
