using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	public interface ICheckPoint : IDisposable
	{
		/// <summary>
		/// Идентификатор точки восстановления
		/// </summary>
		long Id { get; }

		string Name { get; }

		/// <summary>
		/// Зафиксировать данные и саму точку восстановления 
		/// </summary>
		void Commit();

		/// <summary>
		/// Сохранить слот данных ассоцрованых с даннйо точкой восстановления 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context">Контекст с которым ассоцированы данные, конетекст должен быть уникальным в рамках точки восстановления</param>
		/// <param name="data">Данные контекста</param>
		void SaveData<T>(string context, T data);

		/// <summary>
		/// Востановить ассоцированные с точкой востановления контекстные данные заданой структуры
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context">Контекст с которым ассоцированы данные</param>
		/// <returns></returns>
		T RestoreData<T>(string context);
	}
}
