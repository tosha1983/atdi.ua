using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using Newtonsoft.Json;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal class CommandJob : IJobExecutor
	{
		private class CalcCommand
		{
			public CalcCommandTypeCode Type;
			public string ArgsJson;
		}
		private readonly ITaskDispatcher _taskDispatcher;

		private readonly AppServerComponentConfig _config;
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly ILogger _logger;

		public CommandJob(
			ITaskDispatcher taskDispatcher,
			AppServerComponentConfig config,
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			ILogger logger)
		{
			_taskDispatcher = taskDispatcher;
			_config = config;
			_calcServerDataLayer = calcServerDataLayer;
			_logger = logger;
		}

		public JobExecutionResult Execute(JobExecutionContext context)
		{
			using (var dbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
			{
				var nextCommands = this.GetNextCommands(dbScope);
				if (nextCommands != null && nextCommands.Length > 0)
				{
					foreach (var commandId in nextCommands)
					{
						this.ExecuteCommand(dbScope, commandId);
					}
				}
			}

			return JobExecutionResult.Completed;
		}

		private void ExecuteCommand(IDataLayerScope dbScope, long commandId)
		{
			try
			{
				var calcCommand = this.ReadCommand(dbScope, commandId);

				this.SetCommandToExecuting(dbScope, commandId);
				try
				{
					if (calcCommand.Type == CalcCommandTypeCode.CancelCalcTask)
					{
						if (string.IsNullOrEmpty(calcCommand.ArgsJson))
						{
							throw new InvalidOperationException($"Undefined command arguments");
						}
						var args = JsonConvert.DeserializeObject<CancelCalcTaskCommand>(calcCommand.ArgsJson); 

						_taskDispatcher.StopTask(args.ResultId);
					}
					else if (calcCommand.Type == CalcCommandTypeCode.AbortCalcTask)
					{
						if (string.IsNullOrEmpty(calcCommand.ArgsJson))
						{
							throw new InvalidOperationException($"Undefined command arguments");
						}
						var args = JsonConvert.DeserializeObject<AbortCalcTaskCommand>(calcCommand.ArgsJson);

						_taskDispatcher.AbortTask(args.ResultId);
					}
					else
					{
						throw new InvalidOperationException($"Unsupported command type '{calcCommand.Type}'");
					}

					this.SetCommandToCompleted(dbScope, commandId, "The command is well executed");
				}
				catch (Exception e)
				{
					this.SetCommandToFailed(dbScope, commandId, e.ToString());
				}
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.CommandExecuting, e, this);
				_logger.Error(Contexts.ThisComponent, Categories.CommandExecuting, (EventText)$"Failed to execute command with Id #{commandId}", this.ToString());
			}
			
		}

		private void SetCommandToCompleted(IDataLayerScope dbScope, long commandId, string message)
		{
			var updQuery = _calcServerDataLayer.GetBuilder<ICalcCommand>()
				.Update()
				.SetValue(c => c.FinishTime, DateTimeOffset.Now)
				.SetValue(c => c.StatusCode, (byte)CalcCommandStatusCode.Completed)
				.SetValue(c => c.StatusName, CalcCommandStatusCode.Completed.ToString())
				.SetValue(c => c.StatusNote, message)

				.Where(c => c.Id, ConditionOperator.Equal, commandId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcCommandStatusCode.Executing);

			var count = dbScope.Executor.Execute(updQuery);

			if (count == 0)
			{
				throw new InvalidOperationException($"Executing Command not found by ID #{commandId} ");
			}
		}

		private void SetCommandToFailed(IDataLayerScope dbScope, long commandId, string message)
		{
			var updQuery = _calcServerDataLayer.GetBuilder<ICalcCommand>()
				.Update()
				.SetValue(c => c.FinishTime, DateTimeOffset.Now)
				.SetValue(c => c.StatusCode, (byte)CalcCommandStatusCode.Failed)
				.SetValue(c => c.StatusName, CalcCommandStatusCode.Failed.ToString())
				.SetValue(c => c.StatusNote, message)

				.Where(c => c.Id, ConditionOperator.Equal, commandId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcCommandStatusCode.Executing);

			var count = dbScope.Executor.Execute(updQuery);

			if (count == 0)
			{
				throw new InvalidOperationException($"Executing Command not found by ID #{commandId} ");
			}
		}

		private void SetCommandToExecuting(IDataLayerScope dbScope, long commandId)
		{
			var updQuery = _calcServerDataLayer.GetBuilder<ICalcCommand>()
				.Update()
				.SetValue(c => c.StartTime, DateTimeOffset.Now)
				.SetValue(c => c.StatusCode, (byte)CalcCommandStatusCode.Executing)
				
				.Where(c => c.Id, ConditionOperator.Equal, commandId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcCommandStatusCode.Pending);

			var count = dbScope.Executor.Execute(updQuery);

			if (count == 0)
			{
				throw new InvalidOperationException($"Pending Command not found by ID #{commandId} ");
			}
		}

		private CalcCommand ReadCommand(IDataLayerScope dbScope, long commandId)
		{
			var taskResultQuery = _calcServerDataLayer.GetBuilder<ICalcCommand>()
				.From()
				.Select(c => c.TypeCode)
				.Select(c => c.ArgsJson)
				// Рассчет в ожидании запуска
				.Where(c => c.Id, ConditionOperator.Equal, commandId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcCommandStatusCode.Pending);

			var result = dbScope.Executor.ExecuteAndFetch(taskResultQuery, reader =>
			{
				if (!reader.Read())
				{
					throw new InvalidOperationException($"Pending Command not found by ID #{commandId} ");
				}

				return new CalcCommand
				{
					Type = (CalcCommandTypeCode)reader.GetValue(c => c.TypeCode),
					ArgsJson = reader.GetValue(c => c.ArgsJson)
				};
			});

			return result;
		}

		private long[] GetNextCommands(IDataLayerScope dbScope)
		{
			var taskResultQuery = _calcServerDataLayer.GetBuilder<ICalcCommand>()
				.From()
				.Select(c => c.Id)
				// Рассчет в ожидании запуска
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcCommandStatusCode.Pending)
				.OrderByAsc(c => c.Id);

			var result = dbScope.Executor.ExecuteAndFetch(taskResultQuery, reader =>
			{
				var res = new List<long>();
				while (reader.Read())
				{
					res.Add(reader.GetValue(c => c.Id));
				}

				return res.ToArray();
			});
			return result;
		}

		private long[] GetNextCalcTaskResults(IDataLayerScope dbScope)
		{
			var taskResultQuery = _calcServerDataLayer.GetBuilder<ICalcCommand>()
				.From()
				.Select(c => c.Id)
				// Рассчет в ожидании запуска
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)CalcCommandStatusCode.Pending)
				.OrderByAsc(c => c.Id);

			var result = dbScope.Executor.ExecuteAndFetch(taskResultQuery, reader =>
			{
				var res = new List<long>();
				while (reader.Read())
				{
					res.Add(reader.GetValue(c => c.Id));
				}

				return res.ToArray();
			});
			return result;
		}
	}
}
