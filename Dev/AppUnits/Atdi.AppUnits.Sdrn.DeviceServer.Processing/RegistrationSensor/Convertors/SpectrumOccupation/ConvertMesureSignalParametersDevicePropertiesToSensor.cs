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
    public static class ConvertMesureSignalParametersDevicePropertiesToSensor
    {
        public static Sensor Convert(this MesureSignalParametersDeviceProperties mesure, string sensorName, string sensorTechId)
        {
            var sensor = new Sensor();
            sensor.Name = sensorName;

            return sensor;
        }
    }
}
