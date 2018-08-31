using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Globalization;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer;
using System.Data.Common;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    /// <summary>
    /// Класс, извлекающий данные
    /// из поля ORDER_DATA таблицы SYS_ARGUS_ORM
    /// с целью заполнения структуры MEAS_TASK
    /// </summary>
    public class ClassesDBGetTasks 
    {
        public static ILogger logger;
        public ClassesDBGetTasks(ILogger log)
        {
            if (logger==null) logger = log;
        }
       

        private List<ClassTasks> L_IN { get; set; }


        public static void GetMeasTaskSDRNum(int NumValue, out int TaskId, out int SubTaskId, out int SubTaskStationId, out int SensorId)
        {
           int _TaskId = 0;
           int _SubTaskId = 0;
           int _SubTaskStationId = 0;
           int _SensorId = 0;
            System.Threading.Thread thread = new System.Threading.Thread(() =>
            {
                    YXbsMeasTaskSDR meastask = new YXbsMeasTaskSDR();
                    meastask.Format("*");
                    if (meastask.Fetch(string.Format("NUM={0}", NumValue)))
                    {
                        _TaskId = (int)meastask.m_meastaskid;
                        _SubTaskId = (int)meastask.m_meassubtaskid;
                        _SubTaskStationId = (int)meastask.m_meassubtaskstationid;
                        _SensorId = (int)meastask.m_sensorid;
                    }
                    meastask.Close();
                    meastask.Dispose();
               
            });
            thread.Start();
            thread.Join();

            TaskId = _TaskId;
            SubTaskId = _SubTaskId;
            SubTaskStationId = _SubTaskStationId;
            SensorId = _SensorId;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? SaveTaskSDRToDB(int SubTaskId, int SubTaskStationId, int TaskId, int SensorId)
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
                        int? Num = yyy.GetMaxId("XBS_MEASTASK_SDR", "NUM");
                        ++Num;

                        YXbsMeasTaskSDR meastask = new YXbsMeasTaskSDR();
                        meastask.Format("*");
                        meastask.New();
                        meastask.m_meastaskid = TaskId;
                        meastask.m_meassubtaskid = SubTaskId;
                        meastask.m_meassubtaskstationid = SubTaskStationId;
                        meastask.m_sensorid = SensorId;
                        meastask.m_num = Num;
                        meastask.Save(dbConnect, transaction);
                        meastask.Close();
                        meastask.Dispose();
                        transaction.Commit();
                        NUM_Val = Num;
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

        public List<ClassTasks> ReadTask(int MeasTaskId)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            L_IN = new List<ClassTasks>();
            try
            {
                #region Load Tasks from DB
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    logger.Trace("Start procedure ReadTask...");
                    ClassTasks ICSM_T = new ClassTasks();
                    ICSM_T.meas_st = new List<KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>>();
                    ICSM_T.meas_task = new YXbsMeastask();
                    ICSM_T.MeasDtParam = new List<YXbsMeasdtparam>();
                    ICSM_T.MeasFreqLst_param = new List<KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>>();
                    ICSM_T.MeasLocParam = new List<YXbsMeaslocparam>();
                    ICSM_T.MeasOther = new YXbsMeasother();
                    ICSM_T.MeasTimeParamList = new YXbsMeastimeparaml();
                    ICSM_T.Stations = new List<YXbsMeasstation>();
                    ICSM_T.XbsStationdatform = new List<YXbsStationdatform>();


                    YXbsMeastask task = new YXbsMeastask();
                    task.Format("*");
                    task.Filter = string.Format("(ID={0}) AND (STATUS IS NOT NULL) AND (STATUS<>'Z')",MeasTaskId);
                    for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                    {
                        ICSM_T = new ClassTasks();
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


                        List<YXbsStationdatform> LYXbsStationdatform = new List<YXbsStationdatform>();
                        YXbsStationdatform XbsStationdatform_ = new YXbsStationdatform();
                        XbsStationdatform_.Format("*");
                        XbsStationdatform_.Filter = string.Format("(ID_XBS_MEASTASK={0})", task.m_id);
                        for (XbsStationdatform_.OpenRs(); !XbsStationdatform_.IsEOF(); XbsStationdatform_.MoveNext())
                        {
                            var m_fr = new YXbsStationdatform();
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

                        YXbsMeastimeparaml TimeParamList_ = new YXbsMeastimeparaml();
                        TimeParamList_.Format("*");
                        TimeParamList_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (TimeParamList_.OpenRs(); !TimeParamList_.IsEOF(); TimeParamList_.MoveNext())
                        {
                            var m = new YXbsMeastimeparaml();
                            m.CopyDataFrom(TimeParamList_);
                            ICSM_T.MeasTimeParamList = m;
                            m.Dispose();
                        }
                        TimeParamList_.Close();
                        TimeParamList_.Dispose();

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
                            MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND (STATUS<>'Z')", MeasSubTask_.m_id);
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


        public List<ClassTasks> ShortReadTask(int MeasTaskId)
        {
            L_IN = new List<ClassTasks>();
            try
            {
                #region Load Tasks from DB
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    logger.Trace("Start procedure ReadTask...");
                    // сканирование по объектам БД
                    ClassTasks ICSM_T = new ClassTasks();
                    ICSM_T.meas_st = new List<KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>>();
                    ICSM_T.meas_task = new YXbsMeastask();
                    ICSM_T.MeasDtParam = new List<YXbsMeasdtparam>();
                    ICSM_T.MeasFreqLst_param = new List<KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>>();
                    ICSM_T.MeasLocParam = new List<YXbsMeaslocparam>();
                    ICSM_T.MeasOther = new YXbsMeasother();
                    ICSM_T.MeasTimeParamList = new YXbsMeastimeparaml();
                    ICSM_T.Stations = new List<YXbsMeasstation>();
                    ICSM_T.XbsStationdatform = new List<YXbsStationdatform>();


                    //подключение к БД
                    YXbsMeastask task = new YXbsMeastask();
                    task.Format("*");
                    task.Filter = string.Format("(ID={0}) AND (STATUS IS NOT NULL) AND (STATUS<>'Z')", MeasTaskId);
                    for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                    {
                        ICSM_T = new ClassTasks();
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

                        YXbsMeastimeparaml TimeParamList_ = new YXbsMeastimeparaml();
                        TimeParamList_.Format("*");
                        TimeParamList_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (TimeParamList_.OpenRs(); !TimeParamList_.IsEOF(); TimeParamList_.MoveNext())
                        {
                            var m = new YXbsMeastimeparaml();
                            m.CopyDataFrom(TimeParamList_);
                            ICSM_T.MeasTimeParamList = m;
                            m.Dispose();
                        }
                        TimeParamList_.Close();
                        TimeParamList_.Dispose();

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
                            MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND (STATUS<>'Z')", MeasSubTask_.m_id);
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
        public List<ClassTasks> ReadlAllSTasksFromDB()
        {
            // Список объектов в рамках конкретного адаптера ICSM
            L_IN = new List<ClassTasks>();
            try
            {
                #region Load Tasks from DB
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    logger.Trace("Start procedure ReadlAllSTasksFromDB...");
                    ClassTasks ICSM_T = new ClassTasks();
                    ICSM_T.meas_st = new List<KeyValuePair<YXbsMeassubtask, List<YXbsMeassubtasksta>>>();
                    ICSM_T.meas_task = new YXbsMeastask();
                    ICSM_T.MeasDtParam = new List<YXbsMeasdtparam>();
                    ICSM_T.MeasFreqLst_param = new List<KeyValuePair<YXbsMeasfreqparam, List<YXbsMeasfreq>>>();
                    ICSM_T.MeasLocParam = new List<YXbsMeaslocparam>();
                    ICSM_T.MeasOther = new YXbsMeasother();
                    ICSM_T.MeasTimeParamList = new YXbsMeastimeparaml();
                    ICSM_T.Stations = new List<YXbsMeasstation>();
                    ICSM_T.XbsStationdatform = new List<YXbsStationdatform>();


                    YXbsMeastask task = new YXbsMeastask();
                    task.Format("*");
                    task.Filter = "(ID>0) AND (STATUS IS NOT NULL) AND (STATUS<>'Z')";
                    for (task.OpenRs(); !task.IsEOF(); task.MoveNext()) {
                        ICSM_T = new ClassTasks();
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


                        List<YXbsStationdatform> LYXbsStationdatform = new List<YXbsStationdatform>();
                        YXbsStationdatform XbsStationdatform_ = new YXbsStationdatform();
                        XbsStationdatform_.Format("*");
                        XbsStationdatform_.Filter = string.Format("(ID_XBS_MEASTASK={0})", task.m_id);
                        for (XbsStationdatform_.OpenRs(); !XbsStationdatform_.IsEOF(); XbsStationdatform_.MoveNext())
                        {
                            var m_fr = new YXbsStationdatform();
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

                        YXbsMeastimeparaml TimeParamList_ = new YXbsMeastimeparaml();
                        TimeParamList_.Format("*");
                        TimeParamList_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (TimeParamList_.OpenRs(); !TimeParamList_.IsEOF(); TimeParamList_.MoveNext())
                        {
                            var m = new YXbsMeastimeparaml();
                            m.CopyDataFrom(TimeParamList_);
                            ICSM_T.MeasTimeParamList = m;
                            m.Dispose();
                        }
                        TimeParamList_.Close();
                        TimeParamList_.Dispose();

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
                            MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND (STATUS<>'Z')", MeasSubTask_.m_id);
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
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                logger.Trace("Start procedure SaveStatusTaskToDB...");
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
                            MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND (STATUS<>'Z')", MeasSubTask_.m_id);
                            for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext())
                            {
                                MeasSubTaskStation M_STX = M_TS.MeasSubTaskStations.ToList().Find(t => t.Id == MeasSubTaskSt_.m_id);
                                if (M_STX != null)
                                {
                                        task.m_status = NewStatus;
                                        MeasSubTask_.m_status = NewStatus;
                                        MeasSubTaskSt_.m_status = NewStatus;
                                        MeasSubTaskSt_.Save(null,null);
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
                                MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND (STATUS<>'Z')", MeasSubTask_.m_id);
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
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                logger.Trace("Start procedure SaveIdsSdrTasks...");
                 YXbsMeastask task = new YXbsMeastask();
                task.Format("*");
                if (task.Fetch(obj.Id.Value)) {
                    task.m_id_start = ids;
                    task.Save(null,null);
                }
                task.Close();
                task.Dispose();
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
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                logger.Trace("Start procedure GetMaXIdsSdrTasks...");
                YXbsMeastask task = new YXbsMeastask();
                task.Format("*");
                task.Filter = "(ID>0)";
                task.Order = "[ID] DESC";
                for (task.OpenRs(); !task.IsEOF(); task.MoveNext()) {
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
                if (ids.Length>0) {
                    string[] words = ids.Split(new char[] { ';' });
                    if (words.Length>0) {
                        int val = -1;
                        if (int.TryParse(words[words.Length-1], out val)) {
                            MaxIDs = val;
                        }
                    }
                }
                if (MaxIDs == -1) MaxIDs = 0;
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
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                logger.Trace("Start procedure GetMaXIdsSdrResults...");
                YXbsMeasurementres res = new YXbsMeasurementres();
                res.Format("*");
                res.Filter = "(ID>0)";
                res.Order = "[ID] DESC";
                for (res.OpenRs(); !res.IsEOF(); res.MoveNext())
                {
                    if (res.m_id > 0)
                    {
                        MaxIDs = (int)res.m_id;
                        break;
                    }
                }
                if (MaxIDs == -1) MaxIDs = 0;
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
               System.Threading.Thread tsk = new System.Threading.Thread(() => {
               logger.Trace("Start procedure SetHistoryStatusTasksInDB...");
                YXbsMeastask task = new YXbsMeastask();
                task.Format("*");
                task.Filter = string.Format("(ID={0}) AND (STATUS IS NOT NULL)",obj.Id.Value);
                for (task.OpenRs(); !task.IsEOF(); task.MoveNext()) {
                   YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask();
                    MeasSubTask_.Format("*");
                    MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                    for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext()) {
                        MeasSubTask M_TS =  obj.MeasSubTasks.ToList().Find(t => t.Id.Value == MeasSubTask_.m_id);
                        if (M_TS != null)
                           {
                            YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta();
                            MeasSubTaskSt_.Format("*");
                            MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND (STATUS<>'Z')", MeasSubTask_.m_id);
                            for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext()) {
                                MeasSubTaskStation M_STX = M_TS.MeasSubTaskStations.ToList().Find(t => t.Id == MeasSubTaskSt_.m_id);
                                if (M_STX != null) {
                                    if (M_STX.TimeNextTask.GetValueOrDefault().Subtract(DateTime.Now).TotalSeconds < 0) {
                                        MeasSubTaskSt_.m_status = status;
                                        MeasSubTaskSt_.Save(null,null);
                                    }
                                }
                            }
                            MeasSubTaskSt_.Close();
                            MeasSubTaskSt_.Dispose();
                        }
                           MeasSubTask_.m_status = status;
                           MeasSubTask_.Save(null,null);
                       }
                    MeasSubTask_.Close();
                    MeasSubTask_.Dispose();

                    task.m_status = status;
                    task.Save(null,null);
                }
                task.Close();
                task.Dispose();
                logger.Trace("End procedure SetHistoryStatusTasksInDB.");
               });
                tsk.Start();
                tsk.Join();
            }
            catch (Exception ex)
            {
                logger.Error("Error in SetHistoryStatusTasksInDB: " + ex.Message);
            }
            return isSuccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int SaveTaskToDB(MeasTask obj)
        {
            int ID = Constants.NullI;
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
                                    if (obj.DateCreated != null) meastask.m_datecreated = obj.DateCreated.GetValueOrDefault();
                                    if (obj.OrderId != null) meastask.m_orderid = obj.OrderId.GetValueOrDefault();
                                    ID = (int)meastask.Save(dbConnect, transaction);
                                    obj.Id.Value = ID;
                                }
                                else
                                {
                                    ID = (int)meastask.m_id;
                                    obj.Id.Value = ID;
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
                                                    int ID_loc_params = (int)prm_loc.Save(dbConnect, transaction);
                                                    loc_param.Id.Value = ID_loc_params;

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

                                if (obj.MeasTimeParamList != null)
                                {
                                    List<Yyy> BlockInsert_YXbsMeastimeparaml = new List<Yyy>();
                                    foreach (MeasTimeParamList time_par in new List<MeasTimeParamList> { obj.MeasTimeParamList })
                                    {
                                        if (time_par != null)
                                        {
                                            YXbsMeastimeparaml prm_lst = new YXbsMeastimeparaml();
                                            prm_lst.Format("*");
                                            if (!prm_lst.Fetch(string.Format("ID_XBS_MEASTASK={0}", ID)))
                                            {
                                                prm_lst.New();
                                                prm_lst.m_days = time_par.Days;
                                                prm_lst.m_id_xbs_meastask = ID;
                                                prm_lst.m_perstart = time_par.PerStart;
                                                prm_lst.m_perstop = time_par.PerStop;
                                                if (time_par.TimeStart != null) prm_lst.m_timestart = time_par.TimeStart.GetValueOrDefault();
                                                if (time_par.TimeStop != null) prm_lst.m_timestop = time_par.TimeStop.GetValueOrDefault();
                                                if (time_par.PerInterval != null) prm_lst.m_perinterval = time_par.PerInterval.GetValueOrDefault();
                                                for (int i = 0; i < prm_lst.getAllFields.Count; i++)
                                                    prm_lst.getAllFields[i].Value = prm_lst.valc[i];
                                                BlockInsert_YXbsMeastimeparaml.Add(prm_lst);
                                            }
                                            prm_lst.Close();
                                            prm_lst.Dispose();
                                        }
                                    }
                                    if (BlockInsert_YXbsMeastimeparaml.Count > 0)
                                    {
                                        YXbsMeastimeparaml YXbsMeastimeparaml11 = new YXbsMeastimeparaml();
                                        YXbsMeastimeparaml11.Format("*");
                                        YXbsMeastimeparaml11.New();
                                        YXbsMeastimeparaml11.SaveBath(BlockInsert_YXbsMeastimeparaml, dbConnect, transaction);
                                        YXbsMeastimeparaml11.Close();
                                        YXbsMeastimeparaml11.Dispose();
                                    }
                                }

                                if (obj.MeasSubTasks != null)
                                {
                                    foreach (MeasSubTask sub_task in obj.MeasSubTasks.ToArray())
                                    {
                                        int ID_sub_task = Constants.NullI;
                                        if (sub_task.Id != null)
                                        {
                                            if (sub_task.Id.Value != Constants.NullI)
                                            {
                                                YXbsMeassubtask prm_sub_task = new YXbsMeassubtask();
                                                prm_sub_task.Format("*");
                                                if (!prm_sub_task.Fetch(sub_task.Id.Value))
                                                {
                                                    prm_sub_task.New();
                                                    sub_task.Id.Value = ID_sub_task;
                                                    prm_sub_task.m_id_xbs_meastask = ID;
                                                    if (sub_task.Interval != null) prm_sub_task.m_interval = sub_task.Interval.GetValueOrDefault();
                                                    prm_sub_task.m_status = sub_task.Status;
                                                    prm_sub_task.m_timestart = sub_task.TimeStart;
                                                    prm_sub_task.m_timestop = sub_task.TimeStop;
                                                    ID_sub_task = (int)prm_sub_task.Save(dbConnect, transaction);
                                                    sub_task.Id.Value = ID_sub_task;
                                                }
                                                else
                                                {
                                                    ID_sub_task = (int)prm_sub_task.m_id;
                                                    sub_task.Id.Value = ID_sub_task;
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
                                                        sub_task_st.Id = (int)prm_sub_task_st.Save(dbConnect, transaction);
                                                    }
                                                    else
                                                    {
                                                        sub_task_st.Id = (int)prm_sub_task_st.m_id;
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
                                        int ID_MeasFreqParam = Constants.NullI;
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
                                                ID_MeasFreqParam = (int)prm_MeasFreqParam.Save(dbConnect, transaction);
                                            }
                                            else
                                            {
                                                ID_MeasFreqParam = (int)prm_MeasFreqParam.m_id;
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
                                        int ID_loc_params = -1;
                                        YXbsStationdatform prm_loc = new YXbsStationdatform();
                                        prm_loc.Format("*");

                                        prm_loc.New();
                                        if (StationData_param.GlobalSID != null) prm_loc.m_globalsid = StationData_param.GlobalSID;
                                        if (StationData_param.Standart != null) prm_loc.m_standart = StationData_param.Standart;
                                        if (StationData_param.Status != null) prm_loc.m_status = StationData_param.Status;
                                        prm_loc.m_id_xbs_meastask = ID;
                                        ID_loc_params = (int)prm_loc.Save(dbConnect, transaction);
                                        StationData_param.IdStation = ID_loc_params;
                                        prm_loc.Close();
                                        prm_loc.Dispose();


                                        if (StationData_param.Site != null)
                                        {
                                            YXbsSitestformeas site_formeas = new YXbsSitestformeas();
                                            site_formeas.Format("*");
                                            site_formeas.New();
                                            if (StationData_param.Site.Lat != null) site_formeas.m_lat = StationData_param.Site.Lat.GetValueOrDefault();
                                            if (StationData_param.Site.Lon != null) site_formeas.m_lon = StationData_param.Site.Lon.GetValueOrDefault();
                                            if (StationData_param.Site.Region != null) site_formeas.m_region = StationData_param.Site.Region;
                                            if (StationData_param.Site.Adress != null) site_formeas.m_addres = StationData_param.Site.Adress;

                                            int ID_site_formeas = (int)site_formeas.Save(dbConnect, transaction);
                                            site_formeas.m_id_stationdatform = ID_loc_params;
                                            site_formeas.Close();
                                            site_formeas.Dispose();
                                        }

                                        if (StationData_param.LicenseParameter != null)
                                        {
                                            YXbsPermassign perm_formeas = new YXbsPermassign();
                                            perm_formeas.Format("*");
                                            perm_formeas.New();
                                            if (StationData_param.LicenseParameter.CloseDate != null) perm_formeas.m_closedate = StationData_param.LicenseParameter.CloseDate.GetValueOrDefault();
                                            if (StationData_param.LicenseParameter.DozvilName != null) perm_formeas.m_dozvilname = StationData_param.LicenseParameter.DozvilName;
                                            if (StationData_param.LicenseParameter.EndDate != null) perm_formeas.m_enddate = StationData_param.LicenseParameter.EndDate.GetValueOrDefault();
                                            if (StationData_param.LicenseParameter.StartDate != null) perm_formeas.m_startdate = StationData_param.LicenseParameter.StartDate.GetValueOrDefault();
                                            int ID_perm_formeas = (int)perm_formeas.Save(dbConnect, transaction);
                                            perm_formeas.m_id_stationdatform = ID_loc_params;
                                            perm_formeas.Close();
                                            perm_formeas.Dispose();
                                        }

                                        if (StationData_param.Owner != null)
                                        {
                                            YXbsOwnerdata owner_formeas = new YXbsOwnerdata();
                                            owner_formeas.Format("*");
                                            owner_formeas.New();
                                            if (StationData_param.Owner.Addres != null) owner_formeas.m_addres = StationData_param.Owner.Addres;
                                            if (StationData_param.Owner.Code != null) owner_formeas.m_code = StationData_param.Owner.Code;
                                            if (StationData_param.Owner.OKPO != null) owner_formeas.m_okpo = StationData_param.Owner.OKPO;
                                            if (StationData_param.Owner.OwnerName != null) owner_formeas.m_ownername = StationData_param.Owner.OwnerName;
                                            if (StationData_param.Owner.Zip != null) owner_formeas.m_zip = StationData_param.Owner.Zip;
                                            int ID_perm_formeas = (int)owner_formeas.Save(dbConnect, transaction);
                                            owner_formeas.m_id_stationdatform = ID_perm_formeas;
                                            owner_formeas.Close();
                                            owner_formeas.Dispose();
                                        }


                                        if (StationData_param.Sectors != null)
                                        {
                                            foreach (SectorStationForMeas sec in StationData_param.Sectors.ToArray())
                                            {
                                                YXbsSectstformeas sect_formeas = new YXbsSectstformeas();
                                                sect_formeas.Format("*");
                                                sect_formeas.New();
                                                if (sec.AGL != null) sect_formeas.m_agl = sec.AGL.GetValueOrDefault();
                                                if (sec.Azimut != null) sect_formeas.m_azimut = sec.Azimut.GetValueOrDefault();
                                                if (sec.BW != null) sect_formeas.m_bw = sec.BW.GetValueOrDefault();
                                                if (sec.ClassEmission != null) sect_formeas.m_classemission = sec.ClassEmission;
                                                if (sec.EIRP != null) sect_formeas.m_eirp = sec.EIRP.GetValueOrDefault();
                                                sect_formeas.m_id_stationdatform = ID_loc_params;
                                                int ID_secformeas = (int)sect_formeas.Save(dbConnect, transaction);
                                                sect_formeas.Close();
                                                sect_formeas.Dispose();

                                                if (sec.Frequencies != null)
                                                {
                                                    List<Yyy> BlockInsert = new List<Yyy>();
                                                    foreach (FrequencyForSectorFormICSM F in sec.Frequencies)
                                                    {
                                                        YXbsFreqforsectics freq_formeas = new YXbsFreqforsectics();
                                                        freq_formeas.Format("*");
                                                        freq_formeas.New();

                                                        if (F.ChannalNumber != null) freq_formeas.m_channalnumber = F.ChannalNumber.GetValueOrDefault();
                                                        if (F.Frequency != null) freq_formeas.m_frequency = (double)F.Frequency;
                                                        if (F.IdPlan != null) freq_formeas.m_idplan = F.IdPlan.GetValueOrDefault();
                                                        freq_formeas.m_id_sectstformeas = ID_secformeas;
                                                        for (int i = 0; i < freq_formeas.getAllFields.Count; i++)
                                                            freq_formeas.getAllFields[i].Value = freq_formeas.valc[i];
                                                        BlockInsert.Add(freq_formeas);
                                                        freq_formeas.Close();
                                                        freq_formeas.Dispose();
                                                    }
                                                    if (BlockInsert.Count > 0)
                                                    {
                                                        YXbsFreqforsectics freq_formeas11 = new YXbsFreqforsectics();
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
                                                        YXbsMaskelements mask_formeas = new YXbsMaskelements();
                                                        mask_formeas.Format("*");
                                                        mask_formeas.New();
                                                        if (F.BW != null) mask_formeas.m_bw = F.BW.GetValueOrDefault();
                                                        if (F.level != null) mask_formeas.m_level = F.level.GetValueOrDefault();
                                                        mask_formeas.m_id_sectstformeas = ID_secformeas;
                                                        for (int i = 0; i < mask_formeas.getAllFields.Count; i++)
                                                            mask_formeas.getAllFields[i].Value = mask_formeas.valc[i];
                                                        BlockInsert.Add(mask_formeas);
                                                        mask_formeas.Close();
                                                        mask_formeas.Dispose();
                                                    }
                                                    if (BlockInsert.Count > 0)
                                                    {
                                                        YXbsMaskelements freq_formeas11 = new YXbsMaskelements();
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
