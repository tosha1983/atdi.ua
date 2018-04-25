using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    // <summary>
    /// Represents the action of update record
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryChangesetUpdationAction : QueryChangesetAction
    {
        [DataMember]
        public RecordReference RecordRef;
        [DataMember]
        public string[] StringValues;
        [DataMember]
        public bool?[] BooleanValues;
        [DataMember]
        public DateTime?[] DateTimeValues;
        [DataMember]
        public int?[] IntegerValues;
        [DataMember]
        public double?[] DoubleValues;
        [DataMember]
        public byte[][] BytesValues;
    }
}
