using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMeasFreqParam_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasFreqParam: IMeasFreqParam_PK
    {
        string Mode { get; set; }
        double? Step { get; set; }
        double? Rgl { get; set; }
        double? Rgu { get; set; }
        long? MeasTaskId { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
