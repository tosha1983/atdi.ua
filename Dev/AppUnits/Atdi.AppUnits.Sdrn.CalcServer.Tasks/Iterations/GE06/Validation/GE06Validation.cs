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
        public static bool ValidationAllotment(BroadcastingAllotment allotments, out string notValidAdmRefIds)
        {
            string notIdentified = "Allotment with parameters that could not be identified;";
            var notValidParameters = new List<string>();
            notValidAdmRefIds = "";
            bool isSuccess = true;
            if (allotments != null)
            {
                //AdminData
                if (allotments.AdminData == null)
                {
                    isSuccess = false;
                    notValidParameters.Add("'AdminData' is null");
                }
                else if (allotments.AdminData != null)
                {
                    if (string.IsNullOrEmpty(allotments.AdminData.Adm))
                    {
                        isSuccess = false;
                        notValidParameters.Add("'AdminData.Adm' is null or empty");
                    }
                    if (string.IsNullOrEmpty(allotments.AdminData.AdmRefId))
                    {
                        isSuccess = false;
                        notValidParameters.Add("'AdminData.AdmRefId' is null or empty");
                    }
                }
                //AllotmentParameters
                if (allotments.AllotmentParameters == null)
                {
                    isSuccess = false;
                    notValidParameters.Add("'AllotmentParameters' is null");
                }
                else if (allotments.AllotmentParameters != null)
                {
                    if (allotments.AllotmentParameters.ContourId == 0)
                    {
                        isSuccess = false;
                        notValidParameters.Add("'AllotmentParameters.ContourId' is 0");
                    }
                    if (allotments.AllotmentParameters.Contur == null)
                    {
                        isSuccess = false;
                        notValidParameters.Add("'AllotmentParameters.Contur' is null");
                    }
                    else if (allotments.AllotmentParameters.Contur.Length < 3)
                    {
                        isSuccess = false;
                        notValidParameters.Add("'AllotmentParameters.Contur.Length' < 3");
                    }
                    else if (!ValidationAllotmentsPoint(allotments.AllotmentParameters.Contur))
                    {
                        isSuccess = false;
                        notValidParameters.Add("'AllotmentParameters.Contur' - not valid points");
                    }

                }

                // EmissionCharacteristics
                if (allotments.EmissionCharacteristics == null)
                {
                    isSuccess = false;
                    notValidParameters.Add("'EmissionCharacteristics' is null");
                }
                else if (allotments.EmissionCharacteristics != null)
                {
                    if ((((allotments.EmissionCharacteristics.Freq_MHz >= 174) && (allotments.EmissionCharacteristics.Freq_MHz <= 230))
                        || ((allotments.EmissionCharacteristics.Freq_MHz >= 470) && (allotments.EmissionCharacteristics.Freq_MHz <= 582))
                        || ((allotments.EmissionCharacteristics.Freq_MHz >= 582) && (allotments.EmissionCharacteristics.Freq_MHz <= 862))) == false)
                    {
                        isSuccess = false;
                        notValidParameters.Add("'EmissionCharacteristics.Freq_MHz' not valid");
                    }
                }
                else if (allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.Unknown)
                {
                    isSuccess = false;
                    notValidParameters.Add("'EmissionCharacteristics.RefNetworkConfig' is Unknown");
                }
                if (!isSuccess)
                {
                    if (allotments.AdminData != null)
                    {
                        if (!string.IsNullOrEmpty(allotments.AdminData.AdmRefId))
                        {
                            notValidAdmRefIds += $"AdmRefId = '{allotments.AdminData.AdmRefId}'";
                        }
                        else
                        {
                            notValidAdmRefIds += notIdentified;
                        }
                    }
                    else
                    {
                        notValidAdmRefIds += notIdentified;
                    }
                    notValidAdmRefIds = notValidAdmRefIds+";" +string.Join(";", notValidParameters);
                }
            }
            return isSuccess;
        }
        private static bool ValidationAllotmentsPoint (AreaPoint[] points)
        {
            double LonMin = -180;
            double LonMax = 180;
            double LatMin = -80;
            double LatMax = 80;
            bool isSuccess = true;
            for (int i = 0; points.Length > i; i++)
            {
                if ((points[i].Lat_DEC<LatMin)&&(points[i].Lat_DEC > LatMax)) { isSuccess = false; break; }
                if ((points[i].Lon_DEC < LonMin)&& (points[i].Lon_DEC > LonMax)) { isSuccess = false; break; }
            }
            return isSuccess;
        }
        /// <summary>
        /// Валидация параметров для Assignment
        /// </summary>
        /// <param name="assignments"></param>
        /// <returns></returns>
        public static bool ValidationAssignment(BroadcastingAssignment[] assignments, out string notValidAdmRefIds)
        {
            var notValidParameters = new List<string>();
            notValidAdmRefIds = "";
            string notIdentified = "Assignment with parameters that could not be identified;";
            bool isSuccess = true;
            if (assignments != null)
            {
                for (int i = 0; i < assignments.Length; i++)
                {
                    isSuccess = true;
                    // AdmData
                    if (assignments[i].AdmData == null)
                    {
                        isSuccess = false;
                        notValidParameters.Add("'AdmData' is null");
                    }
                    else if (assignments[i].AdmData != null)
                    {
                        if (string.IsNullOrEmpty(assignments[i].AdmData.Adm))
                        {
                            isSuccess = false;
                            notValidParameters.Add("'AdmData.Adm' is null or empty");
                        }
                        if (string.IsNullOrEmpty(assignments[i].AdmData.AdmRefId))
                        {
                            isSuccess = false;
                            notValidParameters.Add("'AdmData.AdmRefId' is null or empty");
                        }
                    }
                    //AntennaCharacteristics
                    if (assignments[i].AntennaCharacteristics == null)
                    {
                        isSuccess = false;
                        notValidParameters.Add("'AntennaCharacteristics' is null");
                    }
                    else if (assignments[i].AntennaCharacteristics != null)
                    {
                        //MaxEffHeight_m
                        if (((assignments[i].AntennaCharacteristics.AglHeight_m >= 0) && (assignments[i].AntennaCharacteristics.AglHeight_m <= 800)) == false)
                        {
                            isSuccess = false;
                            notValidParameters.Add("'AntennaCharacteristics.AglHeight_m' not valid");
                        }
                        //EffHeight_m
                        if ((assignments[i].AntennaCharacteristics.EffHeight_m != null) && (assignments[i].AntennaCharacteristics.EffHeight_m.Length>0))
                        {
                            if (assignments[i].AntennaCharacteristics.EffHeight_m.Length != 36)
                            {
                                isSuccess = false;
                                notValidParameters.Add("'AntennaCharacteristics.EffHeight_m' length != 36");
                            }
                            for (int j = 0; j < assignments[i].AntennaCharacteristics.EffHeight_m.Length; j++)
                            {
                                var effHeight_m = assignments[i].AntennaCharacteristics.EffHeight_m[j];
                                if (((effHeight_m >= -3000) && (effHeight_m <= 3000)) == false)
                                {
                                    isSuccess = false;
                                    notValidParameters.Add("'AntennaCharacteristics.EffHeight_m' not valid");
                                }
                            }
                        }
                        //EmissionCharacteristics
                        if (assignments[i].EmissionCharacteristics == null)
                        {
                            isSuccess = false;
                            notValidParameters.Add("'EmissionCharacteristics' is null");
                        }
                        else if (assignments[i].EmissionCharacteristics != null)
                        {
                            if ((((assignments[i].EmissionCharacteristics.Freq_MHz >= 174) && (assignments[i].EmissionCharacteristics.Freq_MHz <= 230))
                            || ((assignments[i].EmissionCharacteristics.Freq_MHz >= 470) && (assignments[i].EmissionCharacteristics.Freq_MHz <= 582))
                            || ((assignments[i].EmissionCharacteristics.Freq_MHz >= 582) && (assignments[i].EmissionCharacteristics.Freq_MHz <= 862))) == false)
                            {
                                isSuccess = false;
                                notValidParameters.Add("'EmissionCharacteristics.Freq_MHz' not valid");
                            }

                            if (assignments[i].AntennaCharacteristics.Direction == AntennaDirectionType.D)
                            {
                                if ((assignments[i].EmissionCharacteristics.Polar == PolarType.H) || (assignments[i].EmissionCharacteristics.Polar == PolarType.M))
                                {
                                    if (assignments[i].EmissionCharacteristics.ErpH_dBW > 53)
                                    {
                                        isSuccess = false;
                                        notValidParameters.Add("'EmissionCharacteristics.ErpH_dBW' not valid");
                                    }
                                }
                                if ((assignments[i].EmissionCharacteristics.Polar == PolarType.V) || (assignments[i].EmissionCharacteristics.Polar == PolarType.M))
                                {
                                    if (assignments[i].EmissionCharacteristics.ErpV_dBW > 53)
                                    {
                                        isSuccess = false;
                                        notValidParameters.Add("'EmissionCharacteristics.ErpV_dBW' not valid");
                                    }
                                }
                            }
                        }
                        
                        if (assignments[i].AntennaCharacteristics.Direction == AntennaDirectionType.D)
                        {
                            //DiagrV
                            if ((assignments[i].EmissionCharacteristics.Polar == PolarType.V) || (assignments[i].EmissionCharacteristics.Polar == PolarType.M))
                            {
                                if (assignments[i].AntennaCharacteristics.DiagrV == null)
                                {
                                    isSuccess = false;
                                    notValidParameters.Add("'AntennaCharacteristics.DiagrV' is null");
                                }
                                else if ((assignments[i].AntennaCharacteristics.DiagrV != null) && (assignments[i].AntennaCharacteristics.DiagrV.Length>0))
                                {
                                    if (assignments[i].AntennaCharacteristics.DiagrV.Length != 36)
                                    {
                                        isSuccess = false;
                                        notValidParameters.Add("'AntennaCharacteristics.DiagrV' length != 36");
                                    }
                                    for (int j = 0; j < assignments[i].AntennaCharacteristics.DiagrV.Length; j++)
                                    {
                                        var diagrV = assignments[i].AntennaCharacteristics.DiagrV[j];
                                        if (((diagrV >= 0) && (diagrV <= 40)) == false)
                                        {
                                            isSuccess = false;
                                            notValidParameters.Add("'AntennaCharacteristics.DiagrV' not valid");
                                        }
                                    }
                                }
                            }
                            if ((assignments[i].EmissionCharacteristics.Polar == PolarType.H) || (assignments[i].EmissionCharacteristics.Polar == PolarType.M))
                            {
                                //DiagrH
                                if (assignments[i].AntennaCharacteristics.DiagrH == null)
                                {
                                    isSuccess = false;
                                    notValidParameters.Add("'AntennaCharacteristics.DiagrH' is null");
                                }
                                else if ((assignments[i].AntennaCharacteristics.DiagrH != null) && (assignments[i].AntennaCharacteristics.DiagrH.Length>0))
                                {
                                    if (assignments[i].AntennaCharacteristics.DiagrH.Length != 36)
                                    {
                                        isSuccess = false;
                                        notValidParameters.Add("'AntennaCharacteristics.DiagrH' length != 36");
                                    }
                                    for (int j = 0; j < assignments[i].AntennaCharacteristics.DiagrH.Length; j++)
                                    {
                                        var diagrH = assignments[i].AntennaCharacteristics.DiagrH[j];
                                        if (((diagrH >= 0) && (diagrH <= 40)) == false)
                                        {
                                            isSuccess = false;
                                            notValidParameters.Add("'AntennaCharacteristics.DiagrH' not valid");
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
                        notValidParameters.Add("'SiteParameters' is null");
                    }
                    else if (assignments[i].SiteParameters != null)
                    {
                        AreaPoint[] areaPoints = new AreaPoint[1];
                        areaPoints[0] = new AreaPoint() { Lat_DEC = assignments[i].SiteParameters.Lat_Dec, Lon_DEC = assignments[i].SiteParameters.Lon_Dec };
                        if (!ValidationAllotmentsPoint(areaPoints))
                        {
                            isSuccess = false;
                            notValidParameters.Add("'SiteParameters' coordinates not valid");
                        }
                    }
                    if (!isSuccess)
                    {
                        if (assignments[i].AdmData != null)
                        {
                            if (!string.IsNullOrEmpty(assignments[i].AdmData.AdmRefId))
                            {
                                notValidAdmRefIds += $"AdmRefId = '{assignments[i].AdmData.AdmRefId}'";
                            }
                            else if (assignments[i].SiteParameters != null)
                            {
                                if (!string.IsNullOrEmpty(assignments[i].SiteParameters.Name))
                                {
                                    notValidAdmRefIds += $"Site.Name = '{assignments[i].SiteParameters.Name}'";
                                }
                                else
                                {
                                    notValidAdmRefIds += notIdentified;
                                }
                            }
                            else
                            {
                                notValidAdmRefIds += notIdentified;
                            }
                        }
                        else if (assignments[i].SiteParameters != null)
                        {
                            if (!string.IsNullOrEmpty(assignments[i].SiteParameters.Name))
                            {
                                notValidAdmRefIds += $"Site.Name = '{assignments[i].SiteParameters.Name}'";
                            }
                            else
                            {
                                notValidAdmRefIds += notIdentified;
                            }
                        }
                        else 
                        {
                            notValidAdmRefIds += notIdentified;
                        }

                        notValidAdmRefIds = notValidAdmRefIds + ";" + string.Join(";", notValidParameters);
                    }
                }
            }
            return isSuccess;
        }
    }
}
