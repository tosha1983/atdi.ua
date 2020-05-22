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

        private bool IsInsideMap(double lon, double lat, double lonMin, double latMin, double lonMax, double latMax)
        {
            if (lon > lonMin && lon < lonMin &&
                lat > latMin && lat < latMin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ResultCorrelationGSIDGroupeStationsWithoutParameters Run(ITaskContext taskContext, StationCorellationCalcData data)
		{
            var calcCorellationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();

            var calcPointArrayBuffer = default(CalcPoint[]);
            calcPointArrayBuffer = _calcPointArrayPool.Take();


            try
            {
                calcPointArrayBuffer = _calcPointArrayPool.Take();
                

                // Corner coordinates
                //data.FieldStrengthCalcData.MapArea.LowerLeft.X;
                var lowerLeftCoord_m = data.FieldStrengthCalcData.MapArea.LowerLeft;
                var upperRightCoord_m = data.FieldStrengthCalcData.MapArea.UpperRight;
                // Step
                double lonStep_dec = data.FieldStrengthCalcData.MapArea.AxisX.Step;//_transformation.ConvertCoordinateToWgs84(data.FieldStrengthCalcData.MapArea.LowerLeft.X, data.CodeProjection);
                double latStep_dec = data.FieldStrengthCalcData.MapArea.AxisY.Step;

                for (int i = 0; i < data.GSIDGroupeDriveTests.Points.Length; i++)
                {
                    if (data.GSIDGroupeDriveTests.Points[i].FieldStrength_dBmkVm >= data.CorellationParameters.MinRangeMeasurements_dBmkV &&
                        data.GSIDGroupeDriveTests.Points[i].FieldStrength_dBmkVm <= data.CorellationParameters.MaxRangeMeasurements_dBmkV &&
                        IsInsideMap(data.GSIDGroupeDriveTests.Points[i].Coordinate.X, data.GSIDGroupeDriveTests.Points[i].Coordinate.Y, lowerLeftCoord_m.X, lowerLeftCoord_m.Y, upperRightCoord_m.X, upperRightCoord_m.Y))
                    {
                        for (int j = 0; j < data.GSIDGroupeDriveTests.Points.Length; j++)
                        {
                            if (i != j)
                            {

                            }

                        }
                        //polintsList.Add(data.GSIDGroupeDriveTests[i].Points[j]);
                    }
                }

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
            return calcCorellationResult;
        }
	}
}
