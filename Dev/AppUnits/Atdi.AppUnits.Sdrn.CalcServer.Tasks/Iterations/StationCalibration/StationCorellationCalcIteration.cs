using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.DeepServices.Gis;


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
	/// <summary>
    /// 
	/// </summary>
	public class StationCorellationCalcIteration : IIterationHandler<StationCorellationCalcData, ResultCorrelationGSIDGroupeStationsWithoutParameters>
	{
		private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly ITransformation _transformation;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public StationCorellationCalcIteration(
            IIterationsPool iterationsPool,
            ITransformation transformation,
            ILogger logger)
        {
            _iterationsPool = iterationsPool;
            _transformation = transformation;
            _logger = logger;
        }

        public ResultCorrelationGSIDGroupeStationsWithoutParameters Run(ITaskContext taskContext, StationCorellationCalcData data)
		{
            var resultCorrelationGSIDGroupeStationsWithoutParameters = new ResultCorrelationGSIDGroupeStationsWithoutParameters();

            // ниже просто пример преобразования координат с Dec в метры
            // подставляем код проекции и координаты с драйв теста в метод преобразования координат с Dec в метры
            var coordinate_m = _transformation.ConvertCoordinateToEpgs(data.GSIDGroupeDriveTests[0].Points[0].Coordinate, data.CodeProjection);
            
            // заполняем поля TargetCoordinate и TargetAltitude_m
            data.FieldStrengthCalcData.TargetCoordinate = coordinate_m;
            data.FieldStrengthCalcData.TargetAltitude_m = data.GSIDGroupeDriveTests[0].Points[0].Height_m;
            
            // вызываем механизм расчета FieldStrengthCalcData на основе переданных данных data.FieldStrengthCalcData
            var iterationFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();
            var resultFieldStrengthCalcData = iterationFieldStrengthCalcData.Run(taskContext, data.FieldStrengthCalcData);

            
            return resultCorrelationGSIDGroupeStationsWithoutParameters;
        }
	}
}
