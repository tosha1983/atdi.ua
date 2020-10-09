using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal class ClientContextService : IClientContextService
	{
		private class StationDto
		{
			public long StationId;
			public Wgs84Coordinate Coordinate;
		}

		private readonly ITransformation _transformation;
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly ILogger _logger;

		public ClientContextService(
			ITransformation  transformation,
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			ILogger logger)
		{
			_transformation = transformation;
			_calcServerDataLayer = calcServerDataLayer;
			_logger = logger;
		}

		public ClientContext GetContextById(IDataLayerScope dbScope, long contextId)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContext>()
				.From()
				.Select(c => c.Id)
				.Select(c => c.PROJECT.Id)
				.Select(c => c.OwnerInstance)
				.Select(c => c.CreatedDate)
				.Select(c => c.OwnerContextId)
				.Select(c => c.StatusCode)
				.Where(c => c.Id, ConditionOperator.Equal, contextId);

			var clientContext = dbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					throw new InvalidOperationException($"Client context with ID #{contextId} not found");
				}

				return new ClientContext
				{
					Id = reader.GetValue(c => c.Id),
					ProjectId = reader.GetValue(c => c.PROJECT.Id),
					CreatedDate = reader.GetValue(c => c.CreatedDate),
					OwnerContextId = reader.GetValue(c => c.OwnerContextId),
					OwnerInstance = reader.GetValue(c => c.OwnerInstance),
					Status = (ClientContextStatus)reader.GetValue(c => c.StatusCode)
				};
			});

			return clientContext;
		}

		private void FillPropagationModelParameters(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextGlobalParams>()
				.From()
				.Select(c => c.EarthRadius_km)
				.Select(c => c.Location_pc)
				.Select(c => c.Time_pc)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.Parameters = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new GlobalParams
					{
						Time_pc = GlobalParams.TimeDefault,
						Location_pc = GlobalParams.LocationDefault,
						EarthRadius_km = GlobalParams.EarthRadiusDefault
					};
				}

				return new GlobalParams
				{
					Time_pc = reader.GetValue( c => c.Time_pc).GetValueOrDefault(GlobalParams.TimeDefault),
					Location_pc = reader.GetValue(c => c.Location_pc).GetValueOrDefault(GlobalParams.LocationDefault),
					EarthRadius_km = reader.GetValue(c => c.EarthRadius_km).GetValueOrDefault(GlobalParams.EarthRadiusDefault),
				};
			});
		}

		private void FillPropagationModelMainBlock(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextMainBlock>()
				.From()
				.Select(c => c.ModelTypeCode)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.MainBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new MainCalcBlock
					{
						ModelType = MainCalcBlockModelType.Unknown
					};
				}
				return new MainCalcBlock
				{
					ModelType = (MainCalcBlockModelType) reader.GetValue(c => c.ModelTypeCode)
				};
			});
		}

		private void FillPropagationModelDiffraction(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextDiffraction>()
				.From()
				.Select(c => c.ModelTypeCode)
				.Select(c => c.Available)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.DiffractionBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new DiffractionCalcBlock
					{
						ModelType = DiffractionCalcBlockModelType.Unknown,
						Available = false
					};
				}
				return new DiffractionCalcBlock
				{
					ModelType = (DiffractionCalcBlockModelType)reader.GetValue(c => c.ModelTypeCode),
					Available = reader.GetValue(c => c.Available)
				};
			});
		}

		private void FillPropagationModelSubPathDiffraction(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextSubPathDiffraction>()
				.From()
				.Select(c => c.ModelTypeCode)
				.Select(c => c.Available)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.SubPathDiffractionBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new SubPathDiffractionCalcBlock
					{
						ModelType = SubPathDiffractionCalcBlockModelType.Unknown,
						Available = false
					};
				}
				return new SubPathDiffractionCalcBlock
				{
					ModelType = (SubPathDiffractionCalcBlockModelType)reader.GetValue(c => c.ModelTypeCode),
					Available = reader.GetValue(c => c.Available)
				};
			});
		}

		private void FillPropagationModelTropo(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextTropo>()
				.From()
				.Select(c => c.ModelTypeCode)
				.Select(c => c.Available)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.TropoBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new TropoCalcBlock
					{
						ModelType = TropoCalcBlockModelType.Unknown,
						Available = false
					};
				}
				return new TropoCalcBlock
				{
					ModelType = (TropoCalcBlockModelType)reader.GetValue(c => c.ModelTypeCode),
					Available = reader.GetValue(c => c.Available)
				};
			});
		}

		private void FillPropagationModelDucting(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextDucting>()
				.From()
				.Select(c => c.Available)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.DuctingBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new DuctingCalcBlock
					{
						Available = false
					};
				}
				return new DuctingCalcBlock
				{
					Available = reader.GetValue(c => c.Available)
				};
			});
		}

		private void FillPropagationModelAbsorption(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextAbsorption>()
				.From()
				.Select(c => c.ModelTypeCode)
				.Select(c => c.Available)
                .Select(c => c.Hybrid)
                .Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.AbsorptionBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new AbsorptionCalcBlock
					{
						ModelType = AbsorptionCalcBlockModelType.Unknown,
						Available = false,
                        Hybrid = false
					};
				}
				return new AbsorptionCalcBlock
				{
					ModelType = (AbsorptionCalcBlockModelType)reader.GetValue(c => c.ModelTypeCode),
					Available = reader.GetValue(c => c.Available),
                    Hybrid = reader.GetValue(c => c.Hybrid)
                };
			});
		}

		private void FillPropagationModelReflection(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextReflection>()
				.From()
				.Select(c => c.Available)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.ReflectionBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new ReflectionCalcBlock
					{
						Available = false
					};
				}
				return new ReflectionCalcBlock
				{
					Available = reader.GetValue(c => c.Available)
				};
			});
		}

		private void FillPropagationModelAtmospheric(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextAtmospheric>()
				.From()
				.Select(c => c.ModelTypeCode)
				.Select(c => c.Available)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.AtmosphericBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new AtmosphericCalcBlock
					{
						ModelType = AtmosphericCalcBlockModelType.Unknown,
						Available = false
					};
				}
				return new AtmosphericCalcBlock
				{
					ModelType = (AtmosphericCalcBlockModelType)reader.GetValue(c => c.ModelTypeCode),
					Available = reader.GetValue(c => c.Available)
				};
			});
		}

		private void FillPropagationModelAdditional(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextAdditional>()
				.From()
				.Select(c => c.ModelTypeCode)
				.Select(c => c.Available)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.AdditionalBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new AdditionalCalcBlock
					{
						ModelType = AdditionalCalcBlockModelType.Unknown,
						Available = false
					};
				}
				return new AdditionalCalcBlock
				{
					ModelType = (AdditionalCalcBlockModelType)reader.GetValue(c => c.ModelTypeCode),
					Available = reader.GetValue(c => c.Available)
				};
			});
		}

		private void FillPropagationModelClutter(IDataLayerScope calcDbScope, long contextId, PropagationModel model)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContextClutter>()
				.From()
				.Select(c => c.ModelTypeCode)
				.Select(c => c.Available)
				.Where(c => c.ContextId, ConditionOperator.Equal, contextId);

			model.ClutterBlock = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return new ClutterCalcBlock
					{
						ModelType = ClutterCalcBlockModelType.Unknown,
						Available = false
					};
				}
				return new ClutterCalcBlock
				{
					ModelType = (ClutterCalcBlockModelType)reader.GetValue(c => c.ModelTypeCode),
					Available = reader.GetValue(c => c.Available)
				};
			});
		}

		public void PrepareContext(IDataLayerScope dbScope, long contextId)
		{
			try
			{
				_logger.Verbouse(Contexts.ThisComponent, Categories.ContextPreparation, $"Preparing client context with id #{contextId}");

				using (var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
				{
					// ставим статус обработки

					if (!this.ChangeContextStatus(calcDbScope, contextId, ClientContextStatusCode.Processing, ClientContextStatusCode.Pending))
					{
						throw new InvalidOperationException($"Failed to set client context current status in processing status.");
					}

					try
					{
						// все измененяи в рамках тразакции
						//calcDbScope.BeginTran();

						// пока обрабатываем только координаты
						this.PrepareStationsSites(calcDbScope, contextId);

						// фиксируем состояние
						this.ChangeContextStatus(calcDbScope, contextId, ClientContextStatusCode.Prepared);
						//calcDbScope.Commit();
						_logger.Verbouse(Contexts.ThisComponent, Categories.MapPreparation, $"Prepared project map with id #{contextId}");
					}
					catch (Exception e)
					{
						//calcDbScope.Rollback();
						this.ChangeContextStatus(calcDbScope, contextId, ClientContextStatusCode.Failed, e.ToString());
						throw;
					}
				}
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.MapPreparation, $"Failed to prepare client context with ID ${contextId}", e, this);
			}
		}

		private void PrepareStationsSites(IDataLayerScope calcDbScope, long contextId)
		{
			var contextQuery = _calcServerDataLayer.GetBuilder<IClientContext>()
				.From()
				.Select(c => c.PROJECT.Projection)
				.Where(c => c.Id, ConditionOperator.Equal, contextId);

			var projection = calcDbScope.Executor.ExecuteAndFetch(contextQuery, reader =>
			{
				if (!reader.Read())
				{
					throw new InvalidOperationException($"Client context with ID #{contextId} not found");
				}
				return reader.GetValue(c => c.PROJECT.Projection);
			});

			var query = _calcServerDataLayer.GetBuilder<IContextStation>()
				.From()
				.Select(c => c.Id)
				.Select(c => c.CONTEXT.PROJECT.Projection)
				.Select(c => c.SITE.Latitude_DEC)
				.Select(c => c.SITE.Longitude_DEC)
				.Where(c => c.CONTEXT.Id, ConditionOperator.Equal, contextId);

			var stations = calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				var records = new List<StationDto>();
				while (reader.Read())
				{
					var record = new StationDto
					{
						StationId = reader.GetValue(c => c.Id),
						Coordinate = new Wgs84Coordinate
						{
							Latitude = reader.GetValue(c => c.SITE.Latitude_DEC),
							Longitude = reader.GetValue(c => c.SITE.Longitude_DEC)
						}
					};
					records.Add(record);
				}
				return records;
			});

			var projectionId = _transformation.ConvertProjectionToCode(projection);

			foreach (var station in stations)
			{
				var delQuery = _calcServerDataLayer.GetBuilder<IContextStationCoordinates>()
					.Delete()
					.Where(c => c.StationId, ConditionOperator.Equal, station.StationId);
				calcDbScope.Executor.Execute(delQuery);

				var epsgCoordinate = _transformation.ConvertCoordinateToEpgs(station.Coordinate, projectionId);

				var insQuery = _calcServerDataLayer.GetBuilder<IContextStationCoordinates>()
					.Insert()
					.SetValue(c => c.StationId, station.StationId)
					.SetValue(c => c.EpsgX, epsgCoordinate.X)
					.SetValue(c => c.EpsgY, epsgCoordinate.Y)
					.SetValue(c => c.AtdiX, (int)epsgCoordinate.X)
					.SetValue(c => c.AtdiY, (int)epsgCoordinate.Y);
				calcDbScope.Executor.Execute(insQuery);
			}
		}

		private bool ChangeContextStatus(IDataLayerScope dbScope, long contextId, ClientContextStatusCode newStatus, string statusNote = null)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContext>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)newStatus)
				.SetValue(c => c.StatusName, newStatus.ToString())
				.SetValue(c => c.StatusNote, statusNote)
				.Where(c => c.Id, ConditionOperator.Equal, contextId);

			return dbScope.Executor.Execute(query) > 0;
		}

		private bool ChangeContextStatus(IDataLayerScope dbScope, long contextId, ClientContextStatusCode newStatus, ClientContextStatusCode oldStatus, string statusNote = null)
		{
			var query = _calcServerDataLayer.GetBuilder<IClientContext>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)newStatus)
				.SetValue(c => c.StatusName, newStatus.ToString())
				.SetValue(c => c.StatusNote, statusNote)
				.Where(c => c.Id, ConditionOperator.Equal, contextId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)oldStatus);

			return dbScope.Executor.Execute(query) > 0;
		}

		public ClientContextStation GetContextStation(IDataLayerScope dbScope, long contextId, long stationId)
		{
			var query = _calcServerDataLayer.GetBuilder<IContextStation>()
					.From()
					.Select(
						c => c.Id,
						c => c.CONTEXT.Id,
						c => c.StateCode,
						c => c.CreatedDate,
						c => c.CallSign,
						c => c.Name,

						c => c.SITE.Latitude_DEC,
						c => c.SITE.Longitude_DEC,
						c => c.SITE.Altitude_m,

						c => c.COORDINATES.AtdiX,
						c => c.COORDINATES.AtdiY,

						c => c.ANTENNA.Azimuth_deg,
						c => c.ANTENNA.Gain_dB,
						c => c.ANTENNA.ItuPatternCode,
						c => c.ANTENNA.Tilt_deg,
						c => c.ANTENNA.XPD_dB,

						c => c.ANTENNA.HH_PATTERN.StationId,
						c => c.ANTENNA.HH_PATTERN.Angle_deg,
						c => c.ANTENNA.HH_PATTERN.Loss_dB,

						c => c.ANTENNA.HV_PATTERN.StationId,
						c => c.ANTENNA.HV_PATTERN.Angle_deg,
						c => c.ANTENNA.HV_PATTERN.Loss_dB,

						c => c.ANTENNA.VH_PATTERN.StationId,
						c => c.ANTENNA.VH_PATTERN.Angle_deg,
						c => c.ANTENNA.VH_PATTERN.Loss_dB,

						c => c.ANTENNA.VV_PATTERN.StationId,
						c => c.ANTENNA.VV_PATTERN.Angle_deg,
						c => c.ANTENNA.VV_PATTERN.Loss_dB,

						c => c.TRANSMITTER.StationId,
						c => c.TRANSMITTER.Loss_dB,
						c => c.TRANSMITTER.Freq_MHz,
						c => c.TRANSMITTER.BW_kHz,
						c => c.TRANSMITTER.MaxPower_dBm,
						c => c.TRANSMITTER.PolarizationCode
					)
					.Where(c => c.Id, ConditionOperator.Equal, stationId)
					.Where(c => c.CONTEXT.Id, ConditionOperator.Equal, contextId);

			var contextStation = dbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					throw new InvalidOperationException($"Context Station with ID #{stationId} not found in context with ID #{contextId}");
				}

				var stationRecord = new ClientContextStation
				{
					Id = reader.GetValue(c => c.Id),
					ContextId = contextId,
					CreatedDate = reader.GetValue(c => c.CreatedDate),
					Type = (ClientContextStationType)reader.GetValue(c => c.StateCode),
					CallSign = reader.GetValue(c => c.CallSign),
					Name = reader.GetValue(c => c.Name),
					Site = new Wgs84Site
					{
						Latitude = reader.GetValue(c => c.SITE.Latitude_DEC),
						Longitude = reader.GetValue(c => c.SITE.Longitude_DEC),
						Altitude = reader.GetValue(c => c.SITE.Altitude_m)
					},
					Coordinate = new AtdiCoordinate
					{
						X = reader.GetValue(c => c.COORDINATES.AtdiX),
						Y = reader.GetValue(c => c.COORDINATES.AtdiY)
					},
					Antenna = new StationAntenna
					{
						Gain_dB = reader.GetValue(c => c.ANTENNA.Gain_dB),
						XPD_dB = reader.GetValue(c => c.ANTENNA.XPD_dB),
						Azimuth_deg = reader.GetValue(c => c.ANTENNA.Azimuth_deg),
						ItuPattern = (AntennaItuPattern)reader.GetValue(c => c.ANTENNA.ItuPatternCode),
						Tilt_deg = reader.GetValue(c => c.ANTENNA.Tilt_deg)
					},
					
				};

				if (reader.IsNotNull(c => c.TRANSMITTER.StationId))
				{
					stationRecord.Transmitter = new StationTransmitter
					{
						Loss_dB = reader.GetValue(c => c.TRANSMITTER.Loss_dB),
						Polarization = (PolarizationType) reader.GetValue(c => c.TRANSMITTER.PolarizationCode),
						MaxPower_dBm = reader.GetValue(c => c.TRANSMITTER.MaxPower_dBm),
						BW_kHz = reader.GetValue(c => c.TRANSMITTER.BW_kHz),
						Freq_MHz = reader.GetValue(c => c.TRANSMITTER.Freq_MHz)
					};
				}

				if (reader.IsNotNull(c => c.ANTENNA.HH_PATTERN.StationId))
				{
					stationRecord.Antenna.HhPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.HH_PATTERN.Angle_deg);
					stationRecord.Antenna.HhPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.HH_PATTERN.Loss_dB);
				}
				if (reader.IsNotNull(c => c.ANTENNA.HV_PATTERN.StationId))
				{
					stationRecord.Antenna.HvPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.HV_PATTERN.Angle_deg);
					stationRecord.Antenna.HvPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.HV_PATTERN.Loss_dB);
				}
				if (reader.IsNotNull(c => c.ANTENNA.VH_PATTERN.StationId))
				{
					stationRecord.Antenna.VhPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.VH_PATTERN.Angle_deg);
					stationRecord.Antenna.VhPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.VH_PATTERN.Loss_dB);
				}
				if (reader.IsNotNull(c => c.ANTENNA.VV_PATTERN.StationId))
				{
					stationRecord.Antenna.VvPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.VV_PATTERN.Angle_deg);
					stationRecord.Antenna.VvPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.VV_PATTERN.Loss_dB);
				}
				return stationRecord;
			});


			return contextStation;
		}

		public PropagationModel GetPropagationModel(IDataLayerScope dbScope, long contextId)
		{
			var propagationModel = new PropagationModel();

			// параметры
			this.FillPropagationModelParameters(dbScope, contextId, propagationModel);
			this.FillPropagationModelMainBlock(dbScope, contextId, propagationModel);
			this.FillPropagationModelDiffraction(dbScope, contextId, propagationModel);
			this.FillPropagationModelSubPathDiffraction(dbScope, contextId, propagationModel);
			this.FillPropagationModelTropo(dbScope, contextId, propagationModel);
			this.FillPropagationModelDucting(dbScope, contextId, propagationModel);
			this.FillPropagationModelAbsorption(dbScope, contextId, propagationModel);
			this.FillPropagationModelReflection(dbScope, contextId, propagationModel);
			this.FillPropagationModelAtmospheric(dbScope, contextId, propagationModel);
			this.FillPropagationModelAdditional(dbScope, contextId, propagationModel);
			this.FillPropagationModelClutter(dbScope, contextId, propagationModel);

			return propagationModel;
		}
	}
}
