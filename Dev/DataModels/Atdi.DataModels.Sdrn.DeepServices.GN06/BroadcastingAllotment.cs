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
	public class BroadcastingAllotment
	{
		/// <summary>
		/// 
		/// </summary>
		public AdministrativeData AdminData;

		/// <summary>
		/// 
		/// </summary>
		public BroadcastingAssignmentTarget Target; //M for Action = 
		
		/// <summary>
		/// 
		/// </summary>
		public BroadcastingAllotmentEmissionCharacteristics EmissionCharacteristics;

		/// <summary>
		/// 
		/// </summary>
		public AllotmentParameters AllotmentParameters;

		/// <summary>
		/// 
		/// </summary>
		public DigitalPlanEntryParameters DigitalPlanEntryParameters;
	}
}
