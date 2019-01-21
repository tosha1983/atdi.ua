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

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для DFPanel.xaml
    /// </summary>
    public partial class DFPanel : UserControl
    {
        Equipment.RsReceiver_v2 rcv = MainWindow.Rcvr;
        public DFPanel()
        {
            InitializeComponent();
            this.DataContext = rcv;
        }
        private void DFSquelchValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rcv.DFSquelchValueSliderState = true;
            rcv.TelnetDM += rcv.SetDFSQUFromSlider;
        }
        private void BearingDrawMode_Click(object sender, RoutedEventArgs e)
        {
            rcv.BearingDrawMode = !rcv.BearingDrawMode;
        }
    }
}
