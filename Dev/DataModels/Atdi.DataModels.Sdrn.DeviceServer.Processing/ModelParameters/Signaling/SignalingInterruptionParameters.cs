using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    [Serializable]
    public class SignalingInterruptionParameters
    {

        // Первичное выделение излучения
        public int? NumberPointForChangeExcess { get; set; } //минимальное количество точек для нахождения излучения Необходимо для исключения случайных выбросов спектров по умолчанию 10
        public double? windowBW { get; set; } // характеризует размер окна для обработки и хранения спектра излучения по умолчанию 1.1

        // Определение параметров излучения
        public double? DiffLevelForCalcBW { get; set; } // используется для нахождения BW методом ndBDown по умолчанию 25
        public double? nDbLevel_dB { get; set; } //уровень ndBDown для поиска конца излучения метод используется если нельзя определить BW класическим методом по умолчанию 25
        public int? NumberIgnoredPoints { get; set; } // количество точек которые игнорируются при поиске концов излучения метод используется если нельзя определить BW класическим методом по умолчанию 1
        public double? MinExcessNoseLevel_dB { get; set; } // минимально допустимое превышение уровня шума максимумом излучения метод используется если нельзя определить BW класическим методом по умолчанию 5

        // разделение сигналов при обычном сканировании
        public bool? AutoDivisionEmitting { get; set; } // автоматическое разделение сигналов 
        public double? DifferenceMaxMax { get; set; } // разница между максимумом и минимумом для разделения сигналов.

        public double? MaxFreqDeviation { get; set; }
        public bool? CheckLevelChannel { get; set; }
        public int? MinPointForDetailBW { get; set; }
    }
}
