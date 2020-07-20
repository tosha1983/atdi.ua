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
	public class BroadcastingAssignment
	{
		/// <summary>
		/// 
		/// </summary>
		public AdministrativeData AdmData;

		/// <summary>
		/// 
		/// </summary>
		public BroadcastingAssignmentTarget Target; //M for Action = MODIFY

		/// <summary>
		/// 
		/// </summary>
		public BroadcastingAssignmentEmissionCharacteristics EmissionCharacteristics;

		/// <summary>
		/// 
		/// </summary>
		public SiteParameters SiteParameters;

		/// <summary>
		/// 
		/// </summary>
		public AntennaCharacteristics AntennaCharacteristics;

		/// <summary>
		/// 
		/// </summary>
		public DigitalPlanEntryParameters DigitalPlanEntryParameters;
	}
}
