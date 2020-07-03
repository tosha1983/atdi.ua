using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.FieldStrength
{
    public struct LandSea
    {
        public double land;
        public double sea;
        public double Length { get { return land + sea; } }
    }
}

