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
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Markup;

namespace Atdi.Test.WpfContols.WinForm
{
	public partial class MainForm : Form
	{
		public ElementHost _wpfElementHost;

		public MainForm()
		{
			InitializeComponent();

			if (null == System.Windows.Application.Current)
			{
				new System.Windows.Application();
			}

			_wpfElementHost = new ElementHost
			{
				Dock = DockStyle.Fill
			};
			this.Controls.Add(_wpfElementHost);

			var fileName = @"C:\Projects\Repos\atdi.ua\Dev\Test\Atdi.Test.WpfContols.WinForm\Xaml\MainForm.xaml";
			using (var fileStream = new FileStream(fileName, FileMode.Open))
			{
				this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
				(this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = new MainView();
			}
		}
	}
}
