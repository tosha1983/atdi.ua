using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.AntennaPattern;
using Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.FieldStrength;
using Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal;



namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem
{
	public class SignalService : ISignalService
	{

		public double CalcAntennaGain(in CalcAntennaGainArgs args)
		{
            return Signal.CalcAntennaGain.Calc(in args);
		}

		public void CalcLoss(in CalcLossArgs args, ref CalcLossResult result)
		{
            result = Signal.PropagationLoss.Calc(args);
		}


        private void CalcMainBlock(in MainCalcBlock block)
		{

		}

		public void Dispose()
		{
			
		}

        public void CalcAntennaPattern(in DiagrammArgs args, ref DiagrammPoint[] diagrammPointsResult)
        {
            CalculationAntennaPattern.Calc(in args, ref diagrammPointsResult);
        }

        public void CalcFS_ITU1546_4(in CalcFSArgs args, ref CalcFSResult result)
        {
            result.FSResult = ITU1546_4.Get_E(args.ha, args.hef, args.d, args.f, args.p, args.h_gr, args.h2, args.list1);
        }

        public void CalcFS_ITU1546_6(in CalcFSArgs args, ref CalcFSResult result)
        {
            result.FSResult = ITU1546_6.Get_E(args.ha, args.hef, args.d, args.f, args.p, args.h_gr, args.h2, args.h2AboveSea, args.list1);
        }

        public void CalcFS_ITU1546_ge06(in CalcFSArgs args, ref CalcFSResult result)
        {
            result.FSResult = ITU1546_ge06.Get_E(args.ha, args.hef, args.d, args.f, args.p, args.h_gr, args.h2, args.h2AboveSea, args.list1);
        }
              
    }
}
