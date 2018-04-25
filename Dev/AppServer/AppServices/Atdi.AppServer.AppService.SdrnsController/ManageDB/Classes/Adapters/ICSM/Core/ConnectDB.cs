using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using OrmCs;
using DatalayerCs;
using System.IO;
using XMLLibrary;
using System.Windows.Forms;
using CoreICSM.Logs;
using FormsCs;
using System.Web;
using System.Web.UI;



namespace DAL
{

    [Serializable]
    public enum TypeDb
    {
        ORACLE,
        MSSQL,
        UNKNOWN
    }
    [Serializable]
    public enum enumRulesAccess
    {
        Insert,
        Update,
        Delete,
        Select
    }

    [Serializable]
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


        
    }

    [Serializable]
    public class RulesAccess
    {
        private bool insert;
        private bool update;
        private bool delete;
        private bool select;

        public bool Insert
        {
            set { this.insert = value; }
            get { return this.insert; }
        }

        public bool Update
        {
            set { this.update = value; }
            get { return this.update; }
        }

        public bool Delete
        {
            set { this.delete = value; }
            get { return this.delete; }
        }

        public bool Select
        {
            set { this.select = value; }
            get { return this.select; }
        }

        public RulesAccess()
        {
            this.Insert = false;
            this.Update = false;
            this.Delete = false;
            this.Select = false;
        }

        public RulesAccess(bool insert, bool update, bool delete, bool select)
        {
            this.Insert = insert;
            this.Update = update;
            this.Delete = delete;
            this.Select = select;
        }

    }

    [Serializable]
    public class RulesAccessTable
    {
        private string nameTable;
        private RulesAccess rls;

        public string NameTable
        {
            set { this.nameTable = value; }
            get { return this.nameTable; }
        }

        public RulesAccess Rls
        {
            set { this.rls = value; }
            get { return this.rls; }
        }

        public RulesAccessTable() { }

        public RulesAccessTable(string nameTable)
        {
            this.NameTable = nameTable;
            this.Rls = null;
        }

        public RulesAccessTable(string nameTable, RulesAccess rls)
        {
            this.NameTable = nameTable;
            this.Rls = rls;
        }

        public bool isRulesEmpty()
        {
            bool tmp = false;
            if (this.Rls == null)
            // if ((this.Rls.Delete == false) &&  (this.Rls.Insert == false) && (this.Rls.Select == false) && (this.Rls.Update == false))
            {
                tmp = true;
            }
            return tmp;
        }

        public bool HasRules(RulesAccess Rls)
        {
            bool tmp = false;
            if (this.Rls.Delete == Rls.Delete == true)
            {
                tmp = true;
            }
            if (this.Rls.Insert == Rls.Insert == true)
            {
                tmp = true;
            }
            if (this.Rls.Select == Rls.Select == true)
            {
                tmp = true;
            }
            if (this.Rls.Update == Rls.Update == true)
            {
                tmp = true;
            }
            return tmp;
        }

        public void InsertRules(RulesAccess Rls)
        {

            if (Rls.Delete == true)
            {
                this.Rls.Delete = true;
            }
            if (Rls.Insert == true)
            {
                this.Rls.Insert = true;
            }
            if (Rls.Select == true)
            {
                this.Rls.Select = true;
            }
            if (Rls.Update == true)
            {
                this.Rls.Update = true;
            }
        }
    }

    /// <summary>
    /// Класс, обеспечивающий соединение с базой данных,
    /// начальную инициализацию, обмен данными с таблицами СУБД
    /// </summary>
    [Serializable]
    public class ConnectDB :IDisposable
    {
        public static int NullI = 2147483647;
        public static double NullD = 1E-99;
        public static DateTime NullT = new DateTime(1, 1, 1, 0, 0, 0);

        public ConnectDB()
        {
            try
            {
                NewMain();
                //NewSecondary();
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// Деструктор.
        /// </summary>
          ~ConnectDB()
          {
                Dispose();
          }

        #region IDisposable Members

        		public void Dispose() {
        		GC.SuppressFinalize(this);
        }

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
        static public bool inited_main = false;
        static public ANetDb dbx_main = null;
        static public bool inited_secondary = false;
        static public ANetDb dbx_secondary = null;
        static public string SchemaDbx = "";
        static public string PathDbx = "";
        static public XMLObj Settings;


        /// <summary>
        /// Метод, выполняющий начальную инициализацию (открывает соединение с СУБД)
        /// </summary>
        /// <param name="db"> БД </param>
        /// <param name="schema"> ORM - схема</param>
        /// <param name="pathtoapp"> Путь размещения схемы </param>
        static internal void OpenMainConnect(out ANetDb db, out string schema, out string pathtoapp)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting)
            {
                XMLObj xml_out = new XMLObj();
                xml_out = BaseXMLDirect.GetXmlSettings(BaseXMLDirect.file_name_current);
                Settings = xml_out;
                // Получить настройки для отправки почтовых сообщений
                SettingNotification = BaseXMLDirect.GetSettingEmailNotification();
                // Получить настройки по перечню радиотехнологий
                SettingRadioTech = BaseXMLDirect.GetSettingRadioTech();


                pathtoapp = BaseXMLDirect.pathtoapp;
                schema = xml_out._ORMSchema;
                active_schema = schema.Remove(schema.Length - 1, 1).Trim();
                CurrentTypeDB = ConvertToTypeDb(xml_out._TypeRDBMS);

                db = ANetDb.New(xml_out._NameClientLibrary);
                db.ConnectionString = xml_out._MainConnectionString;
                try
                {
                    db.Open();
                }
                catch (Exception e)
                {
                    //throw new Exception("Invalid execute method Init (class ConnectDB):" + e.Message);
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute method OpenConnect (class ConnectDB): " + e.Message);
                }
            }
        }

        static internal void OpenSecondaryConnect(out ANetDb db, out string schema, out string pathtoapp)
        {
            string XmlSetting = "XmlSetting";
            lock (XmlSetting)
            {
                XMLObj xml_out = new XMLObj();
                xml_out = BaseXMLDirect.GetXmlSettings(BaseXMLDirect.file_name_current);
                Settings = xml_out;
                // Получить настройки для отправки почтовых сообщений
                SettingNotification = BaseXMLDirect.GetSettingEmailNotification();
                // Получить настройки по перечню радиотехнологий
                SettingRadioTech = BaseXMLDirect.GetSettingRadioTech();


                pathtoapp = BaseXMLDirect.pathtoapp;
                schema = xml_out._ORMSchema;
                active_schema = schema.Remove(schema.Length - 1, 1).Trim();
                CurrentTypeDB = ConvertToTypeDb(xml_out._TypeRDBMS);

                db = ANetDb.New(xml_out._NameClientLibrary);
                db.ConnectionString = xml_out._SecondaryConnectionString;
                try
                {
                    db.Open();
                }
                catch (Exception e)
                {
                    //throw new Exception("Invalid execute method Init (class ConnectDB):" + e.Message);
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute method OpenConnect (class ConnectDB): " + e.Message);
                }
            }
        }


        static List<string> TableNames_Schema = null;     // Общий список доступных таблиц СУБД
        static string All_TableNames_Schema = "";     // Общий список доступных таблиц СУБД
        static List<RulesAccessTable> List_TableNames_Schema = null;


        static string initializingLock_main = "InitializingMain";
        static string initializingLock_Secondary = "InitializingSecondary";
        static string active_schema = "";
        static OrmLinker lk_main_;
        static OrmLinker lk_secondary_;
        static public DbLinker Connect_Main_ = null;
        static public DbLinker Connect_Secondary_ = null;
        static public List<TemplateLetter> SettingNotification = null;
        static public List<RadioTechSettings> SettingRadioTech = null;


        /// <summary>
        /// Метод, реализующий инициализацию и извлечение сведений о правах доступа пользователей к объектам СУБД
        /// </summary>
        static public void InitMain()
        {

            lock (initializingLock_main)
            {
                if (!inited_main)
                {
                    try
                    {
                        string outErr = ""; 
                        TableNames_Schema = new List<string>();
                        OpenMainConnect(out dbx_main, out SchemaDbx, out PathDbx);
                        if (!OrmCs.OrmSchema.ParseSchema(PathDbx, "SensorPlugin", "XICSM_SensorPlugin", out outErr)) { CoreICSM.Logs.CLogs.WriteError(ELogsWhat.DataDriver, "[InitMain]: "+ outErr); }
                        OrmSchema.LanguagePreferences = new string[] { "CST", "SPA", "ENG", "RUS" };
                        OrmSchema.InitIcsmSchema(dbx_main, SchemaDbx, PathDbx);

                        lk_main_ = OrmSchema.Linker;


                        /* Механизм, реализующий выборку всего доступного пользователям перечня табдиц БД*/
                        /* Данный механизм по разному работает для разных СУБД*/
                        string sql_table_name = "";
                        if (CurrentTypeDB == TypeDb.MSSQL) { sql_table_name = "SELECT DISTINCT T.NAME  FROM SYS.DATABASE_PERMISSIONS R,SYS.OBJECTS T WHERE (R.MAJOR_ID= T.OBJECT_ID) AND (T.TYPE IN ('U','V')) "; }
                        if (CurrentTypeDB == TypeDb.ORACLE) { sql_table_name = "SELECT TABLE_NAME  FROM SYS.ALL_ALL_TABLES where OWNER=" + "'" + SchemaDbx.Remove(SchemaDbx.Count() - 1, 1) + "'"; }


                        if (!string.IsNullOrEmpty(sql_table_name))
                        {
                            //ANetRs rs = db.NewRecordset();
                            ANetRs rs = dbx_main.NewRecordset();
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
                        dbx_main.Destroy();
                    }
                    catch (Exception e)
                    {
                        CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute method Init (class ConnectDB): " + e.Message);
                    }
                    inited_main = true;
                }
            }
        }

        static public void InitSecondary()
        {

            lock (initializingLock_Secondary)
            {
                if (!inited_secondary)
                {
                    try
                    {
                        string outErr = "";
                        TableNames_Schema = new List<string>();
                        OpenSecondaryConnect(out dbx_secondary, out SchemaDbx, out PathDbx);
                        if (!OrmCs.OrmSchema.ParseSchema(PathDbx, "SensorPlugin", "XICSM_SensorPlugin", out outErr)) { CoreICSM.Logs.CLogs.WriteError(ELogsWhat.DataDriver, "[InitSecondary]: " + outErr); }
                        OrmSchema.LanguagePreferences = new string[] { "CST", "SPA", "ENG", "RUS" };
                        OrmSchema.InitIcsmSchema(dbx_secondary, SchemaDbx, PathDbx);
                        lk_secondary_ = OrmSchema.Linker;


                        /* Механизм, реализующий выборку всего доступного пользователям перечня табдиц БД*/
                        /* Данный механизм по разному работает для разных СУБД*/
                        string sql_table_name = "";
                        if (CurrentTypeDB == TypeDb.MSSQL) { sql_table_name = "SELECT DISTINCT T.NAME  FROM SYS.DATABASE_PERMISSIONS R,SYS.OBJECTS T WHERE (R.MAJOR_ID= T.OBJECT_ID) AND (T.TYPE IN ('U','V')) "; }
                        if (CurrentTypeDB == TypeDb.ORACLE) { sql_table_name = "SELECT TABLE_NAME  FROM SYS.ALL_ALL_TABLES where OWNER=" + "'" + SchemaDbx.Remove(SchemaDbx.Count() - 1, 1) + "'"; }


                        if (!string.IsNullOrEmpty(sql_table_name))
                        {
                            //ANetRs rs = db.NewRecordset();
                            ANetRs rs = dbx_secondary.NewRecordset();
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
                        dbx_secondary.Destroy();
                    }
                    catch (Exception e)
                    {
                        CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute method Init (class ConnectDB): " + e.Message);
                    }
                    inited_secondary = true;
                }
            }
        }

        /// <summary>
        /// Метод, возвращающий доступ, посредством класса DbLinker к возможностям взаимодействия с объектами СУБД  
        /// </summary>
        /// <returns>Обїект DbLinker</returns>
        static public DbLinker NewMain()
        {
            if (!inited_main)
            {
                InitMain();
                OpenMainConnect(out dbx_main, out SchemaDbx, out PathDbx);
                Connect_Main_ = new DbLinker(dbx_main, SchemaDbx); //this will initialize OrmSchema.Linker
                Connect_Main_.ClientID = "ICSMServiceID_Main";
            }
            else
            {
                //OpenConnect(out dbx, out SchemaDbx, out PathDbx);
                //Connect_Main_= new DbLinker(dbx, SchemaDbx); //this will initialize OrmSchema.Linker
                //Connect_Main_.ClientID = "ICSMServiceID";
            }
            return Connect_Main_;
        }

        static public DbLinker NewSecondary()
        {
            if (!inited_secondary)
            {
                InitSecondary();
                OpenSecondaryConnect(out dbx_secondary, out SchemaDbx, out PathDbx);
                Connect_Secondary_ = new DbLinker(dbx_secondary, SchemaDbx); //this will initialize OrmSchema.Linker
                Connect_Secondary_.ClientID = "ICSMServiceID_Secondary";
            }
            else
            {
                //OpenConnect(out dbx, out SchemaDbx, out PathDbx);
                //Connect_Main_= new DbLinker(dbx, SchemaDbx); //this will initialize OrmSchema.Linker
                //Connect_Main_.ClientID = "ICSMServiceID";
            }
            return Connect_Secondary_;
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


        public object GetAuthUserID2(string Code, string Name)
        {
            object ID = -1;
            try
            {
                ConnectDB connect = new ConnectDB();

                string Filter = "[{0}] = {1}".Fmt("REGIST_NUM", ToSql(Code));
                List<object> lst = connect.GetFieldValue("USERS", Filter, "ID");
                if (lst != null)
                {
                    if (lst.Count == 1)
                    {
                        ID = lst[0];
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Code))
                        ID = connect.NewRecord("USERS", new string[] { "ID", "CODE", "NAME", "REGIST_NUM", "CUST_TXT3" }, new object[] { (GetMaxID("ID", "USERS") != 2147483647 ? GetMaxID("ID", "USERS") + 1 : 1), Guid.NewGuid().ToString().Substring(0, 8), Name, Code, "Guest" }, "ID", Connect_Main_); 
                }
            }
            catch
            {

            }
            return ID;
        }

        /// <summary>
        /// Проверка по таблице пользователей наличия дубликата Логина
        /// </summary>
        /// <param name="Login"></param>
        /// <returns></returns>
        public bool CheckDublicateLoginUser(string Login)
        {
            object ID = -1;
            bool isDublicate = false;
            try
            {
                ConnectDB connect = new ConnectDB();

                string Filter = "[{0}] = {1}".Fmt("WEB_LOGIN", ToSql(Login));
                List<object> lst = connect.GetFieldValue("USERS", Filter, "ID");
                if (lst != null)
                {
                    if (lst.Count == 1)
                    {
                        ID = lst[0];
                        if ((int)ID > -1)
                        {
                            isDublicate = true;
                        }
                    }
                }
            }
            catch
            {

            }

            return isDublicate;
        }

        /// <summary>
        /// Проверка по таблице пользователей наличия дубликата электронной почты
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public bool CheckDublicateEmailUser(string Email)
        {
            object ID = -1;
            bool isDublicate = false;
            try
            {
                ConnectDB connect = new ConnectDB();
                string Filter = "[{0}] = {1}".Fmt("EMAIL", ToSql(Email));
                List<object> lst = connect.GetFieldValue("USERS", Filter, "ID");
                if (lst != null)
                {
                    if (lst.Count == 1)
                    {
                        ID = lst[0];
                        if ((int)ID > -1)
                        {
                            isDublicate = true;
                        }
                    }
                }
            }
            catch
            {

            }

            return isDublicate;
        }


        /// <summary>
        /// Извлечение прав пользователя на объекты БД
        /// </summary>
        /// <param name="db">БД</param>
        /// <param name="User">Пользователь БД</param>
        /// <returns>список объектов List<RulesAccessTable></returns>
        static public List<RulesAccessTable> GetTablePrivilegiesUser(ANetDb db, string User)
        {
            List_TableNames_Schema = new List<RulesAccessTable>();
            for (int i = 0; i < TableNames_Schema.Count; i++) List_TableNames_Schema.Add(new RulesAccessTable(TableNames_Schema[i].ToString()));

            string sql_table_name = "";
            if (CurrentTypeDB == TypeDb.MSSQL) { sql_table_name = "select t.name, r.permission_name from sys.database_permissions r left join sys.database_principals u on r.grantee_principal_id=u.principal_id left join sys.objects t on r.major_id= t.object_id left join sys.schemas ps on ps.schema_id=t.schema_id where r.class=1 and t.type in ('U','V')  and r.permission_name in ('SELECT','INSERT','UPDATE','DELETE')  and t.name in (" + All_TableNames_Schema + ") and u.name in IN ('" + User + "','" + User + "__')"; }
            if (CurrentTypeDB == TypeDb.ORACLE) { sql_table_name = "SELECT TABLE_NAME,PRIVILEGE FROM SYS.DBA_TAB_PRIVS where OWNER='" + active_schema + "' AND GRANTEE IN ('" + User + "','" + User + "__') AND TABLE_NAME IN (" + All_TableNames_Schema + ")"; }

            if (!string.IsNullOrEmpty(sql_table_name))
            {
                string err = "";
                try
                {
                    ANetRs rs = db.NewRecordset();
                    rs.Open(sql_table_name);
                    err = rs.SqlOrder;
                    for (; !rs.IsEOF(); rs.MoveNext())
                    {
                        if (CurrentTypeDB == TypeDb.ORACLE)
                        {
                            string tb = rs.GetString(0);

                            RulesAccess rc = new RulesAccess();
                            switch (rs.GetString(1))
                            {
                                case "SELECT": rc.Select = true; break;
                                case "INSERT": rc.Insert = true; break;
                                case "UPDATE": rc.Update = true; break;
                                case "DELETE": rc.Delete = true; break;
                                default: throw new Exception("queryRights failed");
                            }

                            for (int i = 0; i < TableNames_Schema.Count(); i++)
                            {
                                if (List_TableNames_Schema[i].NameTable == tb)
                                {
                                    if (List_TableNames_Schema[i].isRulesEmpty()) { List_TableNames_Schema[i].Rls = rc; }
                                    else
                                    {
                                        List_TableNames_Schema[i].InsertRules(rc);
                                    }
                                    break;
                                }
                            }
                        }

                    }
                    rs.Destroy();
                }
                catch (Exception ex)
                {
                    //throw new Exception("Invalid execute GetTablePrivilegiesUser:" + ex.Message);
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute GetTablePrivilegiesUser: " + ex.Message);
                }
            }
            //return PrivilegiesUser;
            return List_TableNames_Schema;
        }

        /// <summary>
        /// Получить все роли пользователей
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public string[] GetAllUserRoles(string TableName, string NameField)
        {
            List<string> List_User_Roles = new List<string>();
            string sql_ = "select " + NameField + " from " + TableName;

            if (!string.IsNullOrEmpty(sql_))
            {
                string err = "";
                try
                {
                    //using (DbLinker lk = ConnectDB.New())
                    if (Connect_Main_!= null)
                    {
                        ANetRs rs = Connect_Main_.Db.NewRecordset();
                        rs.Open(sql_);
                        err = rs.SqlOrder;
                        for (; !rs.IsEOF(); rs.MoveNext())
                        {
                            string tb = rs.GetString(0);
                            List_User_Roles.Add(tb);
                        }
                        rs.Destroy();
                    }
                }
                catch (Exception ex)
                {
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute GetAllUserRoles: " + ex.Message);
                }
            }
            return List_User_Roles.ToArray();
        }

        /// <summary>
        ///  Поиск значения заданного поля в заданной таблице при условии, что FindField = ValueField
        ///  Метод необходим для поиска роли, которая назначена пользователю
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="SelectField"></param>
        /// <param name="FindField"></param>
        /// <param name="ValueField"></param>
        /// <returns></returns>
        public List<object[]> FindUserRole(string TableName, string[] fld, object[] values, bool[] CompareValue)
        {
            List<object[]> Res_Lst = new List<object[]>();
            object[] User_Roles;
            if (fld.Count() > 0)
            {
                User_Roles = new object[fld.Count()];


                string Res_str = "";
                for (int i = 0; i < fld.Count(); i++)
                {
                    if (i < fld.Count() - 1)
                    {
                        Res_str += fld[i].Replace("'", "\"") + ",";
                    }
                    else
                    {
                        Res_str += fld[i].Replace("'", "\"");
                    }
                }

                if (!string.IsNullOrEmpty(GenerateFormat(CompareValue, fld, values, false)))
                {

                    string sql_ = string.Format("select {0}  from {1} where {2}", Res_str, TableName, GenerateFormat(CompareValue, fld, values, false));


                    if (!string.IsNullOrEmpty(sql_))
                    {
                        string err = "";
                        try
                        {
                            //using (DbLinker lk = ConnectDB.New())
                            if (Connect_Main_!= null)
                            {
                                ANetRs rs = Connect_Main_.Db.NewRecordset();
                                rs.Open(sql_);
                                err = rs.SqlOrder;
                                for (; !rs.IsEOF(); rs.MoveNext())
                                {
                                    User_Roles = new object[fld.Count()];
                                    for (int j = 0; j < fld.Count(); j++)
                                    {
                                        User_Roles[j] = rs.GetString(j);
                                    }
                                    Res_Lst.Add(User_Roles);
                                }
                                rs.Destroy();
                            }
                        }
                        catch (Exception ex)
                        {
                            CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute FindUserRole: " + ex.Message);
                        }
                    }
                }
            }
            return Res_Lst;
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
            List<object[]> n_object = new List<object[]>();
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {


                    n_object = new List<dynamic[]>();
                    n_object = OrmSourcer.ICSMQuery(tbl_mass, lst_mass, filter, order, maxRec, Connect_Main_);
                    if (n_object != null)
                    {
                        isSelectedData = true;
                    }
                }

            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in ExecuteSQL: " + ex.Message);
            }

            return n_object;
        }


        /// <summary>
        /// Метод, выполняющий запрос к базе данных посредством SQL - языка
        /// </summary>
        /// <param name="sql_"></param>
        /// <returns></returns>
        public static List<object[]> ExecuteSQLCommand(ref List<string> Fld, string sql_)
        {
            List<object[]> RecordValues = new List<object[]>();
            List<object> RecordsVal = null;
            List<string> fld_f = new List<string>();

            try
            {
                if (!string.IsNullOrEmpty(sql_))
                {
                    //using (DbLinker lk = ConnectDB.New())
                    if (Connect_Main_!= null)
                    {
                        ANetRs rs = Connect_Main_.Db.NewRecordset();
                        rs.Open(sql_);
                        System.Data.DataTable dft = rs.GetSchemaTable();
                        foreach (System.Data.DataRow c in dft.Rows)
                        {
                            fld_f.Add(c[0].ToString());
                        }



                        if (fld_f.Count > 0)
                        {
                            Fld = fld_f;

                            for (; !rs.IsEOF(); rs.MoveNext())
                            {
                                RecordsVal = new List<object>();
                                for (int i = 0; i < fld_f.Count(); i++)
                                {
                                    object value=-1;
                                    string val = rs.GetString(i);
                                    double d_vl;
                                    int i_vl;
                                    DateTime date_vl;

                                    bool isCheck = false;
                                    if (val != null)
                                    {
                                        if (double.TryParse(val.ToString(), out d_vl))
                                        {
                                            value = d_vl;
                                            isCheck = true;
                                        }
                                        else if (int.TryParse(val.ToString(), out i_vl))
                                        {
                                            value = i_vl;
                                            isCheck = true;
                                        }
                                        else if (DateTime.TryParse(val.ToString(), out date_vl))
                                        {
                                            value = date_vl;
                                            isCheck = true;
                                        }
                                        else
                                        {
                                            value = val;
                                            isCheck = true;
                                        }
                                        
                                    }
                                    else if ((fld_f[i]!="ID") && (!isCheck)) value = "";

                                    RecordsVal.Add(value);
                                }


                                RecordValues.Add(RecordsVal.ToArray());


                            }


                        }
                        rs.Destroy();
                    }
                }
                
            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
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
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    if (!string.IsNullOrEmpty(Table))
                    {
                        string sql = string.Format("SELECT MAX({0}) FROM %{1}", Field, Table);
                        scalar_int = Connect_Main_.ExecuteScalarInt(sql);
                    }
                }

            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in ExecuteSQLScalarInt: " + ex.Message);
            }
            return scalar_int;
        }


        /// <summary>
        /// Метод, возвращающий целочисленный результат выполнения SQL - кода 
        /// </summary>
        /// <param name="What">Значение поля, которое длжно быть извлечено</param>
        /// <param name="Table">Наименование таблицы</param>
        /// <param name="Field">Наименование поля для сравнения</param>
        /// <param name="CompVal">Значение поля для сравнения</param>
        /// <returns></returns>
        public int ExecuteSQLScalarInt(string What, string Table, string Field, string CompVal)
        {
            int scalar_int = -1;
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    if ((!string.IsNullOrEmpty(What)) && (!string.IsNullOrEmpty(Table)) && (!string.IsNullOrEmpty(Field)) && (!string.IsNullOrEmpty(CompVal)))
                    {
                        string sql = string.Format("SELECT {0} FROM %{1} WHERE {2}='{3}'", What, Table, Field, CompVal);
                        scalar_int = Connect_Main_.ExecuteScalarInt(sql);
                    }
                }

            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in ExecuteSQLScalarInt: " + ex.Message);
            }
            return scalar_int;
        }

        /// <summary>
        /// Метод, возвращающий  результат выполнения SQL - кода в формате Даты- времени
        /// </summary>
        /// <param name="What">Значение поля, которое длжно быть извлечено</param>
        /// <param name="Table">Наименование таблицы</param>
        /// <param name="Field">Наименование поля для сравнения</param>
        /// <param name="CompVal">Значение поля для сравнения</param>
        public DateTime ExecuteSQLScalarDataTime(string What, string Table, string Field, string CompVal)
        {
            DateTime scalar_date_time = new DateTime();
            try
            {
                // using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    if ((!string.IsNullOrEmpty(What)) && (!string.IsNullOrEmpty(Table)) && (!string.IsNullOrEmpty(Field)) && (!string.IsNullOrEmpty(CompVal)))
                    {
                        string sql = string.Format("SELECT {0} FROM %{1} WHERE {2}='{3}'", What, Table, Field, CompVal);
                        scalar_date_time = Connect_Main_.ExecuteScalarDateTime(sql);
                    }
                }

            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in ExecuteSQLScalarDataTime : " + ex.Message);
            }
            return scalar_date_time;
        }

        /// <summary>
        /// Метод, возвращающий  результат выполнения SQL - кода в формате вещественного типа (Double)
        /// </summary>
        /// <param name="What">Значение поля, которое длжно быть извлечено</param>
        /// <param name="Table">Наименование таблицы</param>
        /// <param name="Field">Наименование поля для сравнения</param>
        /// <param name="CompVal">Значение поля для сравнения</param>
        public double ExecuteSQLScalarDouble(string What, string Table, string Field, string CompVal)
        {
            double scalar_double = -1;
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    if ((!string.IsNullOrEmpty(What)) && (!string.IsNullOrEmpty(Table)) && (!string.IsNullOrEmpty(Field)) && (!string.IsNullOrEmpty(CompVal)))
                    {
                        string sql = string.Format("SELECT {0} FROM %{1} WHERE {2}='{3}'", What, Table, Field, CompVal);
                        return scalar_double = Connect_Main_.ExecuteScalarDouble(sql);
                    }
                }

            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in ExecuteSQLScalarDouble: " + ex.Message);
            }
            return scalar_double;
        }

        /// <summary>
        /// Метод, возвращающий  результат выполнения SQL - кода в строковом формате (string)
        /// </summary>
        /// <param name="What">Значение поля, которое длжно быть извлечено</param>
        /// <param name="Table">Наименование таблицы</param>
        /// <param name="Field">Наименование поля для сравнения</param>
        /// <param name="CompVal">Значение поля для сравнения</param>
        public string ExecuteSQLScalarString(string What, string Table, string Field, string CompVal)
        {
            string scalar_string = string.Empty;
            try
            {
                // using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    if ((!string.IsNullOrEmpty(What)) && (!string.IsNullOrEmpty(Table)) && (!string.IsNullOrEmpty(Field)) && (!string.IsNullOrEmpty(CompVal)))
                    {
                        string sql = string.Format("SELECT {0} FROM %{1} WHERE {2}='{3}'", What, Table, Field, CompVal);
                        return scalar_string = Connect_Main_.ExecuteScalarString(sql);
                    }
                }

            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in ExecuteSQLScalarString: " + ex.Message);
            }
            return scalar_string;
        }

        /// <summary>
        /// Метод, возвращающий перечень записей после выполнения SQL - запроса
        /// Формат запроса GetEmployeeInform("SELECT ID,FIRSTNAME,LASTNAME,APP_USER FROM EMPLOYEE",4)
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <param name="ColumnCount">Количество столбцов</param>
        /// <returns></returns>
        public List<List<dynamic>> GetEmployeeInform(string sql, int ColumnCount)
        {
            List<List<dynamic>> RecordValues = new List<List<dynamic>>();
            List<dynamic> RecordsVal = null;
            try
            {
                if (!string.IsNullOrEmpty(sql))
                {
                    //using (DbLinker lk = ConnectDB.New())
                    if (Connect_Main_!= null)
                    {
                        ANetRs rs = Connect_Main_.Db.NewRecordset();
                        rs.Open(sql);
                        for (; !rs.IsEOF(); rs.MoveNext())
                        {
                            RecordsVal = new List<dynamic>();
                            for (int i = 0; i < ColumnCount; i++)
                            {
                                RecordsVal.Add(rs.GetString(i));
                            }
                            RecordValues.Add(RecordsVal);
                        }

                        rs.Destroy();
                    }
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetEmployeeInform: " + ex.Message);
            }
            return RecordValues;
        }


        /// <summary>
        /// Метод, выполняющий вставку новой записи 
        /// Формат SQLInsertMethod("ZTESTX", (new string[] { "INT1", "STRING1", "DOUBLE1", "DATETIME1" }), (new string[] { "9", "TE'ST", "18.89", "TO_DATE('12.12.2015','DD.MM.YYYY')"}), true)
        /// Важно: передача данных типа дата-время  организуется (для ORACLE) посредтсвом применения функции TO_DATE с параметрами('Дата','DD.MM.YYYY')
        /// </summary>
        /// <param name="NameTable">Имя таблицы</param>
        /// <param name="Fields"> перечень полей таблицы </param>
        /// <param name="RecordItem"> список соответствующих значений </param>
        /// <param name="AllowedInsFirstColumn"> Признак использования первого поля (если true - первое поле используется)</param>
        /// <returns></returns>
        public int SQLInsertMethod(string NameTable, string[] Fields, string[] RecordItem, bool AllowedInsFirstColumn)
        {
            int res_status = -1;
            try
            {
                string SYMB = "'";
                string PrepareSQL = string.Format("INSERT INTO %{0}(", NameTable);

                for (int i = 0; i < Fields.Count(); i++)
                {
                    if ((!AllowedInsFirstColumn) && (i == 0)) continue;

                    if (i < Fields.Count() - 1)
                    {
                        PrepareSQL += Fields[i].Replace("'", "\"") + ",";
                    }
                    else
                    {
                        PrepareSQL += Fields[i].Replace("'", "\"");
                    }
                }
                PrepareSQL += ") VALUES(";

                for (int i = 0; i < RecordItem.Count(); i++)
                {
                    if ((!AllowedInsFirstColumn) && (i == 0)) continue;

                    if (i < RecordItem.Count() - 1)
                    {
                        if (RecordItem[i].Contains("TO_DATE"))
                        {
                            PrepareSQL += RecordItem[i] + ",";
                        }
                        else
                        {
                            PrepareSQL += SYMB + RecordItem[i].Replace("'", "\"") + SYMB + ",";
                        }
                    }
                    else
                    {
                        if (RecordItem[i].Contains("TO_DATE"))
                        {
                            PrepareSQL += RecordItem[i];
                        }
                        else
                        {
                            PrepareSQL += SYMB + RecordItem[i] + SYMB;
                        }


                    }
                }
                PrepareSQL += ")";
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    res_status = Connect_Main_.Execute(PrepareSQL);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in SQLInsertMethod: " + ex.Message);
            }

            return res_status;
        }

        public int SQLInsertMethod(string NameTable, string[] Fields, string[] RecordItem, bool AllowedInsFirstColumn, DbLinker lk)
        {
            int res_status = -1;
            try
            {
                string SYMB = "'";
                string PrepareSQL = string.Format("INSERT INTO %{0}(", NameTable);

                for (int i = 0; i < Fields.Count(); i++)
                {
                    if ((!AllowedInsFirstColumn) && (i == 0)) continue;

                    if (i < Fields.Count() - 1)
                    {
                        PrepareSQL += Fields[i].Replace("'", "\"") + ",";
                    }
                    else
                    {
                        PrepareSQL += Fields[i].Replace("'", "\"");
                    }
                }
                PrepareSQL += ") VALUES(";

                for (int i = 0; i < RecordItem.Count(); i++)
                {
                    if ((!AllowedInsFirstColumn) && (i == 0)) continue;

                    if (i < RecordItem.Count() - 1)
                    {
                        if (RecordItem[i].Contains("TO_DATE"))
                        {
                            PrepareSQL += RecordItem[i] + ",";
                        }
                        else
                        {
                            PrepareSQL += SYMB + RecordItem[i].Replace("'", "\"") + SYMB + ",";
                        }
                    }
                    else
                    {
                        if (RecordItem[i].Contains("TO_DATE"))
                        {
                            PrepareSQL += RecordItem[i];
                        }
                        else
                        {
                            PrepareSQL += SYMB + RecordItem[i] + SYMB;
                        }


                    }
                }
                PrepareSQL += ")";
                res_status = lk.Execute(PrepareSQL);
            }
            catch (Exception ex)
            {
                //throw new Exception("Error : " + ex.Message);
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in SQLInsertMethod: " + ex.Message);
            }

            return res_status;
        }

        /// <summary>
        /// Метод, выполняющий обновление записей из таблицы БД
        /// </summary>
        /// <param name="NameTable">Наименование таблицы</param>
        /// <param name="NameField">Поле для сравнения</param>
        /// <param name="Value">Значение поля</param>
        /// <returns> Количество обновленных записей</returns>
        public int SQLUpdateMethod(string NameTable, string[] SetFields, string[] SetRecord, string[] WhereFields, string[] WhereRecord)
        {
            int res_status = -1;
            try
            {

                string SYMB = "'";
                string PrepareSQL = string.Format("UPDATE  %{0} SET ", NameTable);

                for (int i = 0; i < SetFields.Count(); i++)
                {

                    if (i < SetFields.Count() - 1)
                    {
                        if (SetRecord[i].Contains("TO_DATE"))
                        {
                            PrepareSQL += SetFields[i].Replace("'", "\"") + "=" + SetRecord[i] + ", ";
                        }
                        else
                        {
                            PrepareSQL += SetFields[i].Replace("'", "\"") + "=" + SYMB + SetRecord[i].Replace("'", "\"") + SYMB + ", ";
                        }
                    }
                    else
                    {
                        if (SetRecord[i].Contains("TO_DATE"))
                        {
                            PrepareSQL += SetFields[i].Replace("'", "\"") + "=" + SetRecord[i];
                        }
                        else
                        {
                            PrepareSQL += SetFields[i].Replace("'", "\"") + "=" + SYMB + SetRecord[i].Replace("'", "\"") + SYMB;
                        }
                    }
                }
                PrepareSQL += " WHERE ";

                for (int i = 0; i < WhereFields.Count(); i++)
                {

                    if (i < WhereFields.Count() - 1)
                    {
                        if (WhereFields[i].Contains("TO_DATE"))
                        {
                            PrepareSQL += "(" + WhereFields[i].Replace("'", "\"") + "=" + WhereRecord[i] + ")" + " AND ";
                        }
                        else
                        {
                            PrepareSQL += "(" + WhereFields[i].Replace("'", "\"") + "=" + SYMB + WhereRecord[i].Replace("'", "\"") + SYMB + ")" + " AND ";
                        }
                    }
                    else
                    {
                        if (WhereRecord[i].Contains("TO_DATE"))
                        {
                            PrepareSQL += "(" + WhereFields[i].Replace("'", "\"") + "=" + WhereRecord[i] + ")";
                        }
                        else
                        {
                            PrepareSQL += "(" + WhereFields[i].Replace("'", "\"") + "=" + SYMB + WhereRecord[i].Replace("'", "\"") + SYMB + ")";
                        }
                    }
                }

                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    res_status = Connect_Main_.Execute(PrepareSQL);
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in SQLUpdateMethod: " + ex.Message);
            }
            return res_status;
        }
        /// <summary>
        /// Метод, выполняющий удаление записей из таблицы БД
        /// </summary>
        /// <param name="NameTable">Наименование таблицы</param>
        /// <param name="NameField">Поле для сравнения</param>
        /// <param name="Value">Значение поля</param>
        /// <returns> Количество удаленных записей/returns>
        public int SQLDeleteMethod(string NameTable, string NameField, string Value)
        {
            int res_status = -1;
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    res_status = Connect_Main_.Execute(string.Format("DELETE FROM %{0} WHERE {1}='{2}'", NameTable, NameField, Value));
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in SQLDeleteMethod: " + ex.Message);
            }
            return res_status;
        }

        /// <summary>
        /// Метод, выполняющий удаление списка записей из БД
        /// </summary>
        /// <param name="NameTable">Наименование таблицы</param>
        /// <param name="NameField">массив полей для сравнения</param>
        /// <param name="Value">Массив значений для сравнения</param>
        public void SQLDeleteMassMethod(string NameTable, string[] NameField, string[] Value)
        {
            for (int i = 0; i < NameField.Count(); i++)
            {
                SQLDeleteMethod(NameTable, NameField[i], Value[i]);
            }
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
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            y.Fetch(filter);
                            y.OpenRs();
                            for (; !y.IsEOF(); y.MoveNext())
                            {
                                isSelectedData = true;
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
        /// 
        /// </summary>
        /// <param name="isSelectedData"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<string> SelectAllRec_From_WebQuery(out bool isSelectedData, string TableName, int indexField)
        {
            isSelectedData = false;
            List<string> Val = new List<string>();
            try
            {
                if (Connect_Main_!= null)
                {

                    ANetRs rs = dbx_main.NewRecordset();
                    rs.Open("SELECT * FROM  " + TableName);
                    for (; !rs.IsEOF(); rs.MoveNext())
                    {
                        // Формирование списка наименований таблиц, которые доступны пользователям 
                        try
                        {
                            Val.Add(rs.GetString(indexField));
                            isSelectedData = true;
                        }

                        catch (Exception e)
                        {
                            CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error: " + e.Message);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetRecordsValue: " + ex.Message);
            }
            return Val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSelectedData"></param>
        /// <param name="TableName"></param>
        /// <param name="indexField"></param>
        /// <returns></returns>
        public List<string> NewRecord_WEB_CONSULT_QUEST(string WEB_STATUS, int CONSULTATION_LINK_ID, DateTime DATE_QUESTION, string AUTHOR_QUESTION, string ANSWER, string QUESTION, DateTime DATE_ANSWER, string AUTHOR_ANSWER)
        {
            List<string> Val = new List<string>();
            try
            {
                if (Connect_Main_!= null)
                {
                    //Connect_Main_.Execute(string.Format("UPDATE %{0} SET NAME='{1}' WHERE ID in ({2})", TableName, "GRISHA", "1"));
                    int MaxID = GetMaxID("ID", "WEB_CONSULT_QUEST");
                    Connect_Main_.Execute(string.Format("INSERT INTO %WEB_CONSULT_QUEST(ID,WEB_STATUS,CONSULTATION_LINK_ID,DATE_QUESTION,AUTHOR_QUESTION,ANSWER,QUESTION,DATE_ANSWER,AUTHOR_ANSWER) VALUES({0},{1},{2},'{3}',{4},{5},{6},'{7}',{8})", ((MaxID <= 0) || (MaxID == 2147483647)) ? 1 : MaxID + 1, ToSql(WEB_STATUS), CONSULTATION_LINK_ID, DATE_QUESTION, ToSql(AUTHOR_QUESTION), ToSql(ANSWER), ToSql(QUESTION), DATE_ANSWER, ToSql(AUTHOR_ANSWER))); 
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetRecordsValue: " + ex.Message);
            }
            return Val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DATE_QUESTION"></param>
        /// <param name="AUTHOR_QUESTION"></param>
        /// <param name="QUESTION"></param>
        /// <returns></returns>
        public List<string> Update_WEB_CONSULT_QUEST(DateTime DATE_QUESTION, string AUTHOR_QUESTION, string QUESTION, int ID)
        {
            List<string> Val = new List<string>();
            try
            {
                if (Connect_Main_!= null)
                {
                    Connect_Main_.Execute(string.Format("UPDATE %WEB_CONSULT_QUEST SET DATE_QUESTION='{0}',AUTHOR_QUESTION={1},QUESTION={2} WHERE ID = {3}", DATE_QUESTION, ToSql(AUTHOR_QUESTION), ToSql(QUESTION), ID));
                }
            }
            catch (Exception ex)
            {
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

            if (!string.IsNullOrEmpty(sql_))
            {
                string err = "";
                try
                {
                    //using (DbLinker lk = ConnectDB.New())
                    if (Connect_Main_!= null)
                    {
                        ANetRs rs = Connect_Main_.Db.NewRecordset();
                        rs.Open(sql_);
                        err = rs.SqlOrder;
                        for (; !rs.IsEOF(); rs.MoveNext())
                        {
                            tb = rs.GetString(0);
                            break;
                        }
                        rs.Destroy();
                    }
                }
                catch (Exception ex)
                {
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

            if (!string.IsNullOrEmpty(sql_))
            {
                string err = "";
                try
                {
                    //using (DbLinker lk = ConnectDB.New())
                    if (Connect_Main_!= null)
                    {
                        ANetRs rs = Connect_Main_.Db.NewRecordset();
                        rs.Open(sql_);
                        err = rs.SqlOrder;
                        for (; !rs.IsEOF(); rs.MoveNext())
                        {
                            tb = rs.GetString(0);
                            break;
                        }
                        rs.Destroy();
                    }
                }
                catch (Exception ex)
                {
                    CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Invalid execute GetUserName: " + ex.Message);
                }
            }
           
            return tb;
        }



        public List<object[]> GetRecordsCompareValue(out bool isSelectedData, string tableName, string[] fld, object[] values, bool[] CompareValue)
        {
            isSelectedData = false;
            List<object[]> value = new List<object[]>();
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
                    {
                        if ((!string.IsNullOrEmpty(GenerateFormat(CompareValue, fld, values, true))))
                        {
                            string res_format = "";

                            for (int i = 0; i < fld.Count(); i++)
                            {
                                res_format += fld[i] + ",";
                            }

                            res_format = res_format.Remove(res_format.Length - 1);
                            y.Format(res_format);
                            if (y.Fetch(GenerateFormat(CompareValue, fld, values, true)))
                            {
                                y.OpenRs();

                                for (; !y.IsEOF(); y.MoveNext())
                                {
                                    isSelectedData = true;

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
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetRecordsCompareValue: " + ex.Message);
            }
            return value;
        }

        public object[] GetRecordsCompareValueSQL(out bool isSelectedData, string tableName, string[] fld, object[] values, bool[] CompareValue)
        {
            isSelectedData = false;
            List<object> rec_ptr = new List<object>();
            try
            {

                if ((!string.IsNullOrEmpty(GenerateFormat(CompareValue, fld, values, true))))
                {

                    List<dynamic[]> res = ExecuteSQL(out isSelectedData, fld, tableName, 1000, GenerateFormat(CompareValue, fld, values, true), null);
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
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetRecordsCompareValueSQL: " + ex.Message);
            }
            return rec_ptr.ToArray();
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



        public object GetRecordsCompareValue(string tableName, string[] fld, object[] values, bool[] CompareValue, string WorkFlowField)
        {
            object rec_ptr = new object();
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
                    {
                        if ((!string.IsNullOrEmpty(GenerateFormat(CompareValue, fld, values, true))))
                        {
                            y.Format(WorkFlowField);
                            if (y.Fetch(GenerateFormat(CompareValue, fld, values, true)))
                            {
                                y.OpenRs();

                                for (; !y.IsEOF(); y.MoveNext())
                                {
                                    rec_ptr = y.Get(WorkFlowField);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in GetRecordsCompareValue: " + ex.Message);
            }
            return rec_ptr;
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
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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


        public List<object> GetOneFieldValueDistinct(string tableName, string filter, string fld)
        {
            List<object> value = new List<object>();
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
        public object GetOneFieldValue(string tableName, int rec_id, string fld)
        {
            object value = null;
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
            if (Connect_Main_!= null)
            {
                using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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

        public int NewRecordInt(string tableName, string[] fld, object[] values, bool[] IsPKField)
        {
            bool isSuccessFullNew = true;
            int NewRec = -1;
            // using (DbLinker lk = ConnectDB.New())
            if (Connect_Main_!= null)
            {
                using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
                            NewRec = id;
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
                            NewRec = 1;

                        }
                    }
                    catch (Exception ex)
                    {
                        NewRec = -1;
                        isSuccessFullNew = false;
                        CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in NewRecord: " + ex.Message);
                    }
                }
            }
            return NewRec;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <param name="values"></param>
        /// <param name="IsPKField"></param>
        /// <param name="lk"></param>
        /// <returns></returns>
        public int NewRecord(string tableName, string[] fld, object[] values, bool[] IsPKField, DbLinker lk)
        {
            bool isSuccessFullNew = true;
            string NameFieldException = "";
            int NewRec = -1;
            using (Yyy y = Yyy.CreateObject(tableName, lk))
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
                        NewRec = id;
                        y.New();
                        for (int i = 0; i < fld.Count(); i++)
                        {
                            NameFieldException = fld[i];

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
                            NameFieldException = fld[i];
                            y.Set(fld[i], values[i]);
                        }
                        y.Save();
                        NewRec = 1;

                    }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fld_check"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool CheckFieldNotNull(string fld_check, string tableName)
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
                                        if (fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL))
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
        public static Dictionary<string, RecordPtrDB> GetListMandatoryFields(string tableName)
        {

            Dictionary<string, RecordPtrDB> MandatoryList = new Dictionary<string, RecordPtrDB>();
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


                                if (fjF != null)
                                {
                                    //if ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_FKEY)) || (fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL)))
                                    //if (fjF.Options == OrmCs.OrmFieldFOption.fld_NOTNULL)
                                    if ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_PRIMARY)) || (fjF.Options == OrmCs.OrmFieldFOption.fld_NOTNULL))
                                    {
                                        
                                        RecordPtrDB dbx = new RecordPtrDB();
                                        dbx.CaptionNameTable = fjF.Table.PrettyName;
                                        dbx.NameTableTo = tableName;
                                        dbx.DefVal = fjF.DefVal;
                                        dbx.FieldJoinTo = fjF.Name;
                                        dbx.FieldCaptionTo = f1.Info;
                                        //dbx.Name = fj.Name;
                                        if (f1.DDesc != null) dbx.TypeVal = f1.DDesc.ClassType;
                                        MandatoryList.Add(f1.Name, dbx);
                                    }
                                }

                            }
                            break;

                    }
                }
            }
            return MandatoryList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static Dictionary<string, RecordPtrDB> GetMandatoryFields(string tableName)
        {

            Dictionary<string, RecordPtrDB> MandatoryList = new Dictionary<string, RecordPtrDB>();
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


                                if (fjF != null)
                                {

                                    if (((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL | OrmCs.OrmFieldFOption.fld_FKEY))) || ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL) && fjF.Name.Contains("ROLE"))) || ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL) && fjF.Name.Contains("CODE"))) || ((fjF.Options == (OrmCs.OrmFieldFOption.fld_NOTNULL) && fjF.Name.Contains("NAME"))) || (fjF.Name.Contains("USER_ID")) || (fjF.Name.Contains("OWNER_ID")))
                                    {

                                        RecordPtrDB dbx = new RecordPtrDB();
                                        dbx.CaptionNameTable = fjF.Table.PrettyName;
                                        dbx.NameTableTo = tableName;
                                        dbx.DefVal = fjF.DefVal;
                                        dbx.FieldJoinTo = fjF.Name;
                                        dbx.FieldCaptionTo = f1.Info;
                                        //dbx.Name = fj.Name;
                                        if (f1.DDesc != null) dbx.TypeVal = f1.DDesc.ClassType;
                                        if (f1.Special != null) dbx.Precision = (int)f1.Special.m_max;
                                        MandatoryList.Add(f1.Name, dbx);
                                    }
                                }

                            }
                            break;

                    }
                }
            }
            return MandatoryList;
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
        /// Получить все таблицы, которые удалось обнаружить в запросе
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <param name="lk"></param>
        /// <returns></returns>
        public static string[] GetAllTablesAndFields(string tableName, string[] fld, DbLinker lk)
        {
            List<string> LstAllTables = new List<string>();
            List<string> LstDubl = new List<string>();
            List<string> LstRes = new List<string>();
            try
            {
                List<RecordPtrDB> recDB_Lst = new List<RecordPtrDB>();

                for (int i = 0; i < fld.Count(); i++)
                {
                    string[] Spl = null; string[] Spl_Dubl = null;



                    RecordPtrDB recDB = new RecordPtrDB();
                    recDB.NameTableTo = tableName;

                    if (fld[i].IndexOf(".") > 0)
                    {
                        Spl = fld[i].Split(new char[] { '.' });
                        Spl_Dubl = fld[i].Split(new char[] { '.' });
                    }

                    if (Spl != null)
                    {
                        for (int r = 0; r < Spl.Count() - 1; r++)
                        {
                            recDB = GetTableFromORM(Spl[r], recDB.NameTableTo);
                            Spl[r] = recDB.NameTableTo;
                            Spl_Dubl[r] = "";
                            Spl_Dubl[r] = recDB.Name;

                            if (fld[i].IndexOf(".") > 0) { recDB.FieldCaptionFrom = fld[i].Substring(0, fld[i].IndexOf(".")); }
                            recDB_Lst.Add(recDB);

                        }


                        for (int r = 0; r < Spl.Count() - 1; r++)
                        {

                            if ((!LstAllTables.Contains(Spl[r])))
                            {
                                LstAllTables.Add(Spl[r]);
                            }


                        }
                    }

                }


                List<string> newLst = new List<string>();
                List<string> LstResTmp = new List<string>();
                foreach (RecordPtrDB RecA in recDB_Lst)
                {
                    if (!newLst.Contains(RecA.FieldCaptionFrom))
                        newLst.Add(RecA.FieldCaptionFrom);
                }


                foreach (string TypeCm in newLst)
                {
                    foreach (RecordPtrDB RecA in recDB_Lst)
                    {
                        foreach (string lx in LstAllTables)
                        {
                            if ((RecA.NameTableTo == lx) && (TypeCm == RecA.FieldCaptionFrom) && (!LstResTmp.Contains(lx + TypeCm)))
                            {
                                LstResTmp.Add(lx + TypeCm);
                                LstRes.Add(lx);
                            }
                        }
                    }
                }




            }
            catch (Exception)
            {

            }
            return LstRes.ToArray();
            //return LstAllTables.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <param name="values"></param>
        /// <param name="Val_Mass_Add"></param>
        /// <param name="lk"></param>
        /// <returns></returns>
        public void TestEdit(int ID, string tableName, string[] fld, object[] values, List<List<RecordPtrDB>> Val_Mass_Add, DbLinker lk, int USER_ID)
        {
            int ID_Z = -1;
            string TableName2 = "";
            List<int> Lst_Val_Index = new List<int>();
            Dictionary<string, List<RecordPtrDB>> recDB_ = new Dictionary<string, List<RecordPtrDB>>();
            List<string> LstTable = new List<string>();
            List<string> LstNameField = new List<string>();

            for (int i = 0; i < fld.Count(); i++)
            {

                if (i == 0) { TableName2 = tableName.Trim(); ID_Z = ID; } //else { tableName = TableName2; }
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
                    for (int r = 0; r < Spl.Count(); r++)
                    {
                        if (r < Spl.Count() - 1)
                        {
                            recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                            recDB.NameTableFrom = (r == 0 ? TableName2 : Spl[r - 1]);
                            //List<object[]> Tmp_Val = ConnectDB.ICSMQuery(tableName, new string[] { fld[i] }, string.Format("[ID]={0}", ID), "[ID] DESC", 1, null, ConnectDB.Connect_, false);
                            //if (Tmp_Val[0] != null)
                            //{
                                //if (Tmp_Val[0].Count() > 0)
                                    //recDB.OldVal = Tmp_Val[0][0];
                            //}
                            Spl[r] = recDB.NameTableTo;
                            if (!LstTable.Contains(recDB.NameTableFrom))
                                LstTable.Add(recDB.NameTableFrom);

                            recDB.LinkField = fld[i];
                            recDB_Lst.Add(recDB);
                        }
                        else
                        {
                            recDB = new RecordPtrDB();
                            recDB.NameTableTo = recDB_Lst[recDB_Lst.Count()-1].NameTableTo;
                            recDB.Name = recDB_Lst[recDB_Lst.Count() - 1].Name;
                            //List<object[]> Tmp_Val = ConnectDB.ICSMQuery(tableName, new string[] { fld[i] }, string.Format("[ID]={0}", ID), "[ID] DESC", 1, null, ConnectDB.Connect_, false);
                            //if (Tmp_Val[0] != null)
                            //{
                                //if (Tmp_Val[0].Count() > 0)
                                    //recDB.OldVal = Tmp_Val[0][0];
                            //}
                            recDB.NameFieldForSetValue = Spl[r];
                            recDB.FieldJoinTo = Spl[r];

                            if (!LstTable.Contains(recDB.NameTableTo))
                                LstTable.Add(recDB.NameTableTo);

                            recDB.OldVal = values[i];
                            recDB.LinkField = fld[i];
                            recDB_Lst.Add(recDB);
                        }
                    }
                }
                else
                {
                    recDB = new RecordPtrDB();
                    recDB.NameTableTo = tableName;
                    //List<object[]> Tmp_Val = ConnectDB.ICSMQuery(tableName, new string[] { fld[i] }, string.Format("[ID]={0}", ID), "[ID] DESC", 1, null, ConnectDB.Connect_, false);
                    //if (Tmp_Val[0] != null)
                    //{
                        //if (Tmp_Val[0].Count() > 0)
                            //recDB.OldVal = Tmp_Val[0][0];
                    //}
                    recDB.NameFieldForSetValue = fld[i];

                    if (!LstTable.Contains(recDB.NameTableTo))
                        LstTable.Add(recDB.NameTableTo);

                    recDB.OldVal = values[i];
                    recDB.LinkField = fld[i];
                    recDB_Lst.Add(recDB);
                }
                recDB_.Add(fld[i],recDB_Lst);
            }

            Dictionary<string, List<RecordPtrDB>> Additional_RecDB_ = new Dictionary<string, List<RecordPtrDB>>();

            foreach (KeyValuePair<string,List<RecordPtrDB>> itb in recDB_)
            {

                List<RecordPtrDB> LstX = new List<RecordPtrDB>();
                foreach (RecordPtrDB iu in itb.Value)
                {
                    Dictionary<string, RecordPtrDB> MandatoryFields = ConnectDB.GetMandatoryFields(iu.NameTableTo);
                    foreach (KeyValuePair<string, RecordPtrDB> ity in MandatoryFields)
                    {
                        if (ity.Value.FieldJoinTo != null)
                        {
                            if (((ity.Key != iu.FieldJoinTo) && (ity.Value.NameTableTo == iu.NameTableTo)))
                            {

                                if (LstX.Find(r => r.NameTableTo == iu.NameTableTo && r.FieldJoinTo == iu.FieldJoinTo) == null)
                                {
                                    /*
                                   bool isCheck = true;
                                    foreach (RecordPtrDB check in itb.Value)
                                    {
                                        if (!((LstX.Find(r => r.NameTableTo == check.NameTableTo && r.FieldJoinTo == check.FieldJoinTo )) == null))
                                        {
                                            isCheck = false;
                                            break; 
                                        }
                                        
                                    }

                                    if (isCheck) LstX.Add(ity.Value);
                                     */
                                    ity.Value.Name = iu.Name;
                                    ity.Value.LinkField = iu.LinkField;

                                    if (ity.Value.FieldJoinTo.Contains("CODE"))
                                    {
                                        ity.Value.NewVal = (Guid.NewGuid().ToString()).Substring(0, ity.Value.Precision> 23 ? 23 : ity.Value.Precision);
                                    }
                                    if ((ity.Value.FieldJoinTo.Contains("OWNER_ID")) || (ity.Value.FieldJoinTo.Contains("USER_ID")))
                                    {
                                        ity.Value.NewVal = USER_ID;
                                    }
                                    if (ity.Value.FieldJoinTo.Contains("ROLE"))
                                    {
                                        if ((ity.Value.NameTableTo=="MICROWS") && (ity.Value.Name=="StationA"))
                                        {
                                            ity.Value.NewVal = "A";
                                        }
                                    }
                                    if (ity.Value.FieldJoinTo.Contains("END_ROLE"))
                                    {
                                        if ((ity.Value.NameTableTo == "MICROWS") && (ity.Value.Name == "StationA"))
                                        {
                                            ity.Value.NewVal = "B";
                                        }
                                    }
                                    if (ity.Value.FieldJoinTo.Contains("ROLE"))
                                    {
                                        if ((ity.Value.NameTableTo == "MICROWS") && (ity.Value.Name == "StationB"))
                                        {
                                            ity.Value.NewVal = "B";
                                        }
                                    }
                                    if (ity.Value.FieldJoinTo.Contains("END_ROLE"))
                                    {
                                        if ((ity.Value.NameTableTo == "MICROWS") && (ity.Value.Name == "StationB"))
                                        {
                                            ity.Value.NewVal = "A";
                                        }
                                    }


                                    if (ity.Value != null)
                                    {
                                        if (!((ity.Value.FieldJoinTo == "OWNER_ID") && (ity.Value.NameTableTo == "LICENCE")))
                                            LstX.Add(ity.Value);
                                    }

                                   
                                }

                                //                            if (!LstX.Contains(ity.Value))

                            }
                        }
                    }
                    /*
                    foreach (KeyValuePair<string, RecordPtrDB> ity in MandatoryFields)
                    {
                        if (((ity.Key != iu.FieldJoinFrom) && (ity.Value.NameTableFrom == iu.NameTableFrom)))
                        {
                     
                                LstX.Add(ity.Value);

                        }
                    }
                     */ 
                  
                }
                if (LstX != null) { if (LstX.Count > 0) if (!Additional_RecDB_.ContainsKey(itb.Key)) { Additional_RecDB_.Add(itb.Key, LstX); } }
                
            }


                List<RecordPtrDB> del_ind = new List<RecordPtrDB>();
                List<RecordPtrDB> dist_lst = new List<RecordPtrDB>();
                foreach (KeyValuePair<string, List<RecordPtrDB>> ity in Additional_RecDB_)
                {
                    
                    List<RecordPtrDB> Dist = new List<RecordPtrDB>();
                    foreach (RecordPtrDB Rc in ity.Value)
                    {

                        bool isCheck = true;
                        if (recDB_.FirstOrDefault(r => r.Key == ity.Key).Value != null)
                        {
                            foreach (RecordPtrDB Rc_g in recDB_.FirstOrDefault(r => r.Key == ity.Key).Value)
                            {
                                if (Rc_g.FieldJoinTo == Rc.FieldJoinTo && Rc_g.NameTableTo == Rc.NameTableTo && Rc_g.LinkField == Rc.LinkField)
                                {
                                    isCheck = false;
                                    break;
                                }
                            }
                        }

                        //Rc.NewVal = null;

                        if (((Dist.Find(r => r.FieldJoinTo == Rc.FieldJoinTo && r.NameTableTo == Rc.NameTableTo && r.LinkField == Rc.LinkField)) == null) && (isCheck))
                        {
                            Rc.isMandatory = true;
                            if (!Dist.Contains(Rc))
                            Dist.Add(Rc);
                        }
                    }
                    

                    if (Dist != null)
                    {
                        recDB_.FirstOrDefault(r => r.Key == ity.Key).Value.AddRange(Dist);
                    }

                    
               }

                List<RecordPtrDB> AllFieldRec = new List<RecordPtrDB>();
                foreach (KeyValuePair<string, List<RecordPtrDB>> LX in recDB_)
                {
                    foreach (RecordPtrDB rev in LX.Value)
                    {
                        string str_sub = (rev.LinkField.Length > 0 ? rev.LinkField.IndexOf(".") > 0 ? rev.LinkField.Substring(0, rev.LinkField.IndexOf(".")) : "" : "");
                        if ((AllFieldRec.Find(t => t.NameTableTo == rev.NameTableTo && (t.LinkField.Length > 0 ? t.LinkField.IndexOf(".")>0 ? t.LinkField.Substring(0, t.LinkField.IndexOf(".")) : "" : "") == str_sub && t.FieldJoinTo == rev.FieldJoinTo)) == null)
                        {
                            rev.NameLayer = str_sub;
                            AllFieldRec.Add(rev);
                        }
                    }
                }


                List<RecordPtrDB> L_Tables = AllFieldRec.FindAll(t => t.NameTableTo != "" && t.NameTableTo != null);
                List<string> lst_dist_table = new List<string>();
                List<string> lst_dist_layer = new List<string>();
                if (L_Tables != null)
                {
                    foreach (RecordPtrDB l_item in AllFieldRec)
                    {
                        if (!lst_dist_table.Contains(l_item.NameTableTo))
                            lst_dist_table.Add(l_item.NameTableTo);
                    }

                    foreach (RecordPtrDB l_item in AllFieldRec)
                    {
                        if (!lst_dist_layer.Contains(l_item.NameLayer))
                            lst_dist_layer.Add(l_item.NameLayer);
                    }

                    lst_dist_table.Sort();
                    lst_dist_layer.Sort();


                    List<List<RecordPtrDB>> L_Ch_ALL = new List<List<RecordPtrDB>>();
                    foreach (string vb in lst_dist_layer)
                    {
                        List<RecordPtrDB> L_Ch_M = new List<RecordPtrDB>();
                        List<RecordPtrDB> L_Ch_A = new List<RecordPtrDB>();
                        List<RecordPtrDB> L_Ch_C = new List<RecordPtrDB>();
                        List<RecordPtrDB> find_items = new List<RecordPtrDB>();

                        L_Ch_M = AllFieldRec.FindAll(r => r.NameLayer == vb && r.isMandatory == true);
                        L_Ch_A = AllFieldRec.FindAll(r => r.NameLayer == vb && r.isMandatory == false);


                        if (L_Ch_M != null)
                        {

                            find_items = L_Ch_M.FindAll(r => r.NameTableTo == tableName && (!string.IsNullOrEmpty(r.FieldJoinTo)));
                            if (find_items != null)  { foreach (RecordPtrDB item_rcv in find_items) { if (item_rcv.NameTableTo == tableName) {  item_rcv.OldVal = ID; }  }  }
                            find_items = L_Ch_M.FindAll(r => r.NameTableFrom  == tableName && (!string.IsNullOrEmpty(r.FieldJoinFrom)));
                            if (find_items != null) { foreach (RecordPtrDB item_rcv in find_items) { if (item_rcv.NameTableFrom == tableName) { item_rcv.OldVal = ID; } } }
                            L_Ch_C.AddRange(L_Ch_M);
                        }
                        

                            if (L_Ch_A != null)
                            {
                                find_items = L_Ch_A.FindAll(r => r.NameTableTo == tableName && (!string.IsNullOrEmpty(r.FieldJoinTo)));
                                if (find_items != null) { foreach (RecordPtrDB item_rcv in find_items) { if (item_rcv.NameTableTo == tableName) { item_rcv.OldVal = ID; } } }
                                find_items = L_Ch_A.FindAll(r => r.NameTableFrom == tableName && (!string.IsNullOrEmpty(r.FieldJoinFrom)));
                                if (find_items != null) { foreach (RecordPtrDB item_rcv in find_items) { if (item_rcv.NameTableFrom == tableName) { item_rcv.OldVal = ID; } } }
                                L_Ch_C.AddRange(L_Ch_A);
                            }

                            L_Ch_ALL.Add(L_Ch_C);
                        }






                        List<List<RecordPtrDB>> All_Res_Table_Without_Links = new List<List<RecordPtrDB>>();
                        List<List<RecordPtrDB>> All_Res_Table_With_Links = new List<List<RecordPtrDB>>();



                        lst_dist_table.Remove(tableName);
                        lst_dist_table.Insert(0, tableName);


                        //// поиск значений всех индексов (полей -связей между таблицами)

                        
                        foreach (List<RecordPtrDB> itm in L_Ch_ALL)
                        {

                            List<RecordPtrDB> find_itms = itm.FindAll(r=>(!string.IsNullOrEmpty(r.FieldJoinTo) || !string.IsNullOrEmpty(r.FieldJoinFrom)));
                            if (find_itms != null)
                            {
                                string TableName_Tmp = ""; int idx_tmp = 0;

                                RecordPtrDB find_its = itm.Find(r => r.JoinToIndex > 0 || (r.OldVal != null && (!string.IsNullOrEmpty(r.FieldJoinTo) || !string.IsNullOrEmpty(r.FieldJoinFrom))));
                               if (find_its != null)
                                {
                                    TableName_Tmp = find_its.NameTableTo;
                                    idx_tmp = find_its.JoinToIndex > 0 ? find_its.JoinToIndex : ((find_its.OldVal != null && (!string.IsNullOrEmpty(find_its.FieldJoinTo) || !string.IsNullOrEmpty(find_its.FieldJoinFrom)))) ? (int)find_its.OldVal : -1;
                                    FillData(itm, TableName_Tmp, idx_tmp);
                                    FillDataExtended(itm, TableName_Tmp);
                                    //find_itms = itm.FindAll(r => r.NameTableTo == TableName_Tmp && r.JoinToIndex == idx_tmp);



                                }
                                else
                                {
                                    find_its = itm.Find(r => r.JoinFromIndex > 0 || (r.OldVal != null && (!string.IsNullOrEmpty(r.FieldJoinTo) || !string.IsNullOrEmpty(r.FieldJoinFrom))));
                                   if (find_its != null)
                                    {
                                        TableName_Tmp = find_its.NameTableFrom;
                                        idx_tmp = find_its.JoinFromIndex > 0 ? find_its.JoinFromIndex : ((find_its.OldVal != null && (!string.IsNullOrEmpty(find_its.FieldJoinTo) || !string.IsNullOrEmpty(find_its.FieldJoinFrom)))) ? (int)find_its.OldVal : -1;
                                        FillData(itm, TableName_Tmp, idx_tmp);
                                        FillDataExtended(itm, TableName_Tmp);
                                        
                                       
                                        //find_itms = itm.FindAll(r => r.NameTableFrom == TableName_Tmp && r.JoinFromIndex == idx_tmp);
                                    }
                                }
                            }
                        }


                        /////





                }
                   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Mass"></param>
        /// <param name="InitTable"></param>
        /// <param name="Idx"></param>
        public void FillDataExtended(List<RecordPtrDB> Mass, string InitTable)
        {

            List<RecordPtrDB> find_itms_ = Mass.FindAll(r => (r.NameTableFrom == InitTable && r.JoinToIndex == -1 && r.JoinFromIndex == -1));
            

            /*
            if (find_itms_ != null)
            {
                foreach (RecordPtrDB rc in find_itms_)
                {
                    if ((rc.OldVal != null) && ((!string.IsNullOrEmpty(rc.FieldJoinFrom) && (!string.IsNullOrEmpty(rc.FieldJoinTo)) && (!string.IsNullOrEmpty(rc.NameTableFrom)) && (!string.IsNullOrEmpty(rc.NameTableTo)))))
                    {
                        List<object[]> Tmp_Val = ConnectDB.ICSMQuery(rc.NameTableTo, new string[] { rc.FieldJoinTo }, string.Format("[ID]={0}", (int)rc.OldVal), "", 1, null, ConnectDB.Connect_, false);
                         if (Tmp_Val != null)
                         {
                             if (Tmp_Val.Count() > 0)
                             {
                                 if (Tmp_Val[0].Count() > 0)
                                 {

                                     if ((int)Tmp_Val[0][0] != 2147483647) { rc.JoinToIndex = (int)Tmp_Val[0][0]; }
                                     else { rc.JoinToIndex = -1; }

                                 }
                             }
                         }
                    }
                }
            }

            find_itms_ = Mass.FindAll(r => (r.NameTableTo == InitTable && r.JoinToIndex == -1 && r.JoinFromIndex == -1));
            if (find_itms_ != null)
            {
                foreach (RecordPtrDB rc in find_itms_)
                {
                    if ((rc.OldVal != null) && ((!string.IsNullOrEmpty(rc.FieldJoinFrom) && (!string.IsNullOrEmpty(rc.FieldJoinTo)) && (!string.IsNullOrEmpty(rc.NameTableFrom)) && (!string.IsNullOrEmpty(rc.NameTableTo)))))
                    {
                        
                        List<object[]> Tmp_Val = ConnectDB.ICSMQuery(rc.NameTableFrom, new string[] { rc.FieldJoinFrom }, string.Format("[ID]={0}", (int)rc.OldVal), "", 1, null, ConnectDB.Connect_, false);
                        if (Tmp_Val != null)
                        {
                            if (Tmp_Val.Count() > 0)
                            {
                                if (Tmp_Val[0].Count() > 0)
                                {
                                    if ((int)Tmp_Val[0][0] != 2147483647) { rc.JoinFromIndex = (int)Tmp_Val[0][0]; }
                                    else { rc.JoinFromIndex = -1; }
                                }
                            }
                        }
                    }
                }
            
            } 
             */ 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Mass"></param>
        /// <param name="InitTable"></param>
        /// <param name="Idx"></param>
        public void FillData(List<RecordPtrDB> Mass, string InitTable, int Idx)
        {
            RecordPtrDB find_itms_To = Mass.Find(r => (r.NameTableTo == InitTable && r.JoinFromIndex == 0) && ((!string.IsNullOrEmpty(r.FieldJoinFrom)) && (!string.IsNullOrEmpty(r.FieldJoinTo))));
            if (find_itms_To != null)
            {
                find_itms_To.JoinFromIndex = -1;
                FillData(Mass, find_itms_To.NameTableFrom, find_itms_To.JoinToIndex);
            }

            RecordPtrDB find_itms_From = Mass.Find(r => (r.NameTableFrom == InitTable && r.JoinToIndex == 0) && ((!string.IsNullOrEmpty(r.FieldJoinFrom)) && (!string.IsNullOrEmpty(r.FieldJoinTo))));
            if (find_itms_From != null)
            {
                find_itms_From.JoinToIndex = -1;
                FillData(Mass, find_itms_From.NameTableTo, find_itms_From.JoinFromIndex);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Mass"></param>
        /// <param name="InitTable"></param>
        /// <param name="Idx"></param>
        public void FillDataFor(List<RecordPtrDB> Mass, string InitTable, int Idx)
        {
            /*
            RecordPtrDB find_itms_To = Mass.Find(r => (r.NameTableTo == InitTable && r.JoinFromIndex == -1) && ((!string.IsNullOrEmpty(r.FieldJoinFrom)) && (!string.IsNullOrEmpty(r.FieldJoinTo))));
            if (find_itms_To != null)
            {
                
                List<object[]> Tmp_Val = ConnectDB.ICSMQuery(find_itms_To.NameTableFrom, new string[] { find_itms_To.FieldJoinFrom }, string.Format("[{0}]={1}", find_itms_To.FieldJoinTo, Idx), "", 1, null, ConnectDB.Connect_, false);
                if (Tmp_Val != null)
                {
                    if (Tmp_Val.Count()>0)
                    {
                        if (Tmp_Val[0].Count() > 0)
                        {
                            if ((int)Tmp_Val[0][0] != 2147483647) { find_itms_To.JoinFromIndex = (int)Tmp_Val[0][0]; }
                            else { find_itms_To.JoinFromIndex = -1; }
                        }
                        else
                        {
                            find_itms_To.JoinFromIndex = -1;
                        }
                    }
                    else
                    {
                        find_itms_To.JoinFromIndex = -1;
                    }
                }
                else
                {
                    find_itms_To.JoinFromIndex = -1;
                }

                FillDataFor(Mass, find_itms_To.NameTableFrom, find_itms_To.JoinToIndex);
            }

            RecordPtrDB find_itms_From = Mass.Find(r => (r.NameTableFrom == InitTable && r.JoinToIndex == -1) && ((!string.IsNullOrEmpty(r.FieldJoinFrom)) && (!string.IsNullOrEmpty(r.FieldJoinTo))));
            if (find_itms_From != null)
            {
                
                List<object[]> Tmp_Val = ConnectDB.ICSMQuery(find_itms_From.NameTableTo, new string[] { find_itms_From.FieldJoinTo }, string.Format("[{0}]={1}", find_itms_From.FieldJoinFrom, Idx), "", 1, null, ConnectDB.Connect_, false);
                if (Tmp_Val != null)
                {
                    if (Tmp_Val.Count() > 0)
                    {
                        if (Tmp_Val[0].Count() > 0)
                        {
                            if ((int)Tmp_Val[0][0] != 2147483647) { find_itms_From.JoinToIndex = (int)Tmp_Val[0][0]; }
                            else  {  find_itms_From.JoinToIndex = -1;  }
                        }
                        else
                        {
                            find_itms_From.JoinToIndex = -1;
                        }
                    }
                    else
                    {
                        find_itms_From.JoinToIndex = -1;
                    }
                }
                else
                {
                    find_itms_From.JoinToIndex = -1;
                }

                FillDataFor(Mass, find_itms_From.NameTableTo, find_itms_From.JoinFromIndex);
            }
             */ 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <param name="values"></param>
        /// <param name="IsPKField"></param>
        /// <param name="lk"></param>
        /// <returns></returns>
        public int EditRec(int ID, string tableName, string[] fld, object[] values, List<List<RecordPtrDB>> Val_Mass_Add, DbLinker lk)
        {
            string NameFieldException = "";
            int NewRec = -1;
            int ID_Z = -1;

            string TableName2 = "";
            try
            {
                List<int> Lst_Val_Index = new List<int>();
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
                            recDB.NameTableFrom = (r == 0 ? TableName2 : Spl[r-1]);
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
                                    if (vl != null) { ID_Val = (int)vl; } else { ID_Val = 2147483647; }
                                    recDB_Lst[kk].Value = GetOneFieldValue(recDB_Lst[kk].NameTableTo, string.Format("[{0}]={1}", recDB_Lst[kk].FieldJoinTo, ID_Val), Spl[Spl.Count() - 1]);
                                    if (((ID_Val > -1) && (ID_Val < 2147483640)))
                                    {
                                        SetFieldValue(recDB_Lst[kk].NameTableTo, string.Format("[{0}]={1}", recDB_Lst[kk].FieldJoinTo, ID_Val), Spl[Spl.Count() - 1], values[i]);
                                    }
                                    else
                                    {
                                        foreach (List<RecordPtrDB> RT in Val_Mass_Add)
                                        {
                                            int ID_ = -1;
                                            //List<RecordPtrDB> Rpt = RT.FindAll(a => a.NameTable == recDB_Lst[kk].NameTable);
                                            List<RecordPtrDB> Rpt = RT.FindAll(a => a.NameTableTo == recDB_Lst[kk].NameTableTo && (Lst_Val_Index.Count() > 0 ? a.Index != Lst_Val_Index[Lst_Val_Index.Count() - 1] : a.Index > 0));
                                            if (Rpt != null)
                                            {
                                                List<string> fld_lst = new List<string>();
                                                List<object> val_lst = new List<object>();

                                                foreach (RecordPtrDB item in Rpt)
                                                {

                                                    if (!Lst_Val_Index.Contains(item.Index))
                                                        Lst_Val_Index.Add(item.Index);

                                                    fld_lst.Add(item.FieldJoinTo);
                                                    //val_lst.Add(item.Value);
                                                    object dbx_d = new object();
                                                    switch (item.TypeVal)
                                                    {
                                                        case OrmVarType.var_Dou:
                                                            dbx_d = Convert.ChangeType(item.Value, typeof(double));
                                                            break;
                                                        case OrmVarType.var_Int:
                                                            dbx_d = Convert.ChangeType(item.Value, typeof(int));
                                                            break;
                                                        case OrmVarType.var_String:
                                                            dbx_d = item.Value;
                                                            break;
                                                    }
                                                    val_lst.Add(dbx_d);

                                                }
                                                fld_lst.Add(Spl[Spl.Count() - 1]);
                                                val_lst.Add(values[i]);

                                                //if (recDB_Lst.Count() > 1)
                                                //{
                                                //    fld_lst.Add(recDB_Lst[kk].FieldJoinTo);
                                                //    val_lst.Add(recDB_Lst[kk - 1].KeyValue);
                                                // }

                                                ID_ = NewRecord(recDB_Lst[kk].NameTableTo, fld_lst.ToArray(), val_lst.ToArray(), GetPrimaryField(recDB_Lst[kk].NameTableTo), Connect_Main_);

                                                if ((ID_ > -1) && (ID_ < 2147483640))
                                                {
                                                    SetFieldValue(tableName, ID, recDB_Lst[kk].FieldJoinFrom, ID_);
                                                }

                                                break;
                                            }
                                        }

                                        /*
                                        int ID_ = NewRecord(recDB_Lst[kk].NameTable, new string[] { Spl[Spl.Count() - 1] }, new object[] { values[i] }, recDB_Lst[kk].FieldJoinTo, Connect_);
                                        if ((ID_ > -1) && (ID_ < 2147483640))
                                        {
                                            SetFieldValue(tableName, ID, recDB_Lst[kk].FieldJoinFrom, ID_);
                                        }
                                         */
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
                                    else { ID_Val = NewRecord(recDB_Lst[kk].NameTableTo, new string[] { recDB_Lst[kk].FieldJoinTo }, new object[] { ID_Val }, GetPrimaryField(recDB_Lst[kk].NameTableTo), Connect_Main_); recDB_Lst[kk].KeyValue = ID_Val; }
                                }


                            }


                        }
                        //SetFieldValue(recDB.NameTable,recDB.KeyValue,Spl[Spl.Count()-1],
                    }
                    else
                    {
                        if (!CheckFieldPrimary(fld[i], TableName2))
                        {
                            SetFieldValue(TableName2, ID_Z, fld[i], values[i]);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                NewRec = -1;
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in NewRecord: (" + tableName + ") Field: " + NameFieldException + " - " + ex.Message);
            }
            return NewRec;
        }

        /// <summary>
        /// Создать перечень новых записей со ссылками между собой
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="tableName"></param>
        /// <param name="fld"></param>
        /// <param name="values"></param>
        /// <param name="Val_Mass_Add"></param>
        /// <param name="lk"></param>
        /// <returns></returns>
        public int NewRecord_(string tableName, string[] fld, object[] values, List<List<RecordPtrDB>> Val_Mass_Add, DbLinker lk)
        {
            string NameFieldException = "";
            int NewRec = -1;
            int ID = -1;
            int ID_Z = -1;
            List<RecordPtrDB> IndexVal = new List<RecordPtrDB>();


            string TableName2 = "";
            try
            {
                List<string> Lst_Fld_ = new List<string>();
                List<object> Lst_Val_ = new List<object>();

                for (int i = 0; i < fld.Count(); i++)
                {
                    if (i == 0) { TableName2 = tableName.Trim(); ID_Z = ID; } 

                    string[] Spl = null;
                    RecordPtrDB recDB = new RecordPtrDB();
                    recDB.NameTableTo = tableName;

                    if (fld[i].IndexOf(".") > 0)
                    {
                        Spl = fld[i].Split(new char[] { '.' });
                    }

                    RecordPtrDB r_idsx = new RecordPtrDB();
                    if (Spl != null)
                    {
                        string Vr = "";
                        for (int r = 0; r < Spl.Count() - 1; r++)
                        {
                            recDB = GetTableFromORM(Spl[r], r == 0 ? TableName2 : recDB.NameTableTo);
                            Spl[r] = recDB.NameTableTo;
                            if (r == 0) Vr = recDB.Name;

                            if (recDB.Name == "StationA")
                            {
                                foreach (List<RecordPtrDB> RT in Val_Mass_Add)
                                {
                                    RecordPtrDB RTE = RT.Find(vr => vr.NameTableTo == recDB.NameTableTo && (vr.Index == 1));
                                    if (RTE != null)
                                    {
                                        recDB.Index = RTE.Index;
                                        r_idsx.Index = recDB.Index;
                                        r_idsx.KeyValue = i;
                                        r_idsx.Name = Spl[Spl.Count()-1];
                                        break;
                                    }
                                   
                                }
                            }

                            if (recDB.Name == "StationB")
                            {
                                foreach (List<RecordPtrDB> RT in Val_Mass_Add)
                                {
                                    RecordPtrDB RTE = RT.Find(vr => vr.NameTableTo == recDB.NameTableTo && (vr.Index == 2));
                                    if (RTE != null)
                                    {
                                        recDB.Index = RTE.Index;
                                        r_idsx.KeyValue = i;
                                        r_idsx.Index = recDB.Index;
                                        r_idsx.Name = Spl[Spl.Count() - 1];
                                        break;
                                    }

                                }
                            }

                        }

                        IndexVal.Add(r_idsx); 
                    }

                    if (Spl == null)
                    {
                        r_idsx = new RecordPtrDB();
                        r_idsx.Index = 0;
                        r_idsx.Name = fld[i];
                        r_idsx.KeyValue = i;

                        IndexVal.Add(r_idsx); 

                        Lst_Fld_.Add(fld[i]);

                        object dbx_d = new object();
                        double dou = -1;
                        int doi = -1;

                        if (!int.TryParse(values[i].ToString(), out doi))
                        {
                            if (!double.TryParse(values[i].ToString(), out dou))
                            {
                                dbx_d = values[i].ToString();
                            }
                            else
                            {
                                dbx_d = dou;
                            }
                        }
                        else
                        {
                            dbx_d = doi;
                        }


                        Lst_Val_.Add(dbx_d);
                    }
                }

                Lst_Fld_.Add("STATUS");
                Lst_Val_.Add("A");





                ID = NewRecord(tableName, Lst_Fld_.ToArray(), Lst_Val_.ToArray(), GetPrimaryField(tableName), Connect_Main_);



                for (int i = 0; i < fld.Count(); i++)
                {
                    if (i == 0) { TableName2 = tableName.Trim(); ID_Z = ID; } //else { tableName = TableName2; }

                    NameFieldException = fld[i];
                    string JoinTab = "";
                    string[] Spl = null;

                    List<RecordPtrDB> recDB_Lst = new List<RecordPtrDB>();
                    Dictionary<string, RecordPtrDB> recDic = new Dictionary<string, RecordPtrDB>();

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
                            recDic = GetListMandatoryFields(recDB.NameTableTo);
                            Spl[r] = recDB.NameTableTo;

                            foreach (List<RecordPtrDB> RT in Val_Mass_Add)
                            {
                                RecordPtrDB RTE = RT.Find(vr => vr.NameTableTo == recDB.NameTableTo && vr.Index == i + 1);
                               if (RTE != null)
                               {
                                   recDB.Index = RTE.Index;
                               }
                            }

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

                                    if (ID == -1) ID = ID_Z;
                                    object vl = GetOneFieldValue(tableName, ID, recDB_Lst[kk].FieldJoinFrom);
                                    if (vl != null) { ID_Val = (int)vl; } else { ID_Val = 2147483647; }
                                    recDB_Lst[kk].Value = GetOneFieldValue(recDB_Lst[kk].NameTableTo, string.Format("[{0}]={1}", recDB_Lst[kk].FieldJoinTo, ID_Val), Spl[Spl.Count() - 1]);
                                    if (((ID_Val > -1) && (ID_Val < 2147483640)))
                                    {
                                        SetFieldValue(recDB_Lst[kk].NameTableTo, string.Format("[{0}]={1}", recDB_Lst[kk].FieldJoinTo, ID_Val), Spl[Spl.Count() - 1], values[i]);
                                    }
                                    else
                                    {

                                     
                                        foreach (List<RecordPtrDB> RT in Val_Mass_Add)
                                        {
                                            int ID_ = -1;
                                            
                                            //List<RecordPtrDB> Rpt = RT.FindAll(a => a.NameTable == recDB_Lst[kk].NameTable && (Lst_Val_Index.Count() > 0 ? a.Index > Lst_Val_Index[Lst_Val_Index.Count() - 1] : a.Index > 0));
                                            List<RecordPtrDB> Rpt = RT.FindAll(a => a.NameTableTo == recDB_Lst[kk].NameTableTo && a.Index == recDB_Lst[kk].Index || recDB_Lst[kk].Index == 0);
                                            if (Rpt != null)
                                            {


                                                if (Rpt.Count() > 0)
                                                {
                                                    List<string> fld_lst = new List<string>();
                                                    List<object> val_lst = new List<object>();

                                                    foreach (RecordPtrDB item in Rpt)
                                                    {
                                                        //if (!Lst_Val_Index.Contains(item.Index))
                                                            //Lst_Val_Index.Add(item.Index);

                                                        fld_lst.Add(item.FieldJoinTo);
                                                        //val_lst.Add(item.Value);

                                                        object dbx_d = new object();
                                                        switch (item.TypeVal)
                                                        {
                                                            case OrmVarType.var_Dou:
                                                                dbx_d = Convert.ChangeType(item.Value, typeof(double));
                                                                break;
                                                            case OrmVarType.var_Int:
                                                                dbx_d = Convert.ChangeType(item.Value, typeof(int));
                                                                break;
                                                            case OrmVarType.var_String:
                                                                dbx_d = item.Value;
                                                                break;
                                                            default:
                                                                dbx_d = item.Value;
                                                                break;
                                                        }
                                                        val_lst.Add(dbx_d);

                                                    }
                                                    fld_lst.Add(Spl[Spl.Count() - 1]);

                                                    RecordPtrDB rcx = IndexVal.Find(tt => tt.Index == recDB_Lst[kk].Index && tt.Name == Spl[Spl.Count() - 1]);
                                                    if (rcx != null)
                                                    {
                                                        //val_lst.Add(values[i]);
                                                        val_lst.Add(values[rcx.KeyValue]);
                                                        
                                                    }


                                                    ID_ = NewRecord(recDB_Lst[kk].NameTableTo, fld_lst.ToArray(), val_lst.ToArray(), GetPrimaryField(recDB_Lst[kk].NameTableTo), Connect_Main_);

                                                    if (((ID_ > -1) && (ID_ < 2147483640)) && ((ID > -1) && (ID < 2147483640)))
                                                    {
                                                        SetFieldValue(tableName, ID, recDB_Lst[kk].FieldJoinFrom, ID_);
                                                    }

                                                    

                                                    break;
                                                }


                                            }
                                        }

                                        /*
                                        int ID_ = NewRecord(recDB_Lst[kk].NameTable, new string[] { Spl[Spl.Count() - 1] }, new object[] { values[i] }, recDB_Lst[kk].FieldJoinTo, Connect_);
                                        if ((ID_ > -1) && (ID_ < 2147483640))
                                        {
                                            SetFieldValue(tableName, ID, recDB_Lst[kk].FieldJoinFrom, ID_);
                                        }
                                         */
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

                                    if (((ID_Val > -1) && (ID_Val < 2147483640)))
                                    {
                                        SetFieldValue(recDB_Lst[kk].NameTableTo, string.Format("[{0}]={1}", recDB_Lst[kk].FieldJoinTo, ID_Val), Spl[Spl.Count() - 1], values[i]);
                                    }


                                    bool isSuccess = false;
                                   

                                        foreach (List<RecordPtrDB> RT in Val_Mass_Add)
                                        {
                                            List<RecordPtrDB> Rpt = RT.FindAll(a => a.NameTableTo == recDB_Lst[kk].NameTableTo && a.Index == recDB_Lst[kk].Index || recDB_Lst[kk].Index == 0);
                                            
                                            if (Rpt != null)
                                            {
                                                if (Rpt.Count() > 0)
                                                {
                                                    List<string> fld_lst = new List<string>();
                                                    List<object> val_lst = new List<object>();

                                                    foreach (RecordPtrDB item in Rpt)
                                                    {
                                                       // if (!Lst_Val_Index.Contains(item.Index))
                                                       //     Lst_Val_Index.Add(item.Index);

                                                        fld_lst.Add(item.FieldJoinTo);
                                                        //val_lst.Add(item.Value);

                                                        object dbx_d = new object();
                                                        switch (item.TypeVal)
                                                        {
                                                            case OrmVarType.var_Dou:
                                                                dbx_d = Convert.ChangeType(item.Value, typeof(double));
                                                                break;
                                                            case OrmVarType.var_Int:
                                                                dbx_d = Convert.ChangeType(item.Value, typeof(int));
                                                                break;
                                                            case OrmVarType.var_String:
                                                                dbx_d = item.Value;
                                                                break;
                                                            default:
                                                                dbx_d = item.Value;
                                                                break;
                                                        }
                                                        val_lst.Add(dbx_d);

                                                    }

                                                    fld_lst.Add(recDB_Lst[kk].FieldJoinTo);
                                                    val_lst.Add(ID_Val);

                                                    ID_Val = NewRecord(recDB_Lst[kk].NameTableTo, fld_lst.ToArray(), val_lst.ToArray(), GetPrimaryField(recDB_Lst[kk].NameTableTo), Connect_Main_); recDB_Lst[kk].KeyValue = ID_Val;

                                                                                                       
                                                    isSuccess = true;
                                                    break;
                                                }

                                                //ID_Val = NewRecord(recDB_Lst[kk].NameTable, new string[] { recDB_Lst[kk].FieldJoinTo }, new object[] { ID_Val }, GetPrimaryField(recDB_Lst[kk].NameTable), Connect_); recDB_Lst[kk].KeyValue = ID_Val;
                                            }

                                            
                                            
                                        }



                                        if (!isSuccess)
                                        {
                                            if (kk >= 1)
                                            {
                                                ID_Val = (int)GetOneFieldValue(recDB_Lst[kk - 1].NameTableTo, recDB_Lst[kk].KeyValue, recDB_Lst[kk].FieldJoinFrom);
                                            }
                                            else ID_Val = (int)GetOneFieldValue(TableName2, ID_Z, recDB_Lst[kk].FieldJoinFrom);

                                            obj = GetOneFieldValue(recDB_Lst[kk].NameTableTo, string.Format("[{0}]={1}", recDB_Lst[kk].FieldJoinTo, ID_Val), recDB_Lst[kk].FieldJoinTo);
                                            if (obj != null) { ID_Val = (int)obj; recDB_Lst[kk].KeyValue = ID_Val; }
                                            else { ID_Val = NewRecord(recDB_Lst[kk].NameTableTo, new string[] { recDB_Lst[kk].FieldJoinTo }, new object[] { ID_Val }, GetPrimaryField(recDB_Lst[kk].NameTableTo), Connect_Main_); recDB_Lst[kk].KeyValue = ID_Val; }
                                        }
                                    
                                }

                                
                                 



                            }


                        }
                        //SetFieldValue(recDB.NameTable,recDB.KeyValue,Spl[Spl.Count()-1],
                    }
                    else
                    {
                        if (!CheckFieldPrimary(fld[i], TableName2))
                        {
                            SetFieldValue(TableName2, ID_Z, fld[i], values[i]);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                NewRec = -1;
                ID_Z = -1;
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in NewRecord: (" + tableName + ") Field: " + NameFieldException + " - " + ex.Message);
            }
            return ID_Z;
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
                                    if (vl != null) { ID_Val = (int)vl; } else { ID_Val = 2147483647; }
                                    recDB_Lst[kk].Value = GetOneFieldValue(recDB_Lst[kk].NameTableTo, string.Format("[{0}]={1}", recDB_Lst[kk].FieldJoinTo, ID_Val), Spl[Spl.Count() - 1]);
                                    if ((!((ID_Val > -1) && (ID_Val < 2147483640))) || (ID_Val == ConnectDB.NullI))
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
            catch (Exception)
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
            if (Connect_Main_!= null)
            {
                using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
            //using (DbLinker lk = ConnectDB.New())
            if (Connect_Main_!= null)
            {

                using (Yyy y = Yyy.CreateObject(table_name, Connect_Main_))
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
            if (Connect_Main_!= null)
            {
                using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
                    {
                        if ((!string.IsNullOrEmpty(fld)))
                        {
                            if (y.Fetch(string.Format("[{0}]={1}", fld, value)))
                            {
                                y.Delete();
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
        /// Удаление записи таблицы tableName, соответствующей фильтру filter
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="filter"> поле для фильтра</param>

        public void DeleteRecord(string tableName, string[] fld, object[] values, bool[] IsPKField)
        {
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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
            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            y.Fetch(filter);
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
        /// Установка нового значения поля fld для перечня записей, удовлетворяющих фильтру filter
        /// </summary> 
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="filter"> Фильтр данных </param>
        /// <param name="fld"> поле для установки нового значения </param>
        /// <param name="value"> Новое значение</param>
        public bool SetFieldValue(string tableName, string filter, string fld, object value, DbLinker lk)
        {
            bool isSuccessEdit = false;
            try
            {
                using (Yyy y = Yyy.CreateObject(tableName, lk))
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        y.Fetch(filter);
                    }
                    y.Set(fld, value);
                    y.Save();
                    isSuccessEdit = true;
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in SetFieldValue: " + ex.Message);
            }
            return isSuccessEdit;
        }


        public void SetRecordsCompareValue(string tableName, string[] fld, object[] values, bool[] CompareValue, string WorkFlowField, object StatusWorkFlow)
        {

            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
                    {
                        if ((!string.IsNullOrEmpty(GenerateFormat(CompareValue, fld, values, true))))
                        {
                            if (y.Fetch(GenerateFormat(CompareValue, fld, values, true)))
                            {
                                y.OpenRs();
                                if (!y.IsEOF())
                                {

                                    y.Set(WorkFlowField, StatusWorkFlow);

                                }
                                y.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CLogs.WriteError(ELogsWhat.DataDriver_ConnectDB, "Error in SetRecordsCompareValue: " + ex.Message);
            }
        }

        /// <summary>
        /// Установка нового значения поля fld для запиcи с номером rec_id
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="rec_id">Номер записи </param>
        /// <param name="fld">Наименование поля </param>
        /// <param name="value">Значение поля</param>
        public void SetFieldValue(string tableName, int rec_id, string fld, object value)
        {

            try
            {
                //using (DbLinker lk = ConnectDB.New())
                if (Connect_Main_!= null)
                {
                    using (Yyy y = Yyy.CreateObject(tableName, Connect_Main_))
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

        static public OrmLinker GetOrmRs_Main()
        {
            return lk_main_;
        }

        static public OrmLinker GetOrmRs_Secondary()
        {
            return lk_secondary_;
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
                NewRecord("SYS_LOGS", new string[] { "ID", "EVENT", "TABLE_NAME", "LCOUNT", "INFO1", "WHO", "WHEN" }, new object[] { (ID == 2147483647) ? 1 : ID + 1, who.ToString().Length > 20 ? who.ToString().Substring(0, 19) : who.ToString(), TableName, 1, message, User, DateTime.Now }, new bool[] { true, false, false, false, false, false, false });
            }
            catch (Exception)
            {
                
            }
        }

        public void TST()
        {
            EditRecord("MOB_STATION", 13407, new string[] { "Position.NAME"}, new object[] { "VAR1" });
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
            rs.SetAdditionalFilter(table, filter);
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
                        if ((double)c == 1e-99) c = 0x7FFFFFFF; else c = (int)(double)c;
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
            OrmRs rs = new OrmRs();
            rs.Init(lk == null ? OrmSchema.Linker : lk);
            int count = flds.GetLength(0);
            OrmItem[] it = new OrmItem[count];
            OrmVarType[] vt = new OrmVarType[count];


            for (int i = 0; i < count; i++)
            {
                string fld = flds[i]; if (fld.StartsWith("[") && fld.EndsWith("]")) fld = fld.Substring(1, fld.Length - 2);

                bool isExpr = false;
                if (LstOrmItemExpr!=null) 
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
            rs.SetAdditionalFilter(table, filter);
            rs.SetExternalOrder(table, order);
            List<object[]> dataset = new List<object[]>();
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
                            if ((double)c == 1e-99) c = 0x7FFFFFFF; else c = (int)(double)c;
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
            return dataset;
        }
      

    }
}
