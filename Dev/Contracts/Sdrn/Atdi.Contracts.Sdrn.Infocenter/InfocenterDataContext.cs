using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.Contracts.Sdrn.Infocenter
{
	/// <summary>
	/// Контекст данных инфоцентра.
	/// </summary>
	public sealed class InfocenterDataContext : DataContextBase
	{
		public InfocenterDataContext() : base("SDRN_Infocenter_DB")
		{
		}
	}
}
