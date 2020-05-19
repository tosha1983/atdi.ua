using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations
{
	public class StationTransmitter
	{
        public double[] Freqs_MHz;

        public double Freq_MHz;

		public double BW_kHz;

		public float Loss_dB;

		public float MaxPower_dBm;

		public PolarizationType Polarization;
	}
}
