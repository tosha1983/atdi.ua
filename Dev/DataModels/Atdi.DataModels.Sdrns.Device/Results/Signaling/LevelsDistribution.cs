using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [Serializable]
    public class LevelsDistribution
    {
        public int[] Levels;
        public int[] Count;
    }

}
