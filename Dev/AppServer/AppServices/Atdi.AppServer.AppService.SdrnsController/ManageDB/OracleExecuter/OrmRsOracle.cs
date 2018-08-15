using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace Atdi.Oracle.DataAccess
{
    public class OrmRsOracle : IDisposable
    {
        public OracleDataAccess oracleData;
        public bool isNew = false;
        public Yyy yyy { get; set; }
        private int index = -1;
        private int cnt = 0;
        public List<OracleParameter> paramsOracle { get; set; }
        public List<OracleParameter> paramsOracle2 { get; set; }
        public OracleParameter[][] params_val { get; set; }

        public List<string> AllFields = new List<string>();
        public List<string> AllFields2 = new List<string>();
        public List<OracleParameter> AllPropertiesColumns = new List<OracleParameter>();
        public List<OracleParameter> AllPropertiesColumns2 = new List<OracleParameter>();
        public string Order { get; set; }


        public void Dispose() { Clear(); }
        public void Clear()
        {
            isNew = false;
            AllPropertiesColumns = new List<OracleParameter>();
            AllPropertiesColumns2 = new List<OracleParameter>();
            params_val = null;
            index = -1;
            cnt = 0;
            AllFields = new List<string>();
            AllFields2 = new List<string>();
            paramsOracle = new List<OracleParameter>();
            paramsOracle2 = new List<OracleParameter>();
            yyy = new Yyy();
        }

        public void Init(OracleDataAccess rs)
        {
            isNew = false;
            AllPropertiesColumns = new List<OracleParameter>();
            params_val = null;
            Clear(); oracleData = rs;
            if (oracleData == null) throw new Exception("Recordset cannot be opened without OracleDataAccess");
            paramsOracle = new List<OracleParameter>();
            yyy = (Yyy)rs;
            AllPropertiesColumns = yyy.getAllFields;
            AllFields = new List<string>();
            if (yyy.FormatValue == "*")
            {
                AllFields = AllPropertiesColumns.Select(t => t.SourceColumn).ToList();
            }
            else
            {
                if (yyy.FormatValue == null) { yyy.FormatValue = "*"; AllFields = AllPropertiesColumns.Select(t => t.SourceColumn).ToList(); }
                else
                {
                    // в таком варианте на данный момент не работает,
                    // необходимо разобраться с индексами
                    string[] val = yyy.FormatValue.Split(new char[] { ',' });
                    AllFields = val.ToList();
                }
            }

            if (!string.IsNullOrEmpty(yyy.Order))
            {
                string temp = yyy.Order;
                int idx_start = temp.IndexOf("[");
                int idx_end = temp.IndexOf("]");
                string NameColumnOrder = temp.Substring(idx_start + 1, idx_end - idx_start - 1);
                if (yyy.Order.Contains("DESC")) Order = NameColumnOrder + " DESC";
                if (yyy.Order.Contains("ASC")) Order = NameColumnOrder + " ASC";
            }
        }

        public void OpenRs(OracleDataAccess oracleData)
        {
            isNew = false;
            index = 0;
            cnt = 0;
            params_val = oracleData.GetValues(AllFields, yyy.TableName, yyy.Filter, Order);
            List<object[]> AllObjLObj = new List<object[]>();
            List<object> LObj = new List<object>();
            foreach (OracleParameter[] v in params_val.ToList())
            {
                LObj = new List<object>();
                foreach (OracleParameter x in v) LObj.Add(x.Value);
                AllObjLObj.Add(LObj.ToArray());
            }
            yyy.allvalc = AllObjLObj.ToArray();
            if (yyy.allvalc.Length > 0) { index = 0; cnt = yyy.allvalc.Length; yyy.valc = yyy.allvalc[index]; }
        }

        public void OpenRs()
        {
            isNew = false;
            index = 0;
            cnt = 0;
            params_val = oracleData.GetValues(AllFields, yyy.TableName, yyy.Filter, Order);
            List<object[]> AllObjLObj = new List<object[]>();
            List<object> LObj = new List<object>();
            foreach (OracleParameter[] v in params_val.ToList())
            {
                LObj = new List<object>();
                foreach (OracleParameter x in v) LObj.Add(x.Value);
                AllObjLObj.Add(LObj.ToArray());
            }
            yyy.allvalc = AllObjLObj.ToArray();
            if (yyy.allvalc.Length > 0) { index = 0; cnt = yyy.allvalc.Length; yyy.valc = yyy.allvalc[index]; }
        }

        public bool IsEOF()
        {
            if (index >= cnt) return true;
            else return false;
        }

        public void MoveNext()
        {
            ++index;
            if ((index >= 0) && (index < cnt)) yyy.valc = yyy.allvalc[index];
        }

        public int GetCount()
        {
            return cnt;
        }

        public void New()
        {
            isNew = true;
            yyy.valc = new object[AllPropertiesColumns.Count];
        }

        public bool DeleteRecord()
        {
            int ID_VALUE = -1;
            int i = 0;
            foreach (string val in AllFields)
            {
                foreach (OracleParameter x in AllPropertiesColumns)
                {
                    if (x.SourceColumn == val)
                    {
                        if (x.SourceColumn == "\"ID\"")
                        {
                            ID_VALUE = (int)yyy.valc[i];
                            break;
                        }
                    }
                }
                if (ID_VALUE > -1) break;
            }
            return oracleData.DeleteRecord(yyy.GetTableName(), ID_VALUE);
        }

        public int? UpdateRecord()
        {
            int ID_VALUE = -1;
            int i = 0;
            paramsOracle.Clear();
            foreach (string val in AllFields)
            {
                foreach (OracleParameter x in AllPropertiesColumns)
                {
                    if (x.SourceColumn == val)
                    {
                        if (x.SourceColumn != "\"ID\"")
                        {
                            if (yyy.valc[i] != null)
                            {
                                paramsOracle.Add(new OracleParameter()
                                {
                                    SourceColumn = val,
                                    ParameterName = ":" + val,
                                    OracleDbType = x.OracleDbType,
                                    Direction = System.Data.ParameterDirection.Input,
                                    Value = yyy.valc[i]
                                });
                            }
                            i++;
                            break;
                        }
                        else
                        {
                            ID_VALUE = (int)yyy.valc[i];
                            i++;
                        }
                    }
                }
            }
            return oracleData.UpdateRecord(paramsOracle, yyy.GetTableName(), ID_VALUE);
        }

        public int? InsertRecord()
        {
            int? ID = null;
            int i = 0;
            paramsOracle.Clear();
            foreach (string val in AllFields)
            {
                foreach (OracleParameter x in AllPropertiesColumns)
                {
                    if (x.SourceColumn == val)
                    {
                        if (x.SourceColumn != "\"ID\"")
                        {
                            if (yyy.valc[i] != null)
                            {
                                paramsOracle.Add(new OracleParameter()
                                {
                                    SourceColumn = val,
                                    ParameterName = ":" + val,
                                    OracleDbType = x.OracleDbType,
                                    Direction = System.Data.ParameterDirection.Input,
                                    Value = yyy.valc[i]
                                });
                            }
                            i++;
                            break;
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
            }

            ID = oracleData.InsertRecord(paramsOracle, yyy.GetTableName());
            return ID;
        }

        public bool InsertBulkRecords(List<Yyy> ListY)
        {
            bool isSuccess = false;

            List<OracleParameter> AllPropertiesColumns_ = new List<OracleParameter>();
            paramsOracle = new List<OracleParameter>();
            int r = 1;
            foreach (Yyy vn in ListY)
            {
                AllPropertiesColumns = vn.getAllFields;
                int i = 0;
                //paramsOracle.Clear();
                foreach (string val in AllFields)
                {
                    foreach (OracleParameter x in AllPropertiesColumns)
                    {
                        if (x.SourceColumn == val)
                        {
                            if (x.SourceColumn != "\"ID\"")
                            {
                                if (x.Value != null)
                                {
                                   string valr = val.Insert(val.Length - 1, "_" + r.ToString());
                                    paramsOracle.Add(new OracleParameter()
                                    {
                                        SourceColumn = val,
                                        ParameterName = ":" + valr,
                                        OracleDbType = x.OracleDbType,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = x.Value
                                    });
                                }
                                i++;
                                break;
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                }
                r++;
            }
            isSuccess = oracleData.InsertBulkRecords(paramsOracle, yyy.GetTableName(), ListY.Count);
            return isSuccess;
        }

        public bool InsertBulkRecords(List<Yyy> ListY1, List<Yyy> ListY2, OracleParameter[] oracleParameter)
        {
            bool isSuccess = false;
            string tableName1 = "";
            List<OracleParameter> AllPropertiesColumns_ = new List<OracleParameter>();
            paramsOracle = new List<OracleParameter>();
            paramsOracle2 = new List<OracleParameter>();
            int r = 1;
            foreach (Yyy vn in ListY1)
            {
                tableName1 = vn.GetTableName();
                AllPropertiesColumns = vn.getAllFields;
                int i = 0;
                //paramsOracle.Clear();
                foreach (string val in AllFields)
                {
                    foreach (OracleParameter x in AllPropertiesColumns)
                    {
                        if (x.SourceColumn == val)
                        {
                            if (x.SourceColumn != "\"ID\"")
                            {
                                if (x.Value != null)
                                {
                                    string valr = val.Insert(val.Length - 1, "_" + r.ToString());
                                    paramsOracle.Add(new OracleParameter()
                                    {
                                        SourceColumn = val,
                                        ParameterName = ":" + valr,
                                        OracleDbType = x.OracleDbType,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = x.Value
                                    });
                                }
                                i++;
                                break;
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                }
                r++;
            }
            string tableName2 = "";
            paramsOracle2.AddRange(oracleParameter);
            foreach (Yyy vn in ListY2)
            {
                tableName2 = vn.GetTableName();
                AllPropertiesColumns2 = vn.getAllFields;
                int i = 0;
                //paramsOracle.Clear();

                foreach (string val in AllFields2)
                {
                    foreach (OracleParameter x in AllPropertiesColumns2)
                    {
                        if (x.SourceColumn == val)
                        {
                            if (x.SourceColumn != "\"ID\"")
                            {
                                if (x.Value != null)
                                {
                                    string valr = val.Insert(val.Length - 1, "_" + r.ToString());
                                    paramsOracle2.Add(new OracleParameter()
                                    {
                                        SourceColumn = val,
                                        ParameterName = ":" + valr,
                                        OracleDbType = x.OracleDbType,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = x.Value
                                    });
                                }
                                i++;
                                break;
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                }
                r++;
            }
            isSuccess = oracleData.InsertBulkRecords(paramsOracle, tableName1, ListY1.Count, paramsOracle2, tableName2, ListY2.Count, oracleParameter);
            return isSuccess;
        }
    }
}
