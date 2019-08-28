using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.DataModels.Sdrns.Server
{

    [Serializable]
    [DataContract(Namespace = Specification.Namespace)]
    [KnownType(typeof(StationDataForMeasurements))]
    [KnownType(typeof(SiteStationForMeas))]
    [KnownType(typeof(SectorStationForMeas))]
    [KnownType(typeof(PermissionForAssignment))]
    [KnownType(typeof(FrequencyForSectorFormICSM))]
    [KnownType(typeof(MaskElements))]
    public class StationDataForMeasurements  // этот класс необходим для передачи данных станций для которых производиться обмер
    {
        [DataMember]
        public OwnerData Owner;
        [DataMember]
        public long IdStation; // Идентификатор в ICSM
        [DataMember]
        public string GlobalSID;
        [DataMember]
        public SiteStationForMeas Site;
        [DataMember]
        public SectorStationForMeas[] Sectors;
        [DataMember]
        public string Status;
        [DataMember]
        public string Standart;
        [DataMember]
        public PermissionForAssignment LicenseParameter;
        [DataMember]
        public long IdOwner;
        [DataMember]
        public long IdSite;
    }
}
