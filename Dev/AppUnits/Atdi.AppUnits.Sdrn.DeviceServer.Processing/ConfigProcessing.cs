﻿using System;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class ConfigProcessing 

    {
        public int DurationWaitingEventWithTask { get; set; }
        public int MaxDurationBeforeStartTimeTask { get; set; }
        public int DurationForSendResult { get; set; }
        public int MaxTimeOutReceiveSensorRegistrationResult { get; set; }
        public int DurationWaitingRceivingGPSCoord { get; set; }
        public int maximumTimeForWaitingResultSO { get; set; }
        public int SOKoeffWaitingDevice { get; set; }
        public int LonDelta { get; set; }
        public int LatDelta { get; set; }
    }
}
