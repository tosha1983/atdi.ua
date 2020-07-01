using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
    public class ContoursResult
    {
        public ContourType ContourType;
        public int Distance;
        public int FS;
        public string AffectedADM;
        public int PointsCount;
        public CountoursPoint[] CountoursPoints;
    }
}
