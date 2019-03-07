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
        ADP.SignalHound.Adapter adapter;

        public MainWindow()
        {
            InitializeComponent();
            logger = new ConsoleLogger();
            adapterHost = new DummyAdapterHost(logger);
            TimeService = new DummyTimeService();

            // конфигурация
            var adapterConfig = new ADP.SignalHound.AdapterConfig()
            {
                //AntennaCodeTest = "123",
                Prop1 = 1,
                Prop2 = 2,
                Prop3 = 3,
                Prop4 = 4,
                Prop5 = 5
            };

            adapter = new ADP.SignalHound.Adapter(adapterConfig, logger);
            //adapter = new ADP.SignalHound.Adapter(adapterConfig, logger, TimeService);
            DS.Adapter = adapter;


        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            adapter.Connect(adapterHost);
        }
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            adapter.Disconnect();
        }

        private void SetMeas1_Click(object sender, RoutedEventArgs e)
        {
            // send command
            var context = new DummyExecutionContext(logger);
            var command = new CMD.MesureTraceCommand();
            command.Parameter.Att_dB = -1;
            command.Parameter.FreqStart_Hz = 930 * 1000000;
            command.Parameter.FreqStop_Hz = 950 * 1000000;
            command.Parameter.PreAmp_dB = -1;
            command.Parameter.RBW_Hz = -1;
            command.Parameter.VBW_Hz = -1;
            command.Parameter.RefLevel_dBm = -1;
            command.Parameter.SweepTime_s = 0.001;
            command.Parameter.TraceCount = 1;
            command.Parameter.TracePoint = int.Parse(points.Text);
            command.Parameter.TraceType = CMD.Parameters.TraceType.ClearWhrite;
            command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
            command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

            adapter.MesureTraceCommandHandler(command, context);


        }
        private void SetMeas2_Click(object sender, RoutedEventArgs e)
        {
            // send command
            var context = new DummyExecutionContext(logger);
            var command = new CMD.MesureTraceCommand();
            command.Parameter.Att_dB = 0;
            command.Parameter.FreqStart_Hz = 1800000000;
            command.Parameter.FreqStop_Hz = 1900000000;
            command.Parameter.PreAmp_dB = 0;
            command.Parameter.RBW_Hz = 30000;
            command.Parameter.VBW_Hz = 30000;
            command.Parameter.RefLevel_dBm = -40;
            command.Parameter.SweepTime_s = 0.00001;
            command.Parameter.TraceCount = 100;
            command.Parameter.TracePoint = 1000;
            command.Parameter.TraceType = CMD.Parameters.TraceType.MaxHold;
            command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
            command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBmkV;

            adapter.MesureTraceCommandHandler(command, context);


        }
        private void SetMeas21_Click(object sender, RoutedEventArgs e)
        {
            // send command
            var context = new DummyExecutionContext(logger);
            var command = new CMD.MesureTraceCommand();
            command.Parameter.Att_dB = 0;
            command.Parameter.FreqStart_Hz = 90000000;
            command.Parameter.FreqStop_Hz = 110000000;
            //command.Parameter.FreqStart_Hz = 1800000000;
            //command.Parameter.FreqStop_Hz = 1900000000;
            command.Parameter.PreAmp_dB = 20;
            command.Parameter.RBW_Hz = 30000;
            command.Parameter.VBW_Hz = 30000;
            command.Parameter.RefLevel_dBm = -40;
            command.Parameter.SweepTime_s = 0.00001;
            command.Parameter.TraceCount = 100;
            command.Parameter.TracePoint = 1000;
            command.Parameter.TraceType = CMD.Parameters.TraceType.MinHold;
            command.Parameter.DetectorType = CMD.Parameters.DetectorType.Average;
            command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBmkV;

            adapter.MesureTraceCommandHandler(command, context);


        }
        private void SetMeas22_Click(object sender, RoutedEventArgs e)
        {
            // send command
            var context = new DummyExecutionContext(logger);
            var command = new CMD.MesureTraceCommand();
            command.Parameter.Att_dB = 0;
            command.Parameter.FreqStart_Hz = 90000000;
            command.Parameter.FreqStop_Hz = 110000000;
            //command.Parameter.FreqStart_Hz = 1800000000;
            //command.Parameter.FreqStop_Hz = 1900000000;
            command.Parameter.PreAmp_dB = 20;
            command.Parameter.RBW_Hz = 30000;
            command.Parameter.VBW_Hz = 30000;
            command.Parameter.RefLevel_dBm = -40;
            command.Parameter.SweepTime_s = 0.00001;
            command.Parameter.TraceCount = 100;
            command.Parameter.TracePoint = 1000;
            command.Parameter.TraceType = CMD.Parameters.TraceType.Average;
            command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
            command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

            adapter.MesureTraceCommandHandler(command, context);


        }

        private void GetIQ1_Click(object sender, RoutedEventArgs e)
        {
            var context = new DummyExecutionContext(logger);
            var command = new CMD.MesureIQStreamCommand();

            command.Parameter.FreqStart_Hz = 910 * 1000000;//910 * 1000000;//424.625m * 1000000;//424.650
            command.Parameter.FreqStop_Hz = 930 * 1000000;//930*1000000;//424.675m * 1000000;
            command.Parameter.Att_dB = 0;
            command.Parameter.PreAmp_dB = 10;
            command.Parameter.RefLevel_dBm = -40;
            command.Parameter.BitRate_MBs = 40;
            command.Parameter.IQBlockDuration_s = 0.5;
            command.Parameter.IQReceivTime_s = 0.6;
            command.Parameter.MandatoryPPS = false;
            command.Parameter.MandatorySignal = true;
            command.Parameter.TimeStart = (DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks);
            command.Parameter.TimeStart += 2 * 10000000;
            Debug.WriteLine("\r\n" + new TimeSpan(command.Parameter.TimeStart).ToString() + " Set Param");
            adapter.MesureIQStreamCommandHandler(command, context);
        }



    }
    public class DummyTimeService : ITimeService
    {
        private readonly ITimeStamp _timeStamp;

        public DummyTimeService()
        {
            this._timeStamp = new TimeStamp();
        }

        public ITimeStamp TimeStamp => this._timeStamp;
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
    }
}
