using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.AntennaPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.FieldStrength;

namespace Atdi.Contracts.Sdrn.DeepServices.RadioSystem
{
	public interface ISignalService : IDeepService
	{
		/// <summary>
		/// 
		/// </summary>
		void CalcLoss(in CalcLossArgs args, ref CalcLossResult result);


		double CalcAntennaGain(in CalcAntennaGainArgs args);


        void CalcAntennaPattern(in DiagrammArgs args, ref DiagrammPoint[] diagrammPointsResult);


        void CalcFS_ITU1546_4(in CalcFSArgs args, ref CalcFSResult result);


        void CalcFS_ITU1546_6(in CalcFSArgs args, ref CalcFSResult result);

        void CalcFS_ITU1546_ge06(in CalcFSArgs args, ref CalcFSResult result);
    }
}
