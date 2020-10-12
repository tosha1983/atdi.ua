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
    /// Логика взаимодействия для AllLicenseAgreement.xaml
    /// </summary>
    public partial class AllLicenseAgreement : UserControl
    {
        public AllLicenseAgreement()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            CloseUIControl();
        }
        private void CloseUIControl()
        {
            //Grid gr = (Grid)this.Parent;
            ((SplashWindow)App.Current.MainWindow).m_mainWindow.CloseGlobalUC(this);

            //gr.Children.Remove(this);
        }
    }
}
