using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace IQFilters
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly string decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        public MainWindow()
        {
            InitializeComponent();
        }
        filerType fType = filerType.LOWPASS;
        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            float[] iq = new float[(IQDraw.HMaxDraw - IQDraw.HMinDraw)*2];
            Array.Copy(App.IQArr, IQDraw.HMinDraw * 2, iq, 0, (IQDraw.HMaxDraw - IQDraw.HMinDraw) * 2);

            float f1 = float.Parse(F1.Text.Replace(",", decimalSeparator).Replace(".", decimalSeparator));
            float f2 = float.Parse(F2.Text.Replace(",", decimalSeparator).Replace(".", decimalSeparator));
            float band = float.Parse(Band.Text.Replace(",", decimalSeparator).Replace(".", decimalSeparator));
            float att = float.Parse(ATT.Text.Replace(",", decimalSeparator).Replace(".", decimalSeparator));
            float ripple = (float)double.Parse(Ripple.Text.Replace(",", decimalSeparator).Replace(".", decimalSeparator));
            byte filerType = (byte)fType;
            int firOrder;
            //float sampleRate = 338750.0f;
            float sampleRate = 1562500.0f;

            float trbandNormalized = band / sampleRate;
            //Свой метод сюда
            InitializeRemez FIR = new InitializeRemez();
            //int firOrder = FIR_example.estimatedOrder(trbandNormalized, att, ripple);

            //FIR.init(1, 0, 12500, f2, 1355, sampleRate, 20, 1.1f);
            FIR.init(filerType, 0, f1, f2, band, sampleRate, att, ripple);
            //FIR.init(4, 0, 281250.0f, 481250.0f, 13550.0f, 1562500.0f, 30.0f, 0.8f);

            //FIR.init(4, 0, 4000.0f, 5000.0f, 100.0f, 44000.0f, 20.0f, 0.5f);
            //FIR.init(4, 0, 245750.0f, 516750.0f, 20000, 1562500, 20, 1.1f);
            //float[] hS = new float[FIR.h.Length];// = FIR.h;
            float[] hS = new float[FIR.h.Length];

            string hSToPy = " ";
            for (int i=0; i < FIR.h.Length; i++)
            {
                hSToPy += FIR.h[i].ToString() + ", ";
                hS[i] = (float)FIR.h[i];
            }


            int streamLength = iq.Length / 2;
            //float[] iStream = new float[streamLength];
            //float[] qStream = new float[streamLength];
            //int j = 0;
            //for (int i = 0; i < iq.Length; i++)
            //{
            //    if (i % 2 != 0)
            //    {
            //        qStream[j] = iq[i];
            //        j++;
            //    }
            //    else
            //    {
            //        iStream[j] = iq[i];
            //    }
            //}
            float[] iqFiltered = new float[iq.Length];

            int midTap = hS.Length / 2;
            //float[] iFiltered = new float[streamLength];
            //float[] qFiltered = new float[streamLength];

            int delayCompensation = streamLength + midTap;
            for (int n = midTap; n < delayCompensation; n++)
            {
                for (int i = 0; i < hS.Length - 1; i++)
                {
                    if ((n - i) >= 0)
                    {
                        if ((n - i) <= streamLength - 1)
                        {
                            //iFiltered[n - midTap] += hS[i] * iStream[n - i];
                            //qFiltered[n - midTap] += hS[i] * qStream[n - i];
                            iqFiltered[2 * (n - midTap)] += hS[i] * iq[2 * (n - i) ];
                            iqFiltered[2 * (n - midTap) + 1] += hS[i] * iq[2 * (n - i) + 1];

                        }
                        else
                        {
                            //iFiltered[n - midTap] += hS[i] * iStream[streamLength - 1];
                            //qFiltered[n - midTap] += hS[i] * qStream[streamLength - 1];
                            iqFiltered[2 * (n - midTap)] += hS[i] * iq[2 * (streamLength - 1)];
                            iqFiltered[2 * (n - midTap) + 1] += hS[i] * iq[2 * (streamLength - 1) + 1];
                        }
                    }
                    else
                    {
                        //iFiltered[n - midTap] += hS[i] * iStream[0];
                        //qFiltered[n - midTap] += hS[i] * qStream[0];
                        iqFiltered[2 * (n - midTap)] += hS[i] * iq[0];
                        iqFiltered[2 * (n - midTap) + 1] += hS[i] * iq[1];

                    }
                }
            }

            //float[] iqFiltered = new float[iq.Length];
            //float[] iqDelta = new float[iq.Length];
            //for (int i = 0; i < streamLength; i++)
            //{
            //    iqFiltered[2 * i] = iFiltered[i];
            //    iqFiltered[2 * i + 1] = qFiltered[i];

            //    iqDelta[2 * i] = iqFiltered[2 * i] - iq[2 * i];
            //    iqDelta[2 * i + 1] = iqFiltered[2 * i + 1] - iq[2 * i + 1];
            //}


            //В IQDraw.IQFiltered = присваивать результат фильтрации
            IQDraw.IQFiltered = iqFiltered;
        }
        #region
        private void FilterType_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data = Enum.GetNames(typeof(filerType)).ToList();

            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = data;
            // дефаултное значение
            comboBox.SelectedIndex = 0;
        }
        private void FilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            // узнаем выбранное значение
            Enum.TryParse(comboBox.SelectedItem as string, out fType);
            //port.Text = value;
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    //public enum windowType : byte
    //{
    //    RECTANGULAR = 0,
    //    BARTLETT = 1,
    //    HANNING = 2,
    //    HAMMING = 3,
    //    BLACKMAN = 4,
    //    BLACKMAN_HARRIS = 5,
    //    BLACKMAN_NUTTAL = 6,
    //    NUTTAL = 7
    //}

    public enum filerType : byte
    {
        LOWPASS = 1,
        HIGHPASS = 2,
        BANDSTOP = 3,
        BANDPASS = 4
    }

}
