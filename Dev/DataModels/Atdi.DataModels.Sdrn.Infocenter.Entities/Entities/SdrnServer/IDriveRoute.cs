using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer
{
	public interface IDriveRoute_PK
	{
		long Id { get; set; }
	}

	public interface IDriveRoute : IDriveRoute_PK
	{
		IStationMonitoring RESULT { get; set; }

		double Longitude { get; set; }

		double Latitude { get; set; }

		double? Altitude { get; set; }

	}

	
}
