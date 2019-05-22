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

namespace XICSM.ICSControlClient.Forms
{
    
    

    public partial class MeasResultSignalizationViewForm : Form
    {
        private ElementHost _wpfElementHost;

        public MeasResultSignalizationViewForm(int[] stations, string tableName, string title)
        {
            InitializeComponent();
            if (null == System.Windows.Application.Current)
            {
                new System.Windows.Application();
            }

            this.Text = title;

            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);


            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\MeasResultSignalizationViewForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = new MeasResultSignalizationViewViewModel(stations, tableName);
            }
        }
    }
}
