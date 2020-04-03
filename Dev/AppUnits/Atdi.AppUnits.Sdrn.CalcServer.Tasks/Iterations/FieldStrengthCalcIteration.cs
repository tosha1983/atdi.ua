using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
	public class FieldStrengthCalcIteration : IIterationHandler<FieldStrengthCalcData, FieldStrengthCalcResult>
	{
		public FieldStrengthCalcResult Run(ITaskContext taskContext, FieldStrengthCalcData data)
		{
			return new FieldStrengthCalcResult();
		}
	}
}
