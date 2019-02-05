using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IBWMeasResultRaw
    {
        int Id { get; set; }
        int? MarkerIndex { get; set; }
        int? T1 { get; set; }
        int? T2 { get; set; }
        double? BW_kHz { get; set; }
        int? Сorrectnessestim { get; set; }
        int? TraceCount { get; set; }
        int? ResMeasId { get; set; }
        IResMeas RESMEAS { get; set; }
    }
}
