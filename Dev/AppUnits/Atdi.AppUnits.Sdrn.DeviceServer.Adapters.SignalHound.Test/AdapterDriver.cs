using System;
using System.Runtime.InteropServices;
using EN = Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Enums;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SignalHound.Test
{
    public static class AdapterDriver
    {        
        public static string bbGetDeviceName(int device)
        {
            int device_type = -1;
            return "BB60C";
        }

        public static string bbGetSerialString(int device)
        {
            return "serial_number-332425363";
        }

        public static string bbGetFirmwareString(int device)
        {
            return "firmware_version";
        }

        public static string bbGetAPIString()
        {
            return "bbGetAPIString";
        }

        public static string bbGetStatusString(EN.Status status)
        {
            return "bbGetStatusString";
        }

    }
}
