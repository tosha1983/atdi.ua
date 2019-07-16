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
            CreateViewForAccess(s, "XV_WEB_BS", "WebQuery_BS_View", plugin3, "MOB_STATION2", "WebQuery_BS");
            {
                //s.DeclareField("ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("LICENCE", "VARCHAR(200)", null, null, null);
                s.DeclareField("LIC_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("RADIOTECH", "VARCHAR(10)", null, null, null);
                s.DeclareField("RADIOTECH_NAME", "VARCHAR(255)", null, null, null);
                s.DeclareField("IEEE", "VARCHAR(100)", null, null, null);
                s.DeclareField("CONC_NUMBER", "VARCHAR(200)", null, null, null);
                s.DeclareField("CONC_DATE_FROM", "DATE", "Date", null, null);
                s.DeclareField("CONC_DATE_TO", "DATE", "Date", null, null);
                s.DeclareField("DOZV_NUMBER", "VARCHAR(200)", null, null, null);
                s.DeclareField("DOZV_DATE_FROM", "DATE", "Date", null, null);
                s.DeclareField("DOZV_DATE_TO", "DATE", "Date", null, null);
                s.DeclareField("DOZV_DATE_CANCEL", "DATE", "Date", null, null);
                s.DeclareField("EQUIP_NAME", "VARCHAR(50)", null, null, null);
                s.DeclareField("EQUIP_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("MODULATION", "VARCHAR(4000)", null, null, null);
                s.DeclareField("ADDRESS", "VARCHAR(4000)", null, null, null);
                s.DeclareField("POS_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("LONGITUDE", "NUMBER(10,6)", null, null, null);
                s.DeclareField("LATITUDE", "NUMBER(10,6)", null, null, null);
                s.DeclareField("PROVINCE", "VARCHAR(50)", null, null, null);
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
                s.DeclareField("STATUS", "VARCHAR(4)", null, null, null);
                s.DeclareField("IDENT_REZ", "VARCHAR(100)", null, null, null);
                s.DeclareField("SCANPATH_CONC", "VARCHAR(500)", null, null, null);
                s.DeclareField("SCANPATH_DOZV", "VARCHAR(500)", null, null, null);
            }

            //===============================================
            CreateViewForAccess(s, "XV_WEB_RR", "WebQuery_RR_View", plugin3, "MOB_STATION", "WebQuery_RR");
            {
                //s.DeclareField("ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("OBJ_ID1", "NUMBER(9,0)", null, null, null);
                s.DeclareField("LICENCE", "VARCHAR(4000)", null, null, null);
                s.DeclareField("LIC_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("RADIOTECH", "VARCHAR(10)", null, null, null);
                s.DeclareField("RADIOTECH_NAME", "VARCHAR(255)", null, null, null);
                s.DeclareField("IEEE", "VARCHAR(100)", null, null, null);
                s.DeclareField("CONC_NUMBER", "VARCHAR(200)", null, null, null);
                s.DeclareField("CONC_DATE_FROM", "DATE", "Date", null, null);
                s.DeclareField("CONC_DATE_TO", "DATE", "Date", null, null);
                s.DeclareField("DOZV_NUMBER", "VARCHAR(200)", null, null, null);
                s.DeclareField("DOZV_DATE_FROM", "DATE", "Date", null, null);
                s.DeclareField("DOZV_DATE_TO", "DATE", "Date", null, null);
                s.DeclareField("DOZV_DATE_CANCEL", "DATE", "Date", null, null);
                s.DeclareField("EM_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("EQUIP_NAME", "VARCHAR(50)", null, null, null);
                s.DeclareField("EQUIP_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("EQ_FREQ_RANGE", "VARCHAR(164)", null, null, null);
                s.DeclareField("DUPLEX", "NUMBER(15,6)", null, null, null);
                s.DeclareField("BW", "NUMBER(15,5)", null, null, null);
                s.DeclareField("STEP", "NUMBER(22,8)", null, null, null);
                s.DeclareField("MODULATION", "VARCHAR(4000)", null, null, null);
                s.DeclareField("ADDRESS", "VARCHAR(4000)", null, null, null);
                s.DeclareField("POS_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("LONGITUDE", "NUMBER(10,6)", null, null, null);
                s.DeclareField("LATITUDE", "NUMBER(10,6)", null, null, null);
                s.DeclareField("PROVINCE", "VARCHAR(50)", null, null, null);
                s.DeclareField("POWER", "NUMBER(22,8)", null, null, null);
                s.DeclareField("ANTENNA_NAME", "VARCHAR(50)", null, null, null);
                s.DeclareField("ANTENNA_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("GAIN", "VARCHAR(274)", null, null, null);
                s.DeclareField("AGL", "VARCHAR(274)", null, null, null);
                s.DeclareField("ANGLE_ELEV", "VARCHAR(274)", null, null, null);
                s.DeclareField("ANGLE_ELEV_E", "VARCHAR(40)", null, null, null);
                s.DeclareField("DIAG", "VARCHAR(4000)", null, null, null);
                s.DeclareField("POLARIZATION", "VARCHAR(4)", null, null, null);
                s.DeclareField("AZIMUTH", "VARCHAR(274)", null, null, null);
                s.DeclareField("TX_LOSSES", "VARCHAR(274)", null, null, null);
                s.DeclareField("CHANNELS", "VARCHAR(4000)", null, null, null);
                s.DeclareField("TX_FREQ", "VARCHAR(4000)", null, null, null);
                s.DeclareField("RX_FREQ", "VARCHAR(4000)", null, null, null);
                s.DeclareField("DES_EMISSION", "VARCHAR(9)", null, null, null);
                s.DeclareField("SECTOR_NUMBER", "VARCHAR(1)", null, null, null);
                s.DeclareField("EDRPOU", "VARCHAR(50)", null, null, null);
                s.DeclareField("STATUS", "VARCHAR(4)", null, null, null);
                s.DeclareField("IDENT_REZ", "VARCHAR(100)", null, null, null);
                s.DeclareField("SCANPATH_CONC", "VARCHAR(500)", null, null, null);
                s.DeclareField("SCANPATH_DOZV", "VARCHAR(500)", null, null, null);

            }

            //===============================================
            CreateViewForAccess(s, "XV_WEB_RS", "WebQuery_RS_View", plugin3, "MICROWS", "WebQuery_RS");
            {
                //s.DeclareField("ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("LICENCE", "VARCHAR(200)", null, null, null);
                s.DeclareField("LIC_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("RADIOTECH", "VARCHAR(10)", null, null, null);
                s.DeclareField("RADIOTECH_NAME", "VARCHAR(255)", null, null, null);
                s.DeclareField("IEEE", "VARCHAR(100)", null, null, null);
                s.DeclareField("CONC_NUMBER", "VARCHAR(200)", null, null, null);
                s.DeclareField("CONC_DATE_FROM", "DATE", "Date", null, null);
                s.DeclareField("CONC_DATE_TO", "DATE", "Date", null, null);
                s.DeclareField("DOZV_NUMBER", "VARCHAR(200)", null, null, null);
                s.DeclareField("DOZV_DATE_FROM", "DATE", "Date", null, null);
                s.DeclareField("DOZV_DATE_TO", "DATE", "Date", null, null);
                s.DeclareField("DOZV_DATE_CANCEL", "DATE", "Date", null, null);
                s.DeclareField("EQUIP_NAME", "VARCHAR(50)", null, null, null);
                s.DeclareField("ADDRESS", "VARCHAR(4000)", null, null, null);
                s.DeclareField("POS_ID", "VARCHAR(86)", null, null, null);
                s.DeclareField("LONGITUDE", "VARCHAR(86)", null, null, null);
                s.DeclareField("LATITUDE", "VARCHAR(86)", null, null, null);
                s.DeclareField("PROVINCE", "VARCHAR(106)", null, null, null);
                s.DeclareField("POWER", "VARCHAR(86)", null, null, null);
                s.DeclareField("ANTENNA_NAME", "VARCHAR(106)", null, null, null);
                s.DeclareField("ANTENNA_ID", "VARCHAR(86)", null, null, null);
                s.DeclareField("DIAMETER", "VARCHAR(86)", null, null, null);
                s.DeclareField("GAIN", "VARCHAR(86)", null, null, null);
                s.DeclareField("AGL", "VARCHAR(86)", null, null, null);
                s.DeclareField("POLARIZATION", "VARCHAR(14)", null, null, null);
                s.DeclareField("AZIMUTH", "VARCHAR(86)", null, null, null);
                s.DeclareField("MODULATION", "VARCHAR(4000)", null, null, null);
                s.DeclareField("TX_FREQ", "VARCHAR(86)", null, null, null);
                s.DeclareField("DES_EMISSION", "VARCHAR(9)", null, null, null);
                s.DeclareField("EDRPOU", "VARCHAR(50)", null, null, null);
                s.DeclareField("STATUS", "VARCHAR(4)", null, null, null);
                s.DeclareField("IDENT_REZ", "VARCHAR(10)", null, null, null);
                s.DeclareField("SCANPATH_CONC", "VARCHAR(500)", null, null, null);
                s.DeclareField("SCANPATH_DOZV", "VARCHAR(500)", null, null, null);
            }

            //===============================================
            CreateViewForAccess(s, "XV_WEB_LICENCE", "WebQuery_Licence_View", plugin3, "LICENCE", "WebQuery_Licence");
            {
                //s.DeclareField("ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("STANDARD", "VARCHAR(10)", null, null, null);
                s.DeclareField("RADIOTECH_NAME", "VARCHAR(255)", null, null, null);
                s.DeclareField("OWNER_ID", "NUMBER(9,0)", null, null, null);
                s.DeclareField("EDRPOU", "VARCHAR(50)", null, null, null);
                s.DeclareField("NUMBER_LIC", "VARCHAR(50)", null, null, null);
                s.DeclareField("STATUS", "VARCHAR(1)", null, null, null);
                s.DeclareField("ISSUED_BY", "VARCHAR(7)", null, null, null);
                s.DeclareField("SINGING_DATE", "DATE", "Date", null, null);
                s.DeclareField("START_DATE", "DATE", "Date", null, null);
                s.DeclareField("STOP_DATE", "DATE", "Date", null, null);
                s.DeclareField("END_DATE", "DATE", "Date", null, null);
                s.DeclareField("INSTEAD_OF", "VARCHAR(50)", null, null, null);
            }

            //===============================================
            CreateViewForAccess(s, "XV_WEB_RADIOTECH", "WebQuery_RadioTech_View", plugin3, "RADIO_SYSTEMS", "WebQuery_RadioTech");
            {
                s.DeclareField("STANDARD", "VARCHAR(54)", null, null, null);
                s.DeclareField("EDRPOU", "VARCHAR(50)", null, null, null);
                s.DeclareField("PROVINCE", "VARCHAR(1000)", null, null, null);
                s.DeclareField("STATUS", "VARCHAR(4)", null, null, null);
                s.DeclareField("COUNT_APPL", "NUMBER(9,0)", null, null, null);
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
            if (dbCurVersion < 20181203.0949)
            {
                s.SetDatabaseVersion(20181203.0949);
            }
            return true;
        }

        /// <summary>
        /// Создает схему вида для распределения доступа
        /// </summary>
        /// <param name="s"></param>
        private static void CreateViewForAccess(IMSchema s, string viewName, string viewDecs, string tableGroup, string tableName, string joinName)
        {
            s.DeclareView(viewName, viewDecs, tableGroup);
            s.DeclareField("ID", "NUMBER(9,0)", null, null, null);
            s.DeclareIndex("PK_" + viewName, "PRIMARY", "ID");
            s.DeclareJoin(joinName, tableName, null, "ID", "ID");
        }

        //=============================================================
        /// <summary>
        /// Текущая версия БД плагина
        /// </summary>
        public static readonly double schemaVersion = 20181203.0949;//20161003.0909
        //public static readonly double schemaVersion = 20181129.0949;//20161003.0909
    }
}
