using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XICSM.ICSControlClient.Metadata
{
    public class NfraApplication
    {
        public static readonly string TableName = "XNRFA_APPL";

        public static class Fields
        {
            public static readonly string Id = "ID";
            public static readonly string ObjTable = "OBJ_TABLE";
            public static readonly string ObjId1 = "OBJ_ID1";
            public static readonly string ObjId2 = "OBJ_ID2";
            public static readonly string ObjId3 = "OBJ_ID3";
            public static readonly string ObjId4 = "OBJ_ID4";
            public static readonly string ObjId5 = "OBJ_ID5";
            public static readonly string ObjId6 = "OBJ_ID6";
            public static readonly string DozvDateFrom = "DOZV_DATE_FROM";
            public static readonly string DozvDateTo = "DOZV_DATE_TO";
            public static readonly string DozvDateCancel = "DOZV_DATE_CANCEL";
            public static readonly string DozvNum = "DOZV_NUM";
        }
    }
}
