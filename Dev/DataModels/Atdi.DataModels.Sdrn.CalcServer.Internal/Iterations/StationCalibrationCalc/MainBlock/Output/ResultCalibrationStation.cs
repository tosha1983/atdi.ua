using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct ResultCalibrationStation
    {
        public string TableName;
        public long IdStation;
        public string GSIDByICSM;
        public string GSIDByMeasurement;
        public string ResultStationStatus;
        public ParametersStation ParametersStationbyICSM;
        public ParametersStation ParametersStationbyMeasurement;
        public Clients.ClientContextStation ClientContextStation;
        public double MaxCorellation;
        public int CountStation_CS;
        public int CountStation_NS;
        public int CountStation_IT;
        public int CountStation_NF;
        public int CountStation_UN;
    }
}