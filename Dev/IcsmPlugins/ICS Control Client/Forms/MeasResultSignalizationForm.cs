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
using System.Windows.Media;
using XICSM.ICSControlClient.ViewModels;
using XICSM.ICSControlClient.WpfControls.Maps;
using UserControl = System.Windows.Controls.UserControl;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.Forms
{
    public partial class MeasResultSignalizationForm : Form
    {
        private long _resultId;
        private int _startType;
        private SDR.Emitting[] _emittings;
        private ElementHost _wpfElementHost;
        private MeasResultSignalizationViewModel _model;
        private DateTime? _timeMeas;

        public MeasResultSignalizationForm(long resultId, int startType, SDR.Emitting[] emittings, DateTime? timeMeas)
        {
            _resultId = resultId;
            _startType = startType;
            _emittings = emittings;
            _timeMeas = timeMeas;
            InitializeComponent();
        }
        private void MeasResultSignalizationForm_Load(object sender, EventArgs e)
        {

            _wpfElementHost = new ElementHost();
            _wpfElementHost.Dock = DockStyle.Fill;
            this.Controls.Add(_wpfElementHost);

            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\MeasResultSignalizationForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                this._model = new MeasResultSignalizationViewModel(_resultId, _startType, _emittings, _timeMeas);
                ((UserControl) this._wpfElementHost.Child).DataContext = _model;
            }
        }

        private void MeasResultSignalizationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _model.Dispose();

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
