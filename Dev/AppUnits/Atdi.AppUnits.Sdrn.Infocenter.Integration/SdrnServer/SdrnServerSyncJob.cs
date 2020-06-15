using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common.Extensions;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using ES_IC = Atdi.DataModels.Sdrn.Infocenter.Entities;
using ES_SD = Atdi.DataModels.Sdrns.Server.Entities;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration.SdrnServer
{
	[Serializable]
	public class StationMonitoringSyncKey
	{
		public long LastResultId;
		public DateTime BoundaryTime;

		public override string ToString()
		{
			return $"Last ID = #{LastResultId}; BoundaryTime = '{BoundaryTime:O}'";
		}
	}

	
	internal class SdrnServerSyncJob : IJobExecutor
	{
		private struct SdrnMeasResult
		{
			public long Id;
			public DateTime MeasTime;
			public string SensorName;
			public string SensorTitle;
		}

		private struct InfocDriveTest
		{
			public long Id;

			public string Gsid;

			public double Freq_MHz;

			public string Standard;
		}

		private struct InfocDriveTestRoute
		{
			public double Longitude;

			public double Latitude;

			public double? Altitude;
		}

		private readonly IIntegrationService _integrationService;
		private readonly IDataLayer<EntityDataOrm<ES_IC.InfocenterEntityOrmContext>> _infocDataLayer;
		private readonly IDataLayer<EntityDataOrm<ES_SD.SdrnServerEntityOrmContext>> _sdrnsDataLayer;
		private readonly AppServerComponentConfig _config;
		private readonly ILogger _logger;

		public SdrnServerSyncJob(
			IIntegrationService integrationService,
			IDataLayer<EntityDataOrm<ES_IC.InfocenterEntityOrmContext>> infocDataLayer,
			IDataLayer<EntityDataOrm<ES_SD.SdrnServerEntityOrmContext>> sdrnsDataLayer,
			AppServerComponentConfig config, 
			ILogger logger)
		{
			_integrationService = integrationService;
			_infocDataLayer = infocDataLayer;
			_sdrnsDataLayer = sdrnsDataLayer;
			_config = config;
			_logger = logger;
		}

		public JobExecutionResult Execute(JobExecutionContext context)
		{
			using (var infocDbScope = this._infocDataLayer.CreateScope<InfocenterDataContext>())
			using (var sdrnsDbScope = this._sdrnsDataLayer.CreateScope<SdrnServerDataContext>())
			{
				this.SynchronizeStationMonitoring(infocDbScope, sdrnsDbScope);
			}
			return JobExecutionResult.Completed;
		}

		private void SynchronizeStationMonitoring(IDataLayerScope infocDbScope, IDataLayerScope sdrnsDbScope)
		{
			
			var dataPeriod = _config.AutoImportSdrnServerStationMonitoringPeriod.GetValueOrDefault(1);
			var hasError = false;
			var readCount = 0;
			var importedCount = 0;
			var lastResultId = (long)-1;
			var upToDate = DateTime.Now.AddHours(-dataPeriod);
			var statusNote = "Sync completed successfully";
			var statusCode = ES_IC.IntegrationStatusCode.Done;
			var syncKey = default(StationMonitoringSyncKey);

			var token = _integrationService.Start(DataSource.SdrnServer, IntegrationObjects.StationMonitoring);
			try
			{
				syncKey =
					_integrationService.GetSyncKey<StationMonitoringSyncKey>(DataSource.SdrnServer,
						IntegrationObjects.StationMonitoring);

				if (syncKey == null)
				{
					syncKey = new StationMonitoringSyncKey
					{
						LastResultId = 0
					};
				}

				syncKey.BoundaryTime = upToDate;
				lastResultId = syncKey.LastResultId;

				var sdrnsResults = ReadNextSdrnMeasResult(sdrnsDbScope, lastResultId);
				readCount = sdrnsResults.Length;

				for (var i = 0; i < readCount; i++)
				{
					var sdrnResult = sdrnsResults[i];
					if (sdrnResult.MeasTime > upToDate)
					{
						// мы не можем дальше двигаться
						break;
					}

					hasError = !this.ImportSdrnMeasResult(infocDbScope, sdrnsDbScope, ref sdrnResult, out var message);
					if (hasError)
					{
						// дальше двигаться нельзя, нужно разбираться в ситуации
						statusNote = $"An error occurred while importing a record with ID #{sdrnResult.Id}: {message}";
						statusCode = ES_IC.IntegrationStatusCode.Aborted;
						break;
					}

					++importedCount;
					syncKey.LastResultId = sdrnResult.Id;
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
					$"BoundaryTime='{upToDate:O}', FromId=#{lastResultId}, LastId=#{syncKey.LastResultId}, Read={readCount}, Imported={importedCount}, HasError={hasError}";

				_integrationService.Finish(token, statusCode, statusNote, total, syncKey);
			}
		}

		private SdrnMeasResult[] ReadNextSdrnMeasResult(IDataLayerScope sdrnsDbScope, long lastResultId)
		{
			var fetchRows = _config.AutoImportSdrnServerStationMonitoringFetchRows.GetValueOrDefault(1000);

			var sdrnsReadResultQuery = _sdrnsDataLayer.GetBuilder<ES_SD.IResMeas>()
					.From()
					.Select(c => c.Id)
					.Select(c => c.TimeMeas)
					.Select(c => c.SUBTASK_SENSOR.SENSOR.Name)
					.Select(c => c.SUBTASK_SENSOR.SENSOR.Title)
					// движимся по идентификаторам, последовательно по возрастанию
					.Where(c => c.Id, ConditionOperator.GreaterThan, lastResultId)
					.Where(c => c.TypeMeasurements, ConditionOperator.Equal, "MonitoringStations")
					// время будем проверять в обработчике, важна последовательность
					// записи без даты нам не нужны
					.Where(c => c.TimeMeas, ConditionOperator.IsNotNull)
					.FetchRows(fetchRows)
					.OrderByAsc(c => c.Id);

			var sdrnsResults = sdrnsDbScope.Executor.ExecuteAndFetch(sdrnsReadResultQuery, reader =>
			{
				var data = new List<SdrnMeasResult>();
				while (reader.Read())
				{
					var measTime = reader.GetValue(c => c.TimeMeas);
					if (measTime.HasValue)
					{
						data.Add(new SdrnMeasResult
						{
							Id = reader.GetValue(c => c.Id),
							MeasTime = measTime.Value,
							SensorName = reader.GetValue(c => c.SUBTASK_SENSOR.SENSOR.Name),
							SensorTitle = reader.GetValue(c => c.SUBTASK_SENSOR.SENSOR.Title)
						});
					}
				}

				return data.ToArray();
			});

			return sdrnsResults;
		}

		private bool ImportSdrnMeasResult(IDataLayerScope infocDbScope, IDataLayerScope sdrnsDbScope, ref SdrnMeasResult sdrnMeasResult, out string message)
		{
			
			try
			{
				var resultId = sdrnMeasResult.Id;
				var dtsStats = new Dictionary<string, ES_IC.SdrnServer.DriveTestStandardStats>();
				var gsidCount = 0;
				var maxFreq_MHz = double.MinValue;
				var minFreq_MHz = double.MaxValue;

				//
				this.CleanStationMonitoring(infocDbScope, resultId);

				var insQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IStationMonitoring>()
						.Insert()
						.SetValue(c => c.Id, sdrnMeasResult.Id)
						.SetValue(c => c.StatusCode, (byte) ES_IC.SdrnServer.StationMonitoringStatusCode.Created)
						.SetValue(c => c.StatusName, "Created")
						.SetValue(c => c.CreatedDate, DateTimeOffset.Now)
						.SetValue(c => c.MeasTime, sdrnMeasResult.MeasTime)
						.SetValue(c => c.SensorName, sdrnMeasResult.SensorName)
						.SetValue(c => c.SensorTitle, sdrnMeasResult.SensorTitle)
						.SetValue(c => c.STATS.GsidCount, 0)
					;
				infocDbScope.Executor.Execute(insQuery);

				// читаем постранично одним потоком две сущности
				var fetchRows = _config.AutoImportSdrnServerStationMonitoringPointsFetchRows.GetValueOrDefault(10000);
				var offsetRows = 0;
				var pointsBufferSize = _config.AutoImportSdrnServerStationMonitoringPointsBufferSize.GetValueOrDefault(5000);
				var pointsBuffer = new ES_IC.SdrnServer.DriveTestPoint[pointsBufferSize];
				
				var canceled = false;
				var pointsIndex = -1;
				
				var prevMeasStationId = (long)0;
				var currMeasStationId = (long)0;
				var numberOfReadPoints = 0;

				do
				{
					var sdrnSelQuery = _sdrnsDataLayer.GetBuilder<ES_SD.IResStLevelCar>()
						.From()
						.Select(
							c => c.Id,
							c => c.LevelDbm,
							c => c.LevelDbmkvm,
							c => c.Altitude,
							c => c.Lat,
							c => c.Lon,
							c => c.RES_MEAS_STATION.Id,
							c => c.RES_MEAS_STATION.Standard,
							c => c.RES_MEAS_STATION.Frequency,
							c => c.RES_MEAS_STATION.MeasGlobalSID)
						.Where(c => c.RES_MEAS_STATION.RES_MEAS.Id, ConditionOperator.Equal, resultId)
						.Where(c => c.RES_MEAS_STATION.Standard, ConditionOperator.IsNotNull)
						.OrderByAsc(c => c.RES_MEAS_STATION.Id, c => c.Id)
						.Paginate(offsetRows, fetchRows);

					canceled = sdrnsDbScope.Executor.ExecuteAndFetch(sdrnSelQuery, reader =>
					{
						// достигли конца потока данных?
						if (!reader.Read())
						{
							if (prevMeasStationId > 0)
							{
								// в маиссиве есть данные, нужно слить в БД остаток
								if (pointsIndex >= 0)
								{
									this.CreateDriveTestPoints(infocDbScope, prevMeasStationId, pointsBuffer, pointsIndex + 1);
									pointsIndex = -1;
								}
								this.UpdateStatsDriveTest(infocDbScope, prevMeasStationId, numberOfReadPoints);
							}
							
							return true;
						}
					
						do
						{
							currMeasStationId = reader.GetValue(c => c.RES_MEAS_STATION.Id);
							if (currMeasStationId != prevMeasStationId)
							{
								// 1. нужно сохранить статистику  в предыдущем драйв тесте
								if (prevMeasStationId > 0)
								{
									// 0. в маиссиве есть данные, нужно слить в БД остаток
									if (pointsIndex >= 0)
									{
										this.CreateDriveTestPoints(infocDbScope, prevMeasStationId, pointsBuffer, pointsIndex + 1);
										pointsIndex = -1;
									}
									this.UpdateStatsDriveTest(infocDbScope, prevMeasStationId, numberOfReadPoints);
								}

								prevMeasStationId = currMeasStationId;

								// 2. нужно создать следующий Drive Test
								var driveTest = new InfocDriveTest
								{
									Id = currMeasStationId,
									Standard = reader.GetValue(c => c.RES_MEAS_STATION.Standard),
									Gsid = reader.GetValue(c => c.RES_MEAS_STATION.MeasGlobalSID),
									Freq_MHz = Convert.ToDouble(reader.GetValue(c => c.RES_MEAS_STATION.Frequency))
								};

								this.CreateDriveTest(infocDbScope, resultId, driveTest);

								// 3. Расчет статистики
								++gsidCount;
								if (maxFreq_MHz < driveTest.Freq_MHz)
								{
									maxFreq_MHz = driveTest.Freq_MHz;
								}
								if (minFreq_MHz > driveTest.Freq_MHz)
								{
									minFreq_MHz = driveTest.Freq_MHz;
								}

								if (!dtsStats.ContainsKey(driveTest.Standard))
								{
									dtsStats[driveTest.Standard] = new DriveTestStandardStats
									{
										Standard = driveTest.Standard,
										Count = 1
									};
								}
								else
								{
									var driveTestStandard = dtsStats[driveTest.Standard];
									driveTestStandard.Count += 1;
									dtsStats[driveTest.Standard] = driveTestStandard;
								}
								// 3. сброс счетчика
								numberOfReadPoints = 0;
							}

							++numberOfReadPoints;

							// добавляем элемент в масcив. если достигли конца сливаем на диск
							++pointsIndex;

							var point  = new DriveTestPoint
							{
								Height_m = Convert.ToInt32(reader.GetValue(c => c.Altitude)),
								Coordinate = new Wgs84Coordinate
								{
									Longitude = reader.GetValue(c => c.Lon).GetValueOrDefault(),
									Latitude = reader.GetValue(c => c.Lat).GetValueOrDefault()
								},
								FieldStrength_dBmkVm = Convert.ToSingle(reader.GetValue(c => c.LevelDbmkvm).GetValueOrDefault()),
								Level_dBm = Convert.ToSingle(reader.GetValue(c => c.LevelDbm).GetValueOrDefault())
							};
							pointsBuffer[pointsIndex] = point;

							if (pointsIndex + 1 == pointsBuffer.Length)
							{
								this.CreateDriveTestPoints(infocDbScope, currMeasStationId, pointsBuffer, pointsIndex + 1);
								pointsIndex = -1;
							}
						} while (reader.Read());
						return false;
					});

					// смещаемся на следующую порцию данных
					offsetRows += fetchRows;
				} while(!canceled);

				// нужно залить маршруты
				var routeBufferSize = _config.AutoImportSdrnServerStationMonitoringRouteBufferSize.GetValueOrDefault(5000);
				var routeBuffer = new InfocDriveTestRoute[routeBufferSize];
				var routMap = new HashSet<string>(routeBufferSize);
				var routeIndex = 0; // важно , указывает всегда на следующую ячейку, так сделано специально чтобы  сжимать по необходимости
				var routePackRation = 1;
				var routeStopIndex = 1;
				// читаем еще раз
				offsetRows = 0;
				canceled = false;
				do
				{
					var sdrnSelQuery = _sdrnsDataLayer.GetBuilder<ES_SD.IResStLevelCar>()
						.From()
						.Select(
							c => c.Altitude,
							c => c.Lat,
							c => c.Lon)
						.Where(c => c.RES_MEAS_STATION.RES_MEAS.Id, ConditionOperator.Equal, resultId)
						.Where(c => c.RES_MEAS_STATION.Standard, ConditionOperator.IsNotNull)
						.OrderByAsc(c => c.Lat, c => c.Lon)
						.Paginate(offsetRows, fetchRows);

					canceled = sdrnsDbScope.Executor.ExecuteAndFetch(sdrnSelQuery, reader =>
					{
						// достигли конца потока данных?
						if (!reader.Read())
						{
							return true;
						}

						do
						{
							var altitude = reader.GetValue(c => c.Altitude);
							var longitude = reader.GetValue(c => c.Lon).GetValueOrDefault();
							var latitude = reader.GetValue(c => c.Lat).GetValueOrDefault();

							// маршруты
							var key = $"{latitude}-{longitude}";
							if (!routMap.Contains(key))
							{
								if (routePackRation == routeStopIndex)
								{
									// сбрасываем счетчик
									routeStopIndex = 1;

									// текущее значение нужн осохранить
									routMap.Add(key);

									if (routeIndex == routeBufferSize)
									{
										// пришло время сжаться в два раза так буфер забит полностью
										routeIndex = routeIndex / 2;
										// увеличивает частоту отбора точек
										routePackRation *= 2;
										routeStopIndex = 1;

										// сдвигаем элементы - утесняем в 2 раза - до текущего индекса
										// каждый второй оставляем
										for (int i = 1; i < routeIndex; i++)
										{
											var sourceIndex = i * 2;
											if (sourceIndex < routeBufferSize)
											{
												var source = routeBuffer[sourceIndex];
												var key2 = $"{source.Latitude}-{source.Longitude}";
												routMap.Remove(key2);
												routeBuffer[i] = source;
											}
										}
									}

									routeBuffer[routeIndex] = new InfocDriveTestRoute
									{
										Latitude = latitude,
										Longitude = longitude,
										Altitude = altitude
									};

									++routeIndex;
								}
								else
								{
									// пропускаем это значение, так как берем каждый routePackRation
									++routeStopIndex;
								}
							}
						} while (reader.Read());
						return false;
					});
					// смещаемся на следующую порцию данных
					offsetRows += fetchRows;
				} while (!canceled);

				var routeQueryBuilder = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IDriveRoute>();
				for (int i = 0; i < routeIndex; i++)
				{
					var routePoint = routeBuffer[i];
					var routInsQuery = routeQueryBuilder
						.Insert()
						.SetValue(c => c.RESULT.Id, sdrnMeasResult.Id)
						.SetValue(c => c.Latitude, routePoint.Latitude)
						.SetValue(c => c.Longitude, routePoint.Longitude)
						.SetValue(c => c.Altitude, routePoint.Altitude);
					infocDbScope.Executor.Execute(routInsQuery);
				}

				// тут импортируем все потраха
				var updStsQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IStationMonitoringStats>()
					.Update()
						.SetValue(c => c.GsidCount, gsidCount)
						.SetValue(c => c.MaxFreq_MHz, maxFreq_MHz)
						.SetValue(c => c.MinFreq_MHz, minFreq_MHz)
						.SetValue(c => c.StandardStats, dtsStats.Values.ToArray().Serialize())
					.Where(c => c.Id, ConditionOperator.Equal, sdrnMeasResult.Id);

				var updQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IStationMonitoring>()
					.Update()
						.SetValue(c => c.StatusCode, (byte) ES_IC.SdrnServer.StationMonitoringStatusCode.Available)
						.SetValue(c => c.StatusName, "Available")
					.Where(c => c.Id, ConditionOperator.Equal, sdrnMeasResult.Id);

				infocDbScope.Executor.Execute(updStsQuery);
				infocDbScope.Executor.Execute(updQuery);

				message = null;
				return true;
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.Synchronization, $"An error occurred while importing a record with ID #{sdrnMeasResult.Id}", e, this);
				message = e.Message;
				return false;
			}
		}

		private void CleanStationMonitoring(IDataLayerScope infocDbScope, long resultId)
		{
			var findQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IStationMonitoring>()
					.From()
					.Select(c => c.Id)
					.Where(c => c.Id, ConditionOperator.Equal, resultId)
				;
			var count = infocDbScope.Executor.Execute(findQuery);

			if (count == 0)
			{
				return;
			}

			var delPointsQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IDriveTestPoints>()
				.Delete()
				.Where(c => c.DRIVE_TEST.RESULT.Id, ConditionOperator.Equal, resultId);

			var delDriveTestsQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IDriveTest>()
				.Delete()
				.Where(c => c.RESULT.Id, ConditionOperator.Equal, resultId);

			var delDriveRoutesQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IDriveRoute>()
				.Delete()
				.Where(c => c.RESULT.Id, ConditionOperator.Equal, resultId);

			var delSmStatsQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IStationMonitoringStats>()
				.Delete()
				.Where(c => c.Id, ConditionOperator.Equal, resultId);

			var delSmQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IStationMonitoring>()
				.Delete()
				.Where(c => c.Id, ConditionOperator.Equal, resultId);

			count = infocDbScope.Executor.Execute(delPointsQuery);
			count = infocDbScope.Executor.Execute(delDriveTestsQuery);
			count = infocDbScope.Executor.Execute(delDriveRoutesQuery);
			count = infocDbScope.Executor.Execute(delSmStatsQuery);
			count = infocDbScope.Executor.Execute(delSmQuery);
		}

		private void CreateDriveTestPoints(IDataLayerScope infocDbScope, long driveTestId, DriveTestPoint[] pointsBuffer, int count)
		{
			var points = default(DriveTestPoint[]);

			if (pointsBuffer.Length > count)
			{
				points = new DriveTestPoint[count];
				Array.Copy(pointsBuffer, points, count);
			}
			else
			{
				points = pointsBuffer;
			}

			var insQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IDriveTestPoints>()
				.Insert()
				.SetValue(c => c.DRIVE_TEST.Id, driveTestId)
				.SetValue(c => c.Count, count)
				.SetValue(c => c.Points, points.Serialize());
			infocDbScope.Executor.Execute(insQuery);
		}

		private void UpdateStatsDriveTest(IDataLayerScope infocDbScope, long driveTestId, int pointsCount)
		{
			var updQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IDriveTest>()
				.Update()
				.SetValue(c => c.PointsCount, pointsCount)
				.Where(c => c.Id, ConditionOperator.Equal, driveTestId);
			infocDbScope.Executor.Execute(updQuery);
		}

		private void CreateDriveTest(IDataLayerScope infocDbScope, long resultId, InfocDriveTest data)
		{
			var insQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.IDriveTest>()
				.Insert()
				.SetValue(c => c.Id, data.Id)
				.SetValue(c => c.RESULT.Id, resultId)
				.SetValue(c => c.Standard, data.Standard)
				.SetValue(c => c.Gsid, data.Gsid)
				.SetValue(c => c.Freq_MHz, data.Freq_MHz)
				.SetValue(c => c.PointsCount, 0);
			infocDbScope.Executor.Execute(insQuery);
		}
	}
}
