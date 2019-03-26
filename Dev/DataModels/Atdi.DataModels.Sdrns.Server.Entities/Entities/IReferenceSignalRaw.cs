using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IReferenceSignalRaw
    {
        int Id { get; set; }
        double? Frequency_MHz { get; set; }
        double? Bandwidth_kHz { get; set; }
        double? LevelSignal_dBm { get; set; }
        int? RefSituationId { get; set; }
        IReferenceSituationRaw REFSITUATION { get; set; }
    }
}

