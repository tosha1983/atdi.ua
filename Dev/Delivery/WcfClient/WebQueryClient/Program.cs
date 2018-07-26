using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using Atdi.DataModels;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;

using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.DataConstraint;

using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;


namespace WebQueryClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Run("NetTcpEndpoint");
            Run("BasicHttpEndpoint");
            Run("NetNamedPipeEndpoint");
        }

        static void Run(string endpointSuffix)
        {
            Console.WriteLine($"Running: {endpointSuffix}");

            var authEndpointName = "AuthenticationManager" + endpointSuffix;
            var webQueryEndpointName = "WebQuery" + endpointSuffix;

            try
            {
                //  Аудентификация
                var userIdentity = AuthenticateUser(authEndpointName);

                // Получение списка доступных запросов
                var groups = GetAvailableQueries(webQueryEndpointName, userIdentity);

                // Получение метаданных для запроса по токену
                var queryMetadata1 = GetQueryMetadataByToken(webQueryEndpointName, userIdentity, groups.Groups[0].QueryTokens[0]);

                // Получение метаданных для запроса по коду
                var queryMetadata2 = GetQueryMetadataByCode(webQueryEndpointName, userIdentity, "QueryCode2");

                // Выполнение запроса
                var queryResult = ExecuteQuery(webQueryEndpointName, userIdentity, queryMetadata1.Token);

                // Применение изменений
                var chahgesResult = ApplyChanges(webQueryEndpointName, userIdentity, queryMetadata1.Token);

            }
            catch(Exception e)
            {
                Console.WriteLine($"Error occurred: {e.Message}");
            }

            Console.WriteLine($"Finished: {endpointSuffix}");
        }

        static UserIdentity AuthenticateUser(string endpointName)
        {
            var authManager = GetAuthenticationManagerByEndpoint(endpointName);

            var credential = new UserCredential()
            {
                UserName = "Andrey",
                Password = ""
            };

            var authResult = authManager.AuthenticateUser(credential);
            if (authResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(authResult.FaultCause);
            }

            return authResult.Data;
        }

        static QueryGroups GetAvailableQueries(string endpointName, UserIdentity userIdentity)
        {
            var webQuery = GetWebQueryByEndpoint(endpointName);

            var groupsResult = webQuery.GetQueryGroups(userIdentity.UserToken);
            if (groupsResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(groupsResult.FaultCause);
            }

            return groupsResult.Data;
        }

        static QueryMetadata GetQueryMetadataByToken(string endpointName, UserIdentity userIdentity, QueryToken queryToken)
        {
            var webQuery = GetWebQueryByEndpoint(endpointName);

            var queryMetadataResult = webQuery.GetQueryMetadata(userIdentity.UserToken, queryToken);
            if (queryMetadataResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(queryMetadataResult.FaultCause);
            }

            return queryMetadataResult.Data;
        }

        static QueryMetadata GetQueryMetadataByCode(string endpointName, UserIdentity userIdentity, string queryCode)
        {
            var webQuery = GetWebQueryByEndpoint(endpointName);

            var queryMetadataResult = webQuery.GetQueryMetadataByCode(userIdentity.UserToken, queryCode);
            if (queryMetadataResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(queryMetadataResult.FaultCause);
            }

            return queryMetadataResult.Data;
        }

        static QueryResult ExecuteQuery(string endpointName, UserIdentity userIdentity, QueryToken queryToken)
        {
            var webQuery = GetWebQueryByEndpoint(endpointName);

            // параметры выборки данных для запроса
            var fetchOptions = new FetchOptions
            {
                // генерируем идентификатор выборки, будет возвращен с результатом
                Id = Guid.NewGuid(),
                // указываем ограничение по полям, при условии что нужно меньше чем может дать запрос, в случаи отсутвия такой необходимости поле оставлять пустым (null or new string [] { }) 
                Columns = new string[] { "ID", "FEE", "AZIMUTH" },
                // указываем тип возвращаемой структуры данных, в данном случии будет масив объектов строк состоящих из ячеек типа string.
                ResultStructure = DataSetStructure.StringRows,
                // указываем лимит кол-ва возвращаемых записей
                Limit = new DataLimit 
                {
                    Type = LimitValueType.Records,
                    Value = 1000
                },
                // указываем условие выборки
                Condition = new ConditionExpression 
                {
                    LeftOperand = new ColumnOperand { ColumnName = "ID" },
                    Operator = ConditionOperator.NotBetween,
                    RightOperand = new IntegerValuesOperand { Values = new int?[] { 1, 100} }
                },
                // указываем условие сортировки
                Orders = new OrderExpression[] 
                {
                    new OrderExpression { ColumnName = "AGL", OrderType = OrderType.Ascending },
                    new OrderExpression { ColumnName = "ID", OrderType = OrderType.Descending },
                    new OrderExpression { ColumnName = "FEE", OrderType = OrderType.Descending }
                }
            };

            var executeResult = webQuery.ExecuteQuery(userIdentity.UserToken, queryToken, fetchOptions);
            if (executeResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(executeResult.FaultCause);
            }

            return executeResult.Data;
        }

        static ChangesResult ApplyChanges(string endpointName, UserIdentity userIdentity, QueryToken queryToken)
        {
            var webQuery = GetWebQueryByEndpoint(endpointName);

            // создание записи
            var creationAction = new TypedRowCreationAction
            {
                Id = Guid.NewGuid(),
                Columns = new DataSetColumn[]
                {
                    new DataSetColumn{ Name = "CODE", Type = DataType.String, Index = 0}, // индекс внутри типа - с 0
                    new DataSetColumn{ Name = "DATE_OUT", Type = DataType.DateTime, Index = 0} // индекс внутри типа - с 0
                },
                Row = new TypedDataRow
                {
                    StringCells = new string[] { "CODE1" },
                    DateTimeCells = new DateTime?[] { DateTime.Now }
                },
            };

            // модификация записи
            var updationAction = new ObjectRowUpdationAction
            {
                Id = Guid.NewGuid(),
                Columns = new DataSetColumn[]
                {
                    new DataSetColumn{ Name = "CODE", Type = DataType.String, Index = 0}, // индекс внутри массива ячеек
                    new DataSetColumn{ Name = "DATE_OUT", Type = DataType.DateTime, Index = 1} // индекс внутри массива ячеек
                },
                Row = new ObjectDataRow
                {
                    Cells = new object[] { "CODE2", DateTime.Now.ToLocalTime() }
                },
                Condition = new ConditionExpression // указываем условие выборки
                {
                    LeftOperand = new ColumnOperand { ColumnName = "ID" },
                    Operator = ConditionOperator.Equal,
                    RightOperand = new IntegerValueOperand { Value = 125 }
                }
            };

            // удаление записи
            var deleteionAction = new DeletionAction
            {
                Id = Guid.NewGuid(),
                Condition = new ConditionExpression // указываем условие выборки
                {
                    LeftOperand = new ColumnOperand { ColumnName = "ID" },
                    Operator = ConditionOperator.In,
                    RightOperand = new IntegerValuesOperand { Values = new int?[] { 234, 3234, 599 } }
                }
            };

            // пакуем действия в набо изменений
            var changset = new Changeset
            {
                Id = Guid.NewGuid(),
                Actions = new Atdi.DataModels.Action[] { creationAction, updationAction, deleteionAction }
            };

            var saveChangesResult = webQuery.SaveChanges(userIdentity.UserToken, queryToken, changset);
            if (saveChangesResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(saveChangesResult.FaultCause);
            }

            return saveChangesResult.Data;
        }

        static IWebQuery GetWebQueryByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IWebQuery>(endpointName);
            return f.CreateChannel();
        }

        static IAuthenticationManager GetAuthenticationManagerByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IAuthenticationManager>(endpointName);
            return f.CreateChannel();
        }

    }
}
