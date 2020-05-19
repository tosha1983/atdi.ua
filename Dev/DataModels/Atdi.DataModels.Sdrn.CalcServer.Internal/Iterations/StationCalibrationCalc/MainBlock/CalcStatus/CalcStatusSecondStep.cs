using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct CalcStatusSecondStep
    {
        public string StatusStation;

        public bool PointForCorrelation;

        public bool ExceptionParameter;

        public string ResultStationStatus;

        public string ResultDriveTestStatus;
    }
}
