﻿
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Atdi.WebApiServices.EntityOrm.Controllers
{
    [RoutePrefix("api/orm/data")]
    public class DataController : WebApiController
    {
        private readonly IEntityOrmConfig _ormConfig;
        private readonly IEntityOrm _entityOrm;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public DataController(IEntityOrmConfig ormConfig, IEntityOrm entityOrm, IDataLayer<EntityDataOrm> dataLayer, ILogger logger) : base(logger)
        {
            this._ormConfig = ormConfig;
            this._entityOrm = entityOrm;
            this._dataLayer = dataLayer;
        }

        [HttpGet]
        [Route("{context}/{ns}/{entity}")]
        public IHttpActionResult GetDataSet(string context, string ns, string entity, [FromUri] string[] select = null, [FromUri] string[] filter = null, [FromUri] string[] orderBy = null, [FromUri] int top = -1)
        {
            var query = new DTO.DataSetRequest
            {
                Context = context,
                Namespace = ns,
                Entity = entity,
                Select = select,
                Filter = filter,
                OrderBy = orderBy,
                Top = top
            };

            return this.GetDataSet(query);
        }

        [HttpPost]
        [Route("$DataSet")]
        public IHttpActionResult GetDataSet([FromBody] DTO.DataSetRequest query)
        {
            if (!this.CheckQuery(query, out string messaage))
            {
                return BadRequest(messaage);
            }

            if (!this.TryEnsureEntityMetadata(query.QName, out IEntityMetadata entity, out messaage))
            {
                return BadRequest(messaage);
            }

            if (!this.TryParseFields(entity, query.Select, out DTO.FieldDescriptor[] fields, out messaage))
            {
                return BadRequest(messaage);
            }

            var ormQuery = _dataLayer.Builder
                .From(query.QName)
                .Select(fields.Select(f => f.Path).ToArray());

            if (query.Top > 0)
            {
                ormQuery.OnTop((int)query.Top);
            }
            if (query.Filter != null && query.Filter.Length > 0)
            {
                var condition = Helpers.FilterParser.Parse(query.Filter);
                ormQuery.Where(condition);
            }
            if (query.OrderBy != null && query.OrderBy.Length > 0)
            {
                var orderByExpressions = Helpers.FieldParser.ParseOrderBy(query.OrderBy);
                ormQuery.OrderBy(orderByExpressions);
            }

            using (var scope = _dataLayer.CreateScope(new SimpleDataContext(query.Context)))
            {
                return scope.Executor.ExecuteAndFetch(ormQuery, reader =>
                {
                    var result = new DTO.DataSetResult
                    {
                        Fields = fields
                    };
                    var records = new List<object[]>();
                    long count = 0;
                    while (reader.Read())
                    {
                        var record = new object[fields.Length];
                        for (int i = 0; i < fields.Length; i++)
                        {
                            var field = fields[i];
                            record[i] = reader.GetValue((DataType)field.Type.VarTypeCode, field.Path);
                        }
                        records.Add(record);
                        ++count;
                    }
                    result.Records = records.ToArray();
                    result.Count = count;
                    return (IHttpActionResult)Ok(result);
                });
            }
        }


        

        [HttpGet]
        [Route("{context}/{ns}/{entity}/{primaryKey}")]
        public IHttpActionResult GetDataRecord(string context, string ns, string entity, string primaryKey, [FromUri] string[] select = null, [FromUri] string[] filter = null)
        {
            var query = new DTO.DataRecordRequest
            {
                Context = context,
                Namespace = ns,
                Entity = entity,
                Select = select,
                Filter = filter,
                PrimaryKey = primaryKey
            };

            return this.GetDataRecord(query);
        }

        [HttpPost]
        [Route("$Record")]
        public IHttpActionResult GetDataRecord([FromBody] DTO.DataRecordRequest query)
        {
            if (!this.CheckQuery(query, out string messaage))
            {
                return BadRequest(messaage);
            }

            if (!this.TryEnsureEntityMetadata(query.QName, out IEntityMetadata entity, out messaage))
            {
                return BadRequest(messaage);
            }
            if (!this.TryParseFields(entity, query.Select, out DTO.FieldDescriptor[] fields, out messaage))
            {
                return BadRequest(messaage);
            }
            if (!this.TryEnsurePrimaryKeyCondition(entity, query.PrimaryKey, out Condition pkCondition, out messaage))
            {
                return BadRequest(messaage);
            }

            var ormQuery = _dataLayer.Builder
                .From(query.QName)
                .Select(fields.Select(f => f.Path).ToArray())
                .Where(pkCondition)
                .OnTop(1);

            if (query.Filter != null && query.Filter.Length > 0)
            {
                var condition = Helpers.FilterParser.Parse(query.Filter);
                ormQuery.Where(condition);
            }

            using (var scope = _dataLayer.CreateScope(new SimpleDataContext(query.Context)))
            {
                return scope.Executor.ExecuteAndFetch(ormQuery, reader =>
                {
                    if (reader.Read())
                    {
                        var result = new DTO.RecordResult
                        {
                            Record = new object[fields.Length],
                            Fields = fields
                        };
                        for (int i = 0; i < fields.Length; i++)
                        {
                            var field = fields[i];
                            result.Record[i] = reader.GetValue((DataType)field.Type.VarTypeCode, field.Path);
                        }
                        return (IHttpActionResult)Ok(result);
                    }
                    return NotFound();
                });
            }
        }

        /// <summary>
        /// GET /api/orm/data/context/namespace/entity/1/path?filter=(field1 eq 'sdkfjsdfjkdkdj')or(field2 in (23,32,55))or(field3 isnull)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ns"></param>
        /// <param name="entity"></param>
        /// <param name="primaryKey"></param>
        /// <param name="fieldPath"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{context}/{ns}/{entity}/{primaryKey}/{fieldPath}")]
        public IHttpActionResult GetFieldValue(string context, string ns, string entity, string primaryKey, string fieldPath, [FromUri] string[] filter = null)
        {
            var query = new DTO.FieldValueRequest
            {
                Context = context,
                Namespace = ns,
                Entity = entity,
                PrimaryKey = primaryKey,
                FieldPath = fieldPath,
                Filter = filter
            };

            return this.GetFieldValue(query);
        }

        [HttpPost]
        [Route("$FieldValue")]
        public IHttpActionResult GetFieldValue([FromBody] DTO.FieldValueRequest query)
        {
            if (!this.CheckQuery(query, out string messaage))
            {
                return BadRequest(messaage);
            }
            
            if (!this.TryEnsureEntityMetadata(query.QName, out IEntityMetadata entity, out messaage))
            {
                return BadRequest(messaage);
            }

            if (!this.TryEnsureColumnField(entity, query.FieldPath, out IFieldMetadata field, out messaage))
            {
                return BadRequest(messaage);
            }

            if (!this.TryEnsurePrimaryKeyCondition(entity, query.PrimaryKey, out Condition pkCondition, out messaage))
            {
                return BadRequest(messaage);
            }

            var ormQuery = _dataLayer.Builder
                .From(query.QName)
                .Select(query.FieldPath)
                .Where(pkCondition)
                .OnTop(1);

            if (query.Filter != null && query.Filter.Length > 0)
            {
                var condition = Helpers.FilterParser.Parse(query.Filter);
                ormQuery.Where(condition);
            }

            using (var scope = _dataLayer.CreateScope(new SimpleDataContext(query.Context)))
            {
                return scope.Executor.ExecuteAndFetch(ormQuery, reader => 
                {
                    if (reader.Read())
                    {
                        var result = new DTO.FieldValueResult
                        {
                            Field = new DTO.FieldDescriptor
                            {
                                Index = 0,
                                Path = query.FieldPath,
                                Type = new DTO.DataTypeMetadata(field.DataType)
                            },
                            Value = reader.GetValue(field.DataType.CodeVarType, query.FieldPath)
                        };
                        return (IHttpActionResult)Ok(result);
                    }
                    return NotFound();
                });
            }
        }

        private bool TryParseFields(IEntityMetadata entity, string[] select, out DTO.FieldDescriptor[] fields, out string message)
        {
            var parsedSelect = Helpers.FieldParser.ParseSelect(select);
            fields = new DTO.FieldDescriptor[parsedSelect.Length];
            for (int i = 0; i < parsedSelect.Length; i++)
            {
                var path = parsedSelect[i];
                if (!this.TryEnsureColumnField(entity, path, out IFieldMetadata field, out message))
                {
                    return false;
                }
                fields[i] = new DTO.FieldDescriptor
                {
                    Path = path,
                    Type = new DTO.DataTypeMetadata(field.DataType),
                    Index = i
                };
            }
            message = null;
            return true;
        }

        private bool TryEnsurePrimaryKeyCondition(IEntityMetadata entity, string primaryKey, out Condition condition, out string message)
        {
            try
            {
                if (string.IsNullOrEmpty(primaryKey))
                {
                    message = "Undefined primary key value";
                    condition = null;
                    return false;
                }
                var pkMetadata = entity.DefinePrimaryKey();
                if (pkMetadata == null || pkMetadata.FieldRefs == null || pkMetadata.FieldRefs.Count == 0)
                {
                    message = $"Entity '{entity.QualifiedName}' has not primary key.";
                    condition = null;
                    return false;
                }
                var pkFields = pkMetadata.FieldRefs.Values.ToArray();
                if (pkFields.Length == 1)
                {
                    var pkField = pkFields[0].Field;
                    condition = new ConditionExpression
                    {
                        LeftOperand = new ColumnOperand
                        {
                            ColumnName = pkFields[0].Field.Name
                        },
                        Operator = ConditionOperator.Equal,
                        RightOperand = ValueOperand.Create(pkField.DataType.CodeVarType, Helpers.ValueParser.ParseValue(primaryKey))
                    };
                    message = null;
                    return true;
                }
                var values = Helpers.ValueParser.ParseValues(primaryKey);
                if (values.Length != pkFields.Length)
                {
                    message = $"Invalid primary key values '{primaryKey}'. Expected {pkFields.Length} of values.";
                    condition = null;
                    return false;
                }
                condition = new ComplexCondition
                {
                    Conditions = new Condition[pkFields.Length],
                    Operator = LogicalOperator.And
                };

                for (int i = 0; i < pkFields.Length; i++)
                {
                    var pkField = pkFields[i].Field;
                    
                    ((ComplexCondition)condition).Conditions[i] = new ConditionExpression
                    {
                        LeftOperand = new ColumnOperand
                        {
                            ColumnName = pkField.Name
                        },
                        Operator = ConditionOperator.Equal,
                        RightOperand = ValueOperand.Create(pkField.DataType.CodeVarType, values[i])
                    };
                }
                message = null;
                return true;
            }
            catch (Exception e)
            {
                message = e.ToString();
                condition = null;
                return false;
            }
        }

        private bool TryEnsureEntityMetadata(string name, out IEntityMetadata entity, out string message)
        {
            try
            {
                entity = _entityOrm.GetEntityMetadata(name);
                message = null;
                return true;
            }
            catch (Exception e)
            {
                message = e.ToString();
                entity = null;
                return false;
            }
        }

        private bool TryEnsureColumnField(IEntityMetadata entity, string path, out IFieldMetadata field, out string message)
        {
            var result = true;

            if (string.IsNullOrEmpty(path))
            {
                message = "Undefined field path";
                field = null;
                result = false;
            }
            else if (!entity.TryGetEndFieldByPath(path, out field))
            {
                message = $"Not found field by path '{path}'";
                result = false;
            }
            else if (field.SourceType != FieldSourceType.Column)
            {
                message = $"Expected column field for selection. The source type by path '{path}' is {field.SourceType} with dosen't use this context. ";
                result = false;
            }
            else
            {
                message = null;
            }

            return result;
        }

        private bool CheckQuery(DTO.EntityRequest query, out string message)
        {
            var result = true;

            if (string.IsNullOrEmpty(query.Context))
            {
                message = "Undefined data context";
                result = false;
            }
            else if (string.IsNullOrEmpty(query.Namespace))
            {
                message = "Undefined namespace";
                result = false;
            }
            else if (string.IsNullOrEmpty(query.Entity))
            {
                message = "Undefined entity name";
                result = false;
            }
            else
            {
                message = null;
            }
            
            return result;
            
        }
    }
}