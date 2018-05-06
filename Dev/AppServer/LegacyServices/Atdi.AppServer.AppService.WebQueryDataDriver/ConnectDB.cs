using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using OrmCs;
using DatalayerCs;
using System.IO;
using Atdi.AppServer.AppService.WebQueryDataDriver.XMLLibrary;
using System.Windows.Forms;
using Atdi.AppServer.AppService.WebQueryDataDriver.Logs;
using FormsCs;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Data.OleDb;
using Atdi.AppServer.AppService.WebQueryDataDriver.ICSMUtilities;


namespace Atdi.AppServer.AppService.WebQueryDataDriver
{

    [Serializable]
    [DataContract]
    public enum TypeDb
    {
        ORACLE,
        MSSQL,
        UNKNOWN
    }
    [Serializable]
    [DataContract]
    public enum enumRulesAccess
    {
        Insert,
        Update,
        Delete,
        Select
    }

    [Serializable]
    [DataContract]
    public class RecordPtrDB
    {
        public int JoinFromIndex { get; set; }
        public int JoinToIndex { get; set; }
        public int Precision { get; set; }
        public string FieldJoinFrom { get; set; }
        public string FieldCaptionFrom { get; set; }
        public string FieldJoinTo { get; set; }
        public string FieldCaptionTo { get; set; }
        public string NameTableFrom { get; set; }
        public string NameTableTo { get; set; }
        public string NameFieldForSetValue { get; set; }
        public string Name { get; set; }
        public string CaptionNameTable { get; set; }
        public int KeyValue { get; set; }
        public object Value { get; set; }
        public bool isNotNull { get; set; }
        public string DefVal { get; set; }
        public OrmVarType TypeVal { get; set; }
        public int Index { get; set; }
        public object OldVal { get; set; }
        public object NewVal { get; set; }
        public bool isMandatory { get; set; }
        public string LinkField { get; set; }
        public string NameLayer { get; set; }
        public object ident_loop { get; set; }
    }
    /// <summary>
    /// Класс, обеспечивающий соединение с базой данных,
    /// начальную инициализацию, обмен данными с таблицами СУБД
    /// </summary>
    [Serializable]
    [DataContract]
    public class ConnectDB
    {
        public static int NullI = 2147483647;
        public static double NullD = 1E-99;
        public static DateTime NullT = new DateTime(1, 1, 1, 0, 0, 0);

        public ConnectDB()
        {
            New();
        }

        /// <summary>
        /// Деструктор.
        /// </summary>
        //        ~ConnectDB()
        //      {
        //        Dispose();
        //  }

        #region IDisposable Members

        //		public void Dispose() {
        //		GC.SuppressFinalize(this);
        //}

        #endregion


        /// <summary>
        /// Текущая СУБД
        /// </summary>
        static TypeDb CurrentTypeDB = TypeDb.UNKNOWN;
        /// <summary>
        /// Метод для определения текущей СУБД на основе данных XML файла
        /// </summary>
        /// <param name="NameTypeDB"></param>
        /// <returns></returns>
        static TypeDb ConvertToTypeDb(string NameTypeDB)
        {
            TypeDb ret = TypeDb.UNKNOWN;
            if (TypeDb.MSSQL.ToString() == NameTypeDB.ToString())
            {
                ret = TypeDb.MSSQL;
            }
            if (TypeDb.ORACLE.ToString() == NameTypeDB.ToString())
            {
                ret = TypeDb.ORACLE;
            }
            if (TypeDb.UNKNOWN.ToString() == NameTypeDB.ToString())
            {
                ret = TypeDb.UNKNOWN;
            }
            return ret;
        }

        /// <summary>
        /// Признак инициализации открытия доступа у СУБД
        /// </summary>
        static public bool inited = false;
        static public ANetDb dbx = null;
        static public string SchemaDbx = "";
        static public string PathDbx = "";
        static public string OleDbConnectionString = "";

        /// <summary>
        /// Метод, выполняющий начальную инициализацию (открывает соединение с СУБД)
        /// </summary>
        /// <param name="db"> БД </param>
        /// <param name="schema"> ORM - схема</param>
        /// <param name="pathtoapp"> Путь размещения схемы </param>
        static internal void OpenConnect(out ANetDb db, out string schema, out string pathtoapp)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting)
            {
                XMLObj xml_out = new XMLObj();
                xml_out = BaseXMLDirect.GetXmlSettings(BaseXMLDirect.file_name_current);
                OleDbConnectionString = xml_out._OleConnectionString;
                pathtoapp = BaseXMLDirect.pathtoapp;
                schema = xml_out._ORMSchema;
                active_schema = schema.Remove(schema.Length - 1, 1).Trim();
                CurrentTypeDB = ConvertToTypeDb(xml_out._TypeRDBMS);

                db = ANetDb.New(xml_out._NameClientLibrary);
                db.ConnectionString = xml_out._ConnectionString;
                try
                {
                    db.Open();
                }
                catch (Exception e)
                {
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute method OpenConnect (class ConnectDB): " + e.Message);
                }
            }
        }


        static List<string> TableNames_Schema = null;     // Общий список доступных таблиц СУБД
        static string All_TableNames_Schema = "";     // Общий список доступных таблиц СУБД


        static string initializingLock = "Initializing";
        static string active_schema = "";
        static OrmLinker lk_;
        static public DbLinker Connect_ = null;


        /// <summary>
        /// Метод, реализующий инициализацию и извлечение сведений о правах доступа пользователей к объектам СУБД
        /// </summary>
        static public void Init()
        {

            lock (initializingLock)
            {
                if (!inited)
                {
                    try
                    {

                        TableNames_Schema = new List<string>();

                        OpenConnect(out dbx, out SchemaDbx, out PathDbx);
                        OrmSchema.LanguagePreferences = new string[] { "CST", "SPA", "ENG", "RUS" };
                        OrmSchema.InitIcsmSchema(dbx, SchemaDbx, PathDbx);

                        lk_ = OrmSchema.Linker;


                        /* Механизм, реализующий выборку всего доступного пользователям перечня табдиц БД*/
                        /* Данный механизм по разному работает для разных СУБД*/
                        string sql_table_name = "";
                        if (CurrentTypeDB == TypeDb.MSSQL) { sql_table_name = "SELECT DISTINCT T.NAME  FROM SYS.DATABASE_PERMISSIONS R,SYS.OBJECTS T WHERE (R.MAJOR_ID= T.OBJECT_ID) AND (T.TYPE IN ('U','V')) "; }
                        if (CurrentTypeDB == TypeDb.ORACLE) { sql_table_name = "SELECT TABLE_NAME  FROM SYS.ALL_ALL_TABLES where OWNER=" + "'" + SchemaDbx.Remove(SchemaDbx.Count() - 1, 1) + "'"; }


                        if (!string.IsNullOrEmpty(sql_table_name))
                        {
                            //ANetRs rs = db.NewRecordset();
                            ANetRs rs = dbx.NewRecordset();
                            rs.Open(sql_table_name);
                            for (; !rs.IsEOF(); rs.MoveNext())
                            {
                                // Формирование списка наименований таблиц, которые доступны пользователям 
                                try
                                {
                                    if (CurrentTypeDB == TypeDb.ORACLE)
                                    {
                                        TableNames_Schema.Add(rs.GetString(0));
                                        All_TableNames_Schema += "'" + rs.GetString(0) + "',";
                                    }
                                }

                                catch (Exception e)
                                {
                                    //throw new Exception("Invalid index:" + e.Message);
                                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid index:" + e.Message);
                                }
                            }
                            All_TableNames_Schema = All_TableNames_Schema.Length > 0 ? All_TableNames_Schema.Remove(All_TableNames_Schema.Length - 1, 1) : "";
                            rs.Destroy();
                        }
                        dbx.Destroy();
                        inited = true;
                    }
                    catch (Exception e)
                    {
                        inited = false;
                        CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute method Init (class ConnectDB): " + e.Message);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Метод, возвращающий доступ, посредством класса DbLinker к возможностям взаимодействия с объектами СУБД  
        /// </summary>
        /// <returns>Обїект DbLinker</returns>
        static public DbLinker New()
        {
            if (!inited)
            {
                Init();
                OpenConnect(out dbx, out SchemaDbx, out PathDbx);
                Connect_ = new DbLinker(dbx, SchemaDbx); //this will initialize OrmSchema.Linker
                Connect_.ClientID = "ICSMServiceID";

            }
            else
            {

            }
            return Connect_;
        }

        /// <summary>
        /// Проверка возможности подключения к БД
        /// </summary>
        /// <returns>true - если соединение успешно</returns>
        static public bool CheckConnectToDB()
        {
            bool isSuccesfullConnect = true;
            try
            {
                ConnectDB connect = new ConnectDB();
            }
            catch
            {
                isSuccesfullConnect = false;
            }
            return isSuccesfullConnect;
        }
        /// <summary>
        /// авторизация
        /// </summary>
        /// <returns></returns>
        public bool Authorize(string Login, string Password)
        {
            bool isSuccesfullAuth = false;
            try
            {
                if ((int)GetAuthUserID(Login, Password) > -1)
                    isSuccesfullAuth = true;
            }
            catch
            {
                isSuccesfullAuth = false;
            }
            return isSuccesfullAuth;
        }

        public int AuthorizeRetID(string Login, string Password)
        {
            int ID = ConnectDB.NullI;
            try {
                ID = (int)GetAuthUserID(Login, ICSMUtils.SHA256(Password));
            }
            catch
            {
                ID = ConnectDB.NullI;
            }
            return ID;
        }

        static private bool splitstring(out string s1, out string s2, string s, char sep)
        {
            s1 = null; s2 = null;
            int idx = s.IndexOf(sep);
            if (idx <= 0) return false;
            s1 = s.Substring(0, idx).Trim();
            s2 = s.Substring(idx + 1).Trim();
            return !string.IsNullOrEmpty(s1);
        }


        static public List<LovItem> GetListOfValues_ERI(string eriName, string language)
        {
            string filepath = "";
            filepath = eriName;
            using (FileStream fsa = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (TextReader sr = new StreamReader(fsa, Encoding.Unicode, true))
            {
                List<LovItem> res = new List<LovItem>();
                string line, code, translations, lang, descr;
                while ((line = sr.ReadLine()) != null)
                {
                    if (splitstring(out code, out translations, line, '\t'))
                    {
                        string firstDescr = null;
                        string englishDescr = null;
                        string requiredDescr = null;
                        foreach (string trans in translations.Split('\t'))
                        {
                            if (splitstring(out lang, out descr, trans, '='))
                            {
                                if (lang == "E") lang = "ENG"; if (lang == "F") lang = "FRE"; if (lang == "S") lang = "SPA";
                                if (lang == language) requiredDescr = descr;
                                else if (lang == "ENG") englishDescr = descr;
                                else if (lang != "BMP" && firstDescr == null) firstDescr = descr;
                            }
                        }
                        res.Add(NewLovItem(code, firstDescr, englishDescr, requiredDescr));
                    }
                }
                return res;
            }
        }

        static public LovItem NewLovItem(string code, string firstDescr, string englishDescr, string requiredDescr)
        {
            if (string.IsNullOrEmpty(requiredDescr)) requiredDescr = englishDescr;
            if (string.IsNullOrEmpty(requiredDescr)) requiredDescr = firstDescr;
            if (string.IsNullOrEmpty(requiredDescr)) requiredDescr = code;
            LovItem res = new LovItem();
            res.code = code;
            res.descr = requiredDescr;
            return res;
        }



        /// <summary>
        /// Получить описание значения по коду для Eri - файла
        /// </summary>
        /// <param name="NameEri"></param>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static string GetLovtemFromEri(string NameEri, string Lang, string Code)
        {
            string LovItem = "";
            try
            {
                List<LovItem> LV_LST = GetListOfValues_ERI(NameEri, Lang);
                foreach (LovItem item in LV_LST)
                {
                    if (item.code == Code)
                    {
                        LovItem = item.descr;
                    }
                }

            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid get list form ERI - file: " + ex.Message);
            }
            return LovItem;
        }


        /// <summary>
        /// Получить ID записи для таблицы USERS
        /// </summary>
        /// <param name="Login"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public object GetAuthUserID(string Login, string Password)
        {
            object ID = -1;
            try
            {
                ConnectDB connect = new ConnectDB();

                string Filter = "[{0}] = {1}".Fmt("WEB_LOGIN", ToSql(Login));
                Filter += "AND [{0}] = {1}".Fmt("PWD", ToSql(Password));

                List<object> lst = connect.GetFieldValue("USERS", Filter, "ID");
                if (lst != null)
                {
                    if (lst.Count == 1)
                    {
                        ID = lst[0];
                    }
                }
            }
            catch
            {

            }
            return ID;
        }



        /// <summary>
        /// Специальный метод, возвращающий список записей таблицы по перечню заданных полей
        /// </summary>
        /// <param name="lst_mass">массив полей</param>
        /// <param name="tbl_mass">наименование таблицы</param>
        /// <param name="maxRec">максимальное число записей в результате</param>
        /// <param name="filter">дополнительные параметры для фильтрации записей по заданному параметру(ам)</param>
        /// <param name="order">порядок сортировки</param>
        /// <returns></returns>
        public List<dynamic[]> ExecuteSQL(out bool isSelectedData, string[] lst_mass, string tbl_mass, int maxRec, string filter, string order)
        {
            isSelectedData = false;
            List<object[]> n_object = null;
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {


                    n_object = new List<dynamic[]>();
                    n_object = OrmSourcer.ICSMQuery(tbl_mass, lst_mass, filter, order, maxRec, Connect_);
                    if (n_object != null)
                    {
                        isSelectedData = true;
                    }
                }

            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in ExecuteSQL: " + ex.Message);
            }

            return n_object;
        }


        /// <summary>
        /// Метод, выполняющий запрос к базе данных посредством SQL - языка
        /// </summary>
        /// <param name="sql_"></param>
        /// <returns></returns>
        public static List<object[]> ExecuteSQLCommand(ref List<string> Fld, string sql_, out string TableName, int Max_Rec, string AdditionalFilter, out List<KeyValuePair<string, Type>> L_FieldType, string User_Ident)
        {
            TableName = "";
            List<object[]> RecordValues = new List<object[]>();
            List<object> RecordsVal = null;
            List<string> fld_f = new List<string>();
            int count_rec = 0;
            string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            List<KeyValuePair<string, string>> L_Field = new List<KeyValuePair<string, string>>();
            L_FieldType = new List<KeyValuePair<string, Type>>();
            List<Type[]> LTpx = new List<Type[]>();
            try {
                if ((User_Ident != "") && (sql_.Contains("{0}"))) sql_ = string.Format(sql_,User_Ident);
                if (!string.IsNullOrEmpty(sql_))  {
                    if (Connect_ != null){
                        OleDbConnection cn = new OleDbConnection();  OleDbCommand cmd = new OleDbCommand();
                        OleDbDataReader myReader;  cn.ConnectionString = OleDbConnectionString;
                        cn.Open(); cmd.Connection = cn; cmd.CommandText = sql_;
                        myReader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                        while (myReader.Read()) {
                            DataTable rx = myReader.GetSchemaTable();
                            if (rx.Rows.Count > 0) {
                                object[] cv = rx.Rows[0].ItemArray;
                                if (cv != null) {
                                    if ((cv.Count() - 2) > 0) {
                                        TableName = cv[cv.Count() - 2].ToString();
                                        break;
                                    }
                                }
                            }
                        }
                        myReader.Close();
                        cn.Close();

                        ANetRs rs = Connect_.Db.NewRecordset(); rs.Open(sql_);
                        DataTable dft = rs.GetSchemaTable();
                        foreach (DataRow row in dft.Rows)  {
                            var name = row["ColumnName"];
                            var size = row["ColumnSize"];
                            var dataType = row["DataTypeName"];
                            if (!L_Field.Contains(new KeyValuePair<string, string>(name.ToString(), dataType.ToString())))
                                L_Field.Add(new KeyValuePair<string, string>(name.ToString(), dataType.ToString()));
                        }
                        foreach (DataRow c in dft.Rows) {
                            fld_f.Add(c[0].ToString());
                        }

                        if (fld_f.Count > 0)  {
                            Fld = fld_f;
                            for (; !rs.IsEOF(); rs.MoveNext()) {
                                RecordsVal = new List<object>();
                                int i = 0;
                                foreach (KeyValuePair<string,string> item in L_Field) {
                                    object value=-1;
                                    if (item.Value == "decimal") {
                                        value = rs.GetDouble(i);
                                    }
                                    else if (item.Value == "float"){
                                        value = rs.GetDouble(i);
                                    }
                                    else if (item.Value == "nvarchar") {
                                        object xc = rs.GetString(i);
                                        if (xc != null)
                                        {
                                            string rds_ = xc.ToString();
                                            if ((rds_.EndsWith("_")) && ((rds_.Contains(",")) || (rds_.Contains("."))))
                                                rds_ = rds_.Replace("_", "").Replace(".", decimal_sep).Replace(",", decimal_sep);
                                            else if (rds_.EndsWith("_")) {
                                                foreach (char c in rds_) {
                                                    if (char.IsDigit(c)) {
                                                        rds_ = rds_.Replace("_", "");
                                                        break;
                                                    }
                                                }
                                            }
                                            value = rds_;
                                        }
                                    }
                                    else if (item.Value == "datetime") {
                                        value = rs.GetDatetime(i);
                                    }
                                    else {
                                        object xc = rs.GetString(i);
                                        if (xc != null) {
                                            string rds_ = xc.ToString();
                                            if ((rds_.EndsWith("_")) && ((rds_.Contains(",")) || (rds_.Contains("."))))
                                                rds_ = rds_.Replace("_", "").Replace(".", decimal_sep).Replace(",", decimal_sep);
                                            else if (rds_.EndsWith("_")) {
                                                foreach (char c in rds_) {
                                                    if (char.IsDigit(c)) {
                                                        rds_ = rds_.Replace("_", "");
                                                        break;
                                                    }
                                                }
                                            }
                                            value = rds_;
                                        }
                                    }

                                    if (value == null) value = "";

                                    RecordsVal.Add(value);
                                    i++;
                                }
                                RecordValues.Add(RecordsVal.ToArray());
                                count_rec++;
                                if (count_rec > Max_Rec) break;
                            }
                        }
                        rs.Destroy();
                    }
                }

                /////
                
                foreach (object[] ob in RecordValues){
                    List<Type> Column_Type = new List<Type>();
                    foreach (object bl_f in ob) {
                        double Vl_D; double Temp_d; int Temp_i; DateTime Temp_date;
                        if (bl_f != null){
                            string rds_ = bl_f.ToString();
                            if ((bl_f.ToString().EndsWith("_")) && ((bl_f.ToString().Contains(",")) || (bl_f.ToString().Contains("."))))
                                rds_ = bl_f.ToString().Replace("_", "").Replace(".", decimal_sep).Replace(",", decimal_sep);
                            else {
                                if (bl_f.ToString().EndsWith("_")) {
                                    rds_ = bl_f.ToString().Replace("_", "");
                                    char[] chs = rds_.ToArray();
                                    bool isSuccess = true;
                                    foreach (char c in chs) {
                                        if ((!char.IsDigit(c)) && (c!='-')) {
                                            isSuccess = false;
                                            break;
                                        }
                                    }
                                    if (!isSuccess) {
                                        rds_ = bl_f.ToString();
                                    }
                                }
                                else if (((bl_f.ToString().Contains(",")) || (bl_f.ToString().Contains("."))) && (!bl_f.ToString().EndsWith("_"))) {
                                    rds_ = bl_f.ToString().Replace("_", "").Replace(".", decimal_sep).Replace(",", decimal_sep);
                                }
                            }

                            if (double.TryParse(rds_.ToString(), out Temp_d)) {
                                Column_Type.Add(typeof(double));
                            }
                            else if (int.TryParse(rds_.ToString(), out Temp_i)) {
                                Column_Type.Add(typeof(int));
                            }
                            else if (DateTime.TryParse(rds_.ToString(), out Temp_date)) {
                                Column_Type.Add(typeof(DateTime));
                            }
                            else if (rds_.ToString().Trim() == ""){
                                Column_Type.Add(typeof(int));
                            }
                            else {
                                Column_Type.Add(typeof(string));
                            }
                        }
                        else {
                            Column_Type.Add(typeof(string));
                        }
                    }
                    LTpx.Add(Column_Type.ToArray());
                }

                for (int t = 0; t < fld_f.Count; t++) {
                    bool isStr = false; Type ResultType = typeof(string);
                    for (int h = 0; h < RecordValues.Count(); h++){
                        Type ytx = LTpx[h][t];
                        switch (ytx.ToString()) {
                            case "System.String":
                                isStr = true;
                                break;
                            case "System.Int32":
                                break;
                            case "System.Double":
                                break;
                            case "System.DateTime":
                                break;
                        }
                        if (isStr) { ResultType = typeof(string); break; } else { ResultType = ytx; }
                    }
                    if (ResultType.ToString() == "System.Int32") {
                        if (!L_FieldType.Contains(new KeyValuePair<string, Type>(fld_f[t], typeof(int))))
                            L_FieldType.Add(new KeyValuePair<string, Type>(fld_f[t], typeof(int)));
                    }
                    else if (ResultType.ToString() == "System.Double") {
                        if (!L_FieldType.Contains(new KeyValuePair<string, Type>(fld_f[t], typeof(double))))
                            L_FieldType.Add(new KeyValuePair<string, Type>(fld_f[t], typeof(double)));
                    }
                    else if (ResultType.ToString() == "System.String") {
                        if (!L_FieldType.Contains(new KeyValuePair<string, Type>(fld_f[t], typeof(string))))
                            L_FieldType.Add(new KeyValuePair<string, Type>(fld_f[t], typeof(string)));
                    }
                    else if (ResultType.ToString() == "System.DateTime") {
                        if (!L_FieldType.Contains(new KeyValuePair<string, Type>(fld_f[t], typeof(DateTime))))
                            L_FieldType.Add(new KeyValuePair<string, Type>(fld_f[t], typeof(DateTime)));
                    }
                }
            }
            catch (Exception ex) {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in ExecuteSQLCommand: " + ex.Message);
            }
            return RecordValues;
        }

        /// <summary>
        /// Получить максимальное значение поля
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        public int GetMaxID(string Field, string Table)
        {
            int scalar_int = -1;
            try {
                if (Connect_ != null) {
                    if (!string.IsNullOrEmpty(Table)) {
                        string sql = string.Format("SELECT MAX({0}) FROM %{1}", Field, Table);
                        scalar_int = Connect_.ExecuteScalarInt(sql);
                    }
                }
            }
            catch (Exception ex) {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in ExecuteSQLScalarInt: " + ex.Message);
            }
            return scalar_int;
        }



        /// <summary>
        /// Получить массив записей таблицы, удовлетворяющих фильтру filter
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="filter">фильтр в соответствии с которым будет выполняться поиск соответствия</param>
        /// <param name="fld">Перечень полей для извлечения</param>
        /// <returns></returns>
        public List<object[]> GetRecordsValue(out bool isSelectedData, string tableName, string filter, string[] fld)
        {
            isSelectedData = false;
            List<object[]> value = new List<object[]>();
            try {
                if (Connect_ != null) {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_)) {
                        if (!string.IsNullOrEmpty(filter)) {
                            y.Fetch(filter);
                            y.OpenRs();
                            for (; !y.IsEOF(); y.MoveNext()) {
                                isSelectedData = true;
                                List<object> rec_ptr = new List<object>();
                                for (int i = 0; i < fld.Count(); i++) {
                                    rec_ptr.Add(y.Get(fld[i]));
                                }
                                value.Add(rec_ptr.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetRecordsValue: " + ex.Message);
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSelectedData"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<string> SelectAllRec_From_WebQuery(out bool isSelectedData, string TableName, int indexField)
        {
            isSelectedData = false;
            List<string> Val = new List<string>();
            try {
                if (Connect_ != null) {
                    ANetRs rs = dbx.NewRecordset();
                    rs.Open("SELECT * FROM  " + TableName);
                    for (; !rs.IsEOF(); rs.MoveNext()) {
                        // Формирование списка наименований таблиц, которые доступны пользователям 
                        try {
                            Val.Add(rs.GetString(indexField));
                            isSelectedData = true;
                        }
                        catch (Exception e) {
                            CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error: " + e.Message);
                        }
                    }
                }
            }
            catch (Exception ex) {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetRecordsValue: " + ex.Message);
            }
            return Val;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetUserName(int ID)
        {
            string tb = "";
            string sql_ = string.Format("select NAME from USERS WHERE ID={0}",ID);
            if (!string.IsNullOrEmpty(sql_)) {
                string err = "";
                try {
                    if (Connect_ != null) {
                        ANetRs rs = Connect_.Db.NewRecordset();
                        rs.Open(sql_);
                        err = rs.SqlOrder;
                        for (; !rs.IsEOF(); rs.MoveNext()) {
                            tb = rs.GetString(0);
                            break;
                        }
                        rs.Destroy();
                    }
                }
                catch (Exception ex) {
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute GetUserName: " + ex.Message);
                }
            }
            return tb;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetRoleUserName(int ID)
        {
            string tb = "";
            string sql_ = string.Format("select CUST_TXT3 from USERS WHERE ID={0}", ID);
            if (!string.IsNullOrEmpty(sql_)){
                string err = "";
                try {
                    if (Connect_ != null) {
                        ANetRs rs = Connect_.Db.NewRecordset();
                        rs.Open(sql_);
                        err = rs.SqlOrder;
                        for (; !rs.IsEOF(); rs.MoveNext()) {
                            tb = rs.GetString(0);
                            break;
                        }
                        rs.Destroy();
                    }
                }
                catch (Exception ex) {
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute GetUserName: " + ex.Message);
                }
            }
            return tb;
        }


        public object[] GetRecordsSQL(out bool isSelectedData, string tableName, string[] fld, string filter)
        {
            isSelectedData = false;
            List<object> rec_ptr = new List<object>();
            try
            {

                List<dynamic[]> res = ExecuteSQL(out isSelectedData, fld, tableName, 1000, filter, null);
                if (res != null)
                {
                    isSelectedData = true;
                }

                foreach (dynamic[] item in res)
                {
                    rec_ptr = item.ToList();
                    break;
                }


            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetRecordsSQL: " + ex.Message);
            }
            return rec_ptr.ToArray();
        }






        /// <summary>
        /// Получить перечень запись таблицы для конкретной записи rec_id
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="rec_id"> номер записи таблицы </param>
        /// <param name="fld">Перечень полей для извлечения</param>
        /// <returns></returns>
        public List<object[]> GetRecordsValue(string tableName, int rec_id, string[] fld)
        {
            List<object[]> value = new List<object[]>();
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                    {
                        if (rec_id > -1)
                        {
                            y.Fetch(rec_id);

                            for (; !y.IsEOF(); y.MoveNext())
                            {
                                List<object> rec_ptr = new List<object>();
                                for (int i = 0; i < fld.Count(); i++)
                                {
                                    rec_ptr.Add(y.Get(fld[i]));
                                }
                                value.Add(rec_ptr.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetRecordsValue: " + ex.Message);
            }
            return value;
        }

        /// <summary>
        /// Получить перечень значений (проекцию) для указанного поля заданной таблицы для перечня записей, удовлетворяющих фильтру
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="filter">фильтр в соответствии с которым будет выполняться поиск соответствия</param>
        /// <param name="fld">заданное поле таблицы</param>
        /// <returns></returns>
        public List<object> GetFieldValue(string tableName, string filter, string fld)
        {
            List<object> value = new List<object>();
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            y.Fetch(filter);
                            y.OpenRs();
                            for (; !y.IsEOF(); y.MoveNext())
                            {
                                value.Add(y.Get(fld));
                            }
                        }
                    }
                }
                if (value.Count() == 0) value = null;
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetFieldValue: " + ex.Message);
            }
            return value;
        }



        /// <summary>
        /// Получить перечень значений (проекцию) для указанного поля заданной таблицы для перечня записей, удовлетворяющих фильтру
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="filter">фильтр в соответствии с которым будет выполняться поиск соответствия</param>
        /// <param name="fld">заданное поле таблицы</param>
        /// <returns></returns>
        public List<object> GetFieldValue(string tableName, string filter, string fld, DbLinker lk)
        {
            List<object> value = new List<object>();
            try
            {
                using (Yyy y = Yyy.CreateObject(tableName, lk))
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        y.Fetch(filter);
                        y.OpenRs();
                        for (; !y.IsEOF(); y.MoveNext())
                        {
                            value.Add(y.Get(fld));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetFieldValue: " + ex.Message);
            }
            return value;
        }

        /// <summary>
        /// Получить перечень значений (проекцию) для указанного поля заданной таблицы для конкретной записи rec_id
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="rec_id"> номер записи таблицы </param>
        /// <param name="fld"> поле данных значение которого нужно извлечь</param>
        /// <returns></returns>
        public List<object> GetFieldValue(string tableName, int rec_id, string fld)
        {
            List<object> value = new List<object>();
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                    {
                        if (rec_id > -1)
                        {
                            y.Fetch(rec_id);

                            y.OpenRs();
                            for (; !y.IsEOF(); y.MoveNext())
                            {
                                value.Add(y.Get(fld));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetFieldValue: " + ex.Message);
            }
            return value;
        }

        /// <summary>
        /// Получить значение заданного поля для записи отобранной по фильтру
        /// </summary>
        /// <param name="tableName"> Имя таблицы </param>
        /// <param name="filter">Фильтр записей</param>
        /// <param name="fld">поле данных, которое необходимо извлечь</param>
        /// <returns></returns>
        public object GetOneFieldValue(string tableName, string filter, string fld)
        {
            object value = null;
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            y.Fetch(filter);
                            y.OpenRs();
                            if (!y.IsEOF())
                            {
                                value = y.Get(fld);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetOneFieldValue: " + ex.Message);
            }
            return value;
        }
        /// <summary>
        /// Получить значение заданного поля для конкретного номера записи
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="rec_id">Номер записи</param>
        /// <param name="fld">конкретное поле, значение которого извлекается</param>
        /// <returns></returns>
        public static object GetOneFieldValue(string tableName, int rec_id, string fld)
        {
            object value = null;
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                    {
                        if (rec_id > -1)
                        {
                            y.Fetch(rec_id);
                            y.OpenRs();
                            if (!y.IsEOF())
                            {
                                value = y.Get(fld);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetOneFieldValue: " + ex.Message);
            }
            return value;
        }


        /// <summary>
        /// Вставка новой записи в указанную таблицу
        /// со вставкой значений для полей fld, со значениями values
        /// </summary>
        /// <param name="tableName">Имя таблицы </param>
        /// <param name="fld"> Список полей для вставки</param>
        /// <param name="values">Перечень(Массив) значений, соответствующий списку полей</param>
        public bool NewRecord(string tableName, string[] fld, object[] values, bool[] IsPKField)
        {
            bool isSuccessFullNew = true;
            //using (DbLinker lk = ConnectDB.New())
            if (Connect_ != null)
            {
                using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                {
                    try
                    {
                        int indexPK = -1;
                        for (int i = 0; i < IsPKField.Count(); i++)
                        {
                            if (IsPKField[i]) { indexPK = i; break; }
                        }

                        if (indexPK > -1)
                        {
                            int id = y.AllocID(1, -1);
                            y.New();
                            for (int i = 0; i < fld.Count(); i++)
                            {
                                if (fld[i] == fld[indexPK])
                                {
                                    y.Set(fld[indexPK], id);
                                }
                                else
                                {
                                    y.Set(fld[i], values[i]);
                                }
                            }
                            y.Save();
                        }
                        else
                        {

                            y.New();
                            for (int i = 0; i < fld.Count(); i++)
                            {
                                y.Set(fld[i], values[i]);
                            }
                            y.Save();

                        }
                    }
                    catch (Exception ex)
                    {
                        isSuccessFullNew = false;
                        CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in NewRecord: " + ex.Message);
                    }
                }
            }
            return isSuccessFullNew;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <param name="values"></param>
        /// <param name="lk"></param>
        /// <returns></returns>
        public int NewRecord(string tableName, string[] fld, object[] values, string FLD_KEY, DbLinker lk)
        {
            bool isSuccessFullNew = true;
            string NameFieldException = "";
            int NewRec = -1;
            using (Yyy y = Yyy.CreateObject(tableName, lk))
            {
                try
                {
                    int id = -1;
                    if (FLD_KEY == "ID") { id = y.AllocID(1, -1); }
                    if (id > -1) { NewRec = id; } else { NewRec = GetMaxID(FLD_KEY, tableName); }
                    y.New();
                    for (int i = 0; i < fld.Count(); i++)
                    {
                        NameFieldException = fld[i];
                        if (i == 0) y.Set(FLD_KEY, NewRec);

                        if (fld[i] != FLD_KEY)
                        {
                            y.Set(fld[i], values[i]);
                        }

                    }
                    y.Save();
                }
                catch (Exception ex)
                {
                    NewRec = -1;
                    isSuccessFullNew = false;
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in NewRecord: (" + tableName + ") Field: " + NameFieldException + " - " + ex.Message);
                }
            }
            return NewRec;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fld_check"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static RecordPtrDB GetTableFromORM(string fld_check, string tableName)
        {
            RecordPtrDB rc = new RecordPtrDB();
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null)
            {
                foreach (OrmField f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case OrmFieldNature.Join:
                            {

                                OrmFieldJ fj = (OrmFieldJ)f1;
                                OrmJoin joi = fj.Join;
                                OrmTable tc = joi.JoinedTable;


                                string joinedClass = OrmSourcer.TableNameToClassName(f1.Name);

                                if (fld_check == f1.Name)
                                {
                                    rc.NameTableTo = tc.Name;
                                    rc.Name = f1.Name;
                                    if (joi.From.Count() > 0) rc.FieldJoinFrom = joi.From[0].Name;
                                    if (joi.To.Count() > 0) rc.FieldJoinTo = joi.To[0].Name;
                                    if (joi.To.Count() > 0) { if (joi.To[0].DDesc != null) rc.TypeVal = joi.To[0].DDesc.ClassType; rc.Precision = joi.To[0].DDesc.Precision; }
                                }
                            }
                            break;

                    }
                }
            }
            return rc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fld_check"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool CheckFieldPrimary(string fld_check, string tableName)
        {
            bool rc = false;
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null)
            {
                foreach (OrmField f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case OrmFieldNature.Column:
                            {
                                OrmFieldF fjF = (OrmFieldF)f1;

                                string joinedClass = OrmSourcer.TableNameToClassName(f1.Name);

                                if (fld_check == f1.Name)
                                {
                                    if (fjF != null)
                                    {
                                        if (fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_PRIMARY | OrmCs.OrmFieldFOption.fld_FKEY))
                                        {
                                            rc = true;
                                        }
                                    }


                                }
                            }
                            break;

                    }
                }
            }
            return rc;
        }


        public static List<string> GetNotNullFields(string tableName)
        {
            List<string> rc = new List<string>();
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null)
            {
                foreach (OrmField f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case OrmFieldNature.Column:
                            {
                                OrmFieldF fjF = (OrmFieldF)f1;
                                string joinedClass = OrmSourcer.TableNameToClassName(f1.Name);

                                if (fjF != null)
                                {
                                    //if ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL)) || ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL)) && (fjF.Options == (OrmCs.OrmFieldFOption.fld_PRIMARY))) || ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL)) && (fjF.Options == (OrmCs.OrmFieldFOption.fld_FKEY))))
                                    if (fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_PRIMARY | OrmCs.OrmFieldFOption.fld_FKEY) || (fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_FKEY)) || (fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_PRIMARY)) || (fjF.Options == OrmCs.OrmFieldFOption.fld_NOTNULL) || (fjF.Options == OrmCs.OrmFieldFOption.fld_FKEY))
                                    {
                                        rc.Add(f1.Name);
                                    }
                                }

                            }
                            break;

                    }
                }
            }
            return rc;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="fld_check"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool CheckField(string fld_check, string tableName)
        {
            bool rc = false;
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null)
            {
                foreach (OrmField f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case OrmFieldNature.Column:
                            {
                                OrmFieldF fjF = (OrmFieldF)f1;
                                if (fld_check == f1.Name)
                                {
                                   rc = true;
                                   break;
                                }
                            }
                            break;

                    }
                }
            }
            return rc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetPrimaryField(string tableName)
        {
            string name = "";
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null)
            {
                foreach (OrmField f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case OrmFieldNature.Column:
                            {
                                OrmFieldF fjF = (OrmFieldF)f1;

                                string joinedClass = OrmSourcer.TableNameToClassName(f1.Name);

                                if (fjF != null)
                                {
                                    if ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_PRIMARY | OrmCs.OrmFieldFOption.fld_FKEY)) || ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_PRIMARY)) && (fjF.Index == 0)))
                                    //if ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_PRIMARY | OrmCs.OrmFieldFOption.fld_FKEY)))
                                    {
                                        name = f1.Name;
                                    }
                                }
                            }
                            break;

                    }
                }
            }
            return name;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="fld_check"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string GetTypeField(string fld_check, string tableName)
        {
            string rc = null;
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null)
            {
                foreach (OrmField f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case OrmFieldNature.Column:
                            {
                                OrmFieldF fjF = (OrmFieldF)f1;
                                if (fld_check == f1.Name)
                                {
                                    if (fjF != null)
                                    {
                                        OrmFieldFOption opt = fjF.Options;
                                        OrmDataDesc dd = new OrmDataDesc();
                                        rc = fjF.DDesc.ParamType;
                                    }

                                }
                            }
                            break;

                    }
                }
            }
            return rc;
        }


        public static OrmField GetOrmDataDesc(string fld_check, string tableName)
        {
            OrmField rc = null;
            OrmTable zeta = OrmSchema.Table(tableName, false);
            if (zeta != null)
            {
                foreach (OrmField f1 in zeta.ClassFields)
                {
                    switch (f1.Nature)
                    {
                        case OrmFieldNature.Column:
                            {
                                OrmFieldF fjF = (OrmFieldF)f1;
                                if (fld_check == f1.Name)
                                {
                                    if (fjF != null)
                                    {
                                        rc = fjF;
                                    }

                                }
                            }
                            break;

                    }
                }
            }
            return rc;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <returns></returns>
        public static List<RecordPtrDB> GetFieldFromOrm(string tableName, string[] fld)
        {
              List<RecordPtrDB> recDB_Lst = new List<RecordPtrDB>();
              string TableName2 = "";
                List<int> Lst_Val_Index = new List<int>();
                for (int i = 0; i < fld.Count(); i++)  {
                    if (i == 0) { TableName2 = tableName; }
                    string[] Spl = null;
                    RecordPtrDB recDB = new RecordPtrDB();
                    recDB.NameTableTo = tableName;

                    if (fld[i].IndexOf(".") > 0) {
                        Spl = fld[i].Split(new char[] { '.' });
                    }

                    if (Spl != null)  {
                        for (int r = 0; r < Spl.Count() - 1; r++)  {
                            recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                            recDB.NameTableFrom = (r == 0 ? TableName2 : Spl[r - 1]);
                            Spl[r] = recDB.NameTableTo;
                        }
                        if (Spl.Count() > 0) recDB.NameFieldForSetValue = Spl[Spl.Count() - 1];
                        recDB_Lst.Add(recDB);
                    }
                    else {
                        recDB.NameTableTo = tableName;
                        recDB.NameFieldForSetValue = fld[i];
                        recDB_Lst.Add(recDB);
                    }
                }

                foreach (RecordPtrDB it in recDB_Lst) {
                    OrmField ty_p = ConnectDB.GetOrmDataDesc(it.NameFieldForSetValue, it.NameTableTo);
                    if (ty_p != null) {
                        it.TypeVal = ty_p.DDesc.ClassType;
                    }
                }
                return recDB_Lst;
        }

        public static List<Type> GetAllTypesFromFlds(string TableName, List<string> FLDs)
        {
            List<Type> tp_lst = new List<Type>();
            List<RecordPtrDB> rtv = ConnectDB.GetFieldFromOrm(TableName, FLDs.ToArray());
            foreach (RecordPtrDB iu in rtv)
            {
                if (iu != null)
                {
                    switch (iu.TypeVal)
                    {
                        case OrmVarType.var_Int:
                            tp_lst.Add(typeof(int));
                            break;
                        case OrmVarType.var_Dou:
                            tp_lst.Add(typeof(double));
                            break;
                        case OrmVarType.var_String:
                            tp_lst.Add(typeof(string));
                            break;
                        case OrmVarType.var_Flo:
                            tp_lst.Add(typeof(float));
                            break;
                        case OrmVarType.var_Tim:
                            tp_lst.Add(typeof(DateTime));
                            break;
                        default:
                            tp_lst.Add(typeof(string));
                            break;
                    }
                }
            }
            return tp_lst;
        }

        /// <summary>
        /// Получить описание поля
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <returns></returns>
        public static OrmField GetFieldFromOrm(string tableName, string fld)
        {
            RecordPtrDB recDB_Lst = new RecordPtrDB();
            string TableName2 = "";
            List<int> Lst_Val_Index = new List<int>();
            TableName2 = tableName;
            string[] Spl = null;
            RecordPtrDB recDB = new RecordPtrDB();
            recDB.NameTableTo = tableName;

            if (fld.IndexOf(".") > 0)
            {
                Spl = fld.Split(new char[] { '.' });
            }

            if (Spl != null)
            {
                for (int r = 0; r < Spl.Count() - 1; r++)
                {
                    recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                    recDB.NameTableFrom = (r == 0 ? TableName2 : Spl[r - 1]);
                    Spl[r] = recDB.NameTableTo;
                }
                if (Spl.Count() > 0) recDB.NameFieldForSetValue = Spl[Spl.Count() - 1];
            }
            else
            {
                recDB.NameTableTo = tableName;
                recDB.NameFieldForSetValue = fld;
            }
            return ConnectDB.GetOrmDataDesc(recDB.NameFieldForSetValue, recDB.NameTableTo);
        }



        public static bool SetStatusArchive(int ID, string tableName)
        {
            bool Ret = false;
            try
            {
                if (CheckField("STATUS", tableName))
                {
                    SetFieldValue(tableName, ID, "STATUS", "Z");
                }
                else Ret = false;
            }
            catch (Exception ex) { }
            return Ret;
        }
        /// <summary>
        /// Поиск таблиц для котрых не найдено ID, т.е. таблиц для последующей вставки новых записей
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <param name="values"></param>
        /// <param name="lk"></param>
        /// <returns></returns>
        public string[] GetTablesEmptyLink(int ID, string tableName, string[] fld, DbLinker lk)
        {
            List<string> Lst_Table = new List<string>();
            string NameFieldException = "";
            int ID_Z = -1;

            string TableName2 = "";
            try
            {

                for (int i = 0; i < fld.Count(); i++)
                {
                    if (i == 0) { TableName2 = tableName.Trim(); ID_Z = ID; } //else { tableName = TableName2; }

                    NameFieldException = fld[i];
                    string JoinTab = "";
                    string[] Spl = null;

                    List<RecordPtrDB> recDB_Lst = new List<RecordPtrDB>();

                    RecordPtrDB recDB = new RecordPtrDB();
                    recDB.NameTableTo = tableName;

                    if (fld[i].IndexOf(".") > 0)
                    {
                        Spl = fld[i].Split(new char[] { '.' });
                    }

                    if (Spl != null)
                    {
                        for (int r = 0; r < Spl.Count() - 1; r++)
                        {
                            recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                            Spl[r] = recDB.NameTableTo;
                            recDB_Lst.Add(recDB);
                        }

                        if (recDB_Lst.Count() > 0)
                        {
                            //recDB_Lst[0].FieldJoinTo=recDB_Lst[1].FieldJoinFrom;
                            int ID_Val = -1;
                            ID_Val = (int)GetOneFieldValue(tableName, ID, recDB_Lst[0].FieldJoinFrom);
                            for (int kk = 0; kk < recDB_Lst.Count(); kk++)
                            {

                                if (kk == recDB_Lst.Count() - 1)
                                {
                                    if (kk >= 1)
                                    {
                                        tableName = recDB_Lst[kk - 1].NameTableTo;
                                        ID = recDB_Lst[kk - 1].KeyValue;
                                    }
                                    else if (kk == 0)
                                    {
                                        tableName = TableName2;
                                        ID = ID_Z;
                                    }

                                    object vl = GetOneFieldValue(tableName, ID, recDB_Lst[kk].FieldJoinFrom);
                                    if (vl != null) { ID_Val = (int)vl; } else { ID_Val = NullI; }
                                    recDB_Lst[kk].Value = GetOneFieldValue(recDB_Lst[kk].NameTableTo, string.Format("[{0}]={1}", recDB_Lst[kk].FieldJoinTo, ID_Val), Spl[Spl.Count() - 1]);
                                    if ((!((ID_Val > -1) && (ID_Val < NullI))) || (ID_Val == null))
                                    {
                                        Lst_Table.Add(recDB_Lst[kk].NameTableTo);
                                    }
                                }
                                else
                                {

                                    if (kk >= 1)
                                    {
                                        ID_Val = (int)GetOneFieldValue(recDB_Lst[kk - 1].NameTableTo, recDB_Lst[kk].KeyValue, recDB_Lst[kk].FieldJoinFrom);
                                    }
                                    else ID_Val = (int)GetOneFieldValue(TableName2, ID_Z, recDB_Lst[kk].FieldJoinFrom);


                                    object obj = GetOneFieldValue(recDB_Lst[kk].NameTableTo, string.Format("[{0}]={1}", recDB_Lst[kk].FieldJoinTo, ID_Val), recDB_Lst[kk].FieldJoinTo);
                                    if (obj != null) { ID_Val = (int)obj; recDB_Lst[kk].KeyValue = ID_Val; }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Lst_Table.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <param name="values"></param>
        /// <param name="lk"></param>
        /// <returns></returns>
        public string[] GetTablesEmptyLink(string tableName, string[] fld, DbLinker lk)
        {
            List<string> Lst_Table = new List<string>();
            string NameFieldException = "";
            int ID_Z = -1;

            string TableName2 = "";
            try
            {

                for (int i = 0; i < fld.Count(); i++)
                {
                    if (i == 0) { TableName2 = tableName.Trim(); } //else { tableName = TableName2; }

                    NameFieldException = fld[i];
                    string JoinTab = "";
                    string[] Spl = null;

                    List<RecordPtrDB> recDB_Lst = new List<RecordPtrDB>();

                    RecordPtrDB recDB = new RecordPtrDB();
                    recDB.NameTableTo = tableName;

                    if (fld[i].IndexOf(".") > 0)
                    {
                        Spl = fld[i].Split(new char[] { '.' });
                    }

                    if (Spl != null)
                    {
                        for (int r = 0; r < Spl.Count() - 1; r++)
                        {
                            recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                            Spl[r] = recDB.NameTableTo;
                            recDB_Lst.Add(recDB);
                            if (!Lst_Table.Contains(recDB.NameTableTo))
                                Lst_Table.Add(recDB.NameTableTo);
                        }

                    }
                }
            }
            catch (Exception)
            {
            }
            return Lst_Table.ToArray();
        }

        /// <summary>
        /// Обновление конкретной записи указанной таблицы 
        /// для перечня полей fld новыми значениями values
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="indx">Номер записи</param>
        /// <param name="fld">Перечень полей</param>
        /// <param name="values">Перечень значений</param>
        public void EditRecord(string tableName, int indx, string[] fld, object[] values)
        {
            //using (DbLinker lk = ConnectDB.New())
            if (Connect_ != null)
            {
                using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                {
                    try
                    {
                        if (indx > -1)
                        {
                            y.Fetch(indx);
                            for (int i = 0; i < fld.Count(); i++)
                            {
                                y.Set(fld[i], values[i]);
                            }
                            y.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in EditRecord: " + ex.Message);
                    }
                }
            }
        }

        public bool EditRecord(string tableName, int indx, string[] fld, object[] values, DbLinker lk)
        {
            bool isSuccessFullNew = true;
            using (Yyy y = Yyy.CreateObject(tableName, lk))
            {
                try
                {
                    if (indx > -1)
                    {
                        y.Fetch(indx);
                        for (int i = 0; i < fld.Count(); i++)
                        {
                            y.Set(fld[i], values[i]);
                        }
                        y.Save();
                    }
                }
                catch (Exception ex)
                {
                    isSuccessFullNew = false;
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in EditRecord: (" + tableName + ")" + ex.Message);
                }
            }
            return isSuccessFullNew;
        }

        public List<int> GetIdFromRecords(string filter, string table_name, string indx_fld)
        {
            List<int> res = new List<int>();
            if (Connect_ != null)
            {

                using (Yyy y = Yyy.CreateObject(table_name, Connect_))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            y.Format(indx_fld);
                            y.Fetch(filter);
                            y.OpenRs();
                            for (; !y.IsEOF(); y.MoveNext())
                            {
                                res.Add((int)y.Get(indx_fld));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in EditRecord: " + ex.Message);
                    }
                }
            }
            return res;
        }

        public string ToSql(string s)
        {
            if (s == null) return "NULL";
            return "'" + s.Replace("'", "''") + "'";
        }

        public string GenerateFormat(bool[] IsPKField, string[] fld, object[] values, bool IsExpressionInclude)
        {
            string res = "";
            List<int> Ind = new List<int>();
            for (int i = 0; i < IsPKField.Count(); i++)
            {
                if (IsPKField[i])
                {
                    Ind.Add(i);
                }
            }

            for (int i = 0; i < Ind.Count(); i++)
            {
                if ((IsPKField[Ind[i]]) && (Ind.Count() > 1) && (i < Ind.Count() - 1))
                {
                    if (IsExpressionInclude)
                    {
                        res += "[{0}] = {1}  AND ".Fmt(fld[Ind[i]], ToSql(values[Ind[i]].ToString()));
                    }
                    else
                    {
                        res += "{0} = {1}  AND ".Fmt(fld[Ind[i]], ToSql(values[Ind[i]].ToString()));
                    }
                }
                else if ((IsPKField[Ind[i]]) && (i == Ind.Count() - 1))
                {
                    if (IsExpressionInclude)
                    {
                        res += "[{0}] = {1} ".Fmt(fld[Ind[i]], ToSql(values[Ind[i]].ToString()));
                    }
                    else
                    {
                        res += "{0} = {1} ".Fmt(fld[Ind[i]], ToSql(values[Ind[i]].ToString()));
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Редактирование записей таблицы tableName, которые соответствуют фильтру filter
        /// для перечня полей fld значениями values
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="filter">Фильтр записей</param>
        /// <param name="fld">Поля для обновления данных</param>
        /// <param name="values">Значения, для полей fld</param>
        public void EditRecord(string tableName, string[] fld, object[] values, bool[] IsPKField)
        {
            //using (DbLinker lk = ConnectDB.New())
            if (Connect_ != null)
            {
                using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(GenerateFormat(IsPKField, fld, values, true)))
                        {
                            y.Fetch(GenerateFormat(IsPKField, fld, values, true));
                            for (int i = 0; i < fld.Count(); i++)
                            {
                                y.Set(fld[i], values[i]);
                            }
                            y.Save();
                        }



                    }
                    catch (Exception ex)
                    {
                        CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in EditRecord: " + ex.Message);
                    }
                }
            }
        }
        /// <summary>
        /// Удаление записи таблицы tableName, соответствующей фильтру [fld]=value
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="fld"> поле для фильтра</param>
        /// <param name="value"> значение для фильтра</param>
        public void DeleteRecord(string tableName, string fld, object value)
        {
            try
            {
                // using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                    {
                        if ((!string.IsNullOrEmpty(fld)))
                        {
                            if (y.Fetch(string.Format("[{0}]={1}", fld, value)))
                            {
                                y.DeleteThisID();
                                y.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in DeleteRecord: " + ex.Message);
            }
        }

        /// <summary>
        /// Удалить запись
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <param name="value"></param>
        /// <param name="lk"></param>
        public void DeleteRecord(string tableName, string fld, object value, DbLinker lk)
        {
            try
            {
                using (Yyy y = Yyy.CreateObject(tableName, lk))
                {
                    if ((!string.IsNullOrEmpty(fld)))
                    {
                        if (y.Fetch(string.Format("[{0}]={1}", fld, value)))
                        {
                            y.DeleteThisID();
                            y.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in DeleteRecord: " + ex.Message);
            }
        }

        /// <summary>
        /// Удаление записи таблицы tableName, соответствующей фильтру filter
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="filter"> поле для фильтра</param>

        public void DeleteRecord(string tableName, string[] fld, object[] values, bool[] IsPKField)
        {
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                    {
                        if ((!string.IsNullOrEmpty(GenerateFormat(IsPKField, fld, values, true))))
                        {
                            if (y.Fetch(GenerateFormat(IsPKField, fld, values, true)))
                            {
                                y.OpenRs();
                                for (; !y.IsEOF(); y.MoveNext())
                                {
                                    //y.DeleteThisID();
                                    y.Delete();
                                }
                                y.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in DeleteRecord: " + ex.Message);
            }
        }
        /// <summary>
        /// Удаление конкретной записи таблицы tableName по значению номера записи indx
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="indx">номер записи</param>
        public void DeleteRecord(string tableName, int indx)
        {
            try
            {
                // using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                    {
                        if (indx > -1)
                        {
                            if (y.Fetch(indx))
                            {
                                y.DeleteThisID();
                                y.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in DeleteRecord: " + ex.Message);
            }
        }
        /// <summary>
        /// Установка нового значения поля fld для перечня записей, удовлетворяющих фильтру filter
        /// </summary> 
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="filter"> Фильтр данных </param>
        /// <param name="fld"> поле для установки нового значения </param>
        /// <param name="value"> Новое значение</param>
        public bool SetFieldValue(string tableName, string filter, string fld, object value)
        {
            bool isSuccessEdit = false;
            try {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null) {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_)) {
                        if (!string.IsNullOrEmpty(filter)) {
                            y.Fetch(filter);
                        }
                        string tp = GetTypeField(fld, tableName);
                        switch (tp) {
                            case "double":
                                value = Convert.ChangeType(value, typeof(double));
                                break;
                            case "int":
                                value = Convert.ChangeType(value, typeof(int));
                                break;

                        }
                        y.Set(fld, value);
                        y.Save();
                        isSuccessEdit = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in SetFieldValue: " + ex.Message);
            }
            return isSuccessEdit;
        }



        /// <summary>
        /// Установка нового значения поля fld для запиcи с номером rec_id
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="rec_id">Номер записи </param>
        /// <param name="fld">Наименование поля </param>
        /// <param name="value">Значение поля</param>
        public static void SetFieldValue(string tableName, int rec_id, string fld, object value)
        {

            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_ != null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_))
                    {

                        if (rec_id > -1)
                        {
                            y.Fetch(rec_id);
                        }
                        string tp = GetTypeField(fld, tableName);
                        switch (tp)
                        {
                            case "double":
                                value = Convert.ChangeType(value, typeof(double));
                                break;
                            case "int":
                                value = Convert.ChangeType(value, typeof(int));
                                break;

                        }

                        y.Set(fld, value);
                        y.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in SetFieldValue: " + ex.Message);
            }
        }
        /// <summary>
        /// Спислк таблиц БД
        /// </summary>
        /// <returns></returns>
        public List<string> GetTableNames()
        {
            return TableNames_Schema;
        }

        static public OrmLinker GetOrmRs()
        {
            return lk_;
        }


        /// <summary>
        /// Запись в лог событий базы ICSM
        /// </summary>
        /// <param name="type"></param>
        /// <param name="what"></param>
        /// <param name="message"></param>
        public void WriteLogICSMBase(ELogsWhat who, string message, string User, string TableName)
        {
            try
            {
                int ID =  GetMaxID("ID", "SYS_LOGS");
                NewRecord("SYS_LOGS", new string[] { "ID", "EVENT", "TABLE_NAME", "LCOUNT", "INFO1", "WHO", "WHEN" }, new object[] { (ID == NullI) ? 1 : ID + 1, who.ToString().Length > 20 ? who.ToString().Substring(0, 19) : who.ToString(), TableName, 1, message, User, DateTime.Now }, new bool[] { true, false, false, false, false, false, false });
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="flds"></param>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <param name="nbMaxRec"></param>
        /// <param name="lk"></param>
        /// <param name="isMaxRec"></param>
        /// <returns></returns>
        static public List<object[]> ICSMQuery(string table, string[] flds, string filter, string order, int nbMaxRec, OrmLinker lk = null, bool isMaxRec = true)
        {
            OrmRs rs = new OrmRs();
            rs.Init(lk == null ? OrmSchema.Linker : lk);
            int count = flds.GetLength(0);
            OrmItem[] it = new OrmItem[count];
            OrmVarType[] vt = new OrmVarType[count];

            for (int i = 0; i < count; i++)
            {
                string fld = flds[i]; if (fld.StartsWith("[") && fld.EndsWith("]")) fld = fld.Substring(1, fld.Length - 2);
                it[i] = rs.AddFld(table, fld, null, true);
                vt[i] = it[i] == null ? OrmVarType.var_Null : it[i].m_dataDesc.ClassType;
                Type nettype;
                switch (vt[i])
                {
                    case OrmVarType.var_Dou: nettype = typeof(double); break;
                    case OrmVarType.var_Flo: nettype = typeof(float); break;
                    case OrmVarType.var_Guid: nettype = typeof(Guid); break;
                    case OrmVarType.var_Int: nettype = typeof(int); break;
                    case OrmVarType.var_String: nettype = typeof(string); break;
                    case OrmVarType.var_Tim: nettype = typeof(DateTime); break;
                    default: nettype = typeof(object); break;
                }
            }
            //if (!CheckField("STATUS", table))
                rs.SetAdditionalFilter(table, filter);
            //else
                //rs.SetAdditionalFilter(table, filter + " AND ([STATUS] NOT IN ('Z'))");
            rs.SetExternalOrder(table, order);
            List<object[]> dataset = new List<object[]>();
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                object[] row = new object[count];
                for (int j = 0; j < count; j++)
                {
                    object c = it[j].Val;
                    if (vt[j] == OrmVarType.var_Int && c is double)
                    {
                        if ((double)c == NullD) c = 0x7FFFFFFF; else c = (int)(double)c;
                    }
                    row[j] = c;
                }
                dataset.Add(row);
                if (isMaxRec)
                {
                    if (dataset.Count == nbMaxRec) break;
                }
            }
            rs.Clear();
            return dataset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="flds"></param>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <param name="nbMaxRec"></param>
        /// <param name="NameFldExpr"></param>
        /// <param name="ExprValue"></param>
        /// <param name="NameSemant"></param>
        /// <param name="lk"></param>
        /// <param name="isMaxRec"></param>
        /// <returns></returns>
        static public List<object[]> ICSMQuery(string table, string[] flds, string filter, string order, int nbMaxRec,List<OrmItemExpr> LstOrmItemExpr , OrmLinker lk = null, bool isMaxRec = true)
        {
            List<object[]> dataset = new List<object[]>();
            try
            {
                OrmRs rs = new OrmRs();
                rs.Init(lk == null ? OrmSchema.Linker : lk);
                int count = flds.GetLength(0);
                OrmItem[] it = new OrmItem[count];
                OrmVarType[] vt = new OrmVarType[count];


                for (int i = 0; i < count; i++)
                {
                    string fld = flds[i]; if (fld.StartsWith("[") && fld.EndsWith("]")) fld = fld.Substring(1, fld.Length - 2);

                    bool isExpr = false;
                    if (LstOrmItemExpr != null)
                    {
                        if ((LstOrmItemExpr.Find(r => r.m_name == fld) != null))
                        {
                            isExpr = true;
                            it[i] = rs.AddExpr(table, LstOrmItemExpr.Find(r => r.m_name == fld).m_name, LstOrmItemExpr.Find(r => r.m_name == fld).m_expression, LstOrmItemExpr.Find(r => r.m_name == fld).m_sp);
                        }
                    }

                    if (!isExpr)
                    {
                        it[i] = rs.AddFld(table, fld, null, true);
                    }


                    vt[i] = it[i] == null ? OrmVarType.var_Null : it[i].m_dataDesc.ClassType;
                    Type nettype;
                    switch (vt[i])
                    {
                        case OrmVarType.var_Dou: nettype = typeof(double); break;
                        case OrmVarType.var_Flo: nettype = typeof(float); break;
                        case OrmVarType.var_Guid: nettype = typeof(Guid); break;
                        case OrmVarType.var_Int: nettype = typeof(int); break;
                        case OrmVarType.var_String: nettype = typeof(string); break;
                        case OrmVarType.var_Tim: nettype = typeof(DateTime); break;
                        default: nettype = typeof(object); break;
                    }
                }
                //if (!CheckField("STATUS", table))
                    rs.SetAdditionalFilter(table, filter);
                //else
                    //rs.SetAdditionalFilter(table, filter + " AND ([STATUS] NOT IN ('Z'))");
                rs.SetExternalOrder(table, order);
                for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                {
                    object[] row = new object[count];
                    for (int j = 0; j < count; j++)
                    {
                        if (it[j] != null)
                        {
                            object c = it[j].Val;
                            if (vt[j] == OrmVarType.var_Int && c is double)
                            {
                                if ((double)c == NullD) c = 0x7FFFFFFF; else c = (int)(double)c;
                            }
                            row[j] = c;
                        }
                        else row[j] = "";
                    }
                    dataset.Add(row);
                    if (isMaxRec)
                    {
                        if (dataset.Count == nbMaxRec) break;
                    }
                }
                rs.Clear();
            }
            catch (Exception ex)
            {

            }
            return dataset;
        }


        /// <summary>
        /// Проверка нужно ли создавать новую запись в БД или нет
        /// </summary>
        /// <param name="ID_Primary"></param>
        /// <param name="NameTable"></param>
        /// <param name="OutLst"></param>
        /// <returns></returns>
        public bool isNeedCreateRecord(int ID_Primary, string NameTable, string NameField, List<RecordPtrDB> OutLst, out object RetValue, int Type_x)
        {
            bool isNeedCreateRecord = true;
            RetValue = null;
            try
            {
                string NameFieldOrm = ""; string Field = "";
                foreach (RecordPtrDB item in OutLst)
                {
                    NameFieldOrm += item.Name;
                    if (Type_x == 0) Field = NameFieldOrm.Length > 0 ? (NameFieldOrm + "." + NameField) : NameField;
                    if (Type_x == 1) Field = NameFieldOrm.Length > 0 ? (NameFieldOrm + "." + NameField) : NameField;
                    NameFieldOrm += ".";
                }
                if (Field[0] == '.') Field = Field.Remove(0, 1);

                string FieldPrimary = NameField;
                if (!string.IsNullOrEmpty(FieldPrimary))
                {
                    bool issel = false;
                    object[] Res = GetRecordsSQL(out issel, NameTable, new string[] { Field }, string.Format(" ([{0}]={1}) ", FieldPrimary, ID_Primary));
                    if (Res != null)
                    {
                        if (Res.Length > 0)
                        {
                            isNeedCreateRecord = false;
                            int rc = (int)Res[0];
                            if (rc != NullI) { RetValue = rc; } else return true;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return isNeedCreateRecord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID_Primary"></param>
        /// <param name="OWNER_ID"></param>
        /// <param name="TableName"></param>
        /// <param name="recL"></param>
        /// <returns></returns>
        public int UpdateBlockData(int ID_Primary, int OWNER_ID, string TableName, List<RecordPtrDB> recL)
        {
            int Prim_ID = 0;
            try
            {
                //Connect_.Db.BeginTrans();
                for (int i = 0; i <= 1; i++) {
                    foreach (RecordPtrDB item in recL){
                        if (item != null) {
                            object ident_loop = item.ident_loop;
                            UpdateRecordData(ref ID_Primary, OWNER_ID, TableName, item.LinkField, item.Value, ref ident_loop);
                            item.ident_loop = ident_loop;
                            Prim_ID = ID_Primary;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Connect_.Db.RollbackTrans();
            }
            return Prim_ID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        public List<RecordPtrDB> GetLinkData(string tableName, string fld)
        {
            string TableName2 = "";
            Dictionary<string, List<RecordPtrDB>> recDB_ = new Dictionary<string, List<RecordPtrDB>>();
            List<string> LstTable = new List<string>();
            TableName2 = tableName;
            string[] Spl = null;
            List<RecordPtrDB> recDB_Lst = new List<RecordPtrDB>();
            RecordPtrDB recDB = new RecordPtrDB();
            recDB.NameTableTo = tableName;

            if (fld.IndexOf(".") > 0) { Spl = fld.Split(new char[] { '.' }); }

            if (Spl != null)
            {
                for (int r = 0; r < Spl.Count(); r++)
                {

                    if (r < Spl.Count() - 1)
                    {
                        recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                        recDB.NameTableFrom = (r == 0 ? TableName2 : Spl[r - 1]);
                        Spl[r] = recDB.NameTableTo;
                        if (!LstTable.Contains(recDB.NameTableFrom))
                            LstTable.Add(recDB.NameTableFrom);

                        recDB.LinkField = fld;
                        recDB_Lst.Add(recDB);
                    }
                    else
                    {
                        recDB = new RecordPtrDB();
                        recDB.NameTableTo = recDB_Lst[recDB_Lst.Count() - 1].NameTableTo;
                        recDB.Name = recDB_Lst[recDB_Lst.Count() - 1].Name;
                        recDB.NameFieldForSetValue = Spl[r];
                        recDB.FieldJoinTo = Spl[r];

                        if (!LstTable.Contains(recDB.NameTableTo))
                            LstTable.Add(recDB.NameTableTo);

                        recDB.LinkField = fld;
                        recDB_Lst.Add(recDB);
                    }
                    tableName = GetTableFromORM(Spl[r], tableName).NameTableTo;
                }
            }
            else
            {
                recDB = new RecordPtrDB();
                recDB.NameTableTo = tableName;
                recDB.NameFieldForSetValue = fld;
                if (!LstTable.Contains(recDB.NameTableTo))
                    LstTable.Add(recDB.NameTableTo);

                recDB.LinkField = fld;
                recDB_Lst.Add(recDB);
            }
            return recDB_Lst;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool UpdateRecordData(ref int ID_Primary, int OWNER_ID, string TableName, string NameFld, object Value, ref object ident_loop)
        {
            bool isSuccessfull = true;
            List<RecordPtrDB> Lst = GetLinkData(TableName, NameFld);
            List<RecordPtrDB> OutLst = new List<RecordPtrDB>();
            int cnt = 0;
            int ID = -1;
            try
            {
                // Если OrmLinker пустой, тогда переинициализация
                if (lk_ == null) { inited = false; New(); }
                // собственно сам алгоритм создания/обновления данных 
                foreach (RecordPtrDB rec in Lst)
                {

                    if (cnt == 0)
                    {
                        if (Lst.Count() == 1)
                        {
                            if (rec.NameTableTo != null)
                            {
                                if (((rec.NameTableTo == "MICROWA") || (rec.NameTableTo == "NATFPLAN_SYST")) && (ident_loop == null))
                                {
                                    RecordPtrDB rv = new RecordPtrDB(); rv.FieldJoinTo = "ID"; rv.Name = ""; if (OutLst.Find(r => r.Name == rv.Name) == null) OutLst.Add(rv); object RetVal = null;
                                    bool isNeedCreate = true; if (ID_Primary != NullI) { isNeedCreate = isNeedCreateRecord(ID_Primary, TableName, GetPrimaryField(TableName), OutLst, out RetVal, 0); }
                                    if (isNeedCreate)
                                    {
                                        ID = SetF(rec.NameTableTo, new string[] { "" }, new object[] { "" }, new List<AllocIDs>(), OWNER_ID);
                                        ident_loop = ID;
                                        ID_Primary = ID;
                                        //--------------------------ТЕСт-------------------------
                                        OrmTable tb = OrmSchema.Table(rec.NameTableTo, false);
                                        OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                        OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                        
                                        OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                        if (fct_date != null)
                                        {
                                            object DATE_CREATED = GetOneFieldValue(rec.NameTableTo, ID, "DATE_CREATED");
                                            if (DATE_CREATED == null)
                                            {
                                                SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                            else
                                            {
                                                DateTime DTC = (DateTime)DATE_CREATED;
                                                if (DTC==NullT)
                                                    SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                        }
                                        
                                        if (fct1 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "OWNER_ID", OWNER_ID);
                                        else if (fct2 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "USER_ID", OWNER_ID);
                                        //--------------------------ТЕСт-------------------------
                                    }
                                    else
                                    {
                                        ID = ID_Primary;
                                        ident_loop = ID;
                                    }
                                }
                                else if (((rec.NameTableTo == "MICROWA") || (rec.NameTableTo == "NATFPLAN_SYST")) && (ident_loop != null))
                                {
                                    ID = (int)ident_loop;
                                }
                                else
                                {
                                    RecordPtrDB rv = new RecordPtrDB(); rv.FieldJoinTo = "ID"; rv.Name = ""; if (OutLst.Find(r => r.Name == rv.Name) == null) OutLst.Add(rv); object RetVal = null;
                                    bool isNeedCreate = true; if (ID_Primary != NullI) { isNeedCreate = isNeedCreateRecord(ID_Primary, TableName, GetPrimaryField(TableName), OutLst, out RetVal, 0); }
                                    if (isNeedCreate)
                                    {
                                        ID = SetF(rec.NameTableTo, new string[] { "" }, new object[] { "" }, new List<AllocIDs>(), OWNER_ID);
                                        ID_Primary = ID;
                                        //--------------------------ТЕСт-------------------------
                                        OrmTable tb = OrmSchema.Table(rec.NameTableTo, false);
                                        OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                        OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                        
                                        OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                        if (fct_date != null)
                                        {
                                            object DATE_CREATED = GetOneFieldValue(rec.NameTableTo, ID, "DATE_CREATED");
                                            if (DATE_CREATED == null)
                                            {
                                                SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                            else
                                            {
                                                DateTime DTC = (DateTime)DATE_CREATED;
                                                if (DTC==NullT)
                                                    SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                        }
                                        
                                        if (fct1 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "OWNER_ID", OWNER_ID);
                                        else if (fct2 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "USER_ID", OWNER_ID);
                                        //--------------------------ТЕСт-------------------------
                                    }
                                    else
                                    {
                                        ID = ID_Primary;
                                    }
                                }
                            }

                        }
                        else
                        {

                            if (rec.NameTableFrom != null)
                            {
                                if (((rec.NameTableFrom == "MICROWA") || (rec.NameTableFrom == "NATFPLAN_SYST")) && (ident_loop == null))
                                {
                                    RecordPtrDB rv = new RecordPtrDB(); rv.FieldJoinTo = "ID"; rv.Name = ""; if (OutLst.Find(r => r.Name == rv.Name) == null) OutLst.Add(rv); object RetVal = null;
                                    bool isNeedCreate = true; if (ID_Primary != NullI) { isNeedCreate = isNeedCreateRecord(ID_Primary, TableName, GetPrimaryField(TableName), OutLst, out RetVal, 0); }
                                    if (isNeedCreate)
                                    {
                                        ID = SetF(rec.NameTableFrom, new string[] { "" }, new object[] { "" }, new List<AllocIDs>(), OWNER_ID);
                                        ident_loop = ID;
                                        ID_Primary = ID;
                                        //--------------------------ТЕСт-------------------------
                                        OrmTable tb = OrmSchema.Table(rec.NameTableFrom, false);
                                        OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                        OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                        
                                        OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                        if (fct_date != null)
                                        {
                                            object DATE_CREATED = GetOneFieldValue(rec.NameTableFrom, ID, "DATE_CREATED");
                                            if (DATE_CREATED == null)
                                            {
                                                SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                            else
                                            {
                                                DateTime DTC = (DateTime)DATE_CREATED;
                                                if (DTC==NullT)
                                                    SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                        }
                                        
                                        if (fct1 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), ID), "OWNER_ID", OWNER_ID);
                                        else if (fct2 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), ID), "USER_ID", OWNER_ID);
                                        //--------------------------ТЕСт-------------------------
                                    }
                                    else
                                    {
                                        ID = ID_Primary;
                                        ident_loop = ID;
                                    }
                                }
                                else if (((rec.NameTableFrom == "MICROWA") || (rec.NameTableFrom == "NATFPLAN_SYST")) && (ident_loop != null))
                                {
                                    ID = (int)ident_loop;
                                }
                                else
                                {
                                    RecordPtrDB rv = new RecordPtrDB(); rv.FieldJoinTo = "ID"; rv.Name = ""; if (OutLst.Find(r => r.Name == rv.Name) == null) OutLst.Add(rv); object RetVal = null;
                                    bool isNeedCreate = true; if (ID_Primary != NullI) { isNeedCreate = isNeedCreateRecord(ID_Primary, TableName, GetPrimaryField(TableName), OutLst, out RetVal, 0); }
                                    if (isNeedCreate)
                                    {
                                        ID = SetF(rec.NameTableFrom, new string[] { "" }, new object[] { "" }, new List<AllocIDs>(), OWNER_ID);
                                        ID_Primary = ID;
                                        //--------------------------ТЕСт-------------------------
                                        OrmTable tb = OrmSchema.Table(rec.NameTableFrom, false);
                                        OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                        OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                        
                                        OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                        if (fct_date != null)
                                        {
                                            object DATE_CREATED = GetOneFieldValue(rec.NameTableFrom, ID, "DATE_CREATED");
                                            if (DATE_CREATED == null)
                                            {
                                                SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                            else
                                            {
                                                DateTime DTC = (DateTime)DATE_CREATED;
                                                if (DTC==NullT)
                                                    SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                        }
                                        
                                        if (fct1 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), ID), "OWNER_ID", OWNER_ID);
                                        else if (fct2 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), ID), "USER_ID", OWNER_ID);
                                        //--------------------------ТЕСт-------------------------
                                    }
                                    else
                                    {
                                        ID = ID_Primary;
                                    }

                                }
                            }
                            if (ID != -1)
                            {
                                rec.JoinFromIndex = ID;
                                List<AllocIDs> L_mx = new List<AllocIDs>(); AllocIDs k = new AllocIDs();
                                k.NameField = rec.FieldJoinTo; k.Value = ID; L_mx.Add(k);

                                if (rec.NameTableTo != null)
                                {

                                    RecordPtrDB rv = new RecordPtrDB(); rv.FieldJoinTo = rec.FieldJoinTo; rv.Name = rec.Name; OutLst.Add(rv); object RetVal = null;
                                    bool isNeedCreate = true; if (ID_Primary != NullI) { isNeedCreate = isNeedCreateRecord(ID_Primary, TableName, GetPrimaryField(rec.NameTableTo), OutLst, out RetVal, 0); }
                                    if (isNeedCreate)
                                    {
                                        rec.JoinToIndex = SetF(rec.NameTableTo, new string[] { "" }, new object[] { "" }, L_mx, OWNER_ID);
                                        //--------------------------ТЕСт-------------------------
                                        OrmTable tb = OrmSchema.Table(rec.NameTableTo, false);
                                        OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                        OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                        
                                        OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                        if (fct_date != null) {
                                            object DATE_CREATED = GetOneFieldValue(rec.NameTableTo, ID, "DATE_CREATED");
                                            if (DATE_CREATED == null)
                                            {
                                                SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                            else
                                            {
                                                DateTime DTC = (DateTime)DATE_CREATED;
                                                if (DTC==NullT)
                                                    SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                            }
                                        }
                                        
                                        if (fct1 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "OWNER_ID", OWNER_ID);
                                        else if (fct2 != null)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "USER_ID", OWNER_ID);
                                        //--------------------------ТЕСт-------------------------
                                    }
                                    else
                                    {
                                        rec.JoinToIndex = RetVal != null ? (int)RetVal : 0;
                                    }

                                }
                                if (rec.NameTableFrom != null)
                                {
                                    
                                    SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), rec.FieldJoinFrom, rec.JoinToIndex);
                                    //--------------------------ТЕСт-------------------------
                                    OrmTable tb = OrmSchema.Table(rec.NameTableFrom, false);
                                    OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                    OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                    
                                    OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                    if (fct_date != null)
                                    {
                                        object DATE_CREATED = GetOneFieldValue(rec.NameTableFrom, rec.JoinFromIndex, "DATE_CREATED");
                                        if (DATE_CREATED == null)
                                        {
                                            SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), "DATE_CREATED", DateTime.Now);
                                        }
                                        else
                                        {
                                            DateTime DTC = (DateTime)DATE_CREATED;
                                            if (DTC==NullT)
                                                SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), "DATE_CREATED", DateTime.Now);
                                        }
                                    }
                                    
                                    if (fct1 != null)
                                        SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), "OWNER_ID", OWNER_ID);
                                    else if (fct2 != null)
                                        SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), "USER_ID", OWNER_ID);
                                    //--------------------------ТЕСт-------------------------

                                    
                                }
                            }
                        }
                    }
                    else if ((cnt > 0) && (rec.NameFieldForSetValue == null))
                    {
                        RecordPtrDB R = Lst.Find(r => r.NameTableTo == rec.NameTableFrom);
                        if (R != null)
                        {
                            rec.JoinFromIndex = R.JoinToIndex;
                            if (rec.NameTableTo != null)
                            {
                                RecordPtrDB rv = new RecordPtrDB(); rv.FieldJoinTo = rec.FieldJoinTo; rv.Name = rec.Name; OutLst.Add(rv); object RetVal = null;
                                bool isNeedCreate = true; if (ID_Primary != NullI) { isNeedCreate = isNeedCreateRecord(ID_Primary, TableName, GetPrimaryField(rec.NameTableTo), OutLst, out RetVal, 0); }
                                if (isNeedCreate)
                                {
                                    ID = SetF(rec.NameTableTo, new string[] { "" }, new object[] { "" }, new List<AllocIDs>(), OWNER_ID);
                                    rec.JoinToIndex = ID;
                                    //--------------------------ТЕСт-------------------------
                                    OrmTable tb = OrmSchema.Table(rec.NameTableTo, false);
                                    OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                    OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                    
                                    OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                    if (fct_date != null)
                                    {
                                        object DATE_CREATED = GetOneFieldValue(rec.NameTableTo, ID, "DATE_CREATED");
                                        if (DATE_CREATED == null)
                                        {
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                        }
                                        else
                                        {
                                            DateTime DTC = (DateTime)DATE_CREATED;
                                            if (DTC==NullT)
                                                SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                        }
                                    }
                                     
                                    if (fct1 != null)
                                        SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "OWNER_ID", OWNER_ID);
                                    else if (fct2 != null)
                                        SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "USER_ID", OWNER_ID);
                                    //--------------------------ТЕСт-------------------------
                                }
                                else
                                {
                                    ID = RetVal != null ? (int)RetVal : 0;
                                    rec.JoinToIndex = RetVal != null ? (int)RetVal : 0;
                                }

                            }
                            if (rec.NameTableFrom != null)
                            {
                                SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), rec.FieldJoinFrom, rec.JoinToIndex);
                                //--------------------------ТЕСт-------------------------
                                OrmTable tb = OrmSchema.Table(rec.NameTableFrom, false);
                                OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                
                                OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                if (fct_date != null)
                                {
                                    object DATE_CREATED = GetOneFieldValue(rec.NameTableFrom, rec.JoinFromIndex, "DATE_CREATED");
                                    if (DATE_CREATED == null)
                                    {
                                        SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), "DATE_CREATED", DateTime.Now);
                                    }
                                    else
                                    {
                                        DateTime DTC = (DateTime)DATE_CREATED;
                                            if (DTC==NullT)
                                                SetFieldValue(rec.NameTableFrom, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), "DATE_CREATED", DateTime.Now);
                                    }
                                }
                                 
                                if (fct1 != null)
                                    SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), "OWNER_ID", OWNER_ID);
                                else if (fct2 != null)
                                    SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableFrom), rec.JoinFromIndex), "USER_ID", OWNER_ID);
                                //--------------------------ТЕСт-------------------------
                            }
                        }
                    }

                    if (rec.NameFieldForSetValue != null) {
                        if (Lst.Count() > 1) {
                            RecordPtrDB R = Lst.Find(r => r.NameTableTo == rec.NameTableTo && (r.NameFieldForSetValue == null));
                            if (R != null)
                            {
                                rec.JoinToIndex = R.JoinToIndex;
                                if (rec.NameTableTo != null) {
                                    if ((rec.NameTableTo == "USERS") && (rec.NameFieldForSetValue=="NAME")) {
                                        if (Value != null){
                                            string VB = Value.ToString();
                                            //if (!string.IsNullOrEmpty(VB))
                                                //SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), rec.JoinToIndex), rec.NameFieldForSetValue, Value);
                                        }
                                    }
                                    else  SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), rec.JoinToIndex), rec.NameFieldForSetValue, Value);
                                    //--------------------------ТЕСт-------------------------
                                    OrmTable tb = OrmSchema.Table(rec.NameTableTo, false);
                                    OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                    OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                    
                                    OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                    if (fct_date != null)
                                    {
                                        object DATE_CREATED = GetOneFieldValue(rec.NameTableTo, rec.JoinToIndex, "DATE_CREATED");
                                        if (DATE_CREATED == null)
                                        {
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), rec.JoinToIndex), "DATE_CREATED", DateTime.Now);
                                        }
                                        else
                                        {
                                            DateTime DTC = (DateTime)DATE_CREATED;
                                            if (DTC==NullT)
                                                SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), rec.JoinToIndex), "DATE_CREATED", DateTime.Now);
                                        }
                                    }
                                     
                                    if (fct1 != null)
                                        SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), rec.JoinToIndex), "OWNER_ID", OWNER_ID);
                                    else if (fct2 != null)
                                        SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), rec.JoinToIndex), "USER_ID", OWNER_ID);
                                    //--------------------------ТЕСт-------------------------
                                }
                            }
                        }
                        else if (Lst.Count() == 1) {
                            if (rec.NameTableTo != null)
                            {
                                    if ((rec.NameTableTo == "USERS") && (rec.NameFieldForSetValue=="NAME")) {
                                        if (Value != null){
                                            string VB = Value.ToString();
                                            //if (!string.IsNullOrEmpty(VB))
                                                //SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), rec.NameFieldForSetValue, Value);
                                        }
                                    }
                                    else
                                        SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), rec.NameFieldForSetValue, Value);
                                //--------------------------ТЕСт-------------------------
                                OrmTable tb = OrmSchema.Table(rec.NameTableTo, false);
                                OrmField fct1 = tb.Fields.ToList().Find(r => r.Name == "OWNER_ID");
                                OrmField fct2 = tb.Fields.ToList().Find(r => r.Name == "USER_ID");
                                
                                OrmField fct_date = tb.Fields.ToList().Find(r => r.Name == "DATE_CREATED");
                                if (fct_date != null)
                                {
                                    object DATE_CREATED = GetOneFieldValue(rec.NameTableTo, ID, "DATE_CREATED");
                                    if (DATE_CREATED == null) {
                                        SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                    }
                                    else {
                                        DateTime DTC = (DateTime)DATE_CREATED;
                                        if (DTC==NullT)
                                            SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "DATE_CREATED", DateTime.Now);
                                    }
                                }
                                
                                if (fct1 != null)
                                    SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "OWNER_ID", OWNER_ID);
                                else if (fct2 != null)
                                    SetFieldValue(rec.NameTableTo, string.Format("[{0}]={1}", GetPrimaryField(rec.NameTableTo), ID), "USER_ID", OWNER_ID);
                                //--------------------------ТЕСт-------------------------
                            }
                        }

                    }


                    cnt++;
                }
            }
            catch (Exception ex)
            {
                isSuccessfull = false;
            }

            return isSuccessfull;
        }


        /// <summary>
        /// 
        /// </summary>
        public int SetF(string tab, string[] fld, object[] values, List<AllocIDs> L_m, int OWNER_ID)
        {
            string F_ = GetPrimaryField(tab);
            int MaxID = GetMaxID(F_, tab);
            MaxID = ((MaxID <= 0) || (MaxID == NullI)) ? 1 : MaxID + 1;
            List<string> Lf = new List<string>();
            List<string> Lf_Out = new List<string>();
            Lf.AddRange(GetNotNullFields(tab));
            Lf.AddRange(fld.ToList());
            List<int> index_Nq_Out = new List<int>(); List<object> L_obj_Out = new List<object>();
            OrmTable tb = OrmSchema.Table(tab, false);
            ANetNQ nq = null; int[] indexNq = new int[Lf.Count()];
            bool[] notNull = new bool[Lf.Count()];
            OrmDataDesc[] desc = new OrmDataDesc[Lf.Count()];
            object[] L_obj = new object[Lf.Count()];
            string cmd1 = ""; string cmd2 = ""; string cmd3 = ""; string com = "";
            int nqCount = -1; int nqCount_ = 0;
            for (int i = 0; i < Lf.Count(); i++)
            {
                OrmField fd = tb.Field(Lf[i]);
                if (fd != null && fd is OrmFieldF && fd.IsCompiledInEdition)
                {
                    indexNq[i] = ++nqCount;
                    OrmFieldF f = (OrmFieldF)fd;
                    notNull[i] = (f.Options & OrmFieldFOption.fld_NOTNULL) == OrmFieldFOption.fld_NOTNULL;
                    desc[i] = f.DDesc;
                    if ((f.Options == (OrmFieldFOption.fld_PRIMARY | OrmFieldFOption.fld_NOTNULL | OrmFieldFOption.fld_FKEY)) || (f.Options == (OrmFieldFOption.fld_PRIMARY | OrmFieldFOption.fld_NOTNULL))) { L_obj[i] = MaxID; }
                    else
                    {
                        if (!fld.Contains(Lf[i]))
                        {
                            if (f.DDesc.ClassType == OrmVarType.var_Int) L_obj[i] = 0;
                            if (f.DDesc.ClassType == OrmVarType.var_Dou) L_obj[i] = 0.0;
                            if (f.DDesc.ClassType == OrmVarType.var_Flo) L_obj[i] = 0.0;
                            if (f.DDesc.ClassType == OrmVarType.var_String) L_obj[i] = "";
                            if (f.DDesc.ClassType == OrmVarType.var_Null) L_obj[i] = null;
                            AllocIDs O_ = L_m.Find(r => r.NameField == Lf[i]);
                            if (O_ != null) { L_obj[i] = O_.Value; }
                            if ((f.Options == OrmFieldFOption.fld_FKEY) && (O_ == null)) indexNq[i] = -1;
                            if (("OWNER_ID" == Lf[i]) || ("OWNER_ID".ToLower() == Lf[i].ToLower()) || ("USER_ID".ToLower() == Lf[i].ToLower()) || ("USER_ID" == Lf[i])) L_obj[i] = OWNER_ID;

                        }
                        else
                        {
                            int idx = 0;
                            for (int k = 0; k < fld.Count(); k++)
                            {
                                if (Lf[i] == fld[k])
                                {
                                    idx = k;
                                    break;
                                }
                            }
                            L_obj[i] = values[idx];

                            AllocIDs O_ = L_m.Find(r => r.NameField == Lf[i]);
                            if (O_ != null) L_obj[i] = O_.Value;
                            //if (("OWNER_ID" == Lf[i]) || ("USER_ID" == Lf[i])) L_obj[i] = OWNER_ID;
                            if (("OWNER_ID" == Lf[i]) || ("OWNER_ID".ToLower() == Lf[i].ToLower()) || ("USER_ID".ToLower() == Lf[i].ToLower()) || ("USER_ID" == Lf[i])) L_obj[i] = OWNER_ID;
                        }
                    }

                    if (indexNq[i] >= 0)
                    {
                        Lf_Out.Add(Lf[i]);
                        index_Nq_Out.Add(nqCount_);
                        L_obj_Out.Add(L_obj[i]);
                        cmd1 += com + "{" + Lf[i] + "}";
                        cmd2 += com + "@p" + nqCount_.ToString();
                        cmd3 += com + fd.DDesc.ParamType;
                        com = ",";
                        nqCount_++;
                    }
                }
                else
                {

                    indexNq[i] = -1;
                }
            }
            bool isNewRecord = true;
            // сюда внесены корректировки
            // Если есть возможность использовать существующий OWNER_ID - значит так и делать
            List<AllocIDs> O_GL = L_m.FindAll(r => r.NameField == "ID");
            if (((O_GL != null) && (tab == "USERS")))
            {
                if (O_GL.Count == 1)
                {
                    isNewRecord = false;
                    MaxID = OWNER_ID;
                }
            }
            if ((L_m.Count == 0) && (tab == "USERS"))
            {
                isNewRecord = false;
                MaxID = OWNER_ID;
            }
            // конец корректировок
            if (isNewRecord)
            {
                string sql; ANetDb d;
                lk_.PrepareExecute("INSERT INTO %{0} ({1}) VALUES ({2})".Fmt(tab, cmd1, cmd2), out sql, out d);
                nq = d.NewAdapter();
                nq.Init(sql, cmd3);
                for (int i = 0; i < Lf_Out.Count(); i++)
                {
                    object o = L_obj_Out[i];
                    if (index_Nq_Out[i] >= 0)
                    {
                        if ((Lf_Out[i] == "CODE") || (Lf_Out[i] == "COD"))
                        {
                            o = Guid.NewGuid().ToString().Substring(0, 8);
                        }

                        nq.SetParamValue(index_Nq_Out[i], o);

                        if (tab == "MICROWS")
                        {
                            AllocIDs O_ = L_m.Find(r => r.NameField == "MW_ID");
                            if (O_ != null)
                            {
                                //int cnt_A = OrmSchema.Linker.ExecuteScalarInt("SELECT COUNT(1) FROM %" + tab + " WHERE MW_ID=" + O_.Value + " AND ROLE='A'");
                                int cnt_A = lk_.ExecuteScalarInt("SELECT COUNT(1) FROM %" + tab + " WHERE MW_ID=" + O_.Value + " AND ROLE='A'");
                                if (cnt_A == 0)
                                {
                                    if (Lf_Out[i] == "ROLE") nq.SetParamValue(index_Nq_Out[i], "A");
                                    if (Lf_Out[i] == "END_ROLE") nq.SetParamValue(index_Nq_Out[i], "B");
                                }
                                else if (cnt_A > 0)
                                {
                                    if (Lf_Out[i] == "ROLE") nq.SetParamValue(index_Nq_Out[i], "B");
                                    if (Lf_Out[i] == "END_ROLE") nq.SetParamValue(index_Nq_Out[i], "A");
                                }
                                else nq.SetParamValue(index_Nq_Out[i], o);
                            }
                            else nq.SetParamValue(index_Nq_Out[i], o);
                        }
                        else if (tab == "NATFPLAN_BAND")
                        {
                            AllocIDs O_ = L_m.Find(r => r.NameField == "NFPS_ID");
                            if (O_ != null)
                            {
                                //int cnt_A = OrmSchema.Linker.ExecuteScalarInt("SELECT COUNT(1) FROM %" + tab + " WHERE NFPS_ID=" + O_.Value + " AND ROLE='A'");
                                int cnt_A = lk_.ExecuteScalarInt("SELECT COUNT(1) FROM %" + tab + " WHERE NFPS_ID=" + O_.Value + " AND ROLE='A'");
                                if (cnt_A == 0)
                                {
                                    if (Lf_Out[i] == "ROLE") nq.SetParamValue(index_Nq_Out[i], "A");
                                }
                                else if (cnt_A > 0)
                                {
                                    if (Lf_Out[i] == "ROLE") nq.SetParamValue(index_Nq_Out[i], "B");
                                }
                                else nq.SetParamValue(index_Nq_Out[i], o);
                            }
                            else nq.SetParamValue(index_Nq_Out[i], o);
                        }
                        else
                            nq.SetParamValue(index_Nq_Out[i], o);
                    }
                }
                int nbRowsUpdated = nq.Execute();
            }

            return MaxID;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class AllocIDs
    {
        public string NameField { get; set; }
        public object Value { get; set; }
    }

    public enum ModeCompareLicence
    {
        NotFoundLic_id,
        NotEqually,
        Equally
    }

}
