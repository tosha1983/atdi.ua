using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    [Serializable]
    public class RefSpectrumStationAndSensors
    {
        public ContextStation  ContextStation;

        public SensorParameters  SensorParameters;
    }
}
