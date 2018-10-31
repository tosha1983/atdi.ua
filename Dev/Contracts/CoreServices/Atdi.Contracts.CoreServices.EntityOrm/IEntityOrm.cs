using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.Contracts.CoreServices.EntityOrm
{
    public interface IEntityOrm
    {
     //   IReadOnlyDictionary<IRelationFieldMetadata, string> RelationFieldMetadata { get; }
     //   IReadOnlyDictionary<IReferenceFieldMetadata, string> ReferenceFieldMetadata { get; }
     //   IReadOnlyDictionary<IExtensionFieldMetadata, string> ExtensionFieldMetadata { get; }
    //    IReadOnlyDictionary<IFieldMetadata, string> ColumnFieldMetadata { get; }

        IEntityMetadata GetEntityMetadata(string entityName);

        IDataTypeMetadata GetDataTypeMetadata(string dataTypeName, Atdi.Contracts.CoreServices.EntityOrm.Metadata.DataSourceType dataSourceType);

        IUnitMetadata GetUnitMetadata(string unitName);
    }
}
