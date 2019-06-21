using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    public interface IMeasTaskSignaling_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMeasTaskSignaling: IMeasTaskSignaling_PK
    {
        int? CompareTraceJustWithRefLevels { get; set; }
        int? AutoDivisionEmitting { get; set; }
        double? DifferenceMaxMax { get; set; }
        int? FiltrationTrace { get; set; }
        double? allowableExcess_dB { get; set; }
        int? SignalizationNCount { get; set; }
        int? SignalizationNChenal { get; set; }
        long? IdMeasTask { get; set; }
        IMeasTask MEASTASK { get; set; }
    }
}
