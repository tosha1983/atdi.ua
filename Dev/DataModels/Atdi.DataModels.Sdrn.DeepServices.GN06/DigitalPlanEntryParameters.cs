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
		public Plan_entryType PlanEntry; //M

		/// <summary>
		/// 
		/// </summary>
		public Assgn_codeType Assgn_code; //M

		/// <summary>
		/// 
		/// </summary>
		public string Associated_adm_allot_id;//O

		/// <summary>
		/// 
		/// </summary>
		public string Associated_allot_sfn_id;//O

		/// <summary>
		/// 
		/// </summary>
		public string Sfn_id;//O
	}
}
