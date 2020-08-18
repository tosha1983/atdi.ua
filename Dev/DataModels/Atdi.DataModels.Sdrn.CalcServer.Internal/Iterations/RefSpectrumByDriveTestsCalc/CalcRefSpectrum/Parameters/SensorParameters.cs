using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class SensorParameters
    {

        public long? SensorId;

        public double RxFeederLoss_dB;

        public string SensorName;

        public string SensorTechId;

        public string SensorTitle;

        public float Gain;

        public float Loss;

        public StationAntenna  SensorAntenna;

        public float SensorAntennaHeight_m;

        public AtdiCoordinate Coordinate;

    }
}
