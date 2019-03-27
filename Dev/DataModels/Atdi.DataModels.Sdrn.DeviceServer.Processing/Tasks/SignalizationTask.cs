using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class SignalizationTask : MeasurementTaskBase
    {
        public MeasResults MeasResults; //  результат измерения
        public DateTime? LastTimeSend = null;
        public MesureTraceDeviceProperties mesureTraceDeviceProperties;
        public ReferenceLevels ReferenceLevels;
        public Emitting[] EmittingsRaw;
        public Emitting[] EmittingsDetailed;
        public Emitting[] EmittingsSummary;
        public long maximumTimeForWaitingResultSignalization; // (максимальное время ожидания результата)
    }
}
