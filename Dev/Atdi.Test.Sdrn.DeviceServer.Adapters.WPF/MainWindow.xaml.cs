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

        private void SetMeas_Click(object sender, RoutedEventArgs e)
        {
            // send command
            var context = new DummyExecutionContext(logger);
            var command = new CMD.MesureTraceCommand();
            command.Parameter.Att_dB = 0;
            command.Parameter.FreqStart_Hz = 90000000;
            command.Parameter.FreqStop_Hz = 110000000;
            command.Parameter.PreAmp_dB = 0;
            command.Parameter.RBW_Hz = 1000;
            command.Parameter.VBW_Hz = 1000;
            command.Parameter.RefLevel_dBm = -40;
            command.Parameter.SweepTime_s = 0.001;
            command.Parameter.TraceCount = 10;
            command.Parameter.TracePoint = 1000;

            adapter.MesureTraceParameterHandler(command, context);

            
            
        }

        
    }
}
