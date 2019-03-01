using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface IDeviceProperties
    {
        Guid DeviceId { get; set; }
    }

    public class DevicePropertiesBase : IDeviceProperties
    {
        public decimal FreqMin_Hz;
        public decimal FreqMax_Hz;
        public int AttMin_dB;
        public int AttMax_dB;
        public int PreAmpMin_dB;
        public int PreAmpMax_dB;
        public int RefLevelMin_dBm;
        public int RefLevelMax_dBm;
        public RadioPathParameters[] RadioPathParameters; // в зависимости от частоты
        public EquipmentInfo EquipmentInfo;

        public Guid DeviceId { get; set; }
    }
    public class RadioPathParameters
    {
        public decimal Freq_Hz;
        public double KTBF_dBm;
        public double FeederLoss_dB;
        public double Gain;
        public string DiagA;
        public string DiagH;
        public string DiagV;
    }
    public class EquipmentInfo
    {
        public string EquipmentManufacturer;
        public string EquipmentName;
        public string EquipmentFamily;
        public string EquipmentCode;
        public string AntennaManufacturer;
        public string AntennaName;
        public string AntennaCode;
    }
}
