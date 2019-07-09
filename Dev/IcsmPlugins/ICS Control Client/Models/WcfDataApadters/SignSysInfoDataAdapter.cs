using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using VM = XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;

namespace XICSM.ICSControlClient.Models.WcfDataApadters
{
    public class SignSysInfoDataAdapter : WpfDataAdapter<SDR.SignalingSysInfo, VM.SignSysInfoViewModel, SignSysInfoDataAdapter>
    {
        protected override Func<SDR.SignalingSysInfo, VM.SignSysInfoViewModel> GetMapper()
        {
            return Mappers.Map;
        }
    }
}
