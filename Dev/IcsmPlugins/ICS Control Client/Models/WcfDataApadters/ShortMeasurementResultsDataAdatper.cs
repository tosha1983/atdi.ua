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
    public class ShortMeasurementResultsDataAdatper : WpfDataAdapter<SDR.ShortMeasurementResults, VM.ShortMeasurementResultsViewModel, ShortMeasurementResultsDataAdatper>
    {
        protected override Func<SDR.ShortMeasurementResults, VM.ShortMeasurementResultsViewModel> GetMapper()
        {
            return source => new VM.ShortMeasurementResultsViewModel
            {
                MeasSdrResultsId = source.Id.MeasSdrResultsId,
                DataRank = source.DataRank.ToNull(),
                MeasTaskId = source.Id.MeasTaskId.Value,
                SubMeasTaskId = source.Id.SubMeasTaskId,
                SubMeasTaskStationId = source.Id.SubMeasTaskStationId,
                Number = source.Number,
                Status = source.Status,
                TimeMeas = source.TimeMeas.ToNull(),
                TypeMeasurements = source.TypeMeasurements
            };
        }
    }
}
