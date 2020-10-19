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
    public class SensorParameters
    {

        public long? SensorId;

        public double RxFeederLoss_dB;

        public StationAntenna[]  SensorAntennas;

        public float SensorAntennaHeight_m;

        public AtdiCoordinate Coordinate;

    }
}
