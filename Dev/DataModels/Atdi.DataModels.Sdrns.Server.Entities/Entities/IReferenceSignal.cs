using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IReferenceSignal
    {
        long Id { get; set; }
        double? Frequency_MHz { get; set; }
        double? Bandwidth_kHz { get; set; }
        double? LevelSignal_dBm { get; set; }
        long? RefSituationId { get; set; }
        int? IcsmId { get; set; }
        string IcsmTable { get; set; }
        IReferenceSituation REFSITUATION { get; set; }
    }
}

