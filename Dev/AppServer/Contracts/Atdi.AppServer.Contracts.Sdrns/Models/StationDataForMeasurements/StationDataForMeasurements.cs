using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    [KnownType(typeof(StationDataForMeasurementsExtend))]
    [KnownType(typeof(StationDataForMeasurements))]
    [KnownType(typeof(SiteStationForMeas))]
    [KnownType(typeof(SectorStationForMeas))]
    [KnownType(typeof(PermissionForAssignment))]
    [KnownType(typeof(FrequencyForSectorFormICSM))]
    [KnownType(typeof(MaskElements))]
    public class StationDataForMeasurementsExtend : StationDataForMeasurements // этот класс необходим для передачи данных станций для которых производиться обмер
    {
        [DataMember]
        public int IdOwner;
        [DataMember]
        public int IdSite;

    }
}
