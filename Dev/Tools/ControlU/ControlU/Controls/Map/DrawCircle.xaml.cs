using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для DrawCircle.xaml
    /// </summary>
    public partial class DrawCircle : UserControl
    {
        public DrawCircle()
        {
            InitializeComponent();
            selfWatcher.ChangeTarget(this);

            selfWatcher.Changed += RecalcCircle;
            fromWatcher.Changed += RecalcCircle;
            toWatcher.Changed += RecalcCircle;
            RecalcCircle(null, null);
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
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(FrameworkElement), typeof(DrawCircle),
                new FrameworkPropertyMetadata((o, args) => { var self = (DrawCircle)o; self.fromWatcher.ChangeTarget(self.From); }));
        #endregion

        #region dp FrameworkElement To
        public FrameworkElement To
        {
            get { return (FrameworkElement)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(FrameworkElement), typeof(DrawCircle),
                new FrameworkPropertyMetadata((o, args) => { var self = (DrawCircle)o; self.toWatcher.ChangeTarget(self.To); }));
        #endregion

        void RecalcCircle(object sender, LayoutChangeEventArgs e)
        {
            if (From == null || To == null)
            {
                Circle.Visibility = Visibility.Collapsed;
                return;
            }

            Circle.Visibility = Visibility.Visible;

            var fromRect = LayoutWatcher.ComputeRenderRect(From, this);
            var toRect = LayoutWatcher.ComputeRenderRect(To, this);

            Circle.Width = Math.Abs((toRect.Location.X - fromRect.Location.X) * 2);
            Circle.Height = Math.Abs((toRect.Location.X - fromRect.Location.X) * 2);

            TranslateTransform tt = new TranslateTransform(fromRect.Location.X - Circle.Width / 2, fromRect.Location.Y - Circle.Width / 2);
            Circle.RenderTransform = tt;

        }
    }
}
