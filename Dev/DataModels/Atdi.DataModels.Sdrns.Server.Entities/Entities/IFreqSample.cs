using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IFreqSample
    {
        long Id { get; set; }
        double? Freq_MHz { get; set; }
        double? Level_dBm { get; set; }
        double? Level_dBmkVm { get; set; }
        double? LevelMin_dBm { get; set; }
        double? LevelMax_dBm { get; set; }
        double? OccupationPt { get; set; }
        long? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}
