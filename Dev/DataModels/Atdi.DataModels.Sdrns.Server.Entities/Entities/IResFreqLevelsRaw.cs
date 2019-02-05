using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResFreqLevelsRaw
    {
        int Id { get; set; }
        double? Freq_MHz { get; set; }
        double? Level_dBm { get; set; }
        int? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}
