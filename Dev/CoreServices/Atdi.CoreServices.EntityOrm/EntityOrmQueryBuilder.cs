using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.Contracts.CoreServices.EntityOrm;


namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class EntityOrmQueryBuilder : LoggedObject, IDisposable
    {
        private readonly IDataEngine _dataEngine;
        private readonly IEngineSyntax _syntax;
        private readonly ConditionParser _conditionParser;
        private readonly IEntityOrm _entityMetadata;

        public EntityOrmQueryBuilder(IDataEngine dataEngine, IEntityOrm entityMetadata, ILogger logger) : base(logger)
        {
            this._entityMetadata = entityMetadata;
            this._dataEngine = dataEngine;
            //this._syntax = dataEngine.Syntax;
            //this._conditionParser = new ConditionParser(dataEngine.Syntax);
            logger.Debug(Contexts.LegacyServicesIcsm, Categories.CreatingInstance, Events.CreatedInstanceOfQueryBuilder);
        }

        private void BuildSelectStatement<TModel>(QuerySelectStatement<TModel> statement, string[] fields, out List<QuerySelectStatement.ColumnDescriptor> listColumnDescriptor, out List<IFieldMetadata> listFieldMetadata, out List<IFieldMetadata> listFieldMissingMetadata)
        {
            const string ExtendedName = "EXTENDED";
            var entityMetadata = _entityMetadata.GetEntityMetadata(statement.Statement.Table.Name);
            var oldentityMetadata = entityMetadata;
            listColumnDescriptor = new List<QuerySelectStatement.ColumnDescriptor>();
            listFieldMetadata = new List<IFieldMetadata>();
            listFieldMissingMetadata = new List<IFieldMetadata>();

            for (int i = 0; i < fields.Length; i++)
            {
                var fieldName = new KeyValuePair<string, IFieldMetadata>();
                string[] nextNames = fields[i].Split(new char[] { '.' });
                for (int j = 0; j < nextNames.Length; j++)
                {
                    if (nextNames[j] == ExtendedName)
                    {
                        if (entityMetadata.ExtendEntity != null)
                        {
                            bool LastObject = false;
                            entityMetadata = entityMetadata.ExtendEntity;
                            if (j < nextNames.Length - 1)
                            {
                                j++;
                            }
                            else
                            {
                                LastObject = true;
                            }
                            var fieldMetadataFind = entityMetadata.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                            if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                            {
                                var fieldMetadataFindx1 = entityMetadata.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                if (fieldMetadataFindx1 == null)
                                {
                                    bool isAddedField = false;
                                    var oldEntityMetadataPrev = entityMetadata;
                                    entityMetadata = entityMetadata.BaseEntity;
                                    if (entityMetadata != null)
                                    {
                                        var fieldMetadataFindx = entityMetadata.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                        if (fieldMetadataFindx == null)
                                        {
                                            for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                            {
                                                var column = new QuerySelectStatement.ColumnDescriptor();
                                                if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Column)
                                                {
                                                    column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(l).SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        var Alias = new StringBuilder();
                                                        for (int t = 0; t < j - 1; t++)
                                                        {
                                                            Alias.Append(nextNames[t]).Append(".");
                                                        }
                                                        Alias.Append(ExtendedName).Append(".");
                                                        column.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(l).Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(l));
                                                        isAddedField = true;
                                                    }
                                                }
                                                else if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Extension)
                                                {
                                                    var extensionMetaData = ((ExtensionFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).ExtensionEntity;
                                                    if (extensionMetaData != null)
                                                    {
                                                        entityMetadata = extensionMetaData;
                                                        IFieldMetadata fieldMetadataFindExt = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                        if (fieldMetadataFindExt != null)
                                                        {
                                                            var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFindExt.SourceName, Table = entityMetadata.Name };
                                                            if (string.IsNullOrEmpty(columnExt.Alias))
                                                            {
                                                                StringBuilder Alias = new StringBuilder();
                                                                for (int t = 0; t < j - 1; t++)
                                                                {
                                                                    Alias.Append(nextNames[t]).Append(".");
                                                                }
                                                                Alias.Append(ExtendedName).Append(".");
                                                                columnExt.Alias = Alias.ToString() + entityMetadata.Name;
                                                            }
                                                            if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                            {
                                                                listColumnDescriptor.Add(columnExt);
                                                                listFieldMetadata.Add(fieldMetadataFindExt);
                                                                isAddedField = true;
                                                            }
                                                        }
                                                        else if ((fieldMetadataFindExt == null) && (j == nextNames.Length - 1))
                                                        {
                                                            for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                            {
                                                                var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceName, Table = entityMetadata.Name };
                                                                if (string.IsNullOrEmpty(columnExt.Alias))
                                                                {
                                                                    StringBuilder Alias = new StringBuilder();
                                                                    for (int t = 0; t < j - 1; t++)
                                                                    {
                                                                        Alias.Append(nextNames[t]).Append(".");
                                                                    }
                                                                    Alias.Append(ExtendedName).Append(".");
                                                                    columnExt.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(lx).Name;
                                                                }
                                                                if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                {
                                                                    listColumnDescriptor.Add(columnExt);
                                                                    listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(lx));
                                                                    isAddedField = true;
                                                                }
                                                            }
                                                            entityMetadata = oldentityMetadata;
                                                        }
                                                    }
                                                }
                                                else if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Reference)
                                                {
                                                    var refMetaData = ((ReferenceFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RefEntity;
                                                    if (refMetaData != null)
                                                    {
                                                        if (refMetaData.Name != null)
                                                        {
                                                            entityMetadata = refMetaData;
                                                            var fieldMetadataFindRef = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindRef == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(lx).Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(lx));
                                                                        isAddedField = true;
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Relation)
                                                {
                                                    var relMetaData = ((RelationFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RelatedEntity;
                                                    if (relMetaData != null)
                                                    {
                                                        if (relMetaData.Name != null)
                                                        {
                                                            entityMetadata = relMetaData;
                                                            var fieldMetadataFindRel = relMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindRel == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(lx).Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(lx));
                                                                        isAddedField = true;
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var column = new QuerySelectStatement.ColumnDescriptor();
                                            column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFindx.SourceName, Table = entityMetadata.Name };
                                            if (string.IsNullOrEmpty(column.Alias))
                                            {
                                                StringBuilder Alias = new StringBuilder();
                                                for (int t = 0; t < j - 1; t++)
                                                {
                                                    Alias.Append(nextNames[t]).Append(".");
                                                }
                                                Alias.Append(ExtendedName).Append(".");
                                                column.Alias = Alias.ToString() + fieldMetadataFindx.Name;
                                            }
                                            if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                            {
                                                listColumnDescriptor.Add(column);
                                                listFieldMetadata.Add(fieldMetadataFindx);
                                                isAddedField = true;
                                            }
                                        }
                                        if ((LastObject == true) || (isAddedField == false))
                                        {
                                            entityMetadata = oldEntityMetadataPrev;
                                            for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                            {
                                                var column = new QuerySelectStatement.ColumnDescriptor();
                                                if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Column)
                                                {
                                                    column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(l).SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        StringBuilder Alias = new StringBuilder();
                                                        for (int t = 0; t < j - 1; t++)
                                                        {
                                                            Alias.Append(nextNames[t]).Append(".");
                                                        }
                                                        Alias.Append(ExtendedName).Append(".");
                                                        column.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(l).Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(l));
                                                    }
                                                }
                                                else if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Extension)
                                                {
                                                    var extensionMetaData = ((ExtensionFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).ExtensionEntity;
                                                    if (extensionMetaData != null)
                                                    {
                                                        if (extensionMetaData.Name != null)
                                                        {
                                                            entityMetadata = extensionMetaData;
                                                            var fieldMetadataFindExt = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindExt == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(lx).Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(lx));
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Reference)
                                                {
                                                    var refMetaData = ((ReferenceFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RefEntity;
                                                    if (refMetaData != null)
                                                    {
                                                        if (refMetaData.Name != null)
                                                        {
                                                            entityMetadata = refMetaData;
                                                            var fieldMetadataFindRef = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindRef == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(lx).Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(lx));
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Relation)
                                                {
                                                    var relMetaData = ((RelationFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RelatedEntity;
                                                    if (relMetaData != null)
                                                    {
                                                        if (relMetaData.Name != null)
                                                        {
                                                            entityMetadata = relMetaData;
                                                            var fieldMetadataFindRel = relMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                            if ((fieldMetadataFindRel == null) && (j == nextNames.Length - 1))
                                                            {
                                                                for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                                {
                                                                    var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceName, Table = entityMetadata.Name };
                                                                    if (string.IsNullOrEmpty(columnExt.Alias))
                                                                    {
                                                                        StringBuilder Alias = new StringBuilder();
                                                                        for (int t = 0; t < j - 1; t++)
                                                                        {
                                                                            Alias.Append(nextNames[t]).Append(".");
                                                                        }
                                                                        Alias.Append(ExtendedName).Append(".");
                                                                        columnExt.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(lx).Name;
                                                                    }
                                                                    if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                    {
                                                                        listColumnDescriptor.Add(columnExt);
                                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(lx));
                                                                    }
                                                                }
                                                                entityMetadata = oldentityMetadata;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        entityMetadata = oldEntityMetadataPrev;
                                        for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                        {
                                            var column = new QuerySelectStatement.ColumnDescriptor();
                                            if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Column)
                                            {
                                                column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(l).SourceName, Table = entityMetadata.Name };
                                                if (string.IsNullOrEmpty(column.Alias))
                                                {
                                                    StringBuilder Alias = new StringBuilder();
                                                    for (int t = 0; t < j - 1; t++)
                                                    {
                                                        Alias.Append(nextNames[t]).Append(".");
                                                    }
                                                    Alias.Append(ExtendedName).Append(".");
                                                    column.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(l).Name;
                                                }
                                                if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                {
                                                    listColumnDescriptor.Add(column);
                                                    listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(l));
                                                }
                                            }
                                            else if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Extension)
                                            {
                                                var extensionMetaData = ((ExtensionFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).ExtensionEntity;
                                                if (extensionMetaData != null)
                                                {
                                                    if (extensionMetaData.Name != null)
                                                    {
                                                        entityMetadata = extensionMetaData;
                                                        var fieldMetadataFindExt = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                        if ((fieldMetadataFindExt == null) && (j == nextNames.Length - 1))
                                                        {
                                                            for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                            {
                                                                var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceName, Table = entityMetadata.Name };
                                                                if (string.IsNullOrEmpty(columnExt.Alias))
                                                                {
                                                                    StringBuilder Alias = new StringBuilder();
                                                                    for (int t = 0; t < j - 1; t++)
                                                                    {
                                                                        Alias.Append(nextNames[t]).Append(".");
                                                                    }
                                                                    Alias.Append(ExtendedName).Append(".");
                                                                    columnExt.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(lx).Name;
                                                                }
                                                                if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                {
                                                                    listColumnDescriptor.Add(columnExt);
                                                                    listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(lx));
                                                                }
                                                            }
                                                            entityMetadata = oldentityMetadata;
                                                        }
                                                    }
                                                }
                                            }
                                            else if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Reference)
                                            {
                                                var refMetaData = ((ReferenceFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RefEntity;
                                                if (refMetaData != null)
                                                {
                                                    if (refMetaData.Name != null)
                                                    {
                                                        entityMetadata = refMetaData;
                                                        var fieldMetadataFindRef = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                        if ((fieldMetadataFindRef == null) && (j == nextNames.Length - 1))
                                                        {
                                                            for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                            {
                                                                var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceName, Table = entityMetadata.Name };
                                                                if (string.IsNullOrEmpty(columnExt.Alias))
                                                                {
                                                                    StringBuilder Alias = new StringBuilder();
                                                                    for (int t = 0; t < j - 1; t++)
                                                                    {
                                                                        Alias.Append(nextNames[t]).Append(".");
                                                                    }
                                                                    Alias.Append(ExtendedName).Append(".");
                                                                    columnExt.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(lx).Name;
                                                                }
                                                                if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                {
                                                                    listColumnDescriptor.Add(columnExt);
                                                                    listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(lx));
                                                                }
                                                            }
                                                            entityMetadata = oldentityMetadata;
                                                        }
                                                    }
                                                }
                                            }
                                            else if (entityMetadata.Fields.Values.ToList().ElementAt(l).SourceType == FieldSourceType.Relation)
                                            {
                                                var relMetaData = ((RelationFieldMetadata)(entityMetadata.Fields.Values.ToList().ElementAt(l))).RelatedEntity;
                                                if (relMetaData != null)
                                                {
                                                    if (relMetaData.Name != null)
                                                    {
                                                        entityMetadata = relMetaData;
                                                        var fieldMetadataFindRel = relMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                                        if ((fieldMetadataFindRel == null) && (j == nextNames.Length - 1))
                                                        {
                                                            for (int lx = 0; lx < entityMetadata.Fields.Count; lx++)
                                                            {
                                                                var columnExt = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(lx).SourceName, Table = entityMetadata.Name };
                                                                if (string.IsNullOrEmpty(columnExt.Alias))
                                                                {
                                                                    StringBuilder Alias = new StringBuilder();
                                                                    for (int t = 0; t < j - 1; t++)
                                                                    {
                                                                        Alias.Append(nextNames[t]).Append(".");
                                                                    }
                                                                    Alias.Append(ExtendedName).Append(".");
                                                                    columnExt.Alias = Alias.ToString() + entityMetadata.Fields.Values.ToList().ElementAt(lx).Name;
                                                                }
                                                                if (listColumnDescriptor.Find(b => b.Alias == columnExt.Alias) == null)
                                                                {
                                                                    listColumnDescriptor.Add(columnExt);
                                                                    listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(lx));
                                                                }
                                                            }
                                                            entityMetadata = oldentityMetadata;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    var column = new QuerySelectStatement.ColumnDescriptor();
                                    column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFindx1.SourceName, Table = entityMetadata.Name };

                                    if (string.IsNullOrEmpty(column.Alias))
                                    {
                                        StringBuilder Alias = new StringBuilder();
                                        for (int t = 0; t < j - 1; t++)
                                        {
                                            Alias.Append(nextNames[t]).Append(".");
                                        }
                                        Alias.Append(ExtendedName).Append(".");
                                        column.Alias = Alias.ToString() + entityMetadata.Name;
                                    }
                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                    {
                                        listColumnDescriptor.Add(column);
                                        listFieldMetadata.Add(fieldMetadataFindx1);
                                    }
                                }
                                entityMetadata = oldentityMetadata;
                            }
                        }
                    }
                    fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == nextNames[j]);
                    if (fieldName.Value != null)
                    {
                        if (fieldName.Value.SourceType == FieldSourceType.Column)
                        {
                            var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldName.Value.SourceName, Table = entityMetadata.Name };
                            if (string.IsNullOrEmpty(column.Alias))
                            {
                                column.Alias = fields[i];
                            }
                            if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                            {
                                listColumnDescriptor.Add(column);
                                listFieldMetadata.Add(fieldName.Value);
                            }
                            entityMetadata = oldentityMetadata;
                        }
                        else if (fieldName.Value.SourceType == FieldSourceType.Extension)
                        {
                            var extensionMetaData = ((ExtensionFieldMetadata)(fieldName.Value)).ExtensionEntity;
                            if (extensionMetaData != null)
                            {
                                if (extensionMetaData.Name != null)
                                {
                                    entityMetadata = extensionMetaData;
                                    var fieldMetadataFind = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                    if (fieldMetadataFind != null)
                                    {
                                        var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                        if (string.IsNullOrEmpty(column.Alias))
                                        {
                                            column.Alias = fields[i];
                                        }
                                        if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                        {
                                            listColumnDescriptor.Add(column);
                                            listFieldMetadata.Add(fieldMetadataFind);
                                        }
                                    }
                                    else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                    {
                                        for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                        {
                                            var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(l).SourceName, Table = entityMetadata.Name };
                                            if (string.IsNullOrEmpty(column.Alias))
                                            {
                                                column.Alias = fields[i] + "." + entityMetadata.Fields.Values.ToList().ElementAt(l).Name;
                                            }
                                            if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                            {
                                                listColumnDescriptor.Add(column);
                                                listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(l));
                                            }
                                        }
                                        entityMetadata = oldentityMetadata;
                                    }
                                }
                            }
                        }
                        else if (fieldName.Value.SourceType == FieldSourceType.Reference)
                        {
                            IEntityMetadata refMetaData = ((ReferenceFieldMetadata)(fieldName.Value)).RefEntity;
                            if (refMetaData != null)
                            {
                                if (refMetaData.Name != null)
                                {
                                    entityMetadata = refMetaData;
                                    var fieldMetadataFind = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                    if (fieldMetadataFind != null)
                                    {
                                        if ((refMetaData as ReferenceFieldMetadata).Mapping != null)
                                        {
                                            foreach (var FieldRef in (refMetaData as ReferenceFieldMetadata).Mapping.Fields)
                                            {
                                                if (FieldRef.Value != null)
                                                {
                                                    IFieldMetadata fieldMetaDataRef = (FieldRef.Value as PrimaryKeyFieldMappedMetadata).KeyField;
                                                    PrimaryKeyMappedMatchWith primaryKeyMappedMatchWith = (FieldRef.Value as PrimaryKeyFieldMappedMetadata).MatchWith;
                                                }
                                            }
                                        }

                                        var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                        if (string.IsNullOrEmpty(column.Alias))
                                        {
                                            column.Alias = fields[i];
                                        }
                                        if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                        {
                                            listColumnDescriptor.Add(column);
                                            listFieldMetadata.Add(fieldMetadataFind);
                                        }
                                    }
                                    else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                    {
                                        for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                        {
                                            var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(l).SourceName, Table = entityMetadata.Name };
                                            if (string.IsNullOrEmpty(column.Alias))
                                            {
                                                column.Alias = fields[i] + "." + entityMetadata.Fields.Values.ToList().ElementAt(l).Name;
                                            }
                                            if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                            {
                                                listColumnDescriptor.Add(column);
                                                listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(l));
                                            }
                                        }
                                        entityMetadata = oldentityMetadata;
                                    }
                                }
                            }
                        }
                        else if (fieldName.Value.SourceType == FieldSourceType.Relation)
                        {
                            var relatedMetaData = ((RelationFieldMetadata)(fieldName.Value)).RelatedEntity;
                            if (relatedMetaData != null)
                            {
                                if (relatedMetaData.Name != null)
                                {
                                    entityMetadata = relatedMetaData;
                                    var fieldMetadataFind = relatedMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                    if (fieldMetadataFind != null)
                                    {
                                        var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                        if (string.IsNullOrEmpty(column.Alias))
                                        {
                                            column.Alias = fields[i];
                                        }
                                        if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                        {
                                            listColumnDescriptor.Add(column);
                                            listFieldMetadata.Add(fieldMetadataFind);
                                        }
                                    }
                                    else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                    {
                                        for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                        {
                                            var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(l).SourceName, Table = entityMetadata.Name };
                                            if (string.IsNullOrEmpty(column.Alias))
                                            {
                                                column.Alias = fields[i] + "." + entityMetadata.Fields.Values.ToList().ElementAt(l).Name;
                                            }
                                            if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                            {
                                                listColumnDescriptor.Add(column);
                                                listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(l));
                                            }
                                        }
                                        entityMetadata = oldentityMetadata;
                                    }
                                }
                            }
                        }
                        else if (fieldName.Value.SourceType == FieldSourceType.Expression)
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        var oldEntityMetadataPrev = entityMetadata;
                        entityMetadata = entityMetadata.BaseEntity;
                        if (entityMetadata != null)
                        {
                            fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == nextNames[j]);
                            if (fieldName.Value != null)
                            {
                                if (fieldName.Value.SourceType == FieldSourceType.Column)
                                {
                                    var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldName.Value.SourceName, Table = entityMetadata.Name };
                                    if (string.IsNullOrEmpty(column.Alias))
                                    {
                                        column.Alias = fields[i];
                                    }
                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                    {
                                        listColumnDescriptor.Add(column);
                                        listFieldMetadata.Add(fieldName.Value);
                                    }
                                    entityMetadata = oldentityMetadata;
                                }
                                else if (fieldName.Value.SourceType == FieldSourceType.Extension)
                                {
                                    var extensionMetaData = ((ExtensionFieldMetadata)(fieldName.Value)).ExtensionEntity;
                                    if (extensionMetaData != null)
                                    {
                                        if (extensionMetaData.Name != null)
                                        {
                                            entityMetadata = extensionMetaData;
                                            var fieldMetadataFind = extensionMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                            if (fieldMetadataFind != null)
                                            {
                                                var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                                if (string.IsNullOrEmpty(column.Alias))
                                                {
                                                    column.Alias = fields[i];
                                                }
                                                if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                {
                                                    listColumnDescriptor.Add(column);
                                                    listFieldMetadata.Add(fieldMetadataFind);
                                                }
                                            }
                                            else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                            {
                                                for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                                {
                                                    var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(l).SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        column.Alias = fields[i] + "." + entityMetadata.Fields.Values.ToList().ElementAt(l).Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(l));
                                                    }
                                                }
                                                entityMetadata = oldentityMetadata;
                                            }
                                        }
                                    }
                                }
                                else if (fieldName.Value.SourceType == FieldSourceType.Reference)
                                {
                                    IEntityMetadata refMetaData = ((ReferenceFieldMetadata)(fieldName.Value)).RefEntity;
                                    if (refMetaData != null)
                                    {
                                        if (refMetaData.Name != null)
                                        {
                                            entityMetadata = refMetaData;
                                            var fieldMetadataFind = refMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                            if (fieldMetadataFind != null)
                                            {
                                                if ((refMetaData as ReferenceFieldMetadata).Mapping != null)
                                                {
                                                    foreach (var FieldRef in (refMetaData as ReferenceFieldMetadata).Mapping.Fields)
                                                    {
                                                        if (FieldRef.Value != null)
                                                        {
                                                            IFieldMetadata fieldMetaDataRef = (FieldRef.Value as PrimaryKeyFieldMappedMetadata).KeyField;
                                                            PrimaryKeyMappedMatchWith primaryKeyMappedMatchWith = (FieldRef.Value as PrimaryKeyFieldMappedMetadata).MatchWith;
                                                        }
                                                    }
                                                }

                                                var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                                if (string.IsNullOrEmpty(column.Alias))
                                                {
                                                    column.Alias = fields[i];
                                                }
                                                if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                {
                                                    listColumnDescriptor.Add(column);
                                                    listFieldMetadata.Add(fieldMetadataFind);
                                                }
                                            }
                                            else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                            {
                                                for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                                {
                                                    var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(l).SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        column.Alias = fields[i] + "." + entityMetadata.Fields.Values.ToList().ElementAt(l).Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(l));
                                                    }
                                                }
                                                entityMetadata = oldentityMetadata;
                                            }
                                        }
                                    }
                                }
                                else if (fieldName.Value.SourceType == FieldSourceType.Relation)
                                {
                                    var relatedMetaData = ((RelationFieldMetadata)(fieldName.Value)).RelatedEntity;
                                    if (relatedMetaData != null)
                                    {
                                        if (relatedMetaData.Name != null)
                                        {
                                            entityMetadata = relatedMetaData;
                                            var fieldMetadataFind = relatedMetaData.Fields.Values.ToList().Find(u => u.Name == nextNames[j]);
                                            if (fieldMetadataFind != null)
                                            {
                                                var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = fieldMetadataFind.SourceName, Table = entityMetadata.Name };
                                                if (string.IsNullOrEmpty(column.Alias))
                                                {
                                                    column.Alias = fields[i];
                                                }
                                                if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                {
                                                    listColumnDescriptor.Add(column);
                                                    listFieldMetadata.Add(fieldMetadataFind);
                                                }
                                            }
                                            else if ((fieldMetadataFind == null) && (j == nextNames.Length - 1))
                                            {
                                                for (int l = 0; l < entityMetadata.Fields.Count; l++)
                                                {
                                                    var column = new QuerySelectStatement.ColumnDescriptor() { DBTable = entityMetadata.DataSource.Name, Name = entityMetadata.Fields.Values.ToList().ElementAt(l).SourceName, Table = entityMetadata.Name };
                                                    if (string.IsNullOrEmpty(column.Alias))
                                                    {
                                                        column.Alias = fields[i] + "." + entityMetadata.Fields.Values.ToList().ElementAt(l).Name;
                                                    }
                                                    if (listColumnDescriptor.Find(b => b.Alias == column.Alias) == null)
                                                    {
                                                        listColumnDescriptor.Add(column);
                                                        listFieldMetadata.Add(entityMetadata.Fields.Values.ToList().ElementAt(l));
                                                    }
                                                }
                                                entityMetadata = oldentityMetadata;
                                            }
                                        }
                                    }
                                }
                                else if (fieldName.Value.SourceType == FieldSourceType.Expression)
                                {
                                    throw new NotImplementedException();
                                }
                            }
                        }
                        entityMetadata = oldEntityMetadataPrev;
                    }
                }
            }

        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="statement"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string BuildSelectStatement<TModel>(QuerySelectStatement<TModel> statement, IDictionary<string, EngineCommandParameter> parameters) 
         {
            try
            {
                const string PrefixTableName = "Tcaz_";
                var dicPrefixTables = new Dictionary<string, string>();
                var entityMetadata = _entityMetadata.GetEntityMetadata(statement.Statement.Table.Name);
                var schemasMetadata = new SchemasMetadata<TModel>(entityMetadata);
                var selectedColumns = statement.Statement.Table.Columns.Values.ToArray();
                var conditionsColumns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Statement.Conditions, conditionsColumns);
                var whereColumns = conditionsColumns.ToArray();
                var sortColumns = statement.Statement.Orders == null ? new QuerySelectStatement.OrderByColumnDescriptor[] { } : statement.Statement.Orders.ToArray();
                var fieldCount = whereColumns.Length + sortColumns.Length;

                var listColumnDescriptorSelected = new List<QuerySelectStatement.ColumnDescriptor>();
                var listFieldMetadataSelected = new List<IFieldMetadata>();
                var listFieldMissingMetadataSelected = new List<IFieldMetadata>();

                var listColumnDescriptorWhere = new List<QuerySelectStatement.ColumnDescriptor>();
                var listFieldMetadataWhere = new List<IFieldMetadata>();
                var listFieldMissingMetadataWhere = new List<IFieldMetadata>();

                var listColumnDescriptorSort = new List<QuerySelectStatement.ColumnDescriptor>();
                var listFieldMetadataSort = new List<IFieldMetadata>();
                var listFieldMissingMetadataSort = new List<IFieldMetadata>();

                BuildSelectStatement<TModel>(statement, selectedColumns.Select(t => t.Name).ToArray(), out listColumnDescriptorSelected, out listFieldMetadataSelected, out listFieldMissingMetadataSelected);
                

                List<string> distinctTableNames = new List<string>();
                fieldCount += listColumnDescriptorSelected.Count;
                var fieldPaths = new string[fieldCount];
                int index = 0;
                for (int i=0; i< listColumnDescriptorSelected.Count; i++)
                {
                    fieldPaths[index++] = listColumnDescriptorSelected[i].Name;
                    var dbTableName = entityMetadata.DataSource.Schema + "." + listColumnDescriptorSelected[i].DBTable;
                    if (!dicPrefixTables.ContainsKey(dbTableName))
                    {
                        dicPrefixTables.Add(dbTableName, PrefixTableName + (dicPrefixTables.Count + 1).ToString());
                    }
                }

                
                BuildSelectStatement<TModel>(statement, whereColumns.Select(t => t.ColumnName).ToArray(), out listColumnDescriptorWhere, out listFieldMetadataWhere, out listFieldMissingMetadataWhere);
                for (int i = 0; i < listColumnDescriptorWhere.Count; i++)
                {
                    fieldPaths[index++] = listColumnDescriptorWhere[i].Name;
                    var dbTableName = entityMetadata.DataSource.Schema + "." + listColumnDescriptorWhere[i].DBTable;
                    if (!dicPrefixTables.ContainsKey(dbTableName))
                    {
                        dicPrefixTables.Add(dbTableName, PrefixTableName + (dicPrefixTables.Count + 1).ToString());
                    }
                }
               

                BuildSelectStatement<TModel>(statement, sortColumns.Select(t => t.Column.Name).ToArray(), out listColumnDescriptorSort, out listFieldMetadataSort, out listFieldMissingMetadataSort);
                for (int i = 0; i < listColumnDescriptorSort.Count; i++)
                {
                    fieldPaths[index++] = listColumnDescriptorSort[i].Name;
                    var dbTableName = entityMetadata.DataSource.Schema + "." + listColumnDescriptorSort[i].DBTable;
                    if (!dicPrefixTables.ContainsKey(dbTableName))
                    {
                        dicPrefixTables.Add(dbTableName, PrefixTableName + (dicPrefixTables.Count+1).ToString());
                    }
                }
               
                //Генератор JOIN's
                var fromExpression = schemasMetadata.BuildJoinStatement(this._dataEngine, fieldPaths, out IFieldMetadata[] dbFields);
                index = 0;

                var columnExpressions = new string[listColumnDescriptorSelected.Count];
                for (int i = 0; i < listColumnDescriptorSelected.Count; i++)
                {
                    var dbField = dbFields[index++];
                    columnExpressions[i] = this._syntax.ColumnExpression(this._syntax.EncodeFieldName(dbField.SourceName), listColumnDescriptorSelected[i].Alias);
                }

                // to build the where section
                for (int i = 0; i < whereColumns.Length; i++)
                {
                    var column = whereColumns[i];
                    var dbField = dbFields[index++];
                    column.ColumnName = dbField.Name;
                }
                var whereExpression = this.BuildWhereExpression(statement.Statement.Conditions, parameters);

                // to build the order by section
                var orderByColumns = new string[sortColumns.Length];
                for (int i = 0; i < sortColumns.Length; i++)
                {
                    var column = sortColumns[i];
                    var dbField = dbFields[index++];
                    var encodeColumn = "";
                    encodeColumn = this._syntax.EncodeFieldName(dbField.Name);
                    orderByColumns[i] = _syntax.SortedColumn(encodeColumn, column.Direction);
                }

                // add on top (n)
                var limit = statement.Statement.Limit;

                // add group by

                var selectStatement = this._syntax.SelectExpression(columnExpressions, fromExpression, whereExpression, orderByColumns, limit);
                return selectStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildSelectStatement, e);
            }
         }

       

        public string BuildWhereExpression(List<Condition> conditions, IDictionary<string, EngineCommandParameter> parameters)
         {
             if (conditions == null || conditions.Count == 0)
             {
                 return null;
             }
             var expressions = conditions.Select(condition => this._conditionParser.Parse(condition, parameters)).ToArray();
             var result = this._syntax.Constraint.JoinExpressions(LogicalOperator.And, expressions);
             return result;
         }

         public string BuildSetValuesExpression(List<ColumnValue> columns, IDictionary<string, EngineCommandParameter> parameters)
         {
             if (columns == null || columns.Count == 0)
             {
                 return null;
             }
             var values = new string[columns.Count];
             for (int i = 0; i < columns.Count; i++)
             {
                 var column = columns[i];
                 var parameter = new EngineCommandParameter
                 {
                     DataType = column.DataType,
                     Name = "v_" + column.Name,
                     Value = column.GetValue()

                 };
                parameters.Add(parameter.Name, parameter);
                 values[i] = this._syntax.SetColumnValueExpression(this._syntax.EncodeFieldName(column.Name), this._syntax.EncodeParameterName(parameter.Name));
             }
             var result = string.Join("," + Environment.NewLine, values);
             return result;
         }

         public string BuildDeleteStatement<TModel>(QueryDeleteStatement<TModel> statement, IDictionary<string, EngineCommandParameter> parameters)
         {
             try
             {
                var entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
                var columns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Conditions, columns);
                var columnsArray = columns.ToArray();
                var schemasMetadata = new SchemasMetadata<TModel>(entityMetadata);
                var fromStatement = schemasMetadata.BuildJoinStatement(this._syntax, entityMetadata.DataSource.Name, columnsArray.Select(c => c.ColumnName).ToArray(), out IFieldMetadata[] dbFields);
                for (int i = 0; i < columnsArray.Length; i++)
                {
                     var column = columnsArray[i];
                     var dbField = dbFields[i];
                     column.ColumnName = dbField.Name;
                 }
                 var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);
                 var deleteStatement = this._syntax.DeleteExpression(sourceExpression, fromStatement, whereExpression);
                 return deleteStatement;
             }
             catch (Exception e)
             {
                 this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                 throw new InvalidOperationException(Exceptions.AbortedBuildDeleteStatement, e);
             }
         }

      
        public string BuildInsertStatement<TModel>(QueryInsertStatement<TModel> statement, IDictionary<string, EngineCommandParameter> parameters)
         {
             try
             {
                 var entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                 var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
                 var changedColumns = new string[statement.ColumnsValues.Count];
                 var selectedParameters = new string[statement.ColumnsValues.Count];
                 for (int i = 0; i < statement.ColumnsValues.Count; i++)
                 {
                    KeyValuePair<string, IFieldMetadata> fieldName = entityMetadata.Fields.ToList().Find(t => t.Key == statement.ColumnsValues[i].Name);
                    if (fieldName.Value != null)
                    {
                        var column = statement.ColumnsValues[i];
                        column.Name = fieldName.Value.SourceName;
                        var parameter = new EngineCommandParameter
                        {
                            DataType = column.DataType,
                            Name = "v_" + column.Name,
                            Value = column.GetValue()
                        };
                        parameters.Add(parameter.Name, parameter);
                        selectedParameters[i] = this._syntax.EncodeParameterName(parameter.Name);
                        changedColumns[i] = this._syntax.EncodeFieldName(column.Name);
                    }
                 }

                 var columnsExpression = string.Join(", ", changedColumns);
                 var valuesExpression = string.Join(", ", selectedParameters);
                 var insertStatement = this._syntax.InsertExpression(sourceExpression, columnsExpression, valuesExpression);
                 return insertStatement;
             }
             catch (Exception e)
             {
                 this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                 throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
             }
         }

         public string BuildUpdateStatement<TModel>(QueryUpdateStatement<TModel> statement, IDictionary<string, EngineCommandParameter> parameters)
         {
            try
            {
                var columns = new List<ColumnOperand>();
                AppendColumnsFromConditions(statement.Conditions, columns);
                var columnsArray = columns.ToArray();
                IEntityMetadata entityMetadata = _entityMetadata.GetEntityMetadata(statement.TableName);
                var sourceExpression = this._syntax.EncodeTableName(entityMetadata.DataSource.Schema, entityMetadata.DataSource.Name);
                var valuesExpression = this.BuildSetValuesExpression(statement.ColumnsValues, parameters);
                var schemasMetadata = new SchemasMetadata<TModel>(entityMetadata);
                var fromStatement = schemasMetadata.BuildJoinStatement(this._syntax, entityMetadata.DataSource.Name, columnsArray.Select(c => c.ColumnName).ToArray(), out IFieldMetadata[] dbFields);
                for (int i = 0; i < columnsArray.Length; i++)
                {
                    var column = columnsArray[i];
                    var dbField = dbFields[i];
                    column.ColumnName = dbField.Name;
                }
                var whereExpression = this.BuildWhereExpression(statement.Conditions, parameters);
                var updateStatement = this._syntax.UpdateExpression(sourceExpression, valuesExpression, fromStatement, whereExpression);
                return updateStatement;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.LegacyServicesIcsm, Categories.BuildingStatement, e, this);
                throw new InvalidOperationException(Exceptions.AbortedBuildUpdateStatement, e);
            }
         }

         private void AppendColumnsFromConditions(List<Condition> conditions, List<ColumnOperand> columns)
         {
             if (conditions == null || conditions.Count == 0)
             {
                 return;
             }

             for (int i = 0; i < conditions.Count; i++)
             {
                 AppendColumnsFromCondition(conditions[i], columns);
             }
         }

         private void AppendColumnsFromCondition(Condition condition, List<ColumnOperand> columns)
         {
             if (condition.Type == ConditionType.Complex)
             {
                 var complexCondition = condition as ComplexCondition;
                 var conditions = complexCondition.Conditions;
                 if (conditions != null && conditions.Length > 0)
                 {
                     for (int i = 0; i < conditions.Length; i++)
                     {
                         AppendColumnsFromCondition(conditions[i], columns);
                     }
                 }
                 return;
             }
             else if (condition.Type == ConditionType.Expression)
             {
                 var conditionExpression = condition as ConditionExpression;
                 AppendColumnFromOperand(conditionExpression.LeftOperand, columns);
                 AppendColumnFromOperand(conditionExpression.RightOperand, columns);
             }
         }

         private void AppendColumnFromOperand(Operand operand, List<ColumnOperand> columns)
         {
             if (operand == null)
             {
                 return;
             }
             if (operand.Type == OperandType.Column)
             {
                 var columnOperand = operand as ColumnOperand;
                 columns.Add(columnOperand);
             }
         }


        public void Dispose()
        {
        }

    }
}
