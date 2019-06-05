using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IFreqSampleRaw
    {
        int Id { get; set; }
        float? Freq_MHz { get; set; }
        float? Level_dBm { get; set; }
        float? Level_dBmkVm { get; set; }
        float? LevelMin_dBm { get; set; }
        float? LevelMax_dBm { get; set; }
        float? OccupationPt { get; set; }
        int? ResMeasId { get; set; }
        IResMeasRaw RESMEAS { get; set; }
    }
}
