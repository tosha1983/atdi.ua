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
using Atdi.Platform.Logging;
using Atdi.Platform.Data;


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
        private readonly IObjectPool<CalcPoint[]> _calcPointArrayPool;
        private readonly IObjectPoolSite _poolSite;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public StationCorellationCalcIteration(
            IIterationsPool iterationsPool,
            ITransformation transformation,
            IObjectPoolSite poolSite,
            ILogger logger)
        {
            _iterationsPool = iterationsPool;
            _transformation = transformation;
            _poolSite = poolSite;
            _calcPointArrayPool = _poolSite.GetPool<CalcPoint[]>(ObjectPools.StationCalibrationCalcPointArrayObjectPool);
            _logger = logger;
        }

        public ResultCorrelationGSIDGroupeStationsWithoutParameters Run(ITaskContext taskContext, StationCorellationCalcData data)
		{
            var calcPointArrayBuffer = default(CalcPoint[]);
            calcPointArrayBuffer = _calcPointArrayPool.Take();
            try
            {
                calcPointArrayBuffer = _calcPointArrayPool.Take();


            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (calcPointArrayBuffer != null)
                {
                    _calcPointArrayPool.Put(calcPointArrayBuffer);
                }
            }
            return null;
        }
	}
}
