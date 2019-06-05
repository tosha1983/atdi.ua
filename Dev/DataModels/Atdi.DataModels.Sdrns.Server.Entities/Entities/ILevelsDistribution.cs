using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ILevelsDistribution
    {
        long Id { get; set; }
        int? level { get; set; }
        int? count { get; set; }
        long? EmittingId { get; set; }
        IEmitting EMITTING { get; set; }
    }
}