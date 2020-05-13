using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server.IeStation
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace =Specification.Namespace)]
    public class HeadProtocols
    {
        [DataMember]
        public long? Id { get; set; }
        /// <summary>
        /// Дата створення 
        /// </summary>
        [DataMember]
        public DateTime? DateCreated { get; set; }
        /// <summary>
        /// Хто створив
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Стандарт
        /// </summary>
        [DataMember]
        public string StandardName { get; set; }
        /// <summary>
        /// Означення станції, фактично
        /// </summary>
        [DataMember]
        public string GlobalSID { get; set; }
        /// <summary>
        /// Означення станції (заявлено)
        /// </summary>
        [DataMember]
        public string PermissionGlobalSID { get; set; }
        /// <summary>
        /// Дата вимірювання
        /// </summary>
        [DataMember]
        public DateTime? DateMeas { get; set; }
        /// <summary>
        /// Власник
        /// </summary>
        [DataMember]
        public string OwnerName { get; set; }
        /// <summary>
        /// Адрес РЕЗ
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// Довгота
        /// </summary>
        [DataMember]
        public double? Longitude { get; set; }
        /// <summary>
        /// Широта
        /// </summary>
        [DataMember]
        public double? Latitude { get; set; }
        /// <summary>
        /// № дозволу на експлуатацію
        /// </summary>
        [DataMember]
        public string PermissionNumber { get; set; }
        /// <summary>
        /// Термін дії дозволу (Дата початку)
        /// </summary>
        [DataMember]
        public DateTime? PermissionStart { get; set; }
        /// <summary>
        /// Термін дії дозволу (Дата закінчення)
        /// </summary>
        [DataMember]
        public DateTime? PermissionStop { get; set; }
        /// <summary>
        /// Дата використання плану (пусте)
        /// </summary>
        [DataMember]
        public DateTime? SpecialDate { get; set; }
        /// <summary>
        /// Назва сенсору
        /// </summary>
        [DataMember]
        public string TitleSensor { get; set; }
        /// <summary>
        /// Результат радіоконтролю для станции
        /// </summary>
        [DataMember]
        public string StatusMeasStation { get; set; }
        /// <summary>
        /// Назва сенсору
        /// </summary>
        [DataMember]
        public DetailProtocols[] DetailProtocols { get; set; }

    }
}
