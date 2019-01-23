using System;
using Atdi.AppServer.Contracts.Sdrns;
using NHibernate;
using Atdi.AppUnits.Sdrn.ControlA.Bus;
using Atdi.AppUnits.Sdrn.ControlA;

namespace Atdi.AppUnits.Sdrn.ControlA.ManageDB
{

    /// <summary>
    /// 
    /// </summary>
    public class SaveMeasTaskSDR
    {
        /// <summary>
        /// Создать новую запись в таблице [NH_Meas_Task_SDR]
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool CreateNewMeasTaskSDR(MeasSdrTask mSdr)
        {
            bool isCorrect = false;
            try
            {
                mSdr.UpdateStatus();
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    ITransaction transaction = session.BeginTransaction();
                    #region NH_Meas_Task_SDR
                    {
                        var nhMeasTaskSDR = new NH_MeasTaskSDR();
                        nhMeasTaskSDR.MeasSubTaskStationId = mSdr.MeasSubTaskStationId;
                        nhMeasTaskSDR.MeasSubTaskId = mSdr.MeasSubTaskId.Value;
                        nhMeasTaskSDR.prio = mSdr.prio.ToString();
                        nhMeasTaskSDR.status = mSdr.status;
                        nhMeasTaskSDR.SwNumber = mSdr.SwNumber;
                        nhMeasTaskSDR.PerInterval = (int)mSdr.PerInterval;
                        nhMeasTaskSDR.Time_start = mSdr.Time_start;
                        nhMeasTaskSDR.Time_stop = mSdr.Time_stop;
                        nhMeasTaskSDR.TypeM = mSdr.TypeM.ToString();
                        nhMeasTaskSDR.SensorId = mSdr.SensorId.Value;
                        nhMeasTaskSDR.MeasTaskId = mSdr.MeasTaskId.Value;
                        nhMeasTaskSDR.MeasDataType = mSdr.MeasDataType.ToString();
                        nhMeasTaskSDR = (NH_MeasTaskSDR)MeasTaskSDRExtend.SetNullValue(nhMeasTaskSDR);
                        object ID = session.Save(nhMeasTaskSDR);
                        mSdr.Id = (int)ID;
                        Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewMeasTaskSDR, Events.SuccessfullySavedIntoTablenhMeasTaskSDR);


                        var nMeasSDRFREQPARAM = new NH_MeasSDRFreqParam();
                        if (mSdr.MeasFreqParam != null)
                        {
                            nMeasSDRFREQPARAM.RgU = mSdr.MeasFreqParam.RgU;
                            nMeasSDRFREQPARAM.RgL = mSdr.MeasFreqParam.RgL;
                            nMeasSDRFREQPARAM.Mode = mSdr.MeasFreqParam.Mode.ToString();
                            nMeasSDRFREQPARAM.Step = mSdr.MeasFreqParam.Step;
                        }
                        nMeasSDRFREQPARAM.id_meas_task_sdr = Convert.ToInt32(ID);
                        nMeasSDRFREQPARAM = (NH_MeasSDRFreqParam)MeasTaskSDRExtend.SetNullValue(nMeasSDRFREQPARAM);
                        object iDMEASSDRFPARAM = session.Save(nMeasSDRFREQPARAM);
                        Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewMeasTaskSDR, Events.SuccessfullySavedIntoTablenMeasSDRFREQPARAM);

                        if (mSdr.MeasFreqParam != null)
                        {
                            if (mSdr.MeasFreqParam.MeasFreqs != null)
                            {
                                foreach (var FR in mSdr.MeasFreqParam.MeasFreqs)
                                {
                                    var nNHMeasSDRFreqLST = new NH_MeasSDRFreq();
                                    nNHMeasSDRFreqLST.Freq = FR.Freq;
                                    nNHMeasSDRFreqLST.ID_MeasSDRFreqParam = Convert.ToInt32(iDMEASSDRFPARAM);
                                    nNHMeasSDRFreqLST = (NH_MeasSDRFreq)MeasTaskSDRExtend.SetNullValue(nNHMeasSDRFreqLST);
                                    session.Save(nNHMeasSDRFreqLST);
                                    Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewMeasTaskSDR, Events.SuccessfullySavedIntoTableNH_MeasSDRFreq);
                                }
                            }
                        }

                        if (mSdr.MeasLocParam != null)
                        {
                            foreach (var LOC in mSdr.MeasLocParam)
                            {
                                var nMeas_SDRLocParam = new NH_MeasSDRLoc();
                                nMeas_SDRLocParam.ASL = LOC.ASL;
                                nMeas_SDRLocParam.Lat = LOC.Lat;
                                nMeas_SDRLocParam.Lon = LOC.Lon;
                                nMeas_SDRLocParam = (NH_MeasSDRLoc)MeasTaskSDRExtend.SetNullValue(nMeas_SDRLocParam);
                                session.Save(nMeas_SDRLocParam);
                                Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewMeasTaskSDR, Events.SuccessfullySavedIntoTablenMeas_SDRLocParam);
                            }
                        }


                        var nNHMeasSDRParam = new NH_MeasSDRParam();
                        if (mSdr.MeasSDRParam != null)
                        {
                            nNHMeasSDRParam.DetectTypeSDR = mSdr.MeasSDRParam.DetectTypeSDR.ToString();
                            nNHMeasSDRParam.PreamplificationSDR = mSdr.MeasSDRParam.PreamplificationSDR;
                            nNHMeasSDRParam.RBW = mSdr.MeasSDRParam.RBW;
                            nNHMeasSDRParam.ref_level_dbm = mSdr.MeasSDRParam.ref_level_dbm;
                            nNHMeasSDRParam.RfAttenuationSDR = mSdr.MeasSDRParam.RfAttenuationSDR;
                            nNHMeasSDRParam.MeasTime = mSdr.MeasSDRParam.MeasTime;
                            nNHMeasSDRParam.VBW = mSdr.MeasSDRParam.VBW;
                        }
                        nNHMeasSDRParam.ID_MeasTaskSDR = Convert.ToInt32(ID);
                        nNHMeasSDRParam = (NH_MeasSDRParam)MeasTaskSDRExtend.SetNullValue(nNHMeasSDRParam);
                        session.Save(nNHMeasSDRParam);



                        var nNHMeasSDRSoParam = new NH_MeasSDRSOParam();
                        if (mSdr.MeasSDRSOParam != null)
                        {
                            nNHMeasSDRSoParam.LevelMinOccup = mSdr.MeasSDRSOParam.LevelMinOccup;
                            nNHMeasSDRSoParam.NChenal = mSdr.MeasSDRSOParam.NChenal;
                            nNHMeasSDRSoParam.TypeSO = mSdr.MeasSDRSOParam.TypeSO.ToString();
                        }
                        nNHMeasSDRSoParam.ID_MeasTaskSDR = Convert.ToInt32(ID);
                        nNHMeasSDRSoParam = (NH_MeasSDRSOParam)MeasTaskSDRExtend.SetNullValue(nNHMeasSDRSoParam);
                        session.Save(nNHMeasSDRSoParam);
                        Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewMeasTaskSDR, Events.SuccessfullySavedIntoTablenNHMeasSDRSoParam);
                    }

                    #endregion
                    transaction.Commit();
                    session.Flush();
                    transaction.Dispose();
                    isCorrect = true;
                }

            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.CreateNewMeasTaskSDR, Events.CreateNewMeasTaskSDR, ex.Message, null);
            }
            return isCorrect;
        }

        public  bool SaveStatusMeasTaskSDR(MeasSdrTask sdrM)
        {
            bool isCorrect = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var nHMeasTaskSDR = session.QueryOver<NH_MeasTaskSDR>().Where(t => t.SensorId.Value == sdrM.SensorId.Value && t.MeasTaskId.Value == sdrM.MeasTaskId.Value && t.MeasSubTaskStationId == sdrM.MeasSubTaskStationId && t.MeasSubTaskId.Value == sdrM.MeasSubTaskId.Value).List();
                    if (nHMeasTaskSDR.Count > 0)
                    {
                        ITransaction transaction = session.BeginTransaction();
                        foreach (var s in nHMeasTaskSDR)
                        {
                            var cl = new ClassObjectsSensorOnSDR();
                            s.status = sdrM.status;
                            cl.UpdateObject<NH_MeasTaskSDR>(s.ID.GetValueOrDefault(), s);
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
                Launcher._logger.Error(Contexts.ThisComponent, Categories.SaveStatusMeasTaskSDR, Events.SaveStatusMeasTaskSDR, ex.Message, null);
            }
            return isCorrect;
        }

        /// <summary>
        /// Удаление записи в таблице [NH_Meas_Task_SDR]
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool DeleteMeasTaskSDR(MeasSdrTask sdrM)
        {
            bool isCorrect = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    ITransaction transaction = session.BeginTransaction();
                    var nHMeasTaskSDR = session.QueryOver<NH_MeasTaskSDR>().Where(t => t.SensorId.Value == sdrM.SensorId.Value && t.MeasTaskId.Value == sdrM.MeasTaskId.Value && t.MeasSubTaskStationId == sdrM.MeasSubTaskStationId).List();
                    if (nHMeasTaskSDR.Count > 0)
                    {
                        foreach (NH_MeasTaskSDR s in nHMeasTaskSDR)
                        {
                            session.Delete(s);
                            Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewMeasTaskSDR, Events.RemoveRecordFromTableNH_MeasTaskSDR);
                        }
                    }
                    transaction.Commit();
                    session.Flush();
                    transaction.Dispose();
                    isCorrect = true;
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.DeleteMeasTaskSDR, Events.DeleteMeasTaskSDR, ex.Message, null);
                isCorrect = false;
            }

            return isCorrect;
        }
        /// <summary>
        /// Архивация MeasTaskSDR
        /// </summary>
        /// <param name="sdrM"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public bool ArchiveMeasTaskSDR(MeasSdrTask sdrM, string Status)
        {
            bool isCorrect = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    ITransaction transaction = session.BeginTransaction();
                    var nHMeasTaskSDR = session.QueryOver<NH_MeasTaskSDR>().Where(t => t.SensorId.Value == sdrM.SensorId.Value && t.MeasTaskId.Value == sdrM.MeasTaskId.Value && t.MeasSubTaskStationId == sdrM.MeasSubTaskStationId).List();
                    if (nHMeasTaskSDR.Count > 0)
                    {
                        foreach (var s in nHMeasTaskSDR)
                        {
                            s.status = Status;
                            session.Update(s);
                            Launcher._logger.Info(Contexts.ThisComponent, Categories.CreateNewMeasTaskSDR, Events.ArchiveRecordTableNH_MeasTaskSDR);
                        }
                    }
                    transaction.Commit();
                    session.Flush();
                    transaction.Dispose();
                    isCorrect = true;
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.ArchiveMeasTaskSDR, Events.ArchiveMeasTaskSDR, ex.Message, null);
                isCorrect = false;
            }
            return isCorrect;
        }
    }
}
