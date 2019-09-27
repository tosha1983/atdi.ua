using Atdi.Platform.Logging;
using System;
using System.Threading;

namespace Atdi.Test.Platform
{
    internal static class LoggerTest
    {
        public static void Run(ILogger logger)
        {
            const int count = 20000;

            var thread1 = CreateThread(logger, count, 1);
            var thread2 = CreateThread(logger, count, 2);
            var thread3 = CreateThread(logger, count, 3);

            thread1.Start();
            thread2.Start();
            thread3.Start();
        }

        private static Thread CreateThread(ILogger logger, int count, int number)
        {
            var result = new Thread(() => TestIteration(logger, count, number))
            {
                Name = $"Atdi.Test.Platform.#{number} - {count}"
            };

            Console.WriteLine($"Create thread: Name = '{result.Name}', ID = #{result.ManagedThreadId} ");
            return result;
        }

        private static void TestIteration(ILogger logger, int count, int number)
        {
            var current = Thread.CurrentThread;
            Console.WriteLine($"Run thread: Name = '{current.Name}', ID = #{current.ManagedThreadId} ");
            var counter = 0;
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

                logger.Exception((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Exception - Test iteration #{i}", new InvalidCastException());

                Thread.Sleep(1);

                logger.Critical((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Critical - Test iteration #{i}", new InvalidCastException());

                if (++counter == 1000)
                {
                    counter = 0;
                    Console.WriteLine($"Work thread ({i}): Name = '{current.Name}', ID = #{current.ManagedThreadId} ");
                }
            }

            Console.WriteLine($"Finish thread: Name = '{current.Name}', ID = #{current.ManagedThreadId} ");
        }
    }
}
