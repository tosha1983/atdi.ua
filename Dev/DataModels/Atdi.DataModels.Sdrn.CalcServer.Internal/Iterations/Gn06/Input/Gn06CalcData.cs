﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class Gn06CalcData
    {
        public Gn06TaskParameters Gn06TaskParameters;
        public PropagationModel PropagationModel;
        public ProjectMapData MapData;
        public CluttersDesc CluttersDesc;
        public FieldStrengthCalcData FieldStrengthCalcData;
        public string Projection;
    }
}