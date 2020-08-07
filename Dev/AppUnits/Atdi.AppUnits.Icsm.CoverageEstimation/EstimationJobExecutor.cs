using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.AppUnits.Icsm.CoverageEstimation.Handlers;
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;


namespace Atdi.AppUnits.Icsm.CoverageEstimation
{
    public sealed class EstimationJobExecutor : IJobExecutor
    {
        private readonly ILogger _logger;
        private IDataLayer<IcsmDataOrm> _dataLayer { get; set; }
        private ICalcFinalCoverage _startCalcFinalCoverageForMobstation { get; set; }
        private ICalcFinalCoverage _startCalcFinalCoverageForMobstation2 { get; set; }
        private  AppServerComponentConfig _appServerComponentConfig { get; set; }
        private DataConfig _dataConfig { get; set; }


        public EstimationJobExecutor(IDataLayer<IcsmDataOrm> dataLayer, AppServerComponentConfig appServerComponentConfig, ILogger logger)
        {
            // через конструктр заказываем нужные сервисы
            this._logger = logger;
            this._appServerComponentConfig = appServerComponentConfig;
            this._dataLayer = dataLayer;
            this._dataConfig = CoverageConfig.Load(this._appServerComponentConfig);
            this._startCalcFinalCoverageForMobstation = new CalcFinalCoverageForMobStation(this._appServerComponentConfig, this._dataLayer, this._logger);
            this._startCalcFinalCoverageForMobstation2 = new CalcFinalCoverageForMobStation2(this._appServerComponentConfig, this._dataLayer, this._logger);
        }

        public JobExecutionResult Execute(JobExecutionContext context)
        {
            _logger.Verbouse("CoverageEstimation", "Execution", $"The Coverage Estimation Job is starting: StartAttempts=#{context.Token.StartAttempts}, RecoveryAttempts=#{context.Token.RecoveryAttempts}, IsRepeat={context.IsRepeat}, IsRecovery={context.IsRecovery}");
            try
            {
                if (context.Token.StartAttempts == 1)
                {
                    if (this._appServerComponentConfig.EnableMobStationCalculation)
                    {
                        this._startCalcFinalCoverageForMobstation.Run(this._dataConfig, context.Token.StartAttempts);
                    }
                    if (this._appServerComponentConfig.EnableMobStation2Calculation)
                    {
                        this._startCalcFinalCoverageForMobstation2.Run(this._dataConfig, context.Token.StartAttempts);
                    }
                }
                

                if (this._appServerComponentConfig.IsRepeatable==false)
                {
                    return JobExecutionResult.Canceled;
                }
                else
                {
                    if (this._appServerComponentConfig.EnableMobStationCalculation)
                    {
                        this._startCalcFinalCoverageForMobstation.Run(this._dataConfig, context.Token.StartAttempts);
                    }
                    if (this._appServerComponentConfig.EnableMobStation2Calculation)
                    {
                        this._startCalcFinalCoverageForMobstation2.Run(this._dataConfig, context.Token.StartAttempts);
                    }
                }

                // если есть длительные цыклы, обязательно проверять на отмену
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return JobExecutionResult.Canceled;
                }

                // удачное завершение
                return JobExecutionResult.Completed;
            }
            catch (Exception e)
            {
                _logger.Exception("CoverageEstimation", "Execution", e, this);
                return JobExecutionResult.Failure;
            }
            finally
            {
                _logger.Verbouse("CoverageEstimation", "Execution", "The Coverage Estimation Job has finished");
            }
        }
    }
}
