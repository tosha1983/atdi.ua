using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public class RotatorDeviceProperties : DevicePropertiesBase
    {
        //по скоростям логика такая если есть одно значение то можете и не заморачиваться и в интерфейсе тоже не отображать
        //Если будит больше одного значения то будит выбираться то что передали, если недоступное передали то выбирется самое медленное для сохранения ресурса железа
        //фактически значение ничего не означает(не имеет связи с реальностью т.е. это не гр/с)
        public int[] AzimuthSpeedAvailable;//доступные скорости поворота, если массив пустой то это значат что такая ось недоступна!
        public int[] ElevationSpeedAvailable;//доступные скорости поворота, если массив пустой то это значат что такая ось недоступна!
        public int[] PolarizationSpeedAvailable;//доступные скорости поворота, если массив пустой то это значат что такая ось недоступна!

        public float AzimuthStep_dg;//доступный минимальный шаг поворота в градусах, если 0 то это значат что такая ось недоступна!
        public float ElevationStep_dg;//доступный минимальный шаг поворота в градусах, если 0 то это значат что такая ось недоступна!
        public float PolarizationStep_dg;//доступный минимальный шаг поворота в градусах, если 0й то это значат что такая ось недоступна!

        public float AzimuthMin_dg;//минимальный угл в градусах включительно.
        public float AzimuthMax_dg;//максимальный угл в градусах включительно.
        public float ElevationMin_dg;//минимальный угл в градусах включительно.
        public float ElevationMax_dg;//максимальный угл в градусах включительно.
        public float PolarizationMin_dg;//минимальный угл в градусах включительно.
        public float PolarizationMax_dg;//максимальный угл в градусах включительно.

        public string ControlDeviceManufacturer;
        public string ControlDeviceName;
        public string ControlDeviceCode;
        public string RotationDeviceManufacturer;
        public string RotationDeviceName;
        public string RotationDeviceCode;
    }
}
