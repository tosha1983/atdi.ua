using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer
{
	public interface IStationMonitoringStats : IStationMonitoring_PK
	{
		int GsidCount { get; set; }

		double? MinFreq_MHz { get; set; }

		double? MaxFreq_MHz { get; set; }

		/// <summary>
		/// byte[] == DriveTestStandardStatistic[]
		/// </summary>
		byte[] StandardStats { get; set; }

		//DriveTestStandardStats[] StandardStatistics { get; set; }

	}

	[Serializable]
	public struct DriveTestStandardStats
	{
		public string Standard;

		public int Count;
	}
}
