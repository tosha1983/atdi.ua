using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    /// <summary>
    /// Параметры частоты сектора станции 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class SectorFrequency
    {
        /// <summary>
        /// Идентификатор частоты
        /// </summary>
        [DataMember]
        public int? Id; // 

        /// <summary>
        /// Идетификатор частотного плана
        /// </summary>
        [DataMember]
        public int? PlanId; // 

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? ChannelNumber;

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal? Frequency_MHz; //МГц;
    }
}
