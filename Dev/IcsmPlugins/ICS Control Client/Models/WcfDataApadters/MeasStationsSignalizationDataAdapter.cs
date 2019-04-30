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
    public class MeasStationsSignalizationDataAdapter : WpfDataAdapter<M.MeasStationsSignalization, VM.MeasStationsSignalizationViewModel, MeasStationsSignalizationDataAdapter>
    {
        protected override Func<M.MeasStationsSignalization, VM.MeasStationsSignalizationViewModel> GetMapper()
        {
            return Mappers.Map;
        }
    }
}
