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
using System.Windows.Media;
using XICSM.ICSControlClient.ViewModels;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.WpfControls.Maps;

namespace XICSM.ICSControlClient.Forms
{
    

    public partial class OnlineMeasurementForm : Form
    {
        private readonly ShortSensorViewModel _sensor;
        private ElementHost _wpfElementHost;
        private OnlineMeasurementViewModel _viewModel;
        private OnlineMeasurementParameters _param;

        public OnlineMeasurementForm(ShortSensorViewModel sensor, OnlineMeasurementParameters param)
        {
            this._sensor = sensor;
            this._viewModel = new OnlineMeasurementViewModel(this._sensor, this._param);
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

                    var maps = FindVisualChildren<Map>(_wpfElementHost.Child);
                    foreach (var map in maps)
                    {
                        map.Dispose();
                    }
                    if (_wpfElementHost.Child is FrameworkElement fe)
                    {
                        // Memory leak workaround: elementHost.Child.SizeChanged -= elementHost.childFrameworkElement_SizeChanged;
                        var handler = (SizeChangedEventHandler)Delegate.CreateDelegate(typeof(SizeChangedEventHandler), _wpfElementHost, "childFrameworkElement_SizeChanged");
                        fe.SizeChanged -= handler;
                    }
                    _wpfElementHost.Visible = false;
                    _wpfElementHost.Child = null;
                    _wpfElementHost.Dispose();
                    _wpfElementHost.Parent = null;

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"OnlineMeasurementForm_FormClosed: {ex.ToString()}");
                    throw;
                }
            }
        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
