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
using System.Windows.Media;
using XICSM.ICSControlClient.ViewModels;
using XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Forms
{
    public partial class MeasStationsSignalizationForm : Form
    {
        private SDR.MeasurementResults _measResult;
        private MeasStationsSignalization[] _stationData;
        private bool _buttonAssociatedVisible;
        private long? _emittingId;
        private ElementHost _wpfElementHost;
        private MeasStationsSignalizationFormViewModel _model;
        public MeasStationsSignalizationForm(MeasStationsSignalization[] stationData, SDR.MeasurementResults measResult, bool buttonAssociatedVisible, long? emittingId, string captionAdd)
        {
            this._stationData = stationData;
            this._measResult = measResult;
            this._buttonAssociatedVisible = buttonAssociatedVisible;
            this._emittingId = emittingId;
            InitializeComponent();
            this.Text = this.Text + captionAdd;
        }

        private void MeasStationsSignalizationForm_Load(object sender, EventArgs e)
        {
            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);


            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\MeasStationsSignalizationForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                this._model = new MeasStationsSignalizationFormViewModel(this._stationData, this._measResult, this._buttonAssociatedVisible, this._emittingId) { _form = this };
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = this._model;
            };

        }

        private void MeasStationsSignalizationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
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
            _model._form = null;
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
