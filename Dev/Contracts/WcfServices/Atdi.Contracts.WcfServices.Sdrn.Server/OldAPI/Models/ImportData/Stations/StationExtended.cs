using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    ///  Station Extended
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class StationExtended
    {
        /// <summary>
        /// Идентификатор записи в БД SDRN
        /// </summary>
        [DataMember]
        public long? Id { get; set; }
        /// <summary>
        /// MOB_STATION, или MOB_STATION2
        /// </summary>
        [DataMember]
        public string TableName { get; set; }

        /// <summary>
        /// Идентификатор таблицы
        /// </summary>
        [DataMember]
        public int TableId { get; set; }

        /// <summary>
        /// MOB_STATION.STANDARD
        /// </summary>
        [DataMember]
        public string Standard { get; set; }


        /// <summary>
        /// (RADIO_SYSTEMS.DESCRIPTION взять на основании совпадения MOB_STATION.STANDARD = RADIO_SYSTEMS.NAME)
        /// </summary>
        [DataMember]
        public string StandardName { get; set; }

        /// <summary>
        /// MOB_STATION.Owner.NAME
        /// </summary>
        [DataMember]
        public string OwnerName { get; set; }

        /// <summary>
        /// ALLSTATIONS.PERM_NUM
        /// </summary>
        [DataMember]
        public string PermissionNumber { get; set; }

        /// <summary>
        /// ALLSTATIONS.PERM_DATE
        /// </summary>
        [DataMember]
        public DateTime? PermissionStart { get; set; }

        /// <summary>
        /// ALLSTATIONS.PERM_DATE_STOP
        /// </summary>
        [DataMember]
        public DateTime? PermissionStop { get; set; }

        /// <summary>
        /// MOB_STATION.Position.ADDRESS
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// MOB_STATION.Position.LONGITUDE, MOB_STATION.Position.LATITUDE
        /// </summary>
        [DataMember]
        public DataLocation Location { get; set; }

        /// <summary>
        /// MOB_STATION.BW
        /// </summary>
        [DataMember]
        public double? BandWidth { get; set; }

        /// <summary>
        /// MOB_STATION.DESIG_EMISSION
        /// </summary>
        [DataMember]
        public string DesigEmission { get; set; }

        /// <summary>
        /// MOB_STATION.Position.PROVINCE
        /// </summary>
        [DataMember]
        public string Province { get; set; }
    }
}
