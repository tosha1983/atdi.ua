using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;

namespace Atdi.AppUnits.Icsm.CoverageEstimation
{
    public sealed class EstimationJobExecutor : IJobExecutor
    {
        private readonly ILogger _logger;

        public EstimationJobExecutor(ILogger logger)
        {
            // через конструктр заказываем нужные сервисы
            _logger = logger;
        }

        public JobExecutionResult Execute(JobExecutionContext context)
        {
            _logger.Verbouse("CoverageEstimation", "Execution", $"The Coverage Estimation Job is starting: StartAttempts=#{context.Token.StartAttempts}, RecoveryAttempts=#{context.Token.RecoveryAttempts}, IsRepeat={context.IsRepeat}, IsRecovery={context.IsRecovery}");
            try
            {
                // тут полезная нагрузка и все что нужно

                // можем понять режим запуска
                if (context.IsRecovery)
                {
                    // запуск после ошибки
                }
                if (!context.IsRepeat)
                {
                    // это первый запуска
                }

                if (context.Token.StartAttempts == 5)
                {
                    // это пятый запуск
                }

                if (context.Token.RecoveryAttempts == 10)
                {
                    // уже было 10 запусков востановления
                }

                // если есть длительные цыклы, обязательно проверять на отмену
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return JobExecutionResult.Canceled;
                }

                if (context.Token.StartAttempts > 10)
                {
                    // это пятый запуск
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
