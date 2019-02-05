using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Modules.MonitoringProcess;
using Atdi.Sdrn.Modules.MonitoringProcess;
using NHibernate;
using Atdi.AppUnits.Sdrn.ControlA.Bus;
using Atdi.AppUnits.Sdrn.ControlA;
using NHibernate.Criterion;

namespace Atdi.AppUnits.Sdrn.ControlA.ManageDB
{

    /// <summary>
    /// 
    /// </summary>
    public class LoadDataMeasTask
    {

        public int GetMaxIdFromResults()
        {
            int maxId = 0;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var max = session.QueryOver<NH_MeasSDRResults>().Select(Projections.Max<NH_MeasSDRResults>(x => x.ID)).SingleOrDefault<object>();
                    maxId = (max == null ? 0 : Convert.ToInt32(max));
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.GetMaxIdFromResults, Events.GetMaxIdFromResults, ex.Message, null);
            }
            return maxId;
        }



        public void GetIdentTaskFromMeasTaskSDR(int id, ref int? MeasTaskId, ref int? MeasSubTaskId, ref int? MeasSubTaskStationId, ref int? SensorId)
        {
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var list = session.QueryOver<NH_MeasTaskSDR>().Where(x => x.ID == id).List();
                    if ((list != null) && (list.Count > 0))
                    {
                        MeasTaskId = list[0].MeasTaskId.Value;
                        MeasSubTaskId = list[0].MeasSubTaskId.Value;
                        MeasSubTaskStationId = list[0].MeasSubTaskStationId.Value;
                        SensorId = list[0].SensorId.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.GetIdentTaskFromMeasTaskSDR, Events.GetIdentTaskFromMeasTaskSDR, ex.Message, null);
            }
        }


        public DateTime? LoadDataMeasByTaskId(int TaskId)
        {
            DateTime? DataMeas = null;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var nHMeasTaskSDR = session.QueryOver<NH_MeasSDRResults>().Where(t => t.status != AllStatusSensor.Z.ToString() && t.MeasTaskId == TaskId).OrderBy(z => z.ID).Desc;
                    var listNHMeasTaskSDR = nHMeasTaskSDR.List();
                    if (listNHMeasTaskSDR.Count > 0)
                    {
                        foreach (var s in listNHMeasTaskSDR)
                        {
                            DataMeas = s.DataMeas;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.LoadDataMeasByTaskId, Events.LoadDataMeasByTaskId, ex.Message, null);
            }
            return DataMeas;
        }

        /// <summary>
        /// Загрузка результатов измерений изБД в память
        /// </summary>
        public List<MeasSdrResults> LoadActiveTaskSdrResults()
        {
            var listRes = new List<MeasSdrResults>();
            try
            {
                var sensorDBExtension = new SensorDb();
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var nHMeasSDRResults = session.QueryOver<NH_MeasSDRResults>().Where(t => t.status == AllStatusSensor.C.ToString()).List();
                    if (nHMeasSDRResults.Count > 0)
                    {
                        var sensor = sensorDBExtension.GetCurrentSensor();
                        if (sensor != null)
                        {
                            foreach (var s in nHMeasSDRResults)
                            {
                                var sDrRes = new MeasSdrResults();
                                sDrRes.DataMeas = s.DataMeas.GetValueOrDefault();
                                sDrRes.Id = s.ID.GetValueOrDefault();
                                MeasurementType outMeasurementType;
                                if (Enum.TryParse<MeasurementType>(s.MeasDataType, out outMeasurementType))
                                    sDrRes.MeasDataType = outMeasurementType;

                                sDrRes.MeasSubTaskId = new MeasTaskIdentifier();
                                sDrRes.MeasSubTaskId.Value = s.MeasSubTaskId.GetValueOrDefault();
                                sDrRes.MeasSubTaskStationId = s.MeasSubTaskStationId.GetValueOrDefault();
                                sDrRes.MeasTaskId = new MeasTaskIdentifier();
                                sDrRes.MeasTaskId.Value = s.MeasTaskId.GetValueOrDefault();
                                sDrRes.NN = s.NN.GetValueOrDefault();
                                sDrRes.SensorId = new SensorIdentifier();
                                sDrRes.SensorId.Value = s.SensorId.GetValueOrDefault();
                                sDrRes.status = s.status;
                                sDrRes.SwNumber = s.SwNumber.GetValueOrDefault();
                                var semples = new List<FSemples>();
                                var nHSDRFSemples = session.QueryOver<NH_FSemples>().Where(t => t.ID_MeasSDRResults == s.ID).List();
                                if (nHSDRFSemples.Count > 0)
                                {
                                    foreach (var nHFSemples in nHSDRFSemples)
                                    {
                                        var smp = new FSemples();
                                        if (nHFSemples.Freq != null) smp.Freq = (float)nHFSemples.Freq.GetValueOrDefault();
                                        if (nHFSemples.LeveldBm != null) smp.LeveldBm = (float)nHFSemples.LeveldBm.GetValueOrDefault();
                                        if (nHFSemples.LeveldBmkVm != null) smp.LeveldBmkVm = (float)nHFSemples.LeveldBmkVm.GetValueOrDefault();
                                        if (nHFSemples.LevelMaxdBm != null) smp.LevelMaxdBm = (float)nHFSemples.LevelMaxdBm.GetValueOrDefault();
                                        if (nHFSemples.LevelMindBm != null) smp.LevelMindBm = (float)nHFSemples.LevelMindBm.GetValueOrDefault();
                                        if (nHFSemples.OcupationPt != null) smp.OcupationPt = (float)nHFSemples.OcupationPt.GetValueOrDefault();
                                        semples.Add(smp);
                                    }
                                }
                                sDrRes.FSemples = semples.ToArray();
                                var levels = new List<float>();
                                var nHMeasResultsLevel = session.QueryOver<NH_MeasResultsLevel>().Where(t => t.ID_NH_MeasSDRResults == s.ID).List();
                                if (nHMeasResultsLevel.Count > 0)
                                {
                                    foreach (NH_MeasResultsLevel sNHFSemples in nHMeasResultsLevel)
                                    {
                                        levels.Add((float)sNHFSemples.Level.GetValueOrDefault());
                                    }
                                }
                                sDrRes.Level = levels.ToArray();
                                var freqs = new List<float>();
                                var nHMeasResultsFreqs = session.QueryOver<NH_MeasResultsFreq>().Where(t => t.ID_NH_MeasSDRResults == s.ID).List();
                                if (nHMeasResultsFreqs.Count > 0)
                                {
                                    foreach (var sNHFSemples in nHMeasResultsFreqs)
                                    {
                                        freqs.Add((float)sNHFSemples.Freq.GetValueOrDefault());
                                    }
                                }
                                sDrRes.Freqs = freqs.ToArray();
                                sDrRes.MeasSDRLoc = new LocationSensorMeasurement();
                                var NHMeasSDRLoc = session.QueryOver<NH_LocationSensor>().Where(t => t.ID_MeasSDRResults == s.ID).List();
                                if (NHMeasSDRLoc.Count > 0)
                                {
                                    foreach (var sNHFMeasSDRLo in NHMeasSDRLoc)
                                    {
                                        if (sNHFMeasSDRLo.ASL != null) sDrRes.MeasSDRLoc.ASL = (float)sNHFMeasSDRLo.ASL.GetValueOrDefault();
                                        if (sNHFMeasSDRLo.Lat != null) sDrRes.MeasSDRLoc.Lat = (float)sNHFMeasSDRLo.Lat.GetValueOrDefault();
                                        if (sNHFMeasSDRLo.Lon != null) sDrRes.MeasSDRLoc.Lon = (float)sNHFMeasSDRLo.Lon.GetValueOrDefault();
                                    }
                                }
                                listRes.Add(sDrRes);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.LoadActiveTaskSdrResults, Events.LoadActiveTaskSdrResults, ex.Message, null);
            }
            return listRes;
        }

        /// <summary>
        /// Основной цикл обработки тасков
        /// </summary>
        public void ProcessBB60C()
        {
            int? MeasTaskId = null;
            int? MeasSubTaskId = null;
            int? MeasSubTaskStationId = null;
            int? SensorId = null;
            var sensorDBExtension = new SensorDb();
            var MeasProcessing = new MeasurementProcessing();
            var sensor = sensorDBExtension.GetCurrentSensor();
            if (sensor != null)
            {
                var loadDataMeasTask = new LoadDataMeasTask();
                var measSdrResults = new MeasSdrResults();
                var resHistory = new List<MeasSdrResults>();
                var svSdrRes = new SaveMeasSDRResults();
                var svTaskSDR = new SaveMeasTaskSDR();
                var taskAction = new Action(() =>
                {
                    while (true)
                    {
                        try
                        {
                            var mDR = new MeasSdrTask();
                            {
                                var outlist = loadDataMeasTask.GetAllMeasTaskSDR();
                                if (outlist != null)
                                {
                                    var asd = new List<KeyValuePair<MeasSdrTask, DateTime>>();
                                    var asi = new List<KeyValuePair<MeasSdrTask, int>>();
                                    var ids = new List<DateTime>();
                                    {
                                        foreach (var tskprio in outlist)
                                        {
                                            if (tskprio != null)
                                            {
                                                if ((tskprio.status == AllStatusSensor.O.ToString() || tskprio.status == AllStatusSensor.A.ToString()) && ((tskprio.Time_start < DateTime.Now) && (tskprio.Time_stop > DateTime.Now)))
                                                {
                                                    if (tskprio.MeasTaskId != null)
                                                    {
                                                        var DataMeas = loadDataMeasTask.LoadDataMeasByTaskId(tskprio.MeasTaskId.Value);
                                                        if (DataMeas == null)
                                                        {
                                                            mDR = tskprio;
                                                            asd.Clear();
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            asd.Add(new KeyValuePair<MeasSdrTask, DateTime>(tskprio, DataMeas.Value));
                                                            ids.Add(DataMeas.Value);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (asd.Count > 0)
                                        {
                                            ids.Sort();
                                            if (ids.Count > 0)
                                            {
                                                var vv = asd.Find(c => c.Value == ids[0]);
                                                if (vv.Key != null)
                                                {
                                                    mDR = vv.Key;
                                                }
                                            }
                                        }
                                        if (mDR.MeasTaskId == null)
                                        {
                                            var IR = new List<int>();
                                            foreach (var tskprio in outlist)
                                            {
                                                if (tskprio != null)
                                                {
                                                    if ((tskprio.status == AllStatusSensor.A.ToString()) && ((tskprio.Time_start < DateTime.Now) && (tskprio.Time_stop > DateTime.Now)))
                                                    {
                                                        asi.Add(new KeyValuePair<MeasSdrTask, int>(tskprio, tskprio.prio));
                                                        IR.Add(tskprio.prio);
                                                    }
                                                }
                                            }
                                            IR.Sort();
                                            IR.Reverse();
                                            if (IR.Count > 0)
                                            {
                                                var vv = asi.Find(c => c.Value == IR[0]);
                                                if (vv.Key != null)
                                                {
                                                    mDR = vv.Key;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (mDR == null)
                                {
                                    continue;
                                }
                                if (mDR.MeasTaskId != null)
                                {
                                    if ((mDR.Time_start < DateTime.Now) && (mDR.Time_stop > DateTime.Now))
                                    {
                                        var resH = resHistory.Find(t => t.MeasTaskId.Value == mDR.Id);
                                        if (resH != null)
                                        {
                                            var circulatingData = new CirculatingData();
                                            measSdrResults = MeasProcessing.TaskProcessing(Launcher._sdr, mDR, sensor, ref circulatingData, resH) as MeasSdrResults;
                                            resHistory.RemoveAll(t => t.MeasTaskId.Value == mDR.Id);
                                            resHistory.Add(measSdrResults);
                                            Launcher._logger.Info(Contexts.ThisComponent, Categories.ProcessMeasurements, string.Format(Events.resHNotNull.ToString(), resH.MeasTaskId.Value, resH.NN, mDR.Id));
                                        }
                                        else
                                        {
                                            var circulatingData = new CirculatingData();
                                            measSdrResults = MeasProcessing.TaskProcessing(Launcher._sdr, mDR, sensor, ref circulatingData, resH) as MeasSdrResults;
                                            resHistory.RemoveAll(t => t.MeasTaskId.Value == mDR.Id);
                                            resHistory.Add(measSdrResults);
                                            Launcher._logger.Info(Contexts.ThisComponent, Categories.ProcessMeasurements, string.Format(Events.resNotNull.ToString(), measSdrResults.MeasTaskId.Value, measSdrResults.NN, mDR.Id));
                                        }
                                        if (measSdrResults != null)
                                        {
                                            MeasSdrResults resCpy = new MeasSdrResults { DataMeas = measSdrResults.DataMeas, Freqs = measSdrResults.Freqs, FSemples = measSdrResults.FSemples, Id = measSdrResults.Id, Level = measSdrResults.Level, MeasDataType = measSdrResults.MeasDataType, MeasSDRLoc = measSdrResults.MeasSDRLoc, MeasSubTaskId = measSdrResults.MeasSubTaskId, MeasSubTaskStationId = measSdrResults.MeasSubTaskStationId, MeasTaskId = measSdrResults.MeasTaskId, NN = measSdrResults.NN, ResultsBandwidth = measSdrResults.ResultsBandwidth, ResultsMeasStation = measSdrResults.ResultsMeasStation, SensorId = measSdrResults.SensorId, status = measSdrResults.status, SwNumber = measSdrResults.SwNumber };
                                            int idxmax = 1;
                                            int maxValue = loadDataMeasTask.GetMaxIdFromResults();
                                            if (maxValue >= 0)
                                            {
                                                idxmax = maxValue == 0 ? 1 : maxValue + 1;
                                            }
                                            resCpy.Id = idxmax;
                                            loadDataMeasTask.GetIdentTaskFromMeasTaskSDR(mDR.Id, ref MeasTaskId, ref MeasSubTaskId, ref MeasSubTaskStationId, ref SensorId);
                                            resCpy.MeasTaskId = new MeasTaskIdentifier();
                                            resCpy.MeasTaskId.Value = MeasTaskId.Value;
                                            resCpy.MeasSubTaskId = new MeasTaskIdentifier();
                                            resCpy.MeasSubTaskId.Value = MeasSubTaskId.Value;
                                            resCpy.MeasSubTaskStationId = MeasSubTaskStationId.Value;
                                            resCpy.SensorId = new SensorIdentifier();
                                            resCpy.SensorId.Value = SensorId.Value;
                                            if (resCpy.FSemples != null)
                                            {
                                                if (resCpy.FSemples.Count() > 0)
                                                {
                                                    if (mDR.MeasDataType != MeasurementType.SpectrumOccupation)
                                                    {
                                                        svSdrRes.SaveMeasResultSDR(resCpy, AllStatusSensor.C.ToString());
                                                        Launcher._messagePublisher.Send("SendMeasSdrResults", resCpy);
                                                        if (SaveMeasSDRResults.SaveStatusMeasTaskSDRResults(resCpy, AllStatusSensor.C.ToString()))
                                                        {
                                                            SaveMeasSDRResults.SaveisSendMeasTaskSDRResults(resCpy);
                                                        }
                                                    }
                                                    else if (mDR.MeasDataType == MeasurementType.SpectrumOccupation)
                                                    {
                                                        svSdrRes.SaveMeasResultSDR(resCpy, AllStatusSensor.C.ToString());
                                                        Launcher._messagePublisher.Send("SendMeasSdrResults", resCpy);
                                                        Launcher._counterOnline = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if ((resCpy.Freqs != null) || (resCpy.Level != null))
                                                    {
                                                        Launcher._counterOnline++;
                                                        resCpy.Id = Launcher._counterOnline;
                                                        resCpy.status = mDR.status;
                                                        Launcher._messagePublisher.Send("SendMeasSdrResults", resCpy);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((resCpy.Freqs != null) || (resCpy.Level != null))
                                                {
                                                    Launcher._counterOnline++;
                                                    resCpy.Id = Launcher._counterOnline;
                                                    resCpy.status = mDR.status;
                                                    Launcher._messagePublisher.Send("SendMeasSdrResults", resCpy);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (mDR.Time_stop < DateTime.Now)
                                        {
                                            mDR.status = AllStatusSensor.C.ToString();
                                            svTaskSDR.ArchiveMeasTaskSDR(mDR, AllStatusSensor.C.ToString());
                                            MeasSdrResults resCpy = new MeasSdrResults { DataMeas = measSdrResults.DataMeas, Freqs = measSdrResults.Freqs, FSemples = measSdrResults.FSemples, Id = measSdrResults.Id, Level = measSdrResults.Level, MeasDataType = measSdrResults.MeasDataType, MeasSDRLoc = measSdrResults.MeasSDRLoc, MeasSubTaskId = measSdrResults.MeasSubTaskId, MeasSubTaskStationId = measSdrResults.MeasSubTaskStationId, MeasTaskId = measSdrResults.MeasTaskId, NN = measSdrResults.NN, ResultsBandwidth = measSdrResults.ResultsBandwidth, ResultsMeasStation = measSdrResults.ResultsMeasStation, SensorId = measSdrResults.SensorId, status = measSdrResults.status, SwNumber = measSdrResults.SwNumber };
                                            if (mDR.MeasDataType == MeasurementType.SpectrumOccupation)
                                            {
                                                svSdrRes.SaveMeasResultSDR(resCpy, AllStatusSensor.C.ToString());
                                            }
                                            var ResDX = FindMeasTaskSDR(resCpy.SensorId.Value, resCpy.MeasTaskId.Value, resCpy.MeasSubTaskStationId, resCpy.MeasSubTaskId.Value);
                                            if ((ResDX != null) && (ResDX.Count > 0))
                                            {
                                                ResDX[0].status = resCpy.status;
                                                SaveMeasTaskSDR saveMeasTaskSDR = new SaveMeasTaskSDR();
                                                saveMeasTaskSDR.SaveStatusMeasTaskSDR(ResDX[0]);
                                            }
                                        }
                                    }
                                    System.Threading.Thread.Yield();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Launcher._logger.Error(Contexts.ThisComponent, Categories.ProcessMeasurements, Events.ProcessMeasurements, ex.Message, null);
                        }
                    }
                });
                Task task = new Task(taskAction);
                task.Start();
            }

        }



        public List<MeasSdrTask> FindMeasTaskSDR(int SensorId, int MeasTaskId, int MeasSubTaskStationId, int MeasSubTaskId)
        {
            var listMeasSdrTask = new List<MeasSdrTask>();
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    #region NH_Meas_Task_SDR
                    {
                        session.Clear();
                        var nH_MeasTaskSDR = session.QueryOver<NH_MeasTaskSDR>().Where(t => t.MeasSubTaskId == MeasSubTaskId && t.MeasSubTaskStationId == MeasSubTaskStationId && t.MeasTaskId == MeasTaskId && t.SensorId == SensorId).List();
                        if (nH_MeasTaskSDR.Count > 0)
                        {
                            foreach (var s in nH_MeasTaskSDR)
                            {
                                var mDR = new MeasSdrTask();
                                mDR.MeasTaskId = new MeasTaskIdentifier();
                                if (s.MeasTaskId != null) mDR.MeasTaskId.Value = s.MeasTaskId.GetValueOrDefault();
                                mDR.MeasSubTaskId = new MeasTaskIdentifier();
                                if (s.MeasSubTaskId != null) mDR.MeasSubTaskId.Value = s.MeasSubTaskId.GetValueOrDefault();
                                if (s.ID != null) mDR.Id = s.ID.GetValueOrDefault();
                                if (s.MeasSubTaskStationId != null) mDR.MeasSubTaskStationId = s.MeasSubTaskStationId.GetValueOrDefault();
                                MeasurementType outMeasurementType;
                                if (Enum.TryParse<MeasurementType>(s.MeasDataType, out outMeasurementType))
                                    mDR.MeasDataType = outMeasurementType;
                                mDR.status = s.status;
                                mDR.SensorId = new SensorIdentifier();
                                if (s.SensorId != null) mDR.SensorId.Value = s.SensorId.GetValueOrDefault();
                                if (s.SwNumber != null) mDR.SwNumber = s.SwNumber.GetValueOrDefault();
                                if (s.Time_start != null) mDR.Time_start = s.Time_start.GetValueOrDefault();
                                if (s.Time_stop != null) mDR.Time_stop = s.Time_stop.GetValueOrDefault();
                                mDR.NumberScanPerTask = -999;
                                if (s.PerInterval != null) mDR.PerInterval = s.PerInterval.GetValueOrDefault();

                                SpectrumScanType outSpectrumScanType;
                                if (Enum.TryParse<SpectrumScanType>(s.TypeM, out outSpectrumScanType))
                                    mDR.TypeM = outSpectrumScanType;


                                mDR.MeasSDRSOParam = new MeasSdrSOParam();
                                var nHSDRSoParam = session.QueryOver<NH_MeasSDRSOParam>().Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                if (nHSDRSoParam.Count > 0)
                                {
                                    foreach (var s_so_param in nHSDRSoParam)
                                    {
                                        if (s_so_param.LevelMinOccup != null) mDR.MeasSDRSOParam.LevelMinOccup = s_so_param.LevelMinOccup.GetValueOrDefault();
                                        if (s_so_param.NChenal != null) mDR.MeasSDRSOParam.NChenal = s_so_param.NChenal.GetValueOrDefault();

                                        SpectrumOccupationType outSpectrumOccupationType;
                                        if (Enum.TryParse<SpectrumOccupationType>(s_so_param.TypeSO, out outSpectrumOccupationType))
                                            mDR.MeasSDRSOParam.TypeSO = outSpectrumOccupationType;
                                        break;
                                    }
                                }
                                mDR.MeasSDRParam = new MeasSdrParam();
                                var nHMEASSDRPARAM = session.QueryOver<NH_MeasSDRParam>().Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                if (nHMEASSDRPARAM.Count > 0)
                                {
                                    foreach (var sdrparam in nHMEASSDRPARAM)
                                    {
                                        DetectingType outDetectingType;
                                        if (Enum.TryParse<DetectingType>(sdrparam.DetectTypeSDR, out outDetectingType))
                                            mDR.MeasSDRParam.DetectTypeSDR = outDetectingType;

                                        if (sdrparam.PreamplificationSDR != null) mDR.MeasSDRParam.PreamplificationSDR = sdrparam.PreamplificationSDR.GetValueOrDefault();
                                        if (sdrparam.VBW != null) mDR.MeasSDRParam.VBW = sdrparam.VBW.GetValueOrDefault();
                                        if (sdrparam.RBW != null) mDR.MeasSDRParam.RBW = sdrparam.RBW.GetValueOrDefault();
                                        if (sdrparam.ref_level_dbm != null) mDR.MeasSDRParam.ref_level_dbm = sdrparam.ref_level_dbm.GetValueOrDefault();
                                        if (sdrparam.RfAttenuationSDR != null) mDR.MeasSDRParam.RfAttenuationSDR = sdrparam.RfAttenuationSDR.GetValueOrDefault();
                                        if (sdrparam.MeasTime != null) mDR.MeasSDRParam.MeasTime = sdrparam.MeasTime.GetValueOrDefault();
                                        break;
                                    }
                                }
                                var measLocParam = new List<MeasLocParam>();
                                var nHMEASSDRLOCPARAM = session.QueryOver<NH_MeasSDRLoc>().Where(t => t.ID_MeasSDRID == s.ID).List();
                                if (nHMEASSDRLOCPARAM.Count > 0)
                                {
                                    foreach (var sdrlocparam in nHMEASSDRLOCPARAM)
                                    {
                                        MeasLocParam crsdrloc = new MeasLocParam();
                                        if (sdrlocparam.ASL != null) crsdrloc.ASL = sdrlocparam.ASL.GetValueOrDefault();
                                        if (sdrlocparam.Lat != null) crsdrloc.Lat = sdrlocparam.Lat.GetValueOrDefault();
                                        if (sdrlocparam.Lon != null) crsdrloc.Lon = sdrlocparam.Lon.GetValueOrDefault();
                                        measLocParam.Add(crsdrloc);
                                    }
                                }
                                mDR.MeasLocParam = measLocParam.ToArray();
                                mDR.MeasFreqParam = new MeasFreqParam();
                                var nHMEASSDRFREQPARAM = session.QueryOver<NH_MeasSDRFreqParam>().Where(t => t.id_meas_task_sdr == s.ID).List();
                                if (nHMEASSDRFREQPARAM.Count > 0)
                                {
                                    foreach (var sdrfreqparam in nHMEASSDRFREQPARAM)
                                    {
                                        if (sdrfreqparam.RgU != null) mDR.MeasFreqParam.RgU = sdrfreqparam.RgU.GetValueOrDefault();
                                        if (sdrfreqparam.RgL != null) mDR.MeasFreqParam.RgL = sdrfreqparam.RgL.GetValueOrDefault();
                                        if (sdrfreqparam.Step != null) mDR.MeasFreqParam.Step = sdrfreqparam.Step.GetValueOrDefault();
                                        FrequencyMode outFrequencyMode;
                                        if (Enum.TryParse<FrequencyMode>(sdrfreqparam.Mode, out outFrequencyMode))
                                            mDR.MeasFreqParam.Mode = outFrequencyMode;

                                        var fRL = new List<MeasFreq>();
                                        var nHMEASFREQLST = session.QueryOver<NH_MeasSDRFreq>().Where(t => t.ID_MeasSDRFreqParam == sdrfreqparam.ID).List();
                                        if (nHMEASFREQLST.Count > 0)
                                        {
                                            foreach (var sdrfreqlst in nHMEASFREQLST)
                                            {
                                                var fRLx = new MeasFreq();
                                                if (sdrfreqlst.Freq != null) fRLx.Freq = sdrfreqlst.Freq.GetValueOrDefault();
                                                fRL.Add(fRLx);
                                            }
                                        }
                                        mDR.MeasFreqParam.MeasFreqs = fRL.ToArray();
                                        break;
                                    }
                                }
                                listMeasSdrTask.Add(mDR);
                            }
                        }

                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.FindMeasTaskSDR, Events.FindMeasTaskSDR, ex.Message, null);
            }
            return listMeasSdrTask;
        }


        public List<MeasSdrTask> GetAllMeasTaskSDR()
        {
            var listMeasSdrTask = new List<MeasSdrTask>();
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    #region NH_Meas_Task_SDR
                    {
                        session.Clear();
                        var nHMeasTaskSDR = session.QueryOver<NH_MeasTaskSDR>().Where(t => t.status != AllStatusSensor.Z.ToString() && t.status != AllStatusSensor.F.ToString() && t.status != AllStatusSensor.P.ToString()).List();
                        if (nHMeasTaskSDR.Count > 0)
                        {
                            foreach (var s in nHMeasTaskSDR)
                            {
                                var mDR = new MeasSdrTask();
                                mDR.MeasTaskId = new MeasTaskIdentifier();
                                if (s.MeasTaskId != null) mDR.MeasTaskId.Value = s.MeasTaskId.GetValueOrDefault();
                                mDR.MeasSubTaskId = new MeasTaskIdentifier();
                                if (s.MeasSubTaskId != null) mDR.MeasSubTaskId.Value = s.MeasSubTaskId.GetValueOrDefault();
                                if (s.ID != null) mDR.Id = s.ID.GetValueOrDefault();
                                if (s.MeasSubTaskStationId != null) mDR.MeasSubTaskStationId = s.MeasSubTaskStationId.GetValueOrDefault();
                                MeasurementType outMeasurementType;
                                if (Enum.TryParse<MeasurementType>(s.MeasDataType, out outMeasurementType))
                                    mDR.MeasDataType = outMeasurementType;
                                mDR.status = s.status;
                                mDR.SensorId = new SensorIdentifier();
                                if (s.SensorId != null) mDR.SensorId.Value = s.SensorId.GetValueOrDefault();
                                if (s.SwNumber != null) mDR.SwNumber = s.SwNumber.GetValueOrDefault();
                                if (s.Time_start != null) mDR.Time_start = s.Time_start.GetValueOrDefault();
                                if (s.Time_stop != null) mDR.Time_stop = s.Time_stop.GetValueOrDefault();
                                mDR.NumberScanPerTask = -999;
                                if (s.PerInterval != null) mDR.PerInterval = s.PerInterval.GetValueOrDefault();

                                SpectrumScanType outSpectrumScanType;
                                if (Enum.TryParse<SpectrumScanType>(s.TypeM, out outSpectrumScanType))
                                    mDR.TypeM = outSpectrumScanType;


                                mDR.MeasSDRSOParam = new MeasSdrSOParam();
                                var nHSDRSoParam = session.QueryOver<NH_MeasSDRSOParam>().Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                if (nHSDRSoParam.Count > 0)
                                {
                                    foreach (var soparam in nHSDRSoParam)
                                    {
                                        if (soparam.LevelMinOccup != null) mDR.MeasSDRSOParam.LevelMinOccup = soparam.LevelMinOccup.GetValueOrDefault();
                                        if (soparam.NChenal != null) mDR.MeasSDRSOParam.NChenal = soparam.NChenal.GetValueOrDefault();

                                        SpectrumOccupationType outSpectrumOccupationType;
                                        if (Enum.TryParse<SpectrumOccupationType>(soparam.TypeSO, out outSpectrumOccupationType))
                                            mDR.MeasSDRSOParam.TypeSO = outSpectrumOccupationType;
                                        break;
                                    }
                                }
                                
                                mDR.MeasSDRParam = new MeasSdrParam();
                                var nHMEASSDRPARAM = session.QueryOver<NH_MeasSDRParam>().Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                if (nHMEASSDRPARAM.Count > 0)
                                {
                                    foreach (var sdrparam in nHMEASSDRPARAM)
                                    {
                                        DetectingType outDetectingType;
                                        if (Enum.TryParse<DetectingType>(sdrparam.DetectTypeSDR, out outDetectingType))
                                            mDR.MeasSDRParam.DetectTypeSDR = outDetectingType;

                                        //M_DR.MeasSDRParam. ID = s_sdr_param.ID.GetValueOrDefault();
                                        if (sdrparam.PreamplificationSDR != null) mDR.MeasSDRParam.PreamplificationSDR = sdrparam.PreamplificationSDR.GetValueOrDefault();
                                        if (sdrparam.VBW != null) mDR.MeasSDRParam.VBW = sdrparam.VBW.GetValueOrDefault();
                                        if (sdrparam.RBW != null) mDR.MeasSDRParam.RBW = sdrparam.RBW.GetValueOrDefault();
                                        if (sdrparam.ref_level_dbm != null) mDR.MeasSDRParam.ref_level_dbm = sdrparam.ref_level_dbm.GetValueOrDefault();
                                        if (sdrparam.RfAttenuationSDR != null) mDR.MeasSDRParam.RfAttenuationSDR = sdrparam.RfAttenuationSDR.GetValueOrDefault();
                                        if (sdrparam.MeasTime != null) mDR.MeasSDRParam.MeasTime = sdrparam.MeasTime.GetValueOrDefault();
                                        break;
                                    }
                                }
                                
                                var LMSZ = new List<MeasLocParam>();
                                var nHMEASSDRLOCPARAM = session.QueryOver<NH_MeasSDRLoc>().Where(t => t.ID_MeasSDRID == s.ID).List();
                                if (nHMEASSDRLOCPARAM.Count > 0)
                                {
                                    foreach (var ssdrlocparam in nHMEASSDRLOCPARAM)
                                    {
                                        var crsdrloc = new MeasLocParam();
                                        if (ssdrlocparam.ASL != null) crsdrloc.ASL = ssdrlocparam.ASL.GetValueOrDefault();
                                        if (ssdrlocparam.Lat != null) crsdrloc.Lat = ssdrlocparam.Lat.GetValueOrDefault();
                                        if (ssdrlocparam.Lon != null) crsdrloc.Lon = ssdrlocparam.Lon.GetValueOrDefault();
                                        LMSZ.Add(crsdrloc);
                                    }
                                }
                                mDR.MeasLocParam = LMSZ.ToArray();
                                
                                mDR.MeasFreqParam = new MeasFreqParam();
                                var nHMEASSDRFREQPARAM = session.QueryOver<NH_MeasSDRFreqParam>().Where(t => t.id_meas_task_sdr == s.ID).List();
                                if (nHMEASSDRFREQPARAM.Count > 0)
                                {
                                    foreach (var sdrfreqparam in nHMEASSDRFREQPARAM)
                                    {
                                        if (sdrfreqparam.RgU != null) mDR.MeasFreqParam.RgU = sdrfreqparam.RgU.GetValueOrDefault();
                                        if (sdrfreqparam.RgL != null) mDR.MeasFreqParam.RgL = sdrfreqparam.RgL.GetValueOrDefault();
                                        if (sdrfreqparam.Step != null) mDR.MeasFreqParam.Step = sdrfreqparam.Step.GetValueOrDefault();
                                        FrequencyMode outFrequencyMode;
                                        if (Enum.TryParse<FrequencyMode>(sdrfreqparam.Mode, out outFrequencyMode))
                                            mDR.MeasFreqParam.Mode = outFrequencyMode;

                                        var fRL = new List<MeasFreq>();
                                        var nHMEASFREQLST = session.QueryOver<NH_MeasSDRFreq>().Where(t => t.ID_MeasSDRFreqParam == sdrfreqparam.ID).List();
                                        if (nHMEASFREQLST.Count > 0)
                                        {
                                            foreach (var sdrfreqlst in nHMEASFREQLST)
                                            {
                                                var fRLx = new MeasFreq();
                                                if (sdrfreqlst.Freq != null) fRLx.Freq = sdrfreqlst.Freq.GetValueOrDefault();
                                                fRL.Add(fRLx);
                                            }
                                        }
                                        mDR.MeasFreqParam.MeasFreqs = fRL.ToArray();
                                        break;
                                    }
                                }
                                listMeasSdrTask.Add(mDR);
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.GetAllMeasTaskSDR, Events.GetAllMeasTaskSDR, ex.Message, null);
            }
            return listMeasSdrTask;
        }
    }

}

