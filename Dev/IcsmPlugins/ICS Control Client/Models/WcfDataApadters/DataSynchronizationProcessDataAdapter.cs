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
    public class DataSynchronizationProcessDataAdapter : WpfDataAdapter<SDR.HeadProtocols, VM.DataSynchronizationProcessViewModel, DataSynchronizationProcessDataAdapter>
    {
        protected override Func<SDR.HeadProtocols, VM.DataSynchronizationProcessViewModel> GetMapper()
        {
            return source => new VM.DataSynchronizationProcessViewModel
            {
                GSID = source.PermissionGlobalSID,
                DateMeas = source.DateMeas,
                Owner = source.OwnerName,
                StationAddress = source.Address,
                Coordinates = source.Longitude.ToString() + ", " + source.Latitude.ToString(),
                NumberPermission = source.PermissionNumber,
                PermissionPeriod = source.PermissionStart,
                PermissionStart = source.PermissionStop,
                SensorName = source.TitleSensor,
                DetailProtocols = source.DetailProtocols
            };
        }
    }
}
