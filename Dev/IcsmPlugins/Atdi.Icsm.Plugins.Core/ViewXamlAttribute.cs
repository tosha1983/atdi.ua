using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atdi.Icsm.Plugins.Core
{
	public class ViewXamlAttribute : Attribute
	{
		public readonly string Name;

		public ViewXamlAttribute(string name)
		{
			this.Name = name;
			this.WindowState = FormWindowState.Maximized;
		}

		public int Width { get; set; }

		public int Height { get; set; }

		public System.Windows.Forms.FormWindowState WindowState { get; set; }

	}
}
