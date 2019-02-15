using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [DataContract(Namespace = Specification.Namespace)]
    public class LevelMeasurementsCar
    {
        [DataMember]
        public double? Lon;
        [DataMember]
        public double? Lat; //DEC координата в которой происходило измерение 
        [DataMember]
        public double? Altitude; //м координата в которой происходило измерение 
        [DataMember]
        public double? LeveldBm; // уровень измеренного сигнала в полосе канала 
        [DataMember]
        public double? LeveldBmkVm; // уровень измеренного сигнала в полосе канала 
        [DataMember]
        public DateTime TimeOfMeasurements; // время когда был получен результат 
        [DataMember]
        public double? DifferenceTimestamp; // наносекунды 10^-9 Разсинхронизация с GPS
        [DataMember]
        public decimal? CentralFrequency; // 
        [DataMember]
        public double? BW; // кГц;
        [DataMember]
        public double? RBW; // кГц;
        [DataMember]
        public double? VBW; // кГц;
    }
}
