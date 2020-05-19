using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.Core
{
	public class ViewCaptionAttribute : Attribute
	{
		public readonly string Text;

		public ViewCaptionAttribute(string text)
		{
			this.Text = text;
		}
	}
}
