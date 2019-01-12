using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDR = Atdi.AppServer.Contracts.Sdrns;
using VM = XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;


namespace XICSM.ICSControlClient.Models.WcfDataApadters
{
    public class ShortResultsMeasurementsStationDataAdapter : WpfDataAdapter<SDR.ShortResultsMeasurementsStation, VM.ShortResultsMeasurementsStationViewModel, ShortResultsMeasurementsStationDataAdapter>
    {
        protected override Func<SDR.ShortResultsMeasurementsStation, VM.ShortResultsMeasurementsStationViewModel> GetMapper()
        {
            return source => new VM.ShortResultsMeasurementsStationViewModel
            {
                GlobalSID = source.GlobalSID,
                SectorId = source.IdSector,
                StationId = source.Idstation,
                MeasGlobalSID = source.MeasGlobalSID,
                Standard = source.Standard,
                StationLocations = source.StationLocations,
                GeneralResultCentralFrequencyMeas = source.CentralFrequencyMeas_MHz,
                Status = source.Status
            };
        }
    }
}
