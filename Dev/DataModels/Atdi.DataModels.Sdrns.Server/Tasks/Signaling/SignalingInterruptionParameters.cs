using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.DataModels.Sdrns.Server
{ 
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SignalingInterruptionParameters
    {
        // Первичное выделение излучения
        [DataMember]
        public int? NumberPointForChangeExcess; //минимальное количество точек для нахождения излучения Необходимо для исключения случайных выбросов спектров по умолчанию 10
        [DataMember]
        public double? windowBW; // характеризует размер окна для обработки и хранения спектра излучения по умолчанию 1.1

        // Определение параметров излучения
        [DataMember]
        public double? DiffLevelForCalcBW; // используется для нахождения BW методом ndBDown по умолчанию 25
        [DataMember]
        public double? nDbLevel_dB; //уровень ndBDown для поиска конца излучения метод используется если нельзя определить BW класическим методом по умолчанию 25
        [DataMember]
        public int? NumberIgnoredPoints; // количество точек которые игнорируются при поиске концов излучения метод используется если нельзя определить BW класическим методом по умолчанию 1
        [DataMember]
        public double? MinExcessNoseLevel_dB; // минимально допустимое превышение уровня шума максимумом излучения метод используется если нельзя определить BW класическим методом по умолчанию 5

        // разделение сигналов при обычном сканировании
        [DataMember]
        public bool? AutoDivisionEmitting; // автоматическое разделение сигналов 
        [DataMember]
        public double? DifferenceMaxMax; // разница между максимумом и минимумом для разделения сигналов.

    }
}
