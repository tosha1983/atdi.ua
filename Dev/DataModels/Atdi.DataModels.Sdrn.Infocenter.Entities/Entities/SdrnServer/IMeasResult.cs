using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.Entities.SdrnServer
{
	public interface IMeasResult_PK
	{
		long Id { get; set; }
	}

	public interface IMeasResult : IMeasResult_PK
	{
		DateTime? MeasTime { get; set; }

		string SensorName { get; set; }

		string SensorTitle { get; set; }

		int GsidCount { get; set; }

		double? MinFreq_MHz { get; set; }

		double? MaxFreq_MHz { get; set; }

		/// <summary>
		/// byte[] == DriveTestStandardStatistic[]
		/// </summary>
		byte[] StandardStatistics { get; set; }

		//DriveTestStandardStatistic[] StandardStatistics { get; set; }
		
		int MaxPointsCount { get; set; }
	}

	public struct DriveTestStandardStatistic
	{
		public string Standard;

		public int Count;
	}
}
