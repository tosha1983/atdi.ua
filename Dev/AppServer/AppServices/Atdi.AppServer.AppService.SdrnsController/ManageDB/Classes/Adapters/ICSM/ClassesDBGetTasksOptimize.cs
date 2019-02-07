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
    public class ClassesDBGetTasksOptimize : IDisposable
    {
        public static ILogger logger;
        public ClassesDBGetTasksOptimize(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassesDBGetTasksOptimize()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private List<CLASS_TASKS> L_IN { get; set; }


        public List<CLASS_TASKS> ReadTaskHeader(int MeasTaskId)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            L_IN = new List<CLASS_TASKS>();
            try
            {
                #region Load Tasks from DB
                //System.Threading.Thread tsk = new System.Threading.Thread(() =>
                //{
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
                //});
                //tsk.Start();
                //tsk.Join();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error in ReadTask: " + ex.Message);
            }
            return L_IN;
        }

    }
}
