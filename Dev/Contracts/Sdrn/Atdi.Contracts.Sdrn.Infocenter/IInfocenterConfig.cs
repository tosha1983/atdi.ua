using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Infocenter
{
	public interface IInfocenterConfig
	{
		string Instance { get; }

		string LicenseNumber { get; }

		DateTime LicenseStopDate { get; }

		DateTime LicenseStartDate { get; }

	}
}
