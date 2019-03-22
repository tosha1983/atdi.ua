using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISignalMask
    {
        int Id { get; set; }
        double? Loss_dB { get; set; }
        double? Freq_kHz { get; set; }
        int? ReferenceSignalId { get; set; }
        int? EmittingId { get; set; }
        IReferenceSignal REFSIGNAL { get; set; }
        IEmitting EMITTING { get; set; }
    }
}

