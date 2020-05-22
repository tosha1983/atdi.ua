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
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
	/// <summary>
    /// 
	/// </summary>
	public class StationCalibrationCalcIteration : IIterationHandler<StationCalibrationCalcData, ResultCorrelationGSIDGroupeStations>
	{
		private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly ITransformation _transformation;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public StationCalibrationCalcIteration(
            IIterationsPool iterationsPool,
            ITransformation transformation,
            ILogger logger)
		{
            _transformation = transformation;
            _iterationsPool = iterationsPool;
            _logger = logger;
		}

		public ResultCorrelationGSIDGroupeStations Run(ITaskContext taskContext, StationCalibrationCalcData data)
		{
            var calcCorellationResult = new ResultCorrelationGSIDGroupeStations();


            // заполняем поля TargetCoordinate и TargetAltitude_m (координаты уже преобразованы в метры)
            data.FieldStrengthCalcData.TargetCoordinate = data.GSIDGroupeDriveTests[0].Points[0].Coordinate;
            data.FieldStrengthCalcData.TargetAltitude_m = data.GSIDGroupeDriveTests[0].Points[0].Height_m;

            // вызываем механизм расчета FieldStrengthCalcData на основе переданных данных data.FieldStrengthCalcData
            var iterationFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();
            var resultFieldStrengthCalcData = iterationFieldStrengthCalcData.Run(taskContext, data.FieldStrengthCalcData);

            return  calcCorellationResult;
		}
	}
}
