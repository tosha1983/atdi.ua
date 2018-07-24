using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Identity;
using Atdi.DataModels.WebQuery;
using System.Threading;


namespace Atdi.Test.WebQuery.WinForm
{
    public enum TypeOperation
    {
        Insert,
        Update,
        Delete,
        Select_WithCustomExpr,
        Select_WithoutCustomExpr
    }

    public partial class TestForm : Form
    {

        public static IAuthenticationManager authManager { get; set; }
        public static UserIdentity userIdentity { get; set; }
        public static QueryMetadata queryMeta { get; set; }
        public static QueryToken token_Insert { get; set; }
        public static QueryToken token_Update { get; set; }
        public static QueryToken token_Delete { get; set; }
        public static QueryToken token_Query { get; set; }
        public static QueryToken token_QueryCustExpr { get; set; }

        public static Result<UserIdentity> ResIdent = null;
        public static Result<QueryGroups> ResQueryGroups = null;
        public static IWebQuery webQueryServ = null;

        public static int countIteration = 0;
        public static int countThreads = 0;
        public static int maxCountRecords = 0;
        public static int countIteration2 = 0;

        public static int TimeTest = 0;
        public static int CountStartThreads = 0;
        public static int CountEndThreads = 0;
        public static int maxCountRecords2 = 0;

        public static bool isInsert { get; set; }
        public static bool isUpdate { get; set; }
        public static bool isDelete { get; set; }
        public static bool isSelectWithCustomExpr { get; set; }
        public static bool isSelectWithoutCustomExpr { get; set; }


        public static int CntisInsert { get; set; }
        public static int CntisUpdate { get; set; }
        public static int CntisDelete { get; set; }
        public static int CntisSelectWithCustomExpr { get; set; }
        public static int CntisSelectWithoutCustomExpr { get; set; }


        public static long TimeInsert { get; set; }
        public static long TimeUpdate { get; set; }
        public static long TimeDelete { get; set; }
        public static long TimeSelectWithCustomExpr { get; set; }
        public static long TimeSelectWithoutCustomExpr { get; set; }

        public static long TimeInsertO { get; set; }
        public static long TimeUpdateO { get; set; }
        public static long TimeDeleteO { get; set; }
        public static long TimeSelectWithCustomExprO { get; set; }
        public static long TimeSelectWithoutCustomExprO { get; set; }

        public TestForm()
        {
            InitializeComponent();
            authManager = GetAuthenticationManager("HttpAuthenticationManager");


            CntisInsert = 0;
            CntisUpdate = 0;
            CntisDelete = 0;
            CntisSelectWithCustomExpr = 0;
            CntisSelectWithoutCustomExpr = 0;


            TimeInsert = 0;
            TimeUpdate = 0;
            TimeDelete = 0;
            TimeSelectWithCustomExpr = 0;
            TimeSelectWithoutCustomExpr = 0;

            long AllTime = 0;
            long TimeAuth = 0;


            ResIdent = Authentication("ICSM", "ICSM", out TimeAuth);
            textBox3.Text += string.Format("Call method Authentication (time in milliseconds): {0}", TimeAuth) + Environment.NewLine;
            userIdentity = ResIdent.Data;
            AllTime += TimeAuth;



            long TimeQueryGroups = 0;
            ResQueryGroups = GetQueryGroups(ResIdent, out TimeQueryGroups, out webQueryServ);
            textBox3.Text += string.Format("Call method GetQueryGroups (time in milliseconds): {0}", TimeQueryGroups) + Environment.NewLine;
            AllTime += TimeQueryGroups;
            long TimeQueryMetaData = 0;


            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Select_WithCustomExpr, out TimeQueryMetaData);

            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Select_WithoutCustomExpr, out TimeQueryMetaData);

            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Delete, out TimeQueryMetaData);

            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Insert, out TimeQueryMetaData);

            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Update, out TimeQueryMetaData);

            textBox3.Text += string.Format("Call method GetQueryMetaData (time in milliseconds): {0}", TimeQueryMetaData) + Environment.NewLine;

            tabControl1.TabPages.Remove(tabPage2);
        }


        private void StartExecThreads()
        {
            countIteration = Int32.Parse(textBox_countIteration.Text);
            countThreads = Int32.Parse(textBox_CountThreads.Text);
            maxCountRecords = Int32.Parse(textBox_MaxCountRecords.Text);

            bool is_Success;
            long result = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Task tsk = new Task(() => {
                if (textBox_CountThreads.Text != "")
                {
                    List<Thread> th = new List<Thread>();
                    for (int i = 1; i <= countThreads; i++)
                    {
                        th.Add(new Thread(StartProcessDelete)); th[th.Count - 1].Start();
                        th.Add(new Thread(StartProcessInsert)); th[th.Count - 1].Start();
                        th.Add(new Thread(StartProcessUpdate)); th[th.Count - 1].Start();
                        th.Add(new Thread(StartProcessQueryCustExpr)); th[th.Count - 1].Start();
                        th.Add(new Thread(StartProcessQueryWithoutCustExpr)); th[th.Count - 1].Start();
                    }

                    for (int i = 0; i < th.Count; i++)
                    {
                        th[i].Join();
                    }
                }
               
            });
            tsk.RunSynchronously();
            tsk.Wait();

            sw.Stop();
            result = (sw.ElapsedMilliseconds);

            if (countIteration > 0)
            {
                textBox3.Text += " Кол-во запросов типа 'Insert' " + CntisInsert + " (Сред. время работы потока): " + TimeInsert + " Сред. время на операцию: " + TimeInsert / countIteration + Environment.NewLine;
                textBox3.Text += " Кол-во запросов типа 'Update' " + CntisUpdate + " (Сред. время работы потока): " + TimeUpdate + " Сред. время на операцию: " + TimeUpdate / countIteration + Environment.NewLine;
                textBox3.Text += " Коли-во запросов типа 'Delete' " + CntisDelete + " (Сред. время работы потока): " + TimeDelete + " Сред. время на операцию: " + TimeDelete / countIteration + Environment.NewLine;
                textBox3.Text += " Кол-во запросов типа 'ExecuteQuery with CustExpr' " + CntisSelectWithCustomExpr + " (Среднее время работы потока): " + TimeSelectWithCustomExpr + " Сред. время на операцию: " + TimeSelectWithCustomExpr / countIteration + Environment.NewLine;
                textBox3.Text += " Кол-во запросов типа 'ExecuteQuery without CustExpr' " + CntisSelectWithoutCustomExpr + " (Среднее время работы потока): " + TimeSelectWithoutCustomExpr + " Сред. время на операцию: " + TimeSelectWithoutCustomExpr / countIteration + Environment.NewLine;
            }
           

            //textBox3.Text += " Общее время, которое было затрачено на обработку запросов: "+result.ToString();
        }


        private void StartExecThreadsTimePeriod()
        {
            countIteration = Int32.Parse(textBox_cnt_iteration2.Text);
            CountStartThreads = Int32.Parse(textBox_start_thread.Text);
            CountEndThreads = Int32.Parse(textBox_thread_end.Text);
            TimeTest = Int32.Parse(textBox_time_period.Text);
            maxCountRecords2 = Int32.Parse(textBox_maxcount_record2.Text);
            int iteration = 0;
            long result = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Task tsk = new Task(() => {
                if (textBox_CountThreads.Text != "")
                {
                    List<Thread> th = new List<Thread>();
                    int cnt_Thread = CountStartThreads;
                    while (sw.ElapsedMilliseconds <= TimeTest)
                    {

                        cnt_Thread = (int)(sw.ElapsedMilliseconds / (TimeTest / (CountEndThreads - CountStartThreads)));
                        if (cnt_Thread==0) cnt_Thread = CountStartThreads;

                        for (int i = CountStartThreads; i <= cnt_Thread; i++)
                        {
                            th.Add(new Thread(StartProcessDelete)); th[th.Count - 1].Start();
                            th.Add(new Thread(StartProcessInsert)); th[th.Count - 1].Start();
                            th.Add(new Thread(StartProcessUpdate)); th[th.Count - 1].Start();
                            th.Add(new Thread(StartProcessQueryCustExpr)); th[th.Count - 1].Start();
                            th.Add(new Thread(StartProcessQueryWithoutCustExpr)); th[th.Count - 1].Start();
                            iteration++;
                            if (sw.ElapsedMilliseconds > TimeTest) break;
                        }
                        if (sw.ElapsedMilliseconds > TimeTest) break;

                        for (int i = 0; i < th.Count; i++)
                        {
                            th[i].Join();
                        }
                        textBox3.Text += cnt_Thread;
                    }
                }

            });
            tsk.RunSynchronously();
            tsk.Wait();

            sw.Stop();
            result = (sw.ElapsedMilliseconds);

            if (countIteration > 0)
            {
                textBox3.Text += " Общее время теста :" + result + Environment.NewLine;
                textBox3.Text += " Кол-во запросов типа 'Insert' " + CntisInsert + " (Сред. время работы потока): " + TimeInsert + Environment.NewLine;
                textBox3.Text += " Кол-во запросов типа 'Update' " + CntisUpdate + " (Сред. время работы потока): " + TimeUpdate  + Environment.NewLine;
                textBox3.Text += " Коли-во запросов типа 'Delete' " + CntisDelete + " (Сред. время работы потока): " + TimeDelete+ Environment.NewLine;
                textBox3.Text += " Кол-во запросов типа 'ExecuteQuery with CustExpr' " + CntisSelectWithCustomExpr + " (Среднее время работы потока): " + TimeSelectWithCustomExpr  + Environment.NewLine;
                textBox3.Text += " Кол-во запросов типа 'ExecuteQuery without CustExpr' " + CntisSelectWithoutCustomExpr + " (Среднее время работы потока): " + TimeSelectWithoutCustomExpr + Environment.NewLine;
            }


            //textBox3.Text += " Общее время, которое было затрачено на обработку запросов: "+result.ToString();
        }



        private static void StartProcessInsert()
        {
            long AllTime = 0;

            {
                System.Diagnostics.Stopwatch swa = new System.Diagnostics.Stopwatch();
                swa.Start();
                for (int i = 1; i <= countIteration; i++)
                {
                    long TimeExecQuery = 0;
                    if (isInsert)
                    {
                        System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                        swo.Start();
                       // if (delay > 0) System.Threading.Thread.CurrentThread.Join(delay);
                        if (InsertRecord(webQueryServ, token_Insert, 109455 + CntisInsert))
                            CntisInsert++;
                        swo.Stop();
                        TimeInsertO = (swo.ElapsedMilliseconds);
                    }


                }
                swa.Stop();
                TimeInsert = (swa.ElapsedMilliseconds);
            }

        }

        private static void StartProcessUpdate()
        {
            long AllTime = 0;
            {
                System.Diagnostics.Stopwatch swa = new System.Diagnostics.Stopwatch();
                swa.Start();
                for (int i = 1; i <= countIteration; i++)
                    if (isUpdate)
                    {
                        System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                        swo.Start();
                        //if (delay > 0) System.Threading.Thread.CurrentThread.Join(delay);
                        if (UpdateRecord(webQueryServ, token_Update))
                        {
                            CntisUpdate++;
                        }
                        swo.Stop();
                        TimeUpdateO += (swo.ElapsedMilliseconds);
                    }

                
                swa.Stop();
                TimeUpdate = (swa.ElapsedMilliseconds);
            }

        }

        private static void StartProcessDelete()
        {
            long AllTime = 0;
            {
                System.Diagnostics.Stopwatch swa = new System.Diagnostics.Stopwatch();
                swa.Start();
                for (int i = 1; i <= countIteration; i++)
                    if (isDelete)
                    {
                        System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                        swo.Start();
                        //if (delay > 0) System.Threading.Thread.CurrentThread.Join(delay);
                        if (DeleteRecord(webQueryServ, token_Delete))
                            CntisDelete++;
                        swo.Stop();
                        TimeDeleteO = (swo.ElapsedMilliseconds);
                    }


                swa.Stop();
                TimeDelete = (swa.ElapsedMilliseconds);
            }

        }

        private static void StartProcessQueryCustExpr()
        {
            long AllTime = 0;
            long TimeExecQuery = 0;
            {
                System.Diagnostics.Stopwatch swa = new System.Diagnostics.Stopwatch();
                swa.Start();
                for (int i = 1; i <= countIteration; i++)
                    if (isSelectWithCustomExpr)
                    {
                        System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                        swo.Start();
                        //if (delay > 0) System.Threading.Thread.CurrentThread.Join(delay);
                        if (ExecuteQuery(webQueryServ, token_QueryCustExpr, GetFetchOptions(), out TimeExecQuery))
                        {
                            CntisSelectWithCustomExpr++;
                           
                        }
                        swo.Stop();
                        TimeSelectWithCustomExprO = (swo.ElapsedMilliseconds);
                    }
                 swa.Stop();
                TimeSelectWithCustomExpr = (swa.ElapsedMilliseconds);
            }

        }

        private static void StartProcessQueryWithoutCustExpr()
        {
            long AllTime = 0;
            long TimeExecQuery = 0;
            {
                System.Diagnostics.Stopwatch swa = new System.Diagnostics.Stopwatch();
                swa.Start();
                for (int i = 1; i <= countIteration; i++)
                    if (isSelectWithoutCustomExpr)
                    {
                        System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                        swo.Start();
                        //if (delay > 0) System.Threading.Thread.CurrentThread.Join(delay);
                        if (ExecuteQuery(webQueryServ, token_Query, GetFetchOptions(), out TimeExecQuery))
                        {
                            CntisSelectWithoutCustomExpr++;

                        }
                        swo.Stop();
                        TimeSelectWithoutCustomExprO = (swo.ElapsedMilliseconds);
                    }
                swa.Stop();
               TimeSelectWithoutCustomExpr = (swa.ElapsedMilliseconds);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TimeInsert = 0;
            TimeUpdate = 0;
            TimeDelete = 0;
            TimeSelectWithCustomExpr = 0;
            TimeSelectWithoutCustomExpr = 0;

            CntisInsert = 0;
            CntisUpdate = 0;
            CntisDelete = 0;
            CntisSelectWithCustomExpr = 0;
            CntisSelectWithoutCustomExpr = 0;
            textBox3.Text = "";
            StartExecThreads();
        }


        public static Result<UserIdentity> Authentication(string username, string pass, out long Time)
        {
            string res = "";
            var credential = new UserCredential
            {
                UserName = username,
                Password = pass
            };
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var authResult = authManager.AuthenticateUser(credential);
            if (authResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(authResult.FaultCause);
            }
            sw.Stop();
            long result = (sw.ElapsedMilliseconds);
            Time = result;
            return authResult;
        }

        public static Result<QueryGroups> GetQueryGroups(Result<UserIdentity> authResult, out long Time, out IWebQuery webQueryService)
        {
            var userIdentity = authResult.Data;
            webQueryService = GetWebQuery("HttpWebQuery"); // тип конечно точки зависит от конфигурации сервреа приложений
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var defGroupsResult = webQueryService.GetQueryGroups(userIdentity.UserToken);
            if (defGroupsResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(defGroupsResult.FaultCause);
            }

            var userQueryGroups = defGroupsResult.Data;

            if (userQueryGroups.Groups.Length == 0)
            {
                throw new InvalidOperationException($"Пользователю {userIdentity.Name} не назначены запросы");
            }
            sw.Stop();
            long result = (sw.ElapsedMilliseconds);
            Time = result;
            return defGroupsResult;
        }

        public static string GetQueryMetaData(IWebQuery webQueryService, Result<QueryGroups> userQueryGroupsx, TypeOperation typeOperation, out long Time)
        {
            Time = 0;
            queryMeta = null;
            string res = "";
            var userQueryGroups = userQueryGroupsx.Data;
            for (int i = 0; i < userQueryGroups.Groups.Length; i++)
            {
                var group = userQueryGroups.Groups[i];
                if (group.QueryTokens == null) continue;
                for (int j = 0; j < group.QueryTokens.Length; j++)
                {
                    var queryToken = group.QueryTokens[j];

                    if (typeOperation == TypeOperation.Delete)
                    {
                        if (queryToken.Id != 13) continue;
                        //if (queryToken.Id != 23) continue;
                    }
                    else if (typeOperation == TypeOperation.Insert)
                    {
                        if (queryToken.Id != 13) continue;
                        //if (queryToken.Id != 23) continue;
                    }
                    else if (typeOperation == TypeOperation.Select_WithCustomExpr)
                    {
                        if (queryToken.Id != 46) continue;
                        //if (queryToken.Id != 23) continue;
                    }
                    else if (typeOperation == TypeOperation.Select_WithoutCustomExpr)
                    {
                        if (queryToken.Id != 13) continue;
                        //if (queryToken.Id != 20) continue;
                    }
                    else if (typeOperation == TypeOperation.Update)
                    {
                        if (queryToken.Id != 47) continue;
                        //if (queryToken.Id != 23) continue;
                    }
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    var defQueryMetadataResult = webQueryService.GetQueryMetadata(userIdentity.UserToken, queryToken);
                    queryMeta = defQueryMetadataResult.Data;
                    if (typeOperation == TypeOperation.Delete)
                    {
                        token_Delete = queryMeta.Token;
                    }
                    else if (typeOperation == TypeOperation.Insert)
                    {
                        token_Insert = queryMeta.Token;
                    }
                    else if (typeOperation == TypeOperation.Select_WithCustomExpr)
                    {
                        token_QueryCustExpr = queryMeta.Token;
                    }
                    else if (typeOperation == TypeOperation.Select_WithoutCustomExpr)
                    {
                        token_Query = queryMeta.Token;
                    }
                    else if (typeOperation == TypeOperation.Update)
                    {
                        token_Update = queryMeta.Token;
                    }


                    if (defQueryMetadataResult.State == OperationState.Fault)
                    {
                        throw new InvalidOperationException(defQueryMetadataResult.FaultCause);
                    }
                    sw.Stop();
                    long result = (sw.ElapsedMilliseconds);
                    Time = result;
                    res = string.Format("Call method GetQueryMetaData (time in milliseconds): {0}", result) + Environment.NewLine;
                    break;
                }
            }
            return res;
        }

        public static bool ExecuteQuery(IWebQuery webQueryService, QueryToken token, FetchOptions fetch, out long TimeExecQuery)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var executingResult = webQueryService.ExecuteQuery(userIdentity.UserToken, token, fetch);
            // Валидация результата
            if (executingResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(executingResult.FaultCause);
            }
            sw.Stop();
            long result = (sw.ElapsedMilliseconds);
            TimeExecQuery = result;
            return true;

        }


        public static bool DeleteRecord(IWebQuery webQueryService, QueryToken token)
        {
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
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "ID" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.StringValueOperand{ Value =  "36950" }
                                        //RightOperand = new DataModels.DataConstraint.IntegerValueOperand{ Value =  1 }
                                    },
                                }
                            }

                        }
                   }

            };

            var chsResult = webQueryService.SaveChanges(userIdentity.UserToken, token, changeset);
            if (chsResult.State == OperationState.Success) return true;
            else return false;
        }

        public static bool UpdateRecord(IWebQuery webQueryService, QueryToken token)
        {
            var changeset = new DataModels.Changeset
            {
                Id = Guid.NewGuid(),
                Actions = new DataModels.Action[]
                {
            new DataModels.TypedRowUpdationAction
            {
                Id = Guid.NewGuid(),
                Condition = new DataModels.DataConstraint.ComplexCondition
                {
                    Operator = DataModels.DataConstraint.LogicalOperator.Or,
                    Conditions = new DataModels.DataConstraint.Condition[]
                    {
                                     new DataModels.DataConstraint.ConditionExpression
                                    {
                                        LeftOperand = new DataModels.DataConstraint.ColumnOperand{ ColumnName = "ID" },
                                        Operator = DataModels.DataConstraint.ConditionOperator.Equal,
                                        RightOperand = new DataModels.DataConstraint.IntegerValueOperand{ Value =  18501 }
                                       // RightOperand = new DataModels.DataConstraint.IntegerValueOperand{ Value = 2 }
                                    },

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
                    StringCells = new string[] { "RR" },
                    IntegerCells = new int?[] { 1 },
                    DecimalCells = new decimal?[] { (decimal?)123 }
                }
                }
               }

            };

            var chsResult = webQueryService.SaveChanges(userIdentity.UserToken, token, changeset);
            if (chsResult.State == OperationState.Success) return true;
            else return false;
        }
    

        public static bool InsertRecord(IWebQuery webQueryService, QueryToken token, int IDNew)
        {
            var changeset = new DataModels.Changeset
            {
                Id = Guid.NewGuid(),
                Actions = new DataModels.Action[]
                   {
                        new DataModels.TypedRowCreationAction
                        {
                            Id =  Guid.NewGuid(),
                            Type = DataModels.ActionType.Create,

                            Columns = new DataModels.DataSetColumn[]
                            {
                                 new DataModels.DataSetColumn
                                {
                                    Name = "NAME", Type = DataModels.DataType.String, Index = 1
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "NETWORK_IDENT", Type = DataModels.DataType.String, Index = 0
                                },
                                new DataModels.DataSetColumn
                                {
                                    Name = "ID", Type = DataModels.DataType.Integer, Index = 0
                                },

                            },
                            Row = new DataModels.TypedDataRow
                            {
                                StringCells = new string[] { "VAL","RA67834"},
                                IntegerCells = new int?[] { IDNew },

                            }
                        }
                   }

            };

            var chsResult = webQueryService.SaveChanges(userIdentity.UserToken, token, changeset);
            if (chsResult.State == OperationState.Success) return true;
            else return false;
        }


        public static FetchOptions GetFetchOptions()
        {
            return  new FetchOptions
            {
                Id = Guid.NewGuid(),
                ResultStructure = DataSetStructure.StringRows,
                Orders = new OrderExpression[] // указываем условие сортировки
                {
                    new OrderExpression { ColumnName = "ID", OrderType = OrderType.Descending }//,
                },
              
                Condition = new ConditionExpression // указываем условие выборки
                {
                    LeftOperand = new ColumnOperand { ColumnName = "ID" },
                    Operator = ConditionOperator.Between,
                    // RightOperand = new IntegerValuesOperand { Values = new int?[2] {1300000,2000000 } }
                    RightOperand = new IntegerValuesOperand { Values = new int?[2] { 18500, 40000 } }
                    //RightOperand = new IntegerValuesOperand { Values = new int?[2] { 1, 40000 } }
                },
                
                 Limit = new DataLimit
                 {
                      Type = LimitValueType.Records,
                      Value = maxCountRecords
                 }
            };
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

        private void checkBox_QueryMetaData_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_Authorization_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_Insert_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Insert.Checked)
            {
                isInsert = true;
            }
            else isInsert = false;


        }

        private void checkBox_Update_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Update.Checked)
            {
                isUpdate = true;
            }
            else isUpdate = false;
        }

        private void checkBox_Delete_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Delete.Checked)
            {
                isDelete = true;
            }
            else isDelete = false;
        }

        private void checkBox_ExecuteQueryCustomExpr_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_ExecuteQueryCustomExpr.Checked)
            {
                isSelectWithCustomExpr = true;
            }
            else isSelectWithCustomExpr = false;
        }

        private void checkBox_ExecQuery_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_ExecQuery.Checked)
            {
                isSelectWithoutCustomExpr = true;
            }
            else isSelectWithoutCustomExpr = false;
        }

        private void button_run_time_Click(object sender, EventArgs e)
        {

            TimeInsert = 0;
            TimeUpdate = 0;
            TimeDelete = 0;
            TimeSelectWithCustomExpr = 0;
            TimeSelectWithoutCustomExpr = 0;

            CntisInsert = 0;
            CntisUpdate = 0;
            CntisDelete = 0;
            CntisSelectWithCustomExpr = 0;
            CntisSelectWithoutCustomExpr = 0;
            textBox3.Text = "";
            StartExecThreadsTimePeriod();
        }

        private void checkBox_QueryGroups_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
