using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
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
    }
}
