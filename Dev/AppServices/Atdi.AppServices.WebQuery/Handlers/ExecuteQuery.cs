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
                string notAvailableColumns = "";
                
                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                var queryDescriptor = this._repository.GetQueryDescriptorByToken(tokenData, queryToken);
                var conditions = queryDescriptor.GetConditions(tokenData);
                var allColumns = queryDescriptor.Metadata.Columns.Select(t => t.Name).ToArray();
                var dataColumns = queryDescriptor.Metadata.Columns;
                var orderExpression = new List<DataModels.DataConstraint.OrderExpression>();
                DataModels.DataConstraint.DataLimit limitRecord = null;
                if (fetchOptions != null)   {
                    if (fetchOptions.Columns != null) {
                        if (fetchOptions.Columns.Count()>0) { 
                                notAvailableColumns += queryDescriptor.ValidateColumns(fetchOptions.Columns);
                                allColumns = fetchOptions.Columns;
                                dataColumns = new List<ColumnMetadata>().ToArray();  List<ColumnMetadata> tempMetaColumn = new List<ColumnMetadata>();
                                fetchOptions.Columns.ToList().ForEach(x => { var metaTemp = queryDescriptor.Metadata.Columns.ToList().Find(r => r.Name == x); if (metaTemp != null) tempMetaColumn.Add(metaTemp); });
                                dataColumns = tempMetaColumn.ToArray();
                        }
                    }
                    
                    var conditionsFromFetch = fetchOptions.Condition;
                    if (conditionsFromFetch != null)  {
                        var listConditions = conditions.ToList();
                        listConditions.Add(conditionsFromFetch);
                        conditions = listConditions.ToArray();
                    }
                    if (fetchOptions.Orders != null) {
                        orderExpression = fetchOptions.Orders.ToList();
                        var columnsFromOrders = fetchOptions.Orders.ToList().Select(t => t.ColumnName).ToArray();
                        if (columnsFromOrders != null) notAvailableColumns+= queryDescriptor.ValidateColumns(columnsFromOrders.ToArray());
                    }
                    if (fetchOptions.Limit != null) limitRecord = fetchOptions.Limit;
                }
               

                var statement = this._dataLayer.Builder
                   .From(queryDescriptor.TableName)
                   .Select(allColumns);
   
              
               
                if (conditions.Count() > 0) {
                    statement
                   .Where(conditions);
                }

                var ordersColumns = orderExpression.Select(t => t.ColumnName).ToArray();
                if (ordersColumns.Count() > 0) {
                     statement
                     .OrderByAsc(ordersColumns);
                }

                if (limitRecord != null)
                {
                    statement
                  .OnTop(limitRecord.Value);
                }


                if (conditions!=null) notAvailableColumns += queryDescriptor.ValidateConditions(conditions);
              
                if (notAvailableColumns.Length > 0)   {
                    notAvailableColumns = notAvailableColumns.Remove(notAvailableColumns.Length - 1, 1);
                    throw new InvalidOperationException(string.Format(Exceptions.ColumnIsNotAvailable, notAvailableColumns));
                }
                var result = new QueryResult();
                if (fetchOptions != null)
                {
                    var dataSet = this._queryExecutor.Fetch(statement, dataColumns.Select(c => new DataSetColumn { Name = c.Name, Type = c.Type }).ToArray(), fetchOptions.ResultStructure);
                    result.Dataset = dataSet;
                    result.OptionId = fetchOptions.Id;
                    result.Token = queryToken;
                }
                else
                {
                    throw new InvalidOperationException(string.Format(Exceptions.FetchOptionsNull));

                }
                return result;
            }
        }


      


    }
}
