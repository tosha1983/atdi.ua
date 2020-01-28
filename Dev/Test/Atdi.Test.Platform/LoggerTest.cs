using Atdi.Platform.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Atdi.Test.Platform
{
    internal static class LoggerTest
    {
        public static void Run(ILogger logger)
        {
	  //      var dataCount = 1000_000;
	  //      var data = GetTestData(dataCount);

	  //      for (int i = 0; i < 10; i++)
	  //      {
			//	StartTest(Save1, data, @"C:\Temp\Logger\save_1.log", "Save1");
			//	StartTest(Save2, data, @"C:\Temp\Logger\save_2.log", "Save2");
			//	StartTest(Save3, data, @"C:\Temp\Logger\save_3.log", "Save3");
			//	StartTest(Save4, data, @"C:\Temp\Logger\save_4.log", "Save4");
			//	StartTest(Save5, data, @"C:\Temp\Logger\save_5.log", "Save5");
			//}
			

			//return;

			const int count =  200_000;

            var thread1 = CreateThread(logger, count, 1);
			var thread2 = CreateThread(logger, count, 2);
			var thread3 = CreateThread(logger, count, 3);
			var thread4 = CreateThread(logger, count, 4);

			var thread5 = CreateThread(logger, count, 5);
			var thread6 = CreateThread(logger, count, 6);
			var thread7 = CreateThread(logger, count, 7);
			var thread8 = CreateThread(logger, count, 8);

			thread1.Start();
			thread2.Start();
			thread3.Start();
			thread4.Start();
			thread5.Start();
			thread6.Start();
			thread7.Start();
			thread8.Start();
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

                //Thread.Sleep(1);

                logger.Verbouse((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Verbose - Test iteration #{i}");

                //Thread.Sleep(1);

                logger.Debug((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Debug - Test iteration #{i}");

                //Thread.Sleep(1);

                logger.Warning((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Warning - Test iteration #{i}");

                //Thread.Sleep(1);

                logger.Error((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Error - Test iteration #{i}");

                //Thread.Sleep(1);

                using (logger.StartTrace((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Trace - Test iteration #{i}"))
                {
                   // Thread.Sleep(5);
                }

                //Thread.Sleep(1);

                logger.Exception((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Exception - Test iteration #{i}", new InvalidCastException());

                //Thread.Sleep(1);

                logger.Critical((EventContext)"LoggerTest", (EventCategory)string.Intern($"Testing #{number}"), $"Critical - Test iteration #{i}", new InvalidCastException());

                if (++counter == 1000)
                {
                    counter = 0;
                    Console.WriteLine($"Work thread ({i}): Name = '{current.Name}', ID = #{current.ManagedThreadId} ");
                }
            }

            Console.WriteLine($"Finish thread: Name = '{current.Name}', ID = #{current.ManagedThreadId} ");
        }



        private static string[] GetTestData(int count)
        {
			var data = new string[count];
			var r = new Random();
			for (int i = 0; i < count; i++)
			{
				var c = r.Next(30, 250);
				var row = new char[c];
				for (int j = 0; j < c; j++)
				{
					row[j] = Convert.ToChar(r.Next(60, 90));
				}

				data[i] = new string(row);
			}

			return data;
        }

		private static void Save1(string[] events, string filePath)
		{
			var buffer = new StringBuilder();
			for (int i = 0; i < events.Length; i++)
			{
				var @event = events[i];
				for (int j = 0; j < @event.Length; j++)
				{
					buffer.Append(@event[j]);
				}
				buffer.AppendLine();
			}

			File.AppendAllText(filePath, buffer.ToString(), Encoding.UTF8);
		}

		private static void Save2(string[] events, string filePath)
		{
			using (var writer = (TextWriter) new StreamWriter(filePath, true, Encoding.UTF8))
			{
				for (int i = 0; i < events.Length; i++)
				{
					var @event = events[i];
					for (int j = 0; j < @event.Length; j++)
					{
						writer.Write(@event[j]);
					}
					writer.WriteLine();
				}
			}
		}

		private static void Save3(string[] events, string filePath)
		{
			char c = 'A';
			using (var writer = (TextWriter)new StreamWriter(filePath, true, Encoding.UTF8))
			{
				for (int i = 0; i < events.Length; i++)
				{
					var @event = events[i];
					for (int j = 0; j < @event.Length; j++)
					{
						c = @event[j];
					}
					writer.WriteLine(@event);
				}
			}

			filePath += Convert.ToString(c);
		}

		private static void Save4(string[] events, string filePath)
		{
			using (var writer = (TextWriter)new StreamWriter(filePath, true, Encoding.UTF8, 65535))
			{
				for (int i = 0; i < events.Length; i++)
				{
					var @event = events[i];
					for (int j = 0; j < @event.Length; j++)
					{
						writer.Write(@event[j]);
					}
					writer.WriteLine();
				}
			}
		}

		private static void Save5(string[] events, string filePath)
		{
			using (var writer = (TextWriter)new StreamWriter(filePath, true, Encoding.UTF8, 5000000))
			{
				for (int i = 0; i < events.Length; i++)
				{
					var @event = events[i];
					for (int j = 0; j < @event.Length; j++)
					{
						writer.Write(@event[j]);
					}
					writer.WriteLine();
				}
			}
		}

		private static void StartTest(Action<string[], string> testAction, string[] events, string filePath, string name)
		{
			//Console.WriteLine("TEST: Start - " + name);
			var timer = System.Diagnostics.Stopwatch.StartNew();
			testAction(events, filePath);
			timer.Stop();
			Console.WriteLine($"TEST: {name} - " + timer.Elapsed.TotalMilliseconds);
		}
	}
}
