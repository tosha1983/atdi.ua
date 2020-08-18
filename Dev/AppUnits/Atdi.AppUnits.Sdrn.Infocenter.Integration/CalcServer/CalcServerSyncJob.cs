using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ES_IC = Atdi.DataModels.Sdrn.Infocenter.Entities;
using ES_CC = Atdi.DataModels.Sdrn.CalcServer.Entities;
using ES_SD = Atdi.DataModels.Sdrns.Server.Entities;
using MD_CC = Atdi.DataModels.Sdrn.CalcServer;
using Atdi.AppUnits.Sdrn.Infocenter.Integration.SdrnServer;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.DataConstraint;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration.CalcServer
{
	[Serializable]
	public class ObservedTasksSyncKey
	{
		public long LastResultId;

		public override string ToString()
		{
			return $"Last ID = #{LastResultId}";
		}
	}

	class CalcServerSyncJob : IJobExecutor
	{
		private class ObservedTask
		{
			public long TaskId;

			public int TaskTypeCode;

			public string TaskTypeName;

			public byte TaskStatusCode;

			public string TaskStatusName;

			public long ResultId;

			public byte ResultStatusCode;

			public string ResultStatusName;
		}

		private class InfocObservedTask
		{
			public long Id;

			public int TaskTypeCode;

			public long ResultId;

			public byte ResultStatusCode;
		}

		private readonly IIntegrationService _integrationService;
		private readonly IDataLayer<EntityDataOrm<ES_IC.InfocenterEntityOrmContext>> _infocDataLayer;
		private readonly IDataLayer<EntityDataOrm<ES_SD.SdrnServerEntityOrmContext>> _sdrnsDataLayer;
		private readonly IDataLayer<EntityDataOrm<ES_CC.CalcServerEntityOrmContext>> _calcsDataLayer;
		private readonly AppServerComponentConfig _config;
		private readonly ILogger _logger;

		public CalcServerSyncJob(
			IIntegrationService integrationService,
			IDataLayer<EntityDataOrm<ES_IC.InfocenterEntityOrmContext>> infocDataLayer,
			IDataLayer<EntityDataOrm<ES_SD.SdrnServerEntityOrmContext>> sdrnsDataLayer,
			IDataLayer<EntityDataOrm<ES_CC.CalcServerEntityOrmContext>> calcsDataLayer,
			AppServerComponentConfig config,
			ILogger logger)
		{
			_integrationService = integrationService;
			_infocDataLayer = infocDataLayer;
			_sdrnsDataLayer = sdrnsDataLayer;
			_calcsDataLayer = calcsDataLayer;
			_config = config;
			_logger = logger;
		}

		public JobExecutionResult Execute(JobExecutionContext context)
		{
			using (var infocDbScope = this._infocDataLayer.CreateScope<InfocenterDataContext>())
			using (var sdrnsDbScope = this._sdrnsDataLayer.CreateScope<SdrnServerDataContext>())
			using (var calcsDbScope = this._calcsDataLayer.CreateScope<CalcServerDataContext>())
			{
				this.SynchronizeObservedTasks(infocDbScope, calcsDbScope);
				this.ProcessObservedTasks(infocDbScope, sdrnsDbScope, calcsDbScope);
				
			}
			return JobExecutionResult.Completed;
		}

		private void ProcessObservedTasks(IDataLayerScope infocDbScope, IDataLayerScope sdrnsDbScope, IDataLayerScope calcsDbScope)
		{
			
			try
			{
				var lastId = (long)0;
				do
				{
					var observedTasks = ReadNextInfocObservedTasks(infocDbScope, lastId);
					if (observedTasks.Length == 0)
					{
						break;
					}

					for (var i = 0; i < observedTasks.Length; i++)
					{
						var observedTask = observedTasks[i];

						var currentStatus = this.GetCurrentResultStatusCode(calcsDbScope, observedTask.ResultId);
						if (currentStatus == (byte)ES_CC.CalcResultStatusCode.Processing
						    || currentStatus == (byte)ES_CC.CalcResultStatusCode.Pending
						    || currentStatus == (byte)ES_CC.CalcResultStatusCode.Accepted)
						{
							continue;
						}

						if (currentStatus == (byte) ES_CC.CalcResultStatusCode.Completed)
						{
							// пришло время обработать
							if (this.ProcessObservedTask(calcsDbScope, sdrnsDbScope, observedTask))
							{
								DeleteInfocObservedTask(infocDbScope, observedTask.Id);
							}
						}
						else if (currentStatus == (byte)ES_CC.CalcResultStatusCode.Aborted
						|| currentStatus == (byte)ES_CC.CalcResultStatusCode.Canceled
						|| currentStatus == (byte)ES_CC.CalcResultStatusCode.Failed)
						{
							// перестаем мониторить
							DeleteInfocObservedTask(infocDbScope, observedTask.Id);
						}
						lastId = observedTask.Id;
					}

				} while (true);
				
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.Synchronization, e, this);
				
			}
		}

		private bool ProcessObservedTask(IDataLayerScope calcsDbScope, IDataLayerScope sdrnsDbScope, InfocObservedTask observedTask)
		{
			try
			{
				var headQuery = _sdrnsDataLayer.GetBuilder<ES_SD.IeStation.IHeadRefSpectrum>()
					.Insert()
					.SetValue(c => c.FileName,
						$"Received from CalcServer according to the result with ID #{observedTask.ResultId}")
					.SetValue(c => c.CreatedDate, DateTime.Now);

				var headPk = sdrnsDbScope.Executor.Execute<ES_SD.IeStation.IHeadRefSpectrum_PK>(headQuery);

				var resultQuery = _calcsDataLayer.GetBuilder<ES_CC.Tasks.IRefSpectrumByDriveTestsDetailResult>()
					.From()
					.Select(c => c.Freq_MHz)
					.Select(c => c.Level_dBm)
					.Select(c => c.DateMeas)
					.Select(c => c.GlobalCID)
					.Select(c => c.IdIcsm)
					.Select(c => c.TableIcsmName)
					.Select(c => c.IdSensor)
					.Select(c => c.Percent)
					.Where(c => c.RESULT_REF_SPECTRUM.RESULT.Id, ConditionOperator.Equal , observedTask.ResultId);

				var insQuery = _sdrnsDataLayer.GetBuilder<ES_SD.IeStation.IRefSpectrum>().Insert();
					
				var result = calcsDbScope.Executor.ExecuteAndFetch(resultQuery, reader =>
				{
					var maxFreq = (double?)null;
					var minFreq = (double?)null;
					var count = 0;
					var sensors = new HashSet<long>();
					while (reader.Read())
					{
						var freq = reader.GetValue(c => c.Freq_MHz);
						var sensorId = reader.GetValue(c => c.IdSensor);

						sensors.Add(sensorId);

						if (!maxFreq.HasValue)
						{
							maxFreq = freq;
						}
						if (!minFreq.HasValue)
						{
							minFreq = freq;
						}
						if (maxFreq < freq)
						{
							maxFreq = freq;
						}
						if (minFreq > freq)
						{
							minFreq = freq;
						}

						++count;

						insQuery
							.SetValue(c => c.HEAD_REF_SPECTRUM.Id, headPk.Id)
							.SetValue(c => c.Freq_MHz, reader.GetValue(c => c.Freq_MHz))
							.SetValue(c => c.Level_dBm, reader.GetValue(c => c.Level_dBm))
							.SetValue(c => c.SensorId, reader.GetValue(c => c.IdSensor))
							.SetValue(c => c.TableId, reader.GetValue(c => c.IdIcsm))
							.SetValue(c => c.TableName, reader.GetValue(c => c.TableIcsmName))
							.SetValue(c => c.Percent, reader.GetValue(c => c.Percent))
							.SetValue(c => c.DateMeas, reader.GetValue(c => c.DateMeas))
							.SetValue(c => c.GlobalSID, reader.GetValue(c => c.GlobalCID))
							;

						var pk = sdrnsDbScope.Executor.Execute<ES_SD.IeStation.IRefSpectrum_PK>(insQuery);
					}

					var updHeadQuery = _sdrnsDataLayer.GetBuilder<ES_SD.IeStation.IHeadRefSpectrum>()
						.Update()
						.SetValue(c => c.MaxFreqMHz, maxFreq)
						.SetValue(c => c.MinFreqMHz, minFreq)
						.SetValue(c => c.CountImportRecords, count)
						.SetValue(c => c.CountSensors, sensors.Count)
						.Where(c => c.Id, ConditionOperator.Equal, headPk.Id)
					;
					sdrnsDbScope.Executor.Execute(updHeadQuery);
					return true;
				});

				return result;
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.Synchronization, e, this);
				return false;
			}
		}

		private void SynchronizeObservedTasks(IDataLayerScope infocDbScope, IDataLayerScope calcsDbScope)
		{

			var hasError = false;
			var readCount = 0;
			var importedCount = 0;
			var lastResultId = (long)-1;

			var statusNote = "Sync completed successfully";
			var statusCode = ES_IC.IntegrationStatusCode.Done;
			var syncKey = default(ObservedTasksSyncKey);

			var token = _integrationService.Start(DataSource.CalcServer, IntegrationObjects.ObservedTasks);
			try
			{
				syncKey =
					_integrationService.GetSyncKey<ObservedTasksSyncKey>(DataSource.CalcServer,
						IntegrationObjects.ObservedTasks);

				if (syncKey == null)
				{
					syncKey = new ObservedTasksSyncKey
					{
						LastResultId = 0
					};
				}

				
				lastResultId = syncKey.LastResultId;

				var observedTasks = ReadNextObservedTasks(calcsDbScope, lastResultId);
				readCount = observedTasks.Length;

				for (var i = 0; i < readCount; i++)
				{
					var observedTask = observedTasks[i];
					
					hasError = !this.ImportObservedTask(infocDbScope, observedTask, out var message);
					if (hasError)
					{
						// дальше двигаться нельзя, нужно разбираться в ситуации
						statusNote = $"An error occurred while importing a record with ID #{observedTask.ResultId}: {message}";
						statusCode = ES_IC.IntegrationStatusCode.Aborted;
						break;
					}

					++importedCount;
					syncKey.LastResultId = observedTask.ResultId;
				}

			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.Synchronization, e, this);
				statusNote = e.Message;
				statusCode = ES_IC.IntegrationStatusCode.Aborted;
			}
			finally
			{
				var total =
					$"FromId=#{lastResultId}, LastId=#{syncKey.LastResultId}, Read={readCount}, Imported={importedCount}, HasError={hasError}";

				_integrationService.Finish(token, statusCode, statusNote, total, syncKey);
			}
		}

		private bool ImportObservedTask(IDataLayerScope infocDbScope, ObservedTask observedTask, out object message)
		{
			message = null;
			try
			{
				var insQuery = _infocDataLayer.GetBuilder<ES_IC.CalcServer.IObservedTask>()
						.Insert()
						.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
						.SetValue(c => c.ResultId, observedTask.ResultId)
						.SetValue(c => c.ResultStatusCode, observedTask.ResultStatusCode)
						.SetValue(c => c.ResultStatusName, observedTask.ResultStatusName)
						.SetValue(c => c.TaskId, observedTask.TaskId)
						.SetValue(c => c.TaskStatusCode, observedTask.TaskStatusCode)
						.SetValue(c => c.TaskStatusName, observedTask.TaskStatusName)
						.SetValue(c => c.TaskTypeCode, observedTask.TaskTypeCode)
						.SetValue(c => c.TaskTypeName, observedTask.TaskTypeName)
					;
				infocDbScope.Executor.Execute(insQuery);
				return true;
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.Synchronization, $"An error occurred while importing a record with ID #{observedTask.ResultId}", e, this);
				message = e.Message;
				return false;
			}
		}

		private ObservedTask[] ReadNextObservedTasks(IDataLayerScope calcsDbScope, long lastResultId)
		{
			var fetchRows = _config.AutoImportCalcServerCalcResultsFetchRows.GetValueOrDefault(1000);

			var sdrnsReadResultQuery = _calcsDataLayer.GetBuilder<ES_CC.ICalcResult>()
				.From()
				.Select(c => c.Id)

				.Select(c => c.StatusCode)
				.Select(c => c.StatusName)
				.Select(c => c.TASK.Id)
				.Select(c => c.TASK.StatusCode)
				.Select(c => c.TASK.StatusName)
				.Select(c => c.TASK.TypeCode)
				.Select(c => c.TASK.TypeName)
				// движимся по идентификаторам, последовательно по возрастанию
				.Where(c => c.Id, ConditionOperator.GreaterThan, lastResultId)
				// пока наблюдаем только за однйо задачей эти задачи
				.Where(c => c.TASK.TypeCode, ConditionOperator.Equal, (int)MD_CC.CalcTaskType.RefSpectrumByDriveTestsCalcTask)
				.FetchRows(fetchRows)
				.OrderByAsc(c => c.Id);

			var tasks = calcsDbScope.Executor.ExecuteAndFetch(sdrnsReadResultQuery, reader =>
			{
				var data = new List<ObservedTask>();
				while (reader.Read())
				{
					data.Add(new ObservedTask
					{
						ResultId = reader.GetValue(c => c.Id),
						ResultStatusCode = reader.GetValue(c => c.StatusCode),
						ResultStatusName = reader.GetValue(c => c.StatusName),
						TaskId = reader.GetValue(c => c.TASK.Id),
						TaskStatusCode = reader.GetValue(c => c.TASK.StatusCode),
						TaskStatusName = reader.GetValue(c => c.TASK.StatusName),
						TaskTypeCode = reader.GetValue(c => c.TASK.TypeCode),
						TaskTypeName = reader.GetValue(c => c.TASK.TypeName),
					});
				}

				return data.ToArray();
			});

			return tasks;
		}

		private InfocObservedTask[] ReadNextInfocObservedTasks(IDataLayerScope infocDbScope, long lastId)
		{
			var fetchRows = _config.AutoImportCalcServerCalcResultsFetchRows.GetValueOrDefault(1000);

			var query = _infocDataLayer.GetBuilder<ES_IC.CalcServer.IObservedTask>()
				.From()
				.Select(c => c.Id)

				.Select(c => c.ResultId)
				.Select(c => c.ResultStatusCode)
				.Select(c => c.TaskTypeCode)

				// движимся по идентификаторам, последовательно по возрастанию
				.Where(c => c.Id, ConditionOperator.GreaterThan, lastId)
				.FetchRows(fetchRows)
				.OrderByAsc(c => c.Id);

			var tasks = infocDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				var data = new List<InfocObservedTask>();
				while (reader.Read())
				{
					data.Add(new InfocObservedTask
					{
						Id = reader.GetValue(c => c.Id),
						ResultId = reader.GetValue(c => c.ResultId),
						ResultStatusCode = reader.GetValue(c => c.ResultStatusCode),
						TaskTypeCode = reader.GetValue(c => c.TaskTypeCode)
					});
				}

				return data.ToArray();
			});

			return tasks;
		}

		private byte GetCurrentResultStatusCode(IDataLayerScope calcsDbScope, long resultId)
		{

			var query = _calcsDataLayer.GetBuilder<ES_CC.ICalcResult>()
				.From()
				.Select(c => c.StatusCode)
				.Where(c => c.Id, ConditionOperator.Equal, resultId);

			var statusCode = calcsDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					throw new InvalidOperationException($"Calc Result not found by ID #{resultId}");
				}

				return reader.GetValue(c => c.StatusCode);
			});

			return statusCode;
		}

		private void DeleteInfocObservedTask(IDataLayerScope infocDbScope, long id)
		{
			var query = _infocDataLayer.GetBuilder<ES_IC.CalcServer.IObservedTask>()
				.Delete()
				.Where(c => c.Id, ConditionOperator.Equal, id)
				;
			infocDbScope.Executor.Execute(query);
		}
	}
}
