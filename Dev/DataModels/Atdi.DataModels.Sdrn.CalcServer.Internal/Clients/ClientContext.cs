using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Clients
{
    [Serializable]
    public class ClientContext
	{
		public long Id;

		public long ProjectId;

		public string OwnerInstance;

		public Guid OwnerContextId;

		public DateTimeOffset CreatedDate;

		public ClientContextStatus Status;
	}
    [Serializable]
    public enum ClientContextStatus
	{
		/// <summary>
		/// Контекст создан 
		/// </summary>
		Created = 0,

		/// <summary>
		/// Контекст изменяется
		/// </summary>
		Modifying = 1,

		/// <summary>
		/// Контекст в процессе ожидания подготовки
		/// </summary>
		Pending = 2,

		/// <summary>
		/// Контекст в процессе подготовки
		/// </summary>
		Processing = 3,

		/// <summary>
		/// Контекст полностью подготовлен и доступен для использования в рассчетах
		/// </summary>
		Prepared = 4,

		/// <summary>
		/// Контекст не удалось подготовить
		/// </summary>
		Failed = 5,

		/// <summary>
		/// Контекст более неактуален для использования в рассчетах
		/// </summary>
		Archived = 6
	}
}
