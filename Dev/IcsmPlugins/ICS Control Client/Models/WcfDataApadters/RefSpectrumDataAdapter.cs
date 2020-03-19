using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using VM = XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;

namespace XICSM.ICSControlClient.Models.WcfDataApadters
{
    public class RefSpectrumDataAdapter : WpfDataAdapter<SDR.RefSpectrum, VM.RefSpectrumViewModel, RefSpectrumDataAdapter>
    {
        protected override Func<SDR.RefSpectrum, VM.RefSpectrumViewModel> GetMapper()
        {
            return Mappers.Map;
        }
    }
}
