﻿using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService
{
	public struct CalcAntennaGainArgs
	{
		public StationAntenna Antenna;
        public PolarizationType PolarizationEquipment;
        public PolarizationType PolarizationWave;
        public double AzimutToPoint_deg;
        public double TiltToPoint_deg;
    }
}
