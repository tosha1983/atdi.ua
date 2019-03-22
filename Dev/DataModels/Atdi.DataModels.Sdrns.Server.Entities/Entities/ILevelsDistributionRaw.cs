using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ILevelsDistributionRaw
    {
        int Id { get; set; }
        int? level { get; set; }
        int? count { get; set; }
        int? EmittingId { get; set; }
        IEmittingRaw EMITTING { get; set; }
    }
}