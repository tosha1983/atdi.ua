using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer;
using System.Data.Common;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{

    public class ClassesDBGetResultOptimize : IDisposable
    {
        public static ILogger logger;
        public ClassesDBGetResultOptimize(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassesDBGetResultOptimize()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<ClassSDRResults> ReadlAllResultFromDBByIdTask(MeasurementType measurementType, int TaskId)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadlAllResultFromDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        YXbsResMeas res_val = new YXbsResMeas();
                        res_val.Format("*");
                        // выбирать только таски, для которых STATUS не NULL
                        res_val.Filter = string.Format("(ID>0) AND ((STATUS<>'Z') OR (STATUS IS NULL)) AND (TYPEMEASUREMENTS='{0}') AND (MEASTASKID='{1}')", measurementType.ToString(), TaskId);
                        res_val.Order = "[ID] ASC";
                        for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                        {
                            ClassSDRResults ICSM_T = new ClassSDRResults();
                            ICSM_T.resLevels = new List<YXbsResLevels>();
                            ICSM_T.loc_sensorM = new List<YXbsResLocSensorMeas>();
                            ICSM_T.meas_res = new YXbsResMeas();
                            ICSM_T.XbsResGeneral = new List<YXbsResStGeneral>();
                            ICSM_T.XbsResLevelMeas = new List<YXbsResStLevelCar>();
                            ICSM_T.XbsResmaskBw = new List<YXbsResStMaskElm>();
                            ICSM_T.XbsResmeasstation = new List<YXbsResmeasstation>();
                            ICSM_T.XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
                            ICSM_T.XbsLinkResSensor = new List<YXbsLinkResSensor>();
                            ICSM_T.SensorName = "";

                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;


                            YXbsResmeasstation XbsYXbsResmeasstation_ = new YXbsResmeasstation();
                            XbsYXbsResmeasstation_.Format("*");
                            XbsYXbsResmeasstation_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
                            XbsYXbsResmeasstation_.Order = "[ID] ASC";
                            for (XbsYXbsResmeasstation_.OpenRs(); !XbsYXbsResmeasstation_.IsEOF(); XbsYXbsResmeasstation_.MoveNext())
                            {

                                YXbsLinkResSensor XbsYXbsLinkResSensor_ = new YXbsLinkResSensor();
                                XbsYXbsLinkResSensor_.Format("*");
                                XbsYXbsLinkResSensor_.Filter = string.Format("(IDXBSRESMEASSTA={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsLinkResSensor_.Order = "[ID] ASC";
                                for (XbsYXbsLinkResSensor_.OpenRs(); !XbsYXbsLinkResSensor_.IsEOF(); XbsYXbsLinkResSensor_.MoveNext())
                                {
                                    var m_fr_cv = new YXbsLinkResSensor();
                                    m_fr_cv.CopyDataFrom(XbsYXbsLinkResSensor_);
                                    ICSM_T.XbsLinkResSensor.Add(m_fr_cv);
                                    m_fr_cv.Close();
                                    m_fr_cv.Dispose();


                                    YXbsSensor XbsSensor_ = new YXbsSensor();
                                    XbsSensor_.Format("*");
                                    XbsSensor_.Filter = string.Format("(ID={0})", XbsYXbsLinkResSensor_.m_id_xbs_sensor);
                                    XbsSensor_.Order = "[ID] ASC";
                                    for (XbsSensor_.OpenRs(); !XbsSensor_.IsEOF(); XbsSensor_.MoveNext())
                                    {
                                        ICSM_T.SensorName = XbsSensor_.m_name;
                                    }
                                    XbsSensor_.Close();
                                    XbsSensor_.Dispose();

                                    break;
                                }
                                XbsYXbsLinkResSensor_.Close();
                                XbsYXbsLinkResSensor_.Dispose();
                                break;
                            }
                            XbsYXbsResmeasstation_.Close();
                            XbsYXbsResmeasstation_.Dispose();



                            YXbsResLocSensorMeas XbsYXbsLocationsensorm_ = new YXbsResLocSensorMeas();
                            XbsYXbsLocationsensorm_.Format("*");
                            XbsYXbsLocationsensorm_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
                            XbsYXbsLocationsensorm_.Order = "[ID] ASC";
                            for (XbsYXbsLocationsensorm_.OpenRs(); !XbsYXbsLocationsensorm_.IsEOF(); XbsYXbsLocationsensorm_.MoveNext())
                            {
                                var m_fr_ = new YXbsResLocSensorMeas();
                                m_fr_.CopyDataFrom(XbsYXbsLocationsensorm_);
                                ICSM_T.loc_sensorM.Add(m_fr_);
                                m_fr_.Close();
                                m_fr_.Dispose();
                            }
                            XbsYXbsLocationsensorm_.Close();
                            XbsYXbsLocationsensorm_.Dispose();
                            L_IN.Add(ICSM_T);
                            m_fr.Close();
                            m_fr.Dispose();
                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadlAllResultFromDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadlAllResultFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadlAllResultFromDB: " + ex.Message);
            }
            return L_IN;
        }

        public List<ClassSDRResults> ReadlAllResultFromDB(MeasurementType measurementType)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadlAllResultFromDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        YXbsResMeas res_val = new YXbsResMeas();
                        res_val.Format("*");
                        // выбирать только таски, для которых STATUS не NULL
                        res_val.Filter = string.Format("(ID>0) AND ((STATUS<>'Z') OR (STATUS IS NULL)) AND (TYPEMEASUREMENTS='{0}')", measurementType.ToString());
                        res_val.Order = "[ID] ASC";
                        for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                        {
                            ClassSDRResults ICSM_T = new ClassSDRResults();
                            ICSM_T.resLevels = new List<YXbsResLevels>();
                            ICSM_T.loc_sensorM = new List<YXbsResLocSensorMeas>();
                            ICSM_T.meas_res = new YXbsResMeas();
                            ICSM_T.XbsResGeneral = new List<YXbsResStGeneral>();
                            ICSM_T.XbsResLevelMeas = new List<YXbsResStLevelCar>();
                            ICSM_T.XbsResmaskBw = new List<YXbsResStMaskElm>();
                            ICSM_T.XbsResmeasstation = new List<YXbsResmeasstation>();
                            ICSM_T.XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
                            ICSM_T.XbsLinkResSensor = new List<YXbsLinkResSensor>();

                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;
                           

                            YXbsResLocSensorMeas XbsYXbsLocationsensorm_ = new YXbsResLocSensorMeas();
                            XbsYXbsLocationsensorm_.Format("*");
                            XbsYXbsLocationsensorm_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
                            XbsYXbsLocationsensorm_.Order = "[ID] ASC";
                            for (XbsYXbsLocationsensorm_.OpenRs(); !XbsYXbsLocationsensorm_.IsEOF(); XbsYXbsLocationsensorm_.MoveNext())
                            {
                                var m_fr_ = new YXbsResLocSensorMeas();
                                m_fr_.CopyDataFrom(XbsYXbsLocationsensorm_);
                                ICSM_T.loc_sensorM.Add(m_fr_);
                                m_fr_.Close();
                                m_fr_.Dispose();
                            }
                            XbsYXbsLocationsensorm_.Close();
                            XbsYXbsLocationsensorm_.Dispose();
                            L_IN.Add(ICSM_T);
                            m_fr.Close();
                            m_fr.Dispose();
                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadlAllResultFromDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadlAllResultFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadlAllResultFromDB: " + ex.Message);
            }
            return L_IN;
        }

        public List<ClassSDRResults> ReadGetMeasurementResults(int ResId)
        {
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadlResultFromDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    try
                    {
                        YXbsResMeas res_val = new YXbsResMeas();
                        res_val.Format("*");
                        // выбирать только таски, для которых STATUS не NULL
                        res_val.Filter = string.Format("(ID={0}) AND ((STATUS<>'Z') OR (STATUS IS NULL)) ", ResId);
                        res_val.Order = "[ID] ASC";
                        for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                        {
                            ClassSDRResults ICSM_T = new ClassSDRResults();
                            ICSM_T.resLevels = new List<YXbsResLevels>();
                            ICSM_T.loc_sensorM = new List<YXbsResLocSensorMeas>();
                            ICSM_T.meas_res = new YXbsResMeas();
                            ICSM_T.XbsResGeneral = new List<YXbsResStGeneral>();
                            ICSM_T.XbsResLevelMeas = new List<YXbsResStLevelCar>();
                            ICSM_T.XbsResmaskBw = new List<YXbsResStMaskElm>();
                            ICSM_T.XbsResmeasstation = new List<YXbsResmeasstation>();
                            ICSM_T.XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
                            ICSM_T.XbsLinkResSensor = new List<YXbsLinkResSensor>();


                            var m_frYXbsResMeas = new YXbsResMeas();
                            m_frYXbsResMeas.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_frYXbsResMeas;

                            YXbsResLevels XbsYXbsLevelmeasres_ = new YXbsResLevels();
                            XbsYXbsLevelmeasres_.Format("*");
                            XbsYXbsLevelmeasres_.Filter = string.Format("(XBSRESMEASID={0})", ResId);
                            XbsYXbsLevelmeasres_.Order = "[ID] ASC";
                            for (XbsYXbsLevelmeasres_.OpenRs(); !XbsYXbsLevelmeasres_.IsEOF(); XbsYXbsLevelmeasres_.MoveNext())
                            {
                                var m_fr_ = new YXbsResLevels();
                                m_fr_.CopyDataFrom(XbsYXbsLevelmeasres_);
                                ICSM_T.resLevels.Add(m_fr_);
                                m_fr_.Close();
                                m_fr_.Dispose();
                            }
                            XbsYXbsLevelmeasres_.Close();
                            XbsYXbsLevelmeasres_.Dispose();


                            YXbsResLevmeasonline XbsYXbsLevelmeasonlres_ = new YXbsResLevmeasonline();
                            XbsYXbsLevelmeasonlres_.Format("*");
                            XbsYXbsLevelmeasonlres_.Filter = string.Format("(XBSRESMEASID={0})", ResId);
                            XbsYXbsLevelmeasonlres_.Order = "[ID] ASC";
                            for (XbsYXbsLevelmeasonlres_.OpenRs(); !XbsYXbsLevelmeasonlres_.IsEOF(); XbsYXbsLevelmeasonlres_.MoveNext())
                            {
                                var m_fr_ = new YXbsResLevmeasonline();
                                m_fr_.CopyDataFrom(XbsYXbsLevelmeasonlres_);
                                ICSM_T.level_meas_onl_res.Add(m_fr_);
                                m_fr_.Close();
                                m_fr_.Dispose();
                            }
                            XbsYXbsLevelmeasonlres_.Close();
                            XbsYXbsLevelmeasonlres_.Dispose();


                            YXbsResLocSensorMeas XbsYXbsLocationsensorm_ = new YXbsResLocSensorMeas();
                            XbsYXbsLocationsensorm_.Format("*");
                            XbsYXbsLocationsensorm_.Filter = string.Format("(XBSRESMEASID={0})", ResId);
                            XbsYXbsLocationsensorm_.Order = "[ID] ASC";
                            for (XbsYXbsLocationsensorm_.OpenRs(); !XbsYXbsLocationsensorm_.IsEOF(); XbsYXbsLocationsensorm_.MoveNext())
                            {
                                var m_fr_ = new YXbsResLocSensorMeas();
                                m_fr_.CopyDataFrom(XbsYXbsLocationsensorm_);
                                ICSM_T.loc_sensorM.Add(m_fr_);
                                m_fr_.Close();
                                m_fr_.Dispose();
                            }
                            XbsYXbsLocationsensorm_.Close();
                            XbsYXbsLocationsensorm_.Dispose();
                            L_IN.Add(ICSM_T);


                            m_frYXbsResMeas.Close();
                            m_frYXbsResMeas.Dispose();
                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadlResultFromDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadlResultFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadlResultFromDB: " + ex.Message);
            }
            return L_IN;
        }


        public List<ClassSDRResults> ReadGetMeasurementResultsByTaskId(int MeasTaskId)
        {
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadlResultFromDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    try
                    {
                        YXbsResMeas res_val = new YXbsResMeas();
                        res_val.Format("*");
                        // выбирать только таски, для которых STATUS не NULL
                        res_val.Filter = string.Format("(MEASTASKID='{0}') AND ((STATUS<>'Z') OR (STATUS IS NULL)) ", MeasTaskId);
                        res_val.Order = "[ID] ASC";
                        for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                        {

                            ClassSDRResults ICSM_T = new ClassSDRResults();
                            ICSM_T.resLevels = new List<YXbsResLevels>();
                            ICSM_T.loc_sensorM = new List<YXbsResLocSensorMeas>();
                            ICSM_T.meas_res = new YXbsResMeas();
                            ICSM_T.XbsResGeneral = new List<YXbsResStGeneral>();
                            ICSM_T.XbsResLevelMeas = new List<YXbsResStLevelCar>();
                            ICSM_T.XbsResmaskBw = new List<YXbsResStMaskElm>();
                            ICSM_T.XbsResmeasstation = new List<YXbsResmeasstation>();
                            ICSM_T.XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
                            ICSM_T.XbsLinkResSensor = new List<YXbsLinkResSensor>();

                          
                            var m_frYXbsResMeas = new YXbsResMeas();
                            m_frYXbsResMeas.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_frYXbsResMeas;

                            YXbsResLevels XbsYXbsLevelmeasres_ = new YXbsResLevels();
                            XbsYXbsLevelmeasres_.Format("*");
                            XbsYXbsLevelmeasres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id.Value);
                            XbsYXbsLevelmeasres_.Order = "[ID] ASC";
                            for (XbsYXbsLevelmeasres_.OpenRs(); !XbsYXbsLevelmeasres_.IsEOF(); XbsYXbsLevelmeasres_.MoveNext())
                            {

                                var m_fr_ = new YXbsResLevels();
                                m_fr_.CopyDataFrom(XbsYXbsLevelmeasres_);
                                ICSM_T.resLevels.Add(m_fr_);
                                m_fr_.Close();
                                m_fr_.Dispose();
                            }
                            XbsYXbsLevelmeasres_.Close();
                            XbsYXbsLevelmeasres_.Dispose();


                            YXbsResLevmeasonline XbsYXbsLevelmeasonlres_ = new YXbsResLevmeasonline();
                            XbsYXbsLevelmeasonlres_.Format("*");
                            XbsYXbsLevelmeasonlres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id.Value);
                            XbsYXbsLevelmeasonlres_.Order = "[ID] ASC";
                            for (XbsYXbsLevelmeasonlres_.OpenRs(); !XbsYXbsLevelmeasonlres_.IsEOF(); XbsYXbsLevelmeasonlres_.MoveNext())
                            {
                                var m_fr_ = new YXbsResLevmeasonline();
                                m_fr_.CopyDataFrom(XbsYXbsLevelmeasonlres_);
                                ICSM_T.level_meas_onl_res.Add(m_fr_);
                                m_fr_.Close();
                                m_fr_.Dispose();
                            }
                            XbsYXbsLevelmeasonlres_.Close();
                            XbsYXbsLevelmeasonlres_.Dispose();


                            YXbsResLocSensorMeas XbsYXbsLocationsensorm_ = new YXbsResLocSensorMeas();
                            XbsYXbsLocationsensorm_.Format("*");
                            XbsYXbsLocationsensorm_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id.Value);
                            XbsYXbsLocationsensorm_.Order = "[ID] ASC";
                            for (XbsYXbsLocationsensorm_.OpenRs(); !XbsYXbsLocationsensorm_.IsEOF(); XbsYXbsLocationsensorm_.MoveNext())
                            {
                                var m_fr_ = new YXbsResLocSensorMeas();
                                m_fr_.CopyDataFrom(XbsYXbsLocationsensorm_);
                                ICSM_T.loc_sensorM.Add(m_fr_);
                                m_fr_.Close();
                                m_fr_.Dispose();
                            }
                            XbsYXbsLocationsensorm_.Close();
                            XbsYXbsLocationsensorm_.Dispose();
                            L_IN.Add(ICSM_T);

                            m_frYXbsResMeas.Close();
                            m_frYXbsResMeas.Dispose();


                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadlResultFromDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadlResultFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadlResultFromDB: " + ex.Message);
            }
            return L_IN;
        }


        public List<ClassSDRResults> ReadResultHeaderFromId(int MeasTaskId)
        {
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadlAllResultFromDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        YXbsResMeas res_val = new YXbsResMeas();
                        res_val.Format("*");
                        // выбирать только таски, для которых STATUS не NULL
                        res_val.Filter = string.Format("(MEASTASKID='{0}') AND ((STATUS<>'Z') OR (STATUS IS NULL)) ", MeasTaskId);
                        res_val.Order = "[ID] ASC";
                        for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                        {
                            ClassSDRResults ICSM_T = new ClassSDRResults();
                            ICSM_T.resLevels = new List<YXbsResLevels>();
                            ICSM_T.loc_sensorM = new List<YXbsResLocSensorMeas>();
                            ICSM_T.meas_res = new YXbsResMeas();
                            ICSM_T.XbsResGeneral = new List<YXbsResStGeneral>();
                            ICSM_T.XbsResLevelMeas = new List<YXbsResStLevelCar>();
                            ICSM_T.XbsResmaskBw = new List<YXbsResStMaskElm>();
                            ICSM_T.XbsResmeasstation = new List<YXbsResmeasstation>();
                            ICSM_T.XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
                            ICSM_T.XbsLinkResSensor = new List<YXbsLinkResSensor>();
                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;
                            L_IN.Add(ICSM_T);
                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadlAllResultFromDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadlAllResultFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadlAllResultFromDB: " + ex.Message);
            }
            return L_IN;
        }


        public List<ClassSDRResults> ReadResultHeaderFromIdentifier(MeasurementResultsIdentifier obj)
        {
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            string sql = "";
            try
            {
                logger.Trace("Start procedure ReadResultHeaderFromIdentifier...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    try
                    {
                        if (obj.MeasTaskId != null) sql += string.Format("(MEASTASKID ='{0}')", obj.MeasTaskId.Value.ToString());
                        if (obj.MeasSdrResultsId > 0) sql += sql.Length > 0 ? string.Format(" AND (ID ={0})", obj.MeasSdrResultsId) : string.Format("(ID ={0})", obj.MeasSdrResultsId);
                        if (obj.SubMeasTaskId > 0) sql += sql.Length > 0 ? string.Format(" AND (SUBMEASTASKID ={0})", obj.SubMeasTaskId) : string.Format("(SUBMEASTASKID ={0})", obj.SubMeasTaskId);
                        if (obj.SubMeasTaskStationId > 0) sql += sql.Length > 0 ? string.Format(" AND (SUBMEASTASKSTATIONID ={0})", obj.SubMeasTaskStationId) : string.Format("(SUBMEASTASKSTATIONID ={0})", obj.SubMeasTaskStationId);
                        if (sql.Length > 0)
                        {
                            YXbsResMeas res_val = new YXbsResMeas();
                            res_val.Format("*");
                            res_val.Filter = sql;
                            for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                            {
                                ClassSDRResults ICSM_T = new ClassSDRResults();
                                ICSM_T.resLevels = new List<YXbsResLevels>();
                                ICSM_T.loc_sensorM = new List<YXbsResLocSensorMeas>();
                                ICSM_T.meas_res = new YXbsResMeas();
                                ICSM_T.XbsResGeneral = new List<YXbsResStGeneral>();
                                ICSM_T.XbsResLevelMeas = new List<YXbsResStLevelCar>();
                                ICSM_T.XbsResmaskBw = new List<YXbsResStMaskElm>();
                                ICSM_T.XbsResmeasstation = new List<YXbsResmeasstation>();
                                ICSM_T.XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
                                ICSM_T.XbsLinkResSensor = new List<YXbsLinkResSensor>();
                                var m_fr = new YXbsResMeas();
                                m_fr.CopyDataFrom(res_val);
                                ICSM_T.meas_res = m_fr;

                                L_IN.Add(ICSM_T);
                            }

                            res_val.Close();
                            res_val.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadResultHeaderFromIdentifier... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadResultHeaderFromIdentifier.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadResultHeaderFromIdentifier: " + ex.Message);
            }
            return L_IN;
        }


        public List<ClassSDRResults> ReadResultHeaderFromTaskId(int MeasTaskId)
        {
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadResultHeaders...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        YXbsResMeas res_val = new YXbsResMeas();
                        res_val.Format("*");
                        // выбирать только таски, для которых STATUS не NULL
                        res_val.Filter = string.Format("(MEASTASKID='{0}') AND ((STATUS<>'Z') OR (STATUS IS NULL)) ", MeasTaskId);
                        res_val.Order = "[ID] ASC";
                        for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                        {
                            ClassSDRResults ICSM_T = new ClassSDRResults();
                            ICSM_T.resLevels = new List<YXbsResLevels>();
                            ICSM_T.loc_sensorM = new List<YXbsResLocSensorMeas>();
                            ICSM_T.meas_res = new YXbsResMeas();
                            ICSM_T.XbsResGeneral = new List<YXbsResStGeneral>();
                            ICSM_T.XbsResLevelMeas = new List<YXbsResStLevelCar>();
                            ICSM_T.XbsResmaskBw = new List<YXbsResStMaskElm>();
                            ICSM_T.XbsResmeasstation = new List<YXbsResmeasstation>();
                            ICSM_T.XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
                            ICSM_T.XbsLinkResSensor = new List<YXbsLinkResSensor>();
                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;
                            L_IN.Add(ICSM_T);
                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadResultHeaders... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadResultHeaders.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadResultHeaders: " + ex.Message);
            }
            return L_IN;
        }


        public List<ClassSDRResults> ReadResultStationHeaderByResId(int ID)
        {
            const int Cn = 900;
            // Список объектов в рамках конкретного адаптера ICSM
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadResultStationHeaderByResId...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    try
                    {
                        YXbsResMeas res_val = new YXbsResMeas();
                        res_val.Format("*");
                        res_val.Filter = string.Format("(ID={0})", ID);
                        res_val.Order = "[ID] ASC";
                        for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                        {
                            ClassSDRResults ICSM_T = new ClassSDRResults();
                            ICSM_T.resLevels = new List<YXbsResLevels>();
                            ICSM_T.loc_sensorM = new List<YXbsResLocSensorMeas>();
                            ICSM_T.meas_res = new YXbsResMeas();
                            ICSM_T.XbsResGeneral = new List<YXbsResStGeneral>();
                            ICSM_T.XbsResLevelMeas = new List<YXbsResStLevelCar>();
                            ICSM_T.XbsResmaskBw = new List<YXbsResStMaskElm>();
                            ICSM_T.XbsResmeasstation = new List<YXbsResmeasstation>();
                            ICSM_T.XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
                            ICSM_T.XbsLinkResSensor = new List<YXbsLinkResSensor>();

                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;

                            /////
                            /*
                            YXbsResmeasstation XbsYXbsResmeasstation_ = new YXbsResmeasstation();
                            XbsYXbsResmeasstation_.Format("*");
                            XbsYXbsResmeasstation_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
                            XbsYXbsResmeasstation_.Order = "[ID] ASC";
                            for (XbsYXbsResmeasstation_.OpenRs(); !XbsYXbsResmeasstation_.IsEOF(); XbsYXbsResmeasstation_.MoveNext())
                            {
                                var m_fr_1 = new YXbsResmeasstation();
                                m_fr_1.CopyDataFrom(XbsYXbsResmeasstation_);
                                ICSM_T.XbsResmeasstation.Add(m_fr_1);
                                m_fr_1.Close();
                                m_fr_1.Dispose();

                            }
                            XbsYXbsResmeasstation_.Close();
                            XbsYXbsResmeasstation_.Dispose();
                            */

                            int idx_YXbsResmeasstation = 0;
                            List<int> sql_YXbsResmeasstation_in = new List<int>();
                            string sql_YXbsResmeasstation = "";
                            string sql_YXbsResStLevelCar = "";
                            YXbsResmeasstation XbsYXbsResmeasstation_ = new YXbsResmeasstation();
                            XbsYXbsResmeasstation_.Format("*");
                            XbsYXbsResmeasstation_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
                            XbsYXbsResmeasstation_.Order = "[ID] ASC";
                            for (XbsYXbsResmeasstation_.OpenRs(); !XbsYXbsResmeasstation_.IsEOF(); XbsYXbsResmeasstation_.MoveNext())
                            {

                                var m_fr_1 = new YXbsResmeasstation();
                                m_fr_1.CopyDataFrom(XbsYXbsResmeasstation_);
                                ICSM_T.XbsResmeasstation.Add(m_fr_1);
                                m_fr_1.Close();
                                m_fr_1.Dispose();

                                if (sql_YXbsResmeasstation_in.Count <= Cn)
                                {
                                    sql_YXbsResmeasstation_in.Add(XbsYXbsResmeasstation_.m_id.Value);
                                }
                                if ((sql_YXbsResmeasstation_in.Count > Cn) || ((idx_YXbsResmeasstation + 1) == XbsYXbsResmeasstation_.GetCount()))
                                {
                                    sql_YXbsResmeasstation = string.Format("(XBS_RESMEASSTATIONID IN ({0}))", string.Join(",", sql_YXbsResmeasstation_in));
                                    sql_YXbsResStLevelCar = string.Format("(RESMEASSTATIONID IN ({0}))", string.Join(",", sql_YXbsResmeasstation_in));
                                    sql_YXbsResmeasstation_in.Clear();


                                    List<int> sql_YXbsResStGeneral_in = new List<int>();
                                    YXbsResStGeneral XbsYXbsResGeneral_ = new YXbsResStGeneral();
                                    XbsYXbsResGeneral_.Format("*");
                                    XbsYXbsResGeneral_.Filter = sql_YXbsResStLevelCar;
                                    XbsYXbsResGeneral_.Order = "[ID] ASC";
                                    for (XbsYXbsResGeneral_.OpenRs(); !XbsYXbsResGeneral_.IsEOF(); XbsYXbsResGeneral_.MoveNext())
                                    {
                                        var m_fr_4 = new YXbsResStGeneral();
                                        m_fr_4.CopyDataFrom(XbsYXbsResGeneral_);
                                        ICSM_T.XbsResGeneral.Add(m_fr_4);
                                        m_fr_4.Close();
                                        m_fr_4.Dispose();
                                        if (sql_YXbsResStGeneral_in.Count <= Cn)
                                        {
                                            sql_YXbsResStGeneral_in.Add(XbsYXbsResGeneral_.m_id.Value);
                                        }
                                    }
                                    XbsYXbsResGeneral_.Close();
                                    XbsYXbsResGeneral_.Dispose();
                                }
                                idx_YXbsResmeasstation++;
                            }
                            XbsYXbsResmeasstation_.Close();
                            XbsYXbsResmeasstation_.Dispose();


                            L_IN.Add(ICSM_T);
                            m_fr.Close();
                            m_fr.Dispose();
                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadlResultFromDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadlResultFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadlResultFromDB: " + ex.Message);
            }
            return L_IN;
        }


        public List<ClassSDRResults> ReadResultResMeasStation(int StationId)
        {
            const int Cn = 900;
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            string sql = "";
            try
            {
                logger.Trace("Start procedure ReadResultResMeasStation...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    try
                    {

                        ClassSDRResults ICSM_T = new ClassSDRResults();
                        ICSM_T.resLevels = new List<YXbsResLevels>();
                        ICSM_T.loc_sensorM = new List<YXbsResLocSensorMeas>();
                        ICSM_T.meas_res = new YXbsResMeas();
                        ICSM_T.XbsResGeneral = new List<YXbsResStGeneral>();
                        ICSM_T.XbsResLevelMeas = new List<YXbsResStLevelCar>();
                        ICSM_T.XbsResmaskBw = new List<YXbsResStMaskElm>();
                        ICSM_T.XbsResmeasstation = new List<YXbsResmeasstation>();
                        ICSM_T.XbsLevelSpecrum = new List<YXbsResStLevelsSpect>();
                        ICSM_T.XbsLinkResSensor = new List<YXbsLinkResSensor>();


                        int idx_YXbsResmeasstation = 0;
                        List<int> sql_YXbsResmeasstation_in = new List<int>();
                        string sql_YXbsResmeasstation = "";
                        string sql_YXbsResStLevelCar = "";
                        YXbsResmeasstation XbsYXbsResmeasstation_ = new YXbsResmeasstation();
                        XbsYXbsResmeasstation_.Format("*");
                        XbsYXbsResmeasstation_.Filter = string.Format("(ID = {0})", StationId);
                        XbsYXbsResmeasstation_.Order = "[ID] ASC";
                        for (XbsYXbsResmeasstation_.OpenRs(); !XbsYXbsResmeasstation_.IsEOF(); XbsYXbsResmeasstation_.MoveNext())
                        {

                            var m_fr_2 = new YXbsResmeasstation();
                            m_fr_2.CopyDataFrom(XbsYXbsResmeasstation_);
                            ICSM_T.XbsResmeasstation.Add(m_fr_2);
                            m_fr_2.Close();
                            m_fr_2.Dispose();

                            if (sql_YXbsResmeasstation_in.Count <= Cn)
                            {
                                sql_YXbsResmeasstation_in.Add(XbsYXbsResmeasstation_.m_id.Value);
                            }
                            if ((sql_YXbsResmeasstation_in.Count > Cn) || ((idx_YXbsResmeasstation + 1) == XbsYXbsResmeasstation_.GetCount()))
                            {
                                sql_YXbsResmeasstation = string.Format("(XBS_RESMEASSTATIONID IN ({0}))", string.Join(",", sql_YXbsResmeasstation_in));
                                sql_YXbsResStLevelCar = string.Format("(RESMEASSTATIONID IN ({0}))", string.Join(",", sql_YXbsResmeasstation_in));
                                sql_YXbsResmeasstation_in.Clear();

                                YXbsResStLevelCar XbsYXbsResLevelMeas_ = new YXbsResStLevelCar();
                                XbsYXbsResLevelMeas_.Format("*");
                                XbsYXbsResLevelMeas_.Filter = sql_YXbsResmeasstation;
                                XbsYXbsResLevelMeas_.Order = "[ID] ASC";
                                for (XbsYXbsResLevelMeas_.OpenRs(); !XbsYXbsResLevelMeas_.IsEOF(); XbsYXbsResLevelMeas_.MoveNext())
                                {
                                    var m_fr_3 = new YXbsResStLevelCar();
                                    m_fr_3.CopyDataFrom(XbsYXbsResLevelMeas_);
                                    ICSM_T.XbsResLevelMeas.Add(m_fr_3);
                                    m_fr_3.Close();
                                    m_fr_3.Dispose();
                                }
                                XbsYXbsResLevelMeas_.Close();
                                XbsYXbsResLevelMeas_.Dispose();
                                int idx_YXbsResStGeneral = 0;
                                List<int> sql_YXbsResStGeneral_in = new List<int>();
                                string sql_YXbsResStGeneral = "";
                                YXbsResStGeneral XbsYXbsResGeneral_ = new YXbsResStGeneral();
                                XbsYXbsResGeneral_.Format("*");
                                XbsYXbsResGeneral_.Filter = sql_YXbsResStLevelCar;
                                XbsYXbsResGeneral_.Order = "[ID] ASC";
                                for (XbsYXbsResGeneral_.OpenRs(); !XbsYXbsResGeneral_.IsEOF(); XbsYXbsResGeneral_.MoveNext())
                                {
                                    var m_fr_4 = new YXbsResStGeneral();
                                    m_fr_4.CopyDataFrom(XbsYXbsResGeneral_);
                                    ICSM_T.XbsResGeneral.Add(m_fr_4);
                                    m_fr_4.Close();
                                    m_fr_4.Dispose();
                                    if (sql_YXbsResStGeneral_in.Count <= Cn)
                                    {
                                        sql_YXbsResStGeneral_in.Add(XbsYXbsResGeneral_.m_id.Value);
                                    }
                                    if ((sql_YXbsResStGeneral_in.Count > Cn) || ((idx_YXbsResStGeneral + 1) == XbsYXbsResGeneral_.GetCount()))
                                    {
                                        sql_YXbsResStGeneral = string.Format("(XBS_RESSTGENERALID IN ({0}))", string.Join(",", sql_YXbsResStGeneral_in));
                                        sql_YXbsResmeasstation_in.Clear();
                                        YXbsResStMaskElm XbsYXbsResmaskBw_ = new YXbsResStMaskElm();
                                        XbsYXbsResmaskBw_.Format("*");
                                        XbsYXbsResmaskBw_.Filter = sql_YXbsResStGeneral;
                                        XbsYXbsResmaskBw_.Order = "[ID] ASC";
                                        for (XbsYXbsResmaskBw_.OpenRs(); !XbsYXbsResmaskBw_.IsEOF(); XbsYXbsResmaskBw_.MoveNext())
                                        {
                                            var m_fr_5 = new YXbsResStMaskElm();
                                            m_fr_5.CopyDataFrom(XbsYXbsResmaskBw_);
                                            ICSM_T.XbsResmaskBw.Add(m_fr_5);
                                            m_fr_5.Close();
                                            m_fr_5.Dispose();

                                        }
                                        XbsYXbsResmaskBw_.Close();
                                        XbsYXbsResmaskBw_.Dispose();
                                        YXbsResStLevelsSpect XbsYXbsLevelSpecrum_ = new YXbsResStLevelsSpect();
                                        XbsYXbsLevelSpecrum_.Format("*");
                                        XbsYXbsLevelSpecrum_.Filter = sql_YXbsResStGeneral;
                                        XbsYXbsLevelSpecrum_.Order = "[ID] ASC";
                                        for (XbsYXbsLevelSpecrum_.OpenRs(); !XbsYXbsLevelSpecrum_.IsEOF(); XbsYXbsLevelSpecrum_.MoveNext())
                                        {
                                            var m_fr_6 = new YXbsResStLevelsSpect();
                                            m_fr_6.CopyDataFrom(XbsYXbsLevelSpecrum_);
                                            ICSM_T.XbsLevelSpecrum.Add(m_fr_6);
                                            m_fr_6.Close();
                                            m_fr_6.Dispose();
                                        }
                                        XbsYXbsLevelSpecrum_.Close();
                                        XbsYXbsLevelSpecrum_.Dispose();
                                    }

                                    idx_YXbsResStGeneral++;
                                }
                                XbsYXbsResGeneral_.Close();
                                XbsYXbsResGeneral_.Dispose();
                            }
                            idx_YXbsResmeasstation++;
                        }
                        XbsYXbsResmeasstation_.Close();
                        XbsYXbsResmeasstation_.Dispose();
                        L_IN.Add(ICSM_T);
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadResultResMeasStation... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadResultResMeasStation.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadResultResMeasStation: " + ex.Message);
            }
            return L_IN;
        }

    }
}
