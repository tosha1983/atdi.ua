using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations
{
    [Serializable]
    public class StationReceiver
    {
        public double[] Freqs_MHz;

        public double Freq_MHz;

		public double BW_kHz;

		public float Loss_dB;

		public float KTBF_dBm;

        public float Threshold_dBm;
      
        public PolarizationType Polarization;
	}
}
