using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DAL;
using System.Text;
using System.Web;
using System.Web.SessionState;
using OnlinePortal;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OnlinePortal.Utils;
using LitvaPortal.Utils;
using Utils;
using LitvaPortal.ServiceReference_WebQuery;

namespace LitvaPortal
{

    public class SettingIRPClass
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public int MAX_REC { get; set; }
        public string STATUS_ { get; set; }
        public string DESCRIPTION { get; set; }
        public bool IS_VISIBLE { get; set; }
        public string Query { get; set; }
        public int MAX_COLUMNS { get; set; }
        public string Ident_User { get; set; }
        public bool isCorrectQuery { get; set; }
        public int MaxID { get; set; }
        public int MinID { get; set; }
        public int MAX_REC_PAGE { get; set; }
        public string GROUP_FIELD { get; set; }
        public int index_group_field { get; set; }
        public bool IS_SQL_REQUEST { get; set; }
        public bool IS_VISIBLE_ALL_REC { get; set; }
        public string COMMENTS { get; set; }
        public bool IS_FILL_COLOR { get; set; }
        public string SYS_COORD { get; set; }
        public bool IS_SEND_NOTIFY_MESS { get; set; }
        public bool IS_TESTING_REQUEST { get; set; }
        public string EMAIL_QUESTION { get; set; }        
        public string EMAIL_ANSWER { get; set; }        
        public bool IS_ENABLE_INTERVAL { get; set; }
        public int MAX_VAL_INTERVAL { get; set; }
        public int MIN_VAL_INTERVAL { get; set; }
        public int MAX_VAL { get; set; }
        public bool IS_VALID_LICENSE { get; set; }
        public int TOTAL_COUNT_GROUP { get; set; }
        public int HIDDEN_COLUMNS { get; set; }
        public string TYPE2 { get; set; }
        public string TYPE3 { get; set; }
        public string ADM_NAME { get; set; }
        public string TYPE { get; set; }
        public SettingIRPClass()
        {
            index_group_field = -1;
        }


        ~SettingIRPClass()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }


}