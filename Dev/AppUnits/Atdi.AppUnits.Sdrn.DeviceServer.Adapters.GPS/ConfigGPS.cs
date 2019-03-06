using System;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS
{
    public class ConfigGPS
    {
        public string PortName { get; set; }
        public string PortBaudRate { get; set; }
        public string PortDataBits { get; set; }
        public string PortStopBits { get; set; }
        public string PortHandshake { get; set; }
        public string PortParity { get; set; }
    }
}
