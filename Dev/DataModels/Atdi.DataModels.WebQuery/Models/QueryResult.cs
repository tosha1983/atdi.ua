using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the data of the result of the executed query
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class QueryResult
    {
        [DataMember]
        public int[] ColumnIndexMap { get; set; }

        [DataMember]
        public DataType[] ColumnTypeMap { get; set; }

        [DataMember]
        public int RecordCount { get; set; }

        [DataMember]
        public int[] Records { get; set; }

        [DataMember]
        public string[][] StringValues { get; set; }

        [DataMember]
        public bool?[][] BooleanValues { get; set; }

        [DataMember]
        public DateTime?[][] DateTimeValues { get; set; }

        [DataMember]
        public int?[][] IntegerValues { get; set; }

        [DataMember]
        public double?[][] DoubleValues { get; set; }

        [DataMember]
        public float?[][] FloatValues { get; set; }

        [DataMember]
        public decimal?[][] DecimalValues { get; set; }

        [DataMember]
        public byte[][] ByteValues { get; set; }

        [DataMember]
        public byte[][][] BytesValues { get; set; }
    }
}
