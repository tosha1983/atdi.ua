using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XICSM.ICSControlClient.Metadata
{
    public class SysLogs
    {
        public static readonly string TableName = "SYS_LOGS";

        public static class Fields
        {
            public static readonly string Id = "ID";
            public static readonly string Event = "EVENT";
            public static readonly string TableName = "TABLE_NAME";
            public static readonly string Count = "LCOUNT";
            public static readonly string Info = "INFO1";
            public static readonly string Who = "WHO";
            public static readonly string When = "WHEN";
        }
    }
}
