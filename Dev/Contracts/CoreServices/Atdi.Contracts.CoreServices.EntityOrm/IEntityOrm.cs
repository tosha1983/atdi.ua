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

        IEntityMetadata GetEntityMetadata(string entityPath, IEntityMetadata relatedEntity = null);

        IDataTypeMetadata GetDataTypeMetadata(string dataTypeName, DataSourceType dataSourceType);

        IUnitMetadata GetUnitMetadata(string unitName);

        object CreatePrimaryKeyInstance(IEntityMetadata entity);

    }

    public static class EntityOrmExtensions
    {
        public static IEntityMetadata GetEntityMetadata<TModel>(this IEntityOrm entityOrm)
        {
            var modelTypeName = typeof(TModel).Name;
            var ns = typeof(TModel).Namespace;
            var entityName = (modelTypeName[0] == 'I' ? modelTypeName.Substring(1, modelTypeName.Length - 1) : modelTypeName);
            return entityOrm.GetEntityMetadata(ns + "." + entityName);
        }
    }
}
