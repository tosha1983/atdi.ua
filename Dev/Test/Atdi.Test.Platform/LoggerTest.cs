using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.Test.Platform
{
    internal static class LoggerTest
    {
        public static void Run(ILogger logger)
        {
            logger.Info((EventContext)"LoggerTest", (EventCategory)"Running", "Start test");
            const int count = 20000;

            Task.Run(() =>
            {
                TestIteration(logger, count, 1);
            });
            Task.Run(() =>
            {
                TestIteration(logger, count, 2);
            });
            Task.Run(() =>
            {
                TestIteration(logger, count, 3);
            });
        }

        private static void TestIteration(ILogger logger, int count, int number)
        {
            for (var i = 0; i < count; i++)
            {
                logger.Info((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Info - Test iteration #{i}");

                Thread.Sleep(1);

                logger.Verbouse((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Verbose - Test iteration #{i}");

                Thread.Sleep(1);

                logger.Debug((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Debug - Test iteration #{i}");

                Thread.Sleep(1);

                logger.Warning((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Warning - Test iteration #{i}");

                Thread.Sleep(1);

                logger.Error((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Error - Test iteration #{i}");

                Thread.Sleep(1);

                using (logger.StartTrace((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Trace - Test iteration #{i}"))
                {
                    Thread.Sleep(5);
                }

                Thread.Sleep(1);

                logger.Exception((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Error - Test iteration #{i}", new InvalidCastException());

            }
        }
    }
}
