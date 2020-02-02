using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	/// <summary>
	/// Фабрика обработчиков задач
	/// </summary>
	public interface ITasksFactory
	{
		/// <summary>
		/// Метод регистрации обработчика
		/// </summary>
		/// <param name="taskType">типа расчета</param>
		/// <param name="handlerType">тип объекта обработчика задачи</param>
		void Register(CalcTaskType taskType, Type handlerType);

		/// <summary>
		/// Метод создания нового экземпляра обработчика
		/// </summary>
		/// <param name="taskType">тип расчета</param>
		/// <returns></returns>
		ITaskHandler Create(CalcTaskType taskType);
	}
}
