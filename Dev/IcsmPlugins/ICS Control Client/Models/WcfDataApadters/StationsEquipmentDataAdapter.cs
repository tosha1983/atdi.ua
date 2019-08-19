using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M = XICSM.ICSControlClient.Models;
using VM = XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;

namespace XICSM.ICSControlClient.Models.WcfDataApadters
{
    public class StationsEquipmentDataAdapter : WpfDataAdapter<M.StationsEquipment, VM.StationsEquipmentViewModel, StationsEquipmentDataAdapter>
    {
        protected override Func<M.StationsEquipment, VM.StationsEquipmentViewModel> GetMapper()
        {
            return Mappers.Map;
        }
    }
}
