using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the condition of data filtering
    /// </summary>
    [DataContract]
    public class Filter
    {
        [DataMember]
        public ConditionType Type { get; set; }

        [DataMember]
        public FilterOperand LeftOperand { get; set; }

        [DataMember]
        public ConditionOperator Operator { get; set; }

        [DataMember]
        public FilterOperand RightOperand { get; set; }


        [DataMember]
        public LogicalOperator FilterOperator { get; set; }

        [DataMember]
        public Filter[] Filters { get; set; }

    }
}
