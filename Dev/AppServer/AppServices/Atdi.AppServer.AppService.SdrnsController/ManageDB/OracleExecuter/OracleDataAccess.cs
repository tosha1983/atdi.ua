using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oracle.DataAccess.Client;
using System.Data.Common;


namespace Atdi.Oracle.DataAccess
{
    public class OracleDataAccess 
    {
        public OrmRsOracle rs;
        public List<OracleParameter> getAllFields = new List<OracleParameter>();
        public static bool isConnection { get { if (connection != null) { return connection.State == ConnectionState.Open ? true : false; } else return false; } }
        public static DbConnection connection = null;
        public static string connectionStringValue { get; set; }

     
        /// <summary>
        /// Dispose connect
        /// </summary>
        public void CloseConnection()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }

        public string GetConnectionString()
        {
            return connectionStringValue;
        }


        /// <summary>
        /// Open new connection
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public bool OpenConnection(string connectionString)
        {
            bool isSucess = false;
            connectionStringValue = connectionString;
            try
            {
                if (connection == null)
                {
                    if (isConnection == false)
                    {
                        connection = NewConnection(connectionString);
                        //isConnection = true;
                        isSucess = true;
                    }
                }
            }
            catch (Exception e)
            {
                isSucess = false;
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return isSucess;
        }

        public DbConnection NewConnection(string connectionString)
        {
            DbConnection newConnection = null;
            try
            {
                        DbProviderFactory factory = DbProviderFactories.GetFactory("Oracle.DataAccess.Client");
                        newConnection = factory.CreateConnection();
                        newConnection.ConnectionString = connectionStringValue;
                        newConnection.Open();
                        return newConnection;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
            return newConnection;
        }

        /// <summary>
        /// Insert one record
        /// </summary>
        /// <param name="OraParametr"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public int? InsertRecord(List<OracleParameter> OraParametr, string TableName, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            int? ID = null;
            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
            try
            {
                    if (connection == null) connection = NewConnection(GetConnectionString());
                    if (((dbConnection != null) && (dbConnection.State == ConnectionState.Open)) || ((connection != null) && (connection.State == ConnectionState.Open)))
                    {
                        DbCommand command = dbConnection != null ? dbConnection.CreateCommand() : connection.CreateCommand();
                        if (dbTransaction != null) command.Transaction = dbTransaction;
                        command.Parameters.Clear();
                        foreach (OracleParameter p in OraParametr)
                            command.Parameters.Add(p);
                        command.Parameters.Add(new OracleParameter
                        {
                            ParameterName = ":id",
                            OracleDbType = OracleDbType.Int32,
                            Direction = ParameterDirection.Output
                        });

                        string AllColumns = "";
                        string AllValues = "";

                        string vby = "";

                        foreach (OracleParameter p in OraParametr)
                        {
                            AllValues += p.ParameterName + ",";
                            AllColumns += p.SourceColumn + ",";
                            vby += p.Value != null ? p.Value.ToString() : "null" + ",";
                        }
                        if (AllValues.Length > 0) AllValues = AllValues.Remove(AllValues.Length - 1, 1);
                        if (AllColumns.Length > 0) AllColumns = AllColumns.Remove(AllColumns.Length - 1, 1);

                        command.CommandText = string.Format("INSERT INTO {0}({1}) VALUES ({2}) RETURNING ID INTO :id", TableName, AllColumns, AllValues);
                        command.ExecuteNonQuery();
                        object o = command.Parameters[":id"].Value;
                        if ((o != null) && (o != DBNull.Value))
                        {
                            char[] xx = o.ToString().ToCharArray();
                            bool isOk = true;
                            foreach (char c in xx)
                            {
                                if (!char.IsDigit(c)) { isOk = false; break; }
                            }
                            if (isOk)
                                ID = int.Parse(o.ToString());
                            else
                            {
                                System.Console.WriteLine("Error value: " + o);
                            }
                        }
                    }
            }
            catch (Exception e)
            {
                    if (e is OracleException)
                    {
                        if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                        {
                            CloseConnection();
                            OpenConnection(GetConnectionString());
                        }
                    }
                    System.Console.WriteLine("Neither record was written to database: "+e.ToString());
            }
            });
            tsk.Start();
            tsk.Join();
            return ID;
        }
        /// <summary>
        /// Update one record
        /// </summary>
        /// <param name="OraParametr"></param>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int? UpdateRecord(List<OracleParameter> OraParametr, string TableName, int ID, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            int? ID_VALUE = null;
            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
                try
                {
                    if (connection == null) connection = NewConnection(GetConnectionString());
                    if (((dbConnection!=null) && (dbConnection.State == ConnectionState.Open)) || ((connection != null) && (connection.State == ConnectionState.Open)))
                    {
                        DbCommand command = dbConnection != null ? dbConnection.CreateCommand() : connection.CreateCommand();
                        if (dbTransaction != null) command.Transaction = dbTransaction;
                        command.Parameters.Clear();
                        foreach (OracleParameter p in OraParametr)
                            command.Parameters.Add(p);

                        string AllColumns = "";

                        foreach (OracleParameter p in OraParametr)
                        {
                            string val_set = p.SourceColumn + "=" + p.ParameterName;
                            AllColumns += val_set + ",";
                        }
                        if (AllColumns.Length > 0) AllColumns = AllColumns.Remove(AllColumns.Length - 1, 1);
                        command.CommandText = string.Format("UPDATE {0} SET {1} WHERE ID = {2}", TableName, AllColumns, ID);
                        command.ExecuteNonQuery();
                        //transaction.Commit();
                        ID_VALUE = ID;
                        command.Dispose();
                    }
                }
                catch (Exception e)
                {
                    if (e is OracleException)
                    {
                        if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                        {
                            CloseConnection();
                            OpenConnection(GetConnectionString());
                        }
                    }
                    System.Console.WriteLine(e.ToString());
                }
            
            });
            tsk.Start();
            tsk.Join();
            return ID_VALUE;
        }

        /// <summary>
        /// Update record
        /// </summary>
        /// <param name="OraParametr"></param>
        /// <param name="TableName"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool UpdateRecord(List<OracleParameter> OraParametr, string TableName, string sql, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            bool isSuccess = false;
            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
                try
                {
                    if (connection == null) connection = NewConnection(GetConnectionString());
                    if (((dbConnection != null) && (dbConnection.State == ConnectionState.Open)) || ((connection != null) && (connection.State == ConnectionState.Open)))
                    {
                        DbCommand command = dbConnection != null ? dbConnection.CreateCommand() : connection.CreateCommand();
                        if (dbTransaction != null) command.Transaction = dbTransaction;
                        command.Parameters.Clear();
                        foreach (OracleParameter p in OraParametr)
                            command.Parameters.Add(p);

                        string AllColumns = "";

                        foreach (OracleParameter p in OraParametr)
                        {
                            string val_set = p.SourceColumn + "=" + p.ParameterName;
                            AllColumns += val_set + ",";
                        }
                        if (AllColumns.Length > 0) AllColumns = AllColumns.Remove(AllColumns.Length - 1, 1);
                        command.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}", TableName, AllColumns, sql);
                        command.ExecuteNonQuery();
                        isSuccess = true;
                        command.Dispose();
                    }
                }
                catch (Exception e)
                {
                    if (e is OracleException)
                    {
                        if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                        {
                            CloseConnection();
                            OpenConnection(GetConnectionString());
                        }
                    }
                    System.Console.WriteLine(e.ToString());
                }
            });
            tsk.Start();
            tsk.Join();
            return isSuccess;
        }
        /// <summary>
        /// Delete one record
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool DeleteRecord(string TableName, int ID, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            bool isSuccess = false;
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (((dbConnection != null) && (dbConnection.State == ConnectionState.Open)) || ((connection != null) && (connection.State == ConnectionState.Open)))
                {
                    DbCommand command = dbConnection != null ? dbConnection.CreateCommand() : connection.CreateCommand();
                    if (dbTransaction != null) command.Transaction = dbTransaction;
                    command.CommandText = string.Format("DELETE {0} WHERE ID = {1}", TableName, ID);
                    command.ExecuteNonQuery();
                    isSuccess = true;
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return isSuccess;
        }
        /// <summary>
        /// Get Max ID 
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public int? GetMaxId(string TableName)
        {
            int? Id = null;
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (connection.State == ConnectionState.Open)
                {
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = string.Format("SELECT MAX(ID) FROM {0}", TableName);
                    object o = command.ExecuteScalar();
                    if (o != null)
                    {
                        if (o != DBNull.Value)
                        {
                            Id = Int32.Parse(o.ToString());
                        }
                        else Id = 1;
                    }
                    else
                    {
                        Id = 1;
                    }
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return Id;
        }

        public int? GetMaxId(string TableName, string NameField)
        {
            int? Id = null;
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (connection.State == ConnectionState.Open)
                {
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = string.Format("SELECT MAX({0}) FROM {1}", NameField, TableName);
                    object o = command.ExecuteScalar();
                    if (o != null)
                    {
                        if (o != DBNull.Value)
                        {
                            Id = Int32.Parse(o.ToString());
                        }
                        else Id = 1;
                    }
                    else
                    {
                        Id = 1;
                    }
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return Id;
        }

        public object GetValueFromField(string FieldName, string TableName, int ID)
        {
            object value = null;
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (connection.State == ConnectionState.Open)
                {
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = string.Format("SELECT {0} FROM {1} WHERE ID={2}", FieldName, TableName, ID);
                    value = command.ExecuteScalar();
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return value;
        }

        public int? GetNextId(string sequenceName)
        {
            int? Id = null;
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (connection.State == ConnectionState.Open)
                {
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = string.Format("SELECT {0}.nextval from dual", sequenceName);
                    object o = command.ExecuteScalar();
                    if (o != null)
                    {
                        if (o != DBNull.Value)
                        {
                            Id = Int32.Parse(o.ToString());
                        }
                        else Id = 1;
                    }
                    else
                    {
                        Id = 1;
                    }
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return Id;
        }
        /// <summary>
        /// Get Value from one record
        /// </summary>
        /// <param name="Columns"></param>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public object[] GetValues(List<string> Columns, string TableName, int ID, string OrderBy)
        {
            List<object> LObj = new List<object>();
            DbDataReader reader = null;
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (connection.State == ConnectionState.Open)
                {
                    DbCommand command = connection.CreateCommand();
                    string AllColumns = "";
                    foreach (string p in Columns)
                        AllColumns += p + ",";
                    if (AllColumns.Length > 0) AllColumns = AllColumns.Remove(AllColumns.Length - 1, 1);
                    if (string.IsNullOrEmpty(OrderBy))
                        command.CommandText = string.Format("SELECT {0} FROM {1} WHERE ID={2}", AllColumns, TableName, ID);
                    else
                        command.CommandText = string.Format("SELECT {0} FROM {1} WHERE ID={2} ORDER BY {3}", AllColumns, TableName, ID, OrderBy);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        foreach (string p in Columns)
                        {
                            int ordinal = reader.GetOrdinal(p.Replace("\"", ""));
                            LObj.Add(reader[ordinal]);
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return LObj.ToArray();
        }

        public DbDataReader GetValuesSql(string sql)
        {
            List<object[]> LObj = new List<object[]>();
            DbDataReader reader = null;
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (connection.State == ConnectionState.Open)
                {
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = sql;
                    reader = command.ExecuteReader();
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return reader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Columns"></param>
        /// <param name="TableName"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public OracleParameter[][] GetValues(List<string> Columns, string TableName, string sql, string OrderBy)
        {
            List<OracleParameter[]> LObj = new List<OracleParameter[]>();
            DbDataReader reader = null;
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (connection.State == ConnectionState.Open)
                {
                    DbCommand command = connection.CreateCommand();
                    string AllColumns = "";
                    foreach (string p in Columns)
                        AllColumns += p + ",";

                    if (AllColumns.Length > 0) AllColumns = AllColumns.Remove(AllColumns.Length - 1, 1);
                    if (string.IsNullOrEmpty(OrderBy))
                        command.CommandText = string.Format("SELECT {0} FROM {1} WHERE {2}", AllColumns, TableName, sql);
                    else
                        command.CommandText = string.Format("SELECT {0} FROM {1} WHERE {2} ORDER BY {3}", AllColumns, TableName, sql, OrderBy);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        List<OracleParameter> Obj = new List<OracleParameter>();
                        foreach (string p in Columns)
                        {
                            int ordinal = reader.GetOrdinal(p.Replace("\"", ""));
                            Type tpField = reader.GetFieldType(ordinal);
                            string DataTypeName = reader.GetDataTypeName(ordinal);
                            OracleDbType oracleDbType;
                            Enum.TryParse(DataTypeName, out oracleDbType);
                            Obj.Add(new OracleParameter()
                            {
                                SourceColumn = p,
                                ParameterName = ":" + p,
                                OracleDbType = oracleDbType,
                                Direction = ParameterDirection.Input,
                                Value = reader[ordinal]
                            });
                        }
                        LObj.Add(Obj.ToArray());
                    }
                    reader.Close();
                    reader.Dispose();
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return LObj.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OraParametr_Level1"></param>
        /// <param name="TableName_Level1"></param>
        /// <param name="Cnt"></param>
        /// <returns></returns>
        public bool InsertBulkRecords(List<OracleParameter> OraParametr_Level1, string TableName_Level1, int Cnt, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            bool isSuccess = false;
            const int CountInsertRecordInOneBlock = 800; 
            int Temp_Count = 0;
            List<string> SQL_PART = new List<string>();
            string SQL = ""; string SQL_INSERT = "";
            string allSql = "";
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (((dbConnection != null) && (dbConnection.State == ConnectionState.Open)) || ((connection != null) && (connection.State == ConnectionState.Open)))
                {
                    DbCommand command = dbConnection != null ? dbConnection.CreateCommand() : connection.CreateCommand();
                    if (dbTransaction != null) command.Transaction = dbTransaction;
                    command.Parameters.Clear();
                    foreach (OracleParameter p in OraParametr_Level1)
                        command.Parameters.Add(p);
                    string AllColumns_level1 = ""; string AllValues_level1 = "";
                    List<OracleParameter> DelObj_Level1 = new List<OracleParameter>();
                    for (int z = 1; z <= Cnt; z++)
                    {
                        AllColumns_level1 = ""; AllValues_level1 = "";
                        foreach (OracleParameter p in OraParametr_Level1.ToList().FindAll(r => r.ParameterName.EndsWith("_" + z.ToString() + "\"")))
                        {
                            if (p.OracleDbType != OracleDbType.Object)
                                AllValues_level1 += p.ParameterName + ",";
                            else
                            {
                                AllValues_level1 += p.Value.ToString() + ",";
                                DelObj_Level1.Add(p);
                            }

                            AllColumns_level1 += p.SourceColumn + ",";
                        }
                        while (DelObj_Level1.Count > 0)
                        {
                            command.Parameters.Remove(DelObj_Level1[DelObj_Level1.Count - 1]);
                            DelObj_Level1.Remove(DelObj_Level1[DelObj_Level1.Count - 1]);
                        }
                        if (AllValues_level1.Length > 0) AllValues_level1 = AllValues_level1.Remove(AllValues_level1.Length - 1, 1);
                        if (AllColumns_level1.Length > 0) AllColumns_level1 = AllColumns_level1.Remove(AllColumns_level1.Length - 1, 1);
                        allSql += string.Format(" INTO {0}({1}) VALUES ({2}) ", TableName_Level1, AllColumns_level1, AllValues_level1) + Environment.NewLine;

                        if (Temp_Count == CountInsertRecordInOneBlock)
                        {
                            if (allSql.Length > 0)
                            {
                                allSql += "SELECT 1 FROM DUAL" + Environment.NewLine;
                                allSql += "COMMIT";
                                allSql = allSql.Insert(0, "INSERT ALL" + Environment.NewLine);
                                command.CommandText = allSql;
                                command.ExecuteNonQuery();
                                allSql = "";
                                Temp_Count = 0;
                            }
                        }
                        Temp_Count++;
                    }
                    if (allSql.Length > 0)
                    {
                        allSql += "SELECT 1 FROM DUAL" + Environment.NewLine;
                        allSql += "COMMIT";
                        allSql = allSql.Insert(0, "INSERT ALL" + Environment.NewLine);
                        Temp_Count = 0;
                    }
                    command.CommandText = allSql;
                    command.ExecuteNonQuery();
                    isSuccess = true;
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return true;
        }

        public bool InsertBulkRecords(List<OracleParameter> OraParametr_Level1, string TableName_Level1, int Cnt1, List<OracleParameter> OraParametr_Level2, string TableName_Level2, int Cnt2, OracleParameter[] oracleParameterRefId, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            bool isSuccess = false;
            const int CountInsertRecordInOneBlock = 800;
            int Temp_Count = 0;
            List<string> SQL_PART = new List<string>();
            string SQL = ""; string SQL_INSERT = "";
            string allSql = "";//"INSERT ALL ";
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (((dbConnection != null) && (dbConnection.State == ConnectionState.Open)) || ((connection != null) && (connection.State == ConnectionState.Open)))
                {
                    DbCommand command = dbConnection != null ? dbConnection.CreateCommand() : connection.CreateCommand();
                    if (dbTransaction != null) command.Transaction = dbTransaction;
                    command.Parameters.Clear();
                    foreach (OracleParameter p in OraParametr_Level1)
                        command.Parameters.Add(p);
                    string AllColumns_level1 = ""; string AllValues_level1 = "";
                    List<OracleParameter> DelObj_Level1 = new List<OracleParameter>();
                    for (int z = 1; z <= Cnt1; z++)
                    {
                        AllColumns_level1 = ""; AllValues_level1 = "";
                        foreach (OracleParameter p in OraParametr_Level1.ToList().FindAll(r => r.ParameterName.EndsWith("_" + z.ToString() + "\"")))
                        {
                            if (p.OracleDbType != OracleDbType.Object)
                                AllValues_level1 += p.ParameterName + ",";
                            else
                            {
                                AllValues_level1 += p.Value.ToString() + ",";
                                DelObj_Level1.Add(p);
                            }

                            AllColumns_level1 += p.SourceColumn + ",";
                        }
                        while (DelObj_Level1.Count > 0)
                        {
                            command.Parameters.Remove(DelObj_Level1[DelObj_Level1.Count - 1]);
                            DelObj_Level1.Remove(DelObj_Level1[DelObj_Level1.Count - 1]);
                        }
                        if (AllValues_level1.Length > 0) AllValues_level1 = AllValues_level1.Remove(AllValues_level1.Length - 1, 1);
                        if (AllColumns_level1.Length > 0) AllColumns_level1 = AllColumns_level1.Remove(AllColumns_level1.Length - 1, 1);
                        allSql += string.Format(" INTO {0}({1}) VALUES ({2}) ", TableName_Level1, AllColumns_level1, AllValues_level1) + Environment.NewLine;

                    }

                    string AllColumns_level2 = ""; string AllValues_level2 = "";
                    List<OracleParameter> DelObj_Level2 = new List<OracleParameter>();
                    for (int z = 1; z <= Cnt2; z++)
                    {
                        AllColumns_level2 = ""; AllValues_level2 = "";
                        foreach (OracleParameter p in OraParametr_Level2.ToList().FindAll(r => r.ParameterName.EndsWith("_" + z.ToString() + "\"")))
                        {
                            if (p.OracleDbType != OracleDbType.Object)
                                AllValues_level2 += p.ParameterName + ",";
                            else
                            {
                                AllValues_level2 += p.Value.ToString() + ",";
                                DelObj_Level2.Add(p);
                            }

                            AllColumns_level2 += p.SourceColumn + ",";
                        }
                        while (DelObj_Level2.Count > 0)
                        {
                            command.Parameters.Remove(DelObj_Level2[DelObj_Level2.Count - 1]);
                            DelObj_Level2.Remove(DelObj_Level2[DelObj_Level2.Count - 1]);
                        }
                        if (AllValues_level2.Length > 0) AllValues_level2 = AllValues_level2.Remove(AllValues_level2.Length - 1, 1);
                        if (AllColumns_level2.Length > 0) AllColumns_level2 = AllColumns_level2.Remove(AllColumns_level2.Length - 1, 1);
                        allSql += string.Format(" INTO {0}({1}) VALUES ({2}) ", TableName_Level2, AllColumns_level2, AllValues_level2) + Environment.NewLine;

                    }
                    if (allSql.Length > 0)
                    {
                        allSql += "SELECT 1 FROM DUAL" + Environment.NewLine;
                        allSql += "COMMIT";
                        allSql = allSql.Insert(0, "INSERT ALL" + Environment.NewLine);
                    }
                    command.CommandText = allSql;
                    command.ExecuteNonQuery();
                    isSuccess = true;
                    command.Dispose();
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return true;
        }

        /// <summary>
        /// Batch insert process for two tables joined
        /// </summary>
        /// <param name="OraParametr_Level1"></param>
        /// <param name="TableName_Level1"></param>
        /// <param name="OraParametr_Level2"></param>
        /// <param name="TableName_Level2"></param>
        /// <returns></returns>
        public bool InsertBulkRecords(List<OracleParameter> OraParametr_Level1, string TableName_Level1, List<OracleParameter> OraParametr_Level2_Const, List<OracleParameter[]> OraParametr_Level2_records, string TableName_Level2, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            bool isSuccess = false;
            string allSql = "INSERT ALL ";
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (((dbConnection != null) && (dbConnection.State == ConnectionState.Open)) || ((connection != null) && (connection.State == ConnectionState.Open)))
                {
                    DbCommand command = dbConnection != null ? dbConnection.CreateCommand() : connection.CreateCommand();
                    if (dbTransaction != null) command.Transaction = dbTransaction;
                    command.Parameters.Clear();
                    foreach (OracleParameter p in OraParametr_Level1)
                        command.Parameters.Add(p);
                    string AllColumns_level1 = ""; string AllValues_level1 = "";
                    List<OracleParameter> DelObj_Level1 = new List<OracleParameter>();
                    foreach (OracleParameter p in OraParametr_Level1)
                    {
                        if (p.OracleDbType != OracleDbType.Object)
                            AllValues_level1 += p.ParameterName + ",";
                        else
                        {
                            AllValues_level1 += p.Value.ToString() + ",";
                            DelObj_Level1.Add(p);
                        }

                        AllColumns_level1 += p.SourceColumn + ",";
                    }
                    while (DelObj_Level1.Count > 0)
                    {
                        command.Parameters.Remove(DelObj_Level1[DelObj_Level1.Count - 1]);
                        DelObj_Level1.Remove(DelObj_Level1[DelObj_Level1.Count - 1]);
                    }
                    if (AllValues_level1.Length > 0) AllValues_level1 = AllValues_level1.Remove(AllValues_level1.Length - 1, 1);
                    if (AllColumns_level1.Length > 0) AllColumns_level1 = AllColumns_level1.Remove(AllColumns_level1.Length - 1, 1);
                    allSql += string.Format(" INTO {0}({1}) VALUES ({2}) ", TableName_Level1, AllColumns_level1, AllValues_level1);

                    foreach (OracleParameter p in OraParametr_Level2_Const)
                        command.Parameters.Add(p);

                    string AllColumns_level2 = ""; string AllValues_level2 = "";
                    List<OracleParameter> DelObj_Level2 = new List<OracleParameter>();
                    foreach (OracleParameter p in OraParametr_Level2_Const)
                    {
                        if (p.OracleDbType != OracleDbType.Object)
                            AllValues_level2 += p.ParameterName + ",";
                        else
                        {
                            AllValues_level2 += p.Value.ToString() + ",";
                            DelObj_Level2.Add(p);
                        }

                        AllColumns_level2 += p.SourceColumn + ",";
                    }
                    while (DelObj_Level2.Count > 0)
                    {
                        command.Parameters.Remove(DelObj_Level2[DelObj_Level2.Count - 1]);
                        DelObj_Level2.Remove(DelObj_Level2[DelObj_Level2.Count - 1]);
                    }

                    foreach (OracleParameter[] p in OraParametr_Level2_records)
                        command.Parameters.AddRange(p);

                    foreach (OracleParameter[] p in OraParametr_Level2_records)
                    {
                        string AllValues_level2_records = AllValues_level2;
                        string AllColumns_level2_records = AllColumns_level2;
                        foreach (OracleParameter sp in p)
                        {
                            if (sp.OracleDbType != OracleDbType.Object)
                            {
                                AllValues_level2_records += sp.ParameterName + ",";
                                AllColumns_level2_records += sp.SourceColumn + ",";
                            }
                        }

                        if (AllValues_level2_records.Length > 0) AllValues_level2_records = AllValues_level2_records.Remove(AllValues_level2_records.Length - 1, 1);
                        if (AllColumns_level2_records.Length > 0) AllColumns_level2_records = AllColumns_level2_records.Remove(AllColumns_level2_records.Length - 1, 1);
                        allSql += string.Format(" INTO {0}({1}) VALUES ({2}) ", TableName_Level2, AllColumns_level2_records, AllValues_level2_records);
                    }
                    allSql += " SELECT * FROM dual";
                    command.CommandText = allSql;
                    command.ExecuteNonQuery();
                    isSuccess = true;
                }

            }
            catch (Exception e)
            {
                isSuccess = false;
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return true;
        }

        /// <summary>
        /// Batch insert process for two tables joined
        /// </summary>
        /// <param name="OraParametr_Level1"></param>
        /// <param name="TableName_Level1"></param>
        /// <param name="OraParametr_Level2"></param>
        /// <param name="TableName_Level2"></param>
        /// <returns></returns>
        public bool InsertBulkRecords(List<OracleParameter> OraParametr_Level1, string TableName_Level1, List<OracleParameter> OraParametr_Level2_Const, List<OracleParameter[]> OraParametr_Level2_records, string TableName_Level2, List<OracleParameter> OraParametr_Level3_Const, List<OracleParameter[]> OraParametr_Level3_records, string TableName_Level3, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            bool isSuccess = false;
            string allSql = "INSERT ALL ";
            try
            {
                if (connection == null) connection = NewConnection(GetConnectionString());
                if (((dbConnection != null) && (dbConnection.State == ConnectionState.Open)) || ((connection != null) && (connection.State == ConnectionState.Open)))
                {
                    DbCommand command = dbConnection != null ? dbConnection.CreateCommand() : connection.CreateCommand();
                    if (dbTransaction != null) command.Transaction = dbTransaction;
                    command.Parameters.Clear();
                    foreach (OracleParameter p in OraParametr_Level1)
                        command.Parameters.Add(p);
                    string AllColumns_level1 = ""; string AllValues_level1 = "";
                    List<OracleParameter> DelObj_Level1 = new List<OracleParameter>();
                    foreach (OracleParameter p in OraParametr_Level1)
                    {
                        if (p.OracleDbType != OracleDbType.Object)
                            AllValues_level1 += p.ParameterName + ",";
                        else
                        {
                            AllValues_level1 += p.Value.ToString() + ",";
                            DelObj_Level1.Add(p);
                        }

                        AllColumns_level1 += p.SourceColumn + ",";
                    }
                    while (DelObj_Level1.Count > 0)
                    {
                        command.Parameters.Remove(DelObj_Level1[DelObj_Level1.Count - 1]);
                        DelObj_Level1.Remove(DelObj_Level1[DelObj_Level1.Count - 1]);
                    }
                    if (AllValues_level1.Length > 0) AllValues_level1 = AllValues_level1.Remove(AllValues_level1.Length - 1, 1);
                    if (AllColumns_level1.Length > 0) AllColumns_level1 = AllColumns_level1.Remove(AllColumns_level1.Length - 1, 1);
                    allSql += string.Format(" INTO {0}({1}) VALUES ({2}) ", TableName_Level1, AllColumns_level1, AllValues_level1);
                    foreach (OracleParameter p in OraParametr_Level2_Const)
                        command.Parameters.Add(p);

                    string AllColumns_level2 = ""; string AllValues_level2 = "";
                    List<OracleParameter> DelObj_Level2 = new List<OracleParameter>();
                    foreach (OracleParameter p in OraParametr_Level2_Const)
                    {
                        if (p.OracleDbType != OracleDbType.Object)
                            AllValues_level2 += p.ParameterName + ",";
                        else
                        {
                            AllValues_level2 += p.Value.ToString() + ",";
                            DelObj_Level2.Add(p);
                        }

                        AllColumns_level2 += p.SourceColumn + ",";
                    }
                    while (DelObj_Level2.Count > 0)
                    {
                        command.Parameters.Remove(DelObj_Level2[DelObj_Level2.Count - 1]);
                        DelObj_Level2.Remove(DelObj_Level2[DelObj_Level2.Count - 1]);
                    }

                    foreach (OracleParameter[] p in OraParametr_Level2_records)
                        command.Parameters.AddRange(p);

                    foreach (OracleParameter[] p in OraParametr_Level2_records)
                    {
                        string AllValues_level2_records = AllValues_level2;
                        string AllColumns_level2_records = AllColumns_level2;
                        foreach (OracleParameter sp in p)
                        {
                            if (sp.OracleDbType != OracleDbType.Object)
                            {
                                AllValues_level2_records += sp.ParameterName + ",";
                                AllColumns_level2_records += sp.SourceColumn + ",";
                            }
                        }

                        if (AllValues_level2_records.Length > 0) AllValues_level2_records = AllValues_level2_records.Remove(AllValues_level2_records.Length - 1, 1);
                        if (AllColumns_level2_records.Length > 0) AllColumns_level2_records = AllColumns_level2_records.Remove(AllColumns_level2_records.Length - 1, 1);
                        allSql += string.Format(" INTO {0}({1}) VALUES ({2}) ", TableName_Level2, AllColumns_level2_records, AllValues_level2_records);
                    }


                    foreach (OracleParameter p in OraParametr_Level3_Const)
                        command.Parameters.Add(p);

                    string AllColumns_level3 = ""; string AllValues_level3 = "";
                    List<OracleParameter> DelObj_Level3 = new List<OracleParameter>();
                    foreach (OracleParameter p in OraParametr_Level3_Const)
                    {
                        if (p.OracleDbType != OracleDbType.Object)
                            AllValues_level3 += p.ParameterName + ",";
                        else
                        {
                            AllValues_level3 += p.Value.ToString() + ",";
                            DelObj_Level3.Add(p);
                        }

                        AllColumns_level3 += p.SourceColumn + ",";
                    }
                    while (DelObj_Level3.Count > 0)
                    {
                        command.Parameters.Remove(DelObj_Level3[DelObj_Level3.Count - 1]);
                        DelObj_Level3.Remove(DelObj_Level3[DelObj_Level3.Count - 1]);
                    }

                    foreach (OracleParameter[] p in OraParametr_Level3_records)
                        command.Parameters.AddRange(p);

                    foreach (OracleParameter[] p in OraParametr_Level3_records)
                    {
                        string AllValues_level3_records = AllValues_level3;
                        string AllColumns_level3_records = AllColumns_level3;
                        foreach (OracleParameter sp in p)
                        {
                            if (sp.OracleDbType != OracleDbType.Object)
                            {
                                AllValues_level3_records += sp.ParameterName + ",";
                                AllColumns_level3_records += sp.SourceColumn + ",";
                            }
                        }

                        if (AllValues_level3_records.Length > 0) AllValues_level3_records = AllValues_level3_records.Remove(AllValues_level3_records.Length - 1, 1);
                        if (AllColumns_level3_records.Length > 0) AllColumns_level3_records = AllColumns_level3_records.Remove(AllColumns_level3_records.Length - 1, 1);
                        allSql += string.Format(" INTO {0}({1}) VALUES ({2}) ", TableName_Level3, AllColumns_level3_records, AllValues_level3_records);
                    }


                    allSql += " SELECT * FROM dual";
                    command.CommandText = allSql;
                    command.ExecuteNonQuery();
                    isSuccess = true;
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                if (e is OracleException)
                {
                    if (((e as OracleException).Number == 3114) || ((e as OracleException).Number == 3135))
                    {
                        CloseConnection();
                        OpenConnection(GetConnectionString());
                    }
                }
                System.Console.WriteLine(e.ToString());
            }
            return true;
        }

    }


}

