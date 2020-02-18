using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.Iterations
{
	/// <summary>
	/// пример итерации котора яничего не принимает и ничего не возвращает
	/// но при этом может свои результаты сохранит в БД или просто быть организатором некого юз кейса
	/// в котром испольтзуьюся другие итерации
	/// </summary>
	class VoidIteration: IIterationHandler<VoidData, VoidResult>
	{
		public VoidResult Run(ITaskContext taskContext, VoidData data)
		{
			return VoidResult.Instance;
		}
	}
}
