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
    public class MeasTaskDetailStationDataAdapter : WpfDataAdapter<SDR.StationDataForMeasurements, VM.MeasTaskDetailStationViewModel, MeasTaskDetailStationDataAdapter>
    {
        protected override Func<SDR.StationDataForMeasurements, VM.MeasTaskDetailStationViewModel> GetMapper()
        {
            return Mappers.Map;
        }
    }
}
