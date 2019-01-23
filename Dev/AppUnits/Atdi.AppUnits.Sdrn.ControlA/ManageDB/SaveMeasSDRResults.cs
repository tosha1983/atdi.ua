using System;
using Atdi.AppServer.Contracts.Sdrns;
using NHibernate;
using Atdi.AppUnits.Sdrn.ControlA.Bus;

namespace Atdi.AppUnits.Sdrn.ControlA.ManageDB
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveMeasSDRResults
    {

        public static bool SaveisSendMeasTaskSDRResults(MeasSdrResults sdrRes)
        {
            bool isSuccess = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var nHMeasSDRResults = session.QueryOver<NH_MeasSDRResults>().Where(t => t.ID == sdrRes.Id).List();
                    if (nHMeasSDRResults.Count > 0)
                    {
                        ITransaction tr = session.BeginTransaction();
                        foreach (var s in nHMeasSDRResults)
                        {
                            var cl = new ClassObjectsSensorOnSDR();
                            s.isSend = 1;
                            cl.UpdateObject<NH_MeasSDRResults>(s.ID.GetValueOrDefault(), s);
                        }
                        tr.Commit();
                        session.Flush();
                        tr.Dispose();
                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.SaveisSendMeasTaskSDRResults, Events.SaveisSendMeasTaskSDRResults, ex.Message, null);
            }
            return isSuccess;
        }


        public static bool SaveStatusMeasTaskSDRResults(MeasSdrResults sdrRes, string Status)
        {
            bool isSuccess = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var nHMeasSDRResults = session.QueryOver<NH_MeasSDRResults>().Where(t => t.ID == sdrRes.Id).List();
                    if (nHMeasSDRResults.Count > 0)
                    {
                        ITransaction transaction = session.BeginTransaction();
                        foreach (var s in nHMeasSDRResults)
                        {
                            var cl = new ClassObjectsSensorOnSDR();
                            s.status = Status;
                            cl.UpdateObject<NH_MeasSDRResults>(s.ID.GetValueOrDefault(), s);
                            transaction.Commit();
                            session.Flush();
                            transaction.Dispose();
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.SaveStatusMeasTaskSDRResults, Events.SaveStatusMeasTaskSDRResults, ex.Message, null);
            }
            return isSuccess;
        }

       


        public static void SaveStatusMeasTaskSDR(NH_MeasTaskSDR nhTaskSDR, string newStatus)
        {
            try
            {
                var cl = new ClassObjectsSensorOnSDR();
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    ITransaction transaction = session.BeginTransaction();
                    nhTaskSDR.status = newStatus;
                    cl.UpdateObject<NH_MeasTaskSDR>(nhTaskSDR.ID.GetValueOrDefault(), nhTaskSDR);
                    transaction.Commit();
                    session.Flush();
                    transaction.Dispose();
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.SaveStatusMeasTaskSDR, Events.SaveStatusMeasTaskSDR, ex.Message, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M_SDR_RES"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public bool SaveStatusResultSDR(MeasSdrResults mSdrRes, string Status)
        {
            bool isCorrect = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var nHMeasTaskSDR = session.QueryOver<NH_MeasSDRResults>().Where(t => t.ID == mSdrRes.Id).List();
                    if (nHMeasTaskSDR.Count > 0)
                    {
                        var transaction = session.BeginTransaction();
                        foreach (var s in nHMeasTaskSDR)
                        {
                            var cl = new ClassObjectsSensorOnSDR();
                            s.status = Status;
                            cl.UpdateObject<NH_MeasSDRResults>(s.ID.GetValueOrDefault(), s);
                            Launcher._logger.Info(Contexts.ThisComponent, Categories.SaveStatusResultSDR, Events.SuccessfullySavedIntoTableNH_MeasSDRResults);
                        }
                        transaction.Commit();
                        session.Flush();
                        transaction.Dispose();
                        isCorrect = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.SaveStatusResultSDR, Events.SaveStatusResultSDR, ex.Message, null);
                isCorrect = false;
            }
            return isCorrect;
        }

        /// <summary>
        /// Создать новую запись в таблице [NH_Meas_Task_SDR]
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool SaveMeasResultSDR(MeasSdrResults mSdrRes, string Status)
        {
            bool isCorrect = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    ITransaction transaction = session.BeginTransaction();
                    #region NH_Meas_SDR_Results
                    var nhMeasSdrResults = new NH_MeasSDRResults();
                    nhMeasSdrResults.NN = mSdrRes.NN;
                    nhMeasSdrResults.DataMeas = mSdrRes.DataMeas;
                    nhMeasSdrResults.MeasDataType = mSdrRes.MeasDataType.ToString();
                    if (mSdrRes.MeasSubTaskId != null) nhMeasSdrResults.MeasSubTaskId = mSdrRes.MeasSubTaskId.Value;
                    nhMeasSdrResults.MeasSubTaskStationId = mSdrRes.MeasSubTaskStationId;
                    if (mSdrRes.SensorId != null) nhMeasSdrResults.SensorId = mSdrRes.SensorId.Value;
                    if (mSdrRes.MeasTaskId != null) nhMeasSdrResults.MeasTaskId = mSdrRes.MeasTaskId.Value;
                    nhMeasSdrResults.status = Status;
                    nhMeasSdrResults.SwNumber = mSdrRes.SwNumber;
                    nhMeasSdrResults = (NH_MeasSDRResults)MeasTaskSDRExtend.SetNullValue(nhMeasSdrResults);
                    object iD = session.Save(nhMeasSdrResults);
                    mSdrRes.Id = Convert.ToInt32(iD);
                    Launcher._logger.Info(Contexts.ThisComponent, Categories.SaveMeasResultSDR, Events.SuccessfullySavedIntoTableNH_MeasSDRResults);
                    if (mSdrRes.FSemples != null)
                    {
                        int idxcnt = 0;
                        foreach (var fs in mSdrRes.FSemples)
                        {
                            if (fs.Freq != 0)
                            {
                                var nhFSEMPLES = new NH_FSemples();
                                nhFSEMPLES.Freq = fs.Freq;
                                nhFSEMPLES.LeveldBm = fs.LeveldBm;
                                nhFSEMPLES.LeveldBmkVm = fs.LeveldBmkVm;
                                nhFSEMPLES.LevelMaxdBm = fs.LevelMaxdBm;
                                nhFSEMPLES.LevelMindBm = fs.LevelMindBm;
                                nhFSEMPLES.OcupationPt = fs.OcupationPt;
                                nhFSEMPLES.ID_MeasSDRResults = Convert.ToInt32(iD);
                                object IdSEMPL = session.Save(nhFSEMPLES);
                                Launcher._logger.Info(Contexts.ThisComponent, Categories.SaveMeasResultSDR, Events.SuccessfullySavedIntoTableNHFSEMPLES);
                            }
                            idxcnt++;
                        }
                    }
                    if (mSdrRes.Level != null)
                    {
                        foreach (float fs in mSdrRes.Level)
                        {
                            var nHMeasResultsLevel = new NH_MeasResultsLevel();
                            nHMeasResultsLevel.Level = fs;
                            nHMeasResultsLevel.ID_NH_MeasSDRResults = Convert.ToInt32(iD);
                            object IdSempl = session.Save(nHMeasResultsLevel);
                            Launcher._logger.Info(Contexts.ThisComponent, Categories.SaveMeasResultSDR, Events.SuccessfullySavedIntoTablenHMeasResultsLevel);
                        }
                    }
                    if (mSdrRes.Freqs != null)
                    {
                        int idxcnt = 0;
                        foreach (float fs in mSdrRes.Freqs)
                        {
                            var nHMeasResultsFreq = new NH_MeasResultsFreq();
                            nHMeasResultsFreq.Freq = fs;
                            nHMeasResultsFreq.ID_NH_MeasSDRResults = Convert.ToInt32(iD);
                            object IdSempl = session.Save(nHMeasResultsFreq);
                            Launcher._logger.Info(Contexts.ThisComponent, Categories.SaveMeasResultSDR, Events.SuccessfullySavedIntoTablenHMeasResultsFreq);
                            idxcnt++;
                        }
                    }
                    if (mSdrRes.MeasSDRLoc != null)
                    {
                        var fs = mSdrRes.MeasSDRLoc;
                        {
                            var nhFMeasSDRLoc = new NH_LocationSensor();
                            nhFMeasSDRLoc.ASL = fs.ASL;
                            nhFMeasSDRLoc.Lat = fs.Lat;
                            nhFMeasSDRLoc.Lon = fs.Lon;
                            nhFMeasSDRLoc.ID_MeasSDRResults = Convert.ToInt32(iD);
                            object IdSempl = session.Save(nhFMeasSDRLoc);
                            Launcher._logger.Info(Contexts.ThisComponent, Categories.SaveMeasResultSDR, Events.SuccessfullySavedIntoTablenhFMeasSDRLoc);
                        }
                    }
                    transaction.Commit();
                    session.Flush();
                    transaction.Dispose();
                    isCorrect = true;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.SaveMeasResultSDR, Events.SaveMeasResultSDR, ex.Message, null);
                isCorrect = false;
            }
            return isCorrect;
        }

    }

}
