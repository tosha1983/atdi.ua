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
                var columnsValue = new List<string>();
                var tokenData = this._tokenProvider.UnpackUserToken(userToken);
                var queryDescriptor = this._repository.GetQueryDescriptorByToken(tokenData, queryToken);
                var columnMetadata = queryDescriptor.Metadata.Columns.ToList().Select(t => t.Name).ToList();
                var allColumns = new List<ColumnMetadata>();
                if (fetchOptions.Columns==null) { allColumns = queryDescriptor.Metadata.Columns.ToList(); columnsValue = allColumns.Select(r => r.Name).ToList();  }
                if (fetchOptions.Columns != null) {
                   if (fetchOptions.Columns.Count()==0){
                        allColumns = queryDescriptor.Metadata.Columns.ToList();
                        columnsValue = allColumns.Select(r => r.Name).ToList();
                    }
                   else  {
                        columnsValue.AddRange(fetchOptions.Columns);
                        for (int i=0; i< fetchOptions.Columns.Count(); i++)  {
                            ColumnMetadata metaData = queryDescriptor.Metadata.Columns.ToList().Find(r => r.Name == fetchOptions.Columns[i]);
                            if (metaData !=null) {
                                allColumns.Add(metaData);
                            }
                        }
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
                    if (!listColumns.Contains(queryDescriptor.IdentUserField)) {
                        listColumns.Add(queryDescriptor.IdentUserField);
                        columnsValue = listColumns;
                        ColumnMetadata metaData = queryDescriptor.Metadata.Columns.ToList().Find(r => r.Name == queryDescriptor.IdentUserField);
                        if (metaData != null)    allColumns.Add(metaData);
                    }
                }

              
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

                //получаем список полей, которые не прошли валидацию
                notAvailableColumns = queryDescriptor.ValidateColumns(columnsFromOrders.ToArray());
                notAvailableColumns += queryDescriptor.ValidateColumns(conditions);
                notAvailableColumns += queryDescriptor.ValidateColumns(columnsValue.ToArray());
                if (notAvailableColumns.Length > 0) notAvailableColumns = notAvailableColumns.Remove(notAvailableColumns.Length - 1, 1);

                var dataSet = this._queryExecutor.Fetch(statement, allColumns.Select(c => new DataSetColumn { Name = c.Name, Type = c.Type }).ToArray(), fetchOptions.ResultStructure);
                var result = new QueryResult();
                result.Dataset = dataSet;
                result.OptionId = fetchOptions.Id;
                result.Token = queryToken;
               

                return result;
            }
        }


      


    }
}
