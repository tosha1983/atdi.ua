using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{

    [DataContract(Namespace = ServicesSpecification.Namespace)]
    [KnownType(typeof(LevelMeasurementsCar))]
    [KnownType(typeof(MeasurementsParameterGeneral))]
    public class ResultsMeasurementsStation // этот класс необходим для передачи данных станций для которых производиться обмер
    {
        [DataMember]
        public int? Idstation;
        [DataMember]
        public int? IdSector;
        [DataMember]
        public string Status;
        [DataMember]
        public string GlobalSID; // из ICSM 
        [DataMember]
        public string MeasGlobalSID; // измеренный 
        [DataMember]
        public LevelMeasurementsCar[] LevelMeasurements;
        [DataMember]
        public MeasurementsParameterGeneral GeneralResult;
    }
}

