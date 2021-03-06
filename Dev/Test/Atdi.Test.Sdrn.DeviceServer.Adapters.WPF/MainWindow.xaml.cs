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

        DummyGPSExecutionContextMy KSGPSContext;
        //ADP.SignalHound.Adapter adapter;
        ADP.KTN6841A.Adapter KSadapter;
        ADP.RSFPL.Adapter AN2adapter;
        ADP.SignalHound.Adapter SHadapter;
        ADP.SPIDRotator.Adapter ROadapter;

        ADP.GPS.GPSAdapter GPSadapter;
        //GNSSNMEA gnss;

        private delegate void AnyDelegate();
        private Thread TimeThread;
        private Thread KSThread;
        private Thread AN2Thread;
        private AnyDelegate KSD;
        private AnyDelegate AN2D;
        private Thread SHThread;
        private AnyDelegate SHD;

        private Thread GPSThread;
        private AnyDelegate GPSD;

        private Thread ROThread;
        private AnyDelegate ROD;
        private DummyExecutionContextMy ROContext = null;

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
            //KSThread = new Thread(KSWorks);
            //KSThread.Name = "KSThread";
            //KSThread.IsBackground = true;
            //KSThread.Start();
            //KSD += KSConnect;

            AN2Thread = new Thread(AN2Works);
            AN2Thread.Name = "AN2Thread";
            AN2Thread.IsBackground = true;
            AN2Thread.Start();
            AN2D += AN2Connect;

            //SHThread = new Thread(SHWorks);
            //SHThread.Name = "SHThread";
            //SHThread.IsBackground = true;
            //SHThread.Start();
            //SHD += SHConnect;

            //GPSThread = new Thread(GPSWorks);
            //GPSThread.Name = "GPSThread";
            //GPSThread.IsBackground = true;
            //GPSThread.Start();
            //GPSD += GPSConnect;

            //ROThread = new Thread(ROWorks);
            //ROThread.Name = "ROThread";
            //ROThread.IsBackground = true;
            //ROThread.Start();
            //ROD += ROConnect;
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

        #region KS
        private void KSWorks()
        {
            TimeSpan ts = new TimeSpan(10000);
            bool Cycle = true;
            while (Cycle)
            {
                if (KSD != null) { KSD(); }
                Thread.Sleep(ts);
            }
        }
        private void KSConnect()
        {
            try
            {
                KSGPSContext = new DummyGPSExecutionContextMy(logger);
                var adapterConfig = new ADP.KTN6841A.AdapterConfig()
                {
                    SensorName = "US50310575",
                    SmsHostName = "",
                    WindowType = 4,
                    UseGNSS = true,
                    SensorInLocalNetwork = false,
                    LockSensorResource = false,
                    SelectedAntenna = 2
                    //SerialNumber = "101396",
                    //IPAddress = "192.168.2.110",
                    //DisplayUpdate = true,
                    //OnlyAutoSweepTime = true,
                    //Optimization = 2,
                    ////ConnectionMode = 1
                };

                KSadapter = new ADP.KTN6841A.Adapter(adapterConfig, logger, TimeService);
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    DS_KS.KSAdapter = KSadapter;
                    //ANIQ.KSAdapter = KSadapter;
                });//ANIQ.ANAdapter = ANadapter;
                KSadapter.Connect(adapterHost);

                var command = new CMD.GpsCommand();
                command.Parameter.GpsMode = CMD.Parameters.GpsMode.Start;
                KSadapter.GPSCommandHandler(command, KSGPSContext);
            }
            finally
            {
                KSD -= KSConnect;
            }
        }

        private void ANDisconnect()
        {
            try
            {
                KSadapter.Disconnect();

            }
            finally
            {
                KSThread.Abort();
                KSD -= ANDisconnect;
            }
        }
        #endregion KS

        #region AN2
        private void AN2Works()
        {
            TimeSpan ts = new TimeSpan(10000);
            bool Cycle = true;
            while (Cycle)
            {
                if (AN2D != null) { AN2D(); }
                Thread.Sleep(ts);
            }
        }
        private void AN2Connect()
        {
            try
            {
                var adapterConfig = new ADP.RSFPL.AdapterConfig()
                {
                    SerialNumber = "100706",
                    //IPAddress = "127.0.0.1",
                    IPAddress = "192.168.0.43",
                    DisplayUpdate = true,
                    OnlyAutoSweepTime = true,
                    Optimization = 2,
                    //ConnectionMode = 1
                };

                AN2adapter = new ADP.RSFPL.Adapter(adapterConfig, logger, TimeService);
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    DS_AN2.ANAdapter = AN2adapter;
                    AN2IQ.ANAdapter = AN2adapter;
                });//ANIQ.ANAdapter = ANadapter;
                AN2adapter.Connect(adapterHost);
            }
            finally
            {
                AN2D -= AN2Connect;
            }
        }

        private void AN2Disconnect()
        {
            try
            {
                AN2adapter.Disconnect();

            }
            finally
            {
                AN2Thread.Abort();
                AN2D -= AN2Disconnect;
            }
        }
        #endregion AN

        #region SH
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
                    SerialNumber = 16319373,//18250280,//"18250087",// "16319373",
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
        #endregion SH

        #region RO
        private void ROWorks()
        {
            TimeSpan ts = new TimeSpan(10000);
            bool Cycle = true;
            while (Cycle)
            {
                if (ROD != null) { ROD(); }
                Thread.Sleep(ts);
            }
        }
        private void ROConnect()
        {
            try
            {
                var adapterConfig = new ADP.SPIDRotator.AdapterConfig()
                {
                    PortNumber = 7,
                    ElevationIsPolarization = false,
                    ControlDeviceManufacturer = "SPID",
                    ControlDeviceName = "ROT2PROG",
                    ControlDeviceCode = "N/A",
                    RotationDeviceManufacturer = "SPID",
                    RotationDeviceName = "SPX AZ/EL-02",
                    RotationDeviceCode = "N/A",
                    AzimuthMin_dg = 0,
                    AzimuthMax_dg = 360,
                    ElevationMin_dg = -20,
                    ElevationMax_dg = 100
                    

                    //SyncCPUtoGPS = true,
                    //GPSPortBaudRate = 38400,
                    //GPSPortNumber = 1,

                };

                ROadapter = new ADP.SPIDRotator.Adapter(adapterConfig, logger, TimeService);
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                });
                //SHIQ.ANAdapter = ANadapter;
                ROadapter.Connect(adapterHost);

                ROContext = new DummyExecutionContextMy(logger);
                var command = new CMD.SetRotatorPositionCommand();

                command.Parameter.Mode = CMD.Parameters.RotatorPositionMode.Get;//raz.Next(300, 330);
                command.Parameter.PublicResultAfterSet = false;//raz.Next(300, 330);

                ROadapter.SetRotatorPositionCommandHandler(command, ROContext);
            }
            finally
            {
                ROD -= ROConnect;
            }
        }

        private void RODisconnect()
        {
            try
            {
                ROadapter.Disconnect();
            }
            finally
            {
                ROD -= RODisconnect;
                ROThread.Abort();
            }
        }
        #endregion RO


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
                    PortName = "COM45",
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


        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            KSadapter.Connect(adapterHost);
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            //gnss.CloseConGPS();
            KSD += ANDisconnect;
            AN2D += ANDisconnect;
            SHD += SHDisconnect;
        }

        #region RO     



        private void SetRandomRO1()
        {
            try
            {
                Random raz = new Random();
                Random rel = new Random();
                //var context = new DummyExecutionContextMy(logger);
                if (ROContext.Token.IsCancellationRequested)
                {
                    ROContext = new DummyExecutionContextMy(logger);
                }
                var command = new CMD.SetRotatorPositionCommand();
                command.Parameter.Azimuth_dg = 360;//raz.Next(300, 330); 
                command.Parameter.AzimuthSpeed = 0;//raz.Next(300, 330);
                command.Parameter.AzimuthStep_dg = 2;// 1.5f;//raz.Next(300, 330);
                command.Parameter.AzimuthTimeStep_ms = 500;//raz.Next(300, 330);

                command.Parameter.Polarization_dg = 0;// rel.Next(0, 20);
                command.Parameter.PolarizationSpeed = 0;//raz.Next(300, 330);
                command.Parameter.PolarizationStep_dg = 0;//raz.Next(300, 330);
                command.Parameter.PolarizationTimeStep_ms = 0;//raz.Next(300, 330);

                command.Parameter.Elevation_dg = 0;// rel.Next(0, 20);
                command.Parameter.ElevationSpeed = 0;//raz.Next(300, 330);
                command.Parameter.ElevationStep_dg = 2;//raz.Next(300, 330);
                command.Parameter.ElevationTimeStep_ms = 10000;//raz.Next(300, 330);


                command.Parameter.Mode = CMD.Parameters.RotatorPositionMode.SetAndGet;//raz.Next(300, 330);
                command.Parameter.PublicResultAfterSet = false;//raz.Next(300, 330);

                System.Diagnostics.Debug.WriteLine(command.Parameter.Azimuth_dg + "  st  " + command.Parameter.Polarization_dg);
                ROadapter.SetRotatorPositionCommandHandler(command, ROContext);
            }
            finally
            {
                ROD -= SetRandomRO1;
            }
        }

        private void SetRandomRO2()
        {
            try
            {
                Random raz = new Random();
                Random rel = new Random();
                //var context = new DummyExecutionContextMy(logger);
                if (ROContext.Token.IsCancellationRequested)
                {
                    ROContext = new DummyExecutionContextMy(logger);
                }
                var command = new CMD.SetRotatorPositionCommand();
                command.Parameter.Azimuth_dg = 340;
                command.Parameter.AzimuthSpeed = 0;//raz.Next(300, 330);
                command.Parameter.AzimuthStep_dg = 0;//raz.Next(300, 330);
                command.Parameter.AzimuthTimeStep_ms = 500;//raz.Next(300, 330);

                command.Parameter.Polarization_dg = 0;//rel.Next(0, 20);
                command.Parameter.PolarizationSpeed = 0;//raz.Next(300, 330);
                command.Parameter.PolarizationStep_dg = 0;//raz.Next(300, 330);
                command.Parameter.PolarizationTimeStep_ms = 0;//raz.Next(300, 330);

                command.Parameter.Elevation_dg = 20;// rel.Next(0, 20);
                command.Parameter.ElevationSpeed = 0;//raz.Next(300, 330);
                command.Parameter.ElevationStep_dg = 0;//raz.Next(300, 330);
                command.Parameter.ElevationTimeStep_ms = 1000;//raz.Next(300, 330);

                command.Parameter.Mode = CMD.Parameters.RotatorPositionMode.SetAndGet;
                command.Parameter.PublicResultAfterSet = false;//raz.Next(300, 330);

                System.Diagnostics.Debug.WriteLine(command.Parameter.Azimuth_dg + "  st  " + command.Parameter.Polarization_dg);
                ROadapter.SetRotatorPositionCommandHandler(command, ROContext);
            }
            finally
            {
                ROD -= SetRandomRO2;
            }
        }
        private void SetROStop()
        {
            try
            {
                ROContext.Token = new CancellationToken(true);
            }
            finally
            {
                ROD -= SetROStop;
            }
        }
        private void SetRandomGet()
        {
            try
            {
                Random raz = new Random();
                Random rel = new Random();
                //var context = new DummyExecutionContextMy(logger);
                if (ROContext.Token.IsCancellationRequested)
                {
                    ROContext = new DummyExecutionContextMy(logger);
                }
                var command = new CMD.SetRotatorPositionCommand();
                command.Parameter.Azimuth_dg = 340;
                command.Parameter.AzimuthSpeed = 0;//raz.Next(300, 330);
                command.Parameter.AzimuthStep_dg = 0;// 1.5f;//raz.Next(300, 330);
                command.Parameter.AzimuthTimeStep_ms = 500;//raz.Next(300, 330);

                command.Parameter.Polarization_dg = 20;//rel.Next(0, 20);
                command.Parameter.PolarizationSpeed = 0;//raz.Next(300, 330);
                command.Parameter.PolarizationStep_dg = 0;//raz.Next(300, 330);
                command.Parameter.PolarizationTimeStep_ms = 0;//raz.Next(300, 330);

                command.Parameter.Elevation_dg = 0;// rel.Next(0, 20);
                command.Parameter.ElevationSpeed = 0;//raz.Next(300, 330);
                command.Parameter.ElevationStep_dg = 0;//raz.Next(300, 330);
                command.Parameter.ElevationTimeStep_ms = 0;//raz.Next(300, 330);

                command.Parameter.Mode = CMD.Parameters.RotatorPositionMode.Get;
                command.Parameter.PublicResultAfterSet = false;//raz.Next(300, 330);

                ROadapter.SetRotatorPositionCommandHandler(command, ROContext);
            }
            finally
            {
                ROD -= SetRandomGet;
            }
        }
        private void SetAzz1_Click(object sender, RoutedEventArgs e)
        {
            ROD += SetRandomRO1;
        }

        private void SetAzz2_Click(object sender, RoutedEventArgs e)
        {
            ROD += SetRandomRO2;
        }

        private void StopAzz_Click(object sender, RoutedEventArgs e)
        {
            ROD += SetROStop;

        }
        private void Get_Click(object sender, RoutedEventArgs e)
        {
            ROD += SetRandomGet;
        }
        #endregion RO
        #region KS
        private void GetTraceKS()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                decimal centr = 1000m * 1000000;
                decimal span = 10.0m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = -1;// 0.003;
                command.Parameter.TraceCount = 1;
                command.Parameter.TracePoint = -1;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;
                long t1 = TimeService.TimeStamp.Ticks;
                KSadapter.MesureTraceCommandHandler(command, context);
                long t2 = TimeService.TimeStamp.Ticks;
                string rbw = KSadapter.RBW.ToString() + "  ";
                string tp = KSadapter.LevelArrLength.ToString() + "  ";

                centr = 500m * 1000000;
                span = 5.0m * 1000000;//0.025m
                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.RBW_Hz = 50000;
                command.Parameter.VBW_Hz = 50000;

                KSadapter.MesureTraceCommandHandler(command, context);
                rbw += KSadapter.RBW.ToString() + "  ";
                tp += KSadapter.LevelArrLength.ToString() + "  ";

                long t3 = TimeService.TimeStamp.Ticks;
                centr = 2000m * 1000000;
                span = 20.0m * 1000000;//0.025m
                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.RBW_Hz = 50000;
                command.Parameter.VBW_Hz = 50000;
                KSadapter.MesureTraceCommandHandler(command, context);
                rbw += KSadapter.RBW.ToString() + "  ";
                tp += KSadapter.LevelArrLength.ToString() + "  ";

                long t4 = TimeService.TimeStamp.Ticks;

                Debug.WriteLine(new TimeSpan(t2 - t1).ToString() +
                    "   \r\n" + new TimeSpan(t3 - t2).ToString() +
                    "   \r\n" + new TimeSpan(t4 - t3).ToString() +
                    "   \r\n" + rbw +
                    "   \r\n" + tp);
            }
            finally
            {
                KSD -= GetTraceKS;
            }
        }
        private void GetTraceKS2()
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
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 10;
                command.Parameter.TracePoint = 1000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.MaxHold;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                KSadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                KSD -= GetTraceKS2;
            }
        }
        private void GetTraceKS3()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                decimal centr = 100m * 1000000;
                decimal span = 25.0m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = 50;
                command.Parameter.VBW_Hz = 50;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = -1;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                KSadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                KSD -= GetTraceKS3;
            }
        }
        private void GetTraceKS1800()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                decimal centr = 1850m * 1000000;
                decimal span = 15.0m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 500000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                KSadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                KSD -= GetTraceKS1800;
            }
        }
        private void GetTraceKS1800AVG()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 0;
                command.Parameter.FreqStart_Hz = 150.05m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 167.7625m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 10;
                command.Parameter.TracePoint = 141700;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                KSadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                KSD -= GetTraceKS1800AVG;
            }
        }
        private void GetTraceKS9352()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = 5;
                command.Parameter.FreqStart_Hz = 934.95m * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 935.45m * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = 1;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 2000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                KSadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                KSD -= GetTraceKS9352;
            }
        }
        private void GetTraceKS1350()
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

                KSadapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                KSD -= GetTraceKS1350;
            }
        }
        #endregion AN

        #region AN2
        private void GetTrace2AN()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                decimal centr = 2109.9m * 1000000;
                decimal span = 10.0m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = 2109.9m * 1000000;
                command.Parameter.FreqStop_Hz = 2169.9m * 1000000;
                //command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = -1;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = -1;// 0.003;
                command.Parameter.TraceCount = 1;
                command.Parameter.TracePoint = 3600;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;
                //long t1 = TimeService.TimeStamp.Ticks;
                AN2adapter.MesureTraceCommandHandler(command, context);
                //long t2 = TimeService.TimeStamp.Ticks;

                Thread.Sleep(5000);

                //centr = 500m * 1000000;
                //span = 5.0m * 1000000;//0.025m
                //command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.TracePoint = 2000;
                AN2adapter.MesureTraceCommandHandler(command, context);

                //centr = 2000m * 1000000;
                //span = 20.0m * 1000000;//0.025m
                //command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                Thread.Sleep(5000);
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.TracePoint = 50000;
                AN2adapter.MesureTraceCommandHandler(command, context);
                //long t3 = TimeService.TimeStamp.Ticks;

                //MessageBox.Show(new TimeSpan(t2 - t1).ToString() + "   \r\n" + new TimeSpan(t3 - t2).ToString());

//                Thread.Sleep(5000);

                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.TracePoint = 2000;
                AN2adapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AN2D -= GetTrace2AN;
            }
        }
        private void GetTrace2AN2()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();

                decimal centr = 100m * 1000000;
                decimal span = 30.0m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = 5000;
                command.Parameter.VBW_Hz = 5000;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 2;
                command.Parameter.TracePoint = 1000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.MaxHold;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                AN2adapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AN2D -= GetTrace2AN2;
            }
        }
        private void GetTrace2AN3()
        {
            try
            {
                // send command
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.EstimateRefLevelCommand();
                command.Parameter.Att_dB = -1;
                decimal centr = 2412m * 1000000;
                decimal span = 40.0m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RBW_Hz = 20000;
                command.Parameter.VBW_Hz = 20000;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 1500;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                AN2adapter.EstimateRefLevelCommandHandler(command, context);
            }
            finally
            {
                AN2D -= GetTrace2AN3;
            }
        }
        private void GetTrace2AN1800()
        {
            try
            {
                // send command
                var context = new DummyExecutionContextMy(logger);
                var command = new CMD.MesureTraceCommand();
                command.Parameter.Att_dB = -1;
                command.Parameter.FreqStart_Hz = 413 * 1000000;// 421.5075m * 1000000;// 100000000;421.525m
                command.Parameter.FreqStop_Hz = 430 * 1000000;// 421.5425m * 1000000;//110000000;
                command.Parameter.PreAmp_dB = -1;
                command.Parameter.RBW_Hz = -1;
                command.Parameter.VBW_Hz = -1;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 10;
                command.Parameter.TracePoint = 136000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                AN2adapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AN2D -= GetTrace2AN1800;
            }
        }
        private void GetTrace2AN1800AVG()
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

                AN2adapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AN2D -= GetTrace2AN1800AVG;
            }
        }
        private void GetTrace2AN9352()
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
                command.Parameter.RBW_Hz = 10;
                command.Parameter.VBW_Hz = 10;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.SweepTime_s = 0.00001;
                command.Parameter.TraceCount = 100;
                command.Parameter.TracePoint = 2000;
                command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
                command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
                command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

                AN2adapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AN2D -= GetTrace2AN9352;
            }
        }
        private void GetTrace2AN1350()
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

                AN2adapter.MesureTraceCommandHandler(command, context);
            }
            finally
            {
                AN2D -= GetTrace2AN1350;
            }
        }
        #endregion AN2

        #region SH
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
                command.Parameter.RBW_Hz = 500;
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
                var command = new CMD.EstimateRefLevelCommand();
                command.Parameter.Att_dB = 30;
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

                SHadapter.EstimateRefLevelCommandHandler(command, context);
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
        #endregion SH

        long UTCOffset = 621355968000000000;
        private void GetIQKS()
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
                decimal centr = 424.650m * 1000000;
                decimal span = 12.8m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 0.6;
                command.Parameter.IQBlockDuration_s = 0.1;
                command.Parameter.IQReceivTime_s = 0.1;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = true;

                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                KSadapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                KSD -= GetIQKS;
            }
        }
        private void GetIQ2AN()
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
                decimal centr = 424.650m * 1000000;
                decimal span = 12.8m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 0.6;
                command.Parameter.IQBlockDuration_s = 0.1;
                command.Parameter.IQReceivTime_s = 0.1;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = true;

                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                AN2adapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                AN2D -= GetIQ2AN;
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
                command.Parameter.IQBlockDuration_s = 1.0;
                command.Parameter.IQReceivTime_s = 1.1;
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

        private void GetIQKS2()
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
                decimal centr = 1824.4m * 1000000;
                decimal span = 1.25m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//
                //command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 1.6;
                command.Parameter.IQBlockDuration_s = 0.2;
                command.Parameter.IQReceivTime_s = 0.2;
                command.Parameter.MandatoryPPS = false;
                command.Parameter.MandatorySignal = false;

                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                KSadapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                KSD -= GetIQKS2;
            }
        }
        private void GetIQ2AN2()
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
                decimal centr = 1824.4m * 1000000;
                decimal span = 1.25m * 1000000;//0.025m

                command.Parameter.FreqStart_Hz = centr - span / 2;//910 * 1000000;//424.625m * 1000000;//424.650
                command.Parameter.FreqStop_Hz = centr + span / 2;//
                //command.Parameter.FreqStart_Hz = 935.0645m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 935.3355m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.BitRate_MBs = 1.6;
                command.Parameter.IQBlockDuration_s = 0.2;
                command.Parameter.IQReceivTime_s = 0.2;
                command.Parameter.MandatoryPPS = false;
                command.Parameter.MandatorySignal = false;

                long offset = (long)(0.04 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                AN2adapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                AN2D -= GetIQ2AN2;
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
                command.Parameter.BitRate_MBs = 0.6;
                command.Parameter.Att_dB = 0;
                command.Parameter.PreAmp_dB = 0;
                //command.Parameter.FreqStart_Hz = 424.6375m * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
                //command.Parameter.FreqStop_Hz = 424.6625m * 1000000;//930*1000000;//424.675m * 1000000;
                //command.Parameter.BitRate_MBs = 0.1;
                //command.Parameter.Att_dB = 0;
                //command.Parameter.PreAmp_dB = 30;

                command.Parameter.RefLevel_dBm = -40;
                command.Parameter.IQBlockDuration_s = 1.0;
                command.Parameter.IQReceivTime_s = 1.0;
                command.Parameter.MandatoryPPS = true;
                command.Parameter.MandatorySignal = false;
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


        private void GetIQKS3()
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
                command.Parameter.MandatorySignal = false;

                long offset = (long)(0.03 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                KSadapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                KSD -= GetIQKS3;
            }
        }
        private void GetIQ2AN3()
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
                command.Parameter.MandatorySignal = false;

                long offset = (long)(0.03 * 10000000);
                command.Parameter.TimeStart = TimeService.GetGnssUtcTime().Ticks + offset - UTCOffset;// new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
                ////////Debug.WriteLine("\r\n" + (new TimeSpan(command.Parameter.TimeStart)).ToString(@"hh\:mm\:ss\.fffffff") + " Start");
                ////////command.Parameter.TimeStart += (long)(0.1 * 10000000);

                ////////Debug.WriteLine("\r\n" + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks).ToString() + " Set Param");
                ////////Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
                AN2adapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                AN2D -= GetIQ2AN3;
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

        private void GetIQKS4()
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
                KSadapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                KSD -= GetIQKS4;
            }
        }
        private void GetIQ2AN4()
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
                AN2adapter.MesureIQStreamCommandHandler(command, context);

            }
            finally
            {
                AN2D -= GetIQ2AN4;
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
            KSD += GetTraceKS;
            AN2D += GetTrace2AN;
            SHD += GetTraceSH;
        }
        private void SetMeas2_Click(object sender, RoutedEventArgs e)
        {
            KSD += GetTraceKS2;
            AN2D += GetTrace2AN2;
            SHD += GetTraceSH2;
        }
        private void SetMeas3_Click(object sender, RoutedEventArgs e)
        {
            AN2D += GetTrace2AN3;
            KSD += GetTraceKS3;
            SHD += GetTraceSH3;
        }
        private void SetMeas1800_Click(object sender, RoutedEventArgs e)
        {
            AN2D += GetTrace2AN1800;
            KSD += GetTraceKS1800;
            SHD += GetTraceSH1800;
        }
        private void SetMeas1800AVG_Click(object sender, RoutedEventArgs e)
        {
            AN2D += GetTrace2AN1800AVG;
            KSD += GetTraceKS1800AVG;
            SHD += GetTraceSH1800AVG;
        }
        private void SetMeas9352AVG_Click(object sender, RoutedEventArgs e)
        {
            AN2D += GetTrace2AN9352;
            KSD += GetTraceKS9352;
            SHD += GetTraceSH9352;
        }
        private void SetMeasAVG1350_Click(object sender, RoutedEventArgs e)
        {
            AN2D += GetTrace2AN1350;
            KSD += GetTraceKS1350;
            SHD += GetTraceSH1350;
        }
        private void SetMeasIQ_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("=======================================================================================================");
            KSD += GetIQKS;
            AN2D += GetIQ2AN;
            SHD += GetIQSH;
        }
        private void SetMeasIQ2_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("=======================================================================================================");
            KSD += GetIQKS2;
            AN2D += GetIQ2AN2;
            SHD += GetIQSH2;
        }
        private void SetMeasIQ3_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("=======================================================================================================");
            KSD += GetIQKS3;
            AN2D += GetIQ2AN3;
            SHD += GetIQSH3;
        }
        private void SetMeasIQ4_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("=======================================================================================================");
            KSD += GetIQKS4;
            AN2D += GetIQ2AN4;
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
            //int ind = FindMarkerIndOnTrace(ANadapter.FreqArr, (double)ANadapter.FreqCentr);
            //double lev = Math.Abs(ANadapter.LevelArr[ind]);
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppPath + "\\" + ANadapter.FreqCentr.ToString() + ".txt", false))
            //{
            //    for (int i = 0; i < ANadapter.FreqArr.Length; i++)
            //    {
            //        string str = (ANadapter.FreqArr[i] - (double)ANadapter.FreqCentr).ToString().Replace(".", ",") + ";" +
            //            (lev + (double)ANadapter.LevelArr[i]).ToString().Replace(".", ",");
            //        file.WriteLine(str);
            //    }

            //}
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
            //KSadapter.SetIQWindow();
        }
        private void SetWNDType_Click(object sender, RoutedEventArgs e)
        {
            //ANadapter.SetWindowType();
        }
        bool add = false;
        private void GetIQAN_Click(object sender, RoutedEventArgs e)
        {

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
                KSD -= ANGetIQ;
            }
        }

        private void SaveIQAN_Click(object sender, RoutedEventArgs e)
        {
            //string strPath = @System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\IQ.txt";
            //if (!System.IO.File.Exists(strPath))
            //{
            //    System.IO.File.Create(strPath).Dispose();
            //}
            //using (System.IO.StreamWriter sw = System.IO.File.AppendText(strPath))
            //{
            //    sw.WriteLine((KSadapter.IQArr.Length / 2).ToString() + ";");
            //    for (int i = 0; i < KSadapter.IQArr.Length / 2; i++)
            //    {
            //        sw.WriteLine(KSadapter.IQArr[0 + i * 2].ToString() + ";" + KSadapter.IQArr[1 + i * 2].ToString());
            //    }


            //    sw.Dispose();
            //}
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
        int k = 0;
        
        public void PushResult(ICommandResultPart result)
        {
            k++;
            System.Diagnostics.Debug.WriteLine(k.ToString() + "   ura");
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

            if (result is CMD.Results.RotatorPositionResult)
            {
                var r = (CMD.Results.RotatorPositionResult)result;
                System.Diagnostics.Debug.WriteLine(r.PartIndex + "  " + r.Azimuth_dg + "  " + r.Established + "  " + r.Polarization_dg + "  " + r.Elevation_dg + "  " + r.Status);
            }
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


        public T TakeResult<T>(string key, ulong index, CommandResultStatus status) where T : ICommandResultPart
        {
            object t = new Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureTraceResult()
            {
                Level = new float[10000000]
            };
            return (T)t;
            //throw new NotImplementedException();
        }

        public void ReleaseResult<T>(T result) where T : ICommandResultPart
        {
            throw new NotImplementedException();
        }
    }

    public class DummyGPSExecutionContextMy : IExecutionContext
    {
        private readonly ILogger _logger;

        public DummyGPSExecutionContextMy(ILogger logger)
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
            var myResult = result as Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.GpsResult;
            Debug.WriteLine("Lat " + myResult.Lat.ToString() + "  Lon " + myResult.Lon.ToString() + "   EVL " + myResult.Asl.ToString());
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


        public T TakeResult<T>(string key, ulong index, CommandResultStatus status) where T : ICommandResultPart
        {
            object t = new Atdi.DataModels.Sdrn.DeviceServer.Commands.Results.MesureTraceResult()
            {
                Level = new float[1000000]
            };
            return (T)t;
            //throw new NotImplementedException();
        }

        public void ReleaseResult<T>(T result) where T : ICommandResultPart
        {
            throw new NotImplementedException();
        }
    }
}
