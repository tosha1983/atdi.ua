using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SignalingInterruptionParameters
    {

        // Первичное выделение излучения
        [DataMember]
        public int? NumberPointForChangeExcess { get; set; } //минимальное количество точек для нахождения излучения Необходимо для исключения случайных выбросов спектров по умолчанию 10
        [DataMember]
        public double? windowBW { get; set; } // характеризует размер окна для обработки и хранения спектра излучения по умолчанию 1.1

        // Определение параметров излучения
        [DataMember]
        public double? DiffLevelForCalcBW { get; set; } // используется для нахождения BW методом ndBDown по умолчанию 25
        [DataMember]
        public double? nDbLevel_dB { get; set; } //уровень ndBDown для поиска конца излучения метод используется если нельзя определить BW класическим методом по умолчанию 25
        [DataMember]
        public int? NumberIgnoredPoints { get; set; } // количество точек которые игнорируются при поиске концов излучения метод используется если нельзя определить BW класическим методом по умолчанию 1
        [DataMember]
        public double? MinExcessNoseLevel_dB { get; set; } // минимально допустимое превышение уровня шума максимумом излучения метод используется если нельзя определить BW класическим методом по умолчанию 5

        // разделение сигналов при обычном сканировании
        [DataMember]
        public bool? AutoDivisionEmitting { get; set; } // автоматическое разделение сигналов 
        [DataMember]
        public double? DifferenceMaxMax { get; set; } // разница между максимумом и минимумом для разделения сигналов.
        [DataMember]
        public double? MaxFreqDeviation { get; set; }
        [DataMember]
        public bool? CheckLevelChannel { get; set; }
        [DataMember]
        public int? MinPointForDetailBW { get; set; }
    }
}
