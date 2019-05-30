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
                SerialNumber = "16319373"
            };

            adapter = new ADP.SignalHound.Adapter(adapterConfig, logger, TimeService);
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
            command.Parameter.Att_dB = 10;
            command.Parameter.FreqStart_Hz = 934.85m * 1000000;
            command.Parameter.FreqStop_Hz = 935.55m * 1000000;
            command.Parameter.PreAmp_dB = 30;
            command.Parameter.RBW_Hz = -1;
            command.Parameter.VBW_Hz = -1;
            command.Parameter.RefLevel_dBm = -40;
            command.Parameter.SweepTime_s = 0.001;
            command.Parameter.TraceCount = 100000;
            command.Parameter.TracePoint = int.Parse(points.Text);
            command.Parameter.TraceType = CMD.Parameters.TraceType.MaxHold;
            command.Parameter.DetectorType = CMD.Parameters.DetectorType.MaxPeak;
            command.Parameter.LevelUnit = CMD.Parameters.LevelUnit.dBm;

            adapter.MesureTraceCommandHandler(command, context);


        }
        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            int ind = FindMarkerIndOnTrace(adapter.FreqArr, (double)adapter.FreqCentr);
            double lev = Math.Abs(adapter.LevelArr[ind]);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(AppPath + "\\"+ adapter.FreqCentr.ToString() + ".txt", false))
            {
                for (int i = 0; i < adapter.FreqArr.Length; i++)
                {
                    string str = (adapter.FreqArr[i] - (double)adapter.FreqCentr).ToString().Replace(".", ",") + ";" +
                        (lev + (double)adapter.LevelArr[i]).ToString().Replace(".", ",");
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
            command.Parameter.Att_dB = 10;
            command.Parameter.PreAmp_dB = 10;
            command.Parameter.RefLevel_dBm = -40;
            command.Parameter.BitRate_MBs = 40;
            command.Parameter.IQBlockDuration_s = 0.5;
            command.Parameter.IQReceivTime_s = 0.6;
            command.Parameter.MandatoryPPS = true;
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
