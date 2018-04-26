using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XICSM.ICSControlClient.Metadata
{
    public class Inspection
    {
        public static readonly string TableName = "INSPECTION";

        public static class Fields
        {
            public static readonly string Id = "ID";
            public static readonly string Status = "STATUS";
            public static readonly string Type = "TYPE";
            public static readonly string DoItAfter = "DOIT_AFTER";
            public static readonly string DoItBefore = "DOIT_BEFORE";
            public static readonly string TourId = "INSP_TOUR_ID";
            public static readonly string StationTableName = "STA_TBNM";
            public static readonly string StationTableId = "STA_TBID";
            public static class Station
            {
                public static readonly string Name = "Station.NAME";
        }
            
        }

        public static class Statuses
        {
            public static readonly string New = "new";
            public static readonly string Done = "done";
        }

        public static class Types
        {
            public static readonly string C = "C";
        }
    }
}
