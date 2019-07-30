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
    public static class ConvertMesureDFDevicePropertiesToSensor
    {
        public static Sensor Convert(this MesureDFDeviceProperties mesure, string sensorName, string sensorTechId)
        {
            var sensor = new Sensor();
            sensor.Name = sensorName;
            sensor.Equipment = new SensorEquipment();
            sensor.Equipment.TechId = sensorTechId;
            return sensor;
        }
    }
}
