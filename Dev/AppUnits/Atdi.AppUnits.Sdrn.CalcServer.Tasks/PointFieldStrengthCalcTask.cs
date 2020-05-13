using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
	[TaskHandler(CalcTaskType.PointFieldStrengthCalc)]
	public class PointFieldStrengthCalcTask : ITaskHandler
	{
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
		private readonly IClientContextService _contextService;
		private readonly IMapRepository _mapRepository;
		private readonly IIterationsPool _iterationsPool;
		private readonly ITransformation _transformation;
		private readonly ILogger _logger;
		private ITaskContext _taskContext;
		private IDataLayerScope _calcDbScope;
		private TaskParameters _parameters;
		//private ClientContext _clientContext;
		private ClientContextStation _contextStation;
		private PropagationModel _propagationModel;
		private ProjectMapData _mapData;
		private CluttersDesc _cluttersDesc;

		private class TaskParameters
		{
			public long ContextStationId;

			public Wgs84Site PointSite;

			public string Projection;

			public string MapName;
		}

		public PointFieldStrengthCalcTask(
			IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
			IClientContextService contextService,
			IMapRepository mapRepository,
			IIterationsPool iterationsPool,
			ITransformation transformation,
			ILogger logger)
		{
			_calcServerDataLayer = calcServerDataLayer;
			_contextService = contextService;
			_mapRepository = mapRepository;
			_iterationsPool = iterationsPool;
			_transformation = transformation;
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

			//this._clientContext = _contextService.GetContextById(this._calcDbScope, taskContext.ClientContextId);
			this._contextStation = _contextService.GetContextStation(this._calcDbScope, taskContext.ClientContextId, _parameters.ContextStationId);
			this._propagationModel = _contextService.GetPropagationModel(this._calcDbScope, taskContext.ClientContextId);

			// тут валидация контекста

			// найти и загрузить карту
			this._mapData = _mapRepository.GetMapByName(this._calcDbScope, this._taskContext.ProjectId, this._parameters.MapName);
			this._cluttersDesc = _mapRepository.GetCluttersDesc(this._calcDbScope, this._mapData.Id);

		}
		

		public void Run()
		{
			// расчет координаты точки

			var iterationData = new FieldStrengthCalcData
			{
				Antenna = _contextStation.Antenna,
				PropagationModel = _propagationModel,
				PointCoordinate = _contextStation.Coordinate,
				PointAltitude_m = _contextStation.Site.Altitude,
				TargetCoordinate = _transformation.ConvertCoordinateToAtdi(_parameters.PointSite, _parameters.Projection),
				TargetAltitude_m = _parameters.PointSite.Altitude,
				MapArea = _mapData.Area,
				BuildingContent = _mapData.BuildingContent,
				ClutterContent = _mapData.ClutterContent,
				ReliefContent = _mapData.ReliefContent,
				Transmitter = _contextStation.Transmitter,
				CluttersDesc = _cluttersDesc
			};

			for (int i = 0; i < 1000; i++)
			{
				var iteration = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();
				var result = iteration.Run(_taskContext, iterationData);
			}
			

			///this.SaveTaskResult(in result);
		}

		private void ValidateTaskParameters()
		{
			if (this._parameters == null)
			{
				throw new InvalidOperationException("Undefined a task parameters");
			}
		}

		private void LoadTaskParameters()
		{
			var query = _calcServerDataLayer.GetBuilder<IPointFieldStrengthArgs>()
				.From()
				.Select(
					c => c.STATION.Id,
					c => c.PointAltitude_m,
					c => c.PointLatitude_DEC,
					c => c.PointLongitude_DEC,
					c => c.TASK.CONTEXT.PROJECT.Projection,
					c => c.TASK.MapName
				)
				.Where(c => c.TaskId, ConditionOperator.Equal, _taskContext.TaskId);

			this._parameters = _calcDbScope.Executor.ExecuteAndFetch(query, reader =>
			{
				if (!reader.Read())
				{
					return null;
				}
				return new TaskParameters()
				{
					ContextStationId = reader.GetValue(c => c.STATION.Id),
					PointSite = new Wgs84Site
					{
						Latitude = reader.GetValue(c => c.PointLatitude_DEC).GetValueOrDefault(),
						Altitude = reader.GetValue(c => c.PointAltitude_m).GetValueOrDefault(),
						Longitude = reader.GetValue(c => c.PointLongitude_DEC).GetValueOrDefault(),
					},
					Projection = reader.GetValue(c => c.TASK.CONTEXT.PROJECT.Projection),
					MapName = reader.GetValue(c => c.TASK.MapName)
				};
			});
		}

		private void SaveTaskResult(in FieldStrengthCalcResult result)
		{
			var updateQuery = _calcServerDataLayer.GetBuilder<IPointFieldStrengthResult>()
				.Update()
				.SetValue(c => c.FS_dBuVm, (float?)result.FS_dBuVm)
				.SetValue(c => c.Level_dBm, (float?)result.Level_dBm)
				.Where(c => c.ResultId, ConditionOperator.Equal, _taskContext.ResultId);
			var count = _calcDbScope.Executor.Execute(updateQuery);
			if (count == 0)
			{
				var insertQuery = _calcServerDataLayer.GetBuilder<IPointFieldStrengthResult>()
					.Insert()
					.SetValue(c => c.ResultId, _taskContext.ResultId)
					.SetValue(c => c.FS_dBuVm, (float?) result.FS_dBuVm)
					.SetValue(c => c.Level_dBm, (float?) result.Level_dBm);
				count = _calcDbScope.Executor.Execute(insertQuery);
				if (count == 0)
				{
					throw new InvalidOperationException($"Can't create result record with ID #{_taskContext.ResultId}");
				}
			}
		}
	}
}
