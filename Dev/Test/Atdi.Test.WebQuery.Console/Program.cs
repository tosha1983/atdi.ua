using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.WebQuery
{
    class Program
    {
        //private static ChannelFactory<IAuthenticationManager> _tcpChannelFactory = null;
        //private static ChannelFactory<IAuthenticationManager> _httpChannelFactory = null;
        //private static ChannelFactory<IAuthenticationManager> _pipeChannelFactory = null;

        private static BlockingCollection<int> _count = new BlockingCollection<int>();

        static void Main(string[] args)
        {
            while (true)
            {


                System.Console.WriteLine("Press any key to start testing ...");
                System.Console.ReadLine();

                //TestAuthenticationManager("TcpAuthenticationManager");
                _count = new BlockingCollection<int>();
                var timer = System.Diagnostics.Stopwatch.StartNew();

                //TestWebQuerySaveChanges("TcpAuthenticationManager", "TcpWebQuery");

                TestFileStorage("TcpAuthenticationManager", "TcpFileStorage");

                //var tasks = new Task[]
                //{
                //    Task.Run(() => BathTest()),
                //    Task.Run(() => BathTest()),
                //    Task.Run(() => BathTest()),
                //    Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest()),
                //    //Task.Run(() => BathTest())
                //};


                //Task.WaitAll(tasks);

                System.Console.WriteLine($"Done: {timer.Elapsed.TotalMilliseconds} ms -> Called: {_count.Count}");

            }
        }

        static void BathTest()
        {
            for (int i = 0; i < 50; i++)
            {
                TestWebQueryAccess("TcpAuthenticationManager", "HttpWebQuery");
                TestWebQueryAccess("TcpAuthenticationManager", "TcpWebQuery");
                TestWebQueryAccess("TcpAuthenticationManager", "PipeWebQuery");
            }
        }
        static void TestGetData()
        {
            System.Console.WriteLine("Press any key to start testing ...");
            System.Console.ReadLine();

            var tcpAuthManager = GetWebQueryByEndpoint("TcpWebQuery");
            var httpAuthManager = GetWebQueryByEndpoint("HttpWebQuery");
            var pipeAuthManager = GetWebQueryByEndpoint("PipeWebQuery");

            var result = tcpAuthManager.ExecuteQuery(null, null, null);
        }

        static void BlockTest()
        {
            System.Console.WriteLine("Press any key to start testing ...");
            System.Console.ReadLine();


            var tcpAuthManager = GetWebQueryByEndpoint("TcpWebQuery");
            var httpAuthManager = GetWebQueryByEndpoint("HttpWebQuery");
            var pipeAuthManager = GetWebQueryByEndpoint("PipeWebQuery");

            while (true)
            {
                TestWebQuery(tcpAuthManager, "tcp");
                TestWebQuery(httpAuthManager, "http");
                TestWebQuery(pipeAuthManager, "pipe");
                System.Console.WriteLine("Testing has been done.");
                System.Console.ReadLine();
                System.Console.WriteLine("Repeat ...");
            }
        }

        static IWebQuery GetWebQueryByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IWebQuery>(endpointName);
            return f.CreateChannel();
        }

        static IFileStorage GetFileStorageByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IFileStorage>(endpointName);
            return f.CreateChannel();
        }

        static IAuthenticationManager GetAuthenticationManagerByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IAuthenticationManager>(endpointName);
            return f.CreateChannel();
        }

        static void TestService(IAuthenticationManager authManager, string context)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var c = new UserCredential();
            for (int i = 0; i < 1000; i++)
            {
                var tcpToken = authManager.AuthenticateUser(c);
            }


            timer.Stop();
            System.Console.WriteLine($"{context}: {timer.Elapsed.TotalMilliseconds} ms");
        }

        static void TestWebQuery(IWebQuery webQueryService, string context)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var c = new UserCredential();
            var userToken = new UserToken();
            var queryToken = new DataModels.WebQuery.QueryToken();
            var fetchOptions = new DataModels.WebQuery.FetchOptions();
            var changeset = new DataModels.Changeset();
            for (int i = 0; i < 1; i++)
            {
                // var tree = webQueryService.GetQueriesTree(userToken);
                //  var metadata = webQueryService.GetQueryMetadata(userToken, new DataModels.WebQuery.QueryToken());
                var data = webQueryService.ExecuteQuery(null, queryToken, fetchOptions);
                //  Console.WriteLine(data.Data.Rows[0][0]);
                //  var result = webQueryService.SaveChanges(userToken, queryToken, changeset);
            }


            timer.Stop();
            System.Console.WriteLine($"{context}: {timer.Elapsed.TotalMilliseconds} ms");


            var timer2 = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 1; i++)
            {
                // var tree = webQueryService.GetQueriesTree(userToken);
                //  var metadata = webQueryService.GetQueryMetadata(userToken, new DataModels.WebQuery.QueryToken());
                var data = webQueryService.ExecuteQuery(userToken, queryToken, fetchOptions);
                //  Console.WriteLine(data.Data.RowsAsString[0][0]);
                //  var result = webQueryService.SaveChanges(userToken, queryToken, changeset);
            }


            timer2.Stop();
            System.Console.WriteLine($"{context}: {timer2.Elapsed.TotalMilliseconds} ms");
        }

        static void TestAuthenticationManager(string endpointName)
        {
            var authManager = GetAuthenticationManagerByEndpoint(endpointName);
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var c = new UserCredential()
            {
                UserName = "Andrey",
                Password = "P@ssw0rd"
            };

            for (int i = 0; i < 10; i++)
            {
                var userIdentity = authManager.AuthenticateUser(c);
            }


            timer.Stop();
            System.Console.WriteLine($"AuthenticateUser: {timer.Elapsed.TotalMilliseconds} ms");
        }

        static void TestWebQueryAccess(string authEndpointName, string webQueryEndpointName)
        {
            try
            {
                var recordCount = 100;

                var authManager = GetAuthenticationManagerByEndpoint(authEndpointName);
                var userCredential = new UserCredential()
                {
                    UserName = "Andrey",
                    Password = ""
                };

                var authResult = authManager.AuthenticateUser(userCredential);
                if (authResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    Console.WriteLine(authResult.FaultCause);
                    return;
                }
                _count.Add(1);

                var userIdentity = authResult.Data;

                var webQuery = GetWebQueryByEndpoint(webQueryEndpointName);
                var groupsResult = webQuery.GetQueryGroups(userIdentity.UserToken);
                if (groupsResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    Console.WriteLine(groupsResult.FaultCause);
                    return;
                }
                _count.Add(1);
                //      Console.Write($"Endpoint: {webQueryEndpointName}");
                //        Console.WriteLine($"Count group: {groupsResult.Data.Groups.Length}");

                var queryFristToken = groupsResult.Data.Groups[0].QueryTokens[0];
                var qm = webQuery.GetQueryMetadata(userIdentity.UserToken, queryFristToken);
                _count.Add(1);

                //var timer = System.Diagnostics.Stopwatch.StartNew();
                var re1 = webQuery.ExecuteQuery(userIdentity.UserToken, queryFristToken, new DataModels.WebQuery.FetchOptions { ResultStructure = DataModels.DataSetStructure.StringCells, Limit = new DataModels.DataConstraint.DataLimit { Value = recordCount }, Orders = new DataModels.DataConstraint.OrderExpression[] { } });
                //timer.Stop();
                _count.Add(1);
                //      System.Console.WriteLine($"ObjectRows: {timer.Elapsed.TotalMilliseconds} ms - >>> {re1.Data.Dataset.RowCount}");

                //timer = System.Diagnostics.Stopwatch.StartNew();
                var re2 = webQuery.ExecuteQuery(userIdentity.UserToken, queryFristToken, new DataModels.WebQuery.FetchOptions { ResultStructure = DataModels.DataSetStructure.StringCells, Limit = new DataModels.DataConstraint.DataLimit { Value = recordCount }, Orders = new DataModels.DataConstraint.OrderExpression[] { } });
                //timer.Stop();
                _count.Add(1);
                //       System.Console.WriteLine($"StringRows: {timer.Elapsed.TotalMilliseconds} ms - >>> {re2.Data.Dataset.RowCount}");

                //timer = System.Diagnostics.Stopwatch.StartNew();
                var re3 = webQuery.ExecuteQuery(userIdentity.UserToken, queryFristToken, new DataModels.WebQuery.FetchOptions { ResultStructure = DataModels.DataSetStructure.StringCells, Limit = new DataModels.DataConstraint.DataLimit { Value = recordCount }, Orders = new DataModels.DataConstraint.OrderExpression[] { } });
                //timer.Stop();
                _count.Add(1);
                //       System.Console.WriteLine($"TypedRows: {timer.Elapsed.TotalMilliseconds} ms - >>> {re3.Data.Dataset.RowCount}");

                //timer = System.Diagnostics.Stopwatch.StartNew();
                var re4 = webQuery.ExecuteQuery(userIdentity.UserToken, queryFristToken, new DataModels.WebQuery.FetchOptions { ResultStructure = DataModels.DataSetStructure.StringCells, Limit = new DataModels.DataConstraint.DataLimit { Value = recordCount }, Orders = new DataModels.DataConstraint.OrderExpression[] { } });
                //timer.Stop();
                _count.Add(1);
                //      System.Console.WriteLine($"ObjectCells: {timer.Elapsed.TotalMilliseconds} ms - >>> {re4.Data.Dataset.RowCount}");

                //timer = System.Diagnostics.Stopwatch.StartNew();
                var re5 = webQuery.ExecuteQuery(userIdentity.UserToken, queryFristToken, new DataModels.WebQuery.FetchOptions { ResultStructure = DataModels.DataSetStructure.StringCells, Limit = new DataModels.DataConstraint.DataLimit { Value = recordCount }, Orders = new DataModels.DataConstraint.OrderExpression[] { } });
                //timer.Stop();
                _count.Add(1);
                //     System.Console.WriteLine($"StringCells: {timer.Elapsed.TotalMilliseconds} ms - >>> {re5.Data.Dataset.RowCount}");

                //timer = System.Diagnostics.Stopwatch.StartNew();
                var re6 = webQuery.ExecuteQuery(userIdentity.UserToken, queryFristToken, new DataModels.WebQuery.FetchOptions { ResultStructure = DataModels.DataSetStructure.StringCells, Limit = new DataModels.DataConstraint.DataLimit { Value = recordCount }, Orders = new DataModels.DataConstraint.OrderExpression[] { } });
                //timer.Stop();
                _count.Add(1);
                //  System.Console.WriteLine($"TypedCells: {timer.Elapsed.TotalMilliseconds} ms - >>> {re6.Data.Dataset.RowCount}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            

            
        }

        static void TestFileStorage(string authEndpointName, string fileStorageEndpointName)
        {
            var authManager = GetAuthenticationManagerByEndpoint(authEndpointName);
            var userCredential = new UserCredential()
            {
                UserName = "Andrey",
                Password = ""
            };

            var authResult = authManager.AuthenticateUser(userCredential);
            if (authResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                Console.WriteLine(authResult.FaultCause);
                return;
            }

            var userIdentity = authResult.Data;

            var endpoint = GetFileStorageByEndpoint(fileStorageEndpointName);
            var getFileInfoResult = endpoint.GetFileInfo(userIdentity.UserToken, 100);
            if (getFileInfoResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                Console.WriteLine(getFileInfoResult.FaultCause);
                return;
            }

            if (getFileInfoResult.Data.Id != 100)
            {
                Console.WriteLine(getFileInfoResult.FaultCause);
                return;
            }
        }

        static void TestWebQuerySaveChanges(string authEndpointName, string webQueryEndpointName)
        {
            try
            {
                //var recordCount = 100;

                var authManager = GetAuthenticationManagerByEndpoint(authEndpointName);
                var userCredential = new UserCredential()
                {
                    UserName = "Andrey",
                    Password = ""
                };

                var authResult = authManager.AuthenticateUser(userCredential);
                if (authResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    Console.WriteLine(authResult.FaultCause);
                    return;
                }
                _count.Add(1);

                var userIdentity = authResult.Data;

                var webQuery = GetWebQueryByEndpoint(webQueryEndpointName);
                var groupsResult = webQuery.GetQueryGroups(userIdentity.UserToken);
                if (groupsResult.State == DataModels.CommonOperation.OperationState.Fault)
                {
                    Console.WriteLine(groupsResult.FaultCause);
                    return;
                }
                _count.Add(1);
                //      Console.Write($"Endpoint: {webQueryEndpointName}");
                //        Console.WriteLine($"Count group: {groupsResult.Data.Groups.Length}");

                var queryFristToken = groupsResult.Data.Groups[0].QueryTokens[0];
                var qm = webQuery.GetQueryMetadata(userIdentity.UserToken, queryFristToken);
                _count.Add(1);

                var re1 = webQuery.ExecuteQuery(userIdentity.UserToken, queryFristToken, new DataModels.WebQuery.FetchOptions { ResultStructure = DataModels.DataSetStructure.StringCells, Limit = new DataModels.DataConstraint.DataLimit { Value = 100 }, Orders = new DataModels.DataConstraint.OrderExpression[] { } });

                var changeset = new DataModels.Changeset
                {
                    Id = Guid.NewGuid(),
                    Actions = new DataModels.Action[]
                    {
                        new DataModels.DeletionAction
                        {
                            Id = Guid.NewGuid(),
                            Condition = new DataModels.DataConstraint.ComplexCondition
                            {
                                Operator = DataModels.DataConstraint.LogicalOperator.Or,
                                Conditions = new DataModels.DataConstraint.Condition[]
                                {
                                    new DataModels.DataConstraint.ConditionExpression
                                    {
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "Antenna.DeviceModel.Standard.RadioSystem.EFIS_NAME" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.StringValueOperand{ Value =  "SomeTest111" }
                                    },
                                    new DataModels.DataConstraint.ConditionExpression
                                    {
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "Owner.REPR_FIRSTNAME" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.StringValueOperand{ Value =  "SomeTest111" }
                                    }
                                }
                            }
                                
                        },

                        new DataModels.StringRowUpdationAction
                        {
                            Id =  Guid.NewGuid(),
                            Condition = new DataModels.DataConstraint.ComplexCondition
                            {
                                Operator = DataModels.DataConstraint.LogicalOperator.Or,
                                Conditions = new DataModels.DataConstraint.Condition[]
                                {
                                    new DataModels.DataConstraint.ConditionExpression
                                    {
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "Antenna.DeviceModel.Standard.RadioSystem.EFIS_NAME" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.StringValueOperand{ Value =  "SomeTest111" }
                                    },
                                    new DataModels.DataConstraint.ConditionExpression
                                    {
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "Owner.REPR_FIRSTNAME" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.StringValueOperand{ Value =  "SomeTest111" }
                                    }
                                }
                            },
                            Columns = new DataModels.DataSetColumn[]
                            {
                                new DataModels.DataSetColumn
                                {
                                    Name = "NETWORK_IDENT", Type = DataModels.DataType.String, Index = 0
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "FEE", Type = DataModels.DataType.Integer, Index = 1
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "RX_EQP_NBR", Type = DataModels.DataType.Decimal, Index = 2
                                },
                            },
                            Row = new DataModels.StringDataRow
                            {
                                Cells = new string[]
                                {
                                    "NETWORK_IDENT", "8111", "3917"
                                }
                            }
                        },

                        new DataModels.ObjectRowUpdationAction
                        {
                            Id =  Guid.NewGuid(),
                            Condition = new DataModels.DataConstraint.ComplexCondition
                            {
                                Operator = DataModels.DataConstraint.LogicalOperator.Or,
                                Conditions = new DataModels.DataConstraint.Condition[]
                                {
                                    new DataModels.DataConstraint.ConditionExpression
                                    {
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "Antenna.DeviceModel.Standard.RadioSystem.EFIS_NAME" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.StringValueOperand{ Value =  "SomeTest111" }
                                    },
                                    new DataModels.DataConstraint.ConditionExpression
                                    {
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "Owner.REPR_FIRSTNAME" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.StringValueOperand{ Value =  "SomeTest111" }
                                    }
                                }
                            },
                            Columns = new DataModels.DataSetColumn[]
                            {
                                new DataModels.DataSetColumn
                                {
                                    Name = "NETWORK_IDENT", Type = DataModels.DataType.String, Index = 0
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "FEE", Type = DataModels.DataType.Integer, Index = 1
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "RX_EQP_NBR", Type = DataModels.DataType.Decimal, Index = 2
                                },
                            },
                            Row = new DataModels.ObjectDataRow
                            {
                                Cells = new object[]
                                {
                                    "NETWORK_IDENT", (int)8111, (decimal)3917
                                }
                            }
                        },

                        new DataModels.TypedRowUpdationAction
                        {
                            Id =  Guid.NewGuid(),
                            Condition = new DataModels.DataConstraint.ComplexCondition
                            {
                                Operator = DataModels.DataConstraint.LogicalOperator.Or,
                                Conditions = new DataModels.DataConstraint.Condition[]
                                {
                                    new DataModels.DataConstraint.ConditionExpression
                                    {
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "Antenna.DeviceModel.Standard.RadioSystem.EFIS_NAME" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.StringValueOperand{ Value =  "SomeTest111" }
                                    },
                                    new DataModels.DataConstraint.ConditionExpression
                                    {
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "Owner.REPR_FIRSTNAME" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.StringValueOperand{ Value =  "SomeTest111" }
                                    }
                                }
                            },
                            Columns = new DataModels.DataSetColumn[]
                            {
                                new DataModels.DataSetColumn
                                {
                                    Name = "NETWORK_IDENT", Type = DataModels.DataType.String, Index = 0
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "FEE", Type = DataModels.DataType.Integer, Index = 0
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "RX_EQP_NBR", Type = DataModels.DataType.Decimal, Index = 0
                                },
                            },
                            Row = new DataModels.TypedDataRow
                            {
                                StringCells = new string[] { ""},
                                IntegerCells = new int?[] { null},
                                DecimalCells = new decimal? [] { (decimal?)123 }
                            }
                        },

                        new DataModels.StringRowCreationAction
                        {
                            Id =  Guid.NewGuid(),
                            
                            Columns = new DataModels.DataSetColumn[]
                            {
                                new DataModels.DataSetColumn
                                {
                                    Name = "NETWORK_IDENT", Type = DataModels.DataType.String, Index = 0
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "FEE", Type = DataModels.DataType.Integer, Index = 1
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "RX_EQP_NBR", Type = DataModels.DataType.Decimal, Index = 2
                                },
                            },
                            Row = new DataModels.StringDataRow
                            {
                                Cells = new string[]
                                {
                                    "NETWORK_IDENT", "8111", "3917"
                                }
                            }
                        },

                        new DataModels.ObjectRowCreationAction
                        {
                            Id =  Guid.NewGuid(),
                            Columns = new DataModels.DataSetColumn[]
                            {
                                new DataModels.DataSetColumn
                                {
                                    Name = "NETWORK_IDENT", Type = DataModels.DataType.String, Index = 0
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "FEE", Type = DataModels.DataType.Integer, Index = 1
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "RX_EQP_NBR", Type = DataModels.DataType.Decimal, Index = 2
                                },
                            },
                            Row = new DataModels.ObjectDataRow
                            {
                                Cells = new object[]
                                {
                                    "NETWORK_IDENT", (int)8111, (decimal)3917
                                }
                            }
                        },

                        new DataModels.ObjectRowCreationAction
                        {
                            Id =  Guid.NewGuid(),
                            Columns = new DataModels.DataSetColumn[]
                            {
                                new DataModels.DataSetColumn
                                {
                                    Name = "NETWORK_IDENT", Type = DataModels.DataType.String, Index = 0
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "FEE", Type = DataModels.DataType.Integer, Index = 1
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "RX_EQP_NBR", Type = DataModels.DataType.Decimal, Index = 2
                                },
                            },
                            Row = new DataModels.ObjectDataRow
                            {
                                Cells = new object[]
                                {
                                    "NETWORK_IDENT", (int)8111, (decimal)3917
                                }
                            }
                        },
                    }
                };

                var chsResult = webQuery.SaveChanges(userIdentity.UserToken, queryFristToken, changeset);
                //  System.Console.WriteLine($"TypedCells: {timer.Elapsed.TotalMilliseconds} ms - >>> {re6.Data.Dataset.RowCount}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }



        }
    }
}