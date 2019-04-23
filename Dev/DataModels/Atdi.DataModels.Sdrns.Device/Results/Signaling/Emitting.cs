using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Базовый класс для обработки излучений.
    /// </summary>
    public class Emitting
    {
        public double StartFrequency_MHz;
        public double StopFrequency_MHz;
        public double CurentPower_dBm;
        public double ReferenceLevel_dBm;
        public double MeanDeviationFromReference; // отклонение формы от эталонной в долях от 0 до 1
        public double TriggerDeviationFromReference; // максимально допустимое отклонение формы от эталонной в долях от 0 до 1
        public WorkTime[] WorkTimes;
        public SignalMask SignalMask;
        public Spectrum Spectrum;
        public LevelsDistribution LevelsDistribution;
        public EmittingParameters EmittingParameters;
        public int? SensorId;
        public DateTime LastDetaileMeas; // Время последнего детального измерения
        public bool SpectrumIsDetailed; // Флаг было ли произведено детальное измерение Если было произведено детальное измерение следовательно оно в приоретете.
    }
}
