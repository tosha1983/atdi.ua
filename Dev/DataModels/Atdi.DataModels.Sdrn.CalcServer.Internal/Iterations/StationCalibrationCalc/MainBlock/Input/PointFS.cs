using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct PointFS
    {
        public EpsgCoordinate Coordinate;

        public int Height_m;

        public float FieldStrength_dBmkVm;

        public float Level_dBm;

        public int Count;
    }
}
