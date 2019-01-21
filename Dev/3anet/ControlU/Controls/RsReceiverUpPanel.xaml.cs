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
    /// Логика взаимодействия для RsReceiverUpPanel.xaml
    /// </summary>
    public partial class RsReceiverUpPanel : UserControl
    {
        Equipment.RsReceiver_v2 rcv = MainWindow.Rcvr;
        public RsReceiverUpPanel()
        {
            InitializeComponent();
            this.DataContext = rcv;
        }
        private void DemodBWDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.DemodBWInd--;
            rcv.SetDemodBWFromIndex(rcv.DemodBWInd);
        }
        private void DemodBWUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.DemodBWInd++;
            rcv.SetDemodBWFromIndex(rcv.DemodBWInd);
        }

        private void DemodDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.DemodInd--;
            rcv.SetDemodFromIndex(rcv.DemodInd);
        }
        private void DemodUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.DemodInd++;
            rcv.SetDemodFromIndex(rcv.DemodInd);
        }

        private void AFC_Click(object sender, RoutedEventArgs e)
        {
            rcv.AFCState = !rcv.AFCState;
            rcv.SetAFC(rcv.AFCState);            
        }

        private void DetectorDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.DetectorInd--;
            rcv.SetDetectorFromIndex(rcv.DetectorInd);
        }
        private void DetectorUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.DetectorInd++;
            rcv.SetDetectorFromIndex(rcv.DetectorInd);
        }

        private void ATTDn_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.UniqueData.ATTFix == false)
            {
                rcv.ATT--;
                rcv.SetATT(rcv.ATT);
            }
        }
        private void ATTUp_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.UniqueData.ATTFix == false)
            {
                rcv.ATT++;
                rcv.SetATT(rcv.ATT);
            }
        }
        private void ATTAuto_Click(object sender, RoutedEventArgs e)
        {
            if (rcv.UniqueData.ATTFix == true)
            {
                rcv.ATTFixState = !rcv.ATTFixState;
                rcv.SetATTFixState(rcv.ATTFixState);
            }
            else if (rcv.UniqueData.ATTFix == false)
            {
                rcv.ATTAutoState = !rcv.ATTAutoState;
                rcv.SetATTAutoState(rcv.ATTAutoState);
            }
                //rcv.TelnetDM += rcv.SetATTState;
        }

        private void MGCDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.MGC--;
            rcv.SetMGC(rcv.MGC);
            //rcv.TelnetDM += rcv.SetMGCDn;
        }
        private void MGCUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.MGC++;
            rcv.SetMGC(rcv.MGC);
            //rcv.TelnetDM += rcv.SetMGCUp;
        }
        private void MGCAuto_Click(object sender, RoutedEventArgs e)
        {
            rcv.MGCAutoState = !rcv.MGCAutoState;
            rcv.SetMGCAutoState(rcv.MGCAutoState);
            //rcv.TelnetDM += rcv.SetMGCAutoState;
        }

        private void SQUDn_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetSQUDn;
        }
        private void SQUUp_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetSQUUp;
        }
        private void SQUState_Click(object sender, RoutedEventArgs e)
        {
            rcv.TelnetDM += rcv.SetSQUState;
        }


        private void SquelchValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int squ = (int)((Slider)sender).Value;
            rcv.SQU = squ;
            rcv.TelnetDM += rcv.SetSQU;
        }
    }
}
