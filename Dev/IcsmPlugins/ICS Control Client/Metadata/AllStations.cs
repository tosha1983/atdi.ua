using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XICSM.ICSControlClient.Metadata
{
    public class AllStations
    {
        public static readonly string TableName = "ALL_STATIONS";

        public static class Fields
        {
            public static readonly string Id = "ID";
            public static readonly string TableName = "TABLE_NAME";
            public static readonly string TableId = "TABLE_ID";
            public static readonly string Standart = "STANDARD";
            public static readonly string Longitude = "LONGITUDE";
            public static readonly string Latitude = "LATITUDE";
        }
    }
}
