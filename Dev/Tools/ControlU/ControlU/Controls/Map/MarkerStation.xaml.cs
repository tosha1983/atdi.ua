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
    /// Логика взаимодействия для MarkerStation.xaml
    /// </summary>
    public partial class MarkerStation : UserControl
    {
        public MarkerStation()
        {
            InitializeComponent();
            DataContext = this;
            set();
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
        public Brush RightSegmentColor
        {
            get { return (Brush)GetValue(RightSegmentColorProperty); }
            set { SetValue(RightSegmentColorProperty, value); }
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

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(MarkerStation), new PropertyMetadata(100d, new PropertyChangedCallback(OnPropertyChanged)));

        public static readonly DependencyProperty LeftSegmentColorProperty =
            DependencyProperty.Register("LeftSegmentColor", typeof(Brush), typeof(MarkerStation), new PropertyMetadata(new SolidColorBrush(Colors.Orange), new PropertyChangedCallback(ColorChanged)));

        public static readonly DependencyProperty RightSegmentColorProperty =
           DependencyProperty.Register("RightSegmentColor", typeof(Brush), typeof(MarkerStation), new PropertyMetadata(new SolidColorBrush(Colors.Orange), new PropertyChangedCallback(ColorChanged)));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MarkerStation), new PropertyMetadata(false, null));

        public static readonly DependencyProperty SelectedColorProperty =
           DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(MarkerStation), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty TextCentrProperty =
          DependencyProperty.Register("TextCentr", typeof(string), typeof(MarkerStation), new PropertyMetadata("", null));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MarkerStation circle = sender as MarkerStation;
            circle.set();
        }
        private static void ColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MarkerStation circle = sender as MarkerStation;
            circle.ellipseL.Fill = circle.LeftSegmentColor;
            circle.ellipseR.Fill = circle.RightSegmentColor;
            circle.set();
        }
        private void set()
        {
            //Ellipse lel = new Ellipse();
            RectangleGeometry lr1 = new RectangleGeometry(new Rect(0, 0, Width / 2 - 4, Height - 4));
            RectangleGeometry lr2 = new RectangleGeometry(new Rect(0, 0, Width / 2 - 4, Height - 4));
            CombinedGeometry lcg = new CombinedGeometry(GeometryCombineMode.Intersect, lr1, lr2);
            ellipseL.Clip = lcg;

            //Ellipse rel = new Ellipse();
            RectangleGeometry rr1 = new RectangleGeometry(new Rect(Width / 2 - 4, 0, Width / 2 - 4, Height - 4));
            RectangleGeometry rr2 = new RectangleGeometry(new Rect(Width / 2 - 4, 0, Width / 2 - 4, Height - 4));
            CombinedGeometry rcg = new CombinedGeometry(GeometryCombineMode.Intersect, rr1, rr2);
            ellipseR.Clip = rcg;
        }
    }
}
