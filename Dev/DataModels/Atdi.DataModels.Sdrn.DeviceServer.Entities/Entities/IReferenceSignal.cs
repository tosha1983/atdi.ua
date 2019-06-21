using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    public interface IReferenceSignal_PK
    {
        long? Id { get; set; }
    }

    [Entity]
    public interface IReferenceSignal : IReferenceSignal_PK
    {
        double? Frequency_MHz { get; set; }
        double? Bandwidth_kHz { get; set; }
        double? LevelSignal_dBm { get; set; }
        long? RefSituationId { get; set; }
        IReferenceSituation REFSITUATION { get; set; }
    }
}

