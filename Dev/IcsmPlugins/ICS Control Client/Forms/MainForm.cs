using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using XICSM.ICSControlClient.WpfControls;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using XICSM.ICSControlClient.ViewModels;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Controls;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Models;

namespace XICSM.ICSControlClient.Forms
{

    public partial class MainForm : Form
    {
        private ElementHost _wpfElementHost;
        //private MainFormWpfControl _wpfControl;

        public MainForm()
        {
            InitializeComponent();

            if (null == System.Windows.Application.Current)
            {
                new System.Windows.Application();
            }

            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);


            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\ICSControlClient.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = new ControlClientViewModel(DataStore.GetStore());
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
            //_wpfControl = new MainFormWpfControl();
            //this._wpfElementHost.Child = _wpfControl;
        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row != null)
            {
                //var data = row.Item as ShortMeasTaskViewModel;
                //System.Windows.MessageBox.Show(data.Id.ToString());
            }
        }
    }
}
