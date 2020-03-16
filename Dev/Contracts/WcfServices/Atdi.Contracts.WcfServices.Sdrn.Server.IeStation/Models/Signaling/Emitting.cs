using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    [DataContract(Namespace = Specification.Namespace)]
    public class Emitting
    {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public double StartFrequency_MHz { get; set; }

        [DataMember]
        public double StopFrequency_MHz
        { get; set; }

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
        public string SensorName { get; set; }

        [DataMember]
        public string SensorTechId { get; set; }

        [DataMember]
        public String AssociatedStationTableName { get; set; }

        [DataMember]
        public long AssociatedStationID { get; set; }

        [DataMember]
        public long MeasResultId { get; set; }
    }
}
