using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class SOTask : TaskBase
    {
        public MeasResults MeasResults; //  результат измерения
        public LastResultParameters lastResultParameters;
        public DateTime? LastTimeSend = null;
        public int CountMeasurementDone = 0;
        public MesureTraceParameter mesureTraceParameter;
        public TaskParameters taskParameters;
        public SensorParameters sensorParameters;
    }
}
