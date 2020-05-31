using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class CalibrationStationResult
    {
        public long StationMonitoringId;
        public string ExternalCode;
        public string ExternalSource;
        public string LicenseGsid;
        public string RealGsid;
        public StationStatusResult ResultStationStatus;
        public ParametersStation ParametersStationNew;
        public ParametersStation ParametersStationOld;
        public float MaxCorellation;
    }
}