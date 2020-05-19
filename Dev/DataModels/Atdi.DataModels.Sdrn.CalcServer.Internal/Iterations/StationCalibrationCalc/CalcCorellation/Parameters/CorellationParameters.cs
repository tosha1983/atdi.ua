using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct CorellationParameters
    {
        public float MinRangeMeasurements_dBmkV;

        public float MaxRangeMeasurements_dBmkV;

        public int CorrelationDistance_m;

        public float Delta_dB;

        public float MaxAntennasPatternLoss_dB;

        // здесь проставляем деф. значение false
        public bool Detail;
    }
}
