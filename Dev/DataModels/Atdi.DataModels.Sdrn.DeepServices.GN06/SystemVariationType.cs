using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.GN06
{
	/// <summary>
	/// ITU Name: "Sys_varType".
	/// Тип варианта системы
	/// </summary>
	public enum SystemVariationType
	{
		Unknown = 0,
		A1, A2, A3, A5, A7,
		B1, B2, B3, B5, B7,
		C1, C2, C3, C5, C7,
		D1, D2, D3, D5, D7,
		E1, E2, E3, E5, E7,
		F1, F2, F3, F5, F7
	}
}
