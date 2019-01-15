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
    public class ResultsMeasurementsStationExtentedDataAdapter : WpfDataAdapter<SDR.ResultsMeasurementsStationExtended, VM.ResultsMeasurementsStationExtentedViewModel, ResultsMeasurementsStationExtentedDataAdapter>
    {
        protected override Func<SDR.ResultsMeasurementsStationExtended, VM.ResultsMeasurementsStationExtentedViewModel> GetMapper()
        {
            return Mappers.Map;
        }
    }
}
