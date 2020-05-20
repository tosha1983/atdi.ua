using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct CalibrationStationResult
    {
        public string TableName;
        public long IdStation;
        public string GSIDByICSM;
        public string GSIDByMeasurement;
        public StationStatusResult ResultStationStatus;
        public ParametersStation ParametersStationNew;
        public ParametersStation ParametersStationOld;
        public float MaxCorellation;
    }
}