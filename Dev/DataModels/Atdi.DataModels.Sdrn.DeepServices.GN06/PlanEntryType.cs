using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.GN06
{
	/// <summary>
	/// ITU Name: "Plan_entryType".
	/// </summary>
	public enum PlanEntryType
	{
		/// <summary>
		/// 
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// 
		/// </summary>
		SingleAssignment = 1,

		/// <summary>
		/// 
		/// </summary>
		SFN = 2,

		/// <summary>
		/// 
		/// </summary>
		Allotment = 3,

		/// <summary>
		/// 
		/// </summary>
		AllotmentWithLinkedAssignmentAndSfn = 4,

		/// <summary>
		/// 
		/// </summary>
		AllotmentWithSingleLinkedAssignmentAndNoSfn = 5
	}
}
