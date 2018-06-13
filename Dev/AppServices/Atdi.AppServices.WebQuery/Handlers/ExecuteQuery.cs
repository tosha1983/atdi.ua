using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
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
        private HashSet<string> _hashSet;


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
                var columnsValue = new List<string>();
                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                var queryDescriptor = this._repository.GetQueryDescriptorByToken(tokenData, queryToken);
                var columnMetadata = queryDescriptor.Metadata.Columns.ToList().Select(t => t.Description).ToList();
                 this._hashSet = new HashSet<string>(columnMetadata);
                var fetchColumns = new List<ColumnMetadata>();
                if (fetchOptions.Columns==null) { fetchColumns = queryDescriptor.Metadata.Columns.ToList(); columnsValue = fetchColumns.Select(r => r.Description).ToList(); }
                if (fetchOptions.Columns != null) {
                   if (fetchOptions.Columns.Count()==0){
                        fetchColumns = queryDescriptor.Metadata.Columns.ToList();
                        columnsValue = fetchColumns.Select(r => r.Description).ToList();
                    }
                   else  {
                        columnsValue.AddRange(fetchOptions.Columns);
                     }
                }
     
                var limit = fetchOptions.Limit;
                var conditionsFromFetch = fetchOptions.Condition;
                var orderExpression = fetchOptions.Orders;
                var conditions = queryDescriptor.GetConditions(tokenData);
                var columnsFromOrders = fetchOptions.Orders.ToList().Select(t => t.ColumnName);
                
                if (conditionsFromFetch!=null)  {
                    var listConditions = conditions.ToList();
                    listConditions.Add(conditionsFromFetch);
                    conditions = listConditions.ToArray();
                }
               


                if (!string.IsNullOrEmpty(queryDescriptor.IdentUserField)) {
                    var listColumns = columnsValue.ToList();
                    listColumns.Add(queryDescriptor.IdentUserField);
                    columnsValue = listColumns;
                }
                //Validate columns for orders
                ValidateColumns(columnsFromOrders.ToArray());
                //Validate columns for conditions
                ValidateColumns(conditions);
                //Validate selection columns
                ValidateColumns(columnsValue.ToArray());

                var statement = this._dataLayer.Builder
                   .From(queryDescriptor.TableName)
                   .Select(columnsValue.ToArray())
                   .OnTop(limit.Value);

                

                var ordersColumns = orderExpression.Select(t => t.ColumnName).ToArray();
                if (conditions.Count() > 0) {
                    statement
                   .Where(conditions);
                }

                if (ordersColumns.Count() > 0) {
                     statement
                     .OrderByAsc(ordersColumns);
                }

               
                var queryExecutor = this._queryExecutor.Fetch(statement, reader => {
                    return (QueryResult)null;
                });

                return queryExecutor;
            }
            return new QueryResult();
        }


        private bool HasColumn(string nameColumn)
        {
            if (this._hashSet.Contains(nameColumn)) return true;
            else
            {
                throw new InvalidOperationException(string.Format(Exceptions.ColumnIsNotAvailable, nameColumn));
            }
        }

        private void ValidateColumns(string[] columns)
        {
            if (columns != null) {
                for (int i = 0; i < columns.Count(); i++)  {
                    HasColumn(columns[i]);
                }
            }
        }

        private void ValidateColumns(DataModels.DataConstraint.Condition[] conditions)
        {
            if (conditions != null)  {
                for (int i = 0; i < conditions.Count(); i++)  {
                    if (conditions[i] is Atdi.DataModels.DataConstraint.ConditionExpression)  {
                        DataModels.DataConstraint.Operand operand = (conditions[i] as Atdi.DataModels.DataConstraint.ConditionExpression).LeftOperand;
                        if (operand is Atdi.DataModels.DataConstraint.ColumnOperand)  {
                            string column = (operand as Atdi.DataModels.DataConstraint.ColumnOperand).ColumnName;
                            HasColumn(column);
                        }
                    }
                }
            }
        }


    }
}
