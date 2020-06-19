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
	public class AdministrativeData
	{
		/// <summary>
		/// 
		/// </summary>
		public string Adm; // M

		/// <summary>
		/// 
		/// </summary>
		public string NoticeType; // M

		/// <summary>
		/// 
		/// </summary>
		public string Fragment; // O

		/// <summary>
		/// 
		/// </summary>
		public ActionType Action; //M

		/// <summary>
		/// 
		/// </summary>
		public string AdmRefId; //M ADM_KEY ICSM 
	}
}
