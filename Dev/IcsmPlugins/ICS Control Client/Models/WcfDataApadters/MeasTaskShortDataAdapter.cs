using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M = XICSM.ICSControlClient.Models;
using VM = XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;

namespace XICSM.ICSControlClient.Models.WcfDataApadters
{
    public class MeasTaskShortDataAdapter : WpfDataAdapter<M.MeasTask, VM.MeasTaskShortViewModel, MeasTaskShortDataAdapter>
    {
        protected override Func<M.MeasTask, VM.MeasTaskShortViewModel> GetMapper()
        {
            return Mappers.Map;
        }
    }
}
