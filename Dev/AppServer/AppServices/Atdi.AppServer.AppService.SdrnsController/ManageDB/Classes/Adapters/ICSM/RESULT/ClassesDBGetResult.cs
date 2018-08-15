using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Globalization;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer;
using Oracle.DataAccess.Client;

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


        public List<ClassSDRResults> ReadlResultFromDB(int ID)
        {
            // Список объектов в рамках конкретного адаптера ICSM
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadlResultFromDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    YXbsMeasurementres res_val = new YXbsMeasurementres();
                    res_val.Format("*");
                    // выбирать только таски, для которых STATUS не NULL
                    res_val.Filter = string.Format("(ID={0})", ID);
                    for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                    {
                        ClassSDRResults ICSM_T = new ClassSDRResults();
                        ICSM_T.freq_meas = new List<YXbsFrequencymeas>();
                        ICSM_T.level_meas_res = new List<YXbsLevelmeasres>();
                        ICSM_T.loc_sensorM = new List<YXbsLocationsensorm>();
                        ICSM_T.meas_res = new YXbsMeasurementres();
                        ICSM_T.spect_occup_meas = new List<YXbsSpectoccupmeas>();

                        ICSM_T.meas_res = new YXbsMeasurementres();
                        var m_fr = new YXbsMeasurementres();
                        m_fr.CopyDataFrom(res_val);
                        ICSM_T.meas_res = m_fr;

                        /*
                        YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas();
                        XbsYXbsFrequencymeas_.Format("*");
                        XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                        for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                        {
                            var m_fr_ = new YXbsFrequencymeas();
                            m_fr_.CopyDataFrom(XbsYXbsFrequencymeas_);
                            ICSM_T.freq_meas.Add(m_fr_);
                            m_fr_.Close();
                            m_fr_.Dispose();
                        }
                        XbsYXbsFrequencymeas_.Close();
                        XbsYXbsFrequencymeas_.Dispose();
                        */

                        YXbsLevelmeasres XbsYXbsLevelmeasres_ = new YXbsLevelmeasres();
                        XbsYXbsLevelmeasres_.Format("*");
                        XbsYXbsLevelmeasres_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0})", res_val.m_id);
                        for (XbsYXbsLevelmeasres_.OpenRs(); !XbsYXbsLevelmeasres_.IsEOF(); XbsYXbsLevelmeasres_.MoveNext())
                        {
                            var m_fr_ = new YXbsLevelmeasres();
                            m_fr_.CopyDataFrom(XbsYXbsLevelmeasres_);
                            ICSM_T.level_meas_res.Add(m_fr_);

                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas();
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0}) and (NUM={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas();
                                m_fr_f.CopyDataFrom(XbsYXbsFrequencymeas_);
                                ICSM_T.freq_meas.Add(m_fr_f);
                                m_fr_f.Close();
                                m_fr_f.Dispose();
                            }
                            XbsYXbsFrequencymeas_.Close();
                            XbsYXbsFrequencymeas_.Dispose();
                            /////////

                            m_fr_.Close();
                            m_fr_.Dispose();
                        }
                        XbsYXbsLevelmeasres_.Close();
                        XbsYXbsLevelmeasres_.Dispose();

                        YXbsLevelmeasonlres XbsYXbsLevelmeasonlres_ = new YXbsLevelmeasonlres();
                        XbsYXbsLevelmeasonlres_.Format("*");
                        XbsYXbsLevelmeasonlres_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0})", res_val.m_id);
                        for (XbsYXbsLevelmeasonlres_.OpenRs(); !XbsYXbsLevelmeasonlres_.IsEOF(); XbsYXbsLevelmeasonlres_.MoveNext())
                        {
                            var m_fr_ = new YXbsLevelmeasonlres();
                            m_fr_.CopyDataFrom(XbsYXbsLevelmeasonlres_);
                            ICSM_T.level_meas_onl_res.Add(m_fr_);

                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas();
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0}) and (NUM={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas();
                                m_fr_f.CopyDataFrom(XbsYXbsFrequencymeas_);
                                ICSM_T.freq_meas.Add(m_fr_f);
                                m_fr_f.Close();
                                m_fr_f.Dispose();
                            }
                            XbsYXbsFrequencymeas_.Close();
                            XbsYXbsFrequencymeas_.Dispose();
                            /////////

                            m_fr_.Close();
                            m_fr_.Dispose();
                        }
                        XbsYXbsLevelmeasonlres_.Close();
                        XbsYXbsLevelmeasonlres_.Dispose();

                        YXbsSpectoccupmeas XbsYXbsSpectoccupmeas_ = new YXbsSpectoccupmeas();
                        XbsYXbsSpectoccupmeas_.Format("*");
                        XbsYXbsSpectoccupmeas_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0})", res_val.m_id);
                        for (XbsYXbsSpectoccupmeas_.OpenRs(); !XbsYXbsSpectoccupmeas_.IsEOF(); XbsYXbsSpectoccupmeas_.MoveNext())
                        {
                            var m_fr_ = new YXbsSpectoccupmeas();
                            m_fr_.CopyDataFrom(XbsYXbsSpectoccupmeas_);
                            ICSM_T.spect_occup_meas.Add(m_fr_);

                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas();
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0}) and (NUM={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas();
                                m_fr_f.CopyDataFrom(XbsYXbsFrequencymeas_);
                                ICSM_T.freq_meas.Add(m_fr_f);
                                m_fr_f.Close();
                                m_fr_f.Dispose();
                            }
                            XbsYXbsFrequencymeas_.Close();
                            XbsYXbsFrequencymeas_.Dispose();
                            /////////

                            m_fr_.Close();
                            m_fr_.Dispose();
                        }
                        XbsYXbsSpectoccupmeas_.Close();
                        XbsYXbsSpectoccupmeas_.Dispose();

                        YXbsLocationsensorm XbsYXbsLocationsensorm_ = new YXbsLocationsensorm();
                        XbsYXbsLocationsensorm_.Format("*");
                        XbsYXbsLocationsensorm_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0})", res_val.m_id);
                        for (XbsYXbsLocationsensorm_.OpenRs(); !XbsYXbsLocationsensorm_.IsEOF(); XbsYXbsLocationsensorm_.MoveNext())
                        {
                            var m_fr_ = new YXbsLocationsensorm();
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
                });
                tsk.Start();
                //tsk.Join();
                logger.Trace("End procedure ReadlResultFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadlResultFromDB: "+ex.Message);
            }
            return L_IN;
        }

        public List<ClassSDRResults> ReadlAllResultFromDB()
        {
            // Список объектов в рамках конкретного адаптера ICSM
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                logger.Trace("Start procedure ReadlAllResultFromDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                YXbsMeasurementres res_val = new YXbsMeasurementres();
                res_val.Format("*");
                // выбирать только таски, для которых STATUS не NULL
                res_val.Filter = "(ID>0) AND (STATUS<>'Z')";
                for (res_val.OpenRs(); !res_val.IsEOF(); res_val.MoveNext())
                {
                    ClassSDRResults ICSM_T = new ClassSDRResults();
                    ICSM_T.freq_meas = new List<YXbsFrequencymeas>();
                    ICSM_T.level_meas_res = new List<YXbsLevelmeasres>();
                    ICSM_T.loc_sensorM = new List<YXbsLocationsensorm>();
                    ICSM_T.meas_res = new YXbsMeasurementres();
                    ICSM_T.spect_occup_meas = new List<YXbsSpectoccupmeas>();

                    ICSM_T.meas_res = new YXbsMeasurementres();
                    var m_fr = new YXbsMeasurementres();
                    m_fr.CopyDataFrom(res_val);
                    ICSM_T.meas_res = m_fr;

                    /*
                    YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas();
                    XbsYXbsFrequencymeas_.Format("*");
                    XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                    for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                    {
                        var m_fr_ = new YXbsFrequencymeas();
                        m_fr_.CopyDataFrom(XbsYXbsFrequencymeas_);
                        ICSM_T.freq_meas.Add(m_fr_);
                        m_fr_.Close();
                        m_fr_.Dispose();
                    }
                    XbsYXbsFrequencymeas_.Close();
                    XbsYXbsFrequencymeas_.Dispose();
                    */

                    YXbsLevelmeasres XbsYXbsLevelmeasres_ = new YXbsLevelmeasres();
                    XbsYXbsLevelmeasres_.Format("*");
                    XbsYXbsLevelmeasres_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0})", res_val.m_id);
                    for (XbsYXbsLevelmeasres_.OpenRs(); !XbsYXbsLevelmeasres_.IsEOF(); XbsYXbsLevelmeasres_.MoveNext())
                    {
                        var m_fr_ = new YXbsLevelmeasres();
                        m_fr_.CopyDataFrom(XbsYXbsLevelmeasres_);
                        ICSM_T.level_meas_res.Add(m_fr_);
                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas();
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0}) and (NUM={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas();
                                m_fr_f.CopyDataFrom(XbsYXbsFrequencymeas_);
                                ICSM_T.freq_meas.Add(m_fr_f);
                                m_fr_f.Close();
                                m_fr_f.Dispose();
                            }
                            XbsYXbsFrequencymeas_.Close();
                            XbsYXbsFrequencymeas_.Dispose();
                            /////////

                            m_fr_.Close();
                            m_fr_.Dispose();

                        }
                    XbsYXbsLevelmeasres_.Close();
                    XbsYXbsLevelmeasres_.Dispose();

                    YXbsLevelmeasonlres XbsYXbsLevelmeasonlres_ = new YXbsLevelmeasonlres();
                    XbsYXbsLevelmeasonlres_.Format("*");
                    XbsYXbsLevelmeasonlres_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0})", res_val.m_id);
                    for (XbsYXbsLevelmeasonlres_.OpenRs(); !XbsYXbsLevelmeasonlres_.IsEOF(); XbsYXbsLevelmeasonlres_.MoveNext())
                     {
                            var m_fr_ = new YXbsLevelmeasonlres();
                            m_fr_.CopyDataFrom(XbsYXbsLevelmeasonlres_);
                            ICSM_T.level_meas_onl_res.Add(m_fr_);


                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas();
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0}) and (NUM={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas();
                                m_fr_f.CopyDataFrom(XbsYXbsFrequencymeas_);
                                ICSM_T.freq_meas.Add(m_fr_f);
                                m_fr_f.Close();
                                m_fr_f.Dispose();
                            }
                            XbsYXbsFrequencymeas_.Close();
                            XbsYXbsFrequencymeas_.Dispose();
                            /////////


                            m_fr_.Close();
                            m_fr_.Dispose();
                     }
                    XbsYXbsLevelmeasonlres_.Close();
                    XbsYXbsLevelmeasonlres_.Dispose();

                    YXbsSpectoccupmeas XbsYXbsSpectoccupmeas_ = new YXbsSpectoccupmeas();
                    XbsYXbsSpectoccupmeas_.Format("*");
                    XbsYXbsSpectoccupmeas_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0})", res_val.m_id);
                    for (XbsYXbsSpectoccupmeas_.OpenRs(); !XbsYXbsSpectoccupmeas_.IsEOF(); XbsYXbsSpectoccupmeas_.MoveNext())
                    {
                        var m_fr_ = new YXbsSpectoccupmeas();
                        m_fr_.CopyDataFrom(XbsYXbsSpectoccupmeas_);
                        ICSM_T.spect_occup_meas.Add(m_fr_);

                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas();
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0}) and (NUM={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas();
                                m_fr_f.CopyDataFrom(XbsYXbsFrequencymeas_);
                                ICSM_T.freq_meas.Add(m_fr_f);
                                m_fr_f.Close();
                                m_fr_f.Dispose();
                            }
                            XbsYXbsFrequencymeas_.Close();
                            XbsYXbsFrequencymeas_.Dispose();
                            /////////

                            m_fr_.Close();
                        m_fr_.Dispose();
                    }
                    XbsYXbsSpectoccupmeas_.Close();
                    XbsYXbsSpectoccupmeas_.Dispose();

                    YXbsLocationsensorm XbsYXbsLocationsensorm_ = new YXbsLocationsensorm();
                    XbsYXbsLocationsensorm_.Format("*");
                    XbsYXbsLocationsensorm_.Filter = string.Format("(ID_XBS_MEASUREMENTRES={0})", res_val.m_id);
                    for (XbsYXbsLocationsensorm_.OpenRs(); !XbsYXbsLocationsensorm_.IsEOF(); XbsYXbsLocationsensorm_.MoveNext())
                    {
                        var m_fr_ = new YXbsLocationsensorm();
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
            });
            tsk.Start();
            tsk.Join();
            logger.Trace("End procedure ReadlAllResultFromDB.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure ReadlAllResultFromDB: "+ex.Message);
            }
            return L_IN;
        }

        public bool DeleteResultFromDB(MeasurementResults obj, string Status)
        {
            bool isSuccess = false;
            try
            {
                logger.Trace("Start procedure DeleteResultFromDB...");
                /// Create new record in YXbsMeastask
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    YXbsMeasurementres measRes = new YXbsMeasurementres();
                    measRes.Format("*");
                    if (obj != null)
                    {
                        if ((obj.Id.MeasTaskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != Constants.NullI) && (obj.Id.SubMeasTaskStationId != Constants.NullI))
                        {
                            if (obj.StationMeasurements.StationId != null)
                            {
                                if (measRes.Fetch(string.Format(" (MEASTASKID={0}) and (SENSORID={1}) and (SUBMEASTASKID={2}) and (SUBMEASTASKSTATIONID={3})", obj.Id.MeasTaskId.Value, obj.StationMeasurements.StationId.Value, obj.Id.SubMeasTaskId, obj.Id.SubMeasTaskStationId)))
                                {
                                    isSuccess = true;
                                    measRes.m_status = Status;
                                    measRes.Save();
                                }
                            }
                        }
                        else if (obj.Id.MeasTaskId != null)
                        {
                            {
                                measRes.Filter = string.Format(" (MEASTASKID={0}) ", obj.Id.MeasTaskId.Value);
                                for (measRes.OpenRs(); !measRes.IsEOF(); measRes.MoveNext())
                                {
                                    isSuccess = true;
                                    measRes.m_status = Status;
                                    measRes.Save();
                                }
                            }
                        }
                        measRes.Close();
                        measRes.Dispose();
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure DeleteResultFromDB.");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in procedure DeleteResultFromDB: "+ex.Message);
            }
            return isSuccess;
        }
        public bool DeleteResultFromDB(MeasurementResults obj)
        {
            bool isSuccess = false;
            try
            {
                logger.Trace("Start procedure DeleteResultFromDB...");
                System.Threading.Thread tsk = new System.Threading.Thread(() =>
                {
                    YXbsMeasurementres measRes = new YXbsMeasurementres();
                    measRes.Format("*");
                    if (obj != null)
                    {
                        if ((obj.Id.MeasTaskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != Constants.NullI) && (obj.Id.SubMeasTaskStationId != Constants.NullI))
                        {
                            if (obj.StationMeasurements.StationId != null)
                            {
                                if (measRes.Fetch(string.Format(" (MEASTASKID={0}) and (SENSORID={1}) and (SUBMEASTASKID={2}) and (SUBMEASTASKSTATIONID={3})", obj.Id.MeasTaskId.Value, obj.StationMeasurements.StationId.Value, obj.Id.SubMeasTaskId, obj.Id.SubMeasTaskStationId)))
                                {
                                    foreach (FrequencyMeasurement dt_param in obj.FrequenciesMeasurements.ToArray())
                                    {
                                        YXbsFrequencymeas dtr = new YXbsFrequencymeas();
                                        dtr.Format("*");
                                        if (dt_param != null)
                                        {
                                            if (dtr.Fetch(string.Format("ID_XBS_MEASUREMENTRES={0}", measRes.m_id)))
                                            {
                                                dtr.Delete();
                                            }
                                        }
                                        dtr.Close();
                                        dtr.Dispose();
                                    }
                                    foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray())
                                    {
                                        YXbsLocationsensorm dtr = new YXbsLocationsensorm();
                                        dtr.Format("*");
                                        if (dt_param != null)
                                        {
                                            if (dtr.Fetch(string.Format("ID_XBS_MEASUREMENTRES={0}", measRes.m_id)))
                                            {
                                                dtr.Delete();
                                            }
                                        }
                                        dtr.Close();
                                        dtr.Dispose();
                                    }
                                    foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray())
                                    {
                                        if (obj.TypeMeasurements == MeasurementType.Level)
                                        {
                                            YXbsLevelmeasres dtr = new YXbsLevelmeasres();
                                            dtr.Format("*");
                                            if (dt_param != null)
                                            {
                                                if (dtr.Fetch(string.Format("ID_XBS_MEASUREMENTRES={0}", measRes.m_id)))
                                                {
                                                    dtr.Delete();
                                                }
                                            }
                                            dtr.Close();
                                            dtr.Dispose();

                                        }
                                        else if (obj.TypeMeasurements == MeasurementType.SpectrumOccupation)
                                        {
                                            YXbsSpectoccupmeas dtr = new YXbsSpectoccupmeas();
                                            dtr.Format("*");
                                            if (dt_param != null)
                                            {
                                                if (dtr.Fetch(string.Format("ID_XBS_MEASUREMENTRES={0}", measRes.m_id)))
                                                {
                                                    dtr.Delete();
                                                }
                                            }
                                            dtr.Close();
                                            dtr.Dispose();
                                        }
                                    }
                                    isSuccess = true;
                                    measRes.Delete();
                                }
                            }
                        }
                        else if (obj.Id.MeasTaskId != null)
                        {
                            {
                                measRes.Filter = string.Format(" (MEASTASKID={0}) ", obj.Id.MeasTaskId.Value);
                                for (measRes.OpenRs(); !measRes.IsEOF(); measRes.MoveNext())
                                {
                                    foreach (FrequencyMeasurement dt_param in obj.FrequenciesMeasurements.ToArray())
                                    {
                                        YXbsFrequencymeas dtr = new YXbsFrequencymeas();
                                        dtr.Format("*");
                                        if (dt_param != null)
                                        {
                                            dtr.Filter = string.Format("ID_XBS_MEASUREMENTRES={0}", measRes.m_id);
                                            for (dtr.OpenRs(); !dtr.IsEOF(); dtr.MoveNext())
                                            {
                                                dtr.Delete();
                                            }
                                        }
                                        dtr.Close();
                                        dtr.Dispose();
                                    }

                                    foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray())
                                    {
                                        YXbsLocationsensorm dtr = new YXbsLocationsensorm();
                                        dtr.Format("*");
                                        if (dt_param != null)
                                        {
                                            dtr.Filter = string.Format("ID_XBS_MEASUREMENTRES={0}", measRes.m_id);
                                            for (dtr.OpenRs(); !dtr.IsEOF(); dtr.MoveNext())
                                            {
                                                dtr.Delete();
                                            }
                                        }
                                        dtr.Close();
                                        dtr.Dispose();
                                    }

                                    foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray())
                                    {
                                        if (obj.TypeMeasurements == MeasurementType.Level)
                                        {
                                            YXbsLevelmeasres dtr = new YXbsLevelmeasres();
                                            dtr.Format("*");
                                            if (dt_param != null)
                                            {
                                                dtr.Filter = string.Format("ID_XBS_MEASUREMENTRES={0}", measRes.m_id);
                                                for (dtr.OpenRs(); !dtr.IsEOF(); dtr.MoveNext())
                                                {
                                                    dtr.Delete();
                                                }
                                            }
                                            dtr.Close();
                                            dtr.Dispose();

                                        }
                                        else if (obj.TypeMeasurements == MeasurementType.SpectrumOccupation)
                                        {
                                            YXbsSpectoccupmeas dtr = new YXbsSpectoccupmeas();
                                            dtr.Format("*");
                                            if (dt_param != null)
                                            {
                                                dtr.Filter = string.Format("ID_XBS_MEASUREMENTRES={0}", measRes.m_id);
                                                for (dtr.OpenRs(); !dtr.IsEOF(); dtr.MoveNext())
                                                {
                                                    dtr.Delete();
                                                }
                                            }
                                            dtr.Close();
                                            dtr.Dispose();
                                        }
                                    }
                                    isSuccess = true;
                                    measRes.Delete();
                                }
                            }
                        }
                        measRes.Close();
                        measRes.Dispose();
                    }
                });
                tsk.Start();
                tsk.Join();
                logger.Trace("End procedure DeleteResultFromDB.");
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error("Error in procedure DeleteResultFromDB:" +ex.Message);
            }
            return isSuccess;
        }


        public int SaveResultToDB(MeasurementResults obj)
        {
            int ID = Constants.NullI;
            if (((obj.TypeMeasurements == MeasurementType.SpectrumOccupation) && (obj.Status == "C")) || (obj.TypeMeasurements != MeasurementType.SpectrumOccupation))
            {
                System.Threading.Thread tsk = new System.Threading.Thread(() => {
                    try
                    {
                        logger.Trace("Start procedure SaveResultToDB.");
                        List<Yyy> BlockInsert_FrequencyMeasurement2 = new List<Yyy>();
                        List<Yyy> BlockInsert_YXbsLevelmeasres1 = new List<Yyy>();
                        List<Yyy> BlockInsert_YXbsSpectoccupmeas1 = new List<Yyy>();
                        /// Create new record in YXbsMeastask
                        if (obj != null)
                        {
                            if ((obj.Id.MeasTaskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != Constants.NullI) && (obj.Id.SubMeasTaskStationId != Constants.NullI))
                            {
                            if (obj.StationMeasurements.StationId != null)
                            {
                                YXbsMeasurementres measRes = new YXbsMeasurementres();
                                measRes.Format("*");
                                measRes.Filter = "ID=-1";
                                measRes.New();
                                if (obj.AntVal != null) measRes.m_antval = obj.AntVal.GetValueOrDefault();
                                if (obj.DataRank != null) measRes.m_datarank = obj.DataRank.GetValueOrDefault();
                                measRes.m_status = obj.Status;
                                measRes.m_meastaskid = obj.Id.MeasTaskId.Value;
                                if (obj.N != null) measRes.m_n = obj.N.GetValueOrDefault();
                                measRes.m_sensorid = obj.StationMeasurements.StationId.Value;
                                measRes.m_submeastaskid = obj.Id.SubMeasTaskId;
                                measRes.m_submeastaskstationid = obj.Id.SubMeasTaskStationId;
                                measRes.m_timemeas = obj.TimeMeas;
                                measRes.m_typemeasurements = obj.TypeMeasurements.ToString();
                                ID = (int)measRes.Save();
                                obj.Id.MeasSdrResultsId = ID;
                                measRes.Close();
                                measRes.Dispose();
                            }
                            }
                            if (ID != Constants.NullI)
                            {
                                if (obj.LocationSensorMeasurement != null)
                                {
                                    List<Yyy> BlockInsert_LocationSensorMeasurement = new List<Yyy>();
                                    foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray())
                                    {
                                        if (dt_param != null)
                                        {
                                            YXbsLocationsensorm dtr = new YXbsLocationsensorm();
                                            dtr.Format("*");
                                            dtr.Filter = "ID=-1";
                                            dtr.New();
                                            if (dt_param.ASL != null) dtr.m_asl = dt_param.ASL.GetValueOrDefault();
                                            if (dt_param.Lon != null) dtr.m_lon = dt_param.Lon.GetValueOrDefault();
                                            if (dt_param.Lat != null) dtr.m_lat = dt_param.Lat.GetValueOrDefault();
                                            dtr.m_id_xbs_measurementres = ID;
                                            for (int i = 0; i < dtr.getAllFields.Count; i++)
                                                dtr.getAllFields[i].Value = dtr.valc[i];
                                            BlockInsert_LocationSensorMeasurement.Add(dtr);
                                            dtr.Close();
                                            dtr.Dispose();
                                        }
                                    }
                                    if (BlockInsert_LocationSensorMeasurement.Count > 0)
                                    {
                                        YXbsLocationsensorm YXbsLocationsensorm11 = new YXbsLocationsensorm();
                                        YXbsLocationsensorm11.Format("*");
                                        YXbsLocationsensorm11.New();
                                        YXbsLocationsensorm11.SaveBath(BlockInsert_LocationSensorMeasurement);
                                        YXbsLocationsensorm11.Close();
                                        YXbsLocationsensorm11.Dispose();
                                    }
                                }
                                if (obj.MeasurementsResults != null)
                                {
                                    int AllIdx = 0;
                                    YXbsLevelmeasres dtr_ = new YXbsLevelmeasres();
                                    int idx_cnt = 0;
                                    YXbsLevelmeasres d_ = new YXbsLevelmeasres();
                                    d_.Format("*");
                                    int? indexerYXbsLevelmeasres = d_.GetMaxId(d_.GetTableName());
                                    YXbsLevelmeasres dx_ = new YXbsLevelmeasres();
                                    dx_.Format("*");
                                    int? indexerYXbsSpectoccupmeas = dx_.GetMaxId(dx_.GetTableName());
                                    foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray())
                                    {
                                        if ((obj.TypeMeasurements == MeasurementType.Level) && (obj.Status != "O"))
                                        {
                                            if (dt_param != null)
                                            {
                                                if (dt_param is LevelMeasurementResult)
                                                {
                                                    ++indexerYXbsLevelmeasres;
                                                    YXbsLevelmeasres dtrR = new YXbsLevelmeasres();
                                                    dtrR.Format("*");
                                                    dtrR.Filter = "ID=-1";
                                                    dtrR.New();
                                                    if ((dt_param as LevelMeasurementResult).Value != null) dtrR.m_value = (dt_param as LevelMeasurementResult).Value.GetValueOrDefault();
                                                    if ((dt_param as LevelMeasurementResult).PMax != null) dtrR.m_pmax = (dt_param as LevelMeasurementResult).PMax.GetValueOrDefault();
                                                    if ((dt_param as LevelMeasurementResult).PMin != null) dtrR.m_pmin = (dt_param as LevelMeasurementResult).PMin.GetValueOrDefault();
                                                    dtrR.m_id_xbs_measurementres = ID;
                                                    for (int i = 0; i < dtrR.getAllFields.Count; i++)
                                                        dtrR.getAllFields[i].Value = dtrR.valc[i];
                                                    BlockInsert_YXbsLevelmeasres1.Add(dtrR);
                                                    dtrR.Close();
                                                    dtrR.Dispose();
                                                    
                                                   /*
                                                    YXbsLevelmeasres dtrR = new YXbsLevelmeasres();
                                                    dtrR.Format("*");
                                                    dtrR.Filter = "ID=-1";
                                                    dtrR.New();
                                                    if ((dt_param as LevelMeasurementResult).Value != null) dtrR.m_value = (dt_param as LevelMeasurementResult).Value.GetValueOrDefault();
                                                    if ((dt_param as LevelMeasurementResult).PMax != null) dtrR.m_pmax = (dt_param as LevelMeasurementResult).PMax.GetValueOrDefault();
                                                    if ((dt_param as LevelMeasurementResult).PMin != null) dtrR.m_pmin = (dt_param as LevelMeasurementResult).PMin.GetValueOrDefault();
                                                    dtrR.m_id_xbs_measurementres = ID;
                                                    int? ID_DT_params = dtrR.Save();
                                                    dtrR.Close();
                                                    dtrR.Dispose();
                                                    */

                                                    

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
                                                                        YXbsFrequencymeas dtr_freq = new YXbsFrequencymeas();
                                                                        dtr_freq.Format("*");
                                                                        dtr_freq.Filter = "ID=-1";
                                                                        dtr_freq.New();
                                                                        dtr_freq.m_freq = dt_param_freq.Freq;
                                                                        dtr_freq.m_id_xbs_measurementres = ID;
                                                                        //dtr_freq.m_num = ID_DT_params;
                                                                        //dt_param_freq.Id = (int)ID_DT_params;
                                                                        dtr_freq.m_num = indexerYXbsLevelmeasres;
                                                                        dt_param_freq.Id = (int)indexerYXbsLevelmeasres;

                                                                        for (int i = 0; i < dtr_freq.getAllFields.Count; i++)
                                                                            dtr_freq.getAllFields[i].Value = dtr_freq.valc[i];
                                                                        BlockInsert_FrequencyMeasurement2.Add(dtr_freq);
                                                                        dtr_freq.Close();
                                                                        dtr_freq.Dispose();
                                                                        AllIdx++;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        dt_param.Id = new MeasurementResultIdentifier();
                                                        //dt_param.Id.Value = (int)ID_DT_params;
                                                        dt_param.Id.Value = (int)indexerYXbsLevelmeasres;
                                                    }
                                                }
                                            }
                                            else if ((obj.TypeMeasurements == MeasurementType.SpectrumOccupation) && (obj.Status == "C"))
                                            {
                                                if (dt_param != null)
                                                {
                                                    if (dt_param is SpectrumOccupationMeasurementResult)
                                                    {
                                                        ++indexerYXbsSpectoccupmeas;

                                                        YXbsSpectoccupmeas dtr = new YXbsSpectoccupmeas();
                                                        dtr.Format("*");
                                                        dtr.Filter = "ID=-1";
                                                        dtr.New();
                                                        if ((dt_param as SpectrumOccupationMeasurementResult).Value != null) dtr.m_value = (dt_param as SpectrumOccupationMeasurementResult).Value.GetValueOrDefault();
                                                        if ((dt_param as SpectrumOccupationMeasurementResult).Occupancy != null) dtr.m_occupancy = (dt_param as SpectrumOccupationMeasurementResult).Occupancy.GetValueOrDefault();
                                                        dtr.m_id_xbs_measurementres = ID;
                                                        for (int i = 0; i < dtr.getAllFields.Count; i++)
                                                            dtr.getAllFields[i].Value = dtr.valc[i];
                                                        BlockInsert_YXbsSpectoccupmeas1.Add(dtr);
                                                        //int? ID_DT_params = dtr.Save();
                                                        dtr.Close();
                                                        dtr.Dispose();

                                                        /*
                                                        YXbsSpectoccupmeas dtr = new YXbsSpectoccupmeas();
                                                        dtr.Format("*");
                                                        dtr.Filter = "ID=-1";
                                                        dtr.New();
                                                        if ((dt_param as SpectrumOccupationMeasurementResult).Value != null) dtr.m_value = (dt_param as SpectrumOccupationMeasurementResult).Value.GetValueOrDefault();
                                                        if ((dt_param as SpectrumOccupationMeasurementResult).Occupancy != null) dtr.m_occupancy = (dt_param as SpectrumOccupationMeasurementResult).Occupancy.GetValueOrDefault();
                                                        dtr.m_id_xbs_measurementres = ID;
                                                        int? ID_DT_params = dtr.Save();
                                                        dtr.Close();
                                                        dtr.Dispose();
                                                        */

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
                                                                            YXbsFrequencymeas dtr_freq = new YXbsFrequencymeas();
                                                                            dtr_freq.Format("*");
                                                                            dtr_freq.Filter = "ID=-1";
                                                                            dtr_freq.New();
                                                                            dtr_freq.m_freq = dt_param_freq.Freq;
                                                                            dtr_freq.m_id_xbs_measurementres = ID;
                                                                            //dtr_freq.m_num = ID_DT_params;
                                                                            dtr_freq.m_num = indexerYXbsSpectoccupmeas;
                                                                            for (int i = 0; i < dtr_freq.getAllFields.Count; i++)
                                                                                dtr_freq.getAllFields[i].Value = dtr_freq.valc[i];
                                                                            BlockInsert_FrequencyMeasurement2.Add(dtr_freq);
                                                                            //dt_param_freq.Id = (int)ID_DT_params;
                                                                            dt_param_freq.Id = (int)indexerYXbsSpectoccupmeas;
                                                                            dtr_freq.Close();
                                                                            dtr_freq.Dispose();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        dt_param.Id = new MeasurementResultIdentifier();
                                                        //dt_param.Id.Value = (int)ID_DT_params;
                                                        dt_param.Id.Value = (int)indexerYXbsSpectoccupmeas;
                                                    }
                                                }
                                            }
                                            //else 
                                            {
                                                if (dt_param != null)
                                                {
                                                    if (dt_param is LevelMeasurementOnlineResult)
                                                    {
                                                        YXbsLevelmeasonlres dtr = new YXbsLevelmeasonlres();
                                                        dtr.Format("*");
                                                        dtr.Filter = "ID=-1";
                                                        dtr.New();
                                                        if ((dt_param as LevelMeasurementOnlineResult).Value != Constants.NullD) dtr.m_value = (dt_param as LevelMeasurementOnlineResult).Value;
                                                        dtr.m_id_xbs_measurementres = ID;
                                                        int? ID_DT_params = dtr.Save();
                                                        dt_param.Id = new MeasurementResultIdentifier();
                                                        dt_param.Id.Value = (int)ID_DT_params;
                                                        dtr.Close();
                                                        dtr.Dispose();
                                                    }
                                                }
                                            }
                                            idx_cnt++;
                                        }
                                    }
                                    if (BlockInsert_YXbsLevelmeasres1.Count > 0)
                                    {
                                        int iu = AllIdx;
                                        YXbsLevelmeasres YXbsLevelmeasres11 = new YXbsLevelmeasres();
                                        YXbsLevelmeasres11.Format("*");
                                        YXbsLevelmeasres11.New();
                                        YXbsLevelmeasres11.SaveBath(BlockInsert_YXbsLevelmeasres1);
                                        YXbsLevelmeasres11.Close();
                                        YXbsLevelmeasres11.Dispose();
                                    }

                                    if (BlockInsert_YXbsSpectoccupmeas1.Count > 0)
                                    {
                                        YXbsSpectoccupmeas YXbsSpectoccupmeas11 = new YXbsSpectoccupmeas();
                                        YXbsSpectoccupmeas11.Format("*");
                                        YXbsSpectoccupmeas11.New();
                                        YXbsSpectoccupmeas11.SaveBath(BlockInsert_YXbsSpectoccupmeas1);
                                        YXbsSpectoccupmeas11.Close();
                                        YXbsSpectoccupmeas11.Dispose();
                                    }

                                    if (BlockInsert_FrequencyMeasurement2.Count > 0)
                                    {
                                        YXbsFrequencymeas YXbsFrequencymeas11 = new YXbsFrequencymeas();
                                        YXbsFrequencymeas11.Format("*");
                                        YXbsFrequencymeas11.New();
                                        YXbsFrequencymeas11.SaveBath(BlockInsert_FrequencyMeasurement2);
                                        YXbsFrequencymeas11.Close();
                                        YXbsFrequencymeas11.Dispose();
                                    }
                                }
                            }
                        }
                        logger.Trace("End procedure SaveResultToDB.");
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error in procedure SaveResultToDB: "+ex.Message);
                    }
                });
                tsk.Start();
                tsk.Join();
            }
            return ID;
        }
        
    }
}
