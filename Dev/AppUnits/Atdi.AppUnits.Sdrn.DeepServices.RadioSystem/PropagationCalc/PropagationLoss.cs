﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
	public static class PropagationLoss
	{

        public static CalcLossResult Calc(CalcLossArgs args)
        {// Нужно тестировать
            double Lbf_dB = CalclMainBlock(in args);
            double Ldsub_dB = 0;
            if (args.Model.SubPathDiffractionBlock.Available) {Ldsub_dB = CalcSubPathDiffractionBlok(in args);}
            double Ld_dB = 0;
            if (args.Model.DiffractionBlock.Available) {Ld_dB = CalcDiffraction(in args, Ldsub_dB);}
            CalcLossResult Labsorption_dB = new CalcLossResult();
            if (args.Model.AbsorptionBlock.Available) { Labsorption_dB = CalcAbsorptionBlok(in args); }
            double Lclutter_dB = 0;
            if (args.Model.ClutterBlock.Available) { Lclutter_dB = CalcClutterBlok(in args); }
            //double Ltropo_dB = 9999;
            //if (args.Model.TropoBlock.Available) {Ltropo_dB = CalcTropoBlok(in args);}
            //double Lducting_dB = 9999;
            //if (args.Model.DuctingBlock.Available){ Lducting_dB = CalcDuctingBlok(in args);}
            //double Lreflection_dB = 9999;
            //if (args.Model.ReflectionBlock.Available) { Lreflection_dB = CalcReflectionBlok(in args);}
            //double Latmospheric_dB = 0;
            //if (args.Model.AtmosphericBlock.Available) { Latmospheric_dB = CalcAtmosphericBlok(in args); }
            //double Ladditional_dB = 0;
            //if (args.Model.AdditionalBlock.Available) { Ladditional_dB = CalcAdditionalBlok(in args); }
            // Далее логика обединения моделей будет дописываться по мере возникновения новых моделей 
            double tilta; double tiltb;
            if (args.Model.DiffractionBlock.Available)
            { ProfilesCalculation.CalcTilts(args.Model.Parameters.EarthRadius_km, args.Ha_m, args.Hb_m, args.D_km, args.HeightProfile, args.BuildingStartIndex, args.ProfileLength, out tilta, out tiltb); }
            else
            { ProfilesCalculation.CalcTilts(args.Model.Parameters.EarthRadius_km, args.Ha_m, args.Hb_m, args.D_km, out tilta, out tiltb); }
            CalcLossResult LossResult = new CalcLossResult()
            {
                TiltaD_Deg = tilta,
                TiltbD_Deg = tiltb,
                LossD_dB = Lbf_dB + Ld_dB + Lclutter_dB
            };
            if (args.Model.AbsorptionBlock.Available)
            {
                LossResult.TiltaA_Deg = Labsorption_dB.LossD_dB;
                LossResult.TiltbA_Deg = Labsorption_dB.TiltaA_Deg;
                LossResult.LossA_dB = Lbf_dB + Labsorption_dB.TiltbA_Deg;
            }
            return LossResult;
        }
        private static float CalclMainBlock(in CalcLossArgs args)
		{
            float Lbf_dB = 0;
            switch (args.Model.MainBlock.ModelType)
            {
                case MainCalcBlockModelType.ITU525:
                case MainCalcBlockModelType.Unknown:
                default:
                    Lbf_dB = ITU525.Calc(args.Ha_m, args.Hb_m, args.Freq_Mhz, args.D_km);
                    break;
            }
            return Lbf_dB;
		}
        private static float CalcSubPathDiffractionBlok(in CalcLossArgs args)
        {
            float Ldsub_dB = 0;
            switch (args.Model.SubPathDiffractionBlock.ModelType)
            {
                case SubPathDiffractionCalcBlockModelType.SubDeygout91:
                    // вызов модель дегу для определения дополнительного ослабления субдифракции
                    break;
                default:
                    Ldsub_dB = 0;
                    break;
            }
            return Ldsub_dB;
        }
        private static double CalcDiffraction(in CalcLossArgs args, double Ldsub_dB, bool calcabsorbtion = false)
        {
            short[] prof; int startIndex; 
            if (calcabsorbtion) { prof = args.ReliefProfile; startIndex = args.ReliefStartIndex;}
            else {prof = args.HeightProfile; startIndex = args.HeightStartIndex;}

            double Ld_dB = 0;
            switch (args.Model.DiffractionBlock.ModelType)
            {
                case DiffractionCalcBlockModelType.Deygout91:
                    // вызов модели дегу
                    Ld_dB = Deygout91.Calc(args.Ha_m, args.Hb_m, args.Freq_Mhz, args.D_km, prof, startIndex, args.ProfileLength, args.Model.Parameters.EarthRadius_km, Ldsub_dB);
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
        private static CalcLossResult CalcAbsorptionBlok(in CalcLossArgs args)
        { // Надо тестировать
            // первоначальный расчет дифракции
            double Ldsub_dB = 0;
            if (args.Model.SubPathDiffractionBlock.Available) { Ldsub_dB = CalcSubPathDiffractionBlok(in args); }
            double Ld_dB = 0;
            if (args.Model.DiffractionBlock.Available) { Ld_dB = CalcDiffraction(in args, Ldsub_dB, true); }
            // определение УМ
            ProfilesCalculation.CalcTilts(args.Model.Parameters.EarthRadius_km, args.Ha_m, args.Hb_m, args.D_km, args.ReliefProfile, args.ReliefStartIndex, args.ProfileLength, out double tilta, out double tiltb);
            AbsorptionCalcBlockModelType AbModel = args.Model.AbsorptionBlock.ModelType;
            double Freq_MHz = args.Freq_Mhz;
            double Time_PC = args.Model.Parameters.Time_pc;
            double Labsorption_dB = ProfilesCalculation.CalcLossOfObstacles((currentLoss, obs)=>AbsorptionCalc.CalcAbsorption(AbModel, Freq_MHz, Time_PC, currentLoss, obs),in args, tilta, tiltb);
            CalcLossResult result = new CalcLossResult()
            {
                LossA_dB = Labsorption_dB + Ld_dB,
                TiltaA_Deg = tilta,
                TiltbA_Deg = tiltb
            };
            return result;
        }
        private static double CalcClutterBlok(in CalcLossArgs args)
        {// Надо тестировать
            // определение УМ
            ProfilesCalculation.CalcTilts(args.Model.Parameters.EarthRadius_km, args.Ha_m, args.Hb_m, args.D_km, args.HeightProfile, args.HeightStartIndex, args.ProfileLength, out double tilta, out double tiltb);
            // определяем препятсвия которые входят
            ClutterCalcBlockModelType ClModel = args.Model.ClutterBlock.ModelType;
            double Freq_MHz = args.Freq_Mhz;
            double Time_PC = args.Model.Parameters.Time_pc;
            double Lclutter_dB = ProfilesCalculation.CalcLossOfObstacles((currentLoss, obs) => AbsorptionCalc.CalcClutter(ClModel, Freq_MHz, Time_PC, currentLoss, obs), in args, tilta, tiltb);
            return Lclutter_dB;
        }

        //private static float CalcAtmosphericBlok(in CalcLossArgs args)
        //{
        //    float Latmospheric_dB = 0;
        //    // Вызов функции расчета по данной модели
        //    switch (args.Model.AtmosphericBlock.ModelType)
        //    {
        //        case AtmosphericCalcBlockModelType.ITU678:
        //            // Вызов функции расчета по данной модели
        //            break;
        //        default:
        //            break;
        //    }
        //    return Latmospheric_dB;
        //}
        //private static float CalcAdditionalBlok(in CalcLossArgs args)
        //{
        //    float Ladditional_dB = 0;
        //    // Вызов функции расчета по данной модели
        //    return Ladditional_dB;
        //}

        //private static float CalcTropoBlok(in CalcLossArgs args)
        //{
        //    float Ltropo_dB = 9999;
        //    switch (args.Model.TropoBlock.ModelType)
        //    {
        //        case TropoCalcBlockModelType.ITU617:
        //            // вызов модель ITU617
        //            break;
        //        default:
        //            Ltropo_dB = 9999;
        //            break;
        //    }
        //    return Ltropo_dB;
        //}
        //private static float CalcDuctingBlok(in CalcLossArgs args)
        //{
        //    float Lducting_dB = 9999;
        //    // Вызов функции расчета по данной модели
        //    return Lducting_dB;
        //}
        //private static float CalcReflectionBlok(in CalcLossArgs args)
        //{
        //    float Lreflection_dB = 9999;
        //    // Вызов функции расчета по данной модели
        //    return Lreflection_dB;
        //}
    }
}
