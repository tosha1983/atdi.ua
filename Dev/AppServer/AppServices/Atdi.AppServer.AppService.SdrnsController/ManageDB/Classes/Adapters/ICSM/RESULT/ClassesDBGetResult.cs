using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer;
using System.Data.Common;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{

    public class ClassesDBGetResult : IDisposable
    {
        public static ILogger logger;
        public ClassesDBGetResult(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~ClassesDBGetResult()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public List<ClassSDRResults> ReadResultFromDB(MeasurementResultsIdentifier obj)
        {
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            string sql = "";
            try
            {
                logger.Trace("Start procedure ReadlResultFromDB...");
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

                                var m_fr = new YXbsResMeas();
                                m_fr.CopyDataFrom(res_val);
                                ICSM_T.meas_res = m_fr;

                                /////
                                YXbsResmeasstation XbsYXbsResmeasstation_ = new YXbsResmeasstation();
                                XbsYXbsResmeasstation_.Format("*");
                                XbsYXbsResmeasstation_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
                                XbsYXbsResmeasstation_.Order = "[ID] ASC";
                                for (XbsYXbsResmeasstation_.OpenRs(); !XbsYXbsResmeasstation_.IsEOF(); XbsYXbsResmeasstation_.MoveNext())
                                {
                                    var m_fr_2 = new YXbsResmeasstation();
                                    m_fr_2.CopyDataFrom(XbsYXbsResmeasstation_);
                                    ICSM_T.XbsResmeasstation.Add(m_fr_2);
                                    m_fr_2.Close();
                                    m_fr_2.Dispose();


                                    YXbsResStLevelCar XbsYXbsResLevelMeas_ = new YXbsResStLevelCar();
                                    XbsYXbsResLevelMeas_.Format("*");
                                    XbsYXbsResLevelMeas_.Filter = string.Format("(XBS_RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
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


                                    YXbsResStGeneral XbsYXbsResGeneral_ = new YXbsResStGeneral();
                                    XbsYXbsResGeneral_.Format("*");
                                    XbsYXbsResGeneral_.Filter = string.Format("(RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                    XbsYXbsResGeneral_.Order = "[ID] ASC";
                                    for (XbsYXbsResGeneral_.OpenRs(); !XbsYXbsResGeneral_.IsEOF(); XbsYXbsResGeneral_.MoveNext())
                                    {
                                        var m_fr_4 = new YXbsResStGeneral();
                                        m_fr_4.CopyDataFrom(XbsYXbsResGeneral_);
                                        ICSM_T.XbsResGeneral.Add(m_fr_4);
                                        m_fr_4.Close();
                                        m_fr_4.Dispose();


                                        YXbsResStMaskElm XbsYXbsResmaskBw_ = new YXbsResStMaskElm();
                                        XbsYXbsResmaskBw_.Format("*");
                                        XbsYXbsResmaskBw_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
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
                                        XbsYXbsLevelSpecrum_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
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
                                    XbsYXbsResGeneral_.Close();
                                    XbsYXbsResGeneral_.Dispose();
                                }
                                XbsYXbsResmeasstation_.Close();
                                XbsYXbsResmeasstation_.Dispose();


                                YXbsResLevels XbsYXbsLevelmeasres_ = new YXbsResLevels();
                                XbsYXbsLevelmeasres_.Format("*");
                                XbsYXbsLevelmeasres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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
                                XbsYXbsLevelmeasonlres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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

   


        public SensorPoligonPoint[] GetSensorPoligonPoint(int ID)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            List<SensorPoligonPoint> L_IN = new List<SensorPoligonPoint>();
            try
            {
                logger.Trace("Start procedure GetSensorPoligonPoint...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    try
                    {
                        YXbsResMeas res_val = new YXbsResMeas();
                        res_val.Format("*");
                        res_val.Filter = string.Format("(ID={0})", ID);
                        res_val.Order = "[ID] ASC";
                        for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                        {
                            YXbsResmeasstation XbsResmeasstation = new YXbsResmeasstation();
                            XbsResmeasstation.Format("*");
                            XbsResmeasstation.Filter = string.Format("(XBSRESMEASID='{0}')", res_val.m_id);
                            XbsResmeasstation.Order = "[ID] ASC";
                            for (XbsResmeasstation.OpenRs(); !XbsResmeasstation.IsEOF(); XbsResmeasstation.MoveNext())
                            {
                                YXbsLinkResSensor LinkResSensor = new YXbsLinkResSensor();
                                LinkResSensor.Format("*");
                                LinkResSensor.Filter = string.Format("(IDXBSRESMEASSTA={0})", XbsResmeasstation.m_id);
                                LinkResSensor.Order = "[ID] ASC";
                                for (LinkResSensor.OpenRs(); !LinkResSensor.IsEOF(); LinkResSensor.MoveNext())
                                {
                                    YXbsSensorpolig Sensorpolig = new YXbsSensorpolig();
                                    Sensorpolig.Format("*");
                                    Sensorpolig.Filter = string.Format("(SENSORID={0})", LinkResSensor.m_id_xbs_sensor);
                                    Sensorpolig.Order = "[ID] ASC";
                                    for (Sensorpolig.OpenRs(); !Sensorpolig.IsEOF(); Sensorpolig.MoveNext())
                                    {
                                        SensorPoligonPoint sensorPolygon = new SensorPoligonPoint();
                                        sensorPolygon.Lon = Sensorpolig.m_lon;
                                        sensorPolygon.Lat = Sensorpolig.m_lat;
                                        L_IN.Add(sensorPolygon);
                                    }
                                    Sensorpolig.Close();
                                    Sensorpolig.Dispose();
                                }
                                LinkResSensor.Close();
                                LinkResSensor.Dispose();
                            }
                            XbsResmeasstation.Close();
                            XbsResmeasstation.Dispose();


                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure GetSensorPoligonPoint... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure GetSensorPoligonPoint.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure GetSensorPoligonPoint: " + ex.Message);
            }
            return L_IN.ToArray();
        }

        public List<ClassSDRResults> ReadShortResultResMeasStationsFromDB(int ID)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadShortResultResMeasStationsFromDB...");
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

                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;

                            /////
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
                            L_IN.Add(ICSM_T);
                            XbsYXbsResmeasstation_.Close();
                            XbsYXbsResmeasstation_.Dispose();
                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadShortResultResMeasStationsFromDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadShortResultResMeasStationsFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadlResultFromDB: " + ex.Message);
            }
            return L_IN;
        }

        public List<ClassSDRResults> ReadResultResMeasStationsFromDB(int ResId, int StationId)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadResultResMeasStationsFromDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    try
                    {
                        YXbsResMeas res_val = new YXbsResMeas();
                        res_val.Format("*");
                        res_val.Filter = string.Format("(ID={0})", ResId);
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

                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;

                            /////
                            YXbsResmeasstation XbsYXbsResmeasstation_ = new YXbsResmeasstation();
                            XbsYXbsResmeasstation_.Format("*");
                            XbsYXbsResmeasstation_.Filter = string.Format("(XBSRESMEASID={0}) AND (IDXBSSTATION={1})", res_val.m_id, StationId);
                            XbsYXbsResmeasstation_.Order = "[ID] ASC";
                            for (XbsYXbsResmeasstation_.OpenRs(); !XbsYXbsResmeasstation_.IsEOF(); XbsYXbsResmeasstation_.MoveNext())
                            {
                                var m_fr_1 = new YXbsResmeasstation();
                                m_fr_1.CopyDataFrom(XbsYXbsResmeasstation_);
                                ICSM_T.XbsResmeasstation.Add(m_fr_1);
                                m_fr_1.Close();
                                m_fr_1.Dispose();


                                YXbsResStLevelCar XbsYXbsResLevelMeas_ = new YXbsResStLevelCar();
                                XbsYXbsResLevelMeas_.Format("*");
                                XbsYXbsResLevelMeas_.Filter = string.Format("(XBS_RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResLevelMeas_.Order = "[ID] ASC";
                                for (XbsYXbsResLevelMeas_.OpenRs(); !XbsYXbsResLevelMeas_.IsEOF(); XbsYXbsResLevelMeas_.MoveNext())
                                {
                                    var m_fr_2 = new YXbsResStLevelCar();
                                    m_fr_2.CopyDataFrom(XbsYXbsResLevelMeas_);
                                    ICSM_T.XbsResLevelMeas.Add(m_fr_2);
                                    m_fr_2.Close();
                                    m_fr_2.Dispose();
                                }
                                XbsYXbsResLevelMeas_.Close();
                                XbsYXbsResLevelMeas_.Dispose();


                                YXbsResStGeneral XbsYXbsResGeneral_ = new YXbsResStGeneral();
                                XbsYXbsResGeneral_.Format("*");
                                XbsYXbsResGeneral_.Filter = string.Format("(RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResGeneral_.Order = "[ID] ASC";
                                for (XbsYXbsResGeneral_.OpenRs(); !XbsYXbsResGeneral_.IsEOF(); XbsYXbsResGeneral_.MoveNext())
                                {
                                    var m_fr_3 = new YXbsResStGeneral();
                                    m_fr_3.CopyDataFrom(XbsYXbsResGeneral_);
                                    ICSM_T.XbsResGeneral.Add(m_fr_3);
                                    m_fr_3.Close();
                                    m_fr_3.Dispose();


                                    YXbsResStMaskElm XbsYXbsResmaskBw_ = new YXbsResStMaskElm();
                                    XbsYXbsResmaskBw_.Format("*");
                                    XbsYXbsResmaskBw_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsResmaskBw_.Order = "[ID] ASC";
                                    for (XbsYXbsResmaskBw_.OpenRs(); !XbsYXbsResmaskBw_.IsEOF(); XbsYXbsResmaskBw_.MoveNext())
                                    {
                                        var m_fr_4 = new YXbsResStMaskElm();
                                        m_fr_4.CopyDataFrom(XbsYXbsResmaskBw_);
                                        ICSM_T.XbsResmaskBw.Add(m_fr_4);
                                        m_fr_4.Close();
                                        m_fr_4.Dispose();

                                    }
                                    XbsYXbsResmaskBw_.Close();
                                    XbsYXbsResmaskBw_.Dispose();


                                    YXbsResStLevelsSpect XbsYXbsLevelSpecrum_ = new YXbsResStLevelsSpect();
                                    XbsYXbsLevelSpecrum_.Format("*");
                                    XbsYXbsLevelSpecrum_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsLevelSpecrum_.Order = "[ID] ASC";
                                    for (XbsYXbsLevelSpecrum_.OpenRs(); !XbsYXbsLevelSpecrum_.IsEOF(); XbsYXbsLevelSpecrum_.MoveNext())
                                    {
                                        var m_fr_5 = new YXbsResStLevelsSpect();
                                        m_fr_5.CopyDataFrom(XbsYXbsLevelSpecrum_);
                                        ICSM_T.XbsLevelSpecrum.Add(m_fr_5);
                                        m_fr_5.Close();
                                        m_fr_5.Dispose();
                                    }
                                    XbsYXbsLevelSpecrum_.Close();
                                    XbsYXbsLevelSpecrum_.Dispose();

                                }
                                XbsYXbsResGeneral_.Close();
                                XbsYXbsResGeneral_.Dispose();


                            }
                            L_IN.Add(ICSM_T);
                            XbsYXbsResmeasstation_.Close();
                            XbsYXbsResmeasstation_.Dispose();

                        }
                        res_val.Close();
                        res_val.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure ReadResultResMeasStationsFromDB... " + ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure ReadResultResMeasStationsFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadResultResMeasStationsFromDB: " + ex.Message);
            }
            return L_IN;
        }

        public List<ClassSDRResults> ReadResultFromDB(int ID)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadlResultFromDB...");
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

                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;

                            /////
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


                                YXbsResStLevelCar XbsYXbsResLevelMeas_ = new YXbsResStLevelCar();
                                XbsYXbsResLevelMeas_.Format("*");
                                XbsYXbsResLevelMeas_.Filter = string.Format("(XBS_RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResLevelMeas_.Order = "[ID] ASC";
                                for (XbsYXbsResLevelMeas_.OpenRs(); !XbsYXbsResLevelMeas_.IsEOF(); XbsYXbsResLevelMeas_.MoveNext())
                                {
                                    var m_fr_2 = new YXbsResStLevelCar();
                                    m_fr_2.CopyDataFrom(XbsYXbsResLevelMeas_);
                                    ICSM_T.XbsResLevelMeas.Add(m_fr_2);
                                    m_fr_2.Close();
                                    m_fr_2.Dispose();
                                }
                                XbsYXbsResLevelMeas_.Close();
                                XbsYXbsResLevelMeas_.Dispose();


                                YXbsResStGeneral XbsYXbsResGeneral_ = new YXbsResStGeneral();
                                XbsYXbsResGeneral_.Format("*");
                                XbsYXbsResGeneral_.Filter = string.Format("(RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResGeneral_.Order = "[ID] ASC";
                                for (XbsYXbsResGeneral_.OpenRs(); !XbsYXbsResGeneral_.IsEOF(); XbsYXbsResGeneral_.MoveNext())
                                {
                                    var m_fr_3 = new YXbsResStGeneral();
                                    m_fr_3.CopyDataFrom(XbsYXbsResGeneral_);
                                    ICSM_T.XbsResGeneral.Add(m_fr_3);
                                    m_fr_3.Close();
                                    m_fr_3.Dispose();


                                    YXbsResStMaskElm XbsYXbsResmaskBw_ = new YXbsResStMaskElm();
                                    XbsYXbsResmaskBw_.Format("*");
                                    XbsYXbsResmaskBw_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsResmaskBw_.Order = "[ID] ASC";
                                    for (XbsYXbsResmaskBw_.OpenRs(); !XbsYXbsResmaskBw_.IsEOF(); XbsYXbsResmaskBw_.MoveNext())
                                    {
                                        var m_fr_4 = new YXbsResStMaskElm();
                                        m_fr_4.CopyDataFrom(XbsYXbsResmaskBw_);
                                        ICSM_T.XbsResmaskBw.Add(m_fr_4);
                                        m_fr_4.Close();
                                        m_fr_4.Dispose();

                                    }
                                    XbsYXbsResmaskBw_.Close();
                                    XbsYXbsResmaskBw_.Dispose();


                                    YXbsResStLevelsSpect XbsYXbsLevelSpecrum_ = new YXbsResStLevelsSpect();
                                    XbsYXbsLevelSpecrum_.Format("*");
                                    XbsYXbsLevelSpecrum_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsLevelSpecrum_.Order = "[ID] ASC";
                                    for (XbsYXbsLevelSpecrum_.OpenRs(); !XbsYXbsLevelSpecrum_.IsEOF(); XbsYXbsLevelSpecrum_.MoveNext())
                                    {
                                        var m_fr_5 = new YXbsResStLevelsSpect();
                                        m_fr_5.CopyDataFrom(XbsYXbsLevelSpecrum_);
                                        ICSM_T.XbsLevelSpecrum.Add(m_fr_5);
                                        m_fr_5.Close();
                                        m_fr_5.Dispose();
                                    }
                                    XbsYXbsLevelSpecrum_.Close();
                                    XbsYXbsLevelSpecrum_.Dispose();

                                }
                                XbsYXbsResGeneral_.Close();
                                XbsYXbsResGeneral_.Dispose();


                            }
                            XbsYXbsResmeasstation_.Close();
                            XbsYXbsResmeasstation_.Dispose();


                            YXbsResLevels XbsYXbsLevelmeasres_ = new YXbsResLevels();
                            XbsYXbsLevelmeasres_.Format("*");
                            XbsYXbsLevelmeasres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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
                            XbsYXbsLevelmeasonlres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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

        public List<ClassSDRResults> ReadlAllResultFromDB(MeasurementType measurementType)
        {
            const int MaxExecuteParameters = 800;
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
                        res_val.Filter = string.Format("(ID>0) AND (STATUS<>'Z') AND (TYPEMEASUREMENTS='{0}')", measurementType.ToString());
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

                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;

                            /////
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


                                YXbsResStLevelCar XbsYXbsResLevelMeas_ = new YXbsResStLevelCar();
                                XbsYXbsResLevelMeas_.Format("*");
                                XbsYXbsResLevelMeas_.Filter = string.Format("(XBS_RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResLevelMeas_.Order = "[ID] ASC";
                                for (XbsYXbsResLevelMeas_.OpenRs(); !XbsYXbsResLevelMeas_.IsEOF(); XbsYXbsResLevelMeas_.MoveNext())
                                {
                                    var m_fr_2 = new YXbsResStLevelCar();
                                    m_fr_2.CopyDataFrom(XbsYXbsResLevelMeas_);
                                    ICSM_T.XbsResLevelMeas.Add(m_fr_2);
                                    m_fr_2.Close();
                                    m_fr_2.Dispose();
                                }
                                XbsYXbsResLevelMeas_.Close();
                                XbsYXbsResLevelMeas_.Dispose();


                                YXbsResStGeneral XbsYXbsResGeneral_ = new YXbsResStGeneral();
                                XbsYXbsResGeneral_.Format("*");
                                XbsYXbsResGeneral_.Filter = string.Format("(RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResGeneral_.Order = "[ID] ASC";
                                for (XbsYXbsResGeneral_.OpenRs(); !XbsYXbsResGeneral_.IsEOF(); XbsYXbsResGeneral_.MoveNext())
                                {
                                    var m_fr_3 = new YXbsResStGeneral();
                                    m_fr_3.CopyDataFrom(XbsYXbsResGeneral_);
                                    ICSM_T.XbsResGeneral.Add(m_fr_3);
                                    m_fr_3.Close();
                                    m_fr_3.Dispose();


                                    YXbsResStMaskElm XbsYXbsResmaskBw_ = new YXbsResStMaskElm();
                                    XbsYXbsResmaskBw_.Format("*");
                                    XbsYXbsResmaskBw_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsResmaskBw_.Order = "[ID] ASC";
                                    for (XbsYXbsResmaskBw_.OpenRs(); !XbsYXbsResmaskBw_.IsEOF(); XbsYXbsResmaskBw_.MoveNext())
                                    {
                                        var m_fr_4 = new YXbsResStMaskElm();
                                        m_fr_4.CopyDataFrom(XbsYXbsResmaskBw_);
                                        ICSM_T.XbsResmaskBw.Add(m_fr_4);
                                        m_fr_4.Close();
                                        m_fr_4.Dispose();

                                    }
                                    XbsYXbsResmaskBw_.Close();
                                    XbsYXbsResmaskBw_.Dispose();


                                    YXbsResStLevelsSpect XbsYXbsLevelSpecrum_ = new YXbsResStLevelsSpect();
                                    XbsYXbsLevelSpecrum_.Format("*");
                                    XbsYXbsLevelSpecrum_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsLevelSpecrum_.Order = "[ID] ASC";
                                    for (XbsYXbsLevelSpecrum_.OpenRs(); !XbsYXbsLevelSpecrum_.IsEOF(); XbsYXbsLevelSpecrum_.MoveNext())
                                    {
                                        var m_fr_5 = new YXbsResStLevelsSpect();
                                        m_fr_5.CopyDataFrom(XbsYXbsLevelSpecrum_);
                                        ICSM_T.XbsLevelSpecrum.Add(m_fr_5);
                                        m_fr_5.Close();
                                        m_fr_5.Dispose();
                                    }
                                    XbsYXbsLevelSpecrum_.Close();
                                    XbsYXbsLevelSpecrum_.Dispose();

                                }
                                XbsYXbsResGeneral_.Close();
                                XbsYXbsResGeneral_.Dispose();


                            }
                            XbsYXbsResmeasstation_.Close();
                            XbsYXbsResmeasstation_.Dispose();



                            YXbsResLevels XbsYXbsLevelmeasres_ = new YXbsResLevels();
                            XbsYXbsLevelmeasres_.Format("*");
                            XbsYXbsLevelmeasres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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
                            XbsYXbsLevelmeasonlres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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

        public List<ClassSDRResults> ReadlAllResultFromDB()
        {
            const int MaxExecuteParameters = 800;
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
                        res_val.Filter = "(ID>0) AND (STATUS<>'Z')";
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

                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;

                            /////
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


                                YXbsResStLevelCar XbsYXbsResLevelMeas_ = new YXbsResStLevelCar();
                                XbsYXbsResLevelMeas_.Format("*");
                                XbsYXbsResLevelMeas_.Filter = string.Format("(XBS_RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResLevelMeas_.Order = "[ID] ASC";
                                for (XbsYXbsResLevelMeas_.OpenRs(); !XbsYXbsResLevelMeas_.IsEOF(); XbsYXbsResLevelMeas_.MoveNext())
                                {
                                    var m_fr_2 = new YXbsResStLevelCar();
                                    m_fr_2.CopyDataFrom(XbsYXbsResLevelMeas_);
                                    ICSM_T.XbsResLevelMeas.Add(m_fr_2);
                                    m_fr_2.Close();
                                    m_fr_2.Dispose();
                                }
                                XbsYXbsResLevelMeas_.Close();
                                XbsYXbsResLevelMeas_.Dispose();


                                YXbsResStGeneral XbsYXbsResGeneral_ = new YXbsResStGeneral();
                                XbsYXbsResGeneral_.Format("*");
                                XbsYXbsResGeneral_.Filter = string.Format("(RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResGeneral_.Order = "[ID] ASC";
                                for (XbsYXbsResGeneral_.OpenRs(); !XbsYXbsResGeneral_.IsEOF(); XbsYXbsResGeneral_.MoveNext())
                                {
                                    var m_fr_3 = new YXbsResStGeneral();
                                    m_fr_3.CopyDataFrom(XbsYXbsResGeneral_);
                                    ICSM_T.XbsResGeneral.Add(m_fr_3);
                                    m_fr_3.Close();
                                    m_fr_3.Dispose();


                                    YXbsResStMaskElm XbsYXbsResmaskBw_ = new YXbsResStMaskElm();
                                    XbsYXbsResmaskBw_.Format("*");
                                    XbsYXbsResmaskBw_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsResmaskBw_.Order = "[ID] ASC";
                                    for (XbsYXbsResmaskBw_.OpenRs(); !XbsYXbsResmaskBw_.IsEOF(); XbsYXbsResmaskBw_.MoveNext())
                                    {
                                        var m_fr_4 = new YXbsResStMaskElm();
                                        m_fr_4.CopyDataFrom(XbsYXbsResmaskBw_);
                                        ICSM_T.XbsResmaskBw.Add(m_fr_4);
                                        m_fr_4.Close();
                                        m_fr_4.Dispose();

                                    }
                                    XbsYXbsResmaskBw_.Close();
                                    XbsYXbsResmaskBw_.Dispose();


                                    YXbsResStLevelsSpect XbsYXbsLevelSpecrum_ = new YXbsResStLevelsSpect();
                                    XbsYXbsLevelSpecrum_.Format("*");
                                    XbsYXbsLevelSpecrum_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsLevelSpecrum_.Order = "[ID] ASC";
                                    for (XbsYXbsLevelSpecrum_.OpenRs(); !XbsYXbsLevelSpecrum_.IsEOF(); XbsYXbsLevelSpecrum_.MoveNext())
                                    {
                                        var m_fr_5 = new YXbsResStLevelsSpect();
                                        m_fr_5.CopyDataFrom(XbsYXbsLevelSpecrum_);
                                        ICSM_T.XbsLevelSpecrum.Add(m_fr_5);
                                        m_fr_5.Close();
                                        m_fr_5.Dispose();
                                    }
                                    XbsYXbsLevelSpecrum_.Close();
                                    XbsYXbsLevelSpecrum_.Dispose();

                                }
                                XbsYXbsResGeneral_.Close();
                                XbsYXbsResGeneral_.Dispose();


                            }
                            XbsYXbsResmeasstation_.Close();
                            XbsYXbsResmeasstation_.Dispose();



                            YXbsResLevels XbsYXbsLevelmeasres_ = new YXbsResLevels();
                            XbsYXbsLevelmeasres_.Format("*");
                            XbsYXbsLevelmeasres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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
                            XbsYXbsLevelmeasonlres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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


        public List<ClassSDRResults> ReadResultFromDBTask(int MeasTaskId)
        {
            const int MaxExecuteParameters = 800;
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
                        res_val.Filter = string.Format("(MEASTASKID='{0}') AND (STATUS<>'Z')", MeasTaskId);
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
                            var m_fr = new YXbsResMeas();
                            m_fr.CopyDataFrom(res_val);
                            ICSM_T.meas_res = m_fr;

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


                                YXbsResStLevelCar XbsYXbsResLevelMeas_ = new YXbsResStLevelCar();
                                XbsYXbsResLevelMeas_.Format("*");
                                XbsYXbsResLevelMeas_.Filter = string.Format("(XBS_RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResLevelMeas_.Order = "[ID] ASC";
                                for (XbsYXbsResLevelMeas_.OpenRs(); !XbsYXbsResLevelMeas_.IsEOF(); XbsYXbsResLevelMeas_.MoveNext())
                                {
                                    var m_fr_2 = new YXbsResStLevelCar();
                                    m_fr_2.CopyDataFrom(XbsYXbsResLevelMeas_);
                                    ICSM_T.XbsResLevelMeas.Add(m_fr_2);
                                    m_fr_2.Close();
                                    m_fr_2.Dispose();
                                }
                                XbsYXbsResLevelMeas_.Close();
                                XbsYXbsResLevelMeas_.Dispose();


                                YXbsResStGeneral XbsYXbsResGeneral_ = new YXbsResStGeneral();
                                XbsYXbsResGeneral_.Format("*");
                                XbsYXbsResGeneral_.Filter = string.Format("(RESMEASSTATIONID={0})", XbsYXbsResmeasstation_.m_id);
                                XbsYXbsResGeneral_.Order = "[ID] ASC";
                                for (XbsYXbsResGeneral_.OpenRs(); !XbsYXbsResGeneral_.IsEOF(); XbsYXbsResGeneral_.MoveNext())
                                {
                                    var m_fr_3 = new YXbsResStGeneral();
                                    m_fr_3.CopyDataFrom(XbsYXbsResGeneral_);
                                    ICSM_T.XbsResGeneral.Add(m_fr_3);
                                    m_fr_3.Close();
                                    m_fr_3.Dispose();


                                    YXbsResStMaskElm XbsYXbsResmaskBw_ = new YXbsResStMaskElm();
                                    XbsYXbsResmaskBw_.Format("*");
                                    XbsYXbsResmaskBw_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsResmaskBw_.Order = "[ID] ASC";
                                    for (XbsYXbsResmaskBw_.OpenRs(); !XbsYXbsResmaskBw_.IsEOF(); XbsYXbsResmaskBw_.MoveNext())
                                    {
                                        var m_fr_4 = new YXbsResStMaskElm();
                                        m_fr_4.CopyDataFrom(XbsYXbsResmaskBw_);
                                        ICSM_T.XbsResmaskBw.Add(m_fr_4);
                                        m_fr_4.Close();
                                        m_fr_4.Dispose();

                                    }
                                    XbsYXbsResmaskBw_.Close();
                                    XbsYXbsResmaskBw_.Dispose();


                                    YXbsResStLevelsSpect XbsYXbsLevelSpecrum_ = new YXbsResStLevelsSpect();
                                    XbsYXbsLevelSpecrum_.Format("*");
                                    XbsYXbsLevelSpecrum_.Filter = string.Format("(XBS_RESSTGENERALID={0})", XbsYXbsResGeneral_.m_id);
                                    XbsYXbsLevelSpecrum_.Order = "[ID] ASC";
                                    for (XbsYXbsLevelSpecrum_.OpenRs(); !XbsYXbsLevelSpecrum_.IsEOF(); XbsYXbsLevelSpecrum_.MoveNext())
                                    {
                                        var m_fr_5 = new YXbsResStLevelsSpect();
                                        m_fr_5.CopyDataFrom(XbsYXbsLevelSpecrum_);
                                        ICSM_T.XbsLevelSpecrum.Add(m_fr_5);
                                        m_fr_5.Close();
                                        m_fr_5.Dispose();
                                    }
                                    XbsYXbsLevelSpecrum_.Close();
                                    XbsYXbsLevelSpecrum_.Dispose();

                                }
                                XbsYXbsResGeneral_.Close();
                                XbsYXbsResGeneral_.Dispose();


                            }
                            XbsYXbsResmeasstation_.Close();
                            XbsYXbsResmeasstation_.Dispose();

                            YXbsResLevels XbsYXbsLevelmeasres_ = new YXbsResLevels();
                            XbsYXbsLevelmeasres_.Format("*");
                            XbsYXbsLevelmeasres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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
                            XbsYXbsLevelmeasonlres_.Filter = string.Format("(XBSRESMEASID={0})", res_val.m_id);
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


        public bool DeleteResultFromDB(MeasurementResultsIdentifier obj, string Status)
        {
            bool isSuccess = false;
            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
                Yyy yyy = new Yyy();
                DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                if (dbConnect.State == System.Data.ConnectionState.Open)
                {
                    DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    try
                    {
                        logger.Trace("Start procedure DeleteResultFromDB...");
                        YXbsResMeas measRes = new YXbsResMeas();
                        measRes.Format("*");
                        if (obj != null)
                        {
                            string sql = "";
                            if (obj.MeasTaskId != null) sql += string.Format("(MEASTASKID ='{0}')", obj.MeasTaskId.Value.ToString());
                            if (obj.MeasSdrResultsId > 0) sql += sql.Length > 0 ? string.Format(" AND (ID ={0})", obj.MeasSdrResultsId) : string.Format("(ID ={0})", obj.MeasSdrResultsId);
                            if (obj.SubMeasTaskId > 0) sql += sql.Length > 0 ? string.Format(" AND (SUBMEASTASKID ={0})", obj.SubMeasTaskId) : string.Format("(SUBMEASTASKID ={0})", obj.SubMeasTaskId);
                            if (obj.SubMeasTaskStationId > 0) sql += sql.Length > 0 ? string.Format(" AND (SUBMEASTASKSTATIONID ={0})", obj.SubMeasTaskStationId) : string.Format("(SUBMEASTASKSTATIONID ={0})", obj.SubMeasTaskStationId);
                            if (sql.Length > 0)
                            {
                                measRes.Filter = sql;
                                for (measRes.OpenRs(); !measRes.IsEOF(); measRes.MoveNext())
                                {
                                    measRes.m_status = Status;
                                    measRes.Save(dbConnect, transaction);
                                    isSuccess = true;
                                }
                            }
                            measRes.Close();
                            measRes.Dispose();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception e) { transaction.Dispose(); dbConnect.Close(); dbConnect.Dispose(); logger.Error(e.Message); }
                        logger.Error("Error in procedure DeleteResultFromDB: " + ex.Message);
                        isSuccess = false;
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
            });
            tsk.Start();
            tsk.Join();
            logger.Trace("End procedure DeleteResultFromDB.");
            return isSuccess;
        }
        public bool DeleteResultFromDB(MeasurementResults obj)
        {
            bool isSuccess = false;


            System.Threading.Thread tsk = new System.Threading.Thread(() =>
            {
                logger.Trace("Start procedure DeleteResultFromDB...");
                Yyy yyy = new Yyy();
                DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                if (dbConnect.State == System.Data.ConnectionState.Open)
                {
                    DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    try
                    {
                        YXbsResMeas measRes = new YXbsResMeas();
                        measRes.Format("*");
                        if (obj != null)
                        {
                            if ((obj.Id.MeasTaskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != Constants.NullI) && (obj.Id.SubMeasTaskStationId != Constants.NullI))
                            {
                                if (obj.StationMeasurements.StationId != null)
                                {
                                    if (measRes.Fetch(string.Format(" (MEASTASKID='{0}') and (SENSORID={1}) and (SUBMEASTASKID={2}) and (SUBMEASTASKSTATIONID={3})", obj.Id.MeasTaskId.Value.ToString(), obj.StationMeasurements.StationId.Value, obj.Id.SubMeasTaskId, obj.Id.SubMeasTaskStationId)))
                                    {
                                        foreach (FrequencyMeasurement dt_param in obj.FrequenciesMeasurements.ToArray())
                                        {
                                            YXbsResLevels dtr = new YXbsResLevels();
                                            dtr.Format("*");
                                            if (dt_param != null)
                                            {
                                                if (dtr.Fetch(string.Format("XBSRESMEASID={0}", measRes.m_id)))
                                                {
                                                    dtr.Delete(dbConnect, transaction);
                                                }
                                            }
                                            dtr.Close();
                                            dtr.Dispose();
                                        }
                                        foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray())
                                        {
                                            YXbsResLocSensorMeas dtr = new YXbsResLocSensorMeas();
                                            dtr.Format("*");
                                            if (dt_param != null)
                                            {
                                                if (dtr.Fetch(string.Format("XBSRESMEASID={0}", measRes.m_id)))
                                                {
                                                    dtr.Delete(dbConnect, transaction);
                                                }
                                            }
                                            dtr.Close();
                                            dtr.Dispose();
                                        }
                                        foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray())
                                        {
                                            if (obj.TypeMeasurements == MeasurementType.Level)
                                            {

                                                YXbsResLevels dtr = new YXbsResLevels();
                                                dtr.Format("*");
                                                if (dt_param != null)
                                                {
                                                    if (dtr.Fetch(string.Format("XBSRESMEASID={0}", measRes.m_id)))
                                                    {
                                                        dtr.Delete(dbConnect, transaction);
                                                    }
                                                }
                                                dtr.Close();
                                                dtr.Dispose();

                                            }
                                            else if (obj.TypeMeasurements == MeasurementType.SpectrumOccupation)
                                            {
                                                YXbsResLevels dtr = new YXbsResLevels();
                                                dtr.Format("*");
                                                if (dt_param != null)
                                                {
                                                    if (dtr.Fetch(string.Format("XBSRESMEASID={0}", measRes.m_id)))
                                                    {
                                                        dtr.Delete(dbConnect, transaction);
                                                    }
                                                }
                                                dtr.Close();
                                                dtr.Dispose();
                                            }
                                        }
                                        isSuccess = true;
                                        measRes.Delete(dbConnect, transaction);
                                    }
                                }
                            }
                            else if (obj.Id.MeasTaskId != null)
                            {
                                {
                                    measRes.Filter = string.Format(" (MEASTASKID='{0}') ", obj.Id.MeasTaskId.Value.ToString());
                                    for (measRes.OpenRs(); !measRes.IsEOF(); measRes.MoveNext())
                                    {
                                        foreach (FrequencyMeasurement dt_param in obj.FrequenciesMeasurements.ToArray())
                                        {
                                            YXbsResLevels dtr = new YXbsResLevels();
                                            dtr.Format("*");
                                            if (dt_param != null)
                                            {
                                                if (dtr.Fetch(string.Format("XBSRESMEASID={0}", measRes.m_id)))
                                                {
                                                    dtr.Delete(dbConnect, transaction);
                                                }
                                            }
                                            dtr.Close();
                                            dtr.Dispose();
                                        }

                                        foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray())
                                        {
                                            YXbsResLocSensorMeas dtr = new YXbsResLocSensorMeas();
                                            dtr.Format("*");
                                            if (dt_param != null)
                                            {
                                                dtr.Filter = string.Format("XBSRESMEASID={0}", measRes.m_id);
                                                for (dtr.OpenRs(); !dtr.IsEOF(); dtr.MoveNext())
                                                {
                                                    dtr.Delete(dbConnect, transaction);
                                                }
                                            }
                                            dtr.Close();
                                            dtr.Dispose();
                                        }

                                        foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray())
                                        {
                                            if (obj.TypeMeasurements == MeasurementType.Level)
                                            {
                                                YXbsResLevels dtr = new YXbsResLevels();
                                                dtr.Format("*");
                                                if (dt_param != null)
                                                {
                                                    if (dtr.Fetch(string.Format("XBSRESMEASID={0}", measRes.m_id)))
                                                    {
                                                        dtr.Delete(dbConnect, transaction);
                                                    }
                                                }
                                                dtr.Close();
                                                dtr.Dispose();

                                            }
                                            else if (obj.TypeMeasurements == MeasurementType.SpectrumOccupation)
                                            {
                                                YXbsResLevels dtr = new YXbsResLevels();
                                                dtr.Format("*");
                                                if (dt_param != null)
                                                {
                                                    if (dtr.Fetch(string.Format("XBSRESMEASID={0}", measRes.m_id)))
                                                    {
                                                        dtr.Delete(dbConnect, transaction);
                                                    }
                                                }
                                                dtr.Close();
                                                dtr.Dispose();
                                            }
                                        }
                                        isSuccess = true;
                                        measRes.Delete(dbConnect, transaction);
                                    }
                                }
                            }
                            measRes.Close();
                            measRes.Dispose();
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception e) { transaction.Dispose(); dbConnect.Close(); dbConnect.Dispose(); logger.Error(e.Message); }
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

            });
            tsk.Start();
            tsk.Join();
            logger.Trace("End procedure DeleteResultFromDB.");
            return isSuccess;
        }


        public int? SaveResultToDB(MeasurementResults obj)
        {
            //obj.ResultsMeasStation
            int? ID = Constants.NullI;
            //if (((obj.TypeMeasurements == MeasurementType.SpectrumOccupation) && (obj.Status == "C")) || (obj.TypeMeasurements != MeasurementType.SpectrumOccupation))
            {
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    Yyy yyy = new Yyy();
                    DbConnection dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                    if (dbConnect.State == System.Data.ConnectionState.Open)
                    {
                        DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                        try
                        {
                            logger.Trace("Start procedure SaveResultToDB.");
                            List<Yyy> BlockInsert_YXbsLevelmeasres1 = new List<Yyy>();
                            /// Create new record in YXbsMeastask
                            if (obj != null)
                            {
                                if ((obj.Id.MeasTaskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != Constants.NullI) && (obj.Id.SubMeasTaskStationId != Constants.NullI))
                                {
                                    if (obj.StationMeasurements.StationId != null)
                                    {
                                        YXbsResMeas measRes = new YXbsResMeas();
                                        measRes.Format("*");
                                        measRes.Filter = "ID=-1";
                                        measRes.New();
                                        if (obj.AntVal != null) measRes.m_antval = obj.AntVal.GetValueOrDefault();
                                        if (obj.DataRank != null) measRes.m_datarank = obj.DataRank.GetValueOrDefault();
                                        measRes.m_status = obj.Status;
                                        measRes.m_meastaskid = obj.Id.MeasTaskId.Value.ToString();
                                        if (obj.N != null) measRes.m_n = obj.N.GetValueOrDefault();
                                        measRes.m_sensorid = obj.StationMeasurements.StationId.Value;
                                        measRes.m_submeastaskid = obj.Id.SubMeasTaskId;
                                        measRes.m_submeastaskstationid = obj.Id.SubMeasTaskStationId;
                                        measRes.m_timemeas = obj.TimeMeas;
                                        measRes.m_typemeasurements = obj.TypeMeasurements.ToString();
                                        ID = measRes.Save(dbConnect, transaction);
                                        obj.Id.MeasSdrResultsId = ID.Value;
                                        measRes.Close();
                                        measRes.Dispose();
                                    }
                                }
                                if (ID != Constants.NullI)
                                {
                                    if (obj.ResultsMeasStation != null)
                                    {
                                        foreach (ResultsMeasurementsStation station in obj.ResultsMeasStation)
                                        {

                                            YXbsResmeasstation measResStation = new YXbsResmeasstation();
                                            measResStation.Format("*");
                                            measResStation.Filter = "ID=-1";
                                            measResStation.New();
                                            measResStation.m_globalsid = station.GlobalSID;
                                            measResStation.m_idsector = station.IdSector;
                                            measResStation.m_idstation = station.Idstation;
                                            measResStation.m_status = station.Status;
                                            measResStation.m_measglobalsid = station.MeasGlobalSID;
                                            measResStation.m_xbsresmeasid = ID;
                                            int? IDStation = measResStation.Save(dbConnect, transaction);
                                            measResStation.Close();
                                            measResStation.Dispose();
                                            if (IDStation > 0)
                                            {
                                                //правки от 20.09.2018
                                                YXbsLinkResSensor linkResSensor = new YXbsLinkResSensor();
                                                linkResSensor.Format("*");
                                                linkResSensor.Filter = "ID=-1";
                                                linkResSensor.New();
                                                linkResSensor.m_id_xbs_sensor = obj.StationMeasurements.StationId.Value;
                                                linkResSensor.m_idxbsresmeassta = IDStation;
                                                int? IDXbsLinkResSensor = linkResSensor.Save(dbConnect, transaction);
                                                linkResSensor.Close();
                                                linkResSensor.Dispose();

                                                if (station.GeneralResult != null)
                                                {
                                                    YXbsResStGeneral measResGeneral = new YXbsResStGeneral();
                                                    measResGeneral.Format("*");
                                                    measResGeneral.Filter = "ID=-1";
                                                    measResGeneral.New();
                                                    measResGeneral.m_centralfrequency = station.GeneralResult.CentralFrequency;
                                                    measResGeneral.m_centralfrequencymeas = station.GeneralResult.CentralFrequencyMeas;
                                                    measResGeneral.m_durationmeas = station.GeneralResult.DurationMeas;
                                                    measResGeneral.m_markerindex = station.GeneralResult.MarkerIndex;
                                                    measResGeneral.m_offsetfrequency = station.GeneralResult.OffsetFrequency;
                                                    measResGeneral.m_specrumstartfreq = (double?)station.GeneralResult.SpecrumStartFreq;
                                                    measResGeneral.m_specrumsteps = (double?)station.GeneralResult.SpecrumSteps;
                                                    measResGeneral.m_t1 = station.GeneralResult.T1;
                                                    measResGeneral.m_t2 = station.GeneralResult.T2;
                                                    measResGeneral.m_timefinishmeas = station.GeneralResult.TimeFinishMeas;
                                                    measResGeneral.m_timestartmeasdate = station.GeneralResult.TimeStartMeas;
                                                    measResGeneral.m_resmeasstationid = IDStation;
                                                    int? IDResGeneral = measResGeneral.Save(dbConnect, transaction);
                                                    measResGeneral.Close();
                                                    measResGeneral.Dispose();

                                                    if (IDResGeneral > 0)
                                                    {
                                                        if (station.GeneralResult.MaskBW != null)
                                                        {
                                                            if (station.GeneralResult.MaskBW.Length > 0)
                                                            {
                                                                foreach (MaskElements mslel in station.GeneralResult.MaskBW)
                                                                {
                                                                    YXbsResStMaskElm resmaskBw = new YXbsResStMaskElm();
                                                                    resmaskBw.Format("*");
                                                                    resmaskBw.Filter = "ID=-1";
                                                                    resmaskBw.New();
                                                                    resmaskBw.m_bw = mslel.BW;
                                                                    resmaskBw.m_level = mslel.level;
                                                                    resmaskBw.m_xbs_resstgeneralid = IDResGeneral;
                                                                    int? IDYXbsResmaskBw = resmaskBw.Save(dbConnect, transaction);
                                                                    resmaskBw.Close();
                                                                    resmaskBw.Dispose();
                                                                }
                                                            }
                                                        }


                                                        if (station.GeneralResult.LevelsSpecrum != null)
                                                        {
                                                            if (station.GeneralResult.LevelsSpecrum.Length > 0)
                                                            {
                                                                foreach (float lvl in station.GeneralResult.LevelsSpecrum)
                                                                {
                                                                    YXbsResStLevelsSpect reslevelSpecrum = new YXbsResStLevelsSpect();
                                                                    reslevelSpecrum.Format("*");
                                                                    reslevelSpecrum.Filter = "ID=-1";
                                                                    reslevelSpecrum.New();
                                                                    reslevelSpecrum.m_levelspecrum = lvl;
                                                                    reslevelSpecrum.m_xbs_resstgeneralid = IDResGeneral;
                                                                    int? IDreslevelSpecrum = reslevelSpecrum.Save(dbConnect, transaction);
                                                                    reslevelSpecrum.Close();
                                                                    reslevelSpecrum.Dispose();
                                                                }
                                                            }
                                                        }

                                                    }

                                                    if (station.LevelMeasurements != null)
                                                    {
                                                        if (station.LevelMeasurements.Length > 0)
                                                        {
                                                            foreach (LevelMeasurementsCar car in station.LevelMeasurements)
                                                            {
                                                                YXbsResStLevelCar yXbsResLevelMeas = new YXbsResStLevelCar();
                                                                yXbsResLevelMeas.Format("*");
                                                                yXbsResLevelMeas.Filter = "ID=-1";
                                                                yXbsResLevelMeas.New();
                                                                yXbsResLevelMeas.m_altitude = car.Altitude;
                                                                yXbsResLevelMeas.m_bw = car.BW;
                                                                yXbsResLevelMeas.m_centralfrequency = (double?)car.CentralFrequency;
                                                                yXbsResLevelMeas.m_differencetimestamp = car.DifferenceTimestamp;
                                                                yXbsResLevelMeas.m_lat = car.Lat;
                                                                yXbsResLevelMeas.m_leveldbm = car.LeveldBm;
                                                                yXbsResLevelMeas.m_leveldbmkvm = car.LeveldBmkVm;
                                                                yXbsResLevelMeas.m_lon = car.Lon;
                                                                yXbsResLevelMeas.m_rbw = car.RBW;
                                                                yXbsResLevelMeas.m_timeofmeasurements = car.TimeOfMeasurements;
                                                                yXbsResLevelMeas.m_vbw = car.VBW;
                                                                yXbsResLevelMeas.m_xbs_resmeasstationid = IDStation;
                                                                int? IDyXbsResLevelMeas = yXbsResLevelMeas.Save(dbConnect, transaction);
                                                                yXbsResLevelMeas.Close();
                                                                yXbsResLevelMeas.Dispose();
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }

                                    if (obj.LocationSensorMeasurement != null)
                                    {
                                        List<Yyy> BlockInsert_LocationSensorMeasurement = new List<Yyy>();
                                        foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray())
                                        {
                                            if (dt_param != null)
                                            {
                                                YXbsResLocSensorMeas dtr = new YXbsResLocSensorMeas();
                                                dtr.Format("*");
                                                dtr.Filter = "ID=-1";
                                                dtr.New();
                                                if (dt_param.ASL != null) dtr.m_asl = dt_param.ASL.GetValueOrDefault();
                                                if (dt_param.Lon != null) dtr.m_lon = dt_param.Lon.GetValueOrDefault();
                                                if (dt_param.Lat != null) dtr.m_lat = dt_param.Lat.GetValueOrDefault();
                                                dtr.m_xbsresmeasid = ID;
                                                for (int i = 0; i < dtr.getAllFields.Count; i++)
                                                    dtr.getAllFields[i].Value = dtr.valc[i];
                                                BlockInsert_LocationSensorMeasurement.Add(dtr);
                                                dtr.Close();
                                                dtr.Dispose();
                                            }
                                        }
                                        if (BlockInsert_LocationSensorMeasurement.Count > 0)
                                        {
                                            YXbsResLocSensorMeas YXbsLocationsensorm11 = new YXbsResLocSensorMeas();
                                            YXbsLocationsensorm11.Format("*");
                                            YXbsLocationsensorm11.New();
                                            YXbsLocationsensorm11.SaveBath(BlockInsert_LocationSensorMeasurement, dbConnect, transaction);
                                            YXbsLocationsensorm11.Close();
                                            YXbsLocationsensorm11.Dispose();
                                        }
                                    }
                                    int AllIdx = 0;
                                    if (obj.MeasurementsResults != null)
                                    {
                                        int idx_cnt = 0;
                                        YXbsResLevels d_level = new YXbsResLevels();
                                        d_level.Format("*");
                                        //int? indexerYXbsLevelmeasres = d_level.GetNextId(d_level.GetTableName() + "_SEQ");
                                        foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray())
                                        {
                                            if ((obj.TypeMeasurements == MeasurementType.Level) || (obj.TypeMeasurements == MeasurementType.SpectrumOccupation))
                                            {
                                                YXbsResLevels dtrR = new YXbsResLevels();
                                                dtrR.Format("*");
                                                dtrR.Filter = "ID=-1";
                                                dtrR.New();

                                                if ((obj.TypeMeasurements == MeasurementType.Level) && (obj.Status != "O"))
                                                {
                                                    if (dt_param != null)
                                                    {
                                                        if (dt_param is LevelMeasurementResult)
                                                        {

                                                            if ((dt_param as LevelMeasurementResult).Value != null) dtrR.m_valuelvl = (dt_param as LevelMeasurementResult).Value.GetValueOrDefault();
                                                            if ((dt_param as LevelMeasurementResult).PMax != null) dtrR.m_pmaxlvl = (dt_param as LevelMeasurementResult).PMax.GetValueOrDefault();
                                                            if ((dt_param as LevelMeasurementResult).PMin != null) dtrR.m_pminlvl = (dt_param as LevelMeasurementResult).PMin.GetValueOrDefault();

                                                            if (obj.FrequenciesMeasurements != null)
                                                            {
                                                                List<FrequencyMeasurement> Fr_e = obj.FrequenciesMeasurements.ToList().FindAll(t => t.Id == dt_param.Id.Value);
                                                                if (Fr_e != null)
                                                                {
                                                                    if (Fr_e.Count > 0)
                                                                    {
                                                                        foreach (FrequencyMeasurement dt_param_freq in Fr_e.ToArray())
                                                                        {
                                                                            if (dt_param_freq != null)
                                                                            {
                                                                                dtrR.m_freqmeas = dt_param_freq.Freq;
                                                                                //dtrR.m_nummeas = indexerYXbsLevelmeasres;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if ((obj.TypeMeasurements == MeasurementType.SpectrumOccupation) /*&& (obj.Status == "C")*/)
                                                {
                                                    if (dt_param != null)
                                                    {
                                                        if (dt_param is SpectrumOccupationMeasurementResult)
                                                        {
                                                            if ((dt_param as SpectrumOccupationMeasurementResult).Value != null) dtrR.m_valuespect = (dt_param as SpectrumOccupationMeasurementResult).Value.GetValueOrDefault();
                                                            if ((dt_param as SpectrumOccupationMeasurementResult).Occupancy != null) dtrR.m_occupancyspect = (dt_param as SpectrumOccupationMeasurementResult).Occupancy.GetValueOrDefault();

                                                            if (obj.FrequenciesMeasurements != null)
                                                            {
                                                                List<FrequencyMeasurement> Fr_e = obj.FrequenciesMeasurements.ToList().FindAll(t => t.Id == dt_param.Id.Value);
                                                                if (Fr_e != null)
                                                                {
                                                                    if (Fr_e.Count > 0)
                                                                    {
                                                                        foreach (FrequencyMeasurement dt_param_freq in Fr_e.ToArray())
                                                                        {
                                                                            if (dt_param_freq != null)
                                                                            {
                                                                                dtrR.m_freqmeas = dt_param_freq.Freq;
                                                                                //dtrR.m_nummeas = indexerYXbsLevelmeasres;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                dtrR.m_xbsresmeasid = ID;
                                                for (int i = 0; i < dtrR.getAllFields.Count; i++)
                                                    dtrR.getAllFields[i].Value = dtrR.valc[i];
                                                BlockInsert_YXbsLevelmeasres1.Add(dtrR);
                                                dtrR.Close();
                                                dtrR.Dispose();
                                                //++indexerYXbsLevelmeasres;
                                            }

                                            if (dt_param != null)
                                            {
                                                if (dt_param is LevelMeasurementOnlineResult)
                                                {
                                                    YXbsResLevmeasonline dtr = new YXbsResLevmeasonline();
                                                    dtr.Format("*");
                                                    dtr.Filter = "ID=-1";
                                                    dtr.New();
                                                    if ((dt_param as LevelMeasurementOnlineResult).Value != Constants.NullD) dtr.m_value = (dt_param as LevelMeasurementOnlineResult).Value;
                                                    dtr.m_xbsresmeasid = ID;
                                                    int? ID_DT_params = dtr.Save(dbConnect, transaction);
                                                    dt_param.Id = new MeasurementResultIdentifier();
                                                    dt_param.Id.Value = ID_DT_params.Value;
                                                    dtr.Close();
                                                    dtr.Dispose();
                                                }
                                            }

                                            idx_cnt++;

                                        }
                                    }
                                    if (BlockInsert_YXbsLevelmeasres1.Count > 0)
                                    {
                                        int iu = AllIdx;
                                        YXbsResLevels YXbsLevelmeasres11 = new YXbsResLevels();
                                        YXbsLevelmeasres11.Format("*");
                                        YXbsLevelmeasres11.New();
                                        YXbsLevelmeasres11.SaveBath(BlockInsert_YXbsLevelmeasres1, dbConnect, transaction);
                                        YXbsLevelmeasres11.Close();
                                        YXbsLevelmeasres11.Dispose();
                                    }

                                }
                            }
                            transaction.Commit();
                            logger.Trace("End procedure SaveResultToDB.");
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e) { transaction.Dispose(); dbConnect.Close(); dbConnect.Dispose(); logger.Error(e.Message); }
                            logger.Error("Error in procedure SaveResultToDB: " + ex.Message);
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
                });
                tsk.Start();
                tsk.Join();
            }
            return ID;
        }

    }
}
