using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using OrmCs;
using DatalayerCs;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Globalization;
using CoreICSM.Logs;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    /// <summary>
    /// Класс, извлекающий данные
    /// из поля ORDER_DATA таблицы SYS_ARGUS_ORM
    /// с целью заполнения структуры MEAS_TASK
    /// </summary>
    public class ClassesDBGetTasks : IDisposable
    {
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
                //Task tsk = new Task(() => {
                // сканирование по объектам БД
                CLASS_TASKS ICSM_T = new CLASS_TASKS();
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
                YXbsMeastask task = new YXbsMeastask(ConnectDB.Connect_Main_);
                task.Format("*");
                // выбирать только таски, для которых STATUS не NULL
                task.Filter = "(ID>0) AND (STATUS IS NOT NULL) AND (STATUS<>'Z')";
                for (task.OpenRs(); !task.IsEOF(); task.MoveNext()) {
                    ICSM_T = new CLASS_TASKS();
                    ////////
                   YXbsMeastask m_meas_task = new YXbsMeastask(ConnectDB.Connect_Main_);
                    m_meas_task.CopyDataFrom(task);
                    ICSM_T.meas_task = m_meas_task;
                    m_meas_task.Close();
                    m_meas_task.Dispose();

                    List<YXbsMeasstation> LXbsMeasstation = new List<YXbsMeasstation>();
                    YXbsMeasstation XbsMeasstation_ = new YXbsMeasstation(ConnectDB.Connect_Main_);
                    XbsMeasstation_.Format("*");
                    XbsMeasstation_.Filter = string.Format("(ID_XBS_MEASTASK={0})", task.m_id);
                    for (XbsMeasstation_.OpenRs(); !XbsMeasstation_.IsEOF(); XbsMeasstation_.MoveNext())
                    {
                        var m_fr = new YXbsMeasstation(ConnectDB.Connect_Main_);
                        m_fr.CopyDataFrom(XbsMeasstation_);
                        LXbsMeasstation.Add(m_fr);
                        m_fr.Dispose();
                    }
                    XbsMeasstation_.Close();
                    XbsMeasstation_.Dispose();
                    ICSM_T.Stations = LXbsMeasstation;

                    ///
                    /*
                    List<YXbsStationdatform> LYXbsStationdatform = new List<YXbsStationdatform>();
                    YXbsStationdatform XbsStationdatform_ = new YXbsStationdatform(ConnectDB.Connect_Main_);
                    XbsStationdatform_.Format("*");
                    XbsStationdatform_.Filter = string.Format("(ID_XBS_MEASTASK={0})", task.m_id);
                    for (XbsStationdatform_.OpenRs(); !XbsStationdatform_.IsEOF(); XbsStationdatform_.MoveNext())
                    {
                        var m_fr = new YXbsStationdatform(ConnectDB.Connect_Main_);
                        m_fr.CopyDataFrom(XbsStationdatform_);
                        LYXbsStationdatform.Add(m_fr);
                        m_fr.Dispose();
                    }
                    XbsStationdatform_.Close();
                    XbsStationdatform_.Dispose();
                    ICSM_T.XbsStationdatform = LYXbsStationdatform;
                    */


                    YXbsMeasdtparam MeasDtParam_ = new YXbsMeasdtparam(ConnectDB.Connect_Main_);
                    MeasDtParam_.Format("*");
                    MeasDtParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                    for (MeasDtParam_.OpenRs(); !MeasDtParam_.IsEOF(); MeasDtParam_.MoveNext())  {
                        var m = new YXbsMeasdtparam(ConnectDB.Connect_Main_);
                        m.CopyDataFrom(MeasDtParam_);
                        ICSM_T.MeasDtParam.Add(m);
                        m.Dispose();
                    }
                    MeasDtParam_.Close();
                    MeasDtParam_.Dispose();

                    YXbsMeastimeparaml TimeParamList_ = new YXbsMeastimeparaml(ConnectDB.Connect_Main_);
                    TimeParamList_.Format("*");
                    TimeParamList_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                    for (TimeParamList_.OpenRs(); !TimeParamList_.IsEOF(); TimeParamList_.MoveNext())
                    {
                        var m = new YXbsMeastimeparaml(ConnectDB.Connect_Main_);
                        m.CopyDataFrom(TimeParamList_);
                        ICSM_T.MeasTimeParamList = m;
                        m.Dispose();
                    }
                    TimeParamList_.Close();
                    TimeParamList_.Dispose();

                   YXbsMeaslocparam MeasLocParam_ = new YXbsMeaslocparam(ConnectDB.Connect_Main_);
                    MeasLocParam_.Format("*");
                    MeasLocParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                    for (MeasLocParam_.OpenRs(); !MeasLocParam_.IsEOF(); MeasLocParam_.MoveNext()){
                        var m = new YXbsMeaslocparam(ConnectDB.Connect_Main_);
                        m.CopyDataFrom(MeasLocParam_);
                        ICSM_T.MeasLocParam.Add(m);
                        m.Dispose();
                    }
                    MeasLocParam_.Close();
                    MeasLocParam_.Dispose();

                   YXbsMeasother MeasOther_ = new YXbsMeasother(ConnectDB.Connect_Main_);
                    MeasOther_.Format("*");
                    MeasOther_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                    for (MeasOther_.OpenRs(); !MeasOther_.IsEOF(); MeasOther_.MoveNext()) {
                        var m = new YXbsMeasother(ConnectDB.Connect_Main_);
                        m.CopyDataFrom(MeasOther_);
                        ICSM_T.MeasOther = m;
                        m.Dispose();
                    }
                    MeasOther_.Close();
                    MeasOther_.Dispose();

                   YXbsMeasfreqparam MeasFreqParam_ = new YXbsMeasfreqparam(ConnectDB.Connect_Main_);
                    MeasFreqParam_.Format("*");
                    MeasFreqParam_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                    for (MeasFreqParam_.OpenRs(); !MeasFreqParam_.IsEOF(); MeasFreqParam_.MoveNext())  {
                        var m = new YXbsMeasfreqparam(ConnectDB.Connect_Main_);
                        m.CopyDataFrom(MeasFreqParam_);
                        List<YXbsMeasfreq> FreqL = new List<YXbsMeasfreq>();
                        YXbsMeasfreq MeasFreqLst_ = new YXbsMeasfreq(ConnectDB.Connect_Main_);
                        MeasFreqLst_.Format("*");
                        MeasFreqLst_.Filter = string.Format("ID_XBS_MeasFreqParam={0}", MeasFreqParam_.m_id);
                        for (MeasFreqLst_.OpenRs(); !MeasFreqLst_.IsEOF(); MeasFreqLst_.MoveNext()) {
                            var m_fr = new YXbsMeasfreq(ConnectDB.Connect_Main_);
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

                   YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask(ConnectDB.Connect_Main_);
                    MeasSubTask_.Format("*");
                    MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                    for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext()) {
                        var m = new YXbsMeassubtask(ConnectDB.Connect_Main_);
                        m.CopyDataFrom(MeasSubTask_);
                        List<YXbsMeassubtasksta> SubTaskStL = new List<YXbsMeassubtasksta>();
                        YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta(ConnectDB.Connect_Main_);
                        MeasSubTaskSt_.Format("*");
                        MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND (STATUS<>'Z')", MeasSubTask_.m_id);
                        for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext()) {
                            var m_fr = new YXbsMeassubtasksta(ConnectDB.Connect_Main_);
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
               // });
                //tsk.Start();
                //tsk.Wait();
                #endregion
            }
            catch (Exception ex) {
                CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[ReadlAllSTasksFromDB]:"+ ex.Message);
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
                //Task tsk = new Task(() => {
                //подключение к БД
                YXbsMeastask task = new YXbsMeastask(ConnectDB.Connect_Main_);
                task.Format("*");
                // выбирать только сенсоры, для которых STATUS не NULL
                task.Filter = string.Format("(ID={0})", obj.Id.Value);
                for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                {
                    YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask(ConnectDB.Connect_Main_);
                    MeasSubTask_.Format("*");
                    MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                    for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext())
                    {
                        MeasSubTask M_TS = obj.MeasSubTasks.ToList().Find(t => t.Id.Value == MeasSubTask_.m_id);
                        if (M_TS != null)
                        {
                            YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta(ConnectDB.Connect_Main_);
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
                                        MeasSubTaskSt_.Save();
                                        MeasSubTask_.Save();
                                        task.Save();
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
                //});
                //tsk.Start();
                //tsk.Wait();
            }
            catch (Exception ex)
            {
                CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SaveStatusTaskToDB]:" + ex.Message);
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
                //Task tsk = new Task(() =>
                //{
                    //подключение к БД
                    YXbsMeastask task = new YXbsMeastask(ConnectDB.Connect_Main_);
                    task.Format("*");
                    // выбирать только сенсоры, для которых STATUS не NULL
                    task.Filter = string.Format("(ID={0})", obj.Id.Value);
                    for (task.OpenRs(); !task.IsEOF(); task.MoveNext())
                    {
                        YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask(ConnectDB.Connect_Main_);
                        MeasSubTask_.Format("*");
                        MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                        for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext())
                        {
                            MeasSubTask M_TS = obj.MeasSubTasks.ToList().Find(t => t.Id.Value == MeasSubTask_.m_id);
                            if (M_TS != null)
                            {
                                YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta(ConnectDB.Connect_Main_);
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
                                        MeasSubTaskSt_.Save();
                                        MeasSubTask_.Save();
                                        task.Save();
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
                //});
                //tsk.Start();
                //tsk.Wait();
            }
            catch (Exception ex)
            {
                CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SaveStatusTaskToDB]:" + ex.Message);
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
                //Task tsk = new Task(() => { 
                //подключение к БД
                YXbsMeastask task = new YXbsMeastask(ConnectDB.Connect_Main_);
                task.Format("*");
                if (task.Fetch(obj.Id.Value)) {
                    task.m_id_start = ids;
                    task.Save();
                    CoreICSM.Logs.CLogs.WriteInfo(ELogsWhat.Unknown, "SaveIdsSdrTasks saved..." );
                }
                task.Close();
                task.Dispose();
            //});
            //tsk.Start();
            //tsk.Wait();
        }
            catch (Exception ex)
            {
                CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SaveIdsSdrTasks]:" + ex.Message);
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
                //Task tsk = new Task(() => {
                YXbsMeastask task = new YXbsMeastask(ConnectDB.Connect_Main_);
                task.Format("*");
                task.Filter = "(ID>0)";
                task.Order = "[ID] DESC";
                for (task.OpenRs(); !task.IsEOF(); task.MoveNext()) {
                    if (task.m_id_start.Length > 0) {
                        ids = task.m_id_start;
                        break;
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
                ///});
                //tsk.Start();
                //tsk.Wait();
            }
            catch (Exception ex)
            {
                if (MaxIDs == -1) MaxIDs = 0;
                CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SaveIdsSdrTasks]:" + ex.Message);
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
                string ids = "";
                //Task tsk = new Task(() => {
                YXbsMeasurementres res = new YXbsMeasurementres(ConnectDB.Connect_Main_);
                res.Format("*");
                res.Filter = "(ID>0)";
                res.Order = "[ID] DESC";
                for (res.OpenRs(); !res.IsEOF(); res.MoveNext())
                {
                    if (res.m_id > 0)
                    {
                        MaxIDs = res.m_id;
                        break;
                    }
                }
                if (MaxIDs == -1) MaxIDs = 0;
                //});
                //tsk.Start();
                //tsk.Wait();
            }
            catch (Exception ex)
            {
                CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SaveIdsSdrTasks]:" + ex.Message);
            }
            return MaxIDs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SetHistoryStatusTasksInDB(MeasTask obj)
        {
            bool isSuccess = true;
            try
            {
                //Task tsk = new Task(() => {
                //подключение к БД 
                YXbsMeastask task = new YXbsMeastask(ConnectDB.Connect_Main_);
                task.Format("*");
                // выбирать только сенсоры, для которых STATUS не NULL
                task.Filter = string.Format("(ID={0}) AND (STATUS IS NOT NULL)",obj.Id.Value);
                for (task.OpenRs(); !task.IsEOF(); task.MoveNext()) {
                   YXbsMeassubtask MeasSubTask_ = new YXbsMeassubtask(ConnectDB.Connect_Main_);
                    MeasSubTask_.Format("*");
                    MeasSubTask_.Filter = string.Format("ID_XBS_MEASTASK={0}", task.m_id);
                    for (MeasSubTask_.OpenRs(); !MeasSubTask_.IsEOF(); MeasSubTask_.MoveNext()) {
                        MeasSubTask M_TS =  obj.MeasSubTasks.ToList().Find(t => t.Id.Value == MeasSubTask_.m_id);
                        if (M_TS != null) {
                            YXbsMeassubtasksta MeasSubTaskSt_ = new YXbsMeassubtasksta(ConnectDB.Connect_Main_);
                            MeasSubTaskSt_.Format("*");
                            MeasSubTaskSt_.Filter = string.Format("(ID_XB_MEASSUBTASK={0}) AND (STATUS<>'Z')", MeasSubTask_.m_id);
                            for (MeasSubTaskSt_.OpenRs(); !MeasSubTaskSt_.IsEOF(); MeasSubTaskSt_.MoveNext()) {
                                MeasSubTaskStation M_STX = M_TS.MeasSubTaskStations.ToList().Find(t => t.Id == MeasSubTaskSt_.m_id);
                                if (M_STX != null) {
                                    if (M_STX.TimeNextTask.GetValueOrDefault().Subtract(DateTime.Now).TotalSeconds < 0) {
                                        MeasSubTaskSt_.m_status = "Z";
                                        MeasSubTaskSt_.Save();
                                    }
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
                //});
                //tsk.Start();
                //tsk.Wait();
            }
            catch (Exception ex)
            {
                CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SetHistoryStatusTasksInDB]:" + ex.Message);
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
            int ID = ConnectDB.NullI;
            try
            {
                #region Save Task
                Task tsk = new Task(() =>
                {
                    /// Create new record in YXbsMeastask
                    if (obj.Id != null)
                    {
                        if (obj.Id.Value != ConnectDB.NullI)
                        {
                            YXbsMeastask meastask = new YXbsMeastask(ConnectDB.Connect_Main_);
                            meastask.Format("*");
                            if ((!meastask.Fetch(obj.Id.Value)) && (!meastask.Fetch(string.Format(" (ID={0}) ", obj.Id.Value))))
                            {
                                meastask.New();
                                ID = meastask.AllocID();
                                obj.Id.Value = ID;
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
                                meastask.Save();
                            }
                            else {
                                ID = meastask.m_id;
                                obj.Id.Value = ID;
                            }
                            meastask.Close();
                            meastask.Dispose();
                        }
                        if (ID != ConnectDB.NullI)
                        {
                            //// create YXbMeasDtParam
                            if (obj.MeasDtParam != null)
                            {
                                foreach (MeasDtParam dt_param in new List<MeasDtParam> { obj.MeasDtParam })
                                {
                                    if (dt_param != null)
                                    {
                                        YXbsMeasdtparam dtr = new YXbsMeasdtparam(ConnectDB.Connect_Main_);
                                        dtr.Format("*");
                                        if (!dtr.Fetch(string.Format("ID_XBS_MEASTASK={0}", ID)))
                                        {
                                            dtr.New();
                                            int ID_DT_params = dtr.AllocID();
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
                                            dtr.Save();
                                        }
                                        dtr.Close();
                                        dtr.Dispose();
                                    }
                                }
                            }


                            //// create YXbsMeaslocparam
                            if (obj.MeasLocParams != null)
                            {
                                foreach (MeasLocParam loc_param in obj.MeasLocParams.ToArray())
                                {
                                    if (loc_param.Id != null)
                                    {
                                        if (loc_param.Id.Value != ConnectDB.NullI)
                                        {
                                            YXbsMeaslocparam prm_loc = new YXbsMeaslocparam(ConnectDB.Connect_Main_);
                                            prm_loc.Format("*");
                                            if (!prm_loc.Fetch(string.Format("(ID_XBS_MEASTASK={0}) AND (Lat={1}) AND (Lon={2})", ID, loc_param.Lat.GetValueOrDefault().ToString().Replace(",", "."), loc_param.Lon.GetValueOrDefault().ToString().Replace(",", "."))))
                                            {
                                                prm_loc.New();
                                                int ID_loc_params = prm_loc.AllocID();
                                                loc_param.Id.Value = ID_loc_params;
                                                if (loc_param.ASL != null) prm_loc.m_asl = loc_param.ASL.GetValueOrDefault();
                                                if (loc_param.Lat != null) prm_loc.m_lat = loc_param.Lat.GetValueOrDefault();
                                                if (loc_param.Lon != null) prm_loc.m_lon = loc_param.Lon.GetValueOrDefault();
                                                if (loc_param.MaxDist != null) prm_loc.m_maxdist = loc_param.MaxDist.GetValueOrDefault();
                                                prm_loc.m_id_xbs_meastask = ID;
                                                prm_loc.Save();
                                            }
                                            prm_loc.Close();
                                            prm_loc.Dispose();
                                        }
                                    }
                                }
                            }
                            /// create YXbsMeasother
                            if (obj.MeasOther != null)
                            {
                                foreach (MeasOther other in new List<MeasOther> { obj.MeasOther })
                                {
                                    if (other != null)
                                    {
                                        YXbsMeasother prm_oth = new YXbsMeasother(ConnectDB.Connect_Main_);
                                        prm_oth.Format("*");
                                        if (!prm_oth.Fetch(string.Format("ID_XBS_MEASTASK={0}", ID)))
                                        {
                                            prm_oth.New();
                                            int ID_loc_params = prm_oth.AllocID();
                                            prm_oth.m_id_xbs_meastask = ID;
                                            if (other.LevelMinOccup != null) prm_oth.m_levelminoccup = other.LevelMinOccup.GetValueOrDefault();
                                            if (other.NChenal != null) prm_oth.m_nchenal = other.NChenal.GetValueOrDefault();
                                            if (other.SwNumber != null) prm_oth.m_swnumber = other.SwNumber.GetValueOrDefault();
                                            prm_oth.m_typespectrumscan = other.TypeSpectrumScan.ToString();
                                            prm_oth.m_typespectrumoccupation = other.TypeSpectrumOccupation.ToString();
                                            prm_oth.Save();
                                        }
                                        prm_oth.Close();
                                        prm_oth.Dispose();
                                    }
                                }
                            }

                            ///// Create New YXbsMeastimeparaml
                            if (obj.MeasTimeParamList != null)
                            {
                                foreach (MeasTimeParamList time_par in new List<MeasTimeParamList> { obj.MeasTimeParamList })
                                {
                                    if (time_par != null)
                                    {
                                        YXbsMeastimeparaml prm_lst = new YXbsMeastimeparaml(ConnectDB.Connect_Main_);
                                        prm_lst.Format("*");
                                        if (!prm_lst.Fetch(string.Format("ID_XBS_MEASTASK={0}", ID)))
                                        {
                                            prm_lst.New();
                                            prm_lst.AllocID();
                                            prm_lst.m_days = time_par.Days;
                                            prm_lst.m_id_xbs_meastask = ID;
                                            prm_lst.m_perstart = time_par.PerStart;
                                            prm_lst.m_perstop = time_par.PerStop;
                                            if (time_par.TimeStart != null) prm_lst.m_timestart = time_par.TimeStart.GetValueOrDefault();
                                            if (time_par.TimeStop != null) prm_lst.m_timestop = time_par.TimeStop.GetValueOrDefault();
                                            if (time_par.PerInterval != null) prm_lst.m_perinterval = time_par.PerInterval.GetValueOrDefault();
                                            prm_lst.Save();
                                        }
                                        prm_lst.Close();
                                        prm_lst.Dispose();
                                    }
                                }
                            }
                            ///////// create new YXbMeasSubTask
                            if (obj.MeasSubTasks != null)
                            {
                                foreach (MeasSubTask sub_task in obj.MeasSubTasks.ToArray())
                                {
                                    int ID_sub_task = ConnectDB.NullI;
                                    if (sub_task.Id != null)
                                    {
                                        if (sub_task.Id.Value != ConnectDB.NullI)
                                        {
                                            YXbsMeassubtask prm_sub_task = new YXbsMeassubtask(ConnectDB.Connect_Main_);
                                            prm_sub_task.Format("*");
                                            if (!prm_sub_task.Fetch(sub_task.Id.Value))
                                            {
                                                prm_sub_task.New();
                                                ID_sub_task = prm_sub_task.AllocID();
                                                sub_task.Id.Value = ID_sub_task;
                                                prm_sub_task.m_id_xbs_meastask = ID;
                                                if (sub_task.Interval != null) prm_sub_task.m_interval = sub_task.Interval.GetValueOrDefault();
                                                prm_sub_task.m_status = sub_task.Status;
                                                prm_sub_task.m_timestart = sub_task.TimeStart;
                                                prm_sub_task.m_timestop = sub_task.TimeStop;
                                                prm_sub_task.Save();
                                            }
                                            else {
                                                ID_sub_task = prm_sub_task.m_id;
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
                                            if (sub_task_st.Id != ConnectDB.NullI)
                                            {
                                                YXbsMeassubtasksta prm_sub_task_st = new YXbsMeassubtasksta(ConnectDB.Connect_Main_);
                                                prm_sub_task_st.Format("*");
                                                if (!prm_sub_task_st.Fetch(sub_task_st.Id))
                                                {
                                                    prm_sub_task_st.New();
                                                    sub_task_st.Id = prm_sub_task_st.AllocID();
                                                    prm_sub_task_st.m_id_xb_meassubtask = ID_sub_task;
                                                    prm_sub_task_st.m_status = sub_task_st.Status;
                                                    if (sub_task_st.TimeNextTask != null) prm_sub_task_st.m_timenexttask = sub_task_st.TimeNextTask.GetValueOrDefault();
                                                    if (sub_task_st.Count != null) prm_sub_task_st.m_count = sub_task_st.Count.GetValueOrDefault();
                                                    prm_sub_task_st.m_id_xbs_sensor = sub_task_st.StationId.Value;
                                                    prm_sub_task_st.Save();
                                                }
                                                else {
                                                    sub_task_st.Id = prm_sub_task_st.m_id;
                                                }
                                                prm_sub_task_st.Close();
                                                prm_sub_task_st.Dispose();
                                            }
                                        }
                                    }

                                }
                            }

                            ////// Create  new YXbMeasFreqParam
                            if (obj.MeasFreqParam != null)
                            {

                                foreach (MeasFreqParam freq_param in new List<MeasFreqParam> { obj.MeasFreqParam })
                                {
                                    int ID_MeasFreqParam = ConnectDB.NullI;
                                    if (freq_param != null)
                                    {
                                        YXbsMeasfreqparam prm_MeasFreqParam = new YXbsMeasfreqparam(ConnectDB.Connect_Main_);
                                        prm_MeasFreqParam.Format("*");
                                        if (!prm_MeasFreqParam.Fetch(string.Format("ID_XBS_MEASTASK={0}", ID)))
                                        {
                                            prm_MeasFreqParam.New();
                                            ID_MeasFreqParam = prm_MeasFreqParam.AllocID();
                                            prm_MeasFreqParam.m_id_xbs_meastask = ID;
                                            prm_MeasFreqParam.m_mode = freq_param.Mode.ToString();
                                            if (freq_param.RgL != null) prm_MeasFreqParam.m_rgl = freq_param.RgL.GetValueOrDefault();
                                            if (freq_param.RgU != null) prm_MeasFreqParam.m_rgu = freq_param.RgU.GetValueOrDefault();
                                            if (freq_param.Step != null) prm_MeasFreqParam.m_step = freq_param.Step.GetValueOrDefault();
                                            prm_MeasFreqParam.Save();
                                        }
                                        else {
                                            ID_MeasFreqParam = prm_MeasFreqParam.m_id;
                                        }
                                        prm_MeasFreqParam.Close();
                                        prm_MeasFreqParam.Dispose();
                                    }

                                    if (freq_param.MeasFreqs != null)
                                    {
                                        foreach (MeasFreq sub_freq_st in freq_param.MeasFreqs.ToArray())
                                        {
                                            YXbsMeasfreq prm_sub_freq_st = new YXbsMeasfreq(ConnectDB.Connect_Main_);
                                            prm_sub_freq_st.Format("*");
                                            if (sub_freq_st != null)
                                            {
                                                if (!prm_sub_freq_st.Fetch(string.Format("(ID_XBS_MEASFREQPARAM={0}) AND (FREQ={1})", ID_MeasFreqParam, sub_freq_st.Freq.ToString().Replace(",", "."))))
                                                    prm_sub_freq_st.New();
                                                prm_sub_freq_st.AllocID();
                                                prm_sub_freq_st.m_id_xbs_measfreqparam = ID_MeasFreqParam;
                                                prm_sub_freq_st.m_freq = sub_freq_st.Freq;
                                                prm_sub_freq_st.Save();
                                            }
                                            else {
                                                prm_sub_freq_st.m_id_xbs_measfreqparam = ID_MeasFreqParam;
                                            }
                                            prm_sub_freq_st.Close();
                                            prm_sub_freq_st.Dispose();
                                        }
                                    }
                                }
                            }

                            if (obj.Stations != null)
                            {
                                foreach (MeasStation s_st in obj.Stations.ToArray())
                                {
                                    YXbsMeasstation prm_XbsMeasstation = new YXbsMeasstation(ConnectDB.Connect_Main_);
                                    prm_XbsMeasstation.Format("*");
                                    if (prm_XbsMeasstation != null)
                                    {
                                        if (!prm_XbsMeasstation.Fetch(string.Format("(StationId={0}) AND (ID_XBS_MeasTask={1})", s_st.StationId.Value, ID)))
                                            prm_XbsMeasstation.New();
                                        prm_XbsMeasstation.AllocID();
                                        prm_XbsMeasstation.m_id_xbs_meastask = ID;
                                        prm_XbsMeasstation.m_stationid = s_st.StationId.Value;
                                        prm_XbsMeasstation.m_stationtype = s_st.StationType;
                                        prm_XbsMeasstation.Save();
                                    }
                                    else {
                                        prm_XbsMeasstation.m_id_xbs_meastask = ID;
                                    }
                                    prm_XbsMeasstation.Close();
                                    prm_XbsMeasstation.Dispose();
                                }
                            }

                            ///
                            /*
                            if (obj.StationsForMeasurements != null)
                            {
                                foreach (StationDataForMeasurements StationData_param in obj.StationsForMeasurements.ToArray())
                                {
                                    int ID_loc_params = -1;

                                    YXbsStationdatform prm_loc = new YXbsStationdatform(ConnectDB.Connect_Main_);
                                    prm_loc.Format("*");

                                                prm_loc.New();
                                                ID_loc_params = prm_loc.AllocID();
                                                StationData_param.IdStation = ID_loc_params;
                                                if (StationData_param.GlobalSID != null) prm_loc.m_globalsid = StationData_param.GlobalSID;
                                                if (StationData_param.Standart != null) prm_loc.m_standart = StationData_param.Standart;
                                                if (StationData_param.Status != null) prm_loc.m_status = StationData_param.Status;
                                                prm_loc.m_id_xbs_meastask = ID;
                                                prm_loc.Save();



                                    prm_loc.Close();
                                    prm_loc.Dispose();


                                    if (StationData_param.Site != null)
                                    {
                                        YXbsSitestformeas site_formeas = new YXbsSitestformeas(ConnectDB.Connect_Main_);
                                        site_formeas.Format("*");
                                        site_formeas.New();
                                        int ID_site_formeas = site_formeas.AllocID();
                                        site_formeas.m_id_stationdatform = ID_loc_params;
                                        if (StationData_param.Site.Lat != null) site_formeas.m_lat = StationData_param.Site.Lat.GetValueOrDefault();
                                        if (StationData_param.Site.Lon != null) site_formeas.m_lon = StationData_param.Site.Lon.GetValueOrDefault();
                                        if (StationData_param.Site.Region != null) site_formeas.m_region = StationData_param.Site.Region;
                                        if (StationData_param.Site.Adress != null) site_formeas.m_addres = StationData_param.Site.Adress;

                                        site_formeas.Save();
                                        site_formeas.Close();
                                        site_formeas.Dispose();
                                    }

                                    if (StationData_param.LicenseParameter != null)
                                    {
                                        YXbsPermassign perm_formeas = new YXbsPermassign(ConnectDB.Connect_Main_);
                                        perm_formeas.Format("*");
                                        perm_formeas.New();
                                        int ID_perm_formeas = perm_formeas.AllocID();
                                        perm_formeas.m_id_stationdatform = ID_loc_params;
                                        if (StationData_param.LicenseParameter.CloseDate != null) perm_formeas.m_closedate = StationData_param.LicenseParameter.CloseDate.GetValueOrDefault();
                                        if (StationData_param.LicenseParameter.DozvilName != null) perm_formeas.m_dozvilname = StationData_param.LicenseParameter.DozvilName;
                                        if (StationData_param.LicenseParameter.EndDate != null) perm_formeas.m_enddate = StationData_param.LicenseParameter.EndDate.GetValueOrDefault();
                                        if (StationData_param.LicenseParameter.StartDate != null) perm_formeas.m_startdate = StationData_param.LicenseParameter.StartDate.GetValueOrDefault();
                                        perm_formeas.Save();
                                        perm_formeas.Close();
                                        perm_formeas.Dispose();
                                    }

                                    if (StationData_param.Owner != null)
                                    {
                                        YXbsOwnerdata owner_formeas = new YXbsOwnerdata(ConnectDB.Connect_Main_);
                                        owner_formeas.Format("*");
                                        owner_formeas.New();
                                        int ID_perm_formeas = owner_formeas.AllocID();
                                        owner_formeas.m_id_stationdatform = ID_loc_params;
                                        if (StationData_param.Owner.Addres != null) owner_formeas.m_addres = StationData_param.Owner.Addres;
                                        if (StationData_param.Owner.Code != null) owner_formeas.m_code = StationData_param.Owner.Code;
                                        if (StationData_param.Owner.OKPO != null) owner_formeas.m_okpo = StationData_param.Owner.OKPO;
                                        if (StationData_param.Owner.OwnerName != null) owner_formeas.m_ownername = StationData_param.Owner.OwnerName;
                                        if (StationData_param.Owner.Zip != null) owner_formeas.m_zip = StationData_param.Owner.Zip;
                                        owner_formeas.Save();
                                        owner_formeas.Close();
                                        owner_formeas.Dispose();
                                    }


                                    if (StationData_param.Sectors != null)
                                    {
                                        foreach (SectorStationForMeas sec in StationData_param.Sectors.ToArray())
                                        {
                                            YXbsSectstformeas sect_formeas = new YXbsSectstformeas(ConnectDB.Connect_Main_);
                                            sect_formeas.Format("*");
                                            sect_formeas.New();
                                            int ID_secformeas = sect_formeas.AllocID();
                                            if (sec.AGL != null) sect_formeas.m_agl = sec.AGL.GetValueOrDefault();
                                            if (sec.Azimut != null) sect_formeas.m_azimut = sec.Azimut.GetValueOrDefault();
                                            if (sec.BW != null) sect_formeas.m_bw = sec.BW.GetValueOrDefault();
                                            if (sec.ClassEmission != null) sect_formeas.m_classemission = sec.ClassEmission;
                                            if (sec.EIRP != null) sect_formeas.m_eirp = sec.EIRP.GetValueOrDefault();
                                            sect_formeas.m_id_stationdatform = ID_loc_params;
                                            sect_formeas.Save();
                                            sect_formeas.Close();
                                            sect_formeas.Dispose();

                                            if (sec.Frequencies != null)
                                            {
                                                foreach (FrequencyForSectorFormICSM F in sec.Frequencies)
                                                {
                                                    YXbsFreqforsectics freq_formeas = new YXbsFreqforsectics(ConnectDB.Connect_Main_);
                                                    freq_formeas.Format("*");
                                                    freq_formeas.New();
                                                    int ID_freq_formeas = freq_formeas.AllocID();
                                                    if (F.ChannalNumber != null) freq_formeas.m_channalnumber = F.ChannalNumber.GetValueOrDefault();
                                                    if (F.Frequency != null) freq_formeas.m_frequency = (double)F.Frequency;
                                                    if (F.IdPlan != null) freq_formeas.m_idplan = F.IdPlan.GetValueOrDefault();
                                                    freq_formeas.m_id_sectstformeas = ID_secformeas;
                                                    freq_formeas.Save();
                                                    freq_formeas.Close();
                                                    freq_formeas.Dispose();
                                                }
                                            }

                                            if (sec.MaskBW != null)
                                            {
                                                foreach (MaskElements F in sec.MaskBW)
                                                {
                                                   YXbsMaskelements mask_formeas = new YXbsMaskelements(ConnectDB.Connect_Main_);
                                                    mask_formeas.Format("*");
                                                    mask_formeas.New();
                                                    int ID_freq_formeas = mask_formeas.AllocID();
                                                    if (F.BW != null) mask_formeas.m_bw = F.BW.GetValueOrDefault();
                                                    if (F.level != null) mask_formeas.m_level = F.level.GetValueOrDefault();
                                                    mask_formeas.m_id_sectstformeas = ID_secformeas;
                                                    mask_formeas.Save();
                                                    mask_formeas.Close();
                                                    mask_formeas.Dispose();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            */
                            ///
                        }
                    }
                    #endregion
                });
                tsk.Start();
                tsk.Wait();
            }
            catch (Exception ex)
            {
                ID = ConnectDB.NullI;
                CoreICSM.Logs.CLogs.WriteError(ELogsWhat.Unknown, "[SaveTaskToDB]:" + ex.Message);
            }
            return ID;
        }



    }
}
