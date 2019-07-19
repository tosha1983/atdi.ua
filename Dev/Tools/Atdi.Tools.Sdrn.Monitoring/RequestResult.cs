using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Tools.Sdrn.Monitoring
{
    public class RequestResult
    {
    }
    public class FieldValueResult : RequestResult
    {
        public FieldDescriptor Field { get; set; }
        public object Value { get; set; }
    }
    public class FieldDescriptor
    {
        public int Index { get; set; }
        public string Path { get; set; }
        public DataTypeMetadata Type { get; set; }
    }
    public class RecordResult : RequestResult
    {
        public FieldDescriptor[] Fields { get; set; }
        public object[] Record { get; set; }
    }
    public class DataSetResult : RequestResult
    {
        public FieldDescriptor[] Fields { get; set; }
        public object[] Records { get; set; }
    }
    public class DataTypeMetadata
    {
        public string Name { get; }
        public int VarTypeCode { get; }
        public string VarTypeName { get; }
        public string ClrType { get; }
        public long? Length { get; }
        public int? Precision { get; }
        public int? Scale { get; }
        public bool Autonum { get; }
    }

}
