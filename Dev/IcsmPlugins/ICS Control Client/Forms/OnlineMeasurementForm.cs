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
using System.Windows.Markup;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using XICSM.ICSControlClient.ViewModels;
using XICSM.ICSControlClient.Models.Views;

namespace XICSM.ICSControlClient.Forms
{
    

    public partial class OnlineMeasurementForm : Form
    {
        private readonly ShortSensorViewModel _sensor;
        private ElementHost _wpfElementHost;
        private OnlineMeasurementViewModel _viewModel;

        public OnlineMeasurementForm(ShortSensorViewModel sensor)
        {
            this._sensor = sensor;
            this._viewModel = new OnlineMeasurementViewModel(this._sensor);
            InitializeComponent();
            this.Text = $"ICS Control Client - Online Measurement - Sensor ID #{sensor.Id} '{_sensor.Name}'";
        }

        private void OnlineMeasurementForm_Load(object sender, EventArgs e)
        {
            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);

            try
            {
                var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

                var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\OnlineMeasurementForm.xaml");
                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                    
                    (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = this._viewModel;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnlineMeasurementForm_Load: {ex.ToString()}");
                throw;
            }

            
        }

        private void OnlineMeasurementForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void OnlineMeasurementForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this._viewModel != null)
            {
                try
                {
                    this._viewModel.Dispose();
                    this._viewModel = null;

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"OnlineMeasurementForm_FormClosed: {ex.ToString()}");
                    throw;
                }
            }
        }
    }
}
