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

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
	/// <summary>
    /// 
	/// </summary>
	public class DetermineStationParametersCalcIteration : IIterationHandler<AllStationCorellationCalcData, ResultCalibration>
	{
		private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public DetermineStationParametersCalcIteration(IIterationsPool iterationsPool, ILogger logger)
        {
            _iterationsPool = iterationsPool;
            _logger = logger;
        }

        public ResultCalibration Run(ITaskContext taskContext, AllStationCorellationCalcData data)
        {
            var calcCorellationResult = new ResultCalibration();
            for (int i = 0; i < data.GSIDGroupeStation.Length; i++)
            {
                var stationCorellationCalcData = new StationCorellationCalcData()
                {
                    GSIDGroupeStation = data.GSIDGroupeStation[i],
                    CorellationParameters = data.CorellationParameters,
                    GSIDGroupeDriveTests = data.GSIDGroupeDriveTests,
                    FieldStrengthCalcData = data.FieldStrengthCalcData[i],
                    GeneralParameters = data.GeneralParameters,
                    CodeProjection = data.CodeProjection
                };

                var stationCalibrationCalcData = new StationCalibrationCalcData()
                {
                    GSIDGroupeStation = data.GSIDGroupeStation[i],
                    CorellationParameters = data.CorellationParameters,
                    GSIDGroupeDriveTests = data.GSIDGroupeDriveTests,
                    FieldStrengthCalcData = data.FieldStrengthCalcData[i],
                    CalibrationParameters = data.CalibrationParameters,
                    GeneralParameters = data.GeneralParameters,
                    CodeProjection = data.CodeProjection
                };

                var iterationCorellationCalc = _iterationsPool.GetIteration<StationCorellationCalcData, ResultCorrelationGSIDGroupeStations>();
                var resultCorellationCalcData = iterationCorellationCalc.Run(taskContext, stationCorellationCalcData);

                var iterationCalibrationCalc = _iterationsPool.GetIteration<StationCalibrationCalcData, ResultCorrelationGSIDGroupeStations>();
                var resultCalibrationCalcData = iterationCalibrationCalc.Run(taskContext, stationCalibrationCalcData);
            }
            return calcCorellationResult;
        }

       

    }
}
