using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// Represents frequency for measurement 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class SOFrequency
    {
        /// <summary>
        /// номинальная частота
        /// </summary>
        [DataMember]
        public double? Frequency_MHz;
        /// <summary>
        /// количество превышений тригерных уровней
        /// </summary>
        [DataMember]
        public int? hit;
        /// <summary>
        /// занятость
        /// </summary>
        [DataMember]
        public double? Occupation;
        /// <summary>
        /// идентификаторы станций 
        /// </summary>
        [DataMember]
        public string StantionIDs;
        /// <summary>
        /// количество обнаруженных станций на частоте
        /// </summary>
        [DataMember]
        public int? countStation;
        /// <summary>
        /// занятость по часам
        /// </summary>
        [DataMember]
        public string OccupationByHuors;  
    }
}

