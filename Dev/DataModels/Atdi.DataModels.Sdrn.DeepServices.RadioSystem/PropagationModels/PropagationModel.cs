using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels
{
	public class PropagationModel
	{
		public MainCalcBlock MainBlock;

		public DiffractionCalcBlock DiffractionBlock;

		public SubPathDiffractionCalcBlock SubPathDiffractionBlock;

		public TropoCalcBlock TropoBlock;

		public DuctingCalcBlock DuctingBlock;

		public AbsorptionCalcBlock AbsorptionBlock;

		public ReflectionCalcBlock ReflectionBlock;

		public AtmosphericCalcBlock AtmosphericBlock;

		public AdditionalCalcBlock AdditionalBlock;

		public ClutterCalcBlock ClutterBlock;

		public GlobalParams Parameters;
	}
}
