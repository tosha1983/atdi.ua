using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the action with contains into the changeset of the web query
    /// </summary>
    [DataContract]
    public class DataChangeAction
    {
        // <summary>
        /// The ID of the action
        /// </summary>
        [DataMember]
        public Guid ActionId { get; set; }

        /// <summary>
        /// The type of the action
        /// </summary>
        [DataMember]
        public ActionType Type { get; set; }

        [DataMember]
        public DataSetColumn[] Columns { get; set; }

        [DataMember]
        public Filter Filter { get; set; }

        [DataMember]
        public DataRowType RowType { get; set; }

        [DataMember]
        public ObjectDataRow ObjectRow { get; set; }

        [DataMember]
        public StringDataRow StringRow { get; set; }

        [DataMember]
        public TypedDataRow TypedRow { get; set; }


       
    }
}
