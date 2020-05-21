using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.Entities.SdrnServer
{
	public interface IMeasResultStats : IMeasResult_PK
	{
		int GsidCount { get; set; }

		double? MinFreq_MHz { get; set; }

		double? MaxFreq_MHz { get; set; }

		/// <summary>
		/// byte[] == DriveTestStandardStatistic[]
		/// </summary>
		byte[] StandardStats { get; set; }

		//DriveTestStandardStatistic[] StandardStatistics { get; set; }

	}

	public struct DriveTestStandardStats
	{
		public string Standard;

		public int Count;
	}
}
