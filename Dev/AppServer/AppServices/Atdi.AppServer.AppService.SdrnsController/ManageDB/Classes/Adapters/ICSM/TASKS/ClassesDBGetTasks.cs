using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer;
using System.Data.Common;


namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    /// <summary>
    /// Класс, извлекающий данные
    /// из поля ORDER_DATA таблицы SYS_ARGUS_ORM
    /// с целью заполнения структуры MEAS_TASK
    /// </summary>
    public class ClassesDBGetTasks : IDisposable
    {
        public static ILogger logger;
        public ClassesDBGetTasks(ILogger log)
        {
            if (logger==null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassesDBGetTasks()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private List<CLASS_TASKS> L_IN { get; set; }


        public List<CLASS_TASKS> ReadTask(int MeasTaskId)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            L_IN = new List<CLASS_TASKS>();
            try
            {
                #region Load Tasks from DB
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    logger.Trace("Start procedure ReadTask...");
                    try
                    { 
                    CLASS_TASKS ICSM_T = new CLASS_TASKS();
                    ICSM_T.meas_st = new List<KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>>();
                    ICSM_T.meas_task = new YXbsMeastask();
                    ICSM_T.MeasDtParam = new List<YXbsMeasdtparam>();
                    ICSM_T.MeasFreqLst_param = new List<KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>>();
                    ICSM_T.MeasLocParam = new List<YXbsMeaslocparam>();
                    ICSM_T.MeasOther = new YXbsMeasother();
                    ICSM_T.Stations = new List<YXbsMeasstation>();
                    ICSM_T.XbsStationdatform = new List<YXbsStation>();


                    YXbsMeastask task = new YXbsMeastask();
                    task.Format("*");
                    task.Filter = string.Format("(ID={0}) AND (STATUS IS NOT NULL) AND (STATUS<>'Z')",MeasTaskId);
                    for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                    {
                        ICSM_T = new CLASS_TASKS();
                        YXbsMeastask m_meas_task = new YXbsMeastask();
                        m_meas_task.CopyDataFrom(task);
                        ICSM_T.meas_task = m_meas_task;
                        m_meas_task.Close();
                        m_meas_task.Dispose();

                        List<YXbsMeasstation> LXbsMeasstation = new List<YXbsMeasstation>();
                        YXbsMeasstation XbsMeasstation_ = new YXbsMeasstation();
                        XbsMeasstation_.Format("*");
                        XbsMeasstation_.Filter = string.Format("(ID_XBS_MEASTASK={0})", task.m_id);
                        for (XbsMeasstation_.OpenRs(); !XbsMeasstation_.IsEOF(); XbsMeasstation_.MoveNext())
                        {
                            var m_fr = new YXbsMeasstation();
                            m_fr.CopyDataFrom(XbsMeasstation_);
                            LXbsMeasstation.Add(m_fr);
                            m_fr.Dispose();
                        }
                        XbsMeasstation_.Close();
                        XbsMeasstation_.Dispose();
                        ICSM_T.Stations = LXbsMeasstation;


                        List<YXbsStation> LYXbsStationdatform = new List<YXbsStation>();
                        YXbsStation XbsStationdatform_ = new YXbsStation();
                        XbsStationdatform_.Format("*");
                        XbsStationdatform_.Filter = string.Format("(ID_XBS_MEASTASK={0})", task.m_id);
                        for (XbsStationdatform_.OpenRs(); !XbsStationdatform_.IsEOF(); XbsStationdatform_.MoveNext())
                        {
                            var m_fr = new YXbsStation();
                            m_fr.CopyDataFrom(XbsStationdatform_);
                            LYXbsStationdatform.Add(m_fr);
                            m_fr.Dispose();
                        }
                        XbsStationdatform_.Close();
                        XbsStationdatform_.Dispose();
                        ICSM_T.XbsStationdatform = LYXbsStationdatform;



                        YXbsMeasdtparam MeasDtParam_ = new YXbsMeasdtparam();
                        MeasDtParam_.Format("*");
                        MeasDtParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasDtParam_.OpenRs(); !MeasDtParam_.IsEOF(); MeasDtParam_.MoveNext())
                        {
                            var m = new YXbsMeasdtparam();
                            m.CopyDataFrom(MeasDtParam_);
                            ICSM_T.MeasDtParam.Add(m);
                            m.Dispose();
                        }
                        MeasDtParam_.Close();
                        MeasDtParam_.Dispose();


                        YXbsMeaslocparam MeasLocParam_ = new YXbsMeaslocparam();
                        MeasLocParam_.Format("*");
                        MeasLocParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasLocParam_.OpenRs(); !MeasLocParam_.IsEOF(); MeasLocParam_.MoveNext())
                        {
                            var m = new YXbsMeaslocparam();
                            m.CopyDataFrom(MeasLocParam_);
                            ICSM_T.MeasLocParam.Add(m);
                            m.Dispose();
                        }
                        MeasLocParam_.Close();
                        MeasLocParam_.Dispose();

                        YXbsMeasother MeasOther_ = new YXbsMeasother();
                        MeasOther_.Format("*");
                        MeasOther_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasOther_.OpenRs(); !MeasOther_.IsEOF(); MeasOther_.MoveNext())
                        {
                            var m = new YXbsMeasother();
                            m.CopyDataFrom(MeasOther_);
                            ICSM_T.MeasOther = m;
                            m.Dispose();
                        }
                        MeasOther_.Close();
                        MeasOther_.Dispose();

                        YXbsMeasfreqparam MeasFreqParam_ = new YXbsMeasfreqparam();
                        MeasFreqParam_.Format("*");
                        MeasFreqParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasFreqParam_.OpenRs(); !MeasFreqParam_.IsEOF(); MeasFreqParam_.MoveNext())
                        {
                            var m = new YXbsMeasfreqparam();
                            m.CopyDataFrom(MeasFreqParam_);
                            List<YXbsMeasfreq> FreqL = new List<YXbsMeasfreq>();
                            YXbsMeasfreq MeasFreqLst_ = new YXbsMeasfreq();
                            MeasFreqLst_.Format("*");
                            MeasFreqLst_.Filter = string.Format("ID_XBS_MeasFreqParam={0}", MeasFreqParam_.m_id);
                            for (MeasFreqLst_.OpenRs(); !MeasFreqLst_.IsEOF(); MeasFreqLst_.MoveNext())
                            {
                                var m_fr = new YXbsMeasfreq();
                                m_fr.CopyDataFrom(MeasFreqLst_);
                                FreqL.Add(m_fr);
                                m_fr.Dispose();
                            }
                            MeasFreqLst_.Close();
                            MeasFreqLst_.Dispose();

                            KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>> key = new KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>(m, FreqL);
                            ICSM_T.MeasFreqLst_param.Add(key);
                            m.Dispose();
                        }
                        MeasFreqParam_.Close();
                        MeasFreqParam_.Dispose();

                        YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask();
                        MeasSubTask_.Format("*");
                        MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext())
                        {
                            var m = new YXbsMeassubtask();
                            m.CopyDataFrom(MeasSubTask_);
                            List<YXbsMeassubtasksta> SubTaskStL = new List<YXbsMeassubtasksta>();
                            YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta();
                            MeasSubTaskSt_.Format("*");
                            MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND ((STATUS<>'Z') OR (STATUS IS NULL))", MeasSubTask_.m_id);
                            for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext())
                            {
                                var m_fr = new YXbsMeassubtasksta();
                                m_fr.CopyDataFrom(MeasSubTaskSt_);
                                SubTaskStL.Add(m_fr);
                                m_fr.Dispose();
                            }
                            MeasSubTaskSt_.Close();
                            MeasSubTaskSt_.Dispose();
                            KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>> key = new KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>(m, SubTaskStL);
                            ICSM_T.meas_st.Add(key);
                            m.Dispose();
                        }
                        MeasSubTask_.Close();
                        MeasSubTask_.Dispose();
                        L_IN.Add(ICSM_T);
                    }
                    task.Close();
                    task.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadTask... " + ex.Message);
                    }
                    logger.Trace("End procedure ReadTask.");
                });
                tsk.Start();
                tsk.Join();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error in ReadTask: " + ex.Message);
            }
            return L_IN;
        }


        public List<CLASS_TASKS> ShortReadTask(int MeasTaskId)
        {
            L_IN = new List<CLASS_TASKS>();
            try
            {
                #region Load Tasks from DB
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    logger.Trace("Start procedure ReadTask...");
                    try
                    { 
                    // сканирование по объектам БД
                    CLASS_TASKS ICSM_T = new CLASS_TASKS();
                    ICSM_T.meas_st = new List<KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>>();
                    ICSM_T.meas_task = new YXbsMeastask();
                    ICSM_T.MeasDtParam = new List<YXbsMeasdtparam>();
                    ICSM_T.MeasFreqLst_param = new List<KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>>();
                    ICSM_T.MeasLocParam = new List<YXbsMeaslocparam>();
                    ICSM_T.MeasOther = new YXbsMeasother();
                    ICSM_T.Stations = new List<YXbsMeasstation>();
                    ICSM_T.XbsStationdatform = new List<YXbsStation>();


                    //подключение к БД
                    YXbsMeastask task = new YXbsMeastask();
                    task.Format("*");
                    task.Filter = string.Format("(ID={0}) AND (STATUS IS NOT NULL) AND (STATUS<>'Z')", MeasTaskId);
                    for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                    {
                        ICSM_T = new CLASS_TASKS();
                        YXbsMeastask m_meas_task = new YXbsMeastask();
                        m_meas_task.CopyDataFrom(task);
                        ICSM_T.meas_task = m_meas_task;
                        m_meas_task.Close();
                        m_meas_task.Dispose();

                        List<YXbsMeasstation> LXbsMeasstation = new List<YXbsMeasstation>();
                        YXbsMeasstation XbsMeasstation_ = new YXbsMeasstation();
                        XbsMeasstation_.Format("*");
                        XbsMeasstation_.Filter = string.Format("(ID_XBS_MEASTASK={0})", task.m_id);
                        for (XbsMeasstation_.OpenRs(); !XbsMeasstation_.IsEOF(); XbsMeasstation_.MoveNext())
                        {
                            var m_fr = new YXbsMeasstation();
                            m_fr.CopyDataFrom(XbsMeasstation_);
                            LXbsMeasstation.Add(m_fr);
                            m_fr.Dispose();
                        }
                        XbsMeasstation_.Close();
                        XbsMeasstation_.Dispose();
                        ICSM_T.Stations = LXbsMeasstation;

                        YXbsMeasdtparam MeasDtParam_ = new YXbsMeasdtparam();
                        MeasDtParam_.Format("*");
                        MeasDtParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasDtParam_.OpenRs(); !MeasDtParam_.IsEOF(); MeasDtParam_.MoveNext())
                        {
                            var m = new YXbsMeasdtparam();
                            m.CopyDataFrom(MeasDtParam_);
                            ICSM_T.MeasDtParam.Add(m);
                            m.Dispose();
                        }
                        MeasDtParam_.Close();
                        MeasDtParam_.Dispose();


                        YXbsMeaslocparam MeasLocParam_ = new YXbsMeaslocparam();
                        MeasLocParam_.Format("*");
                        MeasLocParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasLocParam_.OpenRs(); !MeasLocParam_.IsEOF(); MeasLocParam_.MoveNext())
                        {
                            var m = new YXbsMeaslocparam();
                            m.CopyDataFrom(MeasLocParam_);
                            ICSM_T.MeasLocParam.Add(m);
                            m.Dispose();
                        }
                        MeasLocParam_.Close();
                        MeasLocParam_.Dispose();

                        YXbsMeasother MeasOther_ = new YXbsMeasother();
                        MeasOther_.Format("*");
                        MeasOther_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasOther_.OpenRs(); !MeasOther_.IsEOF(); MeasOther_.MoveNext())
                        {
                            var m = new YXbsMeasother();
                            m.CopyDataFrom(MeasOther_);
                            ICSM_T.MeasOther = m;
                            m.Dispose();
                        }
                        MeasOther_.Close();
                        MeasOther_.Dispose();

                        YXbsMeasfreqparam MeasFreqParam_ = new YXbsMeasfreqparam();
                        MeasFreqParam_.Format("*");
                        MeasFreqParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasFreqParam_.OpenRs(); !MeasFreqParam_.IsEOF(); MeasFreqParam_.MoveNext())
                        {
                            var m = new YXbsMeasfreqparam();
                            m.CopyDataFrom(MeasFreqParam_);
                            List<YXbsMeasfreq> FreqL = new List<YXbsMeasfreq>();
                            YXbsMeasfreq MeasFreqLst_ = new YXbsMeasfreq();
                            MeasFreqLst_.Format("*");
                            MeasFreqLst_.Filter = string.Format("ID_XBS_MeasFreqParam={0}", MeasFreqParam_.m_id);
                            for (MeasFreqLst_.OpenRs(); !MeasFreqLst_.IsEOF(); MeasFreqLst_.MoveNext())
                            {
                                var m_fr = new YXbsMeasfreq();
                                m_fr.CopyDataFrom(MeasFreqLst_);
                                FreqL.Add(m_fr);
                                m_fr.Dispose();
                            }
                            MeasFreqLst_.Close();
                            MeasFreqLst_.Dispose();

                            KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>> key = new KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>(m, FreqL);
                            ICSM_T.MeasFreqLst_param.Add(key);
                            m.Dispose();
                        }
                        MeasFreqParam_.Close();
                        MeasFreqParam_.Dispose();

                        YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask();
                        MeasSubTask_.Format("*");
                        MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext())
                        {
                            var m = new YXbsMeassubtask();
                            m.CopyDataFrom(MeasSubTask_);
                            List<YXbsMeassubtasksta> SubTaskStL = new List<YXbsMeassubtasksta>();
                            YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta();
                            MeasSubTaskSt_.Format("*");
                            MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND ((STATUS<>'Z') OR (STATUS IS NULL))", MeasSubTask_.m_id);
                            for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext())
                            {
                                var m_fr = new YXbsMeassubtasksta();
                                m_fr.CopyDataFrom(MeasSubTaskSt_);
                                SubTaskStL.Add(m_fr);
                                m_fr.Dispose();
                            }
                            MeasSubTaskSt_.Close();
                            MeasSubTaskSt_.Dispose();
                            KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>> key = new KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>(m, SubTaskStL);
                            ICSM_T.meas_st.Add(key);
                            m.Dispose();
                        }
                        MeasSubTask_.Close();
                        MeasSubTask_.Dispose();
                        L_IN.Add(ICSM_T);
                    }
                    task.Close();
                    task.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadTask... " + ex.Message);
                    }
                    logger.Trace("End procedure ReadTask.");
                });
                tsk.Start();
                tsk.Join();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error in ReadTask: " + ex.Message);
            }
            return L_IN;
        }

        public List<CLASS_TASKS> VeryShortReadTask(int MeasTaskId)
        {
            L_IN = new List<CLASS_TASKS>();
            try
            {
                #region Load Tasks from DB
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    logger.Trace("Start procedure ReadTask...");
                    try
                    {
                        // сканирование по объектам БД
                        CLASS_TASKS ICSM_T = new CLASS_TASKS();
                        ICSM_T.meas_st = new List<KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>>();
                        ICSM_T.meas_task = new YXbsMeastask();
                        ICSM_T.MeasDtParam = new List<YXbsMeasdtparam>();
                        ICSM_T.MeasFreqLst_param = new List<KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>>();
                        ICSM_T.MeasLocParam = new List<YXbsMeaslocparam>();
                        ICSM_T.MeasOther = new YXbsMeasother();
                        ICSM_T.Stations = new List<YXbsMeasstation>();
                        ICSM_T.XbsStationdatform = new List<YXbsStation>();


                        //подключение к БД
                        YXbsMeastask task = new YXbsMeastask();
                        task.Format("*");
                        task.Filter = string.Format("(ID={0}) AND (STATUS IS NOT NULL) AND (STATUS<>'Z')", MeasTaskId);
                        for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                        {
                            ICSM_T = new CLASS_TASKS();
                            YXbsMeastask m_meas_task = new YXbsMeastask();
                            m_meas_task.CopyDataFrom(task);
                            ICSM_T.meas_task = m_meas_task;
                            m_meas_task.Close();
                            m_meas_task.Dispose();

                            YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask();
                            MeasSubTask_.Format("*");
                            MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                            for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext())
                            {
                                var m = new YXbsMeassubtask();
                                m.CopyDataFrom(MeasSubTask_);
                                List<YXbsMeassubtasksta> SubTaskStL = new List<YXbsMeassubtasksta>();
                                YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta();
                                MeasSubTaskSt_.Format("*");
                                MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND ((STATUS<>'Z') OR (STATUS IS NULL))", MeasSubTask_.m_id);
                                for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext())
                                {
                                    var m_fr = new YXbsMeassubtasksta();
                                    m_fr.CopyDataFrom(MeasSubTaskSt_);
                                    SubTaskStL.Add(m_fr);
                                    m_fr.Dispose();
                                }
                                MeasSubTaskSt_.Close();
                                MeasSubTaskSt_.Dispose();
                                KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>> key = new KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>(m, SubTaskStL);
                                ICSM_T.meas_st.Add(key);
                                m.Dispose();
                            }
                            MeasSubTask_.Close();
                            MeasSubTask_.Dispose();
                            L_IN.Add(ICSM_T);
                        }
                        task.Close();
                        task.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadTask... " + ex.Message);
                    }
                    logger.Trace("End procedure ReadTask.");
                });
                tsk.Start();
                tsk.Join();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error in ReadTask: " + ex.Message);
            }
            return L_IN;
        }

        /// <summary>
        /// Извлечение с БД ICSM списка всех TASKS
        /// </summary>
        /// <returns></returns>
        public List<CLASS_TASKS> ReadlAllSTasksFromDB()
        {
            // Список объектов в рамках конкретного адаптера ICSM
            L_IN = new List<CLASS_TASKS>();
            try
            {
                #region Load Tasks from DB
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    logger.Trace("Start procedure ReadlAllSTasksFromDB...");
                    try
                    { 
                    CLASS_TASKS ICSM_T = new CLASS_TASKS();
                    ICSM_T.meas_st = new List<KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>>();
                    ICSM_T.meas_task = new YXbsMeastask();
                    ICSM_T.MeasDtParam = new List<YXbsMeasdtparam>();
                    ICSM_T.MeasFreqLst_param = new List<KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>>();
                    ICSM_T.MeasLocParam = new List<YXbsMeaslocparam>();
                    ICSM_T.MeasOther = new YXbsMeasother();
                    ICSM_T.Stations = new List<YXbsMeasstation>();
                    ICSM_T.XbsStationdatform = new List<YXbsStation>();


                    YXbsMeastask task = new YXbsMeastask();
                    task.Format("*");
                    task.Filter = "(ID>0) AND (STATUS IS NOT NULL) AND (STATUS<>'Z')";
                    for (task.OpenRs(); !task.IsEOF(); task.MoveNext()) {
                        ICSM_T = new CLASS_TASKS();
                        YXbsMeastask m_meas_task = new YXbsMeastask();
                        m_meas_task.CopyDataFrom(task);
                        ICSM_T.meas_task = m_meas_task;
                        m_meas_task.Close();
                        m_meas_task.Dispose();

                        List<YXbsMeasstation> LXbsMeasstation = new List<YXbsMeasstation>();
                        YXbsMeasstation XbsMeasstation_ = new YXbsMeasstation();
                        XbsMeasstation_.Format("*");
                        XbsMeasstation_.Filter = string.Format("(ID_XBS_MEASTASK={0})", task.m_id);
                        for (XbsMeasstation_.OpenRs(); !XbsMeasstation_.IsEOF(); XbsMeasstation_.MoveNext())
                        {
                            var m_fr = new YXbsMeasstation();
                            m_fr.CopyDataFrom(XbsMeasstation_);
                            LXbsMeasstation.Add(m_fr);
                            m_fr.Dispose();
                        }
                        XbsMeasstation_.Close();
                        XbsMeasstation_.Dispose();
                        ICSM_T.Stations = LXbsMeasstation;


                        List<YXbsStation> LYXbsStationdatform = new List<YXbsStation>();
                            YXbsStation XbsStationdatform_ = new YXbsStation();
                        XbsStationdatform_.Format("*");
                        XbsStationdatform_.Filter = string.Format("(ID_XBS_MEASTASK={0})", task.m_id);
                        for (XbsStationdatform_.OpenRs(); !XbsStationdatform_.IsEOF(); XbsStationdatform_.MoveNext())
                        {
                            var m_fr = new YXbsStation();
                            m_fr.CopyDataFrom(XbsStationdatform_);
                            LYXbsStationdatform.Add(m_fr);
                            m_fr.Dispose();
                        }
                        XbsStationdatform_.Close();
                        XbsStationdatform_.Dispose();
                        ICSM_T.XbsStationdatform = LYXbsStationdatform;



                        YXbsMeasdtparam MeasDtParam_ = new YXbsMeasdtparam();
                        MeasDtParam_.Format("*");
                        MeasDtParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasDtParam_.OpenRs(); !MeasDtParam_.IsEOF(); MeasDtParam_.MoveNext()) {
                            var m = new YXbsMeasdtparam();
                            m.CopyDataFrom(MeasDtParam_);
                            ICSM_T.MeasDtParam.Add(m);
                            m.Dispose();
                        }
                        MeasDtParam_.Close();
                        MeasDtParam_.Dispose();

                      
                        YXbsMeaslocparam MeasLocParam_ = new YXbsMeaslocparam();
                        MeasLocParam_.Format("*");
                        MeasLocParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasLocParam_.OpenRs(); !MeasLocParam_.IsEOF(); MeasLocParam_.MoveNext()) {
                            var m = new YXbsMeaslocparam();
                            m.CopyDataFrom(MeasLocParam_);
                            ICSM_T.MeasLocParam.Add(m);
                            m.Dispose();
                        }
                        MeasLocParam_.Close();
                        MeasLocParam_.Dispose();

                        YXbsMeasother MeasOther_ = new YXbsMeasother();
                        MeasOther_.Format("*");
                        MeasOther_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasOther_.OpenRs(); !MeasOther_.IsEOF(); MeasOther_.MoveNext()) {
                            var m = new YXbsMeasother();
                            m.CopyDataFrom(MeasOther_);
                            ICSM_T.MeasOther = m;
                            m.Dispose();
                        }
                        MeasOther_.Close();
                        MeasOther_.Dispose();

                        YXbsMeasfreqparam MeasFreqParam_ = new YXbsMeasfreqparam();
                        MeasFreqParam_.Format("*");
                        MeasFreqParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasFreqParam_.OpenRs(); !MeasFreqParam_.IsEOF(); MeasFreqParam_.MoveNext()) {
                            var m = new YXbsMeasfreqparam();
                            m.CopyDataFrom(MeasFreqParam_);
                            List<YXbsMeasfreq> FreqL = new List<YXbsMeasfreq>();
                            YXbsMeasfreq MeasFreqLst_ = new YXbsMeasfreq();
                            MeasFreqLst_.Format("*");
                            MeasFreqLst_.Filter = string.Format("ID_XBS_MeasFreqParam={0}", MeasFreqParam_.m_id);
                            for (MeasFreqLst_.OpenRs(); !MeasFreqLst_.IsEOF(); MeasFreqLst_.MoveNext()) {
                                var m_fr = new YXbsMeasfreq();
                                m_fr.CopyDataFrom(MeasFreqLst_);
                                FreqL.Add(m_fr);
                                m_fr.Dispose();
                            }
                            MeasFreqLst_.Close();
                            MeasFreqLst_.Dispose();

                            KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>> key = new KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>(m, FreqL);
                            ICSM_T.MeasFreqLst_param.Add(key);
                            m.Dispose();
                        }
                        MeasFreqParam_.Close();
                        MeasFreqParam_.Dispose();

                        YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask();
                        MeasSubTask_.Format("*");
                        MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext()) {
                            var m = new YXbsMeassubtask();
                            m.CopyDataFrom(MeasSubTask_);
                            List<YXbsMeassubtasksta> SubTaskStL = new List<YXbsMeassubtasksta>();
                            YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta();
                            MeasSubTaskSt_.Format("*");
                            MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND ((STATUS<>'Z') OR (STATUS IS NULL))", MeasSubTask_.m_id);
                            for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext()) {
                                var m_fr = new YXbsMeassubtasksta();
                                m_fr.CopyDataFrom(MeasSubTaskSt_);
                                SubTaskStL.Add(m_fr);
                                m_fr.Dispose();
                            }
                            MeasSubTaskSt_.Close();
                            MeasSubTaskSt_.Dispose();
                            KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>> key = new KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>(m, SubTaskStL);
                            ICSM_T.meas_st.Add(key);
                            m.Dispose();
                        }
                        MeasSubTask_.Close();
                        MeasSubTask_.Dispose();
                        L_IN.Add(ICSM_T);
                    }
                    task.Close();
                    task.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadlAllSTasksFromDB... " + ex.Message);
                    }
                    logger.Trace("End procedure ReadlAllSTasksFromDB.");
                });
                tsk.Start();
                tsk.Join();
                #endregion
            }
            catch (Exception ex) {
                logger.Error("Error in ReadlAllSTasksFromDB: " + ex.Message);
            }           
            return L_IN;
        }
        
        /// <summary>
        /// Change status
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="NewStatus"></param>
        /// <returns></returns>
        public bool SaveStatusTaskToDB(MeasTask obj, string NewStatus)
        {
            bool isSuccess = true;
            try
            {
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    logger.Trace("Start procedure SaveStatusTaskToDB...");
                    try
                    {
                        YXbsMeastask task = new YXbsMeastask();
                        task.Format("*");
                        task.Filter = string.Format("(ID={0})", obj.Id.Value);
                        for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                        {
                            YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask();
                            MeasSubTask_.Format("*");
                            MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                            for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext())
                            {
                                MeasSubTask M_TS = obj.MeasSubTasks.ToList().Find(t => t.Id.Value == MeasSubTask_.m_id);
                                if (M_TS != null)
                                {
                                    YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta();
                                    MeasSubTaskSt_.Format("*");
                                    MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND ((STATUS<>'Z') OR (STATUS IS NULL))", MeasSubTask_.m_id);
                                    for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext())
                                    {
                                        MeasSubTaskStation M_STX = M_TS.MeasSubTaskStations.ToList().Find(t => t.Id == MeasSubTaskSt_.m_id);
                                        if (M_STX != null)
                                        {
                                            task.m_status = NewStatus;
                                            MeasSubTask_.m_status = NewStatus;
                                            MeasSubTaskSt_.m_status = NewStatus;
                                            MeasSubTaskSt_.Save(null, null);
                                            MeasSubTask_.Save(null, null);
                                            task.Save(null, null);
                                        }
                                    }
                                    MeasSubTaskSt_.Close();
                                    MeasSubTaskSt_.Dispose();
                                }
                            }
                            MeasSubTask_.Close();
                            MeasSubTask_.Dispose();
                        }
                        task.Close();
                        task.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure SaveStatusTaskToDB... " + ex.Message);
                    }
                    logger.Trace("End procedure SaveStatusTaskToDB.");
                });
                tsk.Start();
                tsk.Join();
            }
            catch (Exception ex)
            {
                logger.Error("Error in SaveStatusTaskToDB: " + ex.Message);
            }
            return isSuccess;
        }

        /// <summary>
        /// Обновление статусов объекта MeasTask в базе данных
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SaveStatusTaskToDB(MeasTask obj)
        {
            bool isSuccess = true;
            try
            {
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    logger.Trace("Start procedure SaveStatusTaskToDB...");
                    try
                    { 
                    YXbsMeastask task = new YXbsMeastask();
                    task.Format("*");
                    task.Filter = string.Format("(ID={0})", obj.Id.Value);
                    for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                    {
                        YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask();
                        MeasSubTask_.Format("*");
                        MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext())
                        {
                            MeasSubTask M_TS = obj.MeasSubTasks.ToList().Find(t => t.Id.Value == MeasSubTask_.m_id);
                            if (M_TS != null)
                            {
                                YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta();
                                MeasSubTaskSt_.Format("*");
                                MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND ((STATUS<>'Z') OR (STATUS IS NULL))", MeasSubTask_.m_id);
                                for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext())
                                {
                                    MeasSubTaskStation M_STX = M_TS.MeasSubTaskStations.ToList().Find(t => t.Id == MeasSubTaskSt_.m_id);
                                    if (M_STX != null)
                                    {
                                        task.m_status = obj.Status;
                                        MeasSubTask_.m_status = M_TS.Status;
                                        MeasSubTaskSt_.m_status = M_STX.Status;
                                        MeasSubTaskSt_.Save(null,null);
                                        MeasSubTask_.Save(null,null);
                                        task.Save(null,null);
                                    }
                                }
                                MeasSubTaskSt_.Close();
                                MeasSubTaskSt_.Dispose();
                            }
                        }
                        MeasSubTask_.Close();
                        MeasSubTask_.Dispose();
                    }
                    task.Close();
                    task.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure SaveStatusTaskToDB... " + ex.Message);
                    }
                    logger.Trace("End procedure SaveStatusTaskToDB.");
                });
                tsk.Start();
                tsk.Join();

            }
            catch (Exception ex)
            {
                logger.Error("Error in SaveStatusTaskToDB: " + ex.Message);
            }
            return isSuccess;
        }
        /// <summary>
        /// Запись всех новых ids
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool SaveIdsSdrTasks(MeasTask obj, string ids)
        {
            bool isSuccess = true;
            try
            {
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    logger.Trace("Start procedure SaveIdsSdrTasks...");
                    try
                    {
                        YXbsMeastask task = new YXbsMeastask();
                        task.Format("*");
                        if (task.Fetch(obj.Id.Value))
                        {
                            task.m_id_start = ids;
                            task.Save(null, null);
                        }
                        task.Close();
                        task.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure SaveIdsSdrTasks... " + ex.Message);
                    }
                    logger.Trace("End procedure SaveIdsSdrTasks.");
                });
                tsk.Start();
                tsk.Join();
            }
            catch (Exception ex)
            {
                logger.Error("Error in SaveIdsSdrTasks: " + ex.Message);
            }
            return isSuccess;
        }
        /// <summary>
        /// Получить максимальное значение ids
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetMaXIdsSdrTasks(MeasTask obj)
        {
            int MaxIDs=-1;
            try
            {
                string ids = "";
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    logger.Trace("Start procedure GetMaXIdsSdrTasks...");
                    try
                    {
                        YXbsMeastask task = new YXbsMeastask();
                        task.Format("*");
                        task.Filter = "(ID>0)";
                        task.Order = "[ID] DESC";
                        for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                        {
                            if (task.m_id_start != null)
                            {
                                if (task.m_id_start.Length > 0)
                                {
                                    ids = task.m_id_start;
                                    break;
                                }
                            }
                        }
                        task.Close();
                        task.Dispose();
                        if (ids.Length > 0)
                        {
                            string[] words = ids.Split(new char[] { ';' });
                            if (words.Length > 0)
                            {
                                int val = -1;
                                if (int.TryParse(words[words.Length - 1], out val))
                                {
                                    MaxIDs = val;
                                }
                            }
                        }
                        if (MaxIDs == -1) MaxIDs = 0;
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure GetMaXIdsSdrTasks... " + ex.Message);
                    }
                    logger.Trace("End procedure GetMaXIdsSdrTasks.");
                });
                tsk.Start();
                tsk.Join();
            }
            catch (Exception ex)
            {
                if (MaxIDs == -1) MaxIDs = 0;
                logger.Error("Error in SaveIdsSdrTasks: " + ex.Message);
            }
            return MaxIDs;
        }
        /// <summary>
        /// Получить максимальное значение по таблице MeasSdrResults
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetMaXIdsSdrResults(MeasTask obj)
        {
            int MaxIDs = -1;
            try
            {
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    logger.Trace("Start procedure GetMaXIdsSdrResults...");
                    try
                    {
                        YXbsResMeas res = new YXbsResMeas();
                        res.Format("*");
                        res.Filter = "(ID>0)";
                        res.Order = "[ID] DESC";
                        for (res.OpenRs(); !res.IsEOF(); res.MoveNext())
                        {
                            if (res.m_id > 0)
                            {
                                MaxIDs = res.m_id.Value;
                                break;
                            }
                        }
                        if (MaxIDs == -1) MaxIDs = 0;
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure GetMaXIdsSdrResults... " + ex.Message);
                    }
                    logger.Trace("End procedure GetMaXIdsSdrResults.");
                });
                tsk.Start();
                tsk.Join();
            }
            catch (Exception ex)
            {
                logger.Error("Error in GetMaXIdsSdrResults: " + ex.Message);
            }
            return MaxIDs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SetHistoryStatusTasksInDB(MeasTask obj, string status)
        {
            bool isSuccess = true;
            try
            {
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    logger.Trace("Start procedure SetHistoryStatusTasksInDB...");
                    try
                    {
                        YXbsMeastask task = new YXbsMeastask();
                        task.Format("*");
                        task.Filter = string.Format("(ID={0}) AND (STATUS IS NOT NULL)", obj.Id.Value);
                        for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                        {
                            YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask();
                            MeasSubTask_.Format("*");
                            MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                            for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext())
                            {
                                MeasSubTask M_TS = obj.MeasSubTasks.ToList().Find(t => t.Id.Value == MeasSubTask_.m_id);
                                if (M_TS != null)
                                {
                                    YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta();
                                    MeasSubTaskSt_.Format("*");
                                    MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND ((STATUS<>'Z') OR (STATUS IS NULL))", MeasSubTask_.m_id);
                                    for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext())
                                    {
                                        MeasSubTaskStation M_STX = M_TS.MeasSubTaskStations.ToList().Find(t => t.Id == MeasSubTaskSt_.m_id);
                                        if (M_STX != null)
                                        {
                                            if (M_STX.TimeNextTask.GetValueOrDefault().Subtract(DateTime.Now).TotalSeconds < 0)
                                            {
                                                if (MeasSubTaskSt_.m_status != "Z")
                                                {
                                                    MeasSubTaskSt_.m_status = status;
                                                    MeasSubTaskSt_.Save(null, null);
                                                    isSuccess = true;
                                                }
                                            }
                                        }
                                    }
                                    MeasSubTaskSt_.Close();
                                    MeasSubTaskSt_.Dispose();
                                }
                                MeasSubTask_.m_status = status;
                                MeasSubTask_.Save(null, null);
                            }
                            MeasSubTask_.Close();
                            MeasSubTask_.Dispose();

                            task.m_status = status;
                            task.Save(null, null);
                        }
                        task.Close();
                        task.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure SetHistoryStatusTasksInDB... " + ex.Message);
                    }
                    logger.Trace("End procedure SetHistoryStatusTasksInDB.");
                });
                tsk.Start();
                tsk.Join();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in SetHistoryStatusTasksInDB: " + ex.Message);
            }
            return isSuccess;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? SaveTaskSDRToDB(int SubTaskId, int SubTaskStationId, string TaskId, int SensorId)
        {
           int? NUM_Val = null;
           System.Threading.Thread thread = new System.Threading.Thread(() =>
           {
               Yyy yyy = new Yyy();
               DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
               if (dbConnect.State == System.Data.ConnectionState.Open)
               {
                   DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                   try
                   {
                       logger.Trace("Start procedure SaveTaskToDB...");
                       int? Num = yyy.GetMaxId("XBS_MEASTASK_SDR","NUM");
                       ++Num;

                       bool isNew = false;
                       YXbsMeasTaskSDR meastask = new YXbsMeasTaskSDR();
                       meastask.Format("*");
                       if (!meastask.Fetch(string.Format("MEASTASKID='{0}' AND MEASSUBTASKID={1} AND MEASSUBTASKSTATIONID={2} AND SENSORID={3}", TaskId, SubTaskId, SubTaskStationId, SensorId)))
                       {
                           meastask.New();
                           isNew = true;
                           NUM_Val = Num;
                       }
                       else
                       {
                           NUM_Val = meastask.m_num;
                       }
                       meastask.m_meastaskid = TaskId;
                       meastask.m_meassubtaskid = SubTaskId;
                       meastask.m_meassubtaskstationid = SubTaskStationId;
                       meastask.m_sensorid = SensorId;
                       meastask.m_num = Num;
                       if (isNew)  meastask.Save(dbConnect, transaction);
                       meastask.Close();
                       meastask.Dispose();
                       transaction.Commit();
                       
                   }
                   catch (Exception ex)
                   {
                       try
                       {
                           transaction.Rollback();
                       }
                       catch (Exception e) { transaction.Dispose(); dbConnect.Close(); dbConnect.Dispose(); logger.Error(e.Message); }
                       logger.Error("Error in SaveTaskToDB: " + ex.Message);
                   }
                   finally
                   {
                       transaction.Dispose();
                       dbConnect.Close();
                       dbConnect.Dispose();
                   }
               }
           });
          thread.Start();
          thread.Join();
          return NUM_Val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int? SaveTaskToDB(MeasTask obj)
        {
            Dictionary<List<string>, int?> dicXbsSectorFreq = new Dictionary<List<string>, int?>();
            int? ID = Constants.NullI;
            #region Save Task
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                Yyy yyy = new Yyy();
                DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                if (dbConnect.State == System.Data.ConnectionState.Open)
                {
                    DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    try
                    {
                        logger.Trace("Start procedure SaveTaskToDB...");
                        if (obj.Id != null)
                        {
                            if (obj.Id.Value != Constants.NullI)
                            {
                                YXbsMeastask meastask = new YXbsMeastask();
                                meastask.Format("*");
                                if ((!meastask.Fetch(string.Format("ID={0}", obj.Id.Value))) || (obj.Id.Value == -1))
                                {
                                    meastask.New();
                                    if (obj.MaxTimeBs != null) meastask.m_maxtimebs = obj.MaxTimeBs.GetValueOrDefault();
                                    if (obj.Prio != null) meastask.m_prio = obj.Prio.GetValueOrDefault();
                                    meastask.m_resulttype = obj.ResultType.ToString();
                                    meastask.m_name = obj.Name;
                                    meastask.m_status = obj.Status;
                                    meastask.m_task = obj.Task.ToString();
                                    meastask.m_type = obj.Type;
                                    meastask.m_createdby = obj.CreatedBy;
                                    meastask.m_executionmode = obj.ExecutionMode.ToString();
                                    if (obj.MeasTimeParamList != null)
                                    {
                                        meastask.m_days = obj.MeasTimeParamList.Days;
                                        meastask.m_perstart = obj.MeasTimeParamList.PerStart;
                                        meastask.m_perstop = obj.MeasTimeParamList.PerStop;
                                        if (obj.MeasTimeParamList.TimeStart != null) meastask.m_timestart = obj.MeasTimeParamList.TimeStart.GetValueOrDefault();
                                        if (obj.MeasTimeParamList.TimeStop != null) meastask.m_timestop = obj.MeasTimeParamList.TimeStop.GetValueOrDefault();
                                        if (obj.MeasTimeParamList.PerInterval != null) meastask.m_perinterval = obj.MeasTimeParamList.PerInterval.GetValueOrDefault();
                                    }
                                    if (obj.DateCreated != null) meastask.m_datecreated = obj.DateCreated.GetValueOrDefault();
                                    if (obj.OrderId != null) meastask.m_orderid = obj.OrderId.GetValueOrDefault();
                                    ID = meastask.Save(dbConnect, transaction);
                                    obj.Id.Value = ID.Value;
                                }
                                else
                                {
                                    ID = meastask.m_id;
                                    obj.Id.Value = ID.Value;
                                }
                                meastask.Close();
                                meastask.Dispose();
                            }
                            if (ID != Constants.NullI)
                            {
                                if (obj.MeasDtParam != null)
                                {
                                    List<Yyy> BlockInsert_YXbsMeasdtparam = new List<Yyy>();
                                    foreach (MeasDtParam dt_param in new List<MeasDtParam> { obj.MeasDtParam })
                                    {
                                        if (dt_param != null)
                                        {
                                            YXbsMeasdtparam dtr = new YXbsMeasdtparam();
                                            dtr.Format("*");
                                            if (!dtr.Fetch(string.Format("ID_XBS_MEASTASK={0}", ID)))
                                            {
                                                dtr.New();
                                                dtr.m_demod = dt_param.Demod;
                                                dtr.m_detecttype = dt_param.DetectType.ToString();
                                                dtr.m_id_xbs_meastask = ID;
                                                dtr.m_ifattenuation = dt_param.IfAttenuation;
                                                dtr.m_preamplification = dt_param.Preamplification;
                                                dtr.m_rfattenuation = dt_param.RfAttenuation;
                                                if (dt_param.RBW != null) dtr.m_rbw = dt_param.RBW.GetValueOrDefault();
                                                if (dt_param.VBW != null) dtr.m_vbw = dt_param.VBW.GetValueOrDefault();
                                                dtr.m_typemeasurements = dt_param.TypeMeasurements.ToString();
                                                if (dt_param.MeasTime != null) dtr.m_meastime = dt_param.MeasTime.GetValueOrDefault();
                                                dtr.m_mode = dt_param.Mode.ToString();
                                                for (int i = 0; i < dtr.getAllFields.Count; i++)
                                                    dtr.getAllFields[i].Value = dtr.valc[i];
                                                BlockInsert_YXbsMeasdtparam.Add(dtr);

                                            }
                                            dtr.Close();
                                            dtr.Dispose();

                                        }
                                    }
                                    if (BlockInsert_YXbsMeasdtparam.Count > 0)
                                    {
                                        YXbsMeasdtparam YXbsMeasdtparam11 = new YXbsMeasdtparam();
                                        YXbsMeasdtparam11.Format("*");
                                        YXbsMeasdtparam11.New();
                                        YXbsMeasdtparam11.SaveBath(BlockInsert_YXbsMeasdtparam, dbConnect, transaction);
                                        YXbsMeasdtparam11.Close();
                                        YXbsMeasdtparam11.Dispose();
                                    }
                                }


                                if (obj.MeasLocParams != null)
                                {
                                    foreach (MeasLocParam loc_param in obj.MeasLocParams.ToArray())
                                    {
                                        if (loc_param.Id != null)
                                        {
                                            if (loc_param.Id.Value != Constants.NullI)
                                            {
                                                YXbsMeaslocparam prm_loc = new YXbsMeaslocparam();
                                                prm_loc.Format("*");
                                                if (!prm_loc.Fetch(string.Format("(ID_XBS_MEASTASK={0}) AND (Lat={1}) AND (Lon={2})", ID, loc_param.Lat.GetValueOrDefault().ToString().Replace(",", "."), loc_param.Lon.GetValueOrDefault().ToString().Replace(",", "."))))
                                                {
                                                    prm_loc.New();
                                                    if (loc_param.ASL != null) prm_loc.m_asl = loc_param.ASL.GetValueOrDefault();
                                                    if (loc_param.Lat != null) prm_loc.m_lat = loc_param.Lat.GetValueOrDefault();
                                                    if (loc_param.Lon != null) prm_loc.m_lon = loc_param.Lon.GetValueOrDefault();
                                                    if (loc_param.MaxDist != null) prm_loc.m_maxdist = loc_param.MaxDist.GetValueOrDefault();
                                                    prm_loc.m_id_xbs_meastask = ID;
                                                    int? ID_loc_params = prm_loc.Save(dbConnect, transaction);
                                                    loc_param.Id.Value = ID_loc_params.Value;

                                                }
                                                prm_loc.Close();
                                                prm_loc.Dispose();
                                            }
                                        }
                                    }
                                }
                                if (obj.MeasOther != null)
                                {
                                    List<Yyy> BlockInsert_YXbsMeasother = new List<Yyy>();
                                    foreach (MeasOther other in new List<MeasOther> { obj.MeasOther })
                                    {
                                        if (other != null)
                                        {
                                            YXbsMeasother prm_oth = new YXbsMeasother();
                                            prm_oth.Format("*");
                                            if (!prm_oth.Fetch(string.Format("ID_XBS_MEASTASK={0}", ID)))
                                            {
                                                prm_oth.New();
                                                prm_oth.m_id_xbs_meastask = ID;
                                                if (other.LevelMinOccup != null) prm_oth.m_levelminoccup = other.LevelMinOccup.GetValueOrDefault();
                                                if (other.NChenal != null) prm_oth.m_nchenal = other.NChenal.GetValueOrDefault();
                                                if (other.SwNumber != null) prm_oth.m_swnumber = other.SwNumber.GetValueOrDefault();
                                                prm_oth.m_typespectrumscan = other.TypeSpectrumScan.ToString();
                                                prm_oth.m_typespectrumoccupation = other.TypeSpectrumOccupation.ToString();
                                                for (int i = 0; i < prm_oth.getAllFields.Count; i++)
                                                    prm_oth.getAllFields[i].Value = prm_oth.valc[i];
                                                BlockInsert_YXbsMeasother.Add(prm_oth);
                                            }
                                            prm_oth.Close();
                                            prm_oth.Dispose();
                                        }
                                    }
                                    if (BlockInsert_YXbsMeasother.Count > 0)
                                    {
                                        YXbsMeasother YXbsMeasother11 = new YXbsMeasother();
                                        YXbsMeasother11.Format("*");
                                        YXbsMeasother11.New();
                                        YXbsMeasother11.SaveBath(BlockInsert_YXbsMeasother, dbConnect, transaction);
                                        YXbsMeasother11.Close();
                                        YXbsMeasother11.Dispose();
                                    }
                                }


                                if (obj.MeasSubTasks != null)
                                {
                                    foreach (MeasSubTask sub_task in obj.MeasSubTasks.ToArray())
                                    {
                                        int? ID_sub_task = Constants.NullI;
                                        if (sub_task.Id != null)
                                        {
                                            if (sub_task.Id.Value != Constants.NullI)
                                            {
                                                YXbsMeassubtask prm_sub_task = new YXbsMeassubtask();
                                                prm_sub_task.Format("*");
                                                if (!prm_sub_task.Fetch(sub_task.Id.Value))
                                                {
                                                    prm_sub_task.New();
                                                    sub_task.Id.Value = ID_sub_task.Value;
                                                    prm_sub_task.m_id_xbs_meastask = ID;
                                                    if (sub_task.Interval != null) prm_sub_task.m_interval = sub_task.Interval.GetValueOrDefault();
                                                    prm_sub_task.m_status = sub_task.Status;
                                                    prm_sub_task.m_timestart = sub_task.TimeStart;
                                                    prm_sub_task.m_timestop = sub_task.TimeStop;
                                                    ID_sub_task = prm_sub_task.Save(dbConnect, transaction);
                                                    sub_task.Id.Value = ID_sub_task.Value;
                                                }
                                                else
                                                {
                                                    ID_sub_task = prm_sub_task.m_id;
                                                    sub_task.Id.Value = ID_sub_task.Value;
                                                }
                                                prm_sub_task.Close();
                                                prm_sub_task.Dispose();
                                            }
                                        }

                                        if (sub_task.MeasSubTaskStations != null)
                                        {
                                            foreach (MeasSubTaskStation sub_task_st in sub_task.MeasSubTaskStations.ToArray())
                                            {
                                                if (sub_task_st.Id != Constants.NullI)
                                                {
                                                    YXbsMeassubtasksta prm_sub_task_st = new YXbsMeassubtasksta();
                                                    prm_sub_task_st.Format("*");
                                                    if (!prm_sub_task_st.Fetch(sub_task_st.Id))
                                                    {
                                                        prm_sub_task_st.New();
                                                        prm_sub_task_st.m_id_xb_meassubtask = ID_sub_task;
                                                        prm_sub_task_st.m_status = sub_task_st.Status;
                                                        if (sub_task_st.TimeNextTask != null) prm_sub_task_st.m_timenexttask = sub_task_st.TimeNextTask.GetValueOrDefault();
                                                        if (sub_task_st.Count != null) prm_sub_task_st.m_count = sub_task_st.Count.GetValueOrDefault();
                                                        prm_sub_task_st.m_id_xbs_sensor = sub_task_st.StationId.Value;
                                                        sub_task_st.Id = prm_sub_task_st.Save(dbConnect, transaction).Value;
                                                    }
                                                    else
                                                    {
                                                        sub_task_st.Id = prm_sub_task_st.m_id.Value;
                                                    }
                                                    prm_sub_task_st.Close();
                                                    prm_sub_task_st.Dispose();
                                                }
                                            }
                                        }
                                    }
                                }

                                if (obj.MeasFreqParam != null)
                                {
                                    foreach (MeasFreqParam freq_param in new List<MeasFreqParam> { obj.MeasFreqParam })
                                    {
                                        int? ID_MeasFreqParam = Constants.NullI;
                                        if (freq_param != null)
                                        {
                                            YXbsMeasfreqparam prm_MeasFreqParam = new YXbsMeasfreqparam();
                                            prm_MeasFreqParam.Format("*");
                                            if (!prm_MeasFreqParam.Fetch(string.Format("ID_XBS_MEASTASK={0}", ID)))
                                            {
                                                prm_MeasFreqParam.New();
                                                prm_MeasFreqParam.m_id_xbs_meastask = ID;
                                                prm_MeasFreqParam.m_mode = freq_param.Mode.ToString();
                                                if (freq_param.RgL != null) prm_MeasFreqParam.m_rgl = freq_param.RgL.GetValueOrDefault();
                                                if (freq_param.RgU != null) prm_MeasFreqParam.m_rgu = freq_param.RgU.GetValueOrDefault();
                                                if (freq_param.Step != null) prm_MeasFreqParam.m_step = freq_param.Step.GetValueOrDefault();
                                                ID_MeasFreqParam = prm_MeasFreqParam.Save(dbConnect, transaction);
                                            }
                                            else
                                            {
                                                ID_MeasFreqParam = prm_MeasFreqParam.m_id;
                                            }
                                            prm_MeasFreqParam.Close();
                                            prm_MeasFreqParam.Dispose();
                                        }

                                        if (freq_param.MeasFreqs != null)
                                        {
                                            List<Yyy> BlockInsert_MeasFreq = new List<Yyy>();
                                            foreach (MeasFreq sub_freq_st in freq_param.MeasFreqs.ToArray())
                                            {
                                                YXbsMeasfreq prm_sub_freq_st = new YXbsMeasfreq();
                                                prm_sub_freq_st.Format("*");
                                                if (sub_freq_st != null)
                                                {
                                                    if (!prm_sub_freq_st.Fetch(string.Format("(ID_XBS_MEASFREQPARAM={0}) AND (FREQ={1})", ID_MeasFreqParam, sub_freq_st.Freq.ToString().Replace(",", "."))))
                                                        prm_sub_freq_st.New();

                                                    prm_sub_freq_st.m_id_xbs_measfreqparam = ID_MeasFreqParam;
                                                    prm_sub_freq_st.m_freq = sub_freq_st.Freq;
                                                }
                                                else
                                                {
                                                    prm_sub_freq_st.m_id_xbs_measfreqparam = ID_MeasFreqParam;
                                                }

                                                for (int i = 0; i < prm_sub_freq_st.getAllFields.Count; i++)
                                                    prm_sub_freq_st.getAllFields[i].Value = prm_sub_freq_st.valc[i];

                                                BlockInsert_MeasFreq.Add(prm_sub_freq_st);
                                                prm_sub_freq_st.Close();
                                                prm_sub_freq_st.Dispose();
                                            }
                                            if (BlockInsert_MeasFreq.Count > 0)
                                            {
                                                YXbsMeasfreq YXbsMeasfreq11 = new YXbsMeasfreq();
                                                YXbsMeasfreq11.Format("*");
                                                YXbsMeasfreq11.New();
                                                YXbsMeasfreq11.SaveBath(BlockInsert_MeasFreq, dbConnect, transaction);
                                                YXbsMeasfreq11.Close();
                                                YXbsMeasfreq11.Dispose();
                                            }
                                        }
                                    }
                                }

                                if (obj.Stations != null)
                                {
                                    List<Yyy> BlockInsert_YXbsMeasstation = new List<Yyy>();
                                    foreach (MeasStation s_st in obj.Stations.ToArray())
                                    {
                                        YXbsMeasstation prm_XbsMeasstation = new YXbsMeasstation();
                                        prm_XbsMeasstation.Format("*");
                                        if (prm_XbsMeasstation != null)
                                        {
                                            if (!prm_XbsMeasstation.Fetch(string.Format("(STATIONID={0}) AND (ID_XBS_MEASTASK={1})", s_st.StationId.Value, ID)))
                                                prm_XbsMeasstation.New();
                                            prm_XbsMeasstation.m_id_xbs_meastask = ID;
                                            prm_XbsMeasstation.m_stationid = s_st.StationId.Value;
                                            prm_XbsMeasstation.m_stationtype = s_st.StationType;
                                        }
                                        else
                                        {
                                            prm_XbsMeasstation.m_id_xbs_meastask = ID;
                                        }

                                        for (int i = 0; i < prm_XbsMeasstation.getAllFields.Count; i++)
                                            prm_XbsMeasstation.getAllFields[i].Value = prm_XbsMeasstation.valc[i];

                                        BlockInsert_YXbsMeasstation.Add(prm_XbsMeasstation);
                                        prm_XbsMeasstation.Close();
                                        prm_XbsMeasstation.Dispose();
                                    }
                                    if (BlockInsert_YXbsMeasstation.Count > 0)
                                    {
                                        YXbsMeasstation YXbsMeasstation11 = new YXbsMeasstation();
                                        YXbsMeasstation11.Format("*");
                                        YXbsMeasstation11.New();
                                        YXbsMeasstation11.SaveBath(BlockInsert_YXbsMeasstation, dbConnect, transaction);
                                        YXbsMeasstation11.Close();
                                        YXbsMeasstation11.Dispose();
                                    }
                                }

                                if (obj.StationsForMeasurements != null)
                                {
                                    foreach (StationDataForMeasurements StationData_param in obj.StationsForMeasurements.ToArray())
                                    {
                                        int? ID_loc_params = -1;
                                        YXbsStation prm_loc = new YXbsStation();
                                        prm_loc.Format("*");

                                        prm_loc.New();
                                        if (StationData_param.GlobalSID != null) prm_loc.m_globalsid = StationData_param.GlobalSID;
                                        if (StationData_param.Standart != null) prm_loc.m_standart = StationData_param.Standart;
                                        if (StationData_param.Status != null) prm_loc.m_status = StationData_param.Status;
                                        if (StationData_param.IdStation != Constants.NullI) prm_loc.m_id_station = StationData_param.IdStation;
                                        prm_loc.m_id_xbs_meastask = ID;
                                        if (StationData_param.LicenseParameter != null)
                                        {
                                            if (StationData_param.LicenseParameter.CloseDate != null) prm_loc.m_closedate = StationData_param.LicenseParameter.CloseDate.GetValueOrDefault();
                                            if (StationData_param.LicenseParameter.DozvilName != null) prm_loc.m_dozvilname = StationData_param.LicenseParameter.DozvilName;
                                            if (StationData_param.LicenseParameter.EndDate != null) prm_loc.m_enddate = StationData_param.LicenseParameter.EndDate.GetValueOrDefault();
                                            if (StationData_param.LicenseParameter.StartDate != null) prm_loc.m_startdate = StationData_param.LicenseParameter.StartDate.GetValueOrDefault();
                                        }
                                        int? ID_Ownerdata = null;
                                        int? ID_site_formeas = null;
                                        if (StationData_param.Owner != null)
                                        {
                                            YXbsOwnerdata owner_formeas = new YXbsOwnerdata();
                                            owner_formeas.Format("*");
                                            //if (!owner_formeas.Fetch(string.Format("(ADDRES='{0}') AND (CODE='{1}') AND (OKPO='{2}') AND (OWNERNAME='{3}' AND (ZIP='{4}'))", StationData_param.Owner.Addres, StationData_param.Owner.Code, StationData_param.Owner.OKPO, StationData_param.Owner.OwnerName, StationData_param.Owner.Zip)))
                                            //{
                                                owner_formeas.New();
                                                if (StationData_param.Owner.Addres != null) owner_formeas.m_addres = StationData_param.Owner.Addres;
                                                if (StationData_param.Owner.Code != null) owner_formeas.m_code = StationData_param.Owner.Code;
                                                if (StationData_param.Owner.OKPO != null) owner_formeas.m_okpo = StationData_param.Owner.OKPO;
                                                if (StationData_param.Owner.OwnerName != null) owner_formeas.m_ownername = StationData_param.Owner.OwnerName;
                                                if (StationData_param.Owner.Zip != null) owner_formeas.m_zip = StationData_param.Owner.Zip;
                                                ID_Ownerdata = owner_formeas.Save(dbConnect, transaction);
                                            //}
                                            owner_formeas.Close();
                                            owner_formeas.Dispose();
                                        }
                                        if (StationData_param.Site != null)
                                        {
                                            YXbsStationSite site_formeas = new YXbsStationSite();
                                            site_formeas.Format("*");
                                            //if (!site_formeas.Fetch(string.Format("(REGION='{0}') AND (ADDRES='{1}') AND (LAT={2}) AND (LON={3})", StationData_param.Site.Region, StationData_param.Site.Adress, ID, StationData_param.Site.Lat.GetValueOrDefault().ToString().Replace(",", "."), StationData_param.Site.Lon.GetValueOrDefault().ToString().Replace(",", "."))))
                                            //{
                                                site_formeas.New();
                                                if (StationData_param.Site.Lat != null) site_formeas.m_lat = StationData_param.Site.Lat.GetValueOrDefault();
                                                if (StationData_param.Site.Lon != null) site_formeas.m_lon = StationData_param.Site.Lon.GetValueOrDefault();
                                                if (StationData_param.Site.Region != null) site_formeas.m_region = StationData_param.Site.Region;
                                                if (StationData_param.Site.Adress != null) site_formeas.m_addres = StationData_param.Site.Adress;

                                                ID_site_formeas = site_formeas.Save(dbConnect, transaction);

                                                site_formeas.Close();
                                                site_formeas.Dispose();
                                            //}
                                        }
                                        prm_loc.m_id_xbs_stationsite = ID_site_formeas;
                                        prm_loc.m_id_xbs_ownerdata = ID_Ownerdata;

                                        ID_loc_params = prm_loc.Save(dbConnect, transaction);
                                        prm_loc.Close();
                                        prm_loc.Dispose();

                                        if (ID_loc_params > 0)
                                        {
                                            YXbsLinkMeasStation yXbsLinkMeasStation = new YXbsLinkMeasStation();
                                            yXbsLinkMeasStation.Format("*");
                                            yXbsLinkMeasStation.New();
                                            yXbsLinkMeasStation.m_id_xbs_station = ID_loc_params;
                                            yXbsLinkMeasStation.m_id_xbs_meastask = ID;
                                            yXbsLinkMeasStation.Save(dbConnect, transaction);
                                            yXbsLinkMeasStation.Close();
                                            yXbsLinkMeasStation.Dispose();
                                        }

                                        if (StationData_param.Sectors != null)
                                        {
                                            foreach (SectorStationForMeas sec in StationData_param.Sectors.ToArray())
                                            {
                                                YXbsSector sect_formeas = new YXbsSector();
                                                sect_formeas.Format("*");
                                                sect_formeas.New();
                                                if (sec.AGL != null) sect_formeas.m_agl = sec.AGL.GetValueOrDefault();
                                                if (sec.Azimut != null) sect_formeas.m_azimut = sec.Azimut.GetValueOrDefault();
                                                if (sec.BW != null) sect_formeas.m_bw = sec.BW.GetValueOrDefault();
                                                if (sec.ClassEmission != null) sect_formeas.m_classemission = sec.ClassEmission;
                                                if (sec.EIRP != null) sect_formeas.m_eirp = sec.EIRP.GetValueOrDefault();
                                                sect_formeas.m_idsector = sec.IdSector;
                                                sect_formeas.m_id_xbs_station = ID_loc_params;
                                                int? ID_secformeas = sect_formeas.Save(dbConnect, transaction);
                                                sect_formeas.Close();
                                                sect_formeas.Dispose();

                                                if (sec.Frequencies != null)
                                                {
                                                    List<Yyy> BlockInsert = new List<Yyy>();
                                                    foreach (FrequencyForSectorFormICSM F in sec.Frequencies)
                                                    {
                                                        int? ID_sectorfreq = null;
                                                        List<string> valueKey = new List<string> { F.ChannalNumber.GetValueOrDefault().ToString(), F.IdPlan.GetValueOrDefault().ToString(), F.Frequency.ToString() };
                                                        YXbsSectorFreq freq_formeas = new YXbsSectorFreq();
                                                        freq_formeas.Format("*");
                                                        
                                                            //if (!freq_formeas.Fetch(string.Format("(CHANNALNUMBER={0}) AND (IDPLAN={1}) AND (FREQUENCY={2})", F.ChannalNumber.GetValueOrDefault(), F.IdPlan.GetValueOrDefault(), ((double?)F.Frequency).ToString().Replace(",", "."))))
                                                            {
                                                                freq_formeas.New();
                                                                if (F.ChannalNumber != null) freq_formeas.m_channalnumber = F.ChannalNumber.GetValueOrDefault();
                                                                if (F.Frequency != null) freq_formeas.m_frequency = (double?)F.Frequency;
                                                                if (F.IdPlan != null) freq_formeas.m_idplan = F.IdPlan.GetValueOrDefault();
                                                                ID_sectorfreq = freq_formeas.Save(dbConnect, transaction);
                                                            }
                                                            //else
                                                            //{
                                                                //ID_sectorfreq = freq_formeas.m_id;
                                                            //}

                                                        
                                                        freq_formeas.Close();
                                                        freq_formeas.Dispose();

                                                        if ((ID_sectorfreq != null) && (ID_secformeas != null))
                                                        {
                                                            YXbsLinkSectorFreq yXbsLinkSectorFreq = new YXbsLinkSectorFreq();
                                                            yXbsLinkSectorFreq.Format("*");
                                                            yXbsLinkSectorFreq.New();
                                                            yXbsLinkSectorFreq.m_id_xbs_sectorfreq = ID_sectorfreq;
                                                            yXbsLinkSectorFreq.m_id_xbs_sector = ID_secformeas;

                                                            for (int i = 0; i < yXbsLinkSectorFreq.getAllFields.Count; i++)
                                                                yXbsLinkSectorFreq.getAllFields[i].Value = yXbsLinkSectorFreq.valc[i];
                                                            BlockInsert.Add(yXbsLinkSectorFreq);
                                                            yXbsLinkSectorFreq.Close();
                                                            yXbsLinkSectorFreq.Dispose();
                                                        }
                                                      
                                                    }
                                                    if (BlockInsert.Count > 0)
                                                    {
                                                        YXbsLinkSectorFreq freq_formeas11 = new YXbsLinkSectorFreq();
                                                        freq_formeas11.Format("*");
                                                        freq_formeas11.New();
                                                        freq_formeas11.SaveBath(BlockInsert, dbConnect, transaction);
                                                        freq_formeas11.Close();
                                                        freq_formeas11.Dispose();
                                                    }
                                                }

                                                if (sec.MaskBW != null)
                                                {
                                                    List<Yyy> BlockInsert = new List<Yyy>();
                                                    foreach (MaskElements F in sec.MaskBW)
                                                    {
                                                        int? SectorMaskElemId = null;
                                                        YXbsSectorMaskElem yXbsMaskelements = new YXbsSectorMaskElem();
                                                        yXbsMaskelements.Format("*");
                                                        //if (!yXbsMaskelements.Fetch(string.Format("(BW={0}) AND (LEVEL={1})", F.BW.GetValueOrDefault().ToString().Replace(",", "."), F.level.GetValueOrDefault().ToString().Replace(",", "."))))
                                                        {
                                                            yXbsMaskelements.New();
                                                            if (F.BW != null) yXbsMaskelements.m_bw = F.BW.GetValueOrDefault();
                                                            if (F.level != null) yXbsMaskelements.m_level = F.level.GetValueOrDefault();
                                                            SectorMaskElemId = yXbsMaskelements.Save(dbConnect, transaction);
                                                        }
                                                        //else
                                                        //{
                                                            //SectorMaskElemId = yXbsMaskelements.m_id;
                                                        //}

                                                        yXbsMaskelements.Close();
                                                        yXbsMaskelements.Dispose();

                                                        if ((SectorMaskElemId != null) && (ID_secformeas != null))
                                                        {
                                                            YXbsLinkSectorMask yXbsLinkSectorMask = new YXbsLinkSectorMask();
                                                            yXbsLinkSectorMask.Format("*");
                                                            yXbsLinkSectorMask.New();
                                                            yXbsLinkSectorMask.m_id_sectormaskelem = SectorMaskElemId;
                                                            yXbsLinkSectorMask.m_id_xbs_sector = ID_secformeas;

                                                            for (int i = 0; i < yXbsLinkSectorMask.getAllFields.Count; i++)
                                                                yXbsLinkSectorMask.getAllFields[i].Value = yXbsLinkSectorMask.valc[i];
                                                            BlockInsert.Add(yXbsLinkSectorMask);
                                                            yXbsLinkSectorMask.Close();
                                                            yXbsLinkSectorMask.Dispose();
                                                        }
                                                    }
                                                    if (BlockInsert.Count > 0)
                                                    {
                                                        YXbsLinkSectorMask freq_formeas11 = new YXbsLinkSectorMask();
                                                        freq_formeas11.Format("*");
                                                        freq_formeas11.New();
                                                        freq_formeas11.SaveBath(BlockInsert, dbConnect, transaction);
                                                        freq_formeas11.Close();
                                                        freq_formeas11.Dispose();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception e) {   transaction.Dispose();   dbConnect.Close();      dbConnect.Dispose(); logger.Error(e.Message); }
                        ID = Constants.NullI;
                        logger.Error("Error in SaveTaskToDB: " + ex.Message);
                    }
                    finally
                    {
                        transaction.Dispose();
                        dbConnect.Close();
                        dbConnect.Dispose();
                    }
                }
                else
                {
                    dbConnect.Close();
                    dbConnect.Dispose();
                }
                 //GlobalInit.blockingCollectionMeasTask.TryAdd(obj.Id.Value, obj);
                 #endregion
                });
            thread.Start();
            thread.Join();
            logger.Trace("End procedure SaveTaskToDB.");
            return ID;
        }

       

    }
}
