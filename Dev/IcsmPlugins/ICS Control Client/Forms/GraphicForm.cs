using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.ViewModels;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Forms
{
    public partial class GraphicForm : Form
    {
        private ElementHost _wpfElementHost;
        private SDR.MeasurementResults _measResult;
        private GeneralResultViewModel _generalResult;
        private SDR.MeasurementType _measType;
        public GraphicForm(SDR.MeasurementType measType, SDR.MeasurementResults measResult, GeneralResultViewModel generalResult)
        {
            _measResult = measResult;
            _generalResult = generalResult;
            _measType = measType;
            InitializeComponent();
        }

        private void GrapficForm_Load(object sender, EventArgs e)
        {
            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);

            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\Graphic.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = new GrapficViewModel(_measType, _measResult, _generalResult);
            }
        }
    }
}
