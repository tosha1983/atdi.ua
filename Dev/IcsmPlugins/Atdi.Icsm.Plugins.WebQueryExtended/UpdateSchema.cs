using ICSM;
using System;

namespace XICSM.Atdi.Icsm.Plugins.WebQueryExtended
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
            // VIEWS
            //===============================================
            s.DeclareView("XV_WEB_BC", "WebQuery_BS-3_View", plugin3);
            {
                s.DeclareField("LICENCE", "VARCHAR(200)", null, null, null);
                s.DeclareField("LIC_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("CONC_NUMBER", "VARCHAR(200)", null, null, null);
                s.DeclareField("CONC_DATE_FROM", "DATE", "Date", null, null);
                s.DeclareField("DOZV_NUMBER", "VARCHAR(200)", null, null, null);
                s.DeclareField("DOZV_DATE_FROM", "DATE", "Date", null, null);
                s.DeclareField("EQUIP_NAME", "VARCHAR(50)", null, null, null);
                s.DeclareField("EQUIP_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("MODULATION", "VARCHAR(20)", null, null, null);
                s.DeclareField("ADDRESS", "VARCHAR(4000)", null, null, null);
                s.DeclareField("POS_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("LONGITUDE", "NUMBER(10,6)", null, null, null);
                s.DeclareField("LATITUDE", "NUMBER(10,6)", null, null, null);
                s.DeclareField("POWER", "NUMBER(22,8)", null, null, null);
                s.DeclareField("ANTENNA_NAME", "VARCHAR(50)", null, null, null);
                s.DeclareField("ANTENNA_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("GAIN", "NUMBER(6,2)", null, null, null);
                s.DeclareField("AGL", "NUMBER(5,1)", null, null, null);
                s.DeclareField("ANGLE_ELEV", "NUMBER(5,2)", null, null, null);
                s.DeclareField("DIAG", "VARCHAR(4000)", null, null, null);
                s.DeclareField("POLARIZATION", "VARCHAR(4)", null, null, null);
                s.DeclareField("AZIMUTH", "VARCHAR(4000)", null, null, null);
                s.DeclareField("TX_FREQ", "VARCHAR(200)", null, null, null);
                s.DeclareField("RX_FREQ", "VARCHAR(200)", null, null, null);
                s.DeclareField("DES_EMISSION", "VARCHAR(9)", null, null, null);
                s.DeclareField("SECTOR_NUMBER", "VARCHAR(1)", null, null, null);
                s.DeclareField("EDRPOU", "VARCHAR(50)", null, null, null);

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
            /*
            if (dbCurVersion < 20180105.0949)
            {
                s.CreateTables("XWEBQUERY,XWEBCONSTRAINT,XUPDATEOBJECTS");
                s.CreateTableFields("XWEBCONSTRAINT", "ID,WEBQUERYID,NAME,PATH,MIN,MAX,STRVALUE,DATEVALUEMIN,INCLUDE,DATEVALUEMAX");
                s.CreateTableFields("XWEBQUERY", "ID,NAME,QUERY,COMMENTS,IDENTUSER,CODE,TASKFORCEGROUP");
                s.SetDatabaseVersion(20180105.0949);
            }
            if (dbCurVersion < 20181128.0949)
            {
                s.CreateTableFields("XWEBQUERY", "VIEWCOLUMNS,ADDCOLUMNS,EDITCOLUMNS,TABLECOLUMNS");
                s.CreateTables("XWEBQUERYATTRIBUTES");
                s.CreateTableFields("XWEBQUERYATTRIBUTES", "ID,WEBQUERYID,PATH,READONLY,NOTCHANGEADD,NOTCHANGEEDIT");
                s.SetDatabaseVersion(20181128.0949);
            }
            if (dbCurVersion < 20181129.0949)
            {
                s.CreateTableFields("XWEBCONSTRAINT", "STRVALUETO,MOMENTOFUSE,DEFAULTVALUE,MESSAGENOTVALID,OPERCONDITION,TYPECONDITION,DESCRCONDITION");
                s.SetDatabaseVersion(20181129.0949);
            }
            */
            return true;
        }
        //=============================================================
        /// <summary>
        /// Текущая версия БД плагина
        /// </summary>
        public static readonly double schemaVersion = 20180105.0949;//20161003.0909
        //public static readonly double schemaVersion = 20181129.0949;//20161003.0909
    }
}
