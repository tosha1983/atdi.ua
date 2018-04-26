using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient.Metadata
{
    public class MobStations
    {
        public static readonly string TableName = "MOB_STATION";

        public static class Fields
        {
            public static readonly string Id = "ID";
            public static readonly string Name = "NAME";
            public static readonly string Standart = "STANDARD";
            public static readonly string Agl = "AGL";
            public static readonly string Power = "POWER";
            public static readonly string Azimut = "AZIMUTH";
            public static readonly string BW = "BW";
            public static readonly string DesignEmission = "DESIG_EMISSION";
            public static readonly string CustTxt13 = "CUST_TXT13";


            public static class Owner
            {
                public static readonly string Id = "Owner.ID";
                public static readonly string Name = "Owner.NAME";
                public static readonly string RegistNum = "Owner.REGIST_NUM";
                public static readonly string PostCode = "Owner.POSTCODE";
                public static readonly string Code = "Owner.CODE";
                public static readonly string Address = "Owner.ADDRESS";
            }
            public static class Position
            {
                public static readonly string Longitude = "Position.LONGITUDE";
                public static readonly string Latitude = "Position.LATITUDE";
                public static readonly string Address = "Position.ADDRESS";
                public static readonly string SubProvince = "Position.SUBPROVINCE";
            }
        }
    }
}
