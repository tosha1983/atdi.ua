using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.EN
{
    public enum Status
    {
        // Configuration Errors
        InvalidModeErr = -112,
        ReferenceLevelErr = -111,
        InvalidVideoUnitsErr = -110,
        InvalidWindowErr = -109,
        InvalidBandwidthTypeErr = -108,
        InvalidSweepTimeErr = -107,
        BandwidthErr = -106,
        InvalidGainErr = -105,
        AttenuationErr = -104,
        FrequencyRangeErr = -103,
        InvalidSpanErr = -102,
        InvalidScaleErr = -101,
        InvalidDetectorErr = -100,

        // General Errors
        LibusbError = -18,
        NotSupportedErr = -17,
        TrackingGeneratorNotFound = -16,
        USBTimeoutErr = -15,
        DeviceConnectionErr = -14,
        PacketFramingErr = -13,
        GPSErr = -12,
        GainNotSetErr = -11,
        DeviceNotIdleErr = -10,
        DeviceInvalidErr = -9,
        BufferTooSmallErr = -8,
        NullPtrErr = -7,
        AllocationLimitErr = -6,
        DeviceAlreadyStreamingErr = -5,
        InvalidParameterErr = -4,
        DeviceNotConfiguredErr = -3,
        DeviceNotStreamingErr = -2,
        DeviceNotOpenErr = -1,

        // No Error
        NoError = 0,

        // Warnings/Messages
        AdjustedParameter = 1,
        ADCOverflow = 2,
        NoTriggerFound = 3,
        ClampedToUpperLimit = 4,
        ClampedToLowerLimit = 5,
        UncalibratedDevice = 6,
        DataBreak = 7,
        UncalSweep = 8
    }
}
