using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.Entities
{
	public interface IIntegrationLog_PK
	{
		long Id { get; set; }
	}

	public interface IIntegrationLog : IIntegrationLog_PK
	{
		IIntegrationObject OBJECT { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		DateTimeOffset? StartTime{ get; set; }

		DateTimeOffset? FinishTime { get; set; }

		string SyncTotal { get; set; }

	}

	public enum IntegrationStatusCode
	{
		/// <summary>
		/// Начальная фаза
		/// </summary>
		Created = 0,

		/// <summary>
		/// Иде процесс синхронизации данных
		/// </summary>
		Processing = 1,

		/// <summary>
		/// Процесс завершился удачно
		/// </summary>
		Done = 2,

		/// <summary>
		/// Процесс был оборван ка кправило по причине возникшего ексепшена
		/// </summary>
		Aborted = 3,
		 
		/// <summary>
		/// Процесс был остановлен прикладной логикой
		/// </summary>
		Cancelled = 4
	}
}
