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
    }
    public class CountoursPoints
    {
        public double Lon_DEC;
        public double Lat_DEC;
        public int Distance;
        public int FS;
        public int Height;
        public PointType PointType;
    }
    public enum ContourType
    {
        Unknown = 0,
        Etalon = 1,
        New = 2,
        Affected = 3,
        Correct = 4
    }
    public enum PointType
    {
        Unknown = 0,
        Etalon = 1,
        Affected = 2,
        Correct = 3
    }
}
