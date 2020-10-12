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
    /// Логика взаимодействия для Cluster.xaml
    /// </summary>
    public partial class Cluster : UserControl
    {
        public Cluster()
        {
            InitializeComponent();
            set();
        }
        public double LeftValue
        {
            get { return (double)GetValue(LeftValueProperty); }
            set { SetValue(LeftValueProperty, value); }
        }
        public double LeftMinimum
        {
            get { return (double)GetValue(LeftMinimumProperty); }
            set { SetValue(LeftMinimumProperty, value); }
        }
        public double LeftMaximum
        {
            get { return (double)GetValue(LeftMaximumProperty); }
            set { SetValue(LeftMaximumProperty, value); }
        }
        public double RightValue
        {
            get { return (double)GetValue(RightValueProperty); }
            set { SetValue(RightValueProperty, value); }
        }
        public double RightMinimum
        {
            get { return (double)GetValue(RightMinimumProperty); }
            set { SetValue(RightMinimumProperty, value); }
        }
        public double RightMaximum
        {
            get { return (double)GetValue(LeftMaximumProperty); }
            set { SetValue(LeftMaximumProperty, value); }
        }
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public Brush LeftSegmentColor
        {
            get { return (Brush)GetValue(LeftSegmentColorProperty); }
            set { SetValue(LeftSegmentColorProperty, value); }
        }
        public Brush LeftSegmentColorFull
        {
            get { return (Brush)GetValue(LeftSegmentColorFullProperty); }
            set { SetValue(LeftSegmentColorFullProperty, value); }
        }
        public Brush RightSegmentColor
        {
            get { return (Brush)GetValue(RightSegmentColorProperty); }
            set { SetValue(RightSegmentColorProperty, value); }
        }
        public Brush RightSegmentColorFull
        {
            get { return (Brush)GetValue(RightSegmentColorFullProperty); }
            set { SetValue(RightSegmentColorFullProperty, value); }
        }
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public Brush SelectedColor
        {
            get { return (Brush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        public string TextCentr
        {
            get { return (string)GetValue(TextCentrProperty); }
            set { SetValue(TextCentrProperty, value); }
        }

        private double angleL = 0;
        private double angleR = 0;

        public static readonly DependencyProperty LeftValueProperty =
          DependencyProperty.Register("LeftValue", typeof(double), typeof(Cluster), new PropertyMetadata(65d, new PropertyChangedCallback(OnPropertyChanged)));
        public static readonly DependencyProperty LeftMinimumProperty =
           DependencyProperty.Register("LeftMinimum", typeof(double), typeof(Cluster), new PropertyMetadata(0d, new PropertyChangedCallback(OnPropertyChanged)));
        public static readonly DependencyProperty LeftMaximumProperty =
            DependencyProperty.Register("LeftMaximum", typeof(double), typeof(Cluster), new PropertyMetadata(100d, new PropertyChangedCallback(OnPropertyChanged)));

        public static readonly DependencyProperty RightValueProperty =
          DependencyProperty.Register("RightValue", typeof(double), typeof(Cluster), new PropertyMetadata(65d, new PropertyChangedCallback(OnPropertyChanged)));
        public static readonly DependencyProperty RightMinimumProperty =
          DependencyProperty.Register("RightMinimum", typeof(double), typeof(Cluster), new PropertyMetadata(0d, new PropertyChangedCallback(OnPropertyChanged)));
        public static readonly DependencyProperty RightMaximumProperty =
            DependencyProperty.Register("RightMaximum", typeof(double), typeof(Cluster), new PropertyMetadata(100d, new PropertyChangedCallback(OnPropertyChanged)));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Cluster), new PropertyMetadata(100d, new PropertyChangedCallback(OnPropertyChanged)));

        public static readonly DependencyProperty LeftSegmentColorProperty =
            DependencyProperty.Register("LeftSegmentColor", typeof(Brush), typeof(Cluster), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        public static readonly DependencyProperty LeftSegmentColorFullProperty =
            DependencyProperty.Register("LeftSegmentColorFull", typeof(Brush), typeof(Cluster), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, (byte)100, (byte)255, (byte)100))));

        public static readonly DependencyProperty RightSegmentColorProperty =
           DependencyProperty.Register("RightSegmentColor", typeof(Brush), typeof(Cluster), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        public static readonly DependencyProperty RightSegmentColorFullProperty =
            DependencyProperty.Register("RightSegmentColorFull", typeof(Brush), typeof(Cluster), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, (byte)100, (byte)255, (byte)100))));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(Cluster), new PropertyMetadata(false, null));

        public static readonly DependencyProperty SelectedColorProperty =
           DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(Cluster), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty TextCentrProperty =
          DependencyProperty.Register("TextCentr", typeof(string), typeof(Cluster), new PropertyMetadata("", null));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            Cluster circle = sender as Cluster;
            circle.angleL = MAP(circle.LeftValue, circle.LeftMinimum, circle.LeftMaximum, 0, 180);
            circle.angleR = MAP(circle.RightValue, circle.RightMinimum, circle.RightMaximum, 0, 180);
            circle.set();
        }

        public static double MAP(double x, double inMin, double inMax, double outMin, double outMax)
        {
            double d = (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

            if (d > outMax) d = outMax;
            if (d < outMin) d = outMin;
            return d;
        }
        private void set()
        {
            Ellipse lel = new Ellipse();
            RectangleGeometry lr1 = new RectangleGeometry(new Rect(0, 0, Width / 2 - 4, Height - 4));
            RectangleGeometry lr2 = new RectangleGeometry(new Rect(0, 0, Width / 2 - 4, Height - 4));
            lr2.Transform = new RotateTransform(angleL, Width / 2 - 4, Height / 2 - 4);
            CombinedGeometry lcg = new CombinedGeometry(GeometryCombineMode.Exclude, lr1, lr2);
            if (angleL != 180)
                ellipseL.Stroke = LeftSegmentColor;
            else ellipseL.Stroke = LeftSegmentColorFull;
            ellipseL.Clip = lcg;

            Ellipse rel = new Ellipse();
            RectangleGeometry rr1 = new RectangleGeometry(new Rect(Width / 2 - 4, 0, Width / 2 - 4, Height - 4));
            RectangleGeometry rr2 = new RectangleGeometry(new Rect(Width / 2 - 4, 0, Width / 2 - 4, Height - 4));
            rr2.Transform = new RotateTransform(0 - angleR, Width / 2 - 4, Height / 2 - 4);
            CombinedGeometry rcg = new CombinedGeometry(GeometryCombineMode.Exclude, rr1, rr2);
            if (angleR != 180)
                ellipseR.Stroke = RightSegmentColor;
            else ellipseR.Stroke = RightSegmentColorFull;
            ellipseR.Clip = rcg;
        }
    }
}
