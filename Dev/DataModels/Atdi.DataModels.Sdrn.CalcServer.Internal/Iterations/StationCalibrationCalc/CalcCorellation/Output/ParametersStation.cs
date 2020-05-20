using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct ParametersStation
    {
        public int Altitude_m;

        public float Tilt_Deg;

        public float Azimuth_deg;

        public double Lat_deg;

        public double Lon_deg;

        public float Power_dB;

        public double Freq_MHz;
    }
}
