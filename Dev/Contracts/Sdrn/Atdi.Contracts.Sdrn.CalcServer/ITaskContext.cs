﻿using Atdi.DataModels.Sdrn.CalcServer;
using System;
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

		long ProjectId { get; }

		long ClientContextId { get; }

		void SendEvent<TData>(CalcResultEvent<TData> @event);

		void SendEvent(CalcResultEvent @event);

		//TResult RunIteration<TData, TResult>(TData data);

		//IIterationHandler<TData, TResult> GetIteration<TData, TResult>();
	}
}
