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

namespace ControlU.Controls.Map
{
    /// <summary>
    /// Логика взаимодействия для DF_Bearing.xaml
    /// </summary>
    public partial class DF_Bearing : UserControl
    {
        public DF_Bearing()
        {
            InitializeComponent();
            selfWatcher.ChangeTarget(this);
            selfWatcher.ChangeTarget(this);
            selfWatcher.Changed += RecalcLine;
            fromWatcher.Changed += RecalcLine;
            toWatcher.Changed += RecalcLine;
            RecalcLine(null, null);
        }
        LayoutWatcher selfWatcher = new LayoutWatcher(),
                      fromWatcher = new LayoutWatcher(),
                      toWatcher = new LayoutWatcher();

        #region dp FrameworkElement From
        public FrameworkElement From
        {
            get { return (FrameworkElement)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(FrameworkElement), typeof(DF_Bearing),
                new FrameworkPropertyMetadata((o, args) =>
                { var self = (DF_Bearing)o; self.fromWatcher.ChangeTarget(self.From); }));
        #endregion

        #region dp FrameworkElement To
        public FrameworkElement To
        {
            get { return (FrameworkElement)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(FrameworkElement), typeof(DF_Bearing),
                new FrameworkPropertyMetadata((o, args) =>
                { var self = (DF_Bearing)o; self.toWatcher.ChangeTarget(self.To); }));
        #endregion

        void RecalcLine(object sender, LayoutChangeEventArgs e)
        {
            if (From == null || To == null)
            {
                ConnectingLine.Visibility = Visibility.Collapsed;
                return;
            }

            ConnectingLine.Visibility = Visibility.Visible;

            var fromRect = LayoutWatcher.ComputeRenderRect(From, this);
            var toRect = LayoutWatcher.ComputeRenderRect(To, this);

            ConnectingLine.X1 = fromRect.Right;
            ConnectingLine.Y1 = fromRect.Top + fromRect.Height / 2;

            ConnectingLine.X2 = toRect.Left;
            ConnectingLine.Y2 = toRect.Top + toRect.Height / 2;
        }
    }
    public class LayoutChangeEventArgs : EventArgs
    {
        public readonly Rect Rect;
        public LayoutChangeEventArgs(Rect rect) { Rect = rect; }
    }

    public class LayoutWatcher
    {
        public void ChangeTarget(UIElement target, UIElement origin = null)
        {
            if (this.target != null)
                this.target.LayoutUpdated -= OnLayoutUpdate;

            this.target = target;
            this.origin = origin;
            OnLayoutUpdate(null, null);

            if (this.target != null)
                target.LayoutUpdated += OnLayoutUpdate;
        }

        void OnLayoutUpdate(object sender, EventArgs e)
        {
            var newRenderRect = GetRenderRect();
            if (newRenderRect != currRenderRect)
            {
                currRenderRect = newRenderRect;
                FireChanged();
            }
        }

        UIElement target, origin;
        Rect currRenderRect = Rect.Empty;

        public static Rect ComputeRenderRect(UIElement target, UIElement origin) =>
            new Rect(target.TranslatePoint(new Point(), origin), target.RenderSize);

        Rect GetRenderRect() => ComputeRenderRect(target, origin);

        void FireChanged() => Changed?.Invoke(target, new LayoutChangeEventArgs(currRenderRect));

        public event EventHandler<LayoutChangeEventArgs> Changed;
    }
}
