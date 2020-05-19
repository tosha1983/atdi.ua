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
	public class StationCorellationCalcIteration : IIterationHandler<StationCalibrationCalcData, bool>
	{
		private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public StationCorellationCalcIteration(IIterationsPool iterationsPool, ILogger logger)
        {
            _iterationsPool = iterationsPool;
            _logger = logger;
        }

        public bool Run(ITaskContext taskContext, StationCalibrationCalcData data)
		{
            var iterationFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();

            for (int i = 0; i < data.GSIDGroupeStations.Length; i++)
            {
                var resultFieldStrengthCalcData = iterationFieldStrengthCalcData.Run(taskContext, data.FieldStrengthCalcData[i]);

            }
            return false;
        }
	}
}
