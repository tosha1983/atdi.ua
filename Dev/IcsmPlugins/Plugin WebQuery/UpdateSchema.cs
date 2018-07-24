using ICSM;
using OrmCs;

namespace XICSM.Atdi.Icsm.Plugins.WebQuery
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
            s.DeclareTable("XWEBQUERY", "Web query", plugin4);
            {
                s.DeclareField("ID", "NUMBER(9,0)", null, "NOTNULL", null);
                s.DeclareIndex("PK_XWEBQUERY", "PRIMARY", "ID");
                s.DeclareField("NAME", "VARCHAR(50)", null, null, null);
                s.DeclareField("QUERY", "BINARY(20000)", null, null, null);
                s.DeclareField("COMMENTS", "VARCHAR(250)", null, null, null);
                s.DeclareField("IDENTUSER", "VARCHAR(250)", null, null, null);
                s.DeclareField("CODE", "VARCHAR(50)", null, null, null);
                s.DeclareField("TASKFORCEGROUP", "VARCHAR(100)", null, null, null);
            }


            s.DeclareTable("XWEBCONSTRAINT", "Web constraints", plugin4);
            {
                s.DeclareField("ID", "NUMBER(9,0)", null, "NOTNULL", null);
                s.DeclareIndex("PK_XWEBCONSTRAINT", "PRIMARY", "ID");
                s.DeclareField("WEBQUERYID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("NAME", "VARCHAR(50)", null, null, null);
                s.DeclareField("PATH", "VARCHAR(250)", null, null, null);
                s.DeclareField("MIN", "NUMBER(22,8)", null, null, null);
                s.DeclareField("MAX", "NUMBER(22,8)", null, null, null);
                s.DeclareField("STRVALUE", "VARCHAR(250)", null, null, null);
                s.DeclareField("DATEVALUEMIN", "DATE", "Date", null, null);
                s.DeclareField("INCLUDE", "NUMBER(1,0)", null, null, null);
                s.DeclareField("DATEVALUEMAX", "DATE", "Date", null, null);
                s.DeclareJoin("JoinWebQuery", "XWEBQUERY", null, "WEBQUERYID", "ID");
            }

            s.DeclareTable("XUPDATEOBJECTS", "Update objects", plugin4);
            {
                s.DeclareField("ID", "NUMBER(9,0)", null, "NOTNULL", null);
                s.DeclareIndex("PK_XUPDATEOBJECTS", "PRIMARY", "ID");
                s.DeclareField("OBJTABLE", "VARCHAR(50)", null, null, null);
                s.DeclareField("DATEMODIFIED", "DATE", "Date", null, null);
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
            if (dbCurVersion < 20180105.0949)
            {
                //s.CreateTables("XWEBQUERY,XWEBCONSTRAINT,XUPDATEOBJECTS");
                //s.CreateTableFields("XWEBCONSTRAINT", "ID,WEBQUERYID,NAME,PATH,MIN,MAX,STRVALUE,DATEVALUEMIN,INCLUDE,DATEVALUEMAX");
                //s.CreateTableFields("XWEBQUERY", "ID,NAME,QUERY,COMMENTS,IDENTUSER,CODE,TASKFORCEGROUP");
                //s.CreateTableFields("XUPDATEOBJECTS", "ID,OBJTABLE,DATEMODIFIED");
                //s.CreateTables("XUPDATEOBJECTS");
                //s.CreateTableFields("XUPDATEOBJECTS", "ID,OBJTABLE,DATEMODIFIED");
                s.SetDatabaseVersion(20180105.0949);
            }
            return true;
        }

        //=============================================================
        /// <summary>
        /// Текущая версия БД плагина
        /// </summary>
        public static readonly double schemaVersion = 20180105.0949;//20161003.0909
    }
}
