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
using Atdi.Platform.Logging;

namespace Atdi.Tools.Sdrn.OnlineMeasurement
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly ILogger _logger;

		public MainWindow(ILogger logger)
		{
			this._logger = logger;
			InitializeComponent();

			_logger.Info("MainWindow", "Ctor", "MainWindow has been created");
		}
	}
}
