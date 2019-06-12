using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AC = Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using ADP = Atdi.AppUnits.Sdrn.DeviceServer.Adapters;
using CMD = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.UnitTest.Sdrn.DeviceServer;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace Atdi.Test.Sdrn.DeviceServer.Adapters.JacksConsole
{
    class Program
    {
        // подготовка тестового окружения
        static DummyTimeService TimeService;
        static ConsoleLogger logger;
        static DummyAdapterHost adapterHost;

        private delegate void AnyDelegate();
        static ADP.RSTSMx.Adapter TSMxadapter;
        private static Thread TSMxThread;
        private static AnyDelegate TSMxD;
        static void Main(string[] args)
        {
            logger = new ConsoleLogger();
            adapterHost = new DummyAdapterHost(logger);
            TimeService = new DummyTimeService();


            TSMxThread = new Thread(TSMxWorks);
            TSMxThread.Name = "TSMxThread";
            TSMxThread.IsBackground = true;
            TSMxThread.Start();
            TSMxD += TSMxConnect;

            Console.ReadLine();
            TSMxD += TSMxDisconnect;
            Console.ReadLine();
            Console.WriteLine("stop");

            Console.ReadLine();

        }
        private static void TSMxWorks()
        {
            TimeSpan ts = new TimeSpan(10000);
            bool Cycle = true;
            while (Cycle)
            {
                if (TSMxD != null) { TSMxD(); }
                Thread.Sleep(ts);
            }
        }
        private static void TSMxConnect()
        {
            try
            {
                var adapterConfig = new ADP.RSTSMx.AdapterConfig()
                {
                    DeviceType = 2,
                    IPAddress = "192.168.2.50",
                    RSViComPath = @"c:\RuS\RS-ViCom-Pro-16.25.0.743"
                };
                TSMxadapter = new ADP.RSTSMx.Adapter(adapterConfig, logger, TimeService);




                //SHIQ.ANAdapter = ANadapter;
                TSMxadapter.Connect(adapterHost);


            }
            finally
            {
                TSMxD -= TSMxConnect;
            }
        }

        private static void TSMxDisconnect()
        {
            try
            {
                TSMxadapter.Disconnect();
            }
            finally
            {
                TSMxThread.Abort();
                TSMxD -= TSMxDisconnect;
            }
        }
    }
    public class DummyTimeService : ITimeService
    {
        
        private readonly ITimeStamp _timeStamp;
        private long _timeCorrection;

        public DummyTimeService()
        {
            this._timeStamp = new TimeStamp();
        }

        public ITimeStamp TimeStamp => this._timeStamp;

        public long TimeCorrection
        {
            get
            {
                return Interlocked.Read(ref this._timeCorrection);
            }
            set
            {
                Interlocked.Exchange(ref this._timeCorrection, value);
            }
        }

        public DateTime GetGnssTime()
        {
            var date = new DateTime(AC.WinAPITime.GetTimeStamp() + TimeCorrection, DateTimeKind.Utc);
            return date.ToLocalTime();
        }

        public DateTime GetGnssUtcTime()
        {
            return new DateTime(AC.WinAPITime.GetTimeStamp() + TimeCorrection, DateTimeKind.Utc);
        }
    }

    class TimeStamp : ITimeStamp
    {
        public long Milliseconds => AC.TimeStamp.Milliseconds;

        public long Value => AC.TimeStamp.Value;

        public long Ticks => AC.TimeStamp.Ticks;

        public bool HitMilliseconds(long startStampMilliseconds, long timeoutMilliseconds)
        {
            return AC.TimeStamp.HitTimeout(startStampMilliseconds, timeoutMilliseconds);
        }

        public bool HitMilliseconds(long startStampMilliseconds, long timeoutMilliseconds, out long lateness)
        {
            return AC.TimeStamp.HitTimeout(startStampMilliseconds, timeoutMilliseconds, out lateness);
        }
    }
}
