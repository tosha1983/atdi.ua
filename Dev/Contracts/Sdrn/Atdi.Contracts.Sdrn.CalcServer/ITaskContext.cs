﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	public interface ITaskContext
	{
		long ResultId { get; }

		CalculationStatus Status { get; }

		long TaskId { get; }

		void SendEvent(ICalcEvent @event);

		TResult RunIteration<TData, TResult>(TData data);
	}
}