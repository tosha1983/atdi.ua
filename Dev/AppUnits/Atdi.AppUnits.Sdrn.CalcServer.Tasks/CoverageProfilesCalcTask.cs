using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using System.IO;
using Newtonsoft.Json;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
	[TaskHandler(CalcTaskType.CoverageProfilesCalc)]
	public class CoverageProfilesCalcTask : ITaskHandler
	{
		private class TaskParameters
		{
			public CoverageProfilesCalcModeCode Mode { get; set; }

			public int[] PointsX { get; set; }

			public int[] PointsY { get; set; }

			public string ResultPath { get; set; }

			public string MapName { get; set; }
		}

		private class ResultProfile
		{
			public Coordinate Point;
			public Coordinate Target;

			public ResultProfileRecord[] Records { get; set; }

			public int Count { get; set; }
		}

		private struct ResultProfileRecord
		{
			public int Num;
			public int Index;
			public Indexer Indexer;
			public short Relief;
			public byte Clutter;
			public byte Building;
		};

		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly IMapService _mapService;
		private readonly IIterationsPool _iterationsPool;
		private readonly ILogger _logger;
		private ITaskContext _taskContext;
		private IDataLayerScope _calcDbScope;
		private TaskParameters _parameters;
		private ProjectMapData _mapData;

		public CoverageProfilesCalcTask(
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			IMapService mapService,
			IIterationsPool iterationsPool,
			ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
			_mapService = mapService;
			_iterationsPool = iterationsPool;
			_logger = logger;
		}

		public void Dispose()
		{
			if (_calcDbScope != null)
			{
				_calcDbScope.Dispose();
				_calcDbScope = null;
			}

			_taskContext = null;
		}

		public void Load(ITaskContext taskContext)
		{
			this._taskContext = taskContext;
			this._calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>();
			
			// загрузить параметры задачи
			this.LoadTaskParameters();
			this.ValidateTaskParameters();

			// найти и загрузить карту
			this._mapData = _mapService.GetMapByName(this._taskContext.ProjectId, this._parameters.MapName);

			// проверить принадлежность точек к карте

			for (var i = 0; i < _parameters.PointsX.Length; i++)
			{
				var x = _parameters.PointsX[i];
				var y = _parameters.PointsY[i];

				if (!this._mapData.Has(x, y))
				{
					throw new InvalidOperationException($"Incorrect a task parameters. Target point coordinates ({x}:{y}) with index #{i} do not belong to map {this._mapData}");
				}
			}
		}

		public void Run()
		{
			// имя файла результата
			var fileName = Path.Combine(_parameters.ResultPath,
				$"profile_P{_taskContext.ProjectId:D6}_T{_taskContext.TaskId:D8}_R{_taskContext.ResultId:D10}");

			// беффер хранения индексов получени яданных профиля
			var indexesBuffer = new Indexer[this._mapData.AxisX.Number + this._mapData.AxisY.Number];

			var iteration = this._iterationsPool.GetIteration<ProfileIndexersCalcData, int>();

			if (_parameters.Mode == CoverageProfilesCalcModeCode.InPairs)
			{
				
				for (var i = 0; i < _parameters.PointsX.Length; i++)
				{
					var fullFileName = fileName + $"_I{i:D5}.mpf";

					var point = new Coordinate()
					{
						// ReSharper disable once PossibleInvalidOperationException
						X = this._parameters.PointsX[i],
						// ReSharper disable once PossibleInvalidOperationException
						Y = this._parameters.PointsY[i]
					};
					var target = new Coordinate()
					{
						X = _parameters.PointsX[++i],
						Y = _parameters.PointsY[i]
					};

					CalcProfile(indexesBuffer, point, target, iteration, fullFileName);
				}
			}
			else if (_parameters.Mode == CoverageProfilesCalcModeCode.FirstWithAll)
			{
				var point = new Coordinate()
				{
					// ReSharper disable once PossibleInvalidOperationException
					X = this._parameters.PointsX[0],
					// ReSharper disable once PossibleInvalidOperationException
					Y = this._parameters.PointsY[0]
				};
				for (var i = 1; i < _parameters.PointsX.Length; i++)
				{
					var target = new Coordinate()
					{
						X = _parameters.PointsX[i],
						Y = _parameters.PointsY[i]
					};

					var fullFileName = fileName + $"_I{i:D5}.mpf";
					CalcProfile(indexesBuffer, point, target, iteration, fullFileName);
				}
			}
			else if (_parameters.Mode == CoverageProfilesCalcModeCode.AllWithAll)
			{
				for (var i = 1; i < _parameters.PointsX.Length; i++)
				{
					var point = new Coordinate()
					{
						// ReSharper disable once PossibleInvalidOperationException
						X = this._parameters.PointsX[i],
						// ReSharper disable once PossibleInvalidOperationException
						Y = this._parameters.PointsY[i]
					};
					for (var j = i + 1; j < _parameters.PointsX.Length; j++)
					{
						var target = new Coordinate()
						{
							X = _parameters.PointsX[j],
							Y = _parameters.PointsY[j]
						};

						var fullFileName = fileName + $"_I{i:D5}X{j:D5}.mpf";
						CalcProfile(indexesBuffer, point, target, iteration, fullFileName);
					}
				}
			}
			else
			{
				throw new InvalidOperationException($"Unsupported a calculation profile mode {_parameters.Mode}");
			}

			
		}

		private void CalcProfile(Indexer[] indexesBuffer, Coordinate point, Coordinate target, IIterationHandler<ProfileIndexersCalcData, int> iteration,
			string fileName)
		{
			var iterationData = new ProfileIndexersCalcData
			{
				Result = indexesBuffer,
				Area = _mapData.Area,
				Point = point,
				Target = target
			};

			var count = iteration.Run(this._taskContext, iterationData);

			var result = new ResultProfile()
			{
				Point = point,
				Target = target,
				Records = new ResultProfileRecord[count],
				Count = count
			};
			for (var j = 0; j < count; j++)
			{
				var indexer = iterationData.Result[j];
				var mapIndex = indexer.YIndex * _mapData.AxisX.Number + indexer.XIndex;

				result.Records[j] = new ResultProfileRecord
				{
					Num = j,
					Index = mapIndex,
					Indexer = indexer,
					Relief = _mapData.ReliefContent[mapIndex],
					Building = _mapData.BuildingContent[mapIndex],
					Clutter = _mapData.ClutterContent[mapIndex]
				};
			}

			File.AppendAllText(fileName, JsonConvert.SerializeObject(result, Formatting.Indented), Encoding.UTF8);
		}

		private void ValidateTaskParameters()
		{
			if (this._parameters == null)
			{
				throw new InvalidOperationException("Undefined a task parameters");
			}
			

			if (this._parameters.PointsX == null || this._parameters.PointsX.Length == 0)
			{
				throw new InvalidOperationException("Undefined value of target points X");
			}

			if (this._parameters.PointsY == null || this._parameters.PointsY.Length == 0)
			{
				throw new InvalidOperationException("Undefined value of target points Y");
			}

			if (this._parameters.PointsY.Length != this._parameters.PointsX.Length)
			{
				throw new InvalidOperationException("Incorrect array size of target points");
			}

			if (this._parameters.Mode == CoverageProfilesCalcModeCode.InPairs && this._parameters.PointsY.Length % 2 != 0)
			{
				throw new InvalidOperationException("Incorrect array size of target points");
			}
		}

		private void LoadTaskParameters()
		{
			var query = _calcServerDataLayer.GetBuilder<ICoverageProfilesCalcTask>()
				.From()
				.Select(
					c => c.Id,
					c => c.ModeCode,
					c => c.PointsX,
					c => c.PointsY,
					c => c.ResultPath,
					c => c.MapName
				)
				.Where(c => c.Id, ConditionOperator.Equal, _taskContext.TaskId);

			this._parameters = _calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return null;
				}
				return new TaskParameters()
				{
					Mode = (CoverageProfilesCalcModeCode)reader.GetValue(c => c.ModeCode),
					PointsX = reader.GetValue(c => c.PointsX),
					PointsY = reader.GetValue(c => c.PointsY),
					ResultPath = reader.GetValue(c => c.ResultPath),
					MapName = reader.GetValue(c => c.MapName)
				};
			});
		}

		
	}
}
