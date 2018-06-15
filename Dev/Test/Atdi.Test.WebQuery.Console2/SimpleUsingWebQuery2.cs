
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;

using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;


namespace Atdi.Test.WebQuery
{
    public class SimpleUsingWebQuery2
    {
        public static void Run()
        {
            // Аудентификаици пользователя сервиса
            var authManager = GetAuthenticationManager("HttpAuthenticationManager");

            var credential = new UserCredential
            {
                UserName = "ICSM",
                Password = "ICSM"
            };

            var authResult = authManager.AuthenticateUser(credential);
            // Валидация результата аудентификации
            if (authResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(authResult.FaultCause);
            }

            // распаковка результата аудентификации 
            var userIdentity = authResult.Data;

            Console.WriteLine($"User ID: {userIdentity.Id}");

            var webQueryService = GetWebQuery("HttpWebQuery"); // тип конечно точки зависит от конфигурации сервреа приложений

            // Вариант выполнения запрос 1: используем доступные группы запросов

            // обащение к сервису за списком доступных пользователю групп запросов
            var defGroupsResult = webQueryService.GetQueryGroups(userIdentity.UserToken);
            // Валидация результата
            if (defGroupsResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(defGroupsResult.FaultCause);
            }

            // распаковка результата
            var userQueryGroups = defGroupsResult.Data;

            if (userQueryGroups.Groups.Length == 0)
            {
                throw new InvalidOperationException($"Пользователю {userIdentity.Name} не назначены запросы");
            }
           


            for (int i = 0; i < userQueryGroups.Groups.Length; i++)
            {
                var group = userQueryGroups.Groups[i];
                if (group.QueryTokens == null) continue;
                for (int j = 0; j < group.QueryTokens.Length; j++)
                {
                    var queryToken = group.QueryTokens[j];

                    // обащение к сервису за метаданніми запроса
                    var defQueryMetadataResult = webQueryService.GetQueryMetadata(userIdentity.UserToken, queryToken);
                    // Валидация результата
                    if (defQueryMetadataResult.State == OperationState.Fault)
                    {
                        throw new InvalidOperationException(defQueryMetadataResult.FaultCause);
                    }
                    // распаковка результата
                    var queryMetadata = defQueryMetadataResult.Data;
                    Console.WriteLine($"Query code: {queryMetadata.Code }");
                    Console.WriteLine($"      name: {queryMetadata.Name }");
                    Console.WriteLine($"     title: {queryMetadata.Title}");
                    Console.WriteLine($"   version: {queryMetadata.Token.Version}");

                    // ... 

                    // пример выполнения запроса и получения данных
                    // подготовка парамтеров и условий выполнения запроса
               
                    var fetchOptions = new FetchOptions
                    {
                        Id = Guid.NewGuid(), // генерируем идентификатор выборки, будет возвращен с результатом
                                             //Columns = new string[] {"ID", "StationA.Position.ADDRESS", "StationA.Position.CITY" }, // указываем ограничение по полям, при условии что нужно меньше чем может дать запрос, в случаи отсутвия такой необходимости поле оставлять пустым (null or new string [] { }) 
                                            // Columns = new string[] { "ID","CHANNEL_SEP" },
                        ResultStructure = DataSetStructure.StringRows,  // указываем тип возвращаемой структуры данных, в данном случии будет масив объектов строк состоящих из ячеек типа string.
                        Limit = new DataLimit // указываем лимит кол-ва возвращаемых записей
                        {
                            Type = LimitValueType.Records,
                            Value = 1000
                        }, 
                        Condition = new ConditionExpression // указываем условие выборки
                        {
                            LeftOperand = new ColumnOperand { ColumnName = "ID" },
                            Operator = ConditionOperator.NotBetween,
                            RightOperand = new IntegerValuesOperand {  Values = new int?[] {10, 3599 } }
                        },  
                        /*
                        Orders = new OrderExpression[] // указываем условие сортировки
                        {
                            //new OrderExpression { ColumnName = "StationA.Position.ADDRESS", OrderType = OrderType.Ascending },
                            new OrderExpression { ColumnName = "ID", OrderType = OrderType.Descending }//,
                            //new OrderExpression { ColumnName = "StationA.Position.CITY", OrderType = OrderType.Descending }
                        }
                        */
                    };


                    // обащение к сервису для выполнния запроса
                    var executingResult = webQueryService.ExecuteQuery(userIdentity.UserToken, queryMetadata.Token, fetchOptions);
                    // Валидация результата
                    if (executingResult.State == OperationState.Fault)
                    {
                        throw new InvalidOperationException(executingResult.FaultCause);
                    }

                    // распаковка результата
                    var queryResult = executingResult.Data;


                }
            }


            // Вариант выполнения запрос 2: используем преопределнное значение текстового кода запроса 
            // обащение к сервису за метаданніми запроса
            var defQueryMetadataByCodeResult = webQueryService.GetQueryMetadataByCode(userIdentity.UserToken, "SomeCode");
            // Валидация результата получения метаданных
            if (defQueryMetadataByCodeResult.State != OperationState.Fault)
            {
                throw new InvalidOperationException(defQueryMetadataByCodeResult.FaultCause);
            }
            var queryByCode = defQueryMetadataByCodeResult.Data;
            // подготовка параметров и выполнение запроса аналогично ка кпоказано через группы


            // Вариант выполнения запрос 3: используем заранее сохраненый токен запроса


            // выполнение дейстивя модификации данных в рамках запроса

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
                    Operator = ConditionOperator.Equal ,
                    RightOperand = new IntegerValueOperand { Value = 125}
                }
            };

            // удаление записи
            var deleteionAction = new DeleteionAction
            {
                Id = Guid.NewGuid(),
                Condition = new ConditionExpression // указываем условие выборки
                {
                    LeftOperand = new ColumnOperand { ColumnName = "ID" },
                    Operator = ConditionOperator.In,
                    RightOperand = new IntegerValuesOperand { Values = new int?[] { 234, 3234, 599 } }
                }
            };

            // пакуем действия в ченджсет
            var changset = new Changeset
            {
                Id = Guid.NewGuid(),
                Actions = new DataModels.Action[] { creationAction, updationAction, deleteionAction }
            };

            // обащение к сервису для внесения изменений - 3 действия будут отправлены на сервре и выполнены послеовательно
            var saveChangeResult = webQueryService.SaveChanges(userIdentity.UserToken, queryByCode.Token,  changset);
            // Валидация результата изменения данных
            if (saveChangeResult.State != OperationState.Fault)
            {
                throw new InvalidOperationException(defQueryMetadataByCodeResult.FaultCause);
            }
         
        }

        private static IAuthenticationManager GetAuthenticationManager(string endpointName)
        {
            var f = new ChannelFactory<IAuthenticationManager>(endpointName);
            return f.CreateChannel();
        }

        private static IWebQuery GetWebQuery(string endpointName)
        {
            var f = new ChannelFactory<IWebQuery>(endpointName);
            return f.CreateChannel();
        }
    }
}
