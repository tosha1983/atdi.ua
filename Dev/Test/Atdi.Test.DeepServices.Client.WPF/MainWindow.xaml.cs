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

namespace Atdi.Test.DeepServices.Client.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public TypeObject typeObjectInput;
        public Location[] locationsInput;
        public TypeObject typeObjectOutput;
        public Location[] locationsOutput;
        public Map MapX = new Map();

        public MainWindow()
        {
            InitializeComponent();
        }

        public void UpdateSource()
        {
            this.AddChild(MapX);
        }
      
    }
}
