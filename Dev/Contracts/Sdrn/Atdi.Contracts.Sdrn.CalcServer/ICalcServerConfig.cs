using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	public interface ICalcServerConfig
	{
		string Instance { get; }

		string LicenseNumber { get; }

		DateTime LicenseStopDate { get; }

		DateTime LicenseStartDate { get; }

	}
}
