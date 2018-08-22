using System;
using Atdi.Oracle.DataAccess;
using Oracle.DataAccess.Client;
using System.Collections;
using System.Collections.Generic;

namespace Atdi.Oracle.DataAccess
{
    public class Yyy : OracleDataAccess
    {
        OrmRsOracle rs;
        public object[][] allvalc;
        public object[] valc;
        public string TableName = "";
        public List<OracleParameter> getAllFields = new List<OracleParameter>();
        public Yyy getYyy(int idx) { try { return (Yyy)valc[idx]; } catch (Exception) { return new Yyy(); } }
        public string getString(int idx) { try { return valc[idx] as string; } catch (Exception) { return ""; }}
        public int? getInt(int idx) {
            int? val = null;
            try
            { 
             if ((valc[idx] != null) && (valc[idx] != DBNull.Value)) val = Convert.ToInt32(valc[idx]);
            } catch (Exception) { return null; }
            return val;
        }
        public double? getDouble(int idx) {
            double? val = null;
            try
            {
               if ((valc[idx] != null) && (valc[idx] != DBNull.Value)) val = Convert.ToDouble(valc[idx]);
            } catch (Exception) { return null; }
            return val;
        }
        public DateTime? getDateTime(int idx)
        {
            DateTime? val = new DateTime?();
            try
            {
                object o = valc[idx];
                if ((o == null) || (o == DBNull.Value))
                {
                    val = new DateTime?();
                }
                else
                {
                    val = valc[idx] as DateTime?;
                }
            }
            catch (Exception) { return null; }
            return val;
        }
        public Guid? getGuid(int idx) { try { return valc[idx] as Guid?; } catch (Exception) { return null; } }
        public object getObject(int idx) { try { return valc[idx] as object; } catch (Exception) { return null; } }
        public void setYyy(int idx, Yyy value) { try { valc[idx] = value; } catch (Exception) {  } }

        public void setString(int idx, int maxlen, string value)
        {
            try
            {
                if (value == null) valc[idx] = "";
                else
                {
                    int i = value.IndexOf('\0');
                    if (i >= 0) value = value.Substring(0, i);
                    if (value.Length > maxlen) { valc[idx] = value.Substring(0, maxlen); }
                    else valc[idx] = value;
                }
            }
            catch (Exception) { }
        }
        public void setInt(int idx, int? value) { try { valc[idx] = value; } catch (Exception) { } }
        public void setDouble(int idx, double? value) { try { valc[idx] = value; } catch (Exception) { } }
        public void setDateTime(int idx, DateTime? value) { try { valc[idx] = value; } catch (Exception) { } }
        public void setGuid(int idx, Guid? value) { try { valc[idx] = value; } catch (Exception) { } }
        public void setObject(int idx, object ob) { try { valc[idx] = ob; } catch (Exception) { } }


        public string Filter;
        public string FormatValue;
        public string Order;


        public void OpenRs()
        {
            if (isConnection)
            {
                if (rs == null)
                    rs = new OrmRsOracle();
                rs.Init(this);
                rs.OpenRs();
            }
        }


        public string GetTableName()
        {
            return TableName;
        }

        public void Format(string value)
        {
            FormatValue = value;
        }
        public bool IsEOF()
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    return rs.IsEOF();
                }
                else return false;
            }
            else return false;
        }
        public void MoveNext()
        {
            if (isConnection)
            {
                if (rs!=null)
                {
                    rs.MoveNext();
                }
            }
        }

        public int? GetCount()
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    return rs.GetCount();
                }
                else return 0;
            }
            return null;
        }
        public void Close()
        {
            if (isConnection)
            {
                if (rs != null) rs.Clear();
            }
        }


        public int? GetNextId(string sequenceName)
        {
            if (isConnection)
            {
                if (rs == null) rs = new OrmRsOracle();
                return rs.GetNextId(sequenceName);
            }
            return null;
            
        }

        public void Dispose()
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    rs.Dispose();
                }
            }
        }

        public void New()
        {
            if (isConnection)
            {
                if (rs == null) rs = new OrmRsOracle();
                rs.Init(this);
                if (rs != null) rs.New();
            }
        }

        public int? SaveUpdate()
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    if (!rs.isNew)
                        return rs.UpdateRecord();
                }
            }
            return null;
        }

        public int? SaveCreateNew()
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    if (rs.isNew)
                        return rs.InsertRecord();
                }
            }
            return null;
        }

        public int? Save()
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    if (rs.isNew)
                        return rs.InsertRecord();
                    else return rs.UpdateRecord();
                }
            }
            return null;
        }

        public void SetUpdateMode()
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    rs.isNew = false;
                }
            }
        }

        public bool SaveBath(List<Yyy> ListY)
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    if (rs.isNew)
                        return rs.InsertBulkRecords(ListY);
                }
            }
            return false;
        }

        public bool SaveBath(List<Yyy> ListY1, List<Yyy> ListY2, OracleParameter[] oracleParameter)
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    if (rs.isNew)
                        return rs.InsertBulkRecords(ListY1,ListY2, oracleParameter);
                }
            }
            return false;
        }

        public bool Delete()
        {
            if (isConnection)
            {
                if (rs != null)
                {
                    return rs.DeleteRecord();
                }
            }
            return false;
        }

        public bool Fetch(int ID)
        {
            if (isConnection)
            {
                Filter = string.Format("ID = {0}", ID);
                OpenRs();
                if (GetCount() > 0) return true;
                else return false;
            }
            else return false;
        }

        public bool Fetch(string sql)
        {
            if (isConnection)
            {
                Filter = sql;
                OpenRs();
                if (GetCount() > 0) return true;
                else return false;
            }
            else return false;
        }

       public void CopyDataFrom(Yyy y)
        {
            if (isConnection)
            {
                
                New();
                rs.yyy.valc = y.valc;
            }
        }
    }
}
