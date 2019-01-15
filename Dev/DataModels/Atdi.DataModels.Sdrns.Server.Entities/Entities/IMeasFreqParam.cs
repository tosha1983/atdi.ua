using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IMeasFreqParam
    {
        int Id { get; set; }
        string Mode { get; set; }
        double? Step { get; set; }
        double? Rgl { get; set; }
        double? Rgu { get; set; }
        int? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
