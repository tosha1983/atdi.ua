using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using VM = XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.ViewModels.Coordinates;
using XICSM.ICSControlClient.ViewModels.Reports;

namespace XICSM.ICSControlClient.Models.WcfDataApadters
{
    public class DataSynchronizationProcessDataAdapter : WpfDataAdapter<SDR.HeadProtocols, VM.DataSynchronizationProcessViewModel, DataSynchronizationProcessDataAdapter>
    {
        protected override Func<SDR.HeadProtocols, VM.DataSynchronizationProcessViewModel> GetMapper()
        {
            return Mappers.Map;
        }
    }
}
