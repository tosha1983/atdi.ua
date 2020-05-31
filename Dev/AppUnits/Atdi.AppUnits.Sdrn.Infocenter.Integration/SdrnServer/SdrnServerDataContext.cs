using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration.SdrnServer
{
	/// <summary>
	/// Контекст данных инфоцентра.
	/// </summary>
	public sealed class SdrnServerDataContext : DataContextBase
	{
		public SdrnServerDataContext() : base("SDRN_Server_DB")
		{
		}
	}
}
