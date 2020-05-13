using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Forms;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Core;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Environment.Wpf;
using Atdi.Platform.Logging;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.Forms
{
    public partial class ViewForm : WpfFormBase
    {
	    private readonly ViewStarter _starter;
	    private readonly ILogger _logger;

	    public ViewForm(string xamlFileName, WpfViewModelBase viewObject, string caption, ViewStarter starter, ILogger logger)
	    {
		    _starter = starter;
		    _logger = logger;

		    InitializeComponent();

		    var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
		    var fileName = Path.Combine(appFolder, $"XICSM_SdrnCalcServerClient\\Xaml\\{xamlFileName}");
		    base.Text = caption;
		    using (var fileStream = new FileStream(fileName, FileMode.Open))
		    {
			    
			    this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
			    (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = viewObject;
			    
		    }
	    }


	    protected override void OnFormClosed(FormClosedEventArgs e)
	    {
		    _starter?.Close(this);
	    }
    }
}
