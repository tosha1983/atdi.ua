using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using Atdi.DataModels.Sdrn.DeepServices.GN06;


namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations
{
	public struct BroadcastingFieldStrengthCalcData
    {
		public PropagationModel PropagationModel;

        public BroadcastingAssignment BroadcastingAssignment;

		public AtdiCoordinate PointCoordinate;

		public AtdiCoordinate TargetCoordinate;

		public AtdiMapArea MapArea;

		public short[] ReliefContent;

		public byte[] ClutterContent;

		public byte[] BuildingContent;

		public CluttersDesc CluttersDesc;

    }
}
