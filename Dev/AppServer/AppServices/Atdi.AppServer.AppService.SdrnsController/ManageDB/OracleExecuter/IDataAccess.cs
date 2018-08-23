using System.Collections.Generic;
using System.Data.Common;
using Oracle.DataAccess.Client;

namespace Atdi.Oracle.DataAccess
{
    public interface IDataAccess
    {
        void CloseConnection();
        bool DeleteRecord(string TableName, int ID, DbConnection dbConnection, DbTransaction dbTransaction);
        string GetConnectionString();
        int? GetMaxId(string TableName);
        int? GetNextId(string sequenceName);
        object[] GetValues(List<string> Columns, string TableName, int ID, string OrderBy);
        OracleParameter[][] GetValues(List<string> Columns, string TableName, string sql, string OrderBy);
        DbDataReader GetValuesSql(string sql);
        bool InsertBulkRecords(List<OracleParameter> OraParametr_Level1, string TableName_Level1, int Cnt, DbConnection dbConnection, DbTransaction dbTransaction);
        bool InsertBulkRecords(List<OracleParameter> OraParametr_Level1, string TableName_Level1, int Cnt1, List<OracleParameter> OraParametr_Level2, string TableName_Level2, int Cnt2, OracleParameter[] oracleParameterRefId, DbConnection dbConnection, DbTransaction dbTransaction);
        bool InsertBulkRecords(List<OracleParameter> OraParametr_Level1, string TableName_Level1, List<OracleParameter> OraParametr_Level2_Const, List<OracleParameter[]> OraParametr_Level2_records, string TableName_Level2, DbConnection dbConnection, DbTransaction dbTransaction);
        bool InsertBulkRecords(List<OracleParameter> OraParametr_Level1, string TableName_Level1, List<OracleParameter> OraParametr_Level2_Const, List<OracleParameter[]> OraParametr_Level2_records, string TableName_Level2, List<OracleParameter> OraParametr_Level3_Const, List<OracleParameter[]> OraParametr_Level3_records, string TableName_Level3, DbConnection dbConnection, DbTransaction dbTransaction);
        int? InsertRecord(List<OracleParameter> OraParametr, string TableName, DbConnection dbConnection, DbTransaction dbTransaction);
        DbConnection NewConnection(string connectionString);
        bool OpenConnection(string connectionString);
        int? UpdateRecord(List<OracleParameter> OraParametr, string TableName, int ID, DbConnection dbConnection, DbTransaction dbTransaction);
        bool UpdateRecord(List<OracleParameter> OraParametr, string TableName, string sql, DbConnection dbConnection, DbTransaction dbTransaction);
    }
}