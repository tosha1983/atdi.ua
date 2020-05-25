using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct CorrellationPoint
    {
        public double Lon_DEC;

        public double Lat_DEC;

        public double Dist_km;

        public double FSMeas_dBmkVm;

        public double FSCalc_dBmkVm;
    }
}
