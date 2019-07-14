using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IDataTypeMetadata
    {
        string Name { get; }

        int VarTypeCode { get; }

        string VarTypeName { get; }

        string ClrType { get; }

        long? Length { get; }

        int? Precision { get; }

        int? Scale { get; }

        bool Autonum { get; }
    }
}
