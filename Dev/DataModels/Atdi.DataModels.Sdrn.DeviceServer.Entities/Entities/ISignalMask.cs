using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    public interface ISignalMask_PK
    {
       long? Id { get; set; }
    }

        [Entity]
    public interface ISignalMask : ISignalMask_PK
    {
        double? Loss_dB { get; set; }
        double? Freq_kHz { get; set; }
        long? ReferenceSignalId { get; set; }
        IReferenceSignal REFSIGNAL { get; set; }
    }
}

