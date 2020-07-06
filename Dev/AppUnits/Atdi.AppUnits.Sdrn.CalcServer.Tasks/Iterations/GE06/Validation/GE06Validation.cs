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

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class GE06Validation
    {
        /// <summary>
        /// Валидация параметров для Allotment
        /// </summary>
        /// <param name="allotments"></param>
        /// <returns></returns>
        public static bool ValidationAllotment(BroadcastingAllotment allotments)
        {
            bool isSuccess = true;
            if (allotments != null)
            {
                //AdminData
                if (allotments.AdminData == null)
                {
                    isSuccess = false;
                }
                else if (allotments.AdminData != null)
                {
                    if (string.IsNullOrEmpty(allotments.AdminData.Adm))
                    {
                        isSuccess = false;
                    }
                    if (string.IsNullOrEmpty(allotments.AdminData.NoticeType))
                    {
                        isSuccess = false;
                    }
                }
                //AllotmentParameters
                if (allotments.AllotmentParameters == null)
                {
                    isSuccess = false;
                }
                else if (allotments.AllotmentParameters != null)
                {
                    if (allotments.AllotmentParameters.ContourId == 0)
                    {
                        isSuccess = false;
                    }
                    if (string.IsNullOrEmpty(allotments.AllotmentParameters.Name))
                    {
                        isSuccess = false;
                    }
                    if (allotments.AllotmentParameters.Сontur == null)
                    {
                        isSuccess = false;
                    }
                }
                
                // EmissionCharacteristics
                if (allotments.EmissionCharacteristics == null)
                {
                    isSuccess = false;
                }
                else if (allotments.EmissionCharacteristics != null)
                {
                    if ((((allotments.EmissionCharacteristics.Freq_MHz>=174) && (allotments.EmissionCharacteristics.Freq_MHz <= 230))
                        || ((allotments.EmissionCharacteristics.Freq_MHz >= 470) && (allotments.EmissionCharacteristics.Freq_MHz <= 582))
                        || ((allotments.EmissionCharacteristics.Freq_MHz >= 582) && (allotments.EmissionCharacteristics.Freq_MHz <= 862)))==false)
                    {
                        isSuccess = false;
                    }
                }
                // Target
                if (allotments.Target == null)
                {
                    isSuccess = false;
                }
                else if (allotments.Target != null)
                {
                    if (string.IsNullOrEmpty(allotments.Target.AdmRefId))
                    {
                        isSuccess = false;
                    }
                    if ((((allotments.Target.Freq_MHz >= 174) && (allotments.Target.Freq_MHz <= 230))
                        || ((allotments.Target.Freq_MHz >= 470) && (allotments.Target.Freq_MHz <= 582))
                        || ((allotments.Target.Freq_MHz >= 582) && (allotments.Target.Freq_MHz <= 862))) == false)
                    {
                        isSuccess = false;
                    }
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Валидация параметров для Assignment
        /// </summary>
        /// <param name="assignments"></param>
        /// <returns></returns>
        public static bool ValidationAssignment(BroadcastingAssignment[] assignments)
        {
            bool isSuccess = true;
            if (assignments != null)
            {
                for (int i = 0; i < assignments.Length; i++)
                {
                    // AdmData
                    if (assignments[i].AdmData == null)
                    {
                        isSuccess = false;
                    }
                    else if (assignments[i].AdmData != null)
                    {
                        if (string.IsNullOrEmpty(assignments[i].AdmData.Adm))
                        {
                            isSuccess = false;
                        }
                        if (string.IsNullOrEmpty(assignments[i].AdmData.AdmRefId))
                        {
                            isSuccess = false;
                        }
                        if (string.IsNullOrEmpty(assignments[i].AdmData.Fragment))
                        {
                            isSuccess = false;
                        }
                    }
                    //AntennaCharacteristics
                    if (assignments[i].AntennaCharacteristics == null)
                    {
                        isSuccess = false;
                    }
                    else if (assignments[i].AntennaCharacteristics != null)
                    {

                        //MaxEffHeight_m
                        if (((assignments[i].AntennaCharacteristics.MaxEffHeight_m >= 0) && (assignments[i].AntennaCharacteristics.MaxEffHeight_m <= 800)) == false)
                        {
                            isSuccess = false;
                        }
                        //EffHeight_m
                        if (assignments[i].AntennaCharacteristics.EffHeight_m == null)
                        {
                            isSuccess = false;
                        }
                        else if (assignments[i].AntennaCharacteristics.EffHeight_m != null)
                        {
                            if (assignments[i].AntennaCharacteristics.EffHeight_m.Length != 36)
                            {
                                isSuccess = false;
                            }
                            for (int j = 0; j < assignments[i].AntennaCharacteristics.EffHeight_m.Length; j++)
                            {
                                var effHeight_m = assignments[i].AntennaCharacteristics.EffHeight_m[j];
                                if (((effHeight_m >= -3000) && (effHeight_m <= 3000)) == false)
                                {
                                    isSuccess = false;
                                }
                            }
                        }
                        //EmissionCharacteristics
                        if (assignments[i].EmissionCharacteristics == null)
                        {
                            isSuccess = false;
                        }
                        else if (assignments[i].EmissionCharacteristics != null)
                        {
                            if ((((assignments[i].EmissionCharacteristics.Freq_MHz >= 174) && (assignments[i].EmissionCharacteristics.Freq_MHz <= 230))
                            || ((assignments[i].EmissionCharacteristics.Freq_MHz >= 470) && (assignments[i].EmissionCharacteristics.Freq_MHz <= 582))
                            || ((assignments[i].EmissionCharacteristics.Freq_MHz >= 582) && (assignments[i].EmissionCharacteristics.Freq_MHz <= 862))) == false)
                            {
                                isSuccess = false;
                            }

                            if (assignments[i].AntennaCharacteristics.Direction == AntennaDirectionType.D)
                            {
                                if ((assignments[i].EmissionCharacteristics.Polar == PolarType.H) && (assignments[i].EmissionCharacteristics.Polar == PolarType.M))
                                {
                                    if (assignments[i].EmissionCharacteristics.ErpH_dBW > 53)
                                    {
                                        isSuccess = false;
                                    }
                                }
                                if ((assignments[i].EmissionCharacteristics.Polar == PolarType.V) && (assignments[i].EmissionCharacteristics.Polar == PolarType.M))
                                {
                                    if (assignments[i].EmissionCharacteristics.ErpV_dBW > 53)
                                    {
                                        isSuccess = false;
                                    }
                                }

                             
                            }

                         
                        }
                        
                        if (assignments[i].AntennaCharacteristics.Direction == AntennaDirectionType.D)
                        {
                            //DiagrV
                            if ((assignments[i].EmissionCharacteristics.Polar == PolarType.V) && (assignments[i].EmissionCharacteristics.Polar == PolarType.M))
                            {
                                if (assignments[i].AntennaCharacteristics.DiagrV == null)
                                {
                                    isSuccess = false;
                                }
                                else if (assignments[i].AntennaCharacteristics.DiagrV != null)
                                {
                                    if (assignments[i].AntennaCharacteristics.DiagrV.Length != 36)
                                    {
                                        isSuccess = false;
                                    }
                                    for (int j = 0; j < assignments[i].AntennaCharacteristics.DiagrV.Length; j++)
                                    {
                                        var diagrV = assignments[i].AntennaCharacteristics.DiagrV[j];
                                        if (((diagrV >= 0) && (diagrV <= 40)) == false)
                                        {
                                            isSuccess = false;
                                        }
                                    }
                                }
                            }
                            if ((assignments[i].EmissionCharacteristics.Polar == PolarType.H) && (assignments[i].EmissionCharacteristics.Polar == PolarType.M))
                            {
                                //DiagrH
                                if (assignments[i].AntennaCharacteristics.DiagrH == null)
                                {
                                    isSuccess = false;
                                }
                                else if (assignments[i].AntennaCharacteristics.DiagrH != null)
                                {
                                    if (assignments[i].AntennaCharacteristics.DiagrH.Length != 36)
                                    {
                                        isSuccess = false;
                                    }
                                    for (int j = 0; j < assignments[i].AntennaCharacteristics.DiagrH.Length; j++)
                                    {
                                        var diagrH = assignments[i].AntennaCharacteristics.DiagrH[j];
                                        if (((diagrH >= 0) && (diagrH <= 40)) == false)
                                        {
                                            isSuccess = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //SiteParameters
                    if (assignments[i].SiteParameters == null)
                    {
                        isSuccess = false;
                    }
                    else if (assignments[i].SiteParameters != null)
                    {
                        if (((assignments[i].SiteParameters.Alt_m >= -1000) && (assignments[i].SiteParameters.Alt_m <= 8850)) == false)
                        {
                            isSuccess = false;
                        }
                        //if (string.IsNullOrEmpty(assignments[i].SiteParameters.Name))
                        //{
                        //    isSuccess = false;
                        //}
                    }

                    if (assignments[i].Target != null)
                    {
                        if (string.IsNullOrEmpty(assignments[i].Target.AdmRefId))
                        {
                            isSuccess = false;
                        }
                        if ((((assignments[i].Target.Freq_MHz >= 174) && (assignments[i].Target.Freq_MHz <= 230))
                        || ((assignments[i].Target.Freq_MHz >= 470) && (assignments[i].Target.Freq_MHz <= 582))
                        || ((assignments[i].Target.Freq_MHz >= 582) && (assignments[i].Target.Freq_MHz <= 862))) == false)
                        {
                            isSuccess = false;
                        }
                    }
                }
            }
            return isSuccess;
        }
    }
}
