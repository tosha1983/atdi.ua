using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;



namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	
	/// <summary>
	/// 
	/// </summary>
	public struct BroadcastingFieldStrengthInPointGE06
    {
        public Ge06TaskParameters Ge06TaskParameters;
        public PropagationModel PropagationModel;
        public AtdiCoordinate PointCoordinate;
        public AtdiCoordinate TargetCoordinate;
        public AtdiMapArea MapArea;
        public short[] ReliefContent;
        public byte[] ClutterContent;
        public byte[] BuildingContent;
        public CluttersDesc CluttersDesc;

    }
}
