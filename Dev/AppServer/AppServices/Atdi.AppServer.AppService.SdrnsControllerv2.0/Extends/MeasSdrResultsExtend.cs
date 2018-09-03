using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    public static class MeasSdrResultsExtend
    {
        public static MeasurementResults CreateMeasurementResults2_0(this Atdi.DataModels.Sdrns.Device.MeasResults sdrRes)
        {
            MeasurementResults MRs = new MeasurementResults();
            //MRs.N = sdrRes.NN; // подумать 
            MRs.DataRank = sdrRes.SwNumber; // подумать
                                            //sdrRes.Measured
                                            //sdrRes.ResultId



            MRs.StationMeasurements = new StationMeasurements();
           
            //MRs.LocationSensorMeasurement = new LocationSensorMeasurement[1];
            MRs.AntVal = null;
            //if (sdrRes.MeasSDRLoc != null) MRs.LocationSensorMeasurement[0] = sdrRes. .MeasSDRLoc;
            if (MRs.Id == null) MRs.Id = new MeasurementResultsIdentifier();
            int ResId;
            if (int.TryParse(sdrRes.ResultId, out ResId))
            {
                MRs.Id.MeasSdrResultsId = ResId;
            }

            int SensorId;
            int MeasTaskId;
            int SubMeasTaskId;
            int SubMeasTaskStationId;

            int NumValue;
            if (int.TryParse(sdrRes.TaskId, out NumValue))
            {
                ClassesDBGetTasks.GetMeasTaskSDRNum(NumValue, out MeasTaskId, out SubMeasTaskId, out SubMeasTaskStationId, out SensorId);
                MRs.StationMeasurements.StationId = new SensorIdentifier();
                MRs.StationMeasurements.StationId.Value = SensorId;
                MRs.Id.MeasTaskId = new MeasTaskIdentifier();
                MRs.Id.MeasTaskId.Value = MeasTaskId;
                MRs.Id.SubMeasTaskId = SubMeasTaskId;
                MRs.Id.SubMeasTaskStationId = SubMeasTaskStationId;
            }

            
            MRs.TimeMeas = sdrRes.Measured;
            MRs.TypeMeasurements =  MeasurementType.MonitoringStations;
            MRs.Status = "N";
            if (sdrRes.StationResults != null)
            {
                MRs.ResultsMeasStation = new ResultsMeasurementsStation[sdrRes.StationResults.Length];
                for (int i = 0; i < sdrRes.StationResults.Length; i++)
                {
                    if (sdrRes.StationResults[i] != null)
                    {
                        if (sdrRes.StationResults[i].GeneralResult != null)
                        {
                            MRs.ResultsMeasStation[i] = new ResultsMeasurementsStation();
                            MRs.ResultsMeasStation[i].GeneralResult = new MeasurementsParameterGeneral();
                            MRs.ResultsMeasStation[i].GeneralResult.CentralFrequency = sdrRes.StationResults[i].GeneralResult.CentralFrequencyMeas_MHz;
                            MRs.ResultsMeasStation[i].GeneralResult.CentralFrequencyMeas = sdrRes.StationResults[i].GeneralResult.CentralFrequencyMeas_MHz;
                            MRs.ResultsMeasStation[i].GeneralResult.DurationMeas = sdrRes.StationResults[i].GeneralResult.MeasDuration_sec;
                            MRs.ResultsMeasStation[i].GeneralResult.LevelsSpecrum = sdrRes.StationResults[i].GeneralResult.LevelsSpectrum_dBm;
                            MRs.ResultsMeasStation[i].GeneralResult.MarkerIndex = sdrRes.StationResults[i].GeneralResult.BandwidthResult.MarkerIndex;
                            MRs.ResultsMeasStation[i].GeneralResult.T1 = sdrRes.StationResults[i].GeneralResult.BandwidthResult.T1;
                            MRs.ResultsMeasStation[i].GeneralResult.T2 = sdrRes.StationResults[i].GeneralResult.BandwidthResult.T2;
                            MRs.ResultsMeasStation[i].GeneralResult.TimeFinishMeas = sdrRes.StationResults[i].GeneralResult.MeasFinishTime;
                            MRs.ResultsMeasStation[i].GeneralResult.TimeStartMeas = sdrRes.StationResults[i].GeneralResult.MeasStartTime;
                            MRs.ResultsMeasStation[i].GeneralResult.MaskBW = new MaskElements[sdrRes.StationResults[i].GeneralResult.BWMask.Length];

                            for (int j = 0; j < sdrRes.StationResults[i].GeneralResult.BWMask.Length; j++)
                            {
                                MRs.ResultsMeasStation[i].GeneralResult.MaskBW[j] = new MaskElements();
                                MRs.ResultsMeasStation[i].GeneralResult.MaskBW[j].BW = sdrRes.StationResults[i].GeneralResult.BWMask[j].BW_kHz;
                                MRs.ResultsMeasStation[i].GeneralResult.MaskBW[j].level = sdrRes.StationResults[i].GeneralResult.BWMask[j].Level_dB;
                            }

                            MRs.ResultsMeasStation[i].GeneralResult.OffsetFrequency = sdrRes.StationResults[i].GeneralResult.OffsetFrequency_mk;
                            MRs.ResultsMeasStation[i].GeneralResult.SpecrumStartFreq = sdrRes.StationResults[i].GeneralResult.SpectrumStartFreq_MHz;
                            MRs.ResultsMeasStation[i].GeneralResult.SpecrumSteps = sdrRes.StationResults[i].GeneralResult.SpectrumSteps_kHz;
                        }
                        MRs.ResultsMeasStation[i].GlobalSID = sdrRes.StationResults[i].TaskGlobalSid;
                        MRs.ResultsMeasStation[i].MeasGlobalSID = sdrRes.StationResults[i].RealGlobalSid;
                        int SectorId; if (int.TryParse(sdrRes.StationResults[i].SectorId, out SectorId)) MRs.ResultsMeasStation[i].IdSector = SectorId;
                        int Idstation; if (int.TryParse(sdrRes.StationResults[i].StationId, out Idstation)) MRs.ResultsMeasStation[i].Idstation = Idstation;

                        MRs.ResultsMeasStation[i].LevelMeasurements = new LevelMeasurementsCar[sdrRes.StationResults[i].LevelResults.Length];
                        for (int j = 0; j < sdrRes.StationResults[i].LevelResults.Length; j++)
                        {
                            MRs.ResultsMeasStation[i].LevelMeasurements[j] = new LevelMeasurementsCar();
                            if (sdrRes.StationResults[i] != null)
                            {
                                if (sdrRes.StationResults[i].LevelResults[j].Location != null) MRs.ResultsMeasStation[i].LevelMeasurements[j].Altitude = sdrRes.StationResults[i].LevelResults[j].Location.ASL;
                                if (sdrRes.StationResults[i].GeneralResult.BandwidthResult != null) MRs.ResultsMeasStation[i].LevelMeasurements[j].BW = sdrRes.StationResults[i].GeneralResult.BandwidthResult.Bandwidth_kHz;
                                if (sdrRes.StationResults[i].GeneralResult != null) MRs.ResultsMeasStation[i].LevelMeasurements[j].CentralFrequency = (decimal)sdrRes.StationResults[i].GeneralResult.CentralFrequency_MHz;
                                MRs.ResultsMeasStation[i].LevelMeasurements[j].DifferenceTimestamp = sdrRes.StationResults[i].LevelResults[j].DifferenceTimeStamp_ns;
                                if (sdrRes.StationResults[i].LevelResults[j].Location != null) MRs.ResultsMeasStation[i].LevelMeasurements[j].Lat = sdrRes.StationResults[i].LevelResults[j].Location.Lat;
                                if (sdrRes.StationResults[i].LevelResults[j].Location != null) MRs.ResultsMeasStation[i].LevelMeasurements[j].Lon = sdrRes.StationResults[i].LevelResults[j].Location.Lon;
                                MRs.ResultsMeasStation[i].LevelMeasurements[j].LeveldBm = sdrRes.StationResults[i].LevelResults[j].Level_dBm;
                                MRs.ResultsMeasStation[i].LevelMeasurements[j].LeveldBmkVm = sdrRes.StationResults[i].LevelResults[j].Level_dBmkVm;
                                MRs.ResultsMeasStation[i].LevelMeasurements[j].RBW = sdrRes.StationResults[i].GeneralResult.RBW_kHz;
                                MRs.ResultsMeasStation[i].LevelMeasurements[j].TimeOfMeasurements = sdrRes.StationResults[i].LevelResults[j].MeasurementTime;
                                MRs.ResultsMeasStation[i].LevelMeasurements[j].VBW = sdrRes.StationResults[i].GeneralResult.VBW_kHz;
                            }
                        }

                        MRs.ResultsMeasStation[i].Status = sdrRes.StationResults[i].Status;
                        //MRs.StationMeasurements.StationId.Value
                    }
                }
            }
            return MRs;
        }

        public static MeasurementResults CreateMeasurementResults(this MeasSdrResults sdrRes)
        {
            MeasurementResults MRs = new MeasurementResults();

            MRs.N = sdrRes.NN; // подумать 
            MRs.DataRank = sdrRes.SwNumber; // подумать
            MRs.StationMeasurements = new StationMeasurements();
            MRs.StationMeasurements.StationId = sdrRes.SensorId;
            MRs.LocationSensorMeasurement = new LocationSensorMeasurement[1];
            MRs.LocationSensorMeasurement[0] = new LocationSensorMeasurement();
            if (sdrRes.MeasSDRLoc != null) MRs.LocationSensorMeasurement[0] = sdrRes.MeasSDRLoc;
            if (MRs.Id == null) MRs.Id = new MeasurementResultsIdentifier();
            MRs.Id.MeasSdrResultsId = sdrRes.Id;
            if (sdrRes.MeasDataType==  MeasurementType.SpectrumOccupation)   MRs.Id.MeasSdrResultsId = sdrRes.NN;
            if (sdrRes.MeasTaskId != null) MRs.Id.MeasTaskId = sdrRes.MeasTaskId;
            if (sdrRes.MeasSubTaskId != null) MRs.Id.SubMeasTaskId = sdrRes.MeasSubTaskId.Value;
            MRs.Id.SubMeasTaskStationId = sdrRes.MeasSubTaskStationId;
            MRs.TimeMeas = sdrRes.DataMeas;
            MRs.TypeMeasurements = sdrRes.MeasDataType;
            MRs.ResultsMeasStation = sdrRes.ResultsMeasStation;
            MRs.Status = "N";
            if (sdrRes.FSemples != null)
            { // если измерение не online
              // присвоение частот
                if (sdrRes.FSemples.Count() > 0)
                {
                    List<FrequencyMeasurement> ListFM = new List<FrequencyMeasurement>();
                    for (int i = 0; i < sdrRes.FSemples.Count(); i++)
                    {
                        FrequencyMeasurement FM = new FrequencyMeasurement();
                        FM.Freq = sdrRes.FSemples[i].Freq;
                        //FM.Id = FSemples[i].Id;
                        FM.Id = i;
                        ListFM.Add(FM);
                    }
                    MRs.FrequenciesMeasurements = ListFM.ToArray();
                    //присвоение результатов измерений
                    if (sdrRes.MeasDataType == MeasurementType.Level)
                    {
                        List<LevelMeasurementResult> L_MR = new List<LevelMeasurementResult>();
                        for (int i = 0; i < sdrRes.FSemples.Count(); i++)
                        {
                            LevelMeasurementResult MR = new LevelMeasurementResult();
                            MR.Id = new MeasurementResultIdentifier();
                            MR.Id.Value = i;
                            MR.Value = sdrRes.FSemples[i].LeveldBm;
                            //MR.Value = sdrRes.FSemples[i].LeveldBmkVm;
                            MR.PMax = sdrRes.FSemples[i].LevelMaxdBm;
                            MR.PMin = sdrRes.FSemples[i].LevelMindBm;
                            L_MR.Add(MR);
                        }
                        MRs.MeasurementsResults = L_MR.ToArray();
                    }
                    if (sdrRes.MeasDataType == MeasurementType.SpectrumOccupation)
                    {
                        List<SpectrumOccupationMeasurementResult> L_MR = new List<SpectrumOccupationMeasurementResult>();
                        for (int i = 0; i < sdrRes.FSemples.Count(); i++)
                        {
                            SpectrumOccupationMeasurementResult MR = new SpectrumOccupationMeasurementResult();
                            MR.Id = new MeasurementResultIdentifier();
                            MR.Id.Value = i;
                            MR.Occupancy = sdrRes.FSemples[i].OcupationPt;
                            L_MR.Add(MR);
                        }
                        MRs.MeasurementsResults = L_MR.ToArray();
                    }
                }
                else
                {
                    if (sdrRes.Level != null)
                    {
                        if (sdrRes.MeasDataType == MeasurementType.Level)
                        {
                            List<LevelMeasurementOnlineResult> L_MR = new List<LevelMeasurementOnlineResult>();
                            for (int i = 0; i < sdrRes.Level.Length; i++)
                            {
                                LevelMeasurementOnlineResult MR = new LevelMeasurementOnlineResult();
                                MR.Id = new MeasurementResultIdentifier();
                                MR.Id.Value = i;
                                MR.Value = sdrRes.Level[i];
                                L_MR.Add(MR);
                            }
                            MRs.MeasurementsResults = L_MR.ToArray();
                        }
                    }
                    if (sdrRes.Freqs != null)
                    {
                        List<FrequencyMeasurement> ListFM = new List<FrequencyMeasurement>();
                        for (int i = 0; i < sdrRes.Freqs.Length; i++)
                        {
                            FrequencyMeasurement FM = new FrequencyMeasurement();
                            FM.Freq = sdrRes.Freqs[i];
                            FM.Id = i;
                            ListFM.Add(FM);
                        }
                        MRs.FrequenciesMeasurements = ListFM.ToArray();
                    }
                }
            }
            else
            {// для online измерений 
                if (sdrRes.Level != null)
                {
                    if (sdrRes.MeasDataType == MeasurementType.Level)
                    {
                        List<LevelMeasurementOnlineResult> L_MR = new List<LevelMeasurementOnlineResult>();
                        for (int i = 0; i < sdrRes.Level.Length; i++)
                        {
                            LevelMeasurementOnlineResult MR = new LevelMeasurementOnlineResult();
                            MR.Id = new MeasurementResultIdentifier();
                            MR.Id.Value = i;
                            MR.Value = sdrRes.Level[i];
                            L_MR.Add(MR);
                        }
                        MRs.MeasurementsResults = L_MR.ToArray();
                    }
                }
                if (sdrRes.Freqs != null)
                {
                    List<FrequencyMeasurement> ListFM = new List<FrequencyMeasurement>();
                    for (int i = 0; i < sdrRes.Freqs.Length; i++)
                    {
                        FrequencyMeasurement FM = new FrequencyMeasurement();
                        FM.Freq = sdrRes.Freqs[i];
                        FM.Id = i;
                        ListFM.Add(FM);
                    }
                    MRs.FrequenciesMeasurements = ListFM.ToArray();
                }
            }
            return MRs;
        }

    }
}

/*
    sdrRes.Measured
    sdrRes.ResultId
    sdrRes.Status
    sdrRes.SwNumber
    sdrRes.TaskId
    sdrRes.StationResults[0].GeneralResult.BandwidthResult.Bandwidth_kHz
    sdrRes.StationResults[0].GeneralResult.BandwidthResult.MarkerIndex
    sdrRes.StationResults[0].GeneralResult.BandwidthResult.T1
    sdrRes.StationResults[0].GeneralResult.BandwidthResult.T2
    sdrRes.StationResults[0].GeneralResult.BandwidthResult.TraceCount
    sdrRes.StationResults[0].GeneralResult.BandwidthResult.СorrectnessEstimations
    sdrRes.StationResults[0].GeneralResult.BWMask[0].BW_kHz
    sdrRes.StationResults[0].GeneralResult.BWMask[0].Level_dB
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.BandWidth
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.BaseID
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.BSIC
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.ChannelNumber
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.CID
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.Code
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.CtoI
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.ECI
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.eNodeBId
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.Freq
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.IcIo
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.INBAND_POWER
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.InfoBlocks
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.ISCP
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.LAC
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.Location.AGL
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.Location.ASL
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.Location.Lat
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.Location.Lon
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.MCC
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.MNC
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.NID
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.PCI
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.PN
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.Power
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.Ptotal
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.RNC
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.RSCP
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.RSRP
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.RSRQ
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.SC
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.SID
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.TAC
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.TypeCDMAEVDO
    sdrRes.StationResults[0].GeneralResult.StationSysInfo.UCID
    sdrRes.StationResults[0].GeneralResult.CentralFrequencyMeas_MHz
    sdrRes.StationResults[0].GeneralResult.CentralFrequency_MHz
    sdrRes.StationResults[0].GeneralResult.LevelsSpectrum_dBm
    sdrRes.StationResults[0].GeneralResult.MeasDuration_sec
    sdrRes.StationResults[0].GeneralResult.MeasFinishTime
    sdrRes.StationResults[0].GeneralResult.MeasStartTime
    sdrRes.StationResults[0].GeneralResult.OffsetFrequency_mk
    sdrRes.StationResults[0].GeneralResult.RBW_kHz
    sdrRes.StationResults[0].GeneralResult.SpectrumStartFreq_MHz
    sdrRes.StationResults[0].GeneralResult.SpectrumSteps_kHz
    sdrRes.StationResults[0].GeneralResult.VBW_kHz
    sdrRes.StationResults[0].LevelResults[0].DifferenceTimeStamp_ns
    sdrRes.StationResults[0].LevelResults[0].Level_dBm
    sdrRes.StationResults[0].LevelResults[0].Level_dBmkVm
    sdrRes.StationResults[0].LevelResults[0].Location.AGL
    sdrRes.StationResults[0].LevelResults[0].Location.ASL
    sdrRes.StationResults[0].LevelResults[0].Location.Lat
    sdrRes.StationResults[0].LevelResults[0].Location.Lon
    sdrRes.StationResults[0].LevelResults[0].MeasurementTime
    sdrRes.StationResults[0].RealGlobalSid
    sdrRes.StationResults[0].SectorId
    sdrRes.StationResults[0].Standard
    sdrRes.StationResults[0].StationId
    sdrRes.StationResults[0].Status
    sdrRes.StationResults[0].TaskGlobalSid

sdrRes.Routes[0].RouteId
sdrRes.Routes[0].RoutePoints[0].AGL
sdrRes.Routes[0].RoutePoints[0].ASL
sdrRes.Routes[0].RoutePoints[0].Lat
sdrRes.Routes[0].RoutePoints[0].Lon
sdrRes.Routes[0].RoutePoints[0].PointStayType
sdrRes.Routes[0].RoutePoints[0].StayLength
sdrRes.Routes[0].RoutePoints[0].TimeFrom

  */
