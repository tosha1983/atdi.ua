﻿using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct FieldStrengthCalcData
	{
		public PropagationModel PropagationModel;

		public StationAntenna Antenna;

		public AtdiCoordinate StationCoordinate;

		public AtdiCoordinate PointCoordinate;
	}
}
