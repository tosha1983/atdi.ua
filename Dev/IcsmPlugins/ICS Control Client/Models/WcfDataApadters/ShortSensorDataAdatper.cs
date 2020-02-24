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
    public class ShortSensorDataAdatper : WpfDataAdapter<SDR.ShortSensor, VM.ShortSensorViewModel, ShortSensorDataAdatper>
    {
        protected override Func<SDR.ShortSensor, VM.ShortSensorViewModel> GetMapper()
        {
            return source => new VM.ShortSensorViewModel
            {
                Administration = source.Administration,
                AntGainMax = source.AntGainMax.ToNull(),
                Id = source.Id.Value,
                AntManufacturer = source.AntManufacturer,
                AntName = source.AntName,
                BiuseDate = source.BiuseDate.ToNull(),
                CreatedBy = source.CreatedBy,
                DateCreated = source.DateCreated.ToNull(),
                EouseDate = source.EouseDate.ToNull(),
                EquipCode = source.EquipCode,
                EquipManufacturer = source.EquipManufacturer,
                EquipName = source.EquipName,
                LowerFreq = source.LowerFreq.ToNull(),
                Title = source.Title,
                Name = source.Name,
                NetworkId = source.NetworkId,
                RxLoss = source.RxLoss.ToNull(),
                Status = source.Status,
                UpperFreq = source.UpperFreq.ToNull()
            };
        }
    }
}
