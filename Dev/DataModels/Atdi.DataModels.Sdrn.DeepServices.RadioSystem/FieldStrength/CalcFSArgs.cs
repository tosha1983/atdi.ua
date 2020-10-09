using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.FieldStrength
{
    [Serializable]
    public struct CalcFSArgs
    {
		public double ha;

        public double hef;

        public double d;

        public double f;

        public double p;

        public double h_gr;

        public double h2;

        public bool h2AboveSea;

        public LandSea[] list1;

    }
}

