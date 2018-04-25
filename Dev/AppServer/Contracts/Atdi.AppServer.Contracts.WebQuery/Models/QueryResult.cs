using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the data of the result of the executed query
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryResult
    {
        [DataMember]
        public QueryReference QueryRef;
        [DataMember]
        public uint[] ColumnIndexMap;
        [DataMember]
        public CommonDataType[] ColumnTypeMap;
        [DataMember]
        public uint TotalRowCount;
        [DataMember]
        public uint ResultRowCount;
        [DataMember]
        public uint FirstRowIndex;
        [DataMember]
        public RecordReference[] References;
        [DataMember]
        public string[][] StringValues;
        [DataMember]
        public bool?[][] BooleanValues;
        [DataMember]
        public DateTime?[][] DateTimeValues;
        [DataMember]
        public int?[][] IntegerValues;
        [DataMember]
        public double?[][] DoubleValues;
        [DataMember]
        public byte[][][] BytesValues;
    }
}
