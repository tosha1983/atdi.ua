using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06TaskResult
{
    public class AffectedADMModel
    {
        public long Id { get; set; }
        public long Gn06ResultId { get; set; }
        public string Adm { get; set; }
        public string TypeAffected { get; set; }
        public string AffectedServices { get; set; }
    }
}
