using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.FieldStrength;

namespace Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.Signal
{
    public static class PropagationLoss
    {
        //private static int[] obstaclesIndexes = new int[3] { 0, 0, 0 };
        //private static double[] obstaclesLosses = new double[3] { 0, 0, 0 };

        public static CalcLossResult Calc(CalcLossArgs args)
        {
            // Нужно тестировать
            double Lbf_dB = CalclMainBlock(in args);
            double Ldsub_dB = 0;
            if (args.Model.SubPathDiffractionBlock.Available) { Ldsub_dB = CalcSubPathDiffractionBlok(in args); }
            //double Ld_dB = 0;
            //if (args.Model.DiffractionBlock.Available) { Ld_dB = CalcDiffraction(in args, Ldsub_dB); }
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

            DiffractionLossResult diffLoss = new DiffractionLossResult { DiffractionLoss_dB = 0, ObstaclesProfileIndexes = new int[3] { 0, 0, 0 } };
            if (args.Model.DiffractionBlock.Available) { diffLoss = CalcDiffraction(in args, Ldsub_dB); }
            //// в случае если при расчёте диффракции какое-то из учитываемых препятствий попало на лес - пересчитываем диффракцию
            CalcLossResult hybridLoss = new CalcLossResult();
            //bool hybridDiffraction = false;
            if (args.Model.AbsorptionBlock.Hybrid)//(args.Model.AbsorptionBlock.Available && args.Model.DiffractionBlock.Available)
            {
                for (int i = 0; i < diffLoss.ObstaclesProfileIndexes.Length; i++)
                {
                    if (args.ClutterProfile[diffLoss.ObstaclesProfileIndexes[i]] != 0 && args.BuildingProfile[diffLoss.ObstaclesProfileIndexes[i]] == 0)
                    {
                        args.HybridDiffraction = true;
                        break;
                    }
                }
                if (args.HybridDiffraction)
                {
                    //// убираем лес из профиля высот и здания из клатеров
                    //for (int i = args.ReliefStartIndex; i < args.ReliefStartIndex + args.ProfileLength; i++)
                    //{
                    //    if (args.BuildingProfile[i] != 0)
                    //    //if (args.ClutterProfile[i] != 0 && args.BuildingProfile[i] == 0)
                    //    {
                    //        //args.HeightProfile[i] -= (short)args.CluttersDesc.Frequencies[0].Clutters[args.ClutterProfile[i]].Height_m;
                    //        args.ReliefProfile[i] += args.BuildingProfile[i];
                    //    }
                    //    else if (args.ClutterProfile[i] != 0 && args.BuildingProfile[i] != 0)
                    //    {
                    //        args.ClutterProfile[i] = 0;
                    //    }
                    //}

                    // пересчёт диффракции
                    if (args.Model.AbsorptionBlock.Available)
                    {
                        hybridLoss = CalcAbsorptionBlok(in args);
                        //if (diffLoss.DiffractionLoss_dB > hybridLoss.LossA_dB)
                        //{
                        //    diffLoss.DiffractionLoss_dB = hybridLoss.LossA_dB;
                        //}
                        
                    }
                }
            }

            double tilta; double tiltb;
            if (args.Model.DiffractionBlock.Available)
            {
                ProfilesCalculation.CalcTilts(args.Model.Parameters.EarthRadius_km, args.Ha_m, args.Hb_m, args.D_km, args.HeightProfile, args.BuildingStartIndex, args.ProfileLength, out tilta, out tiltb);
            }
            else
            { ProfilesCalculation.CalcTilts(args.Model.Parameters.EarthRadius_km, args.Ha_m, args.Hb_m, args.D_km, out tilta, out tiltb); }
            //// hybr
            CalcLossResult LossResult = new CalcLossResult()
            {
                TiltaD_Deg = tilta,
                TiltbD_Deg = tiltb,
            };
            if (args.HybridDiffraction && hybridLoss.LossA_dB < diffLoss.DiffractionLoss_dB)
            {
                LossResult.LossD_dB = Lbf_dB + hybridLoss.LossA_dB + Lclutter_dB;
                LossResult.DiffractionLoss_dB = hybridLoss.LossA_dB;
                args.HybridDiffraction = false;
            }
            else
            {
                LossResult.LossD_dB = Lbf_dB + diffLoss.DiffractionLoss_dB + Lclutter_dB;
                LossResult.DiffractionLoss_dB = diffLoss.DiffractionLoss_dB;
            }
            //CalcLossResult LossResult = new CalcLossResult()
            //{
            //    TiltaD_Deg = tilta,
            //    TiltbD_Deg = tiltb,
            //    LossD_dB = Lbf_dB + Ld_dB + Lclutter_dB,
            //    DiffractionLoss_dB = Ld_dB
            //};
            if (args.Model.AbsorptionBlock.Available)
            {
                LossResult.TiltaA_Deg = Labsorption_dB.TiltaA_Deg;
                LossResult.TiltbA_Deg = Labsorption_dB.TiltbA_Deg;
                LossResult.LossA_dB = Lbf_dB + Labsorption_dB.LossA_dB;
            }
            return LossResult;
        }
        private static float CalclMainBlock(in CalcLossArgs args)
        {
            float Lbf_dB = 0;
            switch (args.Model.MainBlock.ModelType)
            {
                case MainCalcBlockModelType.ITU525:
                    Lbf_dB = ITU525.Calc(args.Ha_m, args.Hb_m, args.Freq_Mhz, args.D_km);
                    break;
                case MainCalcBlockModelType.ITU1546:
                    // prepare data
                    double hEffective_m = 0;

                    int hMedDist_px = 0;

                    double asl_m = args.Hb_m + args.ReliefProfile[args.ReliefStartIndex + args.ProfileLength - 1];
                    //параметр нужно будет добавить через буффер
                    List<LandSea> landSeaList = new List<LandSea>();
                    bool h2aboveSea = false;

                    double px2km = args.D_km / (args.ProfileLength - 1);
                    double aboveLand_px = 0;
                    double aboveSea_px = 0;
                    byte waterClutter = 7; // clutter code for sea/inland water should be in config
                    //
                    int minDistToHeff_px = (int)(3.0 / px2km);
                    int maxDistToHeff_px = (int)(15.0 / px2km);
                    //
                    // land-sea distance list + effective height calculation
                    for (int i = args.ReliefStartIndex; i < args.ReliefStartIndex + args.ProfileLength; i++)
                    {
                        // coast line intersection condition
                        if (i > args.ReliefStartIndex && args.ClutterProfile[i - 1] != args.ClutterProfile[i] &&
                            (args.ClutterProfile[i - 1] == waterClutter || args.ClutterProfile[i] == waterClutter) &&
                            (aboveSea_px != 0 && aboveLand_px != 0) ||
                            i == args.ReliefStartIndex + args.ProfileLength - 1)
                        {
                            landSeaList.Add(new LandSea { land = aboveLand_px * px2km, sea = aboveSea_px * px2km });
                            aboveSea_px = 0;
                            aboveLand_px = 0;
                        }
                        // count relief points that belongs to land or sea propagation
                        if (args.ClutterProfile[i] == waterClutter)
                        {
                            aboveSea_px++;
                            if (i == args.ReliefStartIndex + args.ProfileLength - 1)
                            {
                                h2aboveSea = true;
                            }

                        }
                        else { aboveLand_px++; }

                        // effective height calculation, 
                        if (i > minDistToHeff_px && i < maxDistToHeff_px)
                        //if (i >= args.ReliefStartIndex + args.ProfileLength * 0.2 && i < maxDistToHeff_px)
                        {
                            hEffective_m += args.ReliefProfile[args.ReliefStartIndex] - args.ReliefProfile[i];
                            hMedDist_px++;
                        }
                    }
                    if (hMedDist_px > 0)
                    {
                        hEffective_m /= hMedDist_px;
                        hEffective_m += args.Ha_m;
                    }

                    //hEffective_m = 52.3;
                    asl_m = hEffective_m;
                    //landSeaList.Add(new land_sea { land = 0, sea = args.D_km });
                    //landSeaList.Add(new land_sea { land = 9.2, sea = 12.8 });
                    //landSeaList.Add(new land_sea { land = 6.0, sea = 0 });
                    double E_dBuVm = (ITU1546_ge06.Get_E(args.Ha_m, hEffective_m, args.D_km, args.Freq_Mhz, args.Model.Parameters.Time_pc, asl_m, args.Hb_m, h2aboveSea, landSeaList.ToArray()));
                    Lbf_dB = (float)(139.3 - E_dBuVm + 20 * Math.Log10(args.Freq_Mhz));
                    break;

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
        private static DiffractionLossResult CalcDiffraction(in CalcLossArgs args, double Ldsub_dB, bool calcabsorbtion = false)
        {
            // 2 - cases
            // lowering ant in above clutter HeightProfile - ReliefStartIndex
            // 999 if ant inside clutter
            short[] prof; int startIndex;
            var hA_m = args.Ha_m;
            var hB_m = args.Hb_m;
            if (calcabsorbtion)
            {
                if (args.HybridDiffraction && args.BuildingProfile != null && args.BuildingProfile.Length > 0)
                {
                    prof = args.BuildHeightProfile; startIndex = args.HeightStartIndex;
                    hA_m -= args.BuildingProfile[startIndex];
                    hB_m -= args.BuildingProfile[startIndex + args.ProfileLength - 1];
                }
                else
                {
                    prof = args.ReliefProfile; startIndex = args.ReliefStartIndex;
                }
            }
            else
            {
                prof = args.HeightProfile; startIndex = args.HeightStartIndex;

                int hAcorr = 0;
                int hBcorr = 0;
                //Ha = Ha - args.CluttersDesc.Frequencies[0].Clutters[0].Height_m;

                if (args.ClutterProfile != null && args.ClutterProfile.Length > 0 &&
                    args.CluttersDesc.Frequencies != null && args.CluttersDesc.Frequencies.Length > 0 &&
                    args.CluttersDesc.Frequencies[0].Clutters != null)
                {
                    // for testing until clutter height is incorrect
                    //hA_m -= args.ClutterProfile[startIndex];
                    //hB_m -= args.ClutterProfile[startIndex + args.ProfileLength - 1];
                    // when clutter height is calculated correctly
                    hAcorr = args.CluttersDesc.Frequencies[0].Clutters[args.ClutterProfile[startIndex]].Height_m;
                    hBcorr = args.CluttersDesc.Frequencies[0].Clutters[args.ClutterProfile[startIndex + args.ProfileLength - 1]].Height_m;
                }
                if (args.BuildingProfile != null && args.BuildingProfile.Length > 0)
                {
                    if (hAcorr < args.BuildingProfile[startIndex])
                    {
                        hAcorr = args.BuildingProfile[startIndex];
                    }
                    if (hBcorr < args.BuildingProfile[startIndex + args.ProfileLength - 1])
                    {
                        hBcorr = args.BuildingProfile[startIndex + args.ProfileLength - 1];
                    }
                }

                hA_m -= hAcorr;
                hB_m -= hBcorr;

            }
            //if (Ha < 0) { return 999;}

            //double Ld_dB = 0;
            DiffractionLossResult Ld = new DiffractionLossResult { DiffractionLoss_dB = 0, ObstaclesProfileIndexes = new int[3] { 0, 0, 0 } };
            switch (args.Model.DiffractionBlock.ModelType)
            {
                case DiffractionCalcBlockModelType.Deygout91:
                    // вызов модели дегу
                    Ld = Deygout91.Calc(hA_m, hB_m, args.Freq_Mhz, args.D_km, prof, startIndex, args.ProfileLength, args.Model.Parameters.EarthRadius_km, Ldsub_dB);
                    break;
                case DiffractionCalcBlockModelType.Deygout66:
                case DiffractionCalcBlockModelType.Bullington:
                case DiffractionCalcBlockModelType.ITU526_15:
                    //default:
                    //    Ld;
                    break;
            }
            return Ld;
        }
        private static CalcLossResult CalcAbsorptionBlok(in CalcLossArgs args)
        { // Надо тестировать
            // первоначальный расчет дифракции
            double Ldsub_dB = 0;
            if (args.Model.SubPathDiffractionBlock.Available) { Ldsub_dB = CalcSubPathDiffractionBlok(in args); }
            double Ld_dB = 0;
            if (args.Model.DiffractionBlock.Available) { Ld_dB = CalcDiffraction(in args, Ldsub_dB, true).DiffractionLoss_dB; }
            // определение УМ
            double tilta;
            double tiltb;
            if (args.HybridDiffraction)
            {
                ProfilesCalculation.CalcTilts(args.Model.Parameters.EarthRadius_km, args.Ha_m, args.Hb_m, args.D_km, args.BuildHeightProfile, args.ReliefStartIndex, args.ProfileLength, out tilta, out tiltb);
            }
            else
            {
                ProfilesCalculation.CalcTilts(args.Model.Parameters.EarthRadius_km, args.Ha_m, args.Hb_m, args.D_km, args.ReliefProfile, args.ReliefStartIndex, args.ProfileLength, out tilta, out tiltb);
            }

            AbsorptionCalcBlockModelType AbModel = args.Model.AbsorptionBlock.ModelType;
            double Freq_MHz = args.Freq_Mhz;
            double Time_PC = args.Model.Parameters.Time_pc;

            // Вычисляем индекс частоты масива клатеров 
            int IndexFreqUp = GetIndexClutterArr(in args);
            var CluttersDesc = args.CluttersDesc;
            double Labsorption_dB = ProfilesCalculation.CalcLossOfObstacles((currentLoss, obs) => AbsorptionCalc.CalcAbsorption(in CluttersDesc, IndexFreqUp, AbModel, Freq_MHz, Time_PC, currentLoss, obs), in args, tilta, tiltb);
            CalcLossResult result = new CalcLossResult()
            {
                DiffractionLoss_dB = Ld_dB,
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
            // Вычисляем индекс частоты масива клатеров 
            int IndexFreqUp = GetIndexClutterArr(in args);
            var CluttersDesc = args.CluttersDesc;
            double Lclutter_dB = ProfilesCalculation.CalcLossOfObstacles((currentLoss, obs) => AbsorptionCalc.CalcClutter(in CluttersDesc, IndexFreqUp, ClModel, Freq_MHz, Time_PC, currentLoss, obs), in args, tilta, tiltb);
            return Lclutter_dB;
        }
        private static int GetIndexClutterArr(in CalcLossArgs args)
        {
            int IndexFreqUp = 0;
            if (args.CluttersDesc != null)
            {
                var model = args.Model;
                if ((model.AbsorptionBlock.ModelType == AbsorptionCalcBlockModelType.FlatAndLinear) ||
                   (model.AbsorptionBlock.ModelType == AbsorptionCalcBlockModelType.Flat) ||
                   (model.AbsorptionBlock.ModelType == AbsorptionCalcBlockModelType.Linear) ||
                   (model.ClutterBlock.ModelType == ClutterCalcBlockModelType.Flat))
                {
                    // нужны в расчете параметры клатеров придется интерполировать
                    IndexFreqUp = args.CluttersDesc.Frequencies.Length;
                    for (int i = 0; i < args.CluttersDesc.Frequencies.Length; i++)
                    {
                        if (args.Freq_Mhz < args.CluttersDesc.Frequencies[i].Freq_MHz)
                        { IndexFreqUp = i; break; }
                    }
                }
            }
            return IndexFreqUp;
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
