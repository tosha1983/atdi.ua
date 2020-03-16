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
    public class DataSynchronizationProcessDataAdapter : WpfDataAdapter<SDR.DataSynchronizationProcess, VM.DataSynchronizationProcessViewModel, DataSynchronizationProcessDataAdapter>
    {
        protected override Func<SDR.DataSynchronizationProcess, VM.DataSynchronizationProcessViewModel> GetMapper()
        {
            return source => new VM.DataSynchronizationProcessViewModel
            {
                Id = source.Id,
                Status = source.Status,
                DateCreated = source.DateCreated,
                DateStart = source.DateStart,
                DateEnd =  source.DateEnd,
                CreatedBy = source.CreatedBy
            };
        }
    }
}
