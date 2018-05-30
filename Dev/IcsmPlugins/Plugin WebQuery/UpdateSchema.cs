using ICSM;
using OrmCs;

namespace XICSM.WebQuery
{
    public class UpdateSchema
    {
        //=============================================================
        // hotfix 12.12.2013 
        // имя поля DIAGА таблицы XFA_ANTEN  содержит русскую букву А - исправлено
        // имя поля DIAGА представления XV_ANTEN содержит русскую букву А -исправлено
        //  размер поля ADDRESS представления ALLSTATIONS не соответствует размер поля в базе (4000) - исправлено
        //=============================================================
        // Описание типов ICSM
        private const string Standard = "Telsys";
        private const string FreqMHz = "F/MHz";
        private const string FreqHz = "F/Hz";
        private const string BwKHz = "BW/kHz";
        private const string DesigEm = "DesigEm";
        private const string MbPerSec = "MBitps";
        private const string dBm = "dBm";
        private const string dB = "dB";
        private const string dBW = "dBW";
        private const string W = "W";
        private const string dBWpHz = "dBW/Hz";
        private const string Number = "Number";
        private const string Unsigned = "Unsigned";
        private const string Longitude = "Longitude";
        private const string Latitude = "Latitude";
        private const string Date = "Date";
        private const string Distance = "Dist";
        private const string Metre = "m";
        private const string Degrees = "deg";
        private const string Asl = "ASML";
        private const string Agl = "AGL";
        private const string Hour = "Hour";
        private const string YesNo = "eri_YesNo";
        //=============================================================
        private const string plugin1 = "PLUGIN_1,97";
        private const string plugin2 = "PLUGIN_2,98";
        private const string plugin3 = "PLUGIN_3,99";
        private const string plugin3plus = "PLUGIN_3,99";
        private const string plugin4 = "PLUGIN_4,100";
        //=============================================================
        private const string RefAccessDbms = "DBMS";
        //=============================================================
        /// <summary>
        /// Регистрирует плагиновские таблицы в ICSM
        /// </summary>
        /// <param name="s">Схема ICSM</param>
        public static void RegisterSchema(IMSchema s)
        {
            //===============================================
            // TABLES
            //===============================================
            s.DeclareTable("XWebQuery", "WEB_QUERY", plugin4);
            {
                s.DeclareField("ID", "NUMBER(9,0)", null, "NOTNULL", null);
                s.DeclareIndex("PK_WebQuery", "PRIMARY", "ID");
                s.DeclareField("Name", "VARCHAR(50)", null, null, null);
                s.DeclareField("Status", "VARCHAR(3)", null, null, null);
                s.DeclareField("Query", "BINARY(20000)", null, null, null);
                s.DeclareField("Comments", "VARCHAR(250)", null, null, null);
                s.DeclareField("IdentUser", "VARCHAR(250)", null, null, null);
                s.DeclareField("IsSqlRequest", "NUMBER(9,0)", null, null, null);
                s.DeclareField("RightGroup", "VARCHAR(100)", null, null, null);
            }



            s.DeclareTable("XWebConstraint", "Web constraints", plugin4);
            {
                s.DeclareField("ID", "NUMBER(9,0)", null, "NOTNULL", null);
                s.DeclareIndex("PK_XWebConstraint", "PRIMARY", "ID");
                s.DeclareField("WebQueryId", "NUMBER(9,0)", null, null, null);
                s.DeclareField("Name", "VARCHAR(50)", null, null, null);
                s.DeclareField("Path", "VARCHAR(250)", null, null, null);
                s.DeclareField("Min", "NUMBER(22,8)", null, null, null);
                s.DeclareField("Max", "NUMBER(22,8)", null, null, null);
                s.DeclareField("StrValue", "VARCHAR(250)", null, null, null);
                s.DeclareField("DateValueMin", "DATE", "Date", null, null);
                s.DeclareField("Include", "NUMBER(1,0)", null, null, null);
                s.DeclareField("DateValueMax", "DATE", "Date", null, null);
                s.DeclareJoin("JoinWebQuery", "XWebQuery", null, "WebQueryId", "ID");
            }
        }
     
        //=============================================================
        /// <summary>
        /// Обновление таблиц
        /// </summary>
        /// <param name="s">Схема ICSM</param>
        /// <param name="dbCurVersion">Текущая версия БД</param>
        /// <returns>true</returns>
        public static bool UpgradeDatabase(IMSchema s, double dbCurVersion)
        {
            if (dbCurVersion < 20180105.0946)
            {
                s.CreateTables("XWebQuery,XWebConstraint");
                s.CreateTableFields("XWebConstraint", "ID,WebQueryId,Name,Path,Min,Max,StrValue,DateValueMin,DateValueMax,Include");
                s.CreateTableFields("XWebQuery", "ID,Name,Status,Query,Comments,IdentUser,IsSqlRequest,RightGroup");
                s.SetDatabaseVersion(20180105.0946);
            }
            return true;
        }

        //=============================================================
        /// <summary>
        /// Текущая версия БД плагина
        /// </summary>
        public static readonly double schemaVersion = 20180105.0946;//20161003.0909
    }
}
