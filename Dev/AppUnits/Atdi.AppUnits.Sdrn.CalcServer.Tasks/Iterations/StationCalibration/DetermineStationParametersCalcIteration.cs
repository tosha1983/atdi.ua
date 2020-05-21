﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.Platform.Logging;
using Atdi.Platform.Data;


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    /// <summary>
    /// 
    /// </summary>
    public class DetermineStationParametersCalcIteration : IIterationHandler<AllStationCorellationCalcData, CalibrationResult>
    {
        private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly IObjectPool<DriveTestsResult[]> _driveTestsResultArrayPool;
        private readonly IObjectPoolSite _poolSite;
        private readonly AppServerComponentConfig _appServerComponentConfig;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public DetermineStationParametersCalcIteration(
            IIterationsPool iterationsPool,
            IObjectPoolSite poolSite,
            AppServerComponentConfig appServerComponentConfig,
            ILogger logger)
        {
            _iterationsPool = iterationsPool;
            _poolSite = poolSite;
            _appServerComponentConfig = appServerComponentConfig;
            _driveTestsResultArrayPool = _poolSite.GetPool<DriveTestsResult[]>(ObjectPools.StationCalibrationDriveTestsResultArrayObjectPool);
            _logger = logger;
        }

        public CalibrationResult Run(ITaskContext taskContext, AllStationCorellationCalcData data)
        {
            var calcCorellationResult = new CalibrationResult();
            var driveTestsResultBuffer = default(DriveTestsResult[]);
            try
            {
                driveTestsResultBuffer = _driveTestsResultArrayPool.Take();
                for (int i = 0; i < data.GSIDGroupeDriveTests.Length; i++)
                {
                    driveTestsResultBuffer[i] = data.GSIDGroupeDriveTests[i];

                    if ((driveTestsResultBuffer[i].Points!=null) && (driveTestsResultBuffer[i].Points.Length> _appServerComponentConfig.MaxCountPointInDriveTest))
                    {
                        // включение механизма перфорации Points
                    }
                }

                for (int i = 0; i < data.GSIDGroupeStation.Length; i++)
                {
                    var stationCorellationCalcData = new StationCorellationCalcData()
                    {
                        GSIDGroupeStation = data.GSIDGroupeStation[i],
                        CorellationParameters = data.CorellationParameters,
                        GSIDGroupeDriveTests = driveTestsResultBuffer,
                        FieldStrengthCalcData = data.FieldStrengthCalcData[i],
                        GeneralParameters = data.GeneralParameters
                    };

                    var stationCalibrationCalcData = new StationCalibrationCalcData()
                    {
                        GSIDGroupeStation = data.GSIDGroupeStation[i],
                        CorellationParameters = data.CorellationParameters,
                        GSIDGroupeDriveTests = driveTestsResultBuffer,
                        FieldStrengthCalcData = data.FieldStrengthCalcData[i],
                        CalibrationParameters = data.CalibrationParameters,
                        GeneralParameters = data.GeneralParameters,
                    };

                    var iterationCorellationCalc = _iterationsPool.GetIteration<StationCorellationCalcData, ResultCorrelationGSIDGroupeStations>();
                    var resultCorellationCalcData = iterationCorellationCalc.Run(taskContext, stationCorellationCalcData);

                    var iterationCalibrationCalc = _iterationsPool.GetIteration<StationCalibrationCalcData, ResultCorrelationGSIDGroupeStations>();
                    var resultCalibrationCalcData = iterationCalibrationCalc.Run(taskContext, stationCalibrationCalcData);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (driveTestsResultBuffer != null)
                {
                    _driveTestsResultArrayPool.Put(driveTestsResultBuffer);
                }
            }
            return calcCorellationResult;
        }
    }
}
