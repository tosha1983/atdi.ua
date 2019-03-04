using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public static class ConvertMesureZeroSpanDevicePropertiesToSensor
    {
        public static Sensor Convert(this MesureZeroSpanDeviceProperties mesure, string sensorName)
        {
            var sensor = new Sensor();
            sensor.Name = sensorName;

            return sensor;
        }
    }
}
