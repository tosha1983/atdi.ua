using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace Atdi.CoreServices.DataLayer
{
    internal class DbProviderInfo
    {
        public string Name  {  get;  private set; }
        public string Description  { get; private set; }
        public string InvariantName  {  get; private set;  }
        public string AssemblyQualifiedName  { get;  private set;  }
        public DbProviderInfo(string name, string description, string invariantName, string assemblyQualifiedName)
        {
            Name = name;
            Description = description;
            InvariantName = invariantName;
            AssemblyQualifiedName = assemblyQualifiedName;
        }

    }

    internal class OracleDbProvider
    {
        private const string OracleDbProviderName = "Oracle.DataAccess.Client";
        private DbProviderFactory _dbProviderFactory;
        public DbProviderFactory DbProviderFactory
        {
            get {   return _dbProviderFactory ?? (_dbProviderFactory = DbProviderFactories.GetFactory(OracleDbProviderName));   }
        }

        public DbProviderInfo GetDbProviderInfo()
        {
            const int nameIndex = 0;
            const int descriptionIndex = 1;
            const int invariantNameIndex = 2;
            const int assemblyQualifiedNameIndex = 3;
            DataTable table = DbProviderFactories.GetFactoryClasses();
            foreach (DataRow row in table.Rows)
            {
                string invariantName = row[invariantNameIndex].ToString();
                if (invariantName == OracleDbProviderName)
                {
                    string name = row[nameIndex].ToString();
                    string description = row[descriptionIndex].ToString();
                    string assemblyQualifiedName = row[assemblyQualifiedNameIndex].ToString();
                    return new DbProviderInfo(name, description, invariantName, assemblyQualifiedName);
                }
            }
            return null;
        }

    }

    class GlobalDbLocal  
    {
        public DbConnection _dbConnection;
        private DbProviderFactory DbProviderFactory  {  get {  return OracleDbProvider.DbProviderFactory; }  }
        private OracleDbProvider _oracleDbProvider;
        private OracleDbProvider OracleDbProvider {  get  {   return _oracleDbProvider ?? (_oracleDbProvider = new OracleDbProvider());  } }



        /// <summary>
        /// Opening Connection to datatbase
        /// </summary>
        /// <returns>if connected success return true</returns>
        public bool OpenConnection(string ConnectString)
        {
            if (_dbConnection == null)
            {
                DbConnection dbConnection = DbProviderFactory.CreateConnection();
                if (dbConnection == null)
                    return false;
                dbConnection.ConnectionString = ConnectString;
                dbConnection.Open();
                _dbConnection = dbConnection;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true - OK, false - Error</returns>
        public bool CloseConnection()
        {
            if (_dbConnection != null)
            {
                _dbConnection.Close();
                _dbConnection.Dispose();
                _dbConnection = null;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipDb"></param>
        /// <param name="port"></param>
        /// <param name="serviceName"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetConectionString(string ipDb, int port, string serviceName, string user, string password)
        {
            const string oradb = "Data Source=(DESCRIPTION="
                               + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))"
                               + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={2})));"
                               + "User Id={3};Password={4};";
            string retStr = string.Format(oradb, ipDb, port, serviceName, user, password);
            return retStr;
        }

        //===================================================
        /// <summary>
        /// Выполнить запрос без ответа
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <returns>TRUE - все ОК</returns>
        private void ExecuteNonQuery(string query)
        {
            DbCommand cmd = _dbConnection.CreateCommand();
            cmd.Connection = _dbConnection;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }
        //===================================================
        /// <summary>
        /// Выполнить запрос с ответом
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <returns>OracleDataReader - ответ</returns>
        private DbDataReader ExecuteQuery(string query)
        {
            DbCommand cmd = _dbConnection.CreateCommand();
            cmd.Connection = _dbConnection;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            return cmd.ExecuteReader();
        }

    }

}
