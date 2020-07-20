using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06TaskResult
{
    public class ContourModel
    {
        public long Id { get; set; }
        public long Gn06ResultId { get; set; }
        public byte ContourType { get; set; }
        public string ContourTypeName { get; set; }
        public int Distance { get; set; }
        public double FS { get; set; }
        public string AffectedADM { get; set; }
        public int PointsCount { get; set; }
        public CountoursPoint[] CountoursPoints { get; set; }
    }
}
