using ICSM;
using System.Windows.Forms;

namespace XICSM.ICSControlClient
{
    public class UpdateSchema
    {
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
            s.DeclareTable("XPROTOCOL_REPORT", "ICS Control protocol report", plugin3);
            {
                s.DeclareField("ID", "NUMBER(9,0)", null, "NOTNULL", null);
                s.DeclareIndex("PK_XPROTOCOL_REPORT", "PRIMARY", "ID");
                s.DeclareField("DATE_CREATED", "DATE", "Date", null, null);
                s.DeclareField("STANDARD_NAME", "VARCHAR(150)", null, null, null);
                s.DeclareField("OWNER_NAME", "VARCHAR(100)", null, null, null);
                s.DeclareField("PERMISSION_NUMBER", "VARCHAR(200)", null, null, null);
                s.DeclareField("PERMISSION_START", "DATE", "Date", null, null);
                s.DeclareField("PERMISSION_STOP", "DATE", "Date", null, null);
                s.DeclareField("ADDRESS", "VARCHAR(1000)", null, null, null);
                s.DeclareField("LONGITUDE", "VARCHAR(40)", null, null, null);
                s.DeclareField("LATITUDE", "VARCHAR(40)", null, null, null);
                s.DeclareField("SENSOR_LON", "VARCHAR(40)", null, null, null);
                s.DeclareField("SENSOR_LAT", "VARCHAR(40)", null, null, null);
                s.DeclareField("SENSOR_NAME", "VARCHAR(100)", null, null, null);
                s.DeclareField("DATE_MEAS", "DATE", "Date", null, null);
                s.DeclareField("S_FREQ_MHZ", "NUMBER(30,10)", null, null, null);
                s.DeclareField("S_BW", "NUMBER(30,10)", null, null, null);
                s.DeclareField("FREQ_MHZ", "NUMBER(30,10)", null, null, null);
                s.DeclareField("BW", "NUMBER(30,10)", null, null, null);
                s.DeclareField("LEVEL_DBM", "NUMBER(30,10)", null, null, null);
                s.DeclareField("DESIG_EMISSION", "VARCHAR(25)", null, null, null);
                s.DeclareField("GLOBAL_SID", "VARCHAR(50)", null, null, null);
                s.DeclareField("CREATED_BY", "VARCHAR(150)", null, null, null);
                s.DeclareField("VISN", "VARCHAR(250)", null, null, null);
            }
        }
    }
}
