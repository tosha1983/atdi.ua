using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Models
{
    public class CalcResultEventsModel
    {
        public long Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string LevelName { get; set; }
        public string Message { get; set; }
    }
}
