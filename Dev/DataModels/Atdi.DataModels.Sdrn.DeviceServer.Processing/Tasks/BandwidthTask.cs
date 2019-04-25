using Atdi.DataModels.Sdrns.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    public class BandWidthTask : MeasurementTaskBase
    {
        public string bandwidthEstimationType;
        public double X_Beta;
        public int MaximumIgnorPoint;
        public BWResult MeasBWResults; //  результат измерения
        public long maximumTimeForWaitingResultBandWidth; // (максимальное время ожидания результата)
        public DateTime? LastTimeSend = null;
    }
}
