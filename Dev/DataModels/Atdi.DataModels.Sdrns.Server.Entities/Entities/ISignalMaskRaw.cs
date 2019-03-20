using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISignalMaskRaw
    {
        int Id { get; set; }
        double? Loss_dB { get; set; }
        double? Freq_kHz { get; set; }
        int? ReferenceSignalId { get; set; }
        int? EmittingId { get; set; }
        IReferenceSignalRaw REFSIGNAL { get; set; }
        IEmittingRaw EMITTING { get; set; }
    }
}

