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
using XICSM.ICSControlClient.Models;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using XICSM.ICSControlClient.ViewModels;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using XICSM.ICSControlClient.Models.Views;

namespace XICSM.ICSControlClient.Forms
{
    public partial class SignalizationSensorsForm : Form
    {
        private ElementHost _wpfElementHost;
        public SignalizationSensorsForm()
        {
            InitializeComponent();
        }

        private void SignalizationSensorsForm_Load(object sender, EventArgs e)
        {
            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);


            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\SignalizationSensorsForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = new SignalizationSensorsViewModel();
            }
        }
    }
}
