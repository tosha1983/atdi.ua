
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.DataModels;
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
            return 0;
        }
        private int PerformUpdationAction(UserTokenData userTokenData, QueryDescriptor queryDescriptor, UpdationAction action)
        {
            return 0;
        }
        private int PerformDeleteionAction(UserTokenData userTokenData, QueryDescriptor queryDescriptor, DeletionAction action)
        {
            var deletionQuery = this._dataLayer.Builder
                .Delete(queryDescriptor.TableName)
                .Where(action.Condition);

            var queryConditions = queryDescriptor.GetConditions(userTokenData);
            if (queryConditions != null && queryConditions.Length > 0)
            {
                deletionQuery.Where(queryConditions);
            }

            var recordsAffected = this._queryExecutor.Execute(deletionQuery);
            return recordsAffected;
        }
    }
}
