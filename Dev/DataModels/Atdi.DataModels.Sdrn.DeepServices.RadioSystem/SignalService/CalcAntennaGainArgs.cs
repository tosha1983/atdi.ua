using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService
{
    [Serializable]
    public struct CalcAntennaGainArgs
	{
		public StationAntenna Antenna;
        public PolarizationType PolarizationEquipment;
        public PolarizationType PolarizationWave;
        public double AzimutToTarget_deg;
        public double TiltToTarget_deg;
    }
}
