using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using Atdi.DataModels;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.Identity;

namespace Atdi.AppServices.WebQuery.Handlers
{
    public sealed class ExecuteQuery : LoggedObject
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly QueriesRepository _repository;
        private readonly IUserTokenProvider _tokenProvider;
        private readonly IQueryExecutor _queryExecutor;
        


        public ExecuteQuery(QueriesRepository repository, IUserTokenProvider tokenProvider, IDataLayer<IcsmDataOrm> dataLayer, ILogger logger) : base(logger)
        {
            this._repository = repository;
            this._tokenProvider = tokenProvider;
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();

        }


        public QueryResult Handle(UserToken userToken, QueryToken queryToken, FetchOptions fetchOptions)
        {
            using (this.Logger.StartTrace(Contexts.WebQueryAppServices, Categories.Handling, TraceScopeNames.ExecuteQuery))
            {
                if (fetchOptions == null)
                {
                    throw new ArgumentNullException(nameof(fetchOptions));
                }

                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                var queryDescriptor = this._repository.GetQueryDescriptorByToken(tokenData, queryToken);

                // 1
                string[] selectedColumns = null;
                if (fetchOptions.Columns != null && fetchOptions.Columns.Length > 0)
                {
                    selectedColumns = fetchOptions.Columns;
                    queryDescriptor.CheckColumns(selectedColumns);
                }
                else
                {
                    selectedColumns = queryDescriptor.Metadata.Columns.Select(t => t.Name).ToArray();
                }

                //2
                var allConditions = new List<DataModels.DataConstraint.Condition>(queryDescriptor.GetConditions(tokenData));
                var optionsCondition = fetchOptions.Condition;
                if (optionsCondition != null)
                {
                    queryDescriptor.CheckCondition(optionsCondition);
                    allConditions.Add(optionsCondition);
                }

                var statement = this._dataLayer.Builder
                   .From(queryDescriptor.TableName)
                   .Select(queryDescriptor.PreperedColumnsForFetching(selectedColumns));

                if (allConditions.Count > 0)
                {
                    statement
                   .Where(allConditions.ToArray());
                }

                //3
                if (fetchOptions.Orders != null && fetchOptions.Orders.Length > 0)
                {
                    queryDescriptor.CheckColumns(fetchOptions.Orders);
                    statement.OrderBy(fetchOptions.Orders); 
                }
                else
                {
                    Atdi.DataModels.DataConstraint.OrderExpression[] orderExpressions = new DataModels.DataConstraint.OrderExpression[queryDescriptor.Metadata.Columns.Length];
                    for (int j=0; j< queryDescriptor.Metadata.Columns.Length; j++)
                    {
                        orderExpressions[j] = new Atdi.DataModels.DataConstraint.OrderExpression() { ColumnName = queryDescriptor.Metadata.Columns[j].Name, OrderType = queryDescriptor.Metadata.Columns[j].Order };
                    }
                    queryDescriptor.CheckColumns(orderExpressions);
                    statement.OrderBy(orderExpressions);
                }

                statement.SetLimit(fetchOptions.Limit);
                var fetchedColumns = selectedColumns.Select(c => queryDescriptor.MakeDataSetColumn(c)).ToArray();
                var dataSet = this._queryExecutor.Fetch(statement, fetchedColumns, fetchOptions.ResultStructure);
                var result = new QueryResult
                {
                    Dataset = dataSet,
                    OptionId = fetchOptions.Id,
                    Token = queryToken
                };

                return result;
            }
        }





    }
}
