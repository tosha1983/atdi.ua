using Atdi.Icsm.Plugins.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.EntityOrmTest
{
	[ViewXaml("TestDialog.xaml", WindowState = FormWindowState.Normal, Width = 150, Height = 150)]
	[ViewCaption("Test Dialog")]
	public class TestDialogView : ViewBase
	{
		public TestDialogView()
		{
			var i = 0;
			var t = 1 / i;
		}
		public override void Dispose()
		{
			
		}
	}
}
