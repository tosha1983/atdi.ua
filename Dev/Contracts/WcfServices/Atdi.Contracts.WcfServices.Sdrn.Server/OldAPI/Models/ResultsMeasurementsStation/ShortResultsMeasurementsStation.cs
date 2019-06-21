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
    [KnownType(typeof(LevelMeasurementsCar))]
    [KnownType(typeof(MeasurementsParameterGeneral))]
    public class ShortResultsMeasurementsStation // этот класс необходим для передачи данных станций для которых производиться обмер
    {
        [DataMember]
        public string Idstation;
        [DataMember]
        public long? IdSector;
        [DataMember]
        public string Status;
        [DataMember]
        public string GlobalSID; // из ICSM 
        [DataMember]
        public string MeasGlobalSID; // измеренный 
        [DataMember]
        public string Standard;
        [DataMember]
        public double? CentralFrequencyMeas_MHz;
        [DataMember]
        public SiteStationForMeas[] StationLocations;
    }
}

