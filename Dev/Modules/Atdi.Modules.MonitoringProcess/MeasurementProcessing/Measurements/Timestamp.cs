using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Modules.MonitoringProcess.ProcessSignal;

namespace Atdi.Modules.MonitoringProcess.MeasurementProcessing.Measurements
{
    public class Timestamp
    {
        public bool SuccessfulTimestamp = false;
        IQStreamTimeStampBloks IQStreamTimeStampBloks;
        public Timestamp(ISDR SDR, TaskParameters taskParameters)
        {
            ReceivedIQStream receivedIQStream = new ReceivedIQStream();
            bool done = SDR.GetIQStream(ref receivedIQStream, taskParameters.ReceivedIQStreemDuration_sec);
            if (done == false)
            {
                SuccessfulTimestamp = false; return;
            }
            else
            {
                SuccessfulTimestamp = true;
            }
            GetTimeStamp TimeStamp = new GetTimeStamp(receivedIQStream, 40000000, 1000 * (taskParameters.MaxFreq_MHz - taskParameters.MinFreq_MHz), taskParameters.TypeTechnology);
            IQStreamTimeStampBloks = TimeStamp.IQStreamTimeStampBloks;
        }
    }
}
