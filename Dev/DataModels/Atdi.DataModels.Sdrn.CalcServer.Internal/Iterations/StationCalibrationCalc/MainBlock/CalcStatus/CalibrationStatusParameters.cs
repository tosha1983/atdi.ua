using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public class CalibrationStatusParameters
    {
        public ClientContextStationType StatusStation;

        public bool Corellation;

        public bool TrustOldResults;

        public bool PointForCorrelation;

        public bool ExceptionParameter;

        public StationStatusResult ResultStationStatus;

        public DriveTestStatusResult ResultDriveTestStatus;

        public ModeStatusParameters ModeStatusParameters;
    }
}
