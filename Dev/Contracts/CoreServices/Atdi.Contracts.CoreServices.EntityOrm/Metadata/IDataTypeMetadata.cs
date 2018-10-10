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
        string Name { get; set; }

        DataSourceType DataSourceType { get; set; }

        DataType CodeVarType { get; set; }

        string SourceVarType { get; set; }

        int? Length { get; set; }

        int? Precision { get; set; }

        int? Scale { get; set; }

        IAutonumMetadata Autonum { get; set; }

    }

}
