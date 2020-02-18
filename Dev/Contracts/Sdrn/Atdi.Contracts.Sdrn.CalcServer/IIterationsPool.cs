using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	public interface IIterationsPool
	{
		/// <summary>
		/// Регистрация типа обработчика итерации в пуле итерациий
		/// </summary>
		/// <param name="handlerType"></param>
		void Register(Type handlerType);

		/// <summary>
		/// Получение обработчика итерации из пулла
		/// </summary>
		/// <typeparam name="TData"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		IIterationHandler<TData, TResult> GetIteration<TData, TResult>();
	}
}
