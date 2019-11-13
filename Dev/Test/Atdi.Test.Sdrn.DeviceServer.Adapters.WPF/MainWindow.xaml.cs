using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AC = Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
using ADP = Atdi.AppUnits.Sdrn.DeviceServer.Adapters;
using CMD = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.UnitTest.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace Atdi.Test.Sdrn.DeviceServer.Adapters.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // подготовка тестового окружения
        DummyTimeService TimeService;
        ConsoleLogger logger;
        DummyAdapterHost adapterHost;
        //ADP.SignalHound.Adapter adapter;
        ADP.SpectrumAnalyzer.Adapter ANadapter;
        ADP.SignalHound.Adapter SHadapter;

        ADP.GPS.GPSAdapter GPSadapter;
        //GNSSNMEA gnss;

        private delegate void AnyDelegate();
        private Thread TimeThread;
        private Thread ANThread;
        private AnyDelegate AND;
        private Thread SHThread;
        private AnyDelegate SHD;

        private Thread GPSThread;
        private AnyDelegate GPSD;

        public MainWindow()
        {
            InitializeComponent();

            logger = new ConsoleLogger();
            adapterHost = new DummyAdapterHost(logger);
            TimeService = new DummyTimeService();
            //gnss = new GNSSNMEA(TimeService);
            //gnss.ConnectToGPS();
            // конфигурация
            //var adapterConfig = new ADP.SignalHound.AdapterConfig()
            //{
            //    SerialNumber = "16319373"
            //};
            //adapter = new ADP.SignalHound.Adapter(adapterConfig, logger, TimeService);
            //DS.Adapter = adapter;


            //adapter = new ADP.SignalHound.Adapter(adapterConfig, logger, TimeService);
            //ANIQ.ANAdapter = ANadapter;
            //SHIQ.ANAdapter = ANadapter;
        }
        private void StartTime_Click(object sender, RoutedEventArgs e)
        {
            ////////////TimeThread = new Thread(GetGPSData);
            ////////////TimeThread.Name = "GPSThread";
            ////////////TimeThread.IsBackground = true;
            ////////////TimeThread.Start();

            //ANThread = new Thread(ANWorks);
            //ANThread.Name = "ANThread";
            //ANThread.IsBackground = true;
            //ANThread.Start();
            //AND += ANConnect;

            SHThread = new Thread(SHWorks);
            SHThread.Name = "SHThread";
            SHThread.IsBackground = true;
            SHThread.Start();
            SHD += SHConnect;

            GPSThread = new Thread(GPSWorks);
            GPSThread.Name = "GPSThread";
            GPSThread.IsBackground = true;
            GPSThread.Start();
            GPSD += GPSConnect;

        }
        //long NextSecond = 0;
        private void GetGPSData()
        {
            //while (true)
            //{
            //    if ((((double)gnss.LocalTime.Ticks) / 10000000 - (double)(gnss.LocalTime.Ticks / 10000000)) < 0.00001)
            //    {
            //        //Debug.WriteLine(gnss.LocalTime.Ticks);
            //        //Debug.WriteLine("GNSS " + new DateTime(gnss.LocalTime.Ticks).ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
            //        //NextSecond = gnss.LocalTime.Ticks + 10000000;
            //        //Debug.WriteLine("Time " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
            //        //Debug.WriteLine("Next " + new DateTime(NextSecond).ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
            //        if (add)
            //        {
            //            SHD += SHGetIQ;
            //            AND += ANGetIQ;
            //            add = false;
            //        }
            //    }
            Thread.Sleep(new TimeSpan(10000));
            //}

        }


        private void ANWorks()
        {
            TimeSpan ts = new TimeSpan(10000);
            bool Cycle = true;
            while (Cycle)
            {
                if (AND != null) { AND(); }
                Thread.Sleep(ts);
            }
        }
        private void ANConnect()
        {
            try
            {
                var adapterConfig = new ADP.SpectrumAnalyzer.AdapterConfig()
                {
                    SerialNumber = "101396",
                    IPAddress = "192.168.2.110",
                    DisplayUpdate = true,
                    OnlyAutoSweepTime = true,
                    Optimization = 2,
                    ConnectionMode = 1,
                };

                ANadapter = new ADP.SpectrumAnalyzer.Adapter(adapterConfig, logger, TimeService);
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    DS_AN.ANAdapter = ANadapter;
                    ANIQ.ANAdapter = ANadapter;
                });//ANIQ.ANAdapter = ANadapter;
                ANadapter.Connect(adapterHost);
            }
            finally
            {
                AND -= ANConnect;
            }
        }

        private void ANDisconnect()
        {
            try
            {
                ANadapter.Disconnect();

            }
            finally
            {
                ANThread.Abort();
                AND -= ANDisconnect;
            }
        }

        private void SHWorks()
        {
            TimeSpan ts = new TimeSpan(10000);
            bool Cycle = true;
            while (Cycle)
            {
                if (SHD != null) { SHD(); }
                Thread.Sleep(ts);
            }
        }
        private void SHConnect()
        {
            try
            {
                var adapterConfig = new ADP.SignalHound.AdapterConfig()
                {
                    SerialNumber = "16319373",//"18250087",// "16319373",
                    GPSPPSConnected = true,
                    Reference10MHzConnected = false,
                    //SyncCPUtoGPS = true,
                    //GPSPortBaudRate = 38400,
                    //GPSPortNumber = 1,

                };

                SHadapter = new ADP.SignalHound.Adapter(adapterConfig, logger, TimeService);
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    DS_SH.SHAdapter = SHadapter;
                    SHIQ.SHAdapter = SHadapter;
                });
                //SHIQ.ANAdapter = ANadapter;
                SHadapter.Connect(adapterHost);
            }
            finally
            {
                SHD -= SHConnect;
            }
        }

        private void SHDisconnect()
        {
            try
            {
                SHadapter.Disconnect();
            }
            finally
            {
                SHD -= SHDisconnect;
                SHThread.Abort();
            }
        }


        private void GPSWorks()
        {
            TimeSpan ts = new TimeSpan(10000);
            bool Cycle = true;
            while (Cycle)
            {
                if (GPSD != null) { GPSD(); }
                Thread.Sleep(ts);
            }
        }
        private void GPSConnect()
        {
            try
            {
                var adapterConfig = new ADP.GPS.ConfigGPS()
                {
                    PortName = "COM21",
                    PortBaudRate = "baudRate9600",
                    PortDataBits = "dataBits8",
                    PortHandshake = "None",
                    PortParity = "None",
                    PortStopBits = "One",
                    EnabledPPS = true
                };
                IWorkScheduler _workScheduler = new Atdi.AppUnits.Sdrn.DeviceServer.Processing.TestWorkScheduler(logger);
                GPSadapter = new ADP.GPS.GPSAdapter(adapterConfig, _workScheduler, TimeService, logger);




                //SHIQ.ANAdapter = ANadapter;
                GPSadapter.Connect(adapterHost);

                CMD.Parameters.GpsParameter par = new CMD.Parameters.GpsParameter();
                par.GpsMode = CMD.Parameters.GpsMode.Start;
                CMD.GpsCommand command = new CMD.GpsCommand(par);
                var context = new DummyExecutionContextMy(logger);
                GPSadapter.GPSCommandHandler(command, context);
            }
            finally
            {
                GPSD -= GPSConnect;
            }
        }

        private void GPSDisconnect()
        {
            try
            {
                SHadapter.Disconnect();
            }
            finally
            {
                GPSThread.Abort();
                GPSD -= GPSDisconnect;
            }
        }

        private void TSMxWorks()
        {
            //TimeSpan ts = new TimeSpan(10000);
            //bool Cycle = true;
            //while (Cycle)
            //{
            //    if (TSMxD != null) { TSMxD(); }
            //    Thread.Sleep(ts);
            //}
        }
        private void TSMxConnect()
        {
            //try
            //{
            //    var adapterConfig = new ADP.RSTSMx.AdapterConfig()
            //    {
            //        DeviceType = 2,
            //        IPAddress = "192.168.2.50",
            //        RSViComPath = @"c:\RuS\RS-ViCom-Pro-16.25.0.743"
            //    };
            //    //TSMxadapter = new ADP.RSTSMx.Adapter(adapterConfig, logger, TimeService);




            //    //SHIQ.ANAdapter = ANadapter;
            //    TSMxadapter.Connect(adapterHost);


            //}
            //finally
            //{
            //    TSMxD -= TSMxConnect;
            //}
        }

        private void TSMxDisconnect()
        {
            //try
            //{
            //    TSMxadapter.Disconnect();
            //}
            //finally
            //{
            //    TSMxThread.Abort();
            //    TSMxD -= TSMxDisconnect;
            //}
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            ANadapter.Connect(adapterHost);
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            //gnss.CloseConGPS();
            AND += ANDisconnect;
            SHD += SHDisconnect;
        }

        private void GetTraceAN()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = -1;
                command.Parameter.FreqStart_Hz = 1805000000;
                command.Parameter.FreqStop_Hz = 1880000000;
                command.Parameter.PreAmp_dB = -1;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -1;
                command.Parameter.SweepTime_s = 0.003;
                command.Parameter.TraceCount = 1;
                command.Parameter.TracePoint = 3750;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                ANadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AND -= GetTraceAN;
            }
        }
        private void GetTraceAN2()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 104.750m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 105.250m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 30;
                command.Parameter.RBW_Hz = 100;
                command.Parameter.VBW_Hz = 100;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 10;
                command.Parameter.TracePoint = -1;
                command.Parameter.TraceType = CMD.Parameters.TraceType.MaxHold;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                ANadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AND -= GetTraceAN2;
            }
        }
        private void GetTraceAN3()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 104.750m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 105.250m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = 5000;
                command.Parameter.VBW_Hz = 5000;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 1600;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                ANadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AND -= GetTraceAN3;
            }
        }
        private void GetTraceAN1800()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 1800 * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 1900 * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = 30000;
                command.Parameter.VBW_Hz = 30000;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 5100;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                ANadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AND -= GetTraceAN1800;
            }
        }
        private void GetTraceAN1800AVG()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 1800 * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 1900 * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = 30000;
                command.Parameter.VBW_Hz = 30000;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 10000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.Average;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                ANadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AND -= GetTraceAN1800AVG;
            }
        }
        private void GetTraceAN9352()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 934.95m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 935.45m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = 1000;
                command.Parameter.VBW_Hz = 1000;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 2000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.Average;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                ANadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AND -= GetTraceAN9352;
            }
        }
        private void GetTraceAN1350()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 1340m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 1360m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 0;
                double rbw = 1;
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    rbw = double.Parse(RBW.Text);
                });
                command.Parameter.RBW_Hz = rbw;
                command.Parameter.VBW_Hz = command.Parameter.RBW_Hz;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 4000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.Average;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.RMS;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                ANadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AND -= GetTraceAN1350;
            }
        }

        private void GetTraceSH()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = -1;
                command.Parameter.FreqStart_Hz = 1805000000;
                command.Parameter.FreqStop_Hz = 1880000000;
                command.Parameter.PreAmp_dB = -1;
                command.Parameter.RBW_Hz = -2;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -1;
                command.Parameter.SweepTime_s = 0.003;
                command.Parameter.TraceCount = 1;
                int point = 1;
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    point = int.Parse(points.Text);
                });
                command.Parameter.TracePoint = point;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                SHadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetTraceSH;
            }
        }
        private void GetTraceSH2()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 20;
                command.Parameter.FreqStart_Hz = 2600m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 2700m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 10;
                command.Parameter.RBW_Hz = 100;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 2;
                command.Parameter.TracePoint = -1;
                command.Parameter.TraceType = CMD.Parameters.TraceType.MaxHold;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                SHadapter.MesureTraceCommandHandler(command, context);


                //// send command
                //var context = new DummyExecutionContextMy(logger);
                //var command = new CMD.MesureTraceCommand();
                //command.Parameter.Att_dB = -1;
                //command.Parameter.FreqStart_Hz = 421589781.033924m;//100000000;
                //command.Parameter.FreqStop_Hz = 421609741.164232m;// 110000000;
                //command.Parameter.PreAmp_dB = -1;
                //command.Parameter.RBW_Hz = -1;
                //command.Parameter.VBW_Hz = -1;
                //command.Parameter.RefLevel_dBm = 1000000000;// - 40;
                //command.Parameter.SweepTime_s = 0.001;// 0.00001;
                //command.Parameter.TraceCount = 10;
                //command.Parameter.TracePoint = 60000;// 1001;
                //command.Parameter.TraceType = CMD.Parameters.TraceType.MaxHold;
                //command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                //command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                //SHadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetTraceSH2;
            }
        }
        private void GetTraceSH3()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 10;
                command.Parameter.FreqStart_Hz = 104.750m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 105.250m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 30;
                command.Parameter.RBW_Hz = 5000;
                command.Parameter.VBW_Hz = 5000;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 1600;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                SHadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetTraceSH3;
            }
        }
        private void GetTraceSH1800()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 10;
                command.Parameter.FreqStart_Hz = 1800 * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 1900 * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 30;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 3300;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                SHadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetTraceSH1800;
            }
        }
        private void GetTraceSH1800AVG()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 20;
                command.Parameter.FreqStart_Hz = 1800 * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 1900 * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 30;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 5500;
                command.Parameter.TraceType = CMD.Parameters.TraceType.Average;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                SHadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetTraceSH1800AVG;
            }
        }

        private void GetTraceSH9352()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 20;
                command.Parameter.FreqStart_Hz = 934.95m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 935.45m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 4000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.Average;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                SHadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetTraceSH9352;
            }
        }
        private void GetTraceSH1350()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 1340m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 1360m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 0;
                double rbw = 1;
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    rbw = double.Parse(RBW.Text);
                });
                command.Parameter.RBW_Hz = rbw;
                command.Parameter.VBW_Hz = command.Parameter.RBW_Hz;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 4000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.Average;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                SHadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetTraceSH1350;
            }
        }

        long UTCOffset = 621355968000000000;
        private void GetIQAN()
        {
            try
            {
                //до следующей секунды
                //long time = TimeService.GetGnssTime().Ticks;
                //long ToNextSecond = (time / 10000000) * 10000000 - time + 10000000;
                //long NextSecond = time + ToNextSecond;
                //long NextSecond2 = time + ToNextSecond - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //Debug.WriteLine("\r\n" + new TimeSpan(ToNextSecond).ToString() + " Next");
                //Debug.WriteLine("\r\n" + new TimeSpan(TimeService.GetGnssTime().Ticks).ToString() + " NOW");
                //Debug.WriteLine("\r\n" + new TimeSpan(NextSecond).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(NextSecond2).ToString() + " Set Param");


                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                //command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 0.6;
                command.Parameter.IQBlockDuration_s = 0.5;
                command.Parameter.IQReceivTime_s = 0.6;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = true;

                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                ANadapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                AND -= GetIQAN;
            }
        }
        private void GetIQSH()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                //command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.BitRate_MBs = 0.9;
                //command.Parameter.Att_dB = 10;
                //command.Parameter.PreAmp_dB = 10;
                command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.BitRate_MBs = 0.1;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 30;

                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.IQBlockDuration_s = 0.5;
                command.Parameter.IQReceivTime_s = 0.6;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = true;
                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //long tttt = AC.WinAPITime.GetTimeStamp();// TimeService.GetGnssUtcTime().Ticks; 
                //command.Parameter.TimeStart = 1000000 + tttt - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////long offset = DateTime.Now.Ticks - NextSecond;
                ////////command.Parameter.TimeStart = DateTime.UtcNow.Ticks + offset - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                //Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(TimeService.GetGnssUtcTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                SHadapter.MesureIQStreamCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetIQSH;
            }
        }

        private void GetIQAN2()
        {
            try
            {
                //до следующей секунды
                //long time = TimeService.GetGnssTime().Ticks;
                //long ToNextSecond = (time / 10000000) * 10000000 - time + 10000000;
                //long NextSecond = time + ToNextSecond;
                //long NextSecond2 = time + ToNextSecond - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //Debug.WriteLine("\r\n" + new TimeSpan(ToNextSecond).ToString() + " Next");
                //Debug.WriteLine("\r\n" + new TimeSpan(TimeService.GetGnssTime().Ticks).ToString() + " NOW");
                //Debug.WriteLine("\r\n" + new TimeSpan(NextSecond).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(NextSecond2).ToString() + " Set Param");


                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 0.6;
                command.Parameter.IQBlockDuration_s = 0.9;
                command.Parameter.IQReceivTime_s = 1.0;
                command.Parameter.MandatoryPPS = false;
                command.Parameter.MandatorySignal = true;

                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                ANadapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                AND -= GetIQAN2;
            }
        }
        private void GetIQSH2()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.BitRate_MBs = 0.9;
                command.Parameter.Att_dB = 10;
                command.Parameter.PreAmp_dB = 10;
                //command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.BitRate_MBs = 0.1;
                //command.Parameter.Att_dB = 0;
                //command.Parameter.PreAmp_dB = 30;

                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.IQBlockDuration_s = 0.9;
                command.Parameter.IQReceivTime_s = 1.0;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = true;
                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //long tttt = AC.WinAPITime.GetTimeStamp();// TimeService.GetGnssUtcTime().Ticks; 
                //command.Parameter.TimeStart = 1000000 + tttt - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////long offset = DateTime.Now.Ticks - NextSecond;
                ////////command.Parameter.TimeStart = DateTime.UtcNow.Ticks + offset - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                //Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(TimeService.GetGnssUtcTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                SHadapter.MesureIQStreamCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetIQSH2;
            }
        }


        private void GetIQAN3()
        {
            try
            {
                //до следующей секунды
                //long time = TimeService.GetGnssTime().Ticks;
                //long ToNextSecond = (time / 10000000) * 10000000 - time + 10000000;
                //long NextSecond = time + ToNextSecond;
                //long NextSecond2 = time + ToNextSecond - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //Debug.WriteLine("\r\n" + new TimeSpan(ToNextSecond).ToString() + " Next");
                //Debug.WriteLine("\r\n" + new TimeSpan(TimeService.GetGnssTime().Ticks).ToString() + " NOW");
                //Debug.WriteLine("\r\n" + new TimeSpan(NextSecond).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(NextSecond2).ToString() + " Set Param");


                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                //command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.FreqStart_Hz = 422.275m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 422.325m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 0.6;
                command.Parameter.IQBlockDuration_s = 0.6;
                command.Parameter.IQReceivTime_s = 1.0;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = true;

                long offset = (long)(0.03 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                ANadapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                AND -= GetIQAN3;
            }
        }
        private void GetIQSH3()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                //command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.BitRate_MBs = 0.9;
                //command.Parameter.Att_dB = 10;
                //command.Parameter.PreAmp_dB = 10;
                command.Parameter.FreqStart_Hz = 422.275m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 422.325m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.BitRate_MBs = 0.1;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 30;

                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.IQBlockDuration_s = 0.6;
                command.Parameter.IQReceivTime_s = 1.0;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = true;
                long offset = (long)(0.03 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //long tttt = AC.WinAPITime.GetTimeStamp();// TimeService.GetGnssUtcTime().Ticks; 
                //command.Parameter.TimeStart = 1000000 + tttt - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////long offset = DateTime.Now.Ticks - NextSecond;
                ////////command.Parameter.TimeStart = DateTime.UtcNow.Ticks + offset - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                //Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(TimeService.GetGnssUtcTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                SHadapter.MesureIQStreamCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetIQSH3;
            }
        }

        private void GetIQAN4()
        {
            try
            {
                //до следующей секунды
                //long time = TimeService.GetGnssTime().Ticks;
                //long ToNextSecond = (time / 10000000) * 10000000 - time + 10000000;
                //long NextSecond = time + ToNextSecond;
                //long NextSecond2 = time + ToNextSecond - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //Debug.WriteLine("\r\n" + new TimeSpan(ToNextSecond).ToString() + " Next");
                //Debug.WriteLine("\r\n" + new TimeSpan(TimeService.GetGnssTime().Ticks).ToString() + " NOW");
                //Debug.WriteLine("\r\n" + new TimeSpan(NextSecond).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(NextSecond2).ToString() + " Set Param");


                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                command.Parameter.FreqStart_Hz = 1805.1m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 1805.7m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 0.6;
                command.Parameter.IQBlockDuration_s = 0.9;
                command.Parameter.IQReceivTime_s = 1.0;
                command.Parameter.MandatoryPPS = false;
                command.Parameter.MandatorySignal = true;

                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                ANadapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                AND -= GetIQAN4;
            }
        }
        private void GetIQSH4()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                command.Parameter.FreqStart_Hz = 1805.1m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 1805.7m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.BitRate_MBs = 1.2;
                command.Parameter.Att_dB = 10;
                command.Parameter.PreAmp_dB = 10;
                //command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.BitRate_MBs = 0.1;
                //command.Parameter.Att_dB = 0;
                //command.Parameter.PreAmp_dB = 30;

                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.IQBlockDuration_s = 0.9;
                command.Parameter.IQReceivTime_s = 1.0;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = true;
                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //long tttt = AC.WinAPITime.GetTimeStamp();// TimeService.GetGnssUtcTime().Ticks; 
                //command.Parameter.TimeStart = 1000000 + tttt - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////long offset = DateTime.Now.Ticks - NextSecond;
                ////////command.Parameter.TimeStart = DateTime.UtcNow.Ticks + offset - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                //Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(TimeService.GetGnssUtcTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                SHadapter.MesureIQStreamCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetIQSH4;
            }
        }
        private void GetTime_Click(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine("new");
            Debug.WriteLine(TimeService.GetGnssTime());
            Debug.WriteLine(TimeService.GetGnssUtcTime());
        }
        private void GetTrace_Click(object sender, RoutedEventArgs e)
        {
            AND += GetTraceAN;
            SHD += GetTraceSH;
        }
        private void SetMeas2_Click(object sender, RoutedEventArgs e)
        {
            AND += GetTraceAN2;
            SHD += GetTraceSH2;
        }
        private void SetMeas3_Click(object sender, RoutedEventArgs e)
        {
            AND += GetTraceAN3;
            SHD += GetTraceSH3;
        }
        private void SetMeas1800_Click(object sender, RoutedEventArgs e)
        {
            AND += GetTraceAN1800;
            SHD += GetTraceSH1800;
        }
        private void SetMeas1800AVG_Click(object sender, RoutedEventArgs e)
        {
            AND += GetTraceAN1800AVG;
            SHD += GetTraceSH1800AVG;
        }
        private void SetMeas9352AVG_Click(object sender, RoutedEventArgs e)
        {
            AND += GetTraceAN9352;
            SHD += GetTraceSH9352;
        }
        private void SetMeasAVG1350_Click(object sender, RoutedEventArgs e)
        {
            AND += GetTraceAN1350;
            SHD += GetTraceSH1350;
        }
        private void SetMeasIQ_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("=======================================================================================================");
            AND += GetIQAN;
            SHD += GetIQSH;
        }
        private void SetMeasIQ2_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("=======================================================================================================");
            AND += GetIQAN2;
            SHD += GetIQSH2;
        }
        private void SetMeasIQ3_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("=======================================================================================================");
            AND += GetIQAN3;
            SHD += GetIQSH3;
        }
        private void SetMeasIQ4_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("=======================================================================================================");
            AND += GetIQAN4;
            SHD += GetIQSH4;
        }


        private void SetMeas1_Click(object sender, RoutedEventArgs e)
        {
            //// send command
            //var context = new DummyExecutionContextMy(logger);
            //var command = new CMD.MesureTraceCommand();
            //command.Parameter.Att_dB = 10;
            //command.Parameter.FreqStart_Hz = 934.85m * 1000000;
            //command.Parameter.FreqStop_Hz = 935.55m * 1000000;
            //command.Parameter.PreAmp_dB = 30;
            //command.Parameter.RBW_Hz = -1;
            //command.Parameter.VBW_Hz = -1;
            //command.Parameter.RefLevel_dBm = -40;
            //command.Parameter.SweepTime_s = 0.001;
            //command.Parameter.TraceCount = 100000;
            //command.Parameter.TracePoint = int.Parse(points.Text);
            //command.Parameter.TraceType = CMD.Parameters.TraceType.MaxHold;
            //command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
            //command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

            //adapter.MesureTraceCommandHandler(command, context);


        }
        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            int ind = FindMarkerIndOnTrace(ANadapter.FreqArr, (double)ANadapter.FreqCentr);
            double lev = Math.Abs(ANadapter.LevelArr[ind]);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppPath + "\\" + ANadapter.FreqCentr.ToString() + ".txt", false))
            {
                for (int i = 0; i < ANadapter.FreqArr.Length; i++)
                {
                    string str = (ANadapter.FreqArr[i] - (double)ANadapter.FreqCentr).ToString().Replace(".", ",") + ";" +
                        (lev + (double)ANadapter.LevelArr[i]).ToString().Replace(".", ",");
                    file.WriteLine(str);
                }

            }
        }
        public int FindMarkerIndOnTrace(double[] tracepoints, double freq)
        {
            int ind = -1;
            if (freq >= tracepoints[0] && freq <= tracepoints[tracepoints.Length - 1])
            {
                double deviation = double.MaxValue;
                for (int i = 0; i < tracepoints.Length; i++)
                {
                    if (Math.Abs(tracepoints[i] - freq) < deviation)
                    {
                        deviation = Math.Abs(tracepoints[i] - freq);
                        ind = i;
                    }
                }
            }
            else if (freq < tracepoints[0])
            { ind = 0; }
            else if (freq > tracepoints[tracepoints.Length - 1])
            { ind = tracepoints.Length - 1; }
            return ind;
        }

        private void SetMeas21_Click(object sender, RoutedEventArgs e)
        {
            //    // send command
            //    var context = new DummyExecutionContextMy(logger);
            //    var command = new CMD.MesureTraceCommand();
            //    command.Parameter.Att_dB = 0;
            //    command.Parameter.FreqStart_Hz = 90000000;
            //    command.Parameter.FreqStop_Hz = 110000000;
            //    //command.Parameter.FreqStart_Hz = 1800000000;
            //    //command.Parameter.FreqStop_Hz = 1900000000;
            //    command.Parameter.PreAmp_dB = 20;
            //    command.Parameter.RBW_Hz = 30000;
            //    command.Parameter.VBW_Hz = 30000;
            //    command.Parameter.RefLevel_dBm = -40;
            //    command.Parameter.SweepTime_s = 0.00001;
            //    command.Parameter.TraceCount = 100;
            //    command.Parameter.TracePoint = 1000;
            //    command.Parameter.TraceType = CMD.Parameters.TraceType.MinHold;
            //    command.Parameter.DetectorType = CMD.Parameters.DetectorType.Average;
            //    command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBmkV;

            //    adapter.MesureTraceCommandHandler(command, context);


        }
        private void SetMeas22_Click(object sender, RoutedEventArgs e)
        {
            //// send command
            //var context = new DummyExecutionContextMy(logger);
            //var command = new CMD.MesureTraceCommand();
            //command.Parameter.Att_dB = 0;
            //command.Parameter.FreqStart_Hz = 90000000;
            //command.Parameter.FreqStop_Hz = 110000000;
            ////command.Parameter.FreqStart_Hz = 1800000000;
            ////command.Parameter.FreqStop_Hz = 1900000000;
            //command.Parameter.PreAmp_dB = 20;
            //command.Parameter.RBW_Hz = 30000;
            //command.Parameter.VBW_Hz = 30000;
            //command.Parameter.RefLevel_dBm = -40;
            //command.Parameter.SweepTime_s = 0.00001;
            //command.Parameter.TraceCount = 100;
            //command.Parameter.TracePoint = 1000;
            //command.Parameter.TraceType = CMD.Parameters.TraceType.Average;
            //command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
            //command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

            //adapter.MesureTraceCommandHandler(command, context);


        }

        private void GetIQ1_Click(object sender, RoutedEventArgs e)
        {
            //var context = new DummyExecutionContextMy(logger);
            //var command = new CMD.MesureIQStreamCommand();

            //command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
            //command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
            //command.Parameter.Att_dB = 20;
            //command.Parameter.PreAmp_dB = 30;
            //command.Parameter.RefLevel_dBm = -20;
            //command.Parameter.BitRate_MBs = 10;
            //command.Parameter.IQBlockDuration_s = 0.2;
            //command.Parameter.IQReceivTime_s = 0.6;
            //command.Parameter.MandatoryPPS = false;
            //command.Parameter.MandatorySignal = true;
            //command.Parameter.TimeStart = (DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks);
            //command.Parameter.TimeStart += 2 * 10000000;
            //Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
            ////adapter.MesureIQStreamCommandHandler(command, context);
        }

        private void SetWND_Click(object sender, RoutedEventArgs e)
        {
            ANadapter.SetIQWindow();
        }
        private void SetWNDType_Click(object sender, RoutedEventArgs e)
        {
            //ANadapter.SetWindowType();
        }
        bool add = false;
        private void GetIQAN_Click(object sender, RoutedEventArgs e)
        {
            add = true;
            //SHD += SHGetIQ;
            //AND += ANGetIQ;


        }
        private void SHGetIQ()
        {
            try
            {
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 0.6;
                command.Parameter.IQBlockDuration_s = 0.1;
                command.Parameter.IQReceivTime_s = 1.1;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = false;
                long offset = DateTime.Now.Ticks;// - NextSecond;
                command.Parameter.TimeStart = DateTime.UtcNow.Ticks + offset - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //command.Parameter.TimeStart += (long)(0.1 * 10000000);

                Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                SHadapter.MesureIQStreamCommandHandler(command, context);
                //App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                //{
                //    SHIQ.IQ = SHadapter.IQArr;
                //});

            }
            finally
            {
                SHD -= SHGetIQ;
            }
        }
        private void ANGetIQ()
        {
            try
            {
                //ANadapter.GetIQ();
                //App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                //{
                //    ANIQ.IQ = ANadapter.IQArr;
                //    ANIQ.TriggerOfset = ANadapter.TriggerOfset;
                //});

            }
            finally
            {
                AND -= ANGetIQ;
            }
        }


    }
    public class DummyTimeService : ITimeService
    {
        //private readonly ITimeStamp _timeStamp;
        //private long _timeCorrection;

        //public DummyTimeService()
        //{
        //    this._timeStamp = new TimeStamp();
        //}

        //public ITimeStamp TimeStamp => this._timeStamp;

        //public long TimeCorrection
        //{
        //    get
        //    {
        //        return Interlocked.Read(ref this._timeCorrection);
        //    }
        //    set
        //    {
        //        Interlocked.Exchange(ref this._timeCorrection, value);
        //    }
        //}

        //public DateTime GetGnssTime()
        //{
        //    ///TODO: Необходимо дописать код учитывающий поправку времени относительно GPS
        //    return DateTime.Now;

        //}

        //public DateTime GetGnssUtcTime()
        //{
        //    ///TODO: Необходимо дописать код учитывающий поправку времени относительно GPS
        //    return DateTime.UtcNow;
        //}

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
    public class DummyExecutionContextMy : IExecutionContext
    {
        private readonly ILogger _logger;

        public DummyExecutionContextMy(ILogger logger)
        {
            this._logger = logger;
        }

        public CancellationToken Token { get; set; }

        public void Abort(Exception e)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Abort");
        }

        public void Cancel()
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Cancel");
        }

        public void Finish()
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Finish");
        }

        public void Lock(params CommandType[] types)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Lock");
        }

        public void Lock(params Type[] commandType)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Lock");
        }

        public void Lock()
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Lock");
        }

        public void PushResult(ICommandResultPart result)
        {
            //this._logger.Verbouse("DummyExecutionContext", "Call method", $"PushResult");

            //Task.Run(() =>
            //{
            //    var myResult = result as MyResultObject

            //    for (int i = 0; i < length; i++)
            //    {
            //        var data = myResult.myProperty[i];
            //        if (data > 10000)
            //        {
            //            this._logger.Critical("Test", "Result", $"Не соотвествие данных");
            //        }
            //    }
            //});
        }

        public void Unlock(params CommandType[] types)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Unlock");
        }

        public void Unlock(params Type[] commandType)
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Unlock");
        }

        public void Unlock()
        {
            this._logger.Verbouse("DummyExecutionContext", "Call method", $"Unlock");
        }
    }
}
