using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.GN06
{
	/// <summary>
	/// 
	/// </summary>
	public class DigitalPlanEntryParameters
	{
		/// <summary>
		/// 
		/// </summary>
		public PlanEntryType PlanEntry; //M

		/// <summary>
		/// ITU Name: "Assgn_code".
		/// </summary>
		public AssignmentCodeType AssignmentCode; //M

		/// <summary>
		/// ITU Name: "Associated_adm_allot_id".
		/// </summary>
		public string AdmAllotAssociatedId;//O

		/// <summary>
		/// ITU Name: "Associated_allot_sfn_id".
		/// </summary>
		public string SfnAllotAssociatedId;//O

		/// <summary>
		/// ITU Name: "Sfn_id".
		/// </summary>
		public string SfnId;//O
	}
}
