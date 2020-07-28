using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis.MapService;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.SignalService;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using System.Runtime.InteropServices.WindowsRuntime;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.FieldStrength;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class GE06PropagationModel
    {
        /// <summary>
        /// Установка модели распространения и ее параметров 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static PropagationModel GetPropagationModel(Ge06TaskParameters ge06TaskParameters, PropagationModel ContextDataModel, CalculationType calculationType)
        {
            var propagationModel = new PropagationModel();
            switch (calculationType)
            {
                case CalculationType.CreateContoursByDistance:
                case CalculationType.CreateContoursByFS:
                case CalculationType.ConformityCheck:
                case CalculationType.FindAffectedADM:
                default:
                    propagationModel.MainBlock.ModelType = MainCalcBlockModelType.ITU1546_ge06;
                    propagationModel.AbsorptionBlock.Available = false;
                    propagationModel.AdditionalBlock.Available = false;
                    propagationModel.AtmosphericBlock.Available = false;
                    propagationModel.ClutterBlock.Available = false;
                    propagationModel.DiffractionBlock.Available = false;
                    propagationModel.DuctingBlock.Available = false;
                    propagationModel.ReflectionBlock.Available = false;
                    propagationModel.SubPathDiffractionBlock.Available = false;
                    propagationModel.TropoBlock.Available = false;
                    propagationModel.Parameters.EarthRadius_km = 8500;
                    propagationModel.Parameters.Location_pc = 50;
                    if (!(ge06TaskParameters.PercentageTime is null))
                    { propagationModel.Parameters.Time_pc = (float)ge06TaskParameters.PercentageTime; }
                    else if ((ContextDataModel.Parameters.Time_pc > 0.01) || (ContextDataModel.Parameters.Time_pc < 99))
                    { propagationModel.Parameters.Time_pc = ContextDataModel.Parameters.Time_pc; }
                    else
                    { ContextDataModel.Parameters.Time_pc = 50; }

                    break;
            }
            return propagationModel;
        }

        /// <summary>
        /// Установка модели распространения и ее параметров для ConformityCheck
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void GetPropagationModelForConformityCheck(ref PropagationModel propagationModel, float location_pc, float percentageTime)
        {
            propagationModel.Parameters.Location_pc = location_pc;
            propagationModel.Parameters.Time_pc = percentageTime;
            //height_m??????????????
        }

        /// <summary>
        /// Установка модели распространения и ее параметров для ContoursByFS
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void GetPropagationModelForContoursByFS(ref PropagationModel propagationModel, float location_pc, float percentageTime)
        {
            propagationModel.Parameters.Location_pc = location_pc;
            propagationModel.Parameters.Time_pc = percentageTime;
            //height_m??????????????
        }

        /// <summary>
        /// Установка модели распространения и ее параметров для FindAffectedADM
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void GetPropagationModelForFindAffectedADM(ref PropagationModel propagationModel, float location_pc, float percentageTime)
        {
            propagationModel.Parameters.Location_pc = location_pc;
            propagationModel.Parameters.Time_pc = percentageTime;
            //height_m??????????????
        }


        
    }
}
