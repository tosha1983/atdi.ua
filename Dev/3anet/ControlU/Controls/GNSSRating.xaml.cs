using System.Windows;
using System.Windows.Controls;

namespace ControlU.Controls
{
    /// <summary>
    /// Логика взаимодействия для GNSSRating.xaml
    /// </summary>
    public partial class GNSSRating : UserControl
    {
        public GNSSRating()
        {
            InitializeComponent();
            this.DataContext = MainWindow.gps;
        }

        public int RatingValue
        {
            get { return (int)GetValue(RatingValueProperty); }
            set { SetValue(RatingValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RatingValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof(int), typeof(GNSSRating), new UIPropertyMetadata(0, OnRatingValuePropertyChanged));
        private static void OnRatingValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GNSSRating).InvalidateArrange();
        }
    }

}
