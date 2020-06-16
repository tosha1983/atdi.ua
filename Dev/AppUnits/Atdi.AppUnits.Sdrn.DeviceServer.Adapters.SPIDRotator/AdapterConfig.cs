using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.SPIDRotator
{
    public class AdapterConfig
    {
        public int PortNumber { get; set; }
        public bool ElevationIsPolarization { get; set; }

        public float AzimuthMin_dg { get; set; }
        public float AzimuthMax_dg { get; set; }
        public float ElevationMin_dg { get; set; }//т.к. две оси то эту можем использовать как ось поляризации, но настройщик вносит данные как видит
        public float ElevationMax_dg { get; set; }//т.к. две оси то эту можем использовать как ось поляризации, но настройщик вносит данные как видит

        public string ControlDeviceManufacturer { get; set; }
        public string ControlDeviceName { get; set; }
        public string ControlDeviceCode { get; set; }
        public string RotationDeviceManufacturer { get; set; }
        public string RotationDeviceName { get; set; }
        public string RotationDeviceCode { get; set; }
    }
}
