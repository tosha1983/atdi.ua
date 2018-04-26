using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XICSM.ICSControlClient.Metadata
{
    public class Allotments
    {
        public static readonly string TableName = "CH_ALLOTMENTS";

        public static class Fields
        {
            public static readonly string Id = "ID";
            public static readonly string TableName = "TABLE_NAME";
            public static readonly string Status = "STATUS";
            public static readonly string UserType = "USE_TYPE";
            public static readonly string CustDate1 = "CUST_DAT1";
            public static readonly string CustDate2 = "CUST_DAT2";
            public static readonly string CustText1 = "CUST_TXT1";
            public static readonly string CustText2 = "CUST_TXT2";
            public static readonly string CustText3 = "CUST_TXT3";
            public static readonly string CustText4 = "CUST_TXT4";
            public static readonly string MeasTaskId = "CUST_NBR1";
            public static readonly string CustNum2 = "CUST_NBR2";
            public static readonly string CustNum3 = "CUST_NBR3";
            
            public static class Plan
            {
                public static readonly string Id = "Plan.ID";
                public static readonly string Bandwidth = "Plan.BANDWIDTH";
                public static readonly string ChannelSep = "Plan.CHANNEL_SEP";
            }
        }

        public static class Statuses
        {
            public static readonly string New = "new";
            public static readonly string Dur = "dur";
        }

        public static class UserTypes
        {
            public static readonly string M = "M";
        }
    }
}
