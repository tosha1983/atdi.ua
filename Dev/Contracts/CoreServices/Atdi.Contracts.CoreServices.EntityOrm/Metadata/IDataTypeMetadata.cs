using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{

    public interface IDataTypeMetadata
    {
        string Name { get; }

        DataSourceType DataSourceType { get; }

        DataType CodeVarType { get; }

        Type CodeVarClrType { get; }

        DataSourceVarType SourceVarType { get; }

        int? Length { get; }

        int? Precision { get; }

        int? Scale { get; }

        IAutonumMetadata Autonum { get; }

        bool Multiple { get; }

    }

}
