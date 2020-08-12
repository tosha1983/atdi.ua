using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class DriveTestParameters
    {
        public string GSID;

        public double Freq_MHz;

        public string Standard;

        public int CountPoints;

        public long DriveTestId;

        public long? SensorId;

        public double RxFeederLoss_dB;

        public string SensorName;

        public string SensorTechId;

        public string SensorTitle;

        public float MaxCorrelation;

        public float Gain;

        public float Loss;

        public StationAntenna  SensorAntenna;

        public float SensorAntennaHeight_m;

        public AtdiCoordinate Coordinate;

        public DateTime? MeasTime;

    }
}
