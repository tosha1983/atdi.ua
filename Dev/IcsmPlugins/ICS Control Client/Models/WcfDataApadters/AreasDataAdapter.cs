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
    public class AreasDataAdapter : WpfDataAdapter<SDR.Area, VM.AreasViewModel, AreasDataAdapter>
    {
        protected override Func<SDR.Area, VM.AreasViewModel> GetMapper()
        {
            return source => new VM.AreasViewModel
            {
                IdentifierFromICSM = source.IdentifierFromICSM,
                Name = source.Name,
                DateCreated = source.DateCreated,
                TypeArea = source.TypeArea,
                CreatedBy = source.CreatedBy,
                Location = source.Location
            };
        }
    }
}
