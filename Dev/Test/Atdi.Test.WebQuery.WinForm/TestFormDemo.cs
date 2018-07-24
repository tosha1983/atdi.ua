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

    public partial class TestFormDemo : Form
    {

        public static IAuthenticationManager authManager { get; set; }
        public static UserIdentity userIdentity { get; set; }
        public static QueryMetadata queryMeta { get; set; }
        public static List<cbItemMetaData> list_queryMeta { get; set; }
        //public static QueryToken token_Insert { get; set; }
        //public static QueryToken token_Update { get; set; }
        //public static QueryToken token_Delete { get; set; }
        //public static QueryToken token_Query { get; set; }
        //public static QueryToken token_QueryCustExpr { get; set; }

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


        public static int IdInsert { get; set; }
        public static int IdUpdate { get; set; }
        public static int IdDelete { get; set; }
        public static int IdSelectWithCustomExpr { get; set; }
        public static int IdSelectWithoutCustomExpr { get; set; }


        public static cbItemMetaData CurrInsert { get; set; }
        public static cbItemMetaData CurrUpdate { get; set; }
        public static cbItemMetaData CurrDelete { get; set; }
        public static cbItemMetaData CurrSelectWithCustomExpr { get; set; }
        public static cbItemMetaData CurrSelectWithoutCustomExpr { get; set; }


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

        public static object ListValueExecuteCustExpr { get; set; }
        public static object ListValueExecute{ get; set; }

        public static int LimitsSelectWithCustomExpr { get; set; }
        public static int LimitSelectWithoutCustomExpr { get; set; }

        public TestFormDemo()
        {
            InitializeComponent();
            authManager = GetAuthenticationManager("HttpAuthenticationManager");
            list_queryMeta = new List<cbItemMetaData>();

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
            userIdentity = ResIdent.Data;
            AllTime += TimeAuth;



            long TimeQueryGroups = 0;
            ResQueryGroups = GetQueryGroups(ResIdent, out TimeQueryGroups, out webQueryServ);
            AllTime += TimeQueryGroups;
            long TimeQueryMetaData = 0;


            comboBox_Insert.Items.Clear();
            comboBox_ExecuteQuery.Items.Clear();
            comboBox_ExecuteQueryCustExpr.Items.Clear();
            comboBox_Update.Items.Clear();
            comboBox_Delete.Items.Clear();

            GetQueryMetaData(webQueryServ, ResQueryGroups);
            foreach (cbItemMetaData metax in list_queryMeta)
            {
                comboBox_Insert.Items.Add(metax);
                comboBox_ExecuteQuery.Items.Add(metax);
                comboBox_ExecuteQueryCustExpr.Items.Add(metax);
                comboBox_Update.Items.Add(metax);
                comboBox_Delete.Items.Add(metax);
            }

            /*
            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Select_WithCustomExpr, out TimeQueryMetaData);

            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Select_WithoutCustomExpr, out TimeQueryMetaData);

            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Delete, out TimeQueryMetaData);

            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Insert, out TimeQueryMetaData);

            GetQueryMetaData(webQueryServ, ResQueryGroups, TypeOperation.Update, out TimeQueryMetaData);
            */


        }


        private void StartExecThreads()
        {

            bool is_Success;
            long result = 0;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Task tsk = new Task(() =>
            {
                List<Thread> th = new List<Thread>();
                th.Add(new Thread(StartProcessDelete)); th[th.Count - 1].Start();
                th.Add(new Thread(StartProcessInsert)); th[th.Count - 1].Start();
                th.Add(new Thread(StartProcessUpdate)); th[th.Count - 1].Start();
                th.Add(new Thread(StartProcessQueryCustExpr)); th[th.Count - 1].Start();
                th.Add(new Thread(StartProcessQueryWithoutCustExpr)); th[th.Count - 1].Start();

                for (int i = 0; i < th.Count; i++)
                {
                    th[i].Join();
                }


            });
            tsk.RunSynchronously();
            tsk.Wait();

            sw.Stop();
            result = (sw.ElapsedMilliseconds);


        }


     


        private static void StartProcessInsert()
        {
            long AllTime = 0;

            {
                System.Diagnostics.Stopwatch swa = new System.Diagnostics.Stopwatch();
                swa.Start();
                for (int i = 1; i <= 1; i++)
                {
                    long TimeExecQuery = 0;
                    if (isInsert)
                    {
                        System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                        swo.Start();
                        if (InsertRecord(webQueryServ, CurrInsert.queryMeta.Token, IdInsert))
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
                for (int i = 1; i <= 1; i++)
                    if (isUpdate)
                    {
                        System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                        swo.Start();
                        if (UpdateRecord(webQueryServ, CurrUpdate.queryMeta.Token))
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
                for (int i = 1; i <= 1; i++)
                    if (isDelete)
                    {
                        System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                        swo.Start();
                        if (DeleteRecord(webQueryServ, CurrDelete.queryMeta.Token))
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
            if (CurrSelectWithCustomExpr == null) return;
            long AllTime = 0;
            long TimeExecQuery = 0;
            {
                System.Diagnostics.Stopwatch swa = new System.Diagnostics.Stopwatch();
                swa.Start();
                for (int i = 1; i <= 1; i++)
                    if (isSelectWithCustomExpr)
                    {
                        
                                System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                                swo.Start();
                                if (ExecuteQuery(webQueryServ, CurrSelectWithCustomExpr.queryMeta.Token, GetFetchOptions(TypeOperation.Select_WithCustomExpr), out TimeExecQuery, TypeOperation.Select_WithCustomExpr))
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
            if (CurrSelectWithoutCustomExpr == null) return;
            long AllTime = 0;
            long TimeExecQuery = 0;
            {
                System.Diagnostics.Stopwatch swa = new System.Diagnostics.Stopwatch();
                swa.Start();
                for (int i = 1; i <= 1; i++)
                    if (isSelectWithoutCustomExpr)
                    {
                        System.Diagnostics.Stopwatch swo = new System.Diagnostics.Stopwatch();
                        swo.Start();
                        if (ExecuteQuery(webQueryServ, CurrSelectWithoutCustomExpr.queryMeta.Token, GetFetchOptions(TypeOperation.Select_WithoutCustomExpr), out TimeExecQuery, TypeOperation.Select_WithoutCustomExpr))
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
            label_time_update.Text = "";
            label_time_insert.Text = "";
            label_time_executeQueryCustExpr.Text = "";
            label_time_ExecuteQuery.Text = "";
            label_time_delete.Text = "";

            ListValueExecuteCustExpr = null;
            ListValueExecuteCustExpr = null;

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

            IdInsert = 0;
            IdDelete = 0;
            IdUpdate = 0;
            IdSelectWithCustomExpr = 0;
            IdSelectWithoutCustomExpr = 0;

            if (checkBox_Insert.Checked)    IdInsert = int.Parse(textBox_ID_Insert.Text);
            if (checkBox_Update.Checked) IdUpdate = int.Parse(textBox_Upate_ID.Text);
            if (checkBox_Delete.Checked) IdDelete =  int.Parse(textBox_Delete_ID.Text);


            LimitsSelectWithCustomExpr = int.Parse(textBox_limit_Cust_Expr.Text);
            LimitSelectWithoutCustomExpr = int.Parse(textBox_Limit_Query.Text);


            dataGridView_ExecuteQuery_CustomExpress.Columns.Clear();
            dataGridView_ExecuteQuery_CustomExpress.Rows.Clear();
            dataGridView_ExecuteQuery_CustomExpress.Refresh();

            dataGridView_ExecuteQuery.Columns.Clear();
            dataGridView_ExecuteQuery.Rows.Clear();
            dataGridView_ExecuteQuery.Refresh();

            StartExecThreads();

            if (checkBox_Delete.Checked)
                label_time_delete.Text = TimeDelete.ToString()+ " мілісекунд";

            if (checkBox_Insert.Checked)
                label_time_insert.Text = TimeInsert.ToString() + " мілісекунд";

            if (checkBox_Update.Checked)
                label_time_update.Text = TimeUpdate.ToString() + " мілісекунд";


            if (checkBox_ExecuteQueryCustomExpr.Checked)
            {

                if (ListValueExecuteCustExpr != null)
                {
                    dataGridView_ExecuteQuery_CustomExpress.AutoGenerateColumns = false;
                    List<StringDataRow> LD = (List<StringDataRow>)((StringRowsDataSet)ListValueExecuteCustExpr).Rows.ToList();
                    IEnumerable<string[]> Rx = LD.Select(r => r.Cells);
                    foreach (DataSetColumn c in ((StringRowsDataSet)ListValueExecuteCustExpr).Columns)
                    {
                        var columnSpec = new DataGridViewColumn
                        {
                            ValueType = typeof(string), // This is of type System.Type
                            Name = c.Name, // This is of type string
                            CellTemplate = new DataGridViewTextBoxCell()
                        };
                        dataGridView_ExecuteQuery_CustomExpress.Columns.Add(columnSpec);
                    }

                    foreach (string[] r in Rx)
                        dataGridView_ExecuteQuery_CustomExpress.Rows.Add(r);
                    dataGridView_ExecuteQuery_CustomExpress.Refresh();

                    label_time_executeQueryCustExpr.Text = TimeSelectWithCustomExpr.ToString() + " мілісекунд";
                }
            }

            if (checkBox_ExecQuery.Checked)
            {
                if (ListValueExecute != null)
                {

                    dataGridView_ExecuteQuery.AutoGenerateColumns = false;
                    List<StringDataRow> LD = (List<StringDataRow>)((StringRowsDataSet)ListValueExecute).Rows.ToList();
                    IEnumerable<string[]> Rx = LD.Select(r => r.Cells);
                    foreach (DataSetColumn c in ((StringRowsDataSet)ListValueExecute).Columns)
                    {
                        var columnSpec = new DataGridViewColumn
                        {
                            ValueType = typeof(string), // This is of type System.Type
                            Name = c.Name, // This is of type string
                            CellTemplate = new DataGridViewTextBoxCell()
                        };
                        dataGridView_ExecuteQuery.Columns.Add(columnSpec);
                    }

                    foreach (string[] r in Rx)
                        dataGridView_ExecuteQuery.Rows.Add(r);
                    dataGridView_ExecuteQuery.Refresh();

                    label_time_ExecuteQuery.Text = TimeSelectWithoutCustomExpr.ToString()+" мілісекунд";
                }
            }
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

        /*
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
                       
                    }
                    else if (typeOperation == TypeOperation.Insert)
                    {
                        if (queryToken.Id != 13) continue;
                      
                    }
                    else if (typeOperation == TypeOperation.Select_WithCustomExpr)
                    {
                        if (queryToken.Id != 46) continue;
                      
                    }
                    else if (typeOperation == TypeOperation.Select_WithoutCustomExpr)
                    {
                        if (queryToken.Id != 13) continue;
                       
                    }
                    else if (typeOperation == TypeOperation.Update)
                    {
                        if (queryToken.Id != 47) continue;
                       
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
        */

        public static void GetQueryMetaData(IWebQuery webQueryService, Result<QueryGroups> userQueryGroupsx)
        {
            var userQueryGroups = userQueryGroupsx.Data;
            for (int i = 0; i < userQueryGroups.Groups.Length; i++)
            {
                var group = userQueryGroups.Groups[i];
                if (group.QueryTokens == null) continue;
                for (int j = 0; j < group.QueryTokens.Length; j++)
                {
                    var queryToken = group.QueryTokens[j];
                    var defQueryMetadataResult = webQueryService.GetQueryMetadata(userIdentity.UserToken, queryToken);
                    cbItemMetaData cb = new cbItemMetaData();
                    cb.queryMeta = new QueryMetadata();
                    cb.queryMeta = defQueryMetadataResult.Data;
                    list_queryMeta.Add(cb);
                    if (defQueryMetadataResult.State == OperationState.Fault)
                    {
                        throw new InvalidOperationException(defQueryMetadataResult.FaultCause);
                    }
                  

                }
            }
        }

        public static bool ExecuteQuery(IWebQuery webQueryService, QueryToken token, FetchOptions fetch, out long TimeExecQuery, TypeOperation operation)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var executingResult = webQueryService.ExecuteQuery(userIdentity.UserToken, token, fetch);
            // Валидация результата
            if (executingResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(executingResult.FaultCause);
            }
            if (executingResult.Data.Dataset.Structure == DataSetStructure.StringRows)
            {
                if (operation== TypeOperation.Select_WithoutCustomExpr)
                 ListValueExecute = (executingResult.Data.Dataset as StringRowsDataSet); 
                else if (operation == TypeOperation.Select_WithCustomExpr)
                    ListValueExecuteCustExpr = (executingResult.Data.Dataset as StringRowsDataSet);
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
                                        RightOperand = new DataModels.DataConstraint.IntegerValueOperand{ Value =  IdDelete }
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
                                        RightOperand = new DataModels.DataConstraint.IntegerValueOperand{ Value =  IdUpdate }
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


        public static FetchOptions GetFetchOptions(TypeOperation operation)
        {
            FetchOptions fetch = new FetchOptions();
            if (operation == TypeOperation.Delete)
            {
                fetch = new FetchOptions
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
                        Operator = ConditionOperator.Equal,
                        RightOperand = new IntegerValuesOperand { Values = new int?[1] { IdDelete } }
                    },

                    Limit = new DataLimit
                    {
                        Type = LimitValueType.Records,
                        Value = 1
                    }
                };
            }
            else if (operation == TypeOperation.Insert)
            {
                fetch = new FetchOptions
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
                        Operator = ConditionOperator.Equal,
                        RightOperand = new IntegerValuesOperand { Values = new int?[1] { IdInsert } }
                    },

                    Limit = new DataLimit
                    {
                        Type = LimitValueType.Records,
                        Value = 1
                    }
                };
            }
            else if (operation == TypeOperation.Update)
            {
                fetch = new FetchOptions
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
                        Operator = ConditionOperator.Equal,
                        RightOperand = new IntegerValuesOperand { Values = new int?[1] { IdUpdate } }
                    },

                    Limit = new DataLimit
                    {
                        Type = LimitValueType.Records,
                        Value = 1
                    }
                };
            }
            else if (operation == TypeOperation.Select_WithCustomExpr)
            {
                
                fetch = new FetchOptions
                {
                    Id = Guid.NewGuid(),
                    ResultStructure = DataSetStructure.StringRows,
                    Orders = new OrderExpression[] // указываем условие сортировки
                    {
                    new OrderExpression { ColumnName = "ID", OrderType = OrderType.Descending }//,
                    },
                    /*
                    Condition = new ConditionExpression // указываем условие выборки
                    {
                        LeftOperand = new ColumnOperand { ColumnName = "ID" },
                        Operator = ConditionOperator.Equal,
                        RightOperand = new IntegerValuesOperand { Values = new int?[1] { IdSelectWithCustomExpr } }
                    },
                    */

                    Limit = new DataLimit
                    {
                        Type = LimitValueType.Records,
                        Value = LimitsSelectWithCustomExpr
                    }
                };
            }
            else if (operation == TypeOperation.Select_WithoutCustomExpr)
            {
                fetch = new FetchOptions
                {
                    Id = Guid.NewGuid(),
                    ResultStructure = DataSetStructure.StringRows,
                    Orders = new OrderExpression[] // указываем условие сортировки
                    {
                    new OrderExpression { ColumnName = "ID", OrderType = OrderType.Descending }//,
                    },
                    /*
                    Condition = new ConditionExpression // указываем условие выборки
                    {
                        LeftOperand = new ColumnOperand { ColumnName = "ID" },
                        Operator = ConditionOperator.Equal,
                        RightOperand = new IntegerValuesOperand { Values = new int?[1] { IdSelectWithoutCustomExpr } }
                    },
                    */

                    Limit = new DataLimit
                    {
                        Type = LimitValueType.Records,
                        Value = LimitSelectWithoutCustomExpr
                    }
                };
            }
            return fetch;
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

        private void comboBox_Insert_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox_Insert.SelectedIndex>-1)   CurrInsert = (cbItemMetaData)comboBox_Insert.Items[comboBox_Insert.SelectedIndex];
        }

        private void comboBox_Update_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox_Update_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox_Update.SelectedIndex>-1) CurrUpdate = (cbItemMetaData)comboBox_Update.Items[comboBox_Update.SelectedIndex];
        }

        private void comboBox_Delete_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox_Delete.SelectedIndex>-1) CurrDelete = (cbItemMetaData)comboBox_Delete.Items[comboBox_Delete.SelectedIndex];
        }

        private void comboBox_ExecuteQueryCustExpr_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox_ExecuteQueryCustExpr.SelectedIndex>-1) CurrSelectWithCustomExpr = (cbItemMetaData)comboBox_ExecuteQueryCustExpr.Items[comboBox_ExecuteQueryCustExpr.SelectedIndex];
        }

        private void comboBox_ExecuteQuery_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox_ExecuteQuery.SelectedIndex>-1) CurrSelectWithoutCustomExpr = (cbItemMetaData)comboBox_ExecuteQuery.Items[comboBox_ExecuteQuery.SelectedIndex];
        }

        private void checkBox_ExecQuery_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_ExecQuery.Checked) isSelectWithoutCustomExpr = true;
            else isSelectWithoutCustomExpr = false;
        }

        private void checkBox_ExecuteQueryCustomExpr_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_ExecuteQueryCustomExpr.Checked) isSelectWithCustomExpr = true;
            else isSelectWithCustomExpr = false;
        }

        private void checkBox_Delete_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Delete.Checked) isDelete = true;
            else isDelete = false;
        }

        private void checkBox_Update_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Update.Checked) isUpdate = true;
            else isUpdate = false;
        }

        private void checkBox_Insert_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Insert.Checked) isInsert = true;
            else isInsert = false;
        }
    }

    public class cbItemMetaData
    {
        public QueryMetadata queryMeta { get; set; }

        public override string ToString()
        {
            return queryMeta.Name + " (ID = " + queryMeta.Token.Id + ")";
        }

    }
}
