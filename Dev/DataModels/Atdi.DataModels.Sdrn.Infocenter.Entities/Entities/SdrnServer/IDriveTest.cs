using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.Entities.SdrnServer
{
	public interface IDriveTest_PK
	{
		long Id { get; set; }
	}

	public interface IDriveTest : IDriveTest_PK
	{
		IStationMonitoring RESULT { get; set; }

		string Gsid { get; set; }

		double Freq_MHz { get; set; }

		string Standard { get; set; }

		int PointsCount { get; set; }

		
	}

	
}
