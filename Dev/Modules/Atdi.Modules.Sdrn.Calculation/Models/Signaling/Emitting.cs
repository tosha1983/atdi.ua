using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.Modules.Sdrn.Calculation
{
    /// <summary>
    /// Базовый класс для обработки излучений.
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class Emitting
    {
        [DataMember]
        public long? Id { get; set; }
        [DataMember]
        public double StartFrequency_MHz { get; set; }
        [DataMember]
        public double StopFrequency_MHz { get; set; }
        [DataMember]
        public double CurentPower_dBm { get; set; }
        [DataMember]
        public double ReferenceLevel_dBm { get; set; }
        [DataMember]
        public double MeanDeviationFromReference { get; set; } // отклонение формы от эталонной в долях от 0 до 1
        [DataMember]
        public double TriggerDeviationFromReference { get; set; } // максимально допустимое отклонение формы от эталонной в долях от 0 до 1
        [DataMember]
        public WorkTime[] WorkTimes { get; set; }
        [DataMember]
        public SignalMask SignalMask { get; set; }
        [DataMember]
        public Spectrum Spectrum { get; set; }
        [DataMember]
        public LevelsDistribution LevelsDistribution { get; set; }
        [DataMember]
        public EmittingParameters EmittingParameters { get; set; }
        [DataMember]
        public int? SensorId { get; set; }
        [DataMember]
        public DateTime LastDetaileMeas { get; set; } // Время последнего детального измерения
        [DataMember]
        public bool SpectrumIsDetailed; // Флаг было ли произведено детальное измерение Если было произведено детальное измерение следовательно оно в приоретете.
        [DataMember]
        public SignalingSysInfo[] SysInfos { get; set; }
        [DataMember]
        public ReferenceLevels ReferenceLevels  { get; set; }
    }
}
