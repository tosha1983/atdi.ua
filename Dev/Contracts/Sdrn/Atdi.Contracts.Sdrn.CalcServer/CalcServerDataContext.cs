using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	/// <summary>
	/// Контекст данных сервера расчетов.
	/// </summary>
	public sealed class CalcServerDataContext : DataContextBase
	{
		public CalcServerDataContext() : base("SDRN_CalcServer_DB")
		{
		}
	}
}
