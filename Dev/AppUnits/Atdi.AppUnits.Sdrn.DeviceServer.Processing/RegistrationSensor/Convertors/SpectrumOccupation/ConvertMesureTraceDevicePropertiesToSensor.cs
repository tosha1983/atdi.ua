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
            var radioPathParameters = mesure.StandardDeviceProperties.RadioPathParameters;
            AntennaPattern[] antennaPattern = null;
            if ((radioPathParameters != null) && (radioPathParameters.Length > 0))
            {
                antennaPattern = new AntennaPattern[radioPathParameters.Length];
                for (int i = 0; i < radioPathParameters.Length; i++)
                {
                    antennaPattern[i] = new AntennaPattern();
                    antennaPattern[i].DiagA = radioPathParameters[i].DiagA;
                    antennaPattern[i].DiagV = radioPathParameters[i].DiagV;
                    antennaPattern[i].Gain = radioPathParameters[i].Gain;
                    antennaPattern[i].Freq_MHz = (double)(radioPathParameters[i].Freq_Hz / 1000000);
                }
            }
            var sensor = new Sensor()
            {
                Status = "NOT_CONFIRMED",
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
