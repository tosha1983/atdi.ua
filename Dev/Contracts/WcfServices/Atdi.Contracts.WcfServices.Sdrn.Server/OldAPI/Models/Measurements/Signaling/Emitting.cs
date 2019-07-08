using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    public class Emitting
    {
        public long? Id;
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
        public string SensorName;
        public string SensorTechId;
        public String AssociatedStationTableName;
        public long AssociatedStationID;
        public SignalingSysInfo[] SysInfos;
    }
}
