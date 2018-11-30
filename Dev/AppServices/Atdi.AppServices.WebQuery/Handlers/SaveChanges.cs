
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

        private int PerformCreationAction(UserTokenData userTokenData, QueryDescriptor queryDescriptor, CreationAction action)
        {
            queryDescriptor.CheckColumns(action.Columns);
            var unPackValues = this.UnpackInsertedValues(action);

            queryDescriptor.PrapareValidationConditions(userTokenData, unPackValues, action);
            queryDescriptor.GetConditions(userTokenData, unPackValues, action);

            var insertQuery = this._dataLayer.Builder
                .Insert(queryDescriptor.TableName)
                .SetValues(unPackValues);

            var recordsAffected = this._queryExecutor.Execute(insertQuery);
            return recordsAffected;
        }

        private int PerformUpdationAction(UserTokenData userTokenData, QueryDescriptor queryDescriptor, UpdationAction action)
        {
            int recordsAffected = 0;
            queryDescriptor.CheckColumns(action.Columns);

            if (action.Condition != null)
            {
                queryDescriptor.CheckCondition(action.Condition);
            }

            var unPackValues = this.UnpackUpdatedValues(action);
            var updationQuery = this._dataLayer.Builder
                .Update(queryDescriptor.TableName)
                .Where(action.Condition);

            queryDescriptor.PrapareValidationConditions(userTokenData, unPackValues, action);
            var queryConditions = queryDescriptor.GetConditions(userTokenData, unPackValues, action);
            if (queryConditions != null && queryConditions.Length > 0)
            {
                updationQuery.Where(queryConditions);
            }
            if (unPackValues.Length > 0)
            {
                updationQuery.SetValues(unPackValues);
                recordsAffected = this._queryExecutor.Execute(updationQuery);
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
