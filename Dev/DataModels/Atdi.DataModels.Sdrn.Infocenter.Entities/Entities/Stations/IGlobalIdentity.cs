using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.Entities.Stations
{
	public interface IGlobalIdentity_PK
	{
		string RegionCode { get; set; }

		string LicenseGsid { get; set; }

		string Standard { get; set; }
	}
	public interface IGlobalIdentity
	{
		DateTimeOffset CreatedDate { get; set; }

		string RealGsid { get; set; }

	}
}
