using System;
using Oracle.DataAccess.Client;
using System.Collections.Generic;
using System.Data.Common;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    public class Yyy : OracleDataAccess
    {
        public object[][] allvalc;
        public object[] valc;
        public string TableName = "";
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
        public byte[] getBlob(int idx)
        {
            byte[] val = null;
            try
            {
                if ((valc[idx] != null) && (valc[idx] != DBNull.Value)) val = valc[idx] as byte[];
            }
            catch (Exception) { return null; }
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
        public void setBlob(int idx, byte[] value) { try { valc[idx] = value; } catch (Exception) { } }
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
            try
            {
                if (isConnection)
                {
                    if (rs == null)
                        rs = new OrmRsOracle();
                    rs.Init(this);
                    rs.OpenRs();
                }
            }
            catch (Exception e) { System.Console.WriteLine(e.ToString()); }
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
            try
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
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }
        }
        public void MoveNext()
        {
            try
            {
                if (isConnection)
                {
                    if (rs != null)
                    {
                        rs.MoveNext();
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }

        public int? GetCount()
        {
            try
            {
                if (isConnection)
                {
                    if (rs != null)
                    {
                        return rs.GetCount();
                    }
                    else return null;
                }
                return null;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return null;
            }
        }
        public void Close()
        {
            try
            {
                if (isConnection)
                {
                    if (rs != null) rs.Clear();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }


        public int? GetNextId(string sequenceName)
        {
            try
            {
                if (rs == null) rs = new OrmRsOracle();
                return rs.GetNextId(sequenceName);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return null;
            }
        }

        public void Dispose()
        {
            try
            {
                if (isConnection)
                {
                    if (rs != null)
                    {
                        rs.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }

        public void New()
        {
            try
            {
                if (isConnection)
                {
                    if (rs == null) rs = new OrmRsOracle();
                    rs.Init(this);
                    if (rs != null) rs.New();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }

        public int? SaveUpdate(DbConnection dbConnection, DbTransaction dbTransaction)
        {
            try
            {
                if (rs != null)
                {
                    if (!rs.isNew)
                        return rs.UpdateRecord(dbConnection, dbTransaction);
                }
                return null;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return null;
            }
        }

        public int? SaveCreateNew(DbConnection dbConnection, DbTransaction dbTransaction)
        {
            try
            {
                if (rs != null)
                {
                    if (rs.isNew)
                        return rs.InsertRecord(dbConnection, dbTransaction);
                }
                return null;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return null;
            }
        }

        public int? Save(DbConnection dbConnection, DbTransaction dbTransaction)
        {
            try
            {
                if (rs != null)
                {
                    if (rs.isNew)
                        return rs.InsertRecord(dbConnection, dbTransaction);
                    else return rs.UpdateRecord(dbConnection, dbTransaction);
                }
                return null;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return null;
            }
        }

        public void SetUpdateMode()
        {
            try
            {
                if (rs != null)
                {
                    rs.isNew = false;
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }

        public bool SaveBath(List<Yyy> ListY, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            try
            {
                if (dbConnection != null)
                {
                    if (rs != null)
                    {
                        if (rs.isNew)
                            return rs.InsertBulkRecords(ListY, dbConnection, dbTransaction);
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }
        }

        public bool SaveBath(List<Yyy> ListY1, List<Yyy> ListY2, OracleParameter[] oracleParameter, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            try
            {
                if (dbConnection != null)
                {
                    if (rs != null)
                    {
                        if (rs.isNew)
                            return rs.InsertBulkRecords(ListY1, ListY2, oracleParameter, dbConnection, dbTransaction);
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }
        }

        public bool Delete(DbConnection dbConnection, DbTransaction dbTransaction)
        {
            try
            {
                if (rs != null)
                {
                    return rs.DeleteRecord(dbConnection, dbTransaction);
                }
                return false;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }
        }

        public bool Fetch(int ID)
        {
            try
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
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }
        }

        public bool Fetch(string sql)
        {
            try
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
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return false;
            }
        }

        public void CopyDataFrom(Yyy y)
        {
            try
            {
                if (isConnection)
                {
                    New();
                    rs.yyy.valc = y.valc;
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }
    }
}
