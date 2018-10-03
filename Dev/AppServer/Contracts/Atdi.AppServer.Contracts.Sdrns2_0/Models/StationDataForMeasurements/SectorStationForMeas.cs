using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{
    
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    [KnownType(typeof(SectorStationForMeas))]
    [KnownType(typeof(FrequencyForSectorFormICSM))]
    [KnownType(typeof(MaskElements))]

    public class SectorStationForMeas // параметры секторов
    {
        [DataMember]
        public double? AGL;//м, высота над уровнем земли
        [DataMember]
        public int IdSector; //В ICSM
        [DataMember]
        public double? EIRP;//дБм
        [DataMember]
        public double? Azimut;//град
        [DataMember]
        public double? BW; //kHz
        [DataMember]
        public string ClassEmission;
        [DataMember]
        public FrequencyForSectorFormICSM[] Frequencies;
        [DataMember]
        public MaskElements[] MaskBW;
    }
}
