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
			public long? SensorId;
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
				this.SynchronizeSensors(infocDbScope, sdrnsDbScope);
				this.SynchronizeSensorAntennas(infocDbScope, sdrnsDbScope);
				this.SynchronizeSensorAntennaPatterns(infocDbScope, sdrnsDbScope);
				this.SynchronizeSensorEquipmentItems(infocDbScope, sdrnsDbScope);
				this.SynchronizeSensorEquipmentSensitivities(infocDbScope, sdrnsDbScope);
				this.SynchronizeSensorLocations(infocDbScope, sdrnsDbScope);
			}
			return JobExecutionResult.Completed;
		}

		private void SynchronizeSensors(IDataLayerScope infocDbScope, IDataLayerScope sdrnsDbScope)
		{
			var statusNote = "Sync completed successfully";
			var statusCode = ES_IC.IntegrationStatusCode.Done;
			var fetchRows = _config.AutoImportSdrnServerSensorsFetchRows.GetValueOrDefault(1000); ;
			var offsetRows = 0;
			var createdCount = 0;
			var updatedCount = 0;
			var token = _integrationService.Start(DataSource.SdrnServer, IntegrationObjects.Sensors);

			try
			{
				var needFetch = true;
				while (needFetch)
				{
					var sdrnsSensorQuery = _sdrnsDataLayer.GetBuilder<ES_SD.ISensor>()
						.From()
						.Select(c => c.Id)
						.Select(c => c.SensorIdentifierId)
						.Select(c => c.Status)
						.Select(c => c.Name)
						.Select(c => c.BiuseDate)
						.Select(c => c.EouseDate)
						.Select(c => c.Azimuth)
						.Select(c => c.Elevation)
						.Select(c => c.Agl)
						.Select(c => c.RxLoss)
						.Select(c => c.TechId)
						.OrderByAsc(c => c.Id)
						.Paginate(offsetRows, fetchRows);

					needFetch = sdrnsDbScope.Executor.ExecuteAndFetch(sdrnsSensorQuery, reader =>
					{
						var result = false;
						while (reader.Read())
						{
							++offsetRows;
							result = true;

							var created = this.SynchronizeSensor(infocDbScope, reader);
							if (created)
							{
								++createdCount;
							}
							else
							{
								++updatedCount;
							}
						}

						return result;
					});
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
					$"Read={offsetRows}, Created={createdCount}, Updated={updatedCount}";

				_integrationService.Finish(token, statusCode, statusNote, total);
			}
		}

		private bool SynchronizeSensor(IDataLayerScope infocDbScope, IDataReader<ES_SD.ISensor> sourceReader)
		{
			var id = sourceReader.GetValue(c => c.Id);

			var existsQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensor>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.Id, ConditionOperator.Equal, id);

			var exists = infocDbScope.Executor.ExecuteAndFetch(existsQuery, reader => reader.Read());

			if (exists)
			{
				var updQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensor>()
					.Update()
					.SetValue(c => c.Name, sourceReader.GetValue(c => c.Name))
					.SetValue(c => c.Status, sourceReader.GetValue(c => c.Status))
					.SetValue(c => c.TechId, sourceReader.GetValue(c => c.TechId))
					.SetValue(c => c.SensorIdentifierId, sourceReader.GetValue(c => c.SensorIdentifierId))
					.SetValue(c => c.Agl, sourceReader.GetValue(c => c.Agl))
					.SetValue(c => c.Azimuth, sourceReader.GetValue(c => c.Azimuth))
					.SetValue(c => c.BiuseDate, sourceReader.GetValue(c => c.BiuseDate))
					.SetValue(c => c.Elevation, sourceReader.GetValue(c => c.Elevation))
					.SetValue(c => c.EouseDate, sourceReader.GetValue(c => c.EouseDate))
					.SetValue(c => c.RxLoss, sourceReader.GetValue(c => c.RxLoss))
					.Where(c => c.Id, ConditionOperator.Equal, id);

				infocDbScope.Executor.Execute(updQuery);

				return false;
			}
			else
			{
				var insQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensor>()
					.Insert()
					.SetValue(c => c.Id, id)
					.SetValue(c => c.Name, sourceReader.GetValue(c => c.Name))
					.SetValue(c => c.Status, sourceReader.GetValue(c => c.Status))
					.SetValue(c => c.TechId, sourceReader.GetValue(c => c.TechId))
					.SetValue(c => c.SensorIdentifierId, sourceReader.GetValue(c => c.SensorIdentifierId))
					.SetValue(c => c.Agl, sourceReader.GetValue(c => c.Agl))
					.SetValue(c => c.Azimuth, sourceReader.GetValue(c => c.Azimuth))
					.SetValue(c => c.BiuseDate, sourceReader.GetValue(c => c.BiuseDate))
					.SetValue(c => c.Elevation, sourceReader.GetValue(c => c.Elevation))
					.SetValue(c => c.EouseDate, sourceReader.GetValue(c => c.EouseDate))
					.SetValue(c => c.RxLoss, sourceReader.GetValue(c => c.RxLoss));

				infocDbScope.Executor.Execute(insQuery);

				return true;
			}
		}

		private void SynchronizeSensorAntennas(IDataLayerScope infocDbScope, IDataLayerScope sdrnsDbScope)
		{
			var statusNote = "Sync completed successfully";
			var statusCode = ES_IC.IntegrationStatusCode.Done;
			var fetchRows = _config.AutoImportSdrnServerSensorAntennasFetchRows.GetValueOrDefault(1000); ;
			var offsetRows = 0;
			var createdCount = 0;
			var updatedCount = 0;
			var token = _integrationService.Start(DataSource.SdrnServer, IntegrationObjects.SensorAntennas);

			try
			{
				var needFetch = true;
				while (needFetch)
				{
					var sdrnsSensorQuery = _sdrnsDataLayer.GetBuilder<ES_SD.ISensorAntenna>()
						.From()
						.Select(c => c.Id)
						.Select(c => c.SENSOR.Id)
						.Select(c => c.Code)
						.Select(c => c.Manufacturer)
						.Select(c => c.Name)
						.Select(c => c.TechId)
						.Select(c => c.AntDir)
						.Select(c => c.HbeamWidth)
						.Select(c => c.VbeamWidth)
						.Select(c => c.Polarization)
						.Select(c => c.GainType)
						.Select(c => c.GainMax)
						.Select(c => c.LowerFreq)
						.Select(c => c.UpperFreq)
						.Select(c => c.AddLoss)
						.Select(c => c.Xpd)
						.Where(c => c.SENSOR.Id, ConditionOperator.IsNotNull)
						.OrderByAsc(c => c.Id)
						.Paginate(offsetRows, fetchRows);

					needFetch = sdrnsDbScope.Executor.ExecuteAndFetch(sdrnsSensorQuery, reader =>
					{
						var result = false;
						while (reader.Read())
						{
							++offsetRows;
							result = true;

							var created = this.SynchronizeSensorAntenna(infocDbScope, reader);
							if (created)
							{
								++createdCount;
							}
							else
							{
								++updatedCount;
							}
						}

						return result;
					});
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
					$"Read={offsetRows}, Created={createdCount}, Updated={updatedCount}";

				_integrationService.Finish(token, statusCode, statusNote, total);
			}
		}

		private bool SynchronizeSensorAntenna(IDataLayerScope infocDbScope, IDataReader<ES_SD.ISensorAntenna> sourceReader)
		{
			var id = sourceReader.GetValue(c => c.Id);

			var existsQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorAntenna>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.Id, ConditionOperator.Equal, id);

			var exists = infocDbScope.Executor.ExecuteAndFetch(existsQuery, reader => reader.Read());

			if (exists)
			{
				var updQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorAntenna>()
					.Update()
					.SetValue(c => c.Code, sourceReader.GetValue(c => c.Code))
					.SetValue(c => c.Polarization, sourceReader.GetValue(c => c.Polarization))
					.SetValue(c => c.Name, sourceReader.GetValue(c => c.Name))
					.SetValue(c => c.TechId, sourceReader.GetValue(c => c.TechId))
					.SetValue(c => c.GainType, sourceReader.GetValue(c => c.GainType))
					.SetValue(c => c.Xpd, sourceReader.GetValue(c => c.Xpd))
					.SetValue(c => c.AntDir, sourceReader.GetValue(c => c.AntDir))
					.SetValue(c => c.AddLoss, sourceReader.GetValue(c => c.AddLoss))
					.SetValue(c => c.GainMax, sourceReader.GetValue(c => c.GainMax))
					.SetValue(c => c.UpperFreq, sourceReader.GetValue(c => c.UpperFreq))
					.SetValue(c => c.HbeamWidth, sourceReader.GetValue(c => c.HbeamWidth))
					.SetValue(c => c.VbeamWidth, sourceReader.GetValue(c => c.VbeamWidth))
					.SetValue(c => c.LowerFreq, sourceReader.GetValue(c => c.LowerFreq))
					.SetValue(c => c.Manufacturer, sourceReader.GetValue(c => c.Manufacturer))
					//.SetValue(c => c.SENSOR.Id, sourceReader.GetValue(c => c.SENSOR.Id))
					.Where(c => c.Id, ConditionOperator.Equal, id);

				infocDbScope.Executor.Execute(updQuery);

				return false;
			}
			else
			{
				var insQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorAntenna>()
					.Insert()
					.SetValue(c => c.Id, id)
					.SetValue(c => c.Code, sourceReader.GetValue(c => c.Code))
					.SetValue(c => c.Polarization, sourceReader.GetValue(c => c.Polarization))
					.SetValue(c => c.Name, sourceReader.GetValue(c => c.Name))
					.SetValue(c => c.TechId, sourceReader.GetValue(c => c.TechId))
					.SetValue(c => c.GainType, sourceReader.GetValue(c => c.GainType))
					.SetValue(c => c.Xpd, sourceReader.GetValue(c => c.Xpd))
					.SetValue(c => c.AntDir, sourceReader.GetValue(c => c.AntDir))
					.SetValue(c => c.AddLoss, sourceReader.GetValue(c => c.AddLoss))
					.SetValue(c => c.GainMax, sourceReader.GetValue(c => c.GainMax))
					.SetValue(c => c.UpperFreq, sourceReader.GetValue(c => c.UpperFreq))
					.SetValue(c => c.HbeamWidth, sourceReader.GetValue(c => c.HbeamWidth))
					.SetValue(c => c.VbeamWidth, sourceReader.GetValue(c => c.VbeamWidth))
					.SetValue(c => c.LowerFreq, sourceReader.GetValue(c => c.LowerFreq))
					.SetValue(c => c.Manufacturer, sourceReader.GetValue(c => c.Manufacturer))
					.SetValue(c => c.SENSOR.Id, sourceReader.GetValue(c => c.SENSOR.Id));

				infocDbScope.Executor.Execute(insQuery);

				return true;
			}
		}

		private void SynchronizeSensorAntennaPatterns(IDataLayerScope infocDbScope, IDataLayerScope sdrnsDbScope)
		{
			var statusNote = "Sync completed successfully";
			var statusCode = ES_IC.IntegrationStatusCode.Done;
			var fetchRows = _config.AutoImportSdrnServerSensorAntennaPatternsFetchRows.GetValueOrDefault(1000); ;
			var offsetRows = 0;
			var createdCount = 0;
			var updatedCount = 0;
			var token = _integrationService.Start(DataSource.SdrnServer, IntegrationObjects.SensorAntennaPatterns);

			try
			{
				var needFetch = true;
				while (needFetch)
				{
					var sdrnsSensorQuery = _sdrnsDataLayer.GetBuilder<ES_SD.IAntennaPattern>()
						.From()
						.Select(c => c.Id)
						.Select(c => c.SENSOR_ANTENNA.Id)
						.Select(c => c.Gain)
						.Select(c => c.DiagA)
						.Select(c => c.DiagH)
						.Select(c => c.DiagV)
						.Select(c => c.Freq)
						.Where(c => c.SENSOR_ANTENNA.Id, ConditionOperator.IsNotNull)
						.OrderByAsc(c => c.Id)
						.Paginate(offsetRows, fetchRows);

					needFetch = sdrnsDbScope.Executor.ExecuteAndFetch(sdrnsSensorQuery, reader =>
					{
						var result = false;
						while (reader.Read())
						{
							++offsetRows;
							result = true;

							var created = this.SynchronizeSensorAntennaPattern(infocDbScope, reader);
							if (created)
							{
								++createdCount;
							}
							else
							{
								++updatedCount;
							}
						}

						return result;
					});
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
					$"Read={offsetRows}, Created={createdCount}, Updated={updatedCount}";

				_integrationService.Finish(token, statusCode, statusNote, total);
			}
		}

		private bool SynchronizeSensorAntennaPattern(IDataLayerScope infocDbScope, IDataReader<ES_SD.IAntennaPattern> sourceReader)
		{
			var id = sourceReader.GetValue(c => c.Id);

			var existsQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorAntennaPattern>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.Id, ConditionOperator.Equal, id);

			var exists = infocDbScope.Executor.ExecuteAndFetch(existsQuery, reader => reader.Read());

			if (exists)
			{
				var updQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorAntennaPattern>()
					.Update()
					.SetValue(c => c.Freq, sourceReader.GetValue(c => c.Freq))
					.SetValue(c => c.DiagA, sourceReader.GetValue(c => c.DiagA))
					.SetValue(c => c.Gain, sourceReader.GetValue(c => c.Gain))
					.SetValue(c => c.DiagV, sourceReader.GetValue(c => c.DiagV))
					//.SetValue(c => c.SENSOR_ANTENNA.Id, sourceReader.GetValue(c => c.SENSOR_ANTENNA.Id))
					.SetValue(c => c.DiagH, sourceReader.GetValue(c => c.DiagH))
					.Where(c => c.Id, ConditionOperator.Equal, id);

				infocDbScope.Executor.Execute(updQuery);

				return false;
			}
			else
			{
				var insQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorAntennaPattern>()
					.Insert()
					.SetValue(c => c.Id, id)
					.SetValue(c => c.Freq, sourceReader.GetValue(c => c.Freq))
					.SetValue(c => c.DiagA, sourceReader.GetValue(c => c.DiagA))
					.SetValue(c => c.Gain, sourceReader.GetValue(c => c.Gain))
					.SetValue(c => c.DiagV, sourceReader.GetValue(c => c.DiagV))
					.SetValue(c => c.SENSOR_ANTENNA.Id, sourceReader.GetValue(c => c.SENSOR_ANTENNA.Id))
					.SetValue(c => c.DiagH, sourceReader.GetValue(c => c.DiagH));

				infocDbScope.Executor.Execute(insQuery);

				return true;
			}
		}

		private void SynchronizeSensorEquipmentItems(IDataLayerScope infocDbScope, IDataLayerScope sdrnsDbScope)
		{
			var statusNote = "Sync completed successfully";
			var statusCode = ES_IC.IntegrationStatusCode.Done;
			var fetchRows = _config.AutoImportSdrnServerSensorEquipmentFetchRows.GetValueOrDefault(1000); ;
			var offsetRows = 0;
			var createdCount = 0;
			var updatedCount = 0;
			var token = _integrationService.Start(DataSource.SdrnServer, IntegrationObjects.SensorEquipment);

			try
			{
				var needFetch = true;
				while (needFetch)
				{
					var sdrnsSensorQuery = _sdrnsDataLayer.GetBuilder<ES_SD.ISensorEquipment>()
						.From()
						.Select(c => c.Id)
						.Select(c => c.SENSOR.Id)
						.Select(c => c.Code)
						.Select(c => c.Manufacturer)
						.Select(c => c.Name)
						.Select(c => c.TechId)
						.Select(c => c.LowerFreq)
						.Select(c => c.UpperFreq)
						.Where(c => c.SENSOR.Id, ConditionOperator.IsNotNull)
						.OrderByAsc(c => c.Id)
						.Paginate(offsetRows, fetchRows);

					needFetch = sdrnsDbScope.Executor.ExecuteAndFetch(sdrnsSensorQuery, reader =>
					{
						var result = false;
						while (reader.Read())
						{
							++offsetRows;
							result = true;

							var created = this.SynchronizeSensorEquipmentItem(infocDbScope, reader);
							if (created)
							{
								++createdCount;
							}
							else
							{
								++updatedCount;
							}
						}

						return result;
					});
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
					$"Read={offsetRows}, Created={createdCount}, Updated={updatedCount}";

				_integrationService.Finish(token, statusCode, statusNote, total);
			}
		}

		private bool SynchronizeSensorEquipmentItem(IDataLayerScope infocDbScope, IDataReader<ES_SD.ISensorEquipment> sourceReader)
		{
			var id = sourceReader.GetValue(c => c.Id);

			var existsQuery = _infocDataLayer.GetBuilder< ES_IC.SdrnServer.ISensorEquipment>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.Id, ConditionOperator.Equal, id);

			var exists = infocDbScope.Executor.ExecuteAndFetch(existsQuery, reader => reader.Read());

			if (exists)
			{
				var updQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorEquipment>()
					.Update()
					.SetValue(c => c.Name, sourceReader.GetValue(c => c.Name))
					.SetValue(c => c.Code, sourceReader.GetValue(c => c.Code))
					.SetValue(c => c.Manufacturer, sourceReader.GetValue(c => c.Manufacturer))
					.SetValue(c => c.TechId, sourceReader.GetValue(c => c.TechId))
					.SetValue(c => c.LowerFreq, sourceReader.GetValue(c => c.LowerFreq))
					.SetValue(c => c.UpperFreq, sourceReader.GetValue(c => c.UpperFreq))
					//.SetValue(c => c.SENSOR.Id, sourceReader.GetValue(c => c.SENSOR.Id))
					.Where(c => c.Id, ConditionOperator.Equal, id);

				infocDbScope.Executor.Execute(updQuery);

				return false;
			}
			else
			{
				var insQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorEquipment>()
					.Insert()
					.SetValue(c => c.Id, id)
					.SetValue(c => c.Name, sourceReader.GetValue(c => c.Name))
					.SetValue(c => c.Code, sourceReader.GetValue(c => c.Code))
					.SetValue(c => c.Manufacturer, sourceReader.GetValue(c => c.Manufacturer))
					.SetValue(c => c.TechId, sourceReader.GetValue(c => c.TechId))
					.SetValue(c => c.LowerFreq, sourceReader.GetValue(c => c.LowerFreq))
					.SetValue(c => c.UpperFreq, sourceReader.GetValue(c => c.UpperFreq))
					.SetValue(c => c.SENSOR.Id, sourceReader.GetValue(c => c.SENSOR.Id));

				infocDbScope.Executor.Execute(insQuery);

				return true;
			}
		}

		private void SynchronizeSensorEquipmentSensitivities(IDataLayerScope infocDbScope, IDataLayerScope sdrnsDbScope)
		{
			var statusNote = "Sync completed successfully";
			var statusCode = ES_IC.IntegrationStatusCode.Done;
			var fetchRows = _config.AutoImportSdrnServerSensorEquipmentSensitivitiesFetchRows.GetValueOrDefault(1000); ;
			var offsetRows = 0;
			var createdCount = 0;
			var updatedCount = 0;
			var token = _integrationService.Start(DataSource.SdrnServer, IntegrationObjects.SensorEquipmentSensitivities);

			try
			{
				var needFetch = true;
				while (needFetch)
				{
					var sdrnsSensorQuery = _sdrnsDataLayer.GetBuilder<ES_SD.ISensorSensitivites>()
						.From()
						.Select(c => c.Id)
						.Select(c => c.SENSOR_EQUIP.Id)
						.Select(c => c.AddLoss)
						.Select(c => c.Freq)
						.Select(c => c.FreqStability)
						.Select(c => c.Ktbf)
						.Select(c => c.Noisef)
						.Where(c => c.SENSOR_EQUIP.Id, ConditionOperator.IsNotNull)
						.OrderByAsc(c => c.Id)
						.Paginate(offsetRows, fetchRows);

					needFetch = sdrnsDbScope.Executor.ExecuteAndFetch(sdrnsSensorQuery, reader =>
					{
						var result = false;
						while (reader.Read())
						{
							++offsetRows;
							result = true;

							var created = this.SynchronizeSensorEquipmentSensitivity(infocDbScope, reader);
							if (created)
							{
								++createdCount;
							}
							else
							{
								++updatedCount;
							}
						}

						return result;
					});
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
					$"Read={offsetRows}, Created={createdCount}, Updated={updatedCount}";

				_integrationService.Finish(token, statusCode, statusNote, total);
			}
		}

		private bool SynchronizeSensorEquipmentSensitivity(IDataLayerScope infocDbScope, IDataReader<ES_SD.ISensorSensitivites> sourceReader)
		{
			var id = sourceReader.GetValue(c => c.Id);

			var existsQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorEquipmentSensitivity>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.Id, ConditionOperator.Equal, id);

			var exists = infocDbScope.Executor.ExecuteAndFetch(existsQuery, reader => reader.Read());

			if (exists)
			{
				var updQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorEquipmentSensitivity>()
					.Update()
					//.SetValue(c => c.SENSOR_EQUIP.Id, sourceReader.GetValue(c => c.SENSOR_EQUIP.Id))
					.SetValue(c => c.AddLoss, sourceReader.GetValue(c => c.AddLoss))
					.SetValue(c => c.Freq, sourceReader.GetValue(c => c.Freq))
					.SetValue(c => c.FreqStability, sourceReader.GetValue(c => c.FreqStability))
					.SetValue(c => c.Ktbf, sourceReader.GetValue(c => c.Ktbf))
					.SetValue(c => c.Noisef, sourceReader.GetValue(c => c.Noisef))
					.Where(c => c.Id, ConditionOperator.Equal, id);

				infocDbScope.Executor.Execute(updQuery);

				return false;
			}
			else
			{
				var insQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorEquipmentSensitivity>()
					.Insert()
					.SetValue(c => c.Id, id)
					.SetValue(c => c.SENSOR_EQUIP.Id, sourceReader.GetValue(c => c.SENSOR_EQUIP.Id))
					.SetValue(c => c.AddLoss, sourceReader.GetValue(c => c.AddLoss))
					.SetValue(c => c.Freq, sourceReader.GetValue(c => c.Freq))
					.SetValue(c => c.FreqStability, sourceReader.GetValue(c => c.FreqStability))
					.SetValue(c => c.Ktbf, sourceReader.GetValue(c => c.Ktbf))
					.SetValue(c => c.Noisef, sourceReader.GetValue(c => c.Noisef));

				infocDbScope.Executor.Execute(insQuery);

				return true;
			}
		}

		private void SynchronizeSensorLocations(IDataLayerScope infocDbScope, IDataLayerScope sdrnsDbScope)
		{
			var statusNote = "Sync completed successfully";
			var statusCode = ES_IC.IntegrationStatusCode.Done;
			var fetchRows = _config.AutoImportSdrnServerSensorLocationsFetchRows.GetValueOrDefault(1000); ;
			var offsetRows = 0;
			var createdCount = 0;
			var updatedCount = 0;
			var token = _integrationService.Start(DataSource.SdrnServer, IntegrationObjects.SensorLocations);

			try
			{
				var needFetch = true;
				while (needFetch)
				{
					var sdrnsSensorQuery = _sdrnsDataLayer.GetBuilder<ES_SD.ISensorLocation>()
						.From()
						.Select(c => c.Id)
						.Select(c => c.Lon)
						.Select(c => c.Lat)
						.Select(c => c.SENSOR.Id)
						.Select(c => c.Asl)
						.Select(c => c.DateCreated)
						.Select(c => c.DateFrom)
						.Select(c => c.DateTo)
						.Select(c => c.Status)
						.Where(c => c.SENSOR.Id, ConditionOperator.IsNotNull)
						.OrderByAsc(c => c.Id)
						.Paginate(offsetRows, fetchRows);

					needFetch = sdrnsDbScope.Executor.ExecuteAndFetch(sdrnsSensorQuery, reader =>
					{
						var result = false;
						while (reader.Read())
						{
							++offsetRows;
							result = true;

							var created = this.SynchronizeSensorLocation(infocDbScope, reader);
							if (created)
							{
								++createdCount;
							}
							else
							{
								++updatedCount;
							}
						}

						return result;
					});
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
					$"Read={offsetRows}, Created={createdCount}, Updated={updatedCount}";

				_integrationService.Finish(token, statusCode, statusNote, total);
			}
		}

		private bool SynchronizeSensorLocation(IDataLayerScope infocDbScope, IDataReader<ES_SD.ISensorLocation> sourceReader)
		{
			var id = sourceReader.GetValue(c => c.Id);

			var existsQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorLocation>()
				.From()
				.Select(c => c.Id)
				.Where(c => c.Id, ConditionOperator.Equal, id);

			var exists = infocDbScope.Executor.ExecuteAndFetch(existsQuery, reader => reader.Read());

			if (exists)
			{
				var updQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorLocation>()
					.Update()
					.SetValue(c => c.Lon, sourceReader.GetValue(c => c.Lon))
					.SetValue(c => c.Lat, sourceReader.GetValue(c => c.Lat))
					.SetValue(c => c.DateCreated, sourceReader.GetValue(c => c.DateCreated))
					//.SetValue(c => c.SENSOR.Id, sourceReader.GetValue(c => c.SENSOR.Id))
					.SetValue(c => c.DateFrom, sourceReader.GetValue(c => c.DateFrom))
					.SetValue(c => c.DateTo, sourceReader.GetValue(c => c.DateTo))
					.SetValue(c => c.Status, sourceReader.GetValue(c => c.Status))
					.SetValue(c => c.Asl, sourceReader.GetValue(c => c.Asl))
					.Where(c => c.Id, ConditionOperator.Equal, id);

				infocDbScope.Executor.Execute(updQuery);

				return false;
			}
			else
			{
				var insQuery = _infocDataLayer.GetBuilder<ES_IC.SdrnServer.ISensorLocation>()
					.Insert()
					.SetValue(c => c.Id, id)
					.SetValue(c => c.Lon, sourceReader.GetValue(c => c.Lon))
					.SetValue(c => c.Lat, sourceReader.GetValue(c => c.Lat))
					.SetValue(c => c.DateCreated, sourceReader.GetValue(c => c.DateCreated))
					.SetValue(c => c.SENSOR.Id, sourceReader.GetValue(c => c.SENSOR.Id))
					.SetValue(c => c.DateFrom, sourceReader.GetValue(c => c.DateFrom))
					.SetValue(c => c.DateTo, sourceReader.GetValue(c => c.DateTo))
					.SetValue(c => c.Status, sourceReader.GetValue(c => c.Status))
					.SetValue(c => c.Asl, sourceReader.GetValue(c => c.Asl));

				infocDbScope.Executor.Execute(insQuery);

				return true;
			}
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
					var firstResultId = _config.AutoImportSdrnServerStationMonitoringFirstResultId.GetValueOrDefault(0);

					syncKey = new StationMonitoringSyncKey
					{
						LastResultId = firstResultId > 0 ? firstResultId - 1 : firstResultId
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
					.Select(c => c.SUBTASK_SENSOR.SENSOR.Id)
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
							SensorId = reader.GetValue(c => c.SUBTASK_SENSOR.SENSOR.Id),
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
				if (sdrnMeasResult.SensorId.HasValue)
				{
					insQuery.SetValue(c => c.SENSOR.Id, sdrnMeasResult.SensorId.Value);
				}
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
						.SetValue(c => c.MaxFreq_MHz, (maxFreq_MHz==double.MinValue || maxFreq_MHz == double.MaxValue) ? 0 : maxFreq_MHz)
                        .SetValue(c => c.MinFreq_MHz, (minFreq_MHz == double.MinValue || minFreq_MHz == double.MaxValue) ? 0 : minFreq_MHz)
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
