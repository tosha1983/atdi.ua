using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;

namespace Atdi.Test.Platform
{
    static class JobsTest
    {
        public static void Run(IJobBroker jobBroker, IServicesContainer container,  ILogger logger)
        {
            container.Register<Test1JobExecutor>(ServiceLifetime.Singleton);
            container.Register<Test2JobExecutor>(ServiceLifetime.Singleton);
            container.Register<Test3JobExecutor>(ServiceLifetime.Singleton);

            var job1 = new JobDefinition<Test1JobExecutor>()
            {
                Name = "Job 1",
                Recoverable = true
            };
            var job2 = new JobDefinition<Test2JobExecutor, string>()
            {
                Name = "Job 2",
                Recoverable = true
            };
            var job3 = new JobDefinition<Test3JobExecutor>()
            {
                Name = "Job 3",
                Recoverable = true,
                Repeatable = true
            };

            var job1Token = jobBroker.Run(job1);
            var job2Token = jobBroker.Run(job2, "Some state");
            var job3Token = jobBroker.Run(job3);

            while (job3Token.StartAttempts < 5)
            {
                Thread.Sleep(1000);
            }
            job3Token.Breaker.Cancel();
            job2Token.Breaker.Cancel();

            Console.ReadLine();
        }
    }

    public class Test1JobExecutor : IJobExecutor
    {
        private readonly ILogger _logger;
        private int _counter;

        public Test1JobExecutor(ILogger logger)
        {
            _logger = logger;
        }

        public JobExecutionResult Execute(JobExecutionContext context)
        {
            _logger.Info("Job1", "Execution", $"Start: Attempts=#{context.Token.StartAttempts}, Recovery=#{context.Token.RecoveryAttempts}, IsRepeat=#{context.IsRepeat}, IsRecovery=#{context.IsRecovery}");
            Thread.Sleep(10000);
            _logger.Info("Job1", "Execution", $"Finish: Attempts=#{context.Token.StartAttempts}, Recovery=#{context.Token.RecoveryAttempts}");
            return JobExecutionResult.Canceled;
        }
    }
    public class Test2JobExecutor : IJobExecutor<string>
    {
        private readonly ILogger _logger;
        private int _counter;

        public Test2JobExecutor(ILogger logger)
        {
            _logger = logger;
        }
        public JobExecutionResult Execute(JobExecutionContext context, string state)
        {
            _logger.Info("Job2", "Execution", $"Start: Attempts=#{context.Token.StartAttempts}, Recovery=#{context.Token.RecoveryAttempts}, IsRepeat=#{context.IsRepeat}, IsRecovery=#{context.IsRecovery}");
            Thread.Sleep(10000);
            _logger.Info("Job2", "Execution", $"Finish: Attempts=#{context.Token.StartAttempts}, Recovery=#{context.Token.RecoveryAttempts}");
            return JobExecutionResult.Failure;
        }
    }

    public class Test3JobExecutor : IJobExecutor
    {
        private readonly ILogger _logger;
        private int _counter;

        public Test3JobExecutor(ILogger logger)
        {
            _logger = logger;
        }
        
        public JobExecutionResult Execute(JobExecutionContext context)
        {
            _logger.Info("Job3", "Execution", $"Start: Attempts=#{context.Token.StartAttempts}, Recovery=#{context.Token.RecoveryAttempts}, IsRepeat=#{context.IsRepeat}, IsRecovery=#{context.IsRecovery}");
            Thread.Sleep(10000);
            _logger.Info("Job3", "Execution", $"Finish: Attempts=#{context.Token.StartAttempts}, Recovery=#{context.Token.RecoveryAttempts}");
            return JobExecutionResult.Completed;
        }
    }
}
