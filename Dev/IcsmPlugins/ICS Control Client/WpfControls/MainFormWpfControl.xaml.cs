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
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using XICSM.ICSControlClient.Environment.Wpf;
using VM = XICSM.ICSControlClient.Models.Views;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using ADS = XICSM.ICSControlClient.Models.WcfDataApadters;
using XICSM.ICSControlClient.ViewModels;
using XICSM.ICSControlClient.Models;

namespace XICSM.ICSControlClient.WpfControls
{
    /// <summary>
    /// Interaction logic for MainFormWpfControl.xaml
    /// </summary>
    public partial class MainFormWpfControl : UserControl
    {
        public MainFormWpfControl()
        {
            InitializeComponent();
            this.DataContext = new ControlClientViewModel(DataStore.GetStore());
        }
    }
}
