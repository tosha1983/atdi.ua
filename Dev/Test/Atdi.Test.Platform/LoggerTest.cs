using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Platform
{
    static class LoggerTest
    {
        public static void Run(ILogger logger)
        {
            logger.Info((EventContext)"LoggerTest", (EventCategory)"Running", "Start test");
            var count = 20000;

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

        static private void TestIteration(ILogger logger, int count, int number)
        {
            for (int i = 0; i < 20000; i++)
            {
                logger.Info((EventContext)"LoggerTest", (EventCategory)$"Testing #{number}", $"Test iteration #{i}");
            }
        }
    }
}
