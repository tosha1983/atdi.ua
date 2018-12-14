using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.Platform.Logging;
using Atdi.AppServices.WebQuery.DTO;

namespace Atdi.AppServices.WebQuery.Handlers
{
    public sealed class SaveChanges : LoggedObject
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly QueriesRepository _repository;
        private readonly IUserTokenProvider _tokenProvider;
        private readonly IQueryExecutor _queryExecutor;

        public SaveChanges(QueriesRepository repository, IUserTokenProvider tokenProvider, IDataLayer<IcsmDataOrm> dataLayer, ILogger logger) : base(logger)
        {
            this._repository = repository;
            this._tokenProvider = tokenProvider;
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
        }


        /// <summary>
        /// Allocation ID
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public int AllocID(string table, bool isTransaction = false)
        {
            int? NextId = null;
            var QueryNext = _dataLayer.Builder
             .From<NEXT_ID>()
             .Where(c => c.TABLE_NAME, ConditionOperator.Equal, table)
             .Select(
                 c => c.TABLE_NAME,
                 c => c.NEXT
                );

            var isNotEmpty = this._queryExecutor
                .Fetch(QueryNext, reader =>
                {
                    var result = false;
                    while (reader.Read())
                    {
                        NextId = reader.GetValue(c => c.NEXT);
                        result = true;
                    }
                    return result;
                });
            if (isNotEmpty == false)
            {
                var QueryFromTable = _dataLayer.Builder
               .From(table)
               .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { Type = OperandType.Column, ColumnName = "ID" }, Operator = ConditionOperator.GreaterThan, RightOperand = new IntegerValueOperand() { Type = OperandType.Value, DataType = DataModels.DataType.Integer, Value = 0 } })
               .Select("ID");
                var isNotEmptyInTable = this._queryExecutor
               .Fetch(QueryFromTable, reader =>
               {
                   var result = false;
                   while (reader.Read())
                   {
                       NextId = reader.GetValueAsInt32(typeof(int), reader.GetOrdinal("ID"));
                       result = true;
                   }
                   return result;
               });

                if (NextId == null)
                {
                    NextId = 1;
                }
                else
                {
                    ++NextId;
                }
                var insertQuery = _dataLayer.Builder
               .Insert("NEXT_ID")
               .SetValue("NEXT", new IntegerValueOperand() { DataType = DataModels.DataType.Integer, Value = NextId })
               .SetValue("TABLE_NAME", new StringValueOperand() { DataType = DataModels.DataType.String, Value = table });
                this._queryExecutor.ExecuteTransaction(insertQuery);
            }
            else
            {
                var updateQuery = _dataLayer.Builder
                .Update("NEXT_ID")
                .SetValue("NEXT", new IntegerValueOperand() { DataType = DataModels.DataType.Integer, Value = ++NextId })
                .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { Type = OperandType.Column, ColumnName = "TABLE_NAME" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Type = OperandType.Value, DataType = DataModels.DataType.String, Value = table } });
                if (isTransaction)
                {
                    this._queryExecutor.ExecuteTransaction(updateQuery);
                }
                else
                {
                    this._queryExecutor.Execute(updateQuery);
                }
            }
            return NextId.Value;
        }

        public ChangesResult Handle(UserToken userToken, QueryToken queryToken, Changeset changeset)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.SaveChanges))
            {
                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                var queryDescriptor = this._repository.GetQueryDescriptorByToken(tokenData, queryToken);

                var result = new ChangesResult
                {
                    Id = changeset.Id
                };

                var actions = changeset.Actions;
                if (actions != null && actions.Length > 0)
                {
                    var actionResults = new ActionResult[actions.Length];
                    for (int i = 0; i < actions.Length; i++)
                    {
                        var action = actions[i];
                        actionResults[i] = this.PerformAction(tokenData, queryDescriptor, action);
                    }
                    result.Actions = actionResults;
                }
                return result;
            }
        }

        private ActionResult PerformAction(UserTokenData userTokenData, QueryDescriptor queryDescriptor, DataModels.Action action)
        {
            var result = new ActionResult
            {
                Id = action.Id,
                RecordsAffected = -1,
                Type = action.Type
            };

            try
            {
                queryDescriptor.VerifyAccessToAction(action.Type);
                
                switch (action.Type)
                {
                    case ActionType.Create:
                        result.RecordsAffected = this.PerformCreationAction(userTokenData, queryDescriptor, action as CreationAction);
                        break;
                    case ActionType.Update:
                        result.RecordsAffected = this.PerformUpdationAction(userTokenData, queryDescriptor, action as UpdationAction);
                        break;
                    case ActionType.Delete:
                        result.RecordsAffected = this.PerformDeleteionAction(userTokenData, queryDescriptor, action as DeletionAction);
                        break;
                    default:
                        throw new InvalidOperationException(Exceptions.ActionTypeNotSupported.With(action.Type));
                }
                result.Success = true;
            }
            catch(Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
                this.Logger.Exception(Contexts.WebQueryAppServices, Categories.Handling, e, this);
            }

            return result;
        }


        private object GetValue(ColumnValue columnValue)
        {
                object result = null;
                switch (columnValue.DataType)
                {
                    case DataType.String:
                        result = (columnValue as StringColumnValue).Value;
                        break;
                    case DataType.Boolean:
                        result = (columnValue as BooleanColumnValue).Value;
                        break;
                    case DataType.Integer:
                        result = (columnValue as IntegerColumnValue).Value;
                        break;
                    case DataType.DateTime:
                        result = (columnValue as DateTimeColumnValue).Value;
                        break;
                    case DataType.Double:
                        result = (columnValue as DoubleColumnValue).Value;
                        break;
                    case DataType.Float:
                        result = (columnValue as FloatColumnValue).Value;
                        break;
                    case DataType.Decimal:
                        result = (columnValue as DecimalColumnValue).Value;
                        break;
                    case DataType.Byte:
                        result = (columnValue as ByteColumnValue).Value;
                        break;
                    case DataType.Bytes:
                        result = (columnValue as BytesColumnValue).Value;
                        break;
                    case DataType.Guid:
                        result = (columnValue as GuidColumnValue).Value;
                        break;
                    case DataType.Char:
                        result = (columnValue as CharColumnValue).Value;
                        break;
                    case DataType.Short:
                        result = (columnValue as ShortColumnValue).Value;
                        break;
                    case DataType.UnsignedShort:
                        result = (columnValue as UnsignedShortColumnValue).Value;
                        break;
                    case DataType.UnsignedInteger:
                        result = (columnValue as UnsignedIntegerColumnValue).Value;
                        break;
                    case DataType.Long:
                        result = (columnValue as LongColumnValue).Value;
                        break;
                    case DataType.UnsignedLong:
                        result = (columnValue as UnsignedLongColumnValue).Value;
                        break;
                    case DataType.SignedByte:
                        result = (columnValue as SignedByteColumnValue).Value;
                        break;
                    case DataType.Time:
                        result = (columnValue as TimeColumnValue).Value;
                        break;
                    case DataType.Date:
                        result = (columnValue as DateColumnValue).Value;
                        break;
                    case DataType.DateTimeOffset:
                        result = (columnValue as DateTimeOffsetColumnValue).Value;
                        break;
                    case DataType.Xml:
                        result = (columnValue as XmlColumnValue).Value;
                        break;
                    case DataType.Json:
                        result = (columnValue as JsonColumnValue).Value;
                        break;
                    case DataType.ClrEnum:
                        result = (columnValue as ClrEnumColumnValue).Value;
                        break;
                    case DataType.ClrType:
                        result = (columnValue as ClrTypeColumnValue).Value;
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported data type with name '{columnValue.DataType}'");
                }
                return result;

        }

        private Type GetType(DataType dataType)
        {
            Type result = null;
            switch (dataType)
            {
                case DataType.String:
                    result = typeof(String);
                    break;
                case DataType.Boolean:
                    result = typeof(bool);
                    break;
                case DataType.Integer:
                    result = typeof(int);
                    break;
                case DataType.DateTime:
                    result = typeof(DateTime);
                    break;
                case DataType.Double:
                    result = typeof(double);
                    break;
                case DataType.Float:
                    result = typeof(float);
                    break;
                case DataType.Decimal:
                    result = typeof(decimal);
                    break;
                case DataType.Byte:
                     result = typeof(byte);
                    break;
                case DataType.Bytes:
                    result = typeof(byte[]);
                    break;
                case DataType.Guid:
                    result = typeof(Guid);
                    break;
                case DataType.Char:
                    result = typeof(char);
                    break;
                case DataType.Short:
                    result = typeof(short);
                    break;
                case DataType.UnsignedShort:
                    result = typeof(ushort);
                    break;
                case DataType.UnsignedInteger:
                    result = typeof(uint);
                    break;
                case DataType.Long:
                    result = typeof(long);
                    break;
                case DataType.UnsignedLong:
                    result = typeof(ulong);
                    break;
                case DataType.SignedByte:
                    result = typeof(sbyte);
                    break;
                case DataType.Date:
                    result = typeof(DateTime);
                    break;
                case DataType.DateTimeOffset:
                     result = typeof(DateTimeOffset);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported data type with name '{dataType}'");
            }
            return result;
        }


        /// <summary>
        /// Получить массив таблиц, которые будут обновляться
        /// </summary>
        /// <param name="irpDescrColumns"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private string[] GetTablesForUpdate(IrpColumn[] irpDescrColumns, UpdationAction action)
        {
            const string IdentColumnName = "ID";
            var distinctTables = new List<string>();
            var unPackValues = this.UnpackUpdatedValues(action);
            var valuesList = unPackValues.ToList();
            for (int i = 0; i < irpDescrColumns.Length; i++)
            {
                if ((irpDescrColumns[i].columnProperties != null) && (irpDescrColumns[i].columnProperties.Length > 0))
                {
                    for (int j = 0; j < irpDescrColumns[i].columnProperties.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(irpDescrColumns[i].columnProperties[j].NameField))
                        {
                            var columnValue = valuesList.Find(z => z.Name == irpDescrColumns[i].columnMeta.Name);
                            if ((columnValue != null) && (columnValue.Name != IdentColumnName))
                            {
                                var TableName = irpDescrColumns[i].columnProperties[j].NameTableTo;
                                if (!distinctTables.Contains(TableName))
                                {
                                    distinctTables.Add(TableName);
                                }
                            }
                        }
                    }
                }
            }
            return distinctTables.ToArray();
        }

        /// <summary>
        /// Получить массив таблиц, которые будут обновляться
        /// </summary>
        /// <param name="irpDescrColumns"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private string[] GetTablesForInsert(IrpColumn[] irpDescrColumns, CreationAction action)
        {
            const string IdentColumnName = "ID";
            var distinctTables = new List<string>();
            var unPackValues = this.UnpackInsertedValues(action);
            var valuesList = unPackValues.ToList();
            for (int i = 0; i < irpDescrColumns.Length; i++)
            {
                if ((irpDescrColumns[i].columnProperties != null) && (irpDescrColumns[i].columnProperties.Length > 0))
                {
                    for (int j = 0; j < irpDescrColumns[i].columnProperties.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(irpDescrColumns[i].columnProperties[j].NameField))
                        {
                            var columnValue = valuesList.Find(z => z.Name == irpDescrColumns[i].columnMeta.Name);
                            if ((columnValue != null) && (columnValue.Name != IdentColumnName))
                            {
                                var TableName = irpDescrColumns[i].columnProperties[j].NameTableTo;
                                if (!distinctTables.Contains(TableName))
                                {
                                    distinctTables.Add(TableName);
                                }
                            }
                        }
                    }
                }
            }
            return distinctTables.ToArray();
        }


        private int? InsertRecord(List<ColumnProperties> mandatoryColumns, ColumnProperties primaryKey, ref List<LinkColumn> listLinkColumn, ColumnProperties columnProperties, string FullColumnName, Atdi.DataModels.Action updateAction, Atdi.DataModels.Action createAction, UserTokenData tokenData, QueryDescriptor queryDescriptor, List<ColumnValue> listUpdateValues = null)
        {
            return InsertRecord(mandatoryColumns, primaryKey, ref listLinkColumn, columnProperties.NameTableTo, FullColumnName, updateAction, createAction, tokenData, queryDescriptor, listUpdateValues);
        }

        private int? InsertRecord(List<ColumnProperties> mandatoryColumns, ColumnProperties primaryKey, ref List<LinkColumn> listLinkColumn, string TableName, string FullColumnName, Atdi.DataModels.Action updateAction, Atdi.DataModels.Action createAction, UserTokenData tokenData, QueryDescriptor queryDescriptor, List<ColumnValue> listUpdateValues = null)
        {
            var mandatoryFields = mandatoryColumns.FindAll(z => z.NameTableTo == TableName);
            var columnsValueAdditional = new List<ColumnValue>();
            for (int k = 0; k < mandatoryFields.Count; k++)
            {
                if ((listUpdateValues==null) || ((listUpdateValues != null) && (listUpdateValues.Find(z => z.Name == mandatoryFields[k].FieldJoinTo) == null)) || (mandatoryFields[k].FieldJoinTo == primaryKey.FieldJoinTo))
                {
                    if ((mandatoryFields[k] != null) && (mandatoryFields[k].TypeColumn == typeof(int)))
                    {
                        if (mandatoryFields[k].Precision > 0)
                        {
                            var columnValueAdditional = new IntegerColumnValue()
                            {
                                Name = mandatoryFields[k].FieldJoinTo,
                                DataType = DataType.Integer,
                                Value = 0
                            };
                            columnsValueAdditional.Add(columnValueAdditional);
                        }
                    }
                    else if ((mandatoryFields[k] != null) && (mandatoryFields[k].TypeColumn == typeof(string)))
                    {
                        if (mandatoryFields[k].Precision > 0)
                        {
                            var columnValueAdditional = new StringColumnValue()
                            {
                                Name = mandatoryFields[k].FieldJoinTo,
                                DataType = DataType.String,
                                Value = string.IsNullOrEmpty(mandatoryFields[k].DefaultValue) ? Guid.NewGuid().ToString().Substring(0, mandatoryFields[k].Precision < 36 ? mandatoryFields[k].Precision - 1 : 35) : mandatoryFields[k].DefaultValue
                            };
                            columnsValueAdditional.Add(columnValueAdditional);
                        }
                    }
                }
            }

            string PrefixColumn = FullColumnName;
            if (listUpdateValues!=null)
            {
                foreach (var x in listUpdateValues)
                {
                    if (x.Name != primaryKey.FieldJoinTo)
                    {
                        if ((columnsValueAdditional.Find(c => c.Name == x.Name)) == null)
                        {
                            columnsValueAdditional.Add(x);
                        }
                        if (listLinkColumn.Find(t => t.LinkFieldName == x.Name && t.TableName == TableName && t.IsMandatory==true) == null)
                        {
                            listLinkColumn.Add(new LinkColumn()
                            {
                                LinkFieldName = x.Name,
                                TableName = TableName,
                                TypeColumn = GetType(x.DataType),
                                ValueLinkId = GetValue(x),
                                FullSourceName = FullColumnName
                            });
                        }
                    }
                }
            }

            if (TableName == "MICROWS")
            {
                var findMWIdData = listLinkColumn.Find(z => z.LinkFieldName == "MW_ID" && z.TableName == TableName && ((z.FullSourceName==FullColumnName) || (z.FullSourceName=="ID")));
                var findRole = columnsValueAdditional.Find(z => z.Name == "ROLE");
                var findEndRole = columnsValueAdditional.Find(z => z.Name == "END_ROLE");
                if (findMWIdData != null)
                {
                    var MWIdData = listLinkColumn.Find(z => z.LinkFieldName == "MW_ID" && z.TableName == TableName);
                    if (MWIdData == null)
                    {
                        columnsValueAdditional.Add(new IntegerColumnValue()
                        {
                            Name = findMWIdData.LinkFieldName,
                            DataType = DataType.Integer,
                            Value = findMWIdData.ValueLinkId as int?,
                        });
                    }
                    else
                    {
                        columnsValueAdditional.RemoveAll(z => z.Name == "MW_ID");
                        columnsValueAdditional.Add(new IntegerColumnValue()
                        {
                            Name = findMWIdData.LinkFieldName,
                            DataType = DataType.Integer,
                            Value = findMWIdData.ValueLinkId as int?
                        });
                    }
                }
               


                if (FullColumnName.Contains("StationA"))
                {
                    if (findRole == null)
                    {
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "ROLE", Value = "A" });
                    }
                    else
                    {
                        columnsValueAdditional.Remove(findRole);
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "ROLE", Value = "A" });
                    }

                    if (findEndRole == null)
                    {
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "END_ROLE", Value = "B" });
                    }
                    else
                    {
                        columnsValueAdditional.Remove(findEndRole);
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "END_ROLE", Value = "B" });
                    }
                }
                else if (FullColumnName.Contains("StationB"))
                {
                    if (findRole == null)
                    {
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "ROLE", Value = "B" });
                    }
                    else
                    {
                        columnsValueAdditional.Remove(findRole);
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "ROLE", Value = "B" });
                    }

                    if (findEndRole == null)
                    {
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "END_ROLE", Value = "A" });
                    }
                    else
                    {
                        columnsValueAdditional.Remove(findEndRole);
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "END_ROLE", Value = "A" });
                    }
                }
                else if (FullColumnName.Contains("StationPa"))
                {
                    if (findRole == null)
                    {
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "ROLE", Value = "Pa" });
                    }
                    else
                    {
                        columnsValueAdditional.Remove(findRole);
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "ROLE", Value = "Pa" });
                    }

                    if (findEndRole == null)
                    {
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pb" });
                    }
                    else
                    {
                        columnsValueAdditional.Remove(findEndRole);
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pb" });
                    }
                }
                else if (FullColumnName.Contains("StationPb"))
                {
                    if (findRole == null)
                    {
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "ROLE", Value = "Pb" });
                    }
                    else
                    {
                        columnsValueAdditional.Remove(findRole);
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "ROLE", Value = "Pb" });
                    }

                    if (findEndRole == null)
                    {
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pa" });
                    }
                    else
                    {
                        columnsValueAdditional.Remove(findEndRole);
                        columnsValueAdditional.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pa" });
                    }
                }
           }

            var columnValuesData = columnsValueAdditional.ToArray();
            queryDescriptor.PrapareValidationConditions(tokenData, columnValuesData, createAction, false);
            queryDescriptor.GetConditions(tokenData, columnValuesData, createAction, false);

            int? identValue = null;
            var queryBuilderInsert = this._dataLayer.Builder
              .Insert(TableName);

           
            var linkColumnD = new LinkColumn();
            linkColumnD.LinkFieldName = primaryKey.FieldJoinTo;
            linkColumnD.TableName = TableName;
            linkColumnD.TypeColumn = primaryKey.TypeColumn;
            linkColumnD.FullSourceName = FullColumnName;
            if (!string.IsNullOrEmpty(PrefixColumn))
            {
                if (listLinkColumn.Find(x => x.LinkFieldName == linkColumnD.LinkFieldName && x.TableName == linkColumnD.TableName && x.FullSourceName == PrefixColumn) == null)
                {
                    var findS = listLinkColumn.Find(x => x.LinkFieldName == linkColumnD.LinkFieldName && x.TableName == linkColumnD.TableName && x.FullSourceName == PrefixColumn);
                    if (findS == null)
                    { 
                        var findVal = listLinkColumn.Find(z => z.LinkFieldName == linkColumnD.LinkFieldName && z.TableName == linkColumnD.TableName);
                        if (findVal != null)
                        {
                            if (((findVal.TableName == queryDescriptor.TableName) && (findVal.LinkFieldName == primaryKey.FieldJoinTo)) == false)
                            {
                                linkColumnD.IsUpdated = true;
                                identValue = AllocID(TableName);
                                linkColumnD.ValueLinkId = identValue;
                                Array.Resize(ref columnValuesData, columnValuesData.Length + 1);
                                columnValuesData[columnValuesData.Length - 1] = new IntegerColumnValue() { Name = primaryKey.FieldJoinTo, Value = identValue };
                                listLinkColumn.Add(linkColumnD);
                                if ((columnValuesData != null) && (columnValuesData.Length > 0))
                                {
                                    queryBuilderInsert.SetValues(columnValuesData);
                                }
                                this._queryExecutor.ExecuteTransaction(queryBuilderInsert);
                            }
                        }
                        else
                        {
                            linkColumnD.IsUpdated = true;
                            identValue = AllocID(TableName);
                            linkColumnD.ValueLinkId = identValue;
                            Array.Resize(ref columnValuesData, columnValuesData.Length + 1);
                            columnValuesData[columnValuesData.Length - 1] = new IntegerColumnValue() { Name = primaryKey.FieldJoinTo, Value = identValue };
                            listLinkColumn.Add(linkColumnD);
                            if ((columnValuesData != null) && (columnValuesData.Length > 0))
                            {
                                queryBuilderInsert.SetValues(columnValuesData);
                            }
                            this._queryExecutor.ExecuteTransaction(queryBuilderInsert);
                        }
                    }
                } 
            }
            else
            {
                if (listLinkColumn.Find(x => x.LinkFieldName == linkColumnD.LinkFieldName && x.TableName == linkColumnD.TableName ) == null)
                {
                    var findS = listLinkColumn.Find(x => x.LinkFieldName == linkColumnD.LinkFieldName && x.TableName == linkColumnD.TableName );
                    if (findS == null)
                    {
                        var findVal = listLinkColumn.Find(z => z.LinkFieldName == linkColumnD.LinkFieldName && z.TableName == linkColumnD.TableName);
                        if (findVal != null)
                        {
                            if (((findVal.TableName == queryDescriptor.TableName) && (findVal.LinkFieldName == primaryKey.FieldJoinTo)) == false)
                            {
                                linkColumnD.IsUpdated = true;
                                identValue = AllocID(TableName);
                                linkColumnD.ValueLinkId = identValue;
                                Array.Resize(ref columnValuesData, columnValuesData.Length + 1);
                                columnValuesData[columnValuesData.Length - 1] = new IntegerColumnValue() { Name = primaryKey.FieldJoinTo, Value = identValue };
                                listLinkColumn.Add(linkColumnD);
                                if ((columnValuesData != null) && (columnValuesData.Length > 0))
                                {
                                    queryBuilderInsert.SetValues(columnValuesData);
                                }
                                this._queryExecutor.ExecuteTransaction(queryBuilderInsert);
                            }
                        }
                        else
                        {
                            linkColumnD.IsUpdated = true;
                            identValue = AllocID(TableName);
                            linkColumnD.ValueLinkId = identValue;
                            Array.Resize(ref columnValuesData, columnValuesData.Length + 1);
                            columnValuesData[columnValuesData.Length - 1] = new IntegerColumnValue() { Name = primaryKey.FieldJoinTo, Value = identValue };
                            listLinkColumn.Add(linkColumnD);
                            if ((columnValuesData != null) && (columnValuesData.Length > 0))
                            {
                                queryBuilderInsert.SetValues(columnValuesData);
                            }
                            this._queryExecutor.ExecuteTransaction(queryBuilderInsert);
                        }
                    }
                }
            }
            return identValue;
        }


        private void UpdateRecord(ref List<LinkColumn> listLinkColumn, QueryDescriptor queryDescriptor, ColumnProperties columnProperties, LinkColumn LinkColumnFinded, List<LinkValue> linkValues, string FullColumnName, Atdi.DataModels.Action updateAction, UserTokenData tokenData)
        {
            var queryBuilderUpdate = this._dataLayer.Builder
            .Update(columnProperties.NameTableTo);

            if (LinkColumnFinded.TypeColumn == typeof(int))
            {
                queryBuilderUpdate
                .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = LinkColumnFinded.LinkFieldName }, Operator = ConditionOperator.Equal, RightOperand = new IntegerValueOperand() { Value = LinkColumnFinded.ValueLinkId as int? } });
            }
            else if (LinkColumnFinded.TypeColumn == typeof(string))
            {
                queryBuilderUpdate
               .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = LinkColumnFinded.LinkFieldName }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = LinkColumnFinded.ValueLinkId as string } });
            }

            if (FullColumnName != "ID")
            {
                var checkedSetValues = new List<ColumnValue>();
                for (int j = 0; j < linkValues.Count; j++)
                {
                    if (((FullColumnName != null) && (linkValues[j].Name.Contains(FullColumnName))) || (FullColumnName == null))
                    {

                        if ((linkValues[j].SourceColumnName != LinkColumnFinded.LinkFieldName))
                        {
                            if (linkValues[j].typeValue == DataType.Integer)
                            {
                                if (checkedSetValues.Find(y => y.Name == linkValues[j].SourceColumnName) == null)
                                {
                                    checkedSetValues.Add(new IntegerColumnValue() { Source = (GetNameColumnInDb(linkValues[j].Name)== linkValues[j].SourceColumnName) ? linkValues[j].Name : linkValues[j].SourceColumnName,  Name = linkValues[j].SourceColumnName, Value = linkValues[j].Value as int? });
                                }
                            }
                            else if (linkValues[j].typeValue == DataType.String)
                            {
                                if (checkedSetValues.Find(y => y.Name == linkValues[j].SourceColumnName) == null)
                                {
                                    checkedSetValues.Add(new StringColumnValue() { Source = (GetNameColumnInDb(linkValues[j].Name) == linkValues[j].SourceColumnName) ? linkValues[j].Name : linkValues[j].SourceColumnName, Name = linkValues[j].SourceColumnName, Value = linkValues[j].Value as string });
                                }
                            }
                            else if (linkValues[j].typeValue == DataType.Double)
                            {
                                if (checkedSetValues.Find(y => y.Name == linkValues[j].SourceColumnName) == null)
                                {
                                    checkedSetValues.Add(new DoubleColumnValue() { Source = (GetNameColumnInDb(linkValues[j].Name) == linkValues[j].SourceColumnName) ? linkValues[j].Name : linkValues[j].SourceColumnName, Name = linkValues[j].SourceColumnName, Value = linkValues[j].Value as double? });
                                }
                            }
                            else if (linkValues[j].typeValue == DataType.Float)
                            {
                                if (checkedSetValues.Find(y => y.Name == linkValues[j].SourceColumnName) == null)
                                {
                                    checkedSetValues.Add(new FloatColumnValue() { Source = (GetNameColumnInDb(linkValues[j].Name) == linkValues[j].SourceColumnName) ? linkValues[j].Name : linkValues[j].SourceColumnName, Name = linkValues[j].SourceColumnName, Value = linkValues[j].Value as float? });
                                }
                            }
                            else if (linkValues[j].typeValue == DataType.DateTime)
                            {
                                if (checkedSetValues.Find(y => y.Name == linkValues[j].SourceColumnName) == null)
                                {
                                    checkedSetValues.Add(new DateTimeColumnValue() { Source = (GetNameColumnInDb(linkValues[j].Name) == linkValues[j].SourceColumnName) ? linkValues[j].Name : linkValues[j].SourceColumnName, Name = linkValues[j].SourceColumnName, Value = linkValues[j].Value as DateTime? });
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported data type for column value '{linkValues[j].typeValue}'");
                            }


                            var linkColumnUpdate = new LinkColumn();
                            linkColumnUpdate.LinkFieldName = linkValues[j].SourceColumnName;
                            linkColumnUpdate.TableName = columnProperties.NameTableTo;
                            linkColumnUpdate.ValueLinkId = linkValues[j].Value;
                            linkColumnUpdate.FullSourceName = FullColumnName;
                            if (linkValues[j].typeValue == DataType.Integer)
                            {
                                linkColumnUpdate.TypeColumn = typeof(int);
                            }
                            else if (linkValues[j].typeValue == DataType.String)
                            {
                                linkColumnUpdate.TypeColumn = typeof(string);
                            }
                            else if (linkValues[j].typeValue == DataType.Double)
                            {
                                linkColumnUpdate.TypeColumn = typeof(double);
                            }
                            else if (linkValues[j].typeValue == DataType.DateTime)
                            {
                                linkColumnUpdate.TypeColumn = typeof(DateTime);
                            }
                            else if (linkValues[j].typeValue == DataType.Float)
                            {
                                linkColumnUpdate.TypeColumn = typeof(float);
                            }


                            if (listLinkColumn.Find(x => x.LinkFieldName == linkValues[j].SourceColumnName && x.TableName == columnProperties.NameTableTo && FullColumnName.Contains(x.FullSourceName)) == null)
                            {
                                listLinkColumn.Add(linkColumnUpdate);
                            }
                        }
                    }
                }


                var checkedValues = checkedSetValues.ToArray();
                queryDescriptor.PrapareValidationConditions(tokenData, checkedValues, updateAction);

                
                var conditions = queryDescriptor.GetConditions(tokenData, checkedValues, updateAction);

                queryBuilderUpdate
                .SetValues(checkedValues);

                if ((conditions != null) && (conditions.Length > 0))
                {
                    queryBuilderUpdate
                    .Where(conditions);
                }
                this._queryExecutor.ExecuteTransaction(queryBuilderUpdate);
            }
        }

        private void GetLinkColumnByTableTo(ref List<LinkColumn> listLinkColumn, ColumnProperties columnProperties, QueryDescriptor queryDescriptor, List<LinkValue> linkValues, List<ColumnValue> listColumnValues, string FullColumnName, Atdi.DataModels.Action updateAction, Atdi.DataModels.Action createAction, UserTokenData tokenData)
        {
            object idValueFieldJoinFrom = null;
            object idValuePrimaryKey = null;
            var primaryColumns = queryDescriptor.PrimaryColumns.ToList();
            var primaryKeys = primaryColumns.Find(z => z.NameTableTo == columnProperties.NameTableTo);
            var linkColumnFinded = listLinkColumn.Find(x => x.LinkFieldName == columnProperties.FieldJoinFrom && x.TableName == columnProperties.NameTableFrom && FullColumnName.Contains(x.FullSourceName));
            if (linkColumnFinded == null)
            {
                if (primaryKeys != null)
                {
                    linkColumnFinded = listLinkColumn.Find(x => x.LinkFieldName == primaryKeys.FieldJoinTo && x.TableName == columnProperties.NameTableTo && FullColumnName.Contains(x.FullSourceName));
                    if (linkColumnFinded != null)
                    {
                        idValuePrimaryKey = linkColumnFinded.ValueLinkId;
                    }
                    else
                    {
                        var mandatoryColumns = queryDescriptor.MandatoryColumns.ToList();
                        var findPreValue = listLinkColumn.Find(x => x.LinkFieldName == columnProperties.FieldJoinFrom && ((x.FullSourceName == "ID") && (queryDescriptor.TableName == x.TableName)));
                        if (findPreValue!= null)
                        {
                            var linkColumn = new LinkColumn();
                            linkColumn.LinkFieldName = columnProperties.FieldJoinTo;
                            linkColumn.TableName = columnProperties.NameTableTo;
                            linkColumn.ValueLinkId = findPreValue.ValueLinkId;
                            linkColumn.TypeColumn = columnProperties.TypeColumn;
                            linkColumn.FullSourceName = FullColumnName;
                            linkColumn.IsUpdated = false;
                            linkColumn.IsMandatory = true;
                            listLinkColumn.Add(linkColumn);
                        }

                        InsertRecord(mandatoryColumns, primaryKeys, ref listLinkColumn, columnProperties, FullColumnName, updateAction, createAction, tokenData, queryDescriptor, listColumnValues);
                        linkColumnFinded = listLinkColumn.Find(x => x.LinkFieldName == columnProperties.FieldJoinTo && x.TableName == columnProperties.NameTableTo && FullColumnName.Contains(x.FullSourceName));
                        if (linkColumnFinded != null)
                        {
                            idValueFieldJoinFrom = linkColumnFinded.ValueLinkId;
                            var primaryKeysLinked = primaryColumns.Find(z => z.NameTableTo == columnProperties.NameTableFrom);
                            if (primaryKeysLinked != null)
                            {
                                var LinkColumnFindedNew = listLinkColumn.Find(x => (x.LinkFieldName == primaryKeysLinked.FieldJoinTo && x.TableName == columnProperties.NameTableFrom && FullColumnName.Contains(x.FullSourceName))  || ((x.LinkFieldName == primaryKeysLinked.FieldJoinTo) && (x.FullSourceName== primaryKeysLinked.FieldJoinTo) && (x.TableName== columnProperties.NameTableFrom)));
                                if (LinkColumnFindedNew != null)
                                {

                                    var queryBuilderUpdate = this._dataLayer.Builder
                                    .Update(columnProperties.NameTableFrom);

                                    if (LinkColumnFindedNew.TypeColumn == typeof(int))
                                    {
                                        queryBuilderUpdate
                                        .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = LinkColumnFindedNew.LinkFieldName }, Operator = ConditionOperator.Equal, RightOperand = new IntegerValueOperand() { Value = LinkColumnFindedNew.ValueLinkId as int? } });
                                    }
                                    else if (LinkColumnFindedNew.TypeColumn == typeof(string))
                                    {
                                        queryBuilderUpdate
                                       .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = LinkColumnFindedNew.LinkFieldName }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = LinkColumnFindedNew.ValueLinkId as string } });
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException($"Unsupported data type for condition column value '{LinkColumnFindedNew.TypeColumn}'");
                                    }


                                    var checkedSetValues = new List<ColumnValue>();
                                    if (columnProperties.TypeColumn == typeof(int))
                                    { 
                                        if (checkedSetValues.Find(y => y.Name == columnProperties.FieldJoinFrom) == null)
                                        {
                                            checkedSetValues.Add(new IntegerColumnValue() { Source = (GetNameColumnInDb(LinkColumnFindedNew.FullSourceName) == columnProperties.FieldJoinFrom) ? LinkColumnFindedNew.FullSourceName : columnProperties.FieldJoinFrom,  Name = columnProperties.FieldJoinFrom, Value = idValueFieldJoinFrom as int? });
                                        }
                                    }
                                    else if (columnProperties.TypeColumn == typeof(string))
                                    {
                                        if (checkedSetValues.Find(y => y.Name == columnProperties.FieldJoinFrom) == null)
                                        {
                                            checkedSetValues.Add(new StringColumnValue() { Source = (GetNameColumnInDb(LinkColumnFindedNew.FullSourceName) == columnProperties.FieldJoinFrom) ? LinkColumnFindedNew.FullSourceName : columnProperties.FieldJoinFrom, Name = columnProperties.FieldJoinFrom, Value = idValueFieldJoinFrom as string });
                                        }
                                    }
                                    else if (columnProperties.TypeColumn == typeof(float))
                                    {
                                        if (checkedSetValues.Find(y => y.Name == columnProperties.FieldJoinFrom) == null)
                                        {
                                            checkedSetValues.Add(new FloatColumnValue() { Source = (GetNameColumnInDb(LinkColumnFindedNew.FullSourceName) == columnProperties.FieldJoinFrom) ? LinkColumnFindedNew.FullSourceName : columnProperties.FieldJoinFrom, Name = columnProperties.FieldJoinFrom, Value = idValueFieldJoinFrom as float? });
                                        }
                                    }
                                    else if (columnProperties.TypeColumn == typeof(DateTime))
                                    {
                                        if (checkedSetValues.Find(y => y.Name == columnProperties.FieldJoinFrom) == null)
                                        {
                                            checkedSetValues.Add(new DateTimeColumnValue() { Source = (GetNameColumnInDb(LinkColumnFindedNew.FullSourceName) == columnProperties.FieldJoinFrom) ? LinkColumnFindedNew.FullSourceName : columnProperties.FieldJoinFrom, Name = columnProperties.FieldJoinFrom, Value = idValueFieldJoinFrom as DateTime? });
                                        }
                                    }
                                    else if (columnProperties.TypeColumn == typeof(double))
                                    {
                                        if (checkedSetValues.Find(y => y.Name == columnProperties.FieldJoinFrom) == null)
                                        {
                                            checkedSetValues.Add(new DoubleColumnValue() { Source = (GetNameColumnInDb(LinkColumnFindedNew.FullSourceName) == columnProperties.FieldJoinFrom) ? LinkColumnFindedNew.FullSourceName : columnProperties.FieldJoinFrom, Name = columnProperties.FieldJoinFrom, Value = idValueFieldJoinFrom as double? });
                                        }
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException($"Unsupported data type for column value '{columnProperties.TypeColumn}'");
                                    }


                                    var linkColumnUpdate = new LinkColumn();
                                    linkColumnUpdate.LinkFieldName = columnProperties.FieldJoinFrom;
                                    linkColumnUpdate.TableName = columnProperties.NameTableFrom;
                                    linkColumnUpdate.ValueLinkId = idValueFieldJoinFrom;
                                    linkColumnUpdate.FullSourceName = FullColumnName;
                                    if (columnProperties.TypeColumn == typeof(int))
                                    {
                                        linkColumnUpdate.TypeColumn = typeof(int);
                                    }
                                    else if (columnProperties.TypeColumn == typeof(string))
                                    {
                                        linkColumnUpdate.TypeColumn = typeof(string);
                                    }
                                    else if (columnProperties.TypeColumn == typeof(double))
                                    {
                                        linkColumnUpdate.TypeColumn = typeof(double);
                                    }
                                    else if (columnProperties.TypeColumn == typeof(DateTime))
                                    {
                                        linkColumnUpdate.TypeColumn = typeof(DateTime);
                                    }
                                    else if (columnProperties.TypeColumn == typeof(float))
                                    {
                                        linkColumnUpdate.TypeColumn = typeof(float);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException($"Unsupported data type for column list '{linkColumnUpdate.TypeColumn}'");
                                    }


                                    var checkedValues = checkedSetValues.ToArray();
                                    queryDescriptor.PrapareValidationConditions(tokenData, checkedValues, updateAction);
                                    var conditions = queryDescriptor.GetConditions(tokenData, checkedValues, updateAction);

                                    queryBuilderUpdate
                                    .SetValues(checkedValues);


                                    if ((conditions != null) && (conditions.Length > 0))
                                    {
                                        queryBuilderUpdate
                                        .Where(conditions);
                                    }


                                    if (listLinkColumn.Find(x => x.LinkFieldName == columnProperties.NameField && x.TableName == columnProperties.NameTableFrom && FullColumnName.Contains(x.FullSourceName)) == null)
                                    {
                                        listLinkColumn.Add(linkColumnUpdate);
                                        this._queryExecutor.ExecuteTransaction(queryBuilderUpdate);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (columnProperties.NameField != null)
                            {
                                if (primaryKeys != null)
                                {
                                    linkColumnFinded = listLinkColumn.Find(x => x.LinkFieldName == primaryKeys.FieldJoinTo && x.TableName == columnProperties.NameTableTo && x.TableName == queryDescriptor.TableName);
                                    if (linkColumnFinded != null)
                                    {
                                        UpdateRecord(ref listLinkColumn, queryDescriptor, columnProperties, linkColumnFinded, linkValues, FullColumnName, updateAction, tokenData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                idValueFieldJoinFrom = linkColumnFinded.ValueLinkId;
            }

            if (linkColumnFinded != null)
            {
                if ((idValueFieldJoinFrom != null) || (idValuePrimaryKey != null) && (columnProperties.FieldJoinTo!=null) && (primaryKeys.FieldJoinTo != null))
                {
                    IQuerySelectStatement QueryNext = null;
                    if (idValueFieldJoinFrom == null)
                    {
                        QueryNext = _dataLayer.Builder
                       .From(columnProperties.NameTableTo)
                       .Select(primaryKeys.FieldJoinTo)
                       .Select(columnProperties.FieldJoinTo);
                    }
                    else if (idValueFieldJoinFrom != null)
                    {
                        QueryNext = _dataLayer.Builder
                        .From(columnProperties.NameTableTo)
                        .Select(primaryKeys.FieldJoinTo)
                        .Select(columnProperties.FieldJoinTo);
                    }

                    if (columnProperties.NameTableTo == "MICROWS")
                    {
                        QueryNext
                       .Select("ROLE")
                       .Select("END_ROLE")
                       .Select("SITE_ID")
                       .Select("PLAN_ID")
                       .Select("ANT2_ID")
                       .Select("ANT_ID");
                    }


                    if (idValueFieldJoinFrom != null)
                    {
                        if (columnProperties.TypeColumn == typeof(int))
                        {
                            QueryNext
                            .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = columnProperties.FieldJoinTo }, Operator = ConditionOperator.Equal, RightOperand = new IntegerValueOperand() { Value = idValueFieldJoinFrom as int? } });
                        }
                        else if (columnProperties.TypeColumn == typeof(string))
                        {
                            QueryNext
                            .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = columnProperties.FieldJoinTo }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = idValueFieldJoinFrom as string } });
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported data type for condition column value '{columnProperties.TypeColumn}'");
                        }
                    }
                    else if (idValuePrimaryKey != null)
                    {
                        if (primaryKeys != null)
                        {
                            if (primaryKeys.TypeColumn == typeof(int))
                            {
                                QueryNext
                                .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = primaryKeys.FieldJoinTo }, Operator = ConditionOperator.Equal, RightOperand = new IntegerValueOperand() { Value = idValuePrimaryKey as int? } });
                            }
                            else if (primaryKeys.TypeColumn == typeof(string))
                            {
                                QueryNext
                                .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = primaryKeys.FieldJoinTo }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = idValuePrimaryKey as string } });
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported data type for condition column value '{primaryKeys.TypeColumn}'");
                            }
                        }
                    }

                    if (columnProperties.NameTableTo == "MICROWS")
                    {
                        if (columnProperties.Name != null)
                        {
                            if ((columnProperties.NameTableTo == "MICROWS") && (columnProperties.Name.Contains("StationA")))
                            {
                                QueryNext.Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = "ROLE" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = "A" } });
                                QueryNext.Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = "END_ROLE" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = "B" } });
                                QueryNext.OrderByAsc(new string[] { "ROLE" });

                            }
                            else if ((columnProperties.NameTableTo == "MICROWS") && (columnProperties.Name.Contains("StationB")))
                            {
                                QueryNext.Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = "ROLE" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = "B" } });
                                QueryNext.Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = "END_ROLE" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = "A" } });
                                QueryNext.OrderByDesc(new string[] { "ROLE" });
                            }
                            else if ((columnProperties.NameTableTo == "MICROWS") && (columnProperties.Name.Contains("StationPa")))
                            {
                                QueryNext.Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = "ROLE" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = "Pa" } });
                                QueryNext.Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = "END_ROLE" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = "Pb" } });
                                QueryNext.OrderByAsc(new string[] { "ROLE" });
                            }
                            else if ((columnProperties.NameTableTo == "MICROWS") && (columnProperties.Name.Contains("StationPb")))
                            {
                                QueryNext.Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = "ROLE" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = "Pb" } });
                                QueryNext.Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = "END_ROLE" }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = "Pa" } });
                                QueryNext.OrderByDesc(new string[] { "ROLE" });
                            }
                        }
                    }
                  

                    object siteID = null;
                    object PlanID = null;
                    object ant1ID = null;
                    object ant2ID = null;
                    object valueLinkFromColumn = null;
                    object valuePrimaryKey = null;
                    this._queryExecutor
                    .Fetch(QueryNext, reader =>
                    {
                        var result = false;
                        while (reader.Read())
                        {
                            if (columnProperties.TypeColumn == typeof(int))
                            {
                                valueLinkFromColumn = reader.GetValueAsObject(DataType.Integer, reader.GetFieldType(reader.GetOrdinal(columnProperties.FieldJoinTo)), reader.GetOrdinal(columnProperties.FieldJoinTo));
                            }
                            else if (columnProperties.TypeColumn == typeof(string))
                            {
                                valueLinkFromColumn = reader.GetValueAsObject(DataType.String, reader.GetFieldType(reader.GetOrdinal(columnProperties.FieldJoinTo)), reader.GetOrdinal(columnProperties.FieldJoinTo));
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported data type for condition column value '{columnProperties.TypeColumn}'");
                            }


                            if (primaryKeys.TypeColumn == typeof(int))
                            {
                                valuePrimaryKey = reader.GetValueAsObject(DataType.Integer, reader.GetFieldType(reader.GetOrdinal(primaryKeys.FieldJoinTo)), reader.GetOrdinal(primaryKeys.FieldJoinTo));
                            }
                            else if (primaryKeys.TypeColumn == typeof(string))
                            {
                                valuePrimaryKey = reader.GetValueAsObject(DataType.String, reader.GetFieldType(reader.GetOrdinal(primaryKeys.FieldJoinTo)), reader.GetOrdinal(primaryKeys.FieldJoinTo));
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported data type for get column value '{primaryKeys.TypeColumn}'");
                            }


                            if (columnProperties.NameTableTo == "MICROWS")
                            {
                                siteID = reader.GetValueAsObject(DataType.Integer, reader.GetFieldType(reader.GetOrdinal("SITE_ID")), reader.GetOrdinal("SITE_ID"));
                                PlanID = reader.GetValueAsObject(DataType.Integer, reader.GetFieldType(reader.GetOrdinal("PLAN_ID")), reader.GetOrdinal("PLAN_ID"));
                                ant1ID  = reader.GetValueAsObject(DataType.Integer, reader.GetFieldType(reader.GetOrdinal("ANT_ID")), reader.GetOrdinal("ANT_ID"));
                                ant2ID = reader.GetValueAsObject(DataType.Integer, reader.GetFieldType(reader.GetOrdinal("ANT2_ID")), reader.GetOrdinal("ANT2_ID"));
                            }
                            result = true;
                            break;
                        }
                        return result;
                    });

                    if (siteID!=null)
                    {
                        var linkColumn = new LinkColumn();
                        linkColumn.LinkFieldName = "SITE_ID";
                        linkColumn.TableName = "MICROWS";
                        linkColumn.ValueLinkId = siteID;
                        linkColumn.TypeColumn = typeof(int);
                        linkColumn.FullSourceName = FullColumnName;
                        if (listLinkColumn.Find(x => x.LinkFieldName == linkColumn.LinkFieldName && x.TableName == linkColumn.TableName && FullColumnName.Contains(x.FullSourceName)) == null)
                        {
                            listLinkColumn.Add(linkColumn);
                        }
                    }

                    if (PlanID != null)
                    {
                        var linkColumn = new LinkColumn();
                        linkColumn.LinkFieldName = "PLAN_ID";
                        linkColumn.TableName = "MICROWS";
                        linkColumn.ValueLinkId = PlanID;
                        linkColumn.TypeColumn = typeof(int);
                        linkColumn.FullSourceName = FullColumnName;
                        if (listLinkColumn.Find(x => x.LinkFieldName == linkColumn.LinkFieldName && x.TableName == linkColumn.TableName && FullColumnName.Contains(x.FullSourceName)) == null)
                        {
                            listLinkColumn.Add(linkColumn);
                        }
                    }
                    if (ant1ID != null)
                    {
                        var linkColumn = new LinkColumn();
                        linkColumn.LinkFieldName = "ANT_ID";
                        linkColumn.TableName = "MICROWS";
                        linkColumn.ValueLinkId = ant1ID;
                        linkColumn.TypeColumn = typeof(int);
                        linkColumn.FullSourceName = FullColumnName;
                        if (listLinkColumn.Find(x => x.LinkFieldName == linkColumn.LinkFieldName && x.TableName == linkColumn.TableName && FullColumnName.Contains(x.FullSourceName)) == null)
                        {
                            listLinkColumn.Add(linkColumn);
                        }
                    }

                    if (ant2ID != null)
                    {
                        var linkColumn = new LinkColumn();
                        linkColumn.LinkFieldName = "ANT2_ID";
                        linkColumn.TableName = "MICROWS";
                        linkColumn.ValueLinkId = ant2ID;
                        linkColumn.TypeColumn = typeof(int);
                        linkColumn.FullSourceName = FullColumnName;
                        if (listLinkColumn.Find(x => x.LinkFieldName == linkColumn.LinkFieldName && x.TableName == linkColumn.TableName && FullColumnName.Contains(x.FullSourceName)) == null)
                        {
                            listLinkColumn.Add(linkColumn);
                        }
                    }


                    if (valueLinkFromColumn != null)
                    {
                        var linkColumn = new LinkColumn();
                        linkColumn.LinkFieldName = columnProperties.FieldJoinTo;
                        linkColumn.TableName = columnProperties.NameTableTo;
                        linkColumn.ValueLinkId = valueLinkFromColumn;
                        linkColumn.TypeColumn = columnProperties.TypeColumn;
                        linkColumn.FullSourceName = FullColumnName;
                        if (listLinkColumn.Find(x => x.LinkFieldName == linkColumn.LinkFieldName && x.TableName == linkColumn.TableName && FullColumnName.Contains(x.FullSourceName)) == null)
                        {
                            listLinkColumn.Add(linkColumn);
                        }
                        
                        if (columnProperties.NameField!=null)
                        {
                            if (primaryKeys != null)
                            {
                                linkColumnFinded = listLinkColumn.Find(x => x.LinkFieldName == primaryKeys.FieldJoinTo && x.TableName == columnProperties.NameTableTo && FullColumnName.Contains(x.FullSourceName));
                                if (linkColumnFinded != null)
                                {
                                    UpdateRecord(ref listLinkColumn, queryDescriptor, columnProperties, linkColumnFinded, linkValues, FullColumnName, updateAction, tokenData);
                                }
                            }
                        }

                    }
                    if (valuePrimaryKey != null)
                    {
                        var linkColumn = new LinkColumn();
                        linkColumn.LinkFieldName = primaryKeys.FieldJoinTo;
                        linkColumn.TableName = columnProperties.NameTableTo;
                        linkColumn.ValueLinkId = valuePrimaryKey;
                        linkColumn.TypeColumn = primaryKeys.TypeColumn;
                        linkColumn.FullSourceName = FullColumnName;
                        if (listLinkColumn.Find(x => x.LinkFieldName == linkColumn.LinkFieldName && x.TableName == linkColumn.TableName && FullColumnName.Contains(x.FullSourceName)) == null)
                        {
                            listLinkColumn.Add(linkColumn);
                        }

                    }



                    if (valueLinkFromColumn == null)
                    {
                        List<ColumnValue> listColumnValue = null;
                        var LinkColumnFindedFoInsert = listLinkColumn.Find(x => x.LinkFieldName == columnProperties.FieldJoinFrom && x.TableName == columnProperties.NameTableFrom && FullColumnName.Contains(x.FullSourceName));
                        if (LinkColumnFindedFoInsert != null)
                        {
                            if (columnProperties.FieldJoinFrom == LinkColumnFindedFoInsert.LinkFieldName)
                            {
                                listColumnValue = new List<ColumnValue>();
                                if (LinkColumnFindedFoInsert.TypeColumn == typeof(int))
                                {
                                    listColumnValue.Add( new IntegerColumnValue() { Name = columnProperties.FieldJoinTo, Value = LinkColumnFindedFoInsert.ValueLinkId as int?  });
                                }
                                else if (LinkColumnFindedFoInsert.TypeColumn == typeof(string))
                                {
                                    listColumnValue.Add(new StringColumnValue() { Name = columnProperties.FieldJoinTo, Value = LinkColumnFindedFoInsert.ValueLinkId as string });
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Unsupported data type for condition column value '{LinkColumnFindedFoInsert.TypeColumn}'");
                                }


                                if (columnProperties.Name != null)
                                {
                                    if ((columnProperties.NameTableTo == "MICROWS") && (columnProperties.Name.Contains("StationA")))
                                    {
                                        listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "A" });
                                        listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "B" });
                                    }
                                    if ((columnProperties.NameTableTo == "MICROWS") && (columnProperties.Name.Contains("StationB")))
                                    {
                                        listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "B" });
                                        listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "A" });
                                    }
                                    else if ((columnProperties.NameTableTo == "MICROWS") && (columnProperties.Name.Contains("StationPa")))
                                    {
                                        listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "Pa" });
                                        listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pb" });
                                    }
                                    if ((columnProperties.NameTableTo == "MICROWS") && (columnProperties.Name.Contains("StationPb")))
                                    {
                                        listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "Pb" });
                                        listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pa" });
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (listLinkColumn.Find(x => x.LinkFieldName == columnProperties.FieldJoinTo && x.TableName == columnProperties.NameTableTo && FullColumnName.Contains(x.FullSourceName)) == null)
                            {
                                var linkColumn = new LinkColumn();
                                linkColumn.LinkFieldName = columnProperties.FieldJoinTo;
                                linkColumn.TableName = columnProperties.NameTableTo;
                                linkColumn.ValueLinkId = idValueFieldJoinFrom;
                                linkColumn.TypeColumn = columnProperties.TypeColumn;
                                linkColumn.FullSourceName = FullColumnName;
                                linkColumn.IsUpdated = false;
                                linkColumn.IsMandatory = true;
                                listLinkColumn.Add(linkColumn);
                            }
                        }

                        var mandatoryColumns = queryDescriptor.MandatoryColumns.ToList();
                        InsertRecord(mandatoryColumns, primaryKeys, ref listLinkColumn, columnProperties, FullColumnName, updateAction, createAction, tokenData, queryDescriptor, listColumnValue);
                        if (columnProperties.NameField != null)
                        {
                            if (primaryKeys != null)
                            {
                                linkColumnFinded = listLinkColumn.Find(x => x.LinkFieldName == primaryKeys.FieldJoinTo && x.TableName == columnProperties.NameTableTo && FullColumnName.Contains(x.FullSourceName));
                                if (linkColumnFinded != null)
                                {
                                    UpdateRecord(ref listLinkColumn, queryDescriptor, columnProperties, linkColumnFinded, linkValues, FullColumnName, updateAction, tokenData);
                                }
                            }
                        }
                    }
                }
                else if (columnProperties.NameField != null)
                {
                    if (primaryKeys != null)
                    {

                        linkColumnFinded = listLinkColumn.Find(x => x.LinkFieldName == primaryKeys.FieldJoinTo && x.TableName == columnProperties.NameTableTo && FullColumnName.Contains(x.FullSourceName));
                        if (linkColumnFinded != null)
                        {
                            UpdateRecord(ref listLinkColumn, queryDescriptor, columnProperties, linkColumnFinded, linkValues, FullColumnName, updateAction, tokenData);
                        }
                    }
                }
            }
        }


        private void GetValueColumnByTableFrom(ref List<LinkColumn> listLinkColumn, ColumnProperties columnProperties, QueryDescriptor queryDescriptor, string FullColumnName)
        {
            object idValueFieldJoinFrom = null;
            object idValuePrimaryKeyFrom = null;
            var LinkColumnFindedFrom = listLinkColumn.Find(x => x.LinkFieldName == columnProperties.FieldJoinFrom && x.TableName == columnProperties.NameTableFrom);
            var primaryColumns = queryDescriptor.PrimaryColumns.ToList();
            var primaryKeys = primaryColumns.Find(z => z.NameTableTo == columnProperties.NameTableFrom);
            if (LinkColumnFindedFrom == null)
            {
                if (primaryKeys != null)
                {
                    LinkColumnFindedFrom = listLinkColumn.Find(x => x.LinkFieldName == primaryKeys.FieldJoinTo && x.TableName == columnProperties.NameTableFrom );
                    if (LinkColumnFindedFrom != null)
                    {
                        idValuePrimaryKeyFrom = LinkColumnFindedFrom.ValueLinkId;
                    }
                }
            }
            else
            {
                idValueFieldJoinFrom = LinkColumnFindedFrom.ValueLinkId;
            }

            if (LinkColumnFindedFrom != null)
            {
                if ((idValueFieldJoinFrom != null) || (idValuePrimaryKeyFrom != null) && (columnProperties.FieldJoinFrom != null) && (primaryKeys.FieldJoinTo != null))
                {
                    IQuerySelectStatement QueryNext = null;
                    if (idValuePrimaryKeyFrom != null)
                    {
                        QueryNext = _dataLayer.Builder
                       .From(columnProperties.NameTableFrom)
                       .Select(primaryKeys.FieldJoinTo)
                       .Select(columnProperties.FieldJoinFrom)
                       .OrderByDesc(new string[] { primaryKeys.FieldJoinTo });
                    }
                    else if (idValueFieldJoinFrom != null)
                    {
                        QueryNext = _dataLayer.Builder
                        .From(columnProperties.NameTableFrom)
                        .Select(primaryKeys.FieldJoinTo)
                        .Select(columnProperties.FieldJoinFrom)
                        .OrderByDesc(new string[] { columnProperties.FieldJoinFrom });
                    }


                    if (idValueFieldJoinFrom != null)
                    {
                        if (columnProperties.TypeColumn == typeof(int))
                        {
                            QueryNext
                            .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = columnProperties.FieldJoinFrom }, Operator = ConditionOperator.Equal, RightOperand = new IntegerValueOperand() { Value = idValueFieldJoinFrom as int? } });
                        }
                        else if (columnProperties.TypeColumn == typeof(string))
                        {
                            QueryNext
                            .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = columnProperties.FieldJoinFrom }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = idValueFieldJoinFrom as string } });
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported data type for condition column value '{columnProperties.TypeColumn}'");
                        }
                    }
                    else if (idValuePrimaryKeyFrom != null)
                    {
                        if (primaryKeys.TypeColumn == typeof(int))
                        {
                            QueryNext
                            .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = primaryKeys.FieldJoinTo }, Operator = ConditionOperator.Equal, RightOperand = new IntegerValueOperand() { Value = idValuePrimaryKeyFrom as int? } });
                        }
                        else if (primaryKeys.TypeColumn == typeof(string))
                        {
                            QueryNext
                            .Where(new ConditionExpression() { LeftOperand = new ColumnOperand() { ColumnName = primaryKeys.FieldJoinTo }, Operator = ConditionOperator.Equal, RightOperand = new StringValueOperand() { Value = idValuePrimaryKeyFrom as string } });
                        }
                        else
                        {
                            throw new InvalidOperationException($"Unsupported data type for condition column value '{primaryKeys.TypeColumn}'");
                        }
                    }

                    object valueLinkFromColumn = null;
                    object valuePrimaryKey = null;
                    this._queryExecutor
                    .Fetch(QueryNext, reader =>
                    {
                        var result = false;
                        while (reader.Read())
                        {
                            if (columnProperties.TypeColumn == typeof(int))
                            {
                                valueLinkFromColumn = reader.GetValueAsObject(DataType.Integer, reader.GetFieldType(reader.GetOrdinal(columnProperties.FieldJoinFrom)), reader.GetOrdinal(columnProperties.FieldJoinFrom));
                            }
                            else if (columnProperties.TypeColumn == typeof(string))
                            {
                                valueLinkFromColumn = reader.GetValueAsObject(DataType.String, reader.GetFieldType(reader.GetOrdinal(columnProperties.FieldJoinFrom)), reader.GetOrdinal(columnProperties.FieldJoinFrom));
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported data type for get column value '{columnProperties.TypeColumn}'");
                            }


                            if (primaryKeys.TypeColumn == typeof(int))
                            {
                                valuePrimaryKey = reader.GetValueAsObject(DataType.Integer, reader.GetFieldType(reader.GetOrdinal(primaryKeys.FieldJoinTo)), reader.GetOrdinal(primaryKeys.FieldJoinTo));
                            }
                            else if (primaryKeys.TypeColumn == typeof(string))
                            {
                                valuePrimaryKey = reader.GetValueAsObject(DataType.String, reader.GetFieldType(reader.GetOrdinal(primaryKeys.FieldJoinTo)), reader.GetOrdinal(primaryKeys.FieldJoinTo));
                            }
                            else
                            {
                                throw new InvalidOperationException($"Unsupported data type for get column value '{primaryKeys.TypeColumn}'");
                            }


                            result = true;
                        }
                        return result;
                    });
                    if (valueLinkFromColumn != null)
                    {
                        var linkColumn = new LinkColumn();
                        linkColumn.LinkFieldName = columnProperties.FieldJoinFrom;
                        linkColumn.TableName = columnProperties.NameTableFrom;
                        linkColumn.ValueLinkId = valueLinkFromColumn;
                        linkColumn.TypeColumn = columnProperties.TypeColumn;
                        linkColumn.FullSourceName = FullColumnName;
                        if (listLinkColumn.Find(x => x.LinkFieldName == linkColumn.LinkFieldName && x.TableName == linkColumn.TableName && FullColumnName.Contains(x.FullSourceName)) == null)
                        {
                            listLinkColumn.Add(linkColumn);
                        }
                    }
                    if (valuePrimaryKey != null)
                    {
                        var linkColumn = new LinkColumn();
                        linkColumn.LinkFieldName = primaryKeys.FieldJoinTo;
                        linkColumn.TableName = columnProperties.NameTableFrom;
                        linkColumn.ValueLinkId = valuePrimaryKey;
                        linkColumn.TypeColumn = primaryKeys.TypeColumn;
                        linkColumn.FullSourceName = FullColumnName;
                        if (listLinkColumn.Find(x => x.LinkFieldName == linkColumn.LinkFieldName && x.TableName == linkColumn.TableName && FullColumnName.Contains(x.FullSourceName)) == null)
                        {
                            listLinkColumn.Add(linkColumn);
                        }
                    }
                }
            }
        }




        private List<ColumnValue> GetColumnValuesFromLevel(List<LinkValue> listForInsertValues, IrpColumn[] irpDescrColumns, int level)
        {
            var listColumnValue = new List<ColumnValue>();
            var oneLevelValues = listForInsertValues.FindAll(z => z.Level == level);
            if (oneLevelValues != null)
            {
                foreach (var val in oneLevelValues)
                {
                    var value = irpDescrColumns.ToList();
                    var findedColumnMeta = value.Find(z => z.columnMeta.Name == val.Name);
                    if ((findedColumnMeta != null) && (findedColumnMeta.columnProperties != null))
                    {

                        for (int j = 0; j < findedColumnMeta.columnProperties.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(findedColumnMeta.columnProperties[j].NameField))
                            {
                                if (val.typeValue == DataType.Integer)
                                {
                                    if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                                    {
                                        listColumnValue.Add(new IntegerColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = val.Value as int? });
                                    }
                                }
                                else if (val.typeValue == DataType.String)
                                {
                                    if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                                    {
                                        listColumnValue.Add(new StringColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = val.Value as string });
                                    }
                                }
                                else if (val.typeValue == DataType.Double)
                                {
                                    if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                                    {
                                        listColumnValue.Add(new DoubleColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = val.Value as double? });
                                    }
                                }
                                else if (val.typeValue == DataType.DateTime)
                                {
                                    if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                                    {
                                        listColumnValue.Add(new DateTimeColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = val.Value as DateTime? });
                                    }
                                }
                                else if (val.typeValue == DataType.Float)
                                {
                                    if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                                    {
                                        listColumnValue.Add(new FloatColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = val.Value as float? });
                                    }
                                }

                                if (findedColumnMeta.columnProperties[j].Name != null)
                                {
                                    if ((findedColumnMeta.columnProperties[j].NameTableTo == "MICROWS") && (findedColumnMeta.columnProperties[j].Name.Contains("StationA")))
                                    {
                                        if (listColumnValue.Find(z => z.Name == "ROLE") == null)
                                        {
                                            listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "A" });
                                        }
                                        if (listColumnValue.Find(z => z.Name == "END_ROLE") == null)
                                        {
                                            listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "B" });
                                        }
                                    }
                                    if ((findedColumnMeta.columnProperties[j].NameTableTo == "MICROWS") && (findedColumnMeta.columnProperties[j].Name.Contains("StationB")))
                                    {
                                        if (listColumnValue.Find(z => z.Name == "ROLE") == null)
                                        {
                                            listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "B" });
                                        }
                                        if (listColumnValue.Find(z => z.Name == "END_ROLE") == null)
                                        {
                                            listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "A" });
                                        }

                                    }
                                    else if ((findedColumnMeta.columnProperties[j].NameTableTo == "MICROWS") && (findedColumnMeta.columnProperties[j].Name.Contains("StationPa")))
                                    {
                                        if (listColumnValue.Find(z => z.Name == "ROLE") == null)
                                        {
                                            listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "Pa" });
                                        }
                                        if (listColumnValue.Find(z => z.Name == "END_ROLE") == null)
                                        {
                                            listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pb" });
                                        }

                                    }
                                    if ((findedColumnMeta.columnProperties[j].NameTableTo == "MICROWS") && (findedColumnMeta.columnProperties[j].Name.Contains("StationPb")))
                                    {
                                        if (listColumnValue.Find(z => z.Name == "ROLE") == null)
                                        {
                                            listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "Pb" });
                                        }
                                        if (listColumnValue.Find(z => z.Name == "END_ROLE") == null)
                                        {
                                            listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pa" });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return listColumnValue;
        }

        private string GetNameColumnInDb(string fullColumnName)
        {
            string nameColumnInDb = "";
            var wrdChecked = fullColumnName.Split(new char[] { '.' });
            if ((wrdChecked != null) && (wrdChecked.Length > 0))
            {
                nameColumnInDb = wrdChecked[wrdChecked.Length - 1];
            }
            return nameColumnInDb;
        }

        private List<ColumnValue> GetColumnValues(IrpColumn[] irpDescrColumns, string tableName, LinkValue linkValue )
        {
            var listColumnValue = new List<ColumnValue>();
            var value = irpDescrColumns.ToList();
            var findedColumnMeta = value.Find(z => z.columnMeta.Name == linkValue.Name);
            if ((findedColumnMeta != null) && (findedColumnMeta.columnProperties != null))
            {
                for (int j = 0; j < findedColumnMeta.columnProperties.Length; j++)
                {
                    if ((!string.IsNullOrEmpty(findedColumnMeta.columnProperties[j].NameField)) && (tableName == findedColumnMeta.columnProperties[j].NameTableTo))
                    {
                        if (linkValue.typeValue == DataType.Integer)
                        {
                            if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                            {
                                listColumnValue.Add(new IntegerColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = linkValue.Value as int? });
                            }
                        }
                        else if (linkValue.typeValue == DataType.String)
                        {
                            if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                            {
                                listColumnValue.Add(new StringColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = linkValue.Value as string });
                            }
                        }
                        else if (linkValue.typeValue == DataType.Double)
                        {
                            if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                            {
                                listColumnValue.Add(new DoubleColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = linkValue.Value as double? });
                            }
                        }
                        else if (linkValue.typeValue == DataType.DateTime)
                        {
                            if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                            {
                                listColumnValue.Add(new DateTimeColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = linkValue.Value as DateTime? });
                            }
                        }
                        else if (linkValue.typeValue == DataType.Float)
                        {
                            if (listColumnValue.Find(z => z.Name == findedColumnMeta.columnProperties[j].NameField) == null)
                            {
                                listColumnValue.Add(new FloatColumnValue() { Name = findedColumnMeta.columnProperties[j].NameField, Value = linkValue.Value as float? });
                            }
                        }

                        if (findedColumnMeta.columnProperties[j].Name != null)
                        {
                            if ((findedColumnMeta.columnProperties[j].NameTableTo == "MICROWS") && (findedColumnMeta.columnProperties[j].Name.Contains("StationA")))
                            {
                                if (listColumnValue.Find(z => z.Name == "ROLE") == null)
                                {
                                    listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "A" });
                                }
                                if (listColumnValue.Find(z => z.Name == "END_ROLE") == null)
                                {
                                    listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "B" });
                                }
                            }
                            if ((findedColumnMeta.columnProperties[j].NameTableTo == "MICROWS") && (findedColumnMeta.columnProperties[j].Name.Contains("StationB")))
                            {
                                if (listColumnValue.Find(z => z.Name == "ROLE") == null)
                                {
                                    listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "B" });
                                }
                                if (listColumnValue.Find(z => z.Name == "END_ROLE") == null)
                                {
                                    listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "A" });
                                }

                            }
                            else if ((findedColumnMeta.columnProperties[j].NameTableTo == "MICROWS") && (findedColumnMeta.columnProperties[j].Name.Contains("StationPa")))
                            {
                                if (listColumnValue.Find(z => z.Name == "ROLE") == null)
                                {
                                    listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "Pa" });
                                }
                                if (listColumnValue.Find(z => z.Name == "END_ROLE") == null)
                                {
                                    listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pb" });
                                }

                            }
                            if ((findedColumnMeta.columnProperties[j].NameTableTo == "MICROWS") && (findedColumnMeta.columnProperties[j].Name.Contains("StationPb")))
                            {
                                if (listColumnValue.Find(z => z.Name == "ROLE") == null)
                                {
                                    listColumnValue.Add(new StringColumnValue() { Name = "ROLE", Value = "Pb" });
                                }
                                if (listColumnValue.Find(z => z.Name == "END_ROLE") == null)
                                {
                                    listColumnValue.Add(new StringColumnValue() { Name = "END_ROLE", Value = "Pa" });
                                }
                            }
                        }
                    }
                }
            }
            return listColumnValue;
        }

        private List<ColumnValue> GetColumnValuesFromLevel(List<LinkValue> listForInsertValues, IrpColumn[] irpDescrColumns, string tableName, string FullColumnName)
        {
            var listColumnValue = new List<ColumnValue>();
            if (!string.IsNullOrEmpty(FullColumnName))
            {
                var oneLevelValues = listForInsertValues;
                if (oneLevelValues != null)
                {
                    var wrdChecked = FullColumnName.Split(new char[] { '.' });
                    var prefixColumn = "";
                    if ((wrdChecked != null) && (wrdChecked.Length > 0))
                    {
                        if (wrdChecked.Length == 1)
                        {
                            prefixColumn = wrdChecked[0];
                        }
                        else if (wrdChecked.Length > 1)
                        {
                            for (int i = 0; i <= wrdChecked.Length - 2; i++)
                            {
                                prefixColumn += wrdChecked[i] + ".";
                            }
                            if (prefixColumn.Length > 0)
                            {
                                prefixColumn = prefixColumn.Remove(prefixColumn.Length - 1, 1);
                            }
                        }
                    }
                    foreach (var val in oneLevelValues)
                    {
                        if (val.Name.Contains(prefixColumn))
                        {
                            listColumnValue.AddRange(GetColumnValues(irpDescrColumns, tableName, val));
                        }
                    }
                }
            }
            else
            {
                var oneLevelValues = listForInsertValues;
                foreach (var val in oneLevelValues)
                {
                    listColumnValue.AddRange(GetColumnValues(irpDescrColumns, tableName, val));
                }
            }
            return listColumnValue;
        }

        private string GetSourceColumnName(List<IrpColumn> irpDescrColumns, string fullColumnName)
        {
            string retSourceName = null;
            var findedColumnMeta = irpDescrColumns.Find(z => z.columnMeta.Name == fullColumnName);
            if ((findedColumnMeta != null) && (findedColumnMeta.columnProperties != null))
            {
                if (findedColumnMeta.columnProperties.Length > 0)
                {
                    retSourceName = findedColumnMeta.columnProperties[findedColumnMeta.columnProperties.Length - 1].NameField;
                }
            }
            return retSourceName;
        }


        private int UpdateData(UserTokenData userTokenData, QueryDescriptor queryDescriptor, UpdationAction updateAction, CreationAction creationAction)
        {
            int maxLevel = 1;
            int recordsAffected = 0;
            var listLinkColumn = new List<LinkColumn>();
            var mandatoryColumns = queryDescriptor.MandatoryColumns.ToList();
            var primaryColumns = queryDescriptor.PrimaryColumns.ToList();
            ColumnValue[] unPackValues = null;
            List<ColumnValue> valuesList = null;
            var irpDescrColumns = queryDescriptor.IrpDescrColumns;
            var IdentValueTable = new List<KeyValuePair<int?, string>>();
            string[] distinctTables = null;
            var listDescrColumns = irpDescrColumns.ToList();
            maxLevel = listDescrColumns.Max(z => z.columnProperties.Length);
            var valuesFromColumns = new List<LinkValue>();
            int? idValue = null;
            int maxLevelsByValues = 0;
            if (creationAction != null)
            {
                queryDescriptor.CheckColumns(creationAction.Columns);
                distinctTables = GetTablesForInsert(irpDescrColumns, creationAction);
                unPackValues = this.UnpackInsertedValues(creationAction);
                valuesList = unPackValues.ToList();
                for (int i = 0; i < unPackValues.Length; i++)
                {
                    valuesFromColumns.Add(new LinkValue()
                    {
                        Level = unPackValues[i].Name.Count(x => x == '.') + 1,
                        Name = unPackValues[i].Name,
                        Value = GetValue(unPackValues[i]),
                        typeValue = unPackValues[i].DataType,
                        SourceColumnName = GetSourceColumnName(listDescrColumns, unPackValues[i].Name)
                    });
                }
            }
            else if (updateAction != null)
            {
                queryDescriptor.CheckColumns(updateAction.Columns);
                if (updateAction.Condition != null)
                {
                    queryDescriptor.CheckCondition(updateAction.Condition);
                }
                distinctTables = GetTablesForUpdate(irpDescrColumns, updateAction);
                unPackValues = this.UnpackUpdatedValues(updateAction);
                valuesList = unPackValues.ToList();
                idValue = (((updateAction.Condition as ComplexCondition).Conditions[0] as ConditionExpression).RightOperand as IntegerValueOperand).Value;
                for (int i = 0; i < unPackValues.Length; i++)
                {
                    var QueryNext = _dataLayer.Builder
                                         .From(queryDescriptor.TableName)
                                         .Where(updateAction.Condition)
                                         .Select(unPackValues[i].Name);
                    object val = null;
                    this._queryExecutor
                    .Fetch(QueryNext, reader =>
                    {
                        var result = false;
                        while (reader.Read())
                        {
                            val = reader.GetValueAsObject(unPackValues[i].DataType, reader.GetFieldType(reader.GetOrdinal(unPackValues[i].Name)), reader.GetOrdinal(unPackValues[i].Name));
                            result = true;
                        }
                        return result;
                    });
                    valuesFromColumns.Add(new LinkValue()
                    {
                        Level = unPackValues[i].Name.Count(x => x == '.') + 1,
                        Name = unPackValues[i].Name,
                        Value = val,
                        typeValue = unPackValues[i].DataType,
                        SourceColumnName = GetSourceColumnName(listDescrColumns, unPackValues[i].Name)
                    });
                }
            }

            List<ColumnValue> listColumnValue = null;
            if (creationAction != null)
            {
                //Инициализация списка начальным значением - ID выбранной записи
                // Получить список значений для обновления в БД
                var listForInsertValues = new List<LinkValue>();
                foreach (var val in valuesFromColumns)
                {
                    var value = irpDescrColumns.ToList();
                    var findedColumnMeta = value.Find(z => z.columnMeta.Name == val.Name);
                    if ((findedColumnMeta != null) && (findedColumnMeta.columnProperties != null))
                    {
                        for (int j = 0; j < findedColumnMeta.columnProperties.Length; j++)
                        {
                            var columnValue = valuesList.Find(z => z.Name == findedColumnMeta.columnMeta.Name);
                            if (columnValue != null)
                            {
                                if (listForInsertValues.Find(z => z.Name == val.Name) == null)
                                {
                                    listForInsertValues.Add(val);
                                }
                            }
                        }
                    }
                }
                maxLevelsByValues = listForInsertValues.Max(x => x.Level);
                listColumnValue = GetColumnValuesFromLevel(listForInsertValues, irpDescrColumns, 1);
                var primaryKeysForInsertAction = primaryColumns.Find(z => z.NameTableTo == queryDescriptor.TableName);
                idValue = InsertRecord(mandatoryColumns, primaryKeysForInsertAction, ref listLinkColumn, queryDescriptor.TableName, "ID", updateAction, creationAction, userTokenData, queryDescriptor, listColumnValue);
            }



            if (updateAction != null)
            {
                //Инициализация списка начальным значением - ID выбранной записи
                // Получить список значений для обновления в БД
                var listForUpdatedValues = new List<LinkValue>();
                foreach (var val in valuesFromColumns)
                {
                    var value = irpDescrColumns.ToList();
                    var findedColumnMeta = value.Find(z => z.columnMeta.Name == val.Name);
                    if ((findedColumnMeta != null) && (findedColumnMeta.columnProperties != null))
                    {
                        for (int j = 0; j < findedColumnMeta.columnProperties.Length; j++)
                        {
                            var columnValue = valuesList.Find(z => z.Name == findedColumnMeta.columnMeta.Name);
                            if (columnValue != null)
                            {
                                object valueUser = GetValue(columnValue);
                                if (val.Value != valueUser)
                                {
                                    if (listForUpdatedValues.Find(z => z.Name == val.Name) == null)
                                    {
                                        val.Value = valueUser;
                                        listForUpdatedValues.Add(val);
                                    }
                                }
                            }
                        }
                    }
                }
                maxLevelsByValues = listForUpdatedValues.Max(x => x.Level);
            }


            // Запуск процедуры обновления

            listLinkColumn.Clear();
            var primaryKeys = primaryColumns.Find(z => z.NameTableTo == queryDescriptor.TableName);
            var linkColumn = new LinkColumn();
            linkColumn.LinkFieldName = primaryKeys.FieldJoinTo;
            linkColumn.TableName = queryDescriptor.TableName;
            linkColumn.ValueLinkId = idValue;
            linkColumn.TypeColumn = primaryKeys.TypeColumn;
            linkColumn.FullSourceName = primaryKeys.FieldJoinTo;
            listLinkColumn.Add(linkColumn);

            for (int i = maxLevelsByValues; i >= 1; i--)
            {
                var findValuesByLevel = valuesFromColumns.FindAll(z => z.Level == i);
                foreach (var val in findValuesByLevel)
                {
                    var value = irpDescrColumns.ToList();
                    var findedColumnMeta = value.Find(z => z.columnMeta.Name == val.Name);
                    if ((findedColumnMeta != null) && (findedColumnMeta.columnProperties != null))
                    {
                        for (int j = 0; j < findedColumnMeta.columnProperties.Length; j++)
                        {
                            string findPrefix = val.Name;
                            if (!string.IsNullOrEmpty(findedColumnMeta.columnProperties[j].Name))
                            {
                                if (findPrefix.IndexOf(findedColumnMeta.columnProperties[j].Name) != -1)
                                {
                                    int idx = findPrefix.IndexOf(findedColumnMeta.columnProperties[j].Name) + findedColumnMeta.columnProperties[j].Name.Length;
                                    findPrefix = findPrefix.Substring(0, idx);
                                }
                            }

                            GetValueColumnByTableFrom(ref listLinkColumn, findedColumnMeta.columnProperties[j], queryDescriptor, findPrefix);
                            var listColumnValuesUpdIns = GetColumnValuesFromLevel(findValuesByLevel, irpDescrColumns, findedColumnMeta.columnProperties[j].NameTableTo, findPrefix);
                            GetLinkColumnByTableTo(ref listLinkColumn, findedColumnMeta.columnProperties[j], queryDescriptor, findValuesByLevel, listColumnValuesUpdIns, findPrefix, updateAction, creationAction, userTokenData);
                        }
                    }
                }
            }
            return recordsAffected;
        }



        private int PerformCreationAction(UserTokenData userTokenData, QueryDescriptor queryDescriptor, CreationAction action)
        {
            int recordsAffected = 0;
            this._queryExecutor.BeginTransaction();
            try
            {
                 recordsAffected = UpdateData(userTokenData, queryDescriptor, null, action);
                this._queryExecutor.CommitTransaction();
            }
            catch (Exception e)
            {
                this._queryExecutor.RollbackTransaction();
                this.Logger.Exception(Contexts.ErrorInsertOperation, Categories.Handling, e, this);
                throw new InvalidOperationException("Failed to insert data :" +e.Message, e);
            }
            return recordsAffected;
        }

        private int PerformUpdationAction(UserTokenData userTokenData, QueryDescriptor queryDescriptor, UpdationAction action)
        {
            int recordsAffected = 0;
            this._queryExecutor.BeginTransaction();
            try
            {
                recordsAffected = UpdateData(userTokenData, queryDescriptor,action, null);
                this._queryExecutor.CommitTransaction();
            }
            catch (Exception e)
            {
                this._queryExecutor.RollbackTransaction();
                this.Logger.Exception(Contexts.ErrorUpdateOperation, Categories.Handling, e, this);
                throw new InvalidOperationException("Failed to update data: "+e.Message, e);
            }
            return recordsAffected;
        }

        private ColumnValue[] UnpackUpdatedValues(UpdationAction action)
        {
            switch (action.RowType)
            {
                case DataRowType.TypedCell:
                    return ((TypedRowUpdationAction)action).Row.GetColumnsValues(action.Columns);
                case DataRowType.StringCell:
                    return ((StringRowUpdationAction)action).Row.GetColumnsValues(action.Columns);
                case DataRowType.ObjectCell:
                    return ((ObjectRowUpdationAction)action).Row.GetColumnsValues(action.Columns);
                default:
                    throw new InvalidOperationException(Exceptions.DataRowTypeNotSupported.With(action.RowType));
            }
        }

        private ColumnValue[] UnpackInsertedValues(CreationAction action)
        {
            switch (action.RowType)
            {
                case DataRowType.TypedCell:
                    return ((TypedRowCreationAction)action).Row.GetColumnsValues(action.Columns);
                case DataRowType.StringCell:
                    return ((StringRowCreationAction)action).Row.GetColumnsValues(action.Columns);
                case DataRowType.ObjectCell:
                    return ((ObjectRowCreationAction)action).Row.GetColumnsValues(action.Columns);
                default:
                    throw new InvalidOperationException(Exceptions.DataRowTypeNotSupported.With(action.RowType));
            }
        }


        private int PerformDeleteionAction(UserTokenData userTokenData, QueryDescriptor queryDescriptor, DeletionAction action)
        {
            if (action.Condition != null)
            {
                queryDescriptor.CheckCondition(action.Condition);
            }

            var deletionQuery = this._dataLayer.Builder
                .Delete(queryDescriptor.TableName)
                .Where(action.Condition);

            var listColumnValues = new List<ColumnValue>();
            queryDescriptor.GetAllColumnValuesFromCondition(action.Condition, ref listColumnValues);
            var arr = listColumnValues.ToArray();
            queryDescriptor.PrapareValidationConditions(userTokenData, arr, action);
            var queryConditions = queryDescriptor.GetConditions(userTokenData, arr, action);
            if (queryConditions != null && queryConditions.Length > 0)
            {
                deletionQuery.Where(queryConditions);
            }

            var recordsAffected = this._queryExecutor.Execute(deletionQuery);
            return recordsAffected;
        }
    }

}
