using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm
{
    public interface IEntityOrm
    {
        IEntityMetadata GetEntityMetadata(string entityName);

        IDataTypeMetadata GetDataTypeMetadata(string dataTypeName);

        IUnitMetadata GetUnitMetadata(string unitName);
    }
}
