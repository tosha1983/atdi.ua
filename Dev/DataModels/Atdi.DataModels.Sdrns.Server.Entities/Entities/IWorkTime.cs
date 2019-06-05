using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IWorkTime
    {
        int Id { get; set; }
        DateTime? StartEmitting { get; set; }
        DateTime? StopEmitting { get; set; }
        int? HitCount { get; set; }
        float? PersentAvailability { get; set; }
        int? EmittingId { get; set; }
        IEmitting EMITTING { get; set; }
    }
}
