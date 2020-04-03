using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
	internal static class PropagationLoss
	{

        public static int Calc(CalcLossArgs args)
        {
            float Lbf_dB = CalclMainBlock(in args.Model.MainBlock);
            float Ld_dB = 0;
            if (args.Model.DiffractionBlock.Available) { Ld_dB = CalcDiffraction(in args.Model.DiffractionBlock);}

            return 0;
        }
        private static float CalclMainBlock(in MainCalcBlock args)
		{
            float ha_m = 15; // Надо сюда передавать
            float hb_m = 20; // Надо сюда передавать
            double Freq_MHz = 1800; // Надо сюда передавать
            double d_km = 15; // Надо сюда передавать
            float Lbf_dB = 0;
            switch (args.ModelType)
            {
                case MainCalcBlockModelType.ITU525:
                case MainCalcBlockModelType.Unknown:
                default:
                    Lbf_dB = ITU525.Calc(ha_m, hb_m, Freq_MHz, d_km);
                    break;
            }
            return Lbf_dB;
		}
        private static float CalcDiffraction(in DiffractionCalcBlock args)
        {
            float Ld_dB = 0;
            switch (args.ModelType)
            {
                case DiffractionCalcBlockModelType.Deygout91:
                    // вызов модели дегу
                    break;
                case DiffractionCalcBlockModelType.Deygout66:
                case DiffractionCalcBlockModelType.Bullington:
                case DiffractionCalcBlockModelType.ITU526_15:
                default:
                    Ld_dB = 0;
                    break;
            }
            return Ld_dB;

        }
	}
}
