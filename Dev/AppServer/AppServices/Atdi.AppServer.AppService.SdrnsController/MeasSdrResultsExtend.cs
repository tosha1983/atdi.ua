using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.AppServer.Contracts.Sdrns
{
    public static class MeasSdrResultsExtend
    {
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
