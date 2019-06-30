using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{

    [Serializable]
    public class DataTypeMetadata : IDataTypeMetadata
    {
        public string Name { get; set; }

        public DataSourceType DataSourceType { get; set; }

        public DataType CodeVarType { get; set; }

        public DataSourceVarType SourceVarType { get; set; }

        public long? Length { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }

        public IAutonumMetadata Autonum { get; set; }

        public Type CodeVarClrType { get; set; }

        public bool Multiple { get; set; }

        public StoreContentType ContentType { get; set; }

        public DataType SourceCodeVarType { get; set; }

        public override string ToString()
        {
            // DataBase.Array.Integer: ClrType, BLOB
            return $"{this.DataSourceType}.{this.Name}: Code = {CodeVarType}, Store = {SourceVarType}";
        }
    }

}
