using Atdi.WpfControls.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Atdi.Test.WpfControls.Charts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _count = 10000;
        private Thread _thread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainChart.Adapter = new OnlineMeasLineChartAdapter();
            MainChart.StaticData = GenerateStaticData(_count);
            MainChart.DynamicData = GenerateDynamicData(_count);
            _thread = new Thread(() => Process());
            _thread.Start();
        }

        private void Process()
        {
            try
            {
                while (true)
                {
                    var data = GenerateDynamicData(_count);
                    UIContext(() =>
                    {
                        MainChart.DynamicData = data;
                    });
                    Thread.Sleep(10);
                }
            }
            catch (Exception)
            {

                //throw;
            }
            
            
        }
        private void UIContext(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        private IFastChartData<OnlineMeasLineChartStaticData> GenerateStaticData(int count)
        {
            var container = new OnlineMeasLineChartStaticData()
            {
                Att_dB = -1,
                DetectorType = DataModels.Sdrns.Device.OnlineMeasurement.DetectorType.MaxPeak,
                OnlineMeasType = DataModels.Sdrns.Device.OnlineMeasurement.OnlineMeasType.Level,
                Freq_MHz = GenerateFreqMHz(count),
                Level = GenerateLevel(count),
                PreAmp_dB = -1,
                RefLevel_dBm = 1000000000,
                RBW_kHz = 1,
                SweepTime_s = -1,
                TraceCount = 1,
                TraceType = DataModels.Sdrns.Device.OnlineMeasurement.TraceType.ClearWhrite
            };

            var data = new FastChartData<OnlineMeasLineChartStaticData>(container)
            {
                Title = new TextDescriptor {  Text = $"Online Measurement: {container.OnlineMeasType}", Forecolor = Brushes.DarkBlue },
                LeftTitle = new TextDescriptor { Text = $"Power: {0}", Forecolor = Brushes.DarkGreen },
                LeftLegenda = new TextDescriptor { Text = "Level (dBm)", Forecolor = Brushes.Gray},
                BottomLegenda = new TextDescriptor { Text = "Freq (MHz)", Forecolor = Brushes.Gray },
                LeftLabelSize = 50,
                BottomLabelSize = 50
            };

            return data;
        }

        private IFastChartData<OnlineMeasLineChartDynamicData> GenerateDynamicData(int count)
        {
            var container = new OnlineMeasLineChartDynamicData
            {
                Overload = true,
                Level = GenerateLevel(count)
            };

            var data = new FastChartData<OnlineMeasLineChartDynamicData>(container)
            {

            };

            return data;
        }

        private double[] GenerateFreqMHz(int count)
        {
            var data = new double[count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 935.554 + 0.00001 * i; 
            }
            return data;
        }

        private float[] GenerateLevel(int count)
        {
            var data = new float[count];
            int point1 = count / 4;
            int point2 = count / 2;
            int point3 = point1+point2;
            var r = new Random();
            for (int i = 0; i < point1; i++)
            {
                data[i] =-100+ (float)-5 * (float)r.NextDouble();
            }
            for (int i = point1; i < point2; i++)
            {
                float k = -100 + (-10 + 100) * (i - point1) / (point2 - point1);
                data[i] = k +(float)-10 * (float)r.NextDouble();
            }
            for (int i = point2; i < point3; i++)
            {
                float k = -10 + (10 - 100) * (i - point2) / (point3 - point2);
                data[i] = k + (float)-15 * (float)r.NextDouble();
            }
            for (int i = point3; i < data.Length; i++)
            {
                data[i] = -100 + (float)-20 * (float)r.NextDouble();
            }
            return data;
        }
    }
}
