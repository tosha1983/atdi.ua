using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.GN06;

namespace Atdi.Contracts.Sdrn.DeepServices.GN06
{
	
	/// <summary>
	/// 
	/// </summary>
	public struct BroadcastingCalcBarycenterGE06
    {
        public BroadcastingAllotment BroadcastingAllotment;
        public BroadcastingAssignment[] BroadcastingAssignments;
    }
}
