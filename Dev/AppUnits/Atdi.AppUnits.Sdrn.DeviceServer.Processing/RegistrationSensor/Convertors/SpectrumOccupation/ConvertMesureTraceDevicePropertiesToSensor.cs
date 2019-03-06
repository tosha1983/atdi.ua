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
    public static class ConvertMesureTraceDevicePropertiesToSensor
    {
        public static Sensor Convert(this MesureTraceDeviceProperties mesure, string sensorName, string sensorTechId)
        {
            var equipmentValue = mesure.StandardDeviceProperties.EquipmentInfo;
            var sensor = new Sensor()
            {
                Status = "A",
                Name = sensorName,
                Equipment = new SensorEquipment()
                {
                    Code = equipmentValue.EquipmentCode,
                    Name = equipmentValue.EquipmentName,
                    Manufacturer = equipmentValue.EquipmentManufacturer,
                    Family = equipmentValue.EquipmentFamily,
                    TechId = sensorTechId
                },
                Antenna = new SensorAntenna()
                {
                    Manufacturer = equipmentValue.AntennaManufacturer,
                    Code = equipmentValue.AntennaCode,
                    Name = equipmentValue.AntennaName
                }
            };
            return sensor;
        }
    }
}
