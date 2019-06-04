﻿using System;
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
using ADP = Atdi.AppUnits.Sdrn.DeviceServer.Adapters;
using CMD = Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.UnitTest.Sdrn.DeviceServer;
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
        GNSSNMEA gnss;

        private delegate void AnyDelegate();
        private Thread TimeThread;
        private Thread ANThread;
        private AnyDelegate AND;
        private Thread SHThread;
        private AnyDelegate SHD;
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
            //TimeThread = new Thread(GetGPSData);
            //TimeThread.Name = "GPSThread";
            //TimeThread.IsBackground = true;
            //TimeThread.Start();

            ANThread = new Thread(ANWorks);
            ANThread.Name = "ANThread";
            ANThread.IsBackground = true;
            ANThread.Start();
            AND += ANConnect;

            //SHThread = new Thread(SHWorks);
            //SHThread.Name = "SHThread";
            //SHThread.IsBackground = true;
            //SHThread.Start();
            //SHD += SHConnect;
        }
        long NextSecond = 0;
        private void GetGPSData()
        {
            while (true)
            {
                if ((((double)gnss.LocalTime.Ticks) / 10000000 - (double)(gnss.LocalTime.Ticks / 10000000)) < 0.00001)
                {
                    //Debug.WriteLine(gnss.LocalTime.Ticks);
                    //Debug.WriteLine("GNSS " + new DateTime(gnss.LocalTime.Ticks).ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                    NextSecond = gnss.LocalTime.Ticks + 10000000;
                    //Debug.WriteLine("Time " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                    //Debug.WriteLine("Next " + new DateTime(NextSecond).ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                    if (add)
                    {
                        SHD += SHGetIQ;
                        AND += ANGetIQ;
                        add = false;
                    }
                }
                Thread.Sleep(new TimeSpan(10000));
            }

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
                    Optimization = 2

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
                    SerialNumber = "16319373",
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
                SHThread.Abort();
                SHD -= SHDisconnect;
            }
        }


        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            ANadapter.Connect(adapterHost);
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            gnss.CloseConGPS();
            AND += ANDisconnect;
            SHD += SHDisconnect;
        }

        private void GetTraceAN()
        {
            try
            {
                // send command
                var context = new DummyExecutionContext(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 1800000000;
                command.Parameter.FreqStop_Hz = 1900000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = 3000;
                command.Parameter.VBW_Hz = 3000;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 10;
                command.Parameter.TracePoint = 100001;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.Average;
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
                var context = new DummyExecutionContext(logger);
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
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
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
                var context = new DummyExecutionContext(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 104.750m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 105.250m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
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
                var context = new DummyExecutionContext(logger);
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
                var context = new DummyExecutionContext(logger);
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

        private void GetTraceSH()
        {
            try
            {
                // send command
                var context = new DummyExecutionContext(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 20;
                command.Parameter.FreqStart_Hz = 1800000000;
                command.Parameter.FreqStop_Hz = 1900000000;
                command.Parameter.PreAmp_dB = 20;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 10;
                command.Parameter.TracePoint = 100001;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.Average;
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
                var context = new DummyExecutionContext(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 10;
                command.Parameter.FreqStart_Hz = 104.750m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 105.250m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 30;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 10;
                command.Parameter.TracePoint = 8000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.MaxHold;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                SHadapter.MesureTraceCommandHandler(command, context);


                //// send command
                //var context = new DummyExecutionContext(logger);
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
                var context = new DummyExecutionContext(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 10;
                command.Parameter.FreqStart_Hz = 104.750m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 105.250m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 30;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
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
                var context = new DummyExecutionContext(logger);
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
                var context = new DummyExecutionContext(logger);
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

        private void GetIQAN()
        {
            try
            {
                // send command
                var context = new DummyExecutionContext(logger);
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
                command.Parameter.IQBlockDuration_s = 1.1;
                command.Parameter.IQReceivTime_s = 2.1;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = false;
                long offset = DateTime.Now.Ticks - NextSecond;
                command.Parameter.TimeStart = DateTime.UtcNow.Ticks + offset - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //command.Parameter.TimeStart += (long)(0.1 * 10000000);

                Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
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
                var context = new DummyExecutionContext(logger);
                var command = new CMD.MesureIQStreamCommand();
                //135,5
                //command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 10;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 0.05;
                command.Parameter.IQBlockDuration_s = 0.2;
                command.Parameter.IQReceivTime_s = 2.5;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = true;
                command.Parameter.TimeStart = 1000000 + DateTime.UtcNow.Ticks + - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                //long offset = DateTime.Now.Ticks - NextSecond;
                //command.Parameter.TimeStart = DateTime.UtcNow.Ticks + offset - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                //Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                //Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                SHadapter.MesureIQStreamCommandHandler(command, context);
            }
            finally
            {
                SHD -= GetIQSH;
            }
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
        private void SetMeasIQ_Click(object sender, RoutedEventArgs e)
        {
            AND += GetIQAN;
            SHD += GetIQSH;
        }





        private void SetMeas1_Click(object sender, RoutedEventArgs e)
        {
            //// send command
            //var context = new DummyExecutionContext(logger);
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
            //    var context = new DummyExecutionContext(logger);
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
            //var context = new DummyExecutionContext(logger);
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
            //var context = new DummyExecutionContext(logger);
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
                var context = new DummyExecutionContext(logger);
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
                long offset = DateTime.Now.Ticks - NextSecond;
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
            ///TODO: Необходимо дописать код учитывающий поправку времени относительно GPS
            return DateTime.Now; 

        }

        public DateTime GetGnssUtcTime()
        {
            ///TODO: Необходимо дописать код учитывающий поправку времени относительно GPS
            return DateTime.UtcNow;
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
