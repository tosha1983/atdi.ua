using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.LicenseGenerator
{
	public static class Storage
	{
		public class Companies
		{
			public static readonly string LIS_Uk = "ТОВ 'Лабораторія інформаційних систем'";

			public static readonly string ATDI_Ukraine_EN = "ADTI Ukraine, LLC";
		}
		public static class Clients
		{
			public static class UDCR
			{
				public static readonly string OwnerKey = "BD13-G65";
				public static readonly string OwnerId = "OID-BD13-G65-N00";
				public static readonly string OwnerName = "Державне підприємство «Український державний центр радіочастот»";
				
			}

			public static class ATDI_SA
			{
				public static readonly string OwnerKey = "BD10-A00";
				public static readonly string OwnerId = "OID-BD10-A00-N00";
				public static readonly string OwnerName = "ATDI SA";

			}

			public static class LIS_ForTest
			{
				public static readonly string OwnerKey = "BD12-A00";
				public static readonly string OwnerId = "OID-BD12-A00-N00";
				public static readonly string OwnerName = "ТОВ 'Лабораторія інформаційних систем'";

			}
		}

		public static class LicenseTypes
		{
			public static readonly string DeviceLicense = "DeviceLicense";
			public static readonly string ClientLicense = "ClientLicense";
			public static readonly string ServerLicense = "ServerLicense";
		}

		public static class Products
		{
			public static readonly string ICS_Control_Device = "ICS Control Device";

			public static readonly string ICSM_Plugin_GE06_Calc = "ICSM Plugin - GE06 Calc";
			public static readonly string SDRN_Calc_Server = "SDRN Calc Server";
		}
	}
}
