using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XICSM.ICSControlClient.Metadata
{
    public static class Tours
    {
        //public static readonly string Tours = "ADM_EQUIP";
        public static readonly string TableName = "INSP_TOUR";

        public static class Fields
        {
            public static readonly string Id = "ID";
            public static readonly string Status = "STATUS";
            public static readonly string LocationList = "CUST_TXT9";
            public static readonly string RadioTechList = "CUST_TXT6";
            public static readonly string StartDate = "START_DATE";
            public static readonly string StopDate = "STOP_DATE";

            public static readonly string MeasTaskId = "CUST_NBR1";
            public static readonly string MeasTaskName = "CUST_TXT1";
            public static readonly string SensorName = "CUST_TXT2";
            public static readonly string SensorEquipTechId = "CUST_TXT7";
        }

        public static class Statuses
        {
            public static readonly string New = "NEW";
            public static readonly string Dur = "DUR";
        }
    }
}
