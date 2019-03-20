using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IWorkTimeRaw
    {
        int Id { get; set; }
        DateTime? StartEmitting { get; set; }
        DateTime? StopEmitting { get; set; }
        int? HitCount { get; set; }
        double? PersentAvailability { get; set; }
        int? EmittingId { get; set; }
        IEmittingRaw EMITTING { get; set; }
    }
}
