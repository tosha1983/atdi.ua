using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct ResultCorrelationGSIDGroupeStations
    {
        public float Corellation_pc;
        public double Corellation_factor;
        public float StdDev_dB;
        public float AvErr_dB;
        public string StatusResult;
        public Clients.ClientContextStation ClientContextStation;
        public DriveTestsResult DriveTestsResult;
        public CorrellationPoint[] CorrellationPoints;
        public ParametersStation ParametersStationNew;
        public ParametersStation ParametersStationOld;
    }
}