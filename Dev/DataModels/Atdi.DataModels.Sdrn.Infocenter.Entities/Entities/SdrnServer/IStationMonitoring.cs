using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer
{
	public interface IStationMonitoring_PK
	{
		long Id { get; set; }
	}

	public interface IStationMonitoring : IStationMonitoring_PK
	{
		DateTimeOffset CreatedDate { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		DateTime? MeasTime { get; set; }

		string SensorName { get; set; }

		string SensorTitle { get; set; }

		IStationMonitoringStats STATS { get; set; }
	}

	public enum StationMonitoringStatusCode
	{
		/// <summary>
		/// создана запись, вложенная структура данных пустая 
		/// </summary>
		Created = 0,
		/// <summary>
		/// модифицируется - структура данных дополняется
		/// </summary>
		Modifying = 1,
		/// <summary>
		/// результаты измерения получены полностью и доступны для использования
		/// </summary>
		Available = 2,
	}

	
}
