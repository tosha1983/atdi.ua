using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.GN06
{
	/// <summary>
	/// ITU Name: "Assgn_codeType".
	/// Тип кода присвоения
	/// </summary>
	public enum AssignmentCodeType
	{
		U = 0, // unknown
		L, // linked with a SFN or an allotment, 
		C, // converted, 
		S // standalone
	}
}
