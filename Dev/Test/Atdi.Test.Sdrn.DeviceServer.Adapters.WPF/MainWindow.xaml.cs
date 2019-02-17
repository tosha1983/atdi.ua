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
        ConsoleLogger logger;
        DummyAdapterHost adapterHost;
        ADP.SignalHound.Adapter adapter;
        public MainWindow()
        {
            InitializeComponent();
            logger = new ConsoleLogger();
            adapterHost = new DummyAdapterHost(logger);

            
            // конфигурация
            var adapterConfig = new ADP.SignalHound.AdapterConfig()
            {
                Prop1 = 1,
                Prop2 = 2,
                Prop3 = 3,
                Prop4 = 4,
                Prop5 = 5
            };
            adapter = new ADP.SignalHound.Adapter(adapterConfig, logger);
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
            command.Parameter.Att_dB = 0;
            command.Parameter.FreqStart_Hz = 90000000;
            command.Parameter.FreqStop_Hz = 110000000;
            command.Parameter.PreAmp_dB = 20;
            command.Parameter.RBW_Hz = 30000;
            command.Parameter.VBW_Hz = 30000;
            command.Parameter.RefLevel_dBm = -40;
            command.Parameter.SweepTime_s = 0.00001;
            command.Parameter.TraceCount = 10;
            command.Parameter.TracePoint = 1000;
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
            
            command.Parameter.FreqStart_Hz = 424.625m * 1000000;//424.650
            command.Parameter.FreqStop_Hz = 424.675m * 1000000;
            command.Parameter.Att_dB = 0;
            command.Parameter.PreAmp_dB = 30;
            command.Parameter.RefLevel_dBm = -40;
            command.Parameter.BitRate_MBs = 0.8;
            command.Parameter.IQBlockDuration_s = 0.5;
            command.Parameter.IQReceivTime_s = 5;
            command.Parameter.MandatoryPPS = false;
            command.Parameter.MandatorySignal = true;
            command.Parameter.TimeStart = (DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks);
            command.Parameter.TimeStart += 2*10000000;
            Debug.WriteLine("\r\n" + new TimeSpan(DateTime.Now.Ticks).ToString() + " Set Param");
            adapter.MesureIQStreamCommandHandler(command, context);
        }
    }
}
