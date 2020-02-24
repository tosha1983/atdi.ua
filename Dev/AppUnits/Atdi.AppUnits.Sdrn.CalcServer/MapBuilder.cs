using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.CalcServer.Helpers;
using Atdi.Common;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.Infocenter;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using IC=Atdi.DataModels.Sdrn.Infocenter.Entities;
using Atdi.Platform.Logging;
using DM = Atdi.AppUnits.Sdrn.CalcServer.DataModel;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal class MapBuilder
	{
		private readonly AppServerComponentConfig _config;
		private readonly IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> _infocenterDataLayer;
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly ILogger _logger;

		public MapBuilder(
			AppServerComponentConfig config,
			IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> infocenterDataLayer,
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			ILogger logger)
		{
			_config = config;
			_infocenterDataLayer = infocenterDataLayer;
			_calcServerDataLayer = calcServerDataLayer;
			_logger = logger;
		}

		public void PrepareMap(long projectMapId)
		{
			try
			{
				_logger.Verbouse(Contexts.ThisComponent, Categories.MapPreparation, $"Preparing project map with id #{projectMapId}");

				using (var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>())
				{
					// ставим статус обработки

					if (!this.ChangeProjectMapStatus(calcDbScope, projectMapId, ProjectMapStatusCode.Processing, ProjectMapStatusCode.Pending))
					{
						throw new InvalidOperationException($"Failed to set project map current status in processing status.");
					}

					try
					{
						// все измененяи по карте в рамках тразакции
						calcDbScope.BeginTran();

						// чистим контекст карты
						CleanProjectMapContext(calcDbScope, projectMapId);

						// читаем свойства карты проекта
						var projectMapData = this.LoadProjectMapData(calcDbScope, projectMapId);

						// проверим свойства шага: шаговый бокс и ко-во
						CheckAxis(projectMapData);

						// расчитываем область карты и проверяем ее с трешхолодом (не более 15 000 х 15 000 шагов), при нарушении падаем
						CheckThresholdMaxSteps(projectMapData.StepsNumber);

						// проверка проекции
						CheckProjection(projectMapData.Projection);

						// проверка единици измерения карты
						CheckStepUnit(projectMapData.StepUnit);

						// расчитываем конечные координаты относительно указаных шагов
						projectMapData.LowerRightX =
							projectMapData.UpperLeftX + projectMapData.AxisXNumber * projectMapData.AxisXStep;
						projectMapData.LowerRightY =
							projectMapData.UpperLeftY + projectMapData.AxisYNumber * projectMapData.AxisYStep;

						// сохраним расчеті
						SaveProjectMapData(calcDbScope, projectMapId, projectMapData);

						// с этого момент можем приступить к построеннию карт
						using (var infoDbScope = this._infocenterDataLayer.CreateScope<InfocenterDataContext>())
						{
							// строим рельеф
							this.BuildMap(calcDbScope, infoDbScope, projectMapData, ProjectMapType.Relief);
							this.BuildMap(calcDbScope, infoDbScope, projectMapData, ProjectMapType.Building);
							this.BuildMap(calcDbScope, infoDbScope, projectMapData, ProjectMapType.Clutter);
						}
							
						// фиксируем состояние
						this.ChangeProjectMapStatus(calcDbScope, projectMapId, ProjectMapStatusCode.Prepared);
						calcDbScope.Commit();
						_logger.Verbouse(Contexts.ThisComponent, Categories.MapPreparation, $"Prepared project map with id #{projectMapId}");
					}
					catch (Exception e)
					{
						calcDbScope.Rollback();
						this.ChangeProjectMapStatus(calcDbScope, projectMapId, ProjectMapStatusCode.Failed, e.ToString());
						throw;
					}
				}
			}
			catch (Exception e)
			{
				_logger.Exception(Contexts.ThisComponent, Categories.MapPreparation, $"Failed to prepare map with ID ${projectMapId}", e, this);
			}
			
		}

		private void BuildMap(IDataLayerScope calcDbScope, IDataLayerScope infoDbScope, DM.ProjectMapData data, ProjectMapType mapType)
		{
			var sourceMaps = this.FindSourceMaps(infoDbScope, data, mapType);
			if (sourceMaps.Length == 0)
			{
				throw new InvalidOperationException($"Could not find any matching maps of type '{mapType}' in the Infocenter DB");
			}

			var stepDataSize = DefineMapStepDataSize(mapType);

			// выделяем буффер
			var mapBuffer = new byte[data.AxisXNumber * data.AxisYNumber * stepDataSize];

			for (int i = 0; i < sourceMaps.Length; i++)
			{
				var sourceMap = sourceMaps[i];

				// TODO:  тут сложная логика наложения карт с которпой необходим оопределиться

				// 1. цыкл по покрытой площади
				// 2.   подгружаем нужный сектор карты если нужно
				// 3.   опредедляем значение  игнорируя ячейки которые были уже определны
				// 4. помечаем покрытую площадь как занятую
				// 5. переходим к следущей карте
			}

			// сохраянем буффер - контент ProjectMapContent и все его источники ProjectMapContentSource
			var contentId = this.SaveProjectMapContent(calcDbScope, data.ProjectMapId, mapType, stepDataSize,
				sourceMaps.Length, 100, mapBuffer);

			foreach (var sourceMap in sourceMaps)
			{
				this.SaveSourceMap(calcDbScope, contentId, sourceMap);
			}
		}

		private long SaveProjectMapContent(IDataLayerScope calcDbScope, long projectMapId, ProjectMapType mapType, int stepDataSize,  int sourceCount, decimal? sourceCoverage, byte[] buffer)
		{
			var compressedBuffer = Compressor.Compress(buffer);
			var insertQuery = _calcServerDataLayer.GetBuilder<IProjectMapContent>()
					.Insert()
					.SetValue(c => c.MAP.Id, projectMapId)
					.SetValue(c => c.TypeCode, (byte) mapType)
					.SetValue(c => c.TypeName, mapType.ToString())
					.SetValue(c => c.StepDataSize, stepDataSize)
					.SetValue(c => c.StepDataType, DefineMapStepDataType(mapType).AssemblyQualifiedName)
					.SetValue(c => c.SourceCount, sourceCount)
					.SetValue(c => c.SourceCoverage, sourceCoverage)
					.SetValue(c => c.ContentSize, buffer.Length)
					.SetValue(c => c.ContentType, compressedBuffer.GetType().AssemblyQualifiedName)
					.SetValue(c => c.ContentEncoding, "compressed")
					.SetValue(c => c.Content, compressedBuffer)
				;

			var pk = calcDbScope.Executor.Execute<IProjectMapContent_PK>(insertQuery);
			return pk.Id;
		}

		private static Type DefineMapStepDataType(ProjectMapType mapType)
		{
			if (mapType == ProjectMapType.Clutter || mapType == ProjectMapType.Building)
			{
				return typeof(byte);
			}
			else if (mapType == ProjectMapType.Relief)
			{
				return typeof(short);
			}
			throw new InvalidOperationException($"Unsupported the project map type '{mapType}'");
		}

		private long SaveSourceMap(IDataLayerScope calcDbScope, long contentId, DM.SourceMapData source)
		{
			var insertQuery = _calcServerDataLayer.GetBuilder<IProjectMapContentSource>()
					.Insert()
					.SetValue(c => c.CONTENT.Id, contentId)
					.SetValue(c => c.InfocMapId, source.MapId)
					.SetValue(c => c.InfocMapName, source.MapName)
					.SetValue(c => c.Coverage, source.CoveragePercent)
					.SetValue(c => c.UpperLeftX, source.CoverageUpperLeftX)
					.SetValue(c => c.UpperLeftY, source.CoverageUpperLeftY)
					.SetValue(c => c.LowerRightX, source.CoverageLowerRightX)
					.SetValue(c => c.LowerRightY, source.CoverageLowerRightY)
				;

			var pk = calcDbScope.Executor.Execute<IProjectMapContentSource_PK>(insertQuery);
			return pk.Id;
		}

		private static byte DefineMapStepDataSize(ProjectMapType mapType)
		{
			if (mapType == ProjectMapType.Clutter || mapType == ProjectMapType.Building)
			{
				return 1;
			}
			else if (mapType == ProjectMapType.Relief)
			{
				return 2;
			}
			throw new InvalidOperationException($"Unsupported the map type '{mapType}'");
		}

		private DM.SourceMapData[] FindSourceMaps(IDataLayerScope infoDbScope, DM.ProjectMapData data, ProjectMapType mapType)
		{
			var sourceMapQuery = _infocenterDataLayer.GetBuilder<IC.IMap>()
				.From()
				.Select(
					c => c.Id,
					c => c.MapName,
					c => c.UpperLeftX,
					c => c.UpperLeftY,
					c => c.LowerRightX,
					c => c.LowerRightY,
					c => c.AxisXNumber,
					c => c.AxisXStep,
					c => c.AxisYNumber,
					c => c.AxisYStep,
					c => c.StepDataSize,
					c => c.CreatedDate)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)IC.MapStatusCode.Available)
				.Where(c => c.TypeCode, ConditionOperator.Equal, (byte)mapType) // одного типа
				.Where(c => c.Projection, ConditionOperator.Equal, data.Projection) // одной проэкции
				.Where(c => c.StepUnit, ConditionOperator.Equal, data.StepUnit) // одной единицы измерения 
				.OrderByDesc(c => c.CreatedDate);

			return infoDbScope.Executor.ExecuteAndFetch(sourceMapQuery, reader =>
			{
				var sourceMaps = new List<DM.SourceMapData>();
				while (reader.Read())
				{
					var sourceMap = new DM.SourceMapData()
					{
						AxisXNumber = reader.GetValue(c => c.AxisXNumber),
						AxisXStep = reader.GetValue(c => c.AxisXStep),
						AxisYNumber = reader.GetValue(c => c.AxisXNumber),
						AxisYStep = reader.GetValue(c => c.AxisXNumber),
						UpperLeftX = reader.GetValue(c => c.UpperLeftX),
						UpperLeftY = reader.GetValue(c => c.UpperLeftY),
						LowerRightX = reader.GetValue(c => c.LowerRightX),
						LowerRightY = reader.GetValue(c => c.LowerRightY),
						MapId = reader.GetValue(c => c.Id),
						StepDataSize = reader.GetValue(c => c.StepDataSize),
						MapName = reader.GetValue(c => c.MapName)
					};

					if (sourceMap.IntersectWith(data))
					{
						sourceMaps.Add(sourceMap);
					}
				}
				return sourceMaps
					.OrderBy(c => c.Priority)
					.ThenByDescending(c => c.CoveragePercent)
					.ThenByDescending(c => c.MapId)
					.ToArray(); 
			});
	
		}

		private void CheckAxis(DM.ProjectMapData data)
		{
			if (data.AxisXNumber <= 0)
			{
				throw new InvalidOperationException("Incorrect value of AxisXNumber");
			}
			if (data.AxisYNumber <= 0)
			{
				throw new InvalidOperationException("Incorrect value of AxisYNumber");
			}
			if (data.AxisXStep <= 0)
			{
				throw new InvalidOperationException("Incorrect value of AxisXStep");
			}
			if (data.AxisYStep <= 0)
			{
				throw new InvalidOperationException("Incorrect value of AxisYStep");
			}
		}
		private void CheckThresholdMaxSteps(int stepsNumber)
		{
			var thresholdMaxSteps = _config.ThresholdMaxSteps ?? 15000 * 15000;
			if (stepsNumber > thresholdMaxSteps)
			{
				throw new InvalidOperationException(
					$"Violation of the threshold value of the total number of steps. The threshold value is {thresholdMaxSteps}. The project map total number of steps is {stepsNumber}.");
			}
		}

		private  void CheckProjection(string projectionName)
		{
			if (string.IsNullOrEmpty(projectionName))
			{
				throw new InvalidOperationException("Undefined a projection");
			}

			if (projectionName.Length >= 4)
			{
				if ("4UTN".Equals(projectionName.SubString(4)))
				{
					return;
				}
			}

			throw new InvalidOperationException($"Unsupported the projection '{projectionName}'");
		}

		private void CheckStepUnit(string stepUnit)
		{
			if (string.IsNullOrEmpty(stepUnit))
			{
				throw new InvalidOperationException("Undefined a step unit");
			}

			if (stepUnit.Length == 1)
			{
				if ("M".Equals(stepUnit))
				{
					return;
				}
			}

			throw new InvalidOperationException($"Unsupported the step unit '{stepUnit}'");
		}

		private void CleanProjectMapContext(IDataLayerScope dbScope, long projectMapId)
		{
			var sourceDeleteQuery = _calcServerDataLayer.GetBuilder<IProjectMapContentSource>()
				.Delete()
				.Where(c => c.CONTENT.MAP.Id, ConditionOperator.Equal, projectMapId);
			var contentDeleteQuery = _calcServerDataLayer.GetBuilder<IProjectMapContent>()
				.Delete()
				.Where(c => c.MAP.Id, ConditionOperator.Equal, projectMapId);

			var sourceDelCount = dbScope.Executor.Execute(sourceDeleteQuery);
			var contentDelCount = dbScope.Executor.Execute(contentDeleteQuery);
		}

		private bool ChangeProjectMapStatus(IDataLayerScope dbScope, long projectMapId, ProjectMapStatusCode newStatus, string statusNote = null)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)newStatus)
				.SetValue(c => c.StatusName, newStatus.ToString())
				.SetValue(c => c.StatusNote, statusNote)
				.Where(c => c.Id, ConditionOperator.Equal, projectMapId);

			return dbScope.Executor.Execute(query) > 0;
		}
		private bool ChangeProjectMapStatus(IDataLayerScope dbScope, long projectMapId, ProjectMapStatusCode newStatus, ProjectMapStatusCode oldStatus , string statusNote = null)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.Update()
				.SetValue(c => c.StatusCode, (byte)newStatus)
				.SetValue(c => c.StatusName, newStatus.ToString())
				.SetValue(c => c.StatusNote, statusNote)
				.Where(c => c.Id, ConditionOperator.Equal, projectMapId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)oldStatus);

			return dbScope.Executor.Execute(query) > 0;
		}

		private DM.ProjectMapData LoadProjectMapData(IDataLayerScope dbScope, long projectMapId)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.From()
				.Select(
					c => c.AxisXNumber,
					c => c.AxisXStep,
					c => c.AxisYNumber,
					c => c.AxisYStep,
					c => c.LowerRightX,
					c => c.LowerRightY,
					c => c.UpperLeftX,
					c => c.UpperLeftY,
					c => c.Projection,
					c => c.StepUnit)
				.Where(c => c.Id, ConditionOperator.Equal, projectMapId);

			return dbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				DM.ProjectMapData map = null;
				while (reader.Read())
				{
					map = new DM.ProjectMapData()
					{
						ProjectMapId = projectMapId,
						AxisXNumber = reader.GetValue(c => c.AxisXNumber),
						AxisXStep = reader.GetValue(c => c.AxisXStep),
						AxisYNumber = reader.GetValue(c => c.AxisXNumber),
						AxisYStep = reader.GetValue(c => c.AxisXNumber),
						UpperLeftX = reader.GetValue(c => c.UpperLeftX),
						UpperLeftY = reader.GetValue(c => c.UpperLeftY),
						LowerRightX = reader.GetValue(c => c.LowerRightX),
						LowerRightY = reader.GetValue(c => c.LowerRightY),
						Projection = reader.GetValue(c => c.Projection),
						StepUnit = reader.GetValue(c => c.StepUnit),
					};

				}
				return map;
			});
		}

		private bool SaveProjectMapData(IDataLayerScope dbScope, long projectMapId, DM.ProjectMapData data)
		{
			var query = _calcServerDataLayer.GetBuilder<IProjectMap>()
				.Update()
				.SetValue(c => c.LowerRightX, data.LowerRightX)
				.SetValue(c => c.LowerRightY, data.LowerRightY)
				.Where(c => c.Id, ConditionOperator.Equal, projectMapId)
				.Where(c => c.StatusCode, ConditionOperator.Equal, (byte)ProjectMapStatusCode.Processing);

			return dbScope.Executor.Execute(query) > 0;
		}
	}
}
