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
            Console.WriteLine("START GSM");
            TSMxD += SetGSM;
            //Console.ReadLine();
            //TSMxD += StopGSM;
            Console.WriteLine("START UMTS");
            TSMxD += SetUMTS;
            Console.WriteLine("START LTE");
            TSMxD += SetLTE;
            Console.WriteLine("START CDMAEVDO");
            TSMxD += SetCDMAEVDO;
            Console.ReadLine();
            Console.WriteLine("stop");
            TSMxD += TSMxDisconnect;
            Console.ReadLine();


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
                IWorkScheduler _workScheduler = new Atdi.AppUnits.Sdrn.DeviceServer.Processing.TestWorkScheduler(logger);
                TSMxadapter = new ADP.RSTSMx.Adapter(adapterConfig, logger, TimeService, _workScheduler);




                //SHIQ.ANAdapter = ANadapter;
                TSMxadapter.Connect(adapterHost);


            }
            finally
            {
                TSMxD -= TSMxConnect;
            }
        }
        static bool ResultOnlyWithGCID = true;

        static DummyExecutionContext GCMcontext;
        private static void SetGSM()
        {
            try
            {
                // send command
                GCMcontext = new DummyExecutionContext(logger);

                //List<decimal> freq1 = new List<decimal>() { };
                //for (decimal i = 918200000; i <= 959800000; i += 200000)
                //{ freq1.Add(i); }
                //for (decimal i = 1805200000; i <= 1879800000; i += 200000)
                //{ freq1.Add(i); }


                var command = new CMD.MesureSystemInfoCommand();
                //command.Parameter.Freqs_Hz = freq1.ToArray();
                command.Parameter.Bands = new string[] { CMD.Parameters.MesureSystemInfo.GSMBands.P_GSM900.ToString(), CMD.Parameters.MesureSystemInfo.GSMBands.GSM1800.ToString() };
                command.Parameter.Standart = "GSM";
                command.Parameter.FreqType = CMD.Parameters.MesureSystemInfo.FreqType.New;
                command.Parameter.DelayToSendResult_s = 10;
                command.Parameter.ResultOnlyWithGCID = ResultOnlyWithGCID;
                command.Parameter.PeriodicResult = true;
                TSMxadapter.MesureSystemInfoHandler(command, GCMcontext);
            }
            finally
            {
                TSMxD -= SetGSM;
            }
        }
        private static void StopGSM()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                source.Cancel();
                GCMcontext.Token = source.Token;
            }
            finally
            {
                TSMxD -= StopGSM;
            }
        }
        private static void SetUMTS()
        {
            try
            {
                // send command
                var context = new DummyExecutionContext(logger);

                List<decimal> freq1 = new List<decimal>()
                {
                    2112800000,
                    2117600000,
                    2122400000,
                    2127400000,
                    2132400000,
                    2137400000,
                    2142400000,
                    2147400000,
                    2152400000,
                    2157400000,
                    2162400000,
                    2167200000,
                };
                


                var command = new CMD.MesureSystemInfoCommand();
                command.Parameter.Freqs_Hz = freq1.ToArray();
                command.Parameter.Standart = "UMTS";
                command.Parameter.FreqType = CMD.Parameters.MesureSystemInfo.FreqType.New;
                command.Parameter.DelayToSendResult_s = 30;
                command.Parameter.ResultOnlyWithGCID = ResultOnlyWithGCID;
                TSMxadapter.MesureSystemInfoHandler(command, context);
            }
            finally
            {
                TSMxD -= SetUMTS;
            }
        }
        private static void SetLTE()
        {
            try
            {
                // send command
                var context = new DummyExecutionContext(logger);

                List<decimal> freq1 = new List<decimal>()
                {
                    1815000000,
                    1837500000,
                    1855000000,
                    2635000000,
                    2647500000,
                    2660000000,
                    2687500000,
                };



                var command = new CMD.MesureSystemInfoCommand();
                command.Parameter.Freqs_Hz = freq1.ToArray();
                command.Parameter.Standart = "LTE";
                command.Parameter.FreqType = CMD.Parameters.MesureSystemInfo.FreqType.New;
                command.Parameter.DelayToSendResult_s = 30;
                command.Parameter.ResultOnlyWithGCID = ResultOnlyWithGCID;
                TSMxadapter.MesureSystemInfoHandler(command, context);
            }
            finally
            {
                TSMxD -= SetLTE;
            }
        }
        private static void SetCDMAEVDO()
        {
            try
            {
                // send command
                var context = new DummyExecutionContext(logger);

                List<decimal> freq1 = new List<decimal>()
                {
                    873480000,
                    874740000,
                    876000000,
                    877260000,
                    878520000,
                    879780000,
                    881040000,
                    882300000,
                    883560000,
                    884820000,
                    886080000,
                    887340000,
                    1981250000,
                    1982500000,
                    1983750000,
                    1985000000,
                };
                List<bool> freqType = new List<bool>()
                {
                    true,
                    true,
                    true,
                    true,
                    false,
                    true,
                    true,
                    true,
                    true,
                    true,
                    false,
                    true,
                    true,
                    true,
                    true,
                    true,
                };


                var command = new CMD.MesureSystemInfoCommand();
                command.Parameter.Freqs_Hz = freq1.ToArray();
                command.Parameter.CDMAEVDOFreqTypes = freqType.ToArray();
                command.Parameter.Standart = "CDMAEVDO";
                command.Parameter.FreqType = CMD.Parameters.MesureSystemInfo.FreqType.New;
                command.Parameter.DelayToSendResult_s = 30;
                command.Parameter.ResultOnlyWithGCID = ResultOnlyWithGCID;

                TSMxadapter.MesureSystemInfoHandler(command, context);
            }
            finally
            {
                TSMxD -= SetCDMAEVDO;
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
