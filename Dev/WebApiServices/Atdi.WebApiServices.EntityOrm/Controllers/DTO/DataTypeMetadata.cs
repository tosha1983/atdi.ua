using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = Atdi.DataModels.EntityOrm.Api;

namespace Atdi.WebApiServices.EntityOrm.Controllers.DTO
{
    public class DataTypeMetadata : API.IDataTypeMetadata
    {
        public DataTypeMetadata(IDataTypeMetadata dataType)
        {
            if (dataType == null)
            {
                throw new ArgumentNullException(nameof(dataType));
            }

            this.Name = dataType.Name;
            this.Autonum = dataType.Autonum != null;
            this.Length = dataType.Length;
            this.Precision = dataType.Precision;
            this.Scale = dataType.Scale;
            this.ClrType = dataType.CodeVarClrType?.Name;
            this.VarTypeCode = (int)dataType.CodeVarType;
            this.VarTypeName = dataType.CodeVarType.ToString();
        }

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
