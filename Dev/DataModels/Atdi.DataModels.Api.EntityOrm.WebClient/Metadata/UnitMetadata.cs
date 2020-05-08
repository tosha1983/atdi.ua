using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.EntityOrm.WebClient
{
	public class UnitMetadata
	{
		public string Name;

		public string Dimension;

		public string Category;

		public override string ToString()
		{
			
			return $"{Name}";
		}
	}
}
