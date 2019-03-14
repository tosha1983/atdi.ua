using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class SignalizationTask : TaskBase
    {
        public MeasResults MeasResults; //  результат измерения
        public DateTime? LastTimeSend = null;
        public MesureTraceParameter mesureTraceParameter;
        public TaskParameters taskParameters;
        public ReferenceLevels ReferenceLevels;
        public Emitting[] EmittingsRaw;
        public Emitting[] EmittingsDetailed;
        public Emitting[] EmittingsSummary;
    }
}
