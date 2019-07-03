using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IReferenceSignal_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IReferenceSignal : IReferenceSignal_PK
    {
        double? Frequency_MHz { get; set; }
        double? Bandwidth_kHz { get; set; }
        double? LevelSignal_dBm { get; set; }
        int? IcsmId { get; set; }
        string IcsmTable { get; set; }
        float[] Loss_dB { get; set; }
        double[] Freq_kHz { get; set; }
        IReferenceSituation REFERENCE_SITUATION { get; set; }
    }
}

