using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.Core
{
	public class ViewXamlAttribute : Attribute
	{
		public readonly string Name;

		public ViewXamlAttribute(string name)
		{
			this.Name = name;
		}
	}
}
