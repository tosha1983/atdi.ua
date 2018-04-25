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

    public class ClassesDBGetResult : IDisposable
    {
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
                Task tsk = new Task(() => {
                    YXbsMeasurementres res_val = new YXbsMeasurementres(ConnectDB.Connect_Main_);
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
                        var m_fr = new YXbsMeasurementres(ConnectDB.Connect_Main_);
                        m_fr.CopyDataFrom(res_val);
                        ICSM_T.meas_res = m_fr;

                        /*
                        YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                        XbsYXbsFrequencymeas_.Format("*");
                        XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                        for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                        {
                            var m_fr_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                            m_fr_.CopyDataFrom(XbsYXbsFrequencymeas_);
                            ICSM_T.freq_meas.Add(m_fr_);
                            m_fr_.Close();
                            m_fr_.Dispose();
                        }
                        XbsYXbsFrequencymeas_.Close();
                        XbsYXbsFrequencymeas_.Dispose();
                        */

                        YXbsLevelmeasres XbsYXbsLevelmeasres_ = new YXbsLevelmeasres(ConnectDB.Connect_Main_);
                        XbsYXbsLevelmeasres_.Format("*");
                        XbsYXbsLevelmeasres_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                        for (XbsYXbsLevelmeasres_.OpenRs(); !XbsYXbsLevelmeasres_.IsEOF(); XbsYXbsLevelmeasres_.MoveNext())
                        {
                            var m_fr_ = new YXbsLevelmeasres(ConnectDB.Connect_Main_);
                            m_fr_.CopyDataFrom(XbsYXbsLevelmeasres_);
                            ICSM_T.level_meas_res.Add(m_fr_);

                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0}) and (Num={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
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

                        YXbsLevelmeasonlres XbsYXbsLevelmeasonlres_ = new YXbsLevelmeasonlres(ConnectDB.Connect_Main_);
                        XbsYXbsLevelmeasonlres_.Format("*");
                        XbsYXbsLevelmeasonlres_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                        for (XbsYXbsLevelmeasonlres_.OpenRs(); !XbsYXbsLevelmeasonlres_.IsEOF(); XbsYXbsLevelmeasonlres_.MoveNext())
                        {
                            var m_fr_ = new YXbsLevelmeasonlres(ConnectDB.Connect_Main_);
                            m_fr_.CopyDataFrom(XbsYXbsLevelmeasonlres_);
                            ICSM_T.level_meas_onl_res.Add(m_fr_);

                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0}) and (Num={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
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

                        YXbsSpectoccupmeas XbsYXbsSpectoccupmeas_ = new YXbsSpectoccupmeas(ConnectDB.Connect_Main_);
                        XbsYXbsSpectoccupmeas_.Format("*");
                        XbsYXbsSpectoccupmeas_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                        for (XbsYXbsSpectoccupmeas_.OpenRs(); !XbsYXbsSpectoccupmeas_.IsEOF(); XbsYXbsSpectoccupmeas_.MoveNext())
                        {
                            var m_fr_ = new YXbsSpectoccupmeas(ConnectDB.Connect_Main_);
                            m_fr_.CopyDataFrom(XbsYXbsSpectoccupmeas_);
                            ICSM_T.spect_occup_meas.Add(m_fr_);

                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0}) and (Num={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
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

                        YXbsLocationsensorm XbsYXbsLocationsensorm_ = new YXbsLocationsensorm(ConnectDB.Connect_Main_);
                        XbsYXbsLocationsensorm_.Format("*");
                        XbsYXbsLocationsensorm_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                        for (XbsYXbsLocationsensorm_.OpenRs(); !XbsYXbsLocationsensorm_.IsEOF(); XbsYXbsLocationsensorm_.MoveNext())
                        {
                            var m_fr_ = new YXbsLocationsensorm(ConnectDB.Connect_Main_);
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
                //tsk.Wait();
                Console.WriteLine("Readl Result From DB Objects ...");
            }
            catch (Exception)
            {

            }
            return L_IN;
        }

        public List<ClassSDRResults> ReadlAllResultFromDB()
        {
            // Список объектов в рамках конкретного адаптера ICSM
            List<ClassSDRResults> L_IN = new List<ClassSDRResults>();
            try
            {
                Task tsk = new Task(() => {
                YXbsMeasurementres res_val = new YXbsMeasurementres(ConnectDB.Connect_Main_);
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
                    var m_fr = new YXbsMeasurementres(ConnectDB.Connect_Main_);
                    m_fr.CopyDataFrom(res_val);
                    ICSM_T.meas_res = m_fr;

                    /*
                    YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                    XbsYXbsFrequencymeas_.Format("*");
                    XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                    for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                    {
                        var m_fr_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                        m_fr_.CopyDataFrom(XbsYXbsFrequencymeas_);
                        ICSM_T.freq_meas.Add(m_fr_);
                        m_fr_.Close();
                        m_fr_.Dispose();
                    }
                    XbsYXbsFrequencymeas_.Close();
                    XbsYXbsFrequencymeas_.Dispose();
                    */

                    YXbsLevelmeasres XbsYXbsLevelmeasres_ = new YXbsLevelmeasres(ConnectDB.Connect_Main_);
                    XbsYXbsLevelmeasres_.Format("*");
                    XbsYXbsLevelmeasres_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                    for (XbsYXbsLevelmeasres_.OpenRs(); !XbsYXbsLevelmeasres_.IsEOF(); XbsYXbsLevelmeasres_.MoveNext())
                    {
                        var m_fr_ = new YXbsLevelmeasres(ConnectDB.Connect_Main_);
                        m_fr_.CopyDataFrom(XbsYXbsLevelmeasres_);
                        ICSM_T.level_meas_res.Add(m_fr_);
                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0}) and (Num={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
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

                    YXbsLevelmeasonlres XbsYXbsLevelmeasonlres_ = new YXbsLevelmeasonlres(ConnectDB.Connect_Main_);
                    XbsYXbsLevelmeasonlres_.Format("*");
                    XbsYXbsLevelmeasonlres_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                    for (XbsYXbsLevelmeasonlres_.OpenRs(); !XbsYXbsLevelmeasonlres_.IsEOF(); XbsYXbsLevelmeasonlres_.MoveNext())
                     {
                            var m_fr_ = new YXbsLevelmeasonlres(ConnectDB.Connect_Main_);
                            m_fr_.CopyDataFrom(XbsYXbsLevelmeasonlres_);
                            ICSM_T.level_meas_onl_res.Add(m_fr_);


                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0}) and (Num={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
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

                    YXbsSpectoccupmeas XbsYXbsSpectoccupmeas_ = new YXbsSpectoccupmeas(ConnectDB.Connect_Main_);
                    XbsYXbsSpectoccupmeas_.Format("*");
                    XbsYXbsSpectoccupmeas_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                    for (XbsYXbsSpectoccupmeas_.OpenRs(); !XbsYXbsSpectoccupmeas_.IsEOF(); XbsYXbsSpectoccupmeas_.MoveNext())
                    {
                        var m_fr_ = new YXbsSpectoccupmeas(ConnectDB.Connect_Main_);
                        m_fr_.CopyDataFrom(XbsYXbsSpectoccupmeas_);
                        ICSM_T.spect_occup_meas.Add(m_fr_);

                            //////// получить частоты
                            YXbsFrequencymeas XbsYXbsFrequencymeas_ = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                            XbsYXbsFrequencymeas_.Format("*");
                            XbsYXbsFrequencymeas_.Filter = string.Format("(id_xbs_measurementres={0}) and (Num={1})", res_val.m_id, m_fr_.m_id);
                            for (XbsYXbsFrequencymeas_.OpenRs(); !XbsYXbsFrequencymeas_.IsEOF(); XbsYXbsFrequencymeas_.MoveNext())
                            {
                                var m_fr_f = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
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

                    YXbsLocationsensorm XbsYXbsLocationsensorm_ = new YXbsLocationsensorm(ConnectDB.Connect_Main_);
                    XbsYXbsLocationsensorm_.Format("*");
                    XbsYXbsLocationsensorm_.Filter = string.Format("(id_xbs_measurementres={0})", res_val.m_id);
                    for (XbsYXbsLocationsensorm_.OpenRs(); !XbsYXbsLocationsensorm_.IsEOF(); XbsYXbsLocationsensorm_.MoveNext())
                    {
                        var m_fr_ = new YXbsLocationsensorm(ConnectDB.Connect_Main_);
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
            //tsk.Wait();
            }
            catch (Exception)
            {

            }
            Console.WriteLine("Read All Result From DB Objects ...");
            return L_IN;
        }

        public bool DeleteResultFromDB(MeasurementResults obj, string Status)
        {
            bool isSuccess = false;
            try
            {
                /// Create new record in YXbsMeastask
                YXbsMeasurementres measRes = new YXbsMeasurementres(ConnectDB.Connect_Main_);
                measRes.Format("*");
                if (obj != null)
                {
                    if ((obj.Id.MeasTaskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != ConnectDB.NullI) && (obj.Id.SubMeasTaskStationId != ConnectDB.NullI))
                    {
                        if (obj.StationMeasurements.StationId != null)
                        {
                            if (measRes.Fetch(string.Format(" (meastaskid={0}) and (sensorid={1}) and (submeastaskid={2}) and (submeastaskstationid={3})", obj.Id.MeasTaskId.Value, obj.StationMeasurements.StationId.Value, obj.Id.SubMeasTaskId, obj.Id.SubMeasTaskStationId)))
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
                            measRes.Filter = string.Format(" (meastaskid={0}) ", obj.Id.MeasTaskId.Value);
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
            }
            catch (Exception)
            { isSuccess = false; }
            return isSuccess;
        }
        public bool DeleteResultFromDB(MeasurementResults obj)
        {
            bool isSuccess = false;
            try
            {
                /// Create new record in YXbsMeastask
                YXbsMeasurementres measRes = new YXbsMeasurementres(ConnectDB.Connect_Main_);
                measRes.Format("*");
                if (obj != null) {
                    if ((obj.Id.MeasTaskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != ConnectDB.NullI) && (obj.Id.SubMeasTaskStationId != ConnectDB.NullI)) {
                        if (obj.StationMeasurements.StationId != null) {
                            if (measRes.Fetch(string.Format(" (meastaskid={0}) and (sensorid={1}) and (submeastaskid={2}) and (submeastaskstationid={3})", obj.Id.MeasTaskId.Value, obj.StationMeasurements.StationId.Value, obj.Id.SubMeasTaskId, obj.Id.SubMeasTaskStationId))) {
                                foreach (FrequencyMeasurement dt_param in obj.FrequenciesMeasurements.ToArray()) {
                                    YXbsFrequencymeas dtr = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                                    dtr.Format("*");
                                    if (dt_param != null) {
                                        if (dtr.Fetch(string.Format("id_xbs_measurementres={0}", measRes.m_id))) {
                                            dtr.Delete();
                                        }
                                    }
                                    dtr.Close();
                                    dtr.Dispose();
                                }
                                foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray()) {
                                    YXbsLocationsensorm dtr = new YXbsLocationsensorm(ConnectDB.Connect_Main_);
                                    dtr.Format("*");
                                    if (dt_param != null) {
                                        if (dtr.Fetch(string.Format("id_xbs_measurementres={0}", measRes.m_id))) {
                                            dtr.Delete();
                                        }
                                    }
                                    dtr.Close();
                                    dtr.Dispose();
                                }
                                foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray()) {
                                    if (obj.TypeMeasurements == MeasurementType.Level) {
                                        YXbsLevelmeasres dtr = new YXbsLevelmeasres(ConnectDB.Connect_Main_);
                                        dtr.Format("*");
                                        if (dt_param != null) {
                                            if (dtr.Fetch(string.Format("id_xbs_measurementres={0}", measRes.m_id))) {
                                                dtr.Delete();
                                            }
                                        }
                                        dtr.Close();
                                        dtr.Dispose();

                                    }
                                    else if (obj.TypeMeasurements == MeasurementType.SpectrumOccupation) {
                                        YXbsSpectoccupmeas dtr = new YXbsSpectoccupmeas(ConnectDB.Connect_Main_);
                                        dtr.Format("*");
                                        if (dt_param != null) {
                                            if (dtr.Fetch(string.Format("id_xbs_measurementres={0}", measRes.m_id))) {
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
                    else if (obj.Id.MeasTaskId != null) {
                        {
                            measRes.Filter = string.Format(" (meastaskid={0}) ", obj.Id.MeasTaskId.Value);
                            for (measRes.OpenRs(); !measRes.IsEOF(); measRes.MoveNext()) {
                                foreach (FrequencyMeasurement dt_param in obj.FrequenciesMeasurements.ToArray()) {
                                    YXbsFrequencymeas dtr = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                                    dtr.Format("*");
                                    if (dt_param != null) {
                                        dtr.Filter = string.Format("id_xbs_measurementres={0}", measRes.m_id);
                                        for (dtr.OpenRs(); !dtr.IsEOF(); dtr.MoveNext()) {
                                            dtr.Delete();
                                        }
                                    }
                                    dtr.Close();
                                    dtr.Dispose();
                                }

                                foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray()) {
                                    YXbsLocationsensorm dtr = new YXbsLocationsensorm(ConnectDB.Connect_Main_);
                                    dtr.Format("*");
                                    if (dt_param != null)  {
                                        dtr.Filter = string.Format("id_xbs_measurementres={0}", measRes.m_id);
                                        for (dtr.OpenRs(); !dtr.IsEOF(); dtr.MoveNext()) {
                                            dtr.Delete();
                                        }
                                    }
                                    dtr.Close();
                                    dtr.Dispose();
                                }

                                foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray()) {
                                    if (obj.TypeMeasurements == MeasurementType.Level) {
                                        YXbsLevelmeasres dtr = new YXbsLevelmeasres(ConnectDB.Connect_Main_);
                                        dtr.Format("*");
                                        if (dt_param != null) {
                                            dtr.Filter = string.Format("id_xbs_measurementres={0}", measRes.m_id);
                                            for (dtr.OpenRs(); !dtr.IsEOF(); dtr.MoveNext()) {
                                                dtr.Delete();
                                            }
                                        }
                                        dtr.Close();
                                        dtr.Dispose();

                                    }
                                    else if (obj.TypeMeasurements == MeasurementType.SpectrumOccupation) {
                                        YXbsSpectoccupmeas dtr = new YXbsSpectoccupmeas(ConnectDB.Connect_Main_);
                                        dtr.Format("*");
                                        if (dt_param != null) {
                                            dtr.Filter = string.Format("id_xbs_measurementres={0}", measRes.m_id);
                                            for (dtr.OpenRs(); !dtr.IsEOF(); dtr.MoveNext()) {
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
            }
            catch (Exception)
            { isSuccess = false; }
            return isSuccess;
        }


        public int SaveResultToDB(MeasurementResults obj)
        {
            int ID = ConnectDB.NullI;
            if (((obj.TypeMeasurements == MeasurementType.SpectrumOccupation) && (obj.Status == "C")) || (obj.TypeMeasurements != MeasurementType.SpectrumOccupation))
            {
                //Task tsk = new Task(() =>
                //{
                    try
                    {
                        /// Create new record in YXbsMeastask
                        if (obj != null)
                        {
                            if ((obj.Id.MeasTaskId != null) && (obj.StationMeasurements != null) && (obj.Id.SubMeasTaskId != ConnectDB.NullI) && (obj.Id.SubMeasTaskStationId != ConnectDB.NullI))
                            {
                            if (obj.StationMeasurements.StationId != null)
                            {
                                YXbsMeasurementres measRes = new YXbsMeasurementres(ConnectDB.Connect_Main_);
                                measRes.Format("*");
                                measRes.Filter = "[ID]=-1";
                                measRes.New();
                                ID = measRes.AllocID();
                                obj.Id.MeasSdrResultsId = ID;
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
                                measRes.Save();
                                measRes.Close();
                                measRes.Dispose();
                            }
                            }
                            if (ID != ConnectDB.NullI)
                            {

                                if (obj.LocationSensorMeasurement != null)
                                {
                                    foreach (LocationSensorMeasurement dt_param in obj.LocationSensorMeasurement.ToArray())
                                    {
                                        if (dt_param != null)
                                        {
                                                YXbsLocationsensorm dtr = new YXbsLocationsensorm(ConnectDB.Connect_Main_);
                                                dtr.Format("*");
                                                dtr.Filter = "[ID]=-1";
                                                dtr.New();
                                                int ID_DT_params = dtr.AllocID();
                                                if (dt_param.ASL != null) dtr.m_asl = dt_param.ASL.GetValueOrDefault();
                                                if (dt_param.Lon != null) dtr.m_lon = dt_param.Lon.GetValueOrDefault();
                                                if (dt_param.Lat != null) dtr.m_lat = dt_param.Lat.GetValueOrDefault();
                                                dtr.m_id_xbs_measurementres = ID;
                                                dtr.Save();
                                                dtr.Close();
                                                dtr.Dispose();
                                        }
                                    }
                                }
                                if (obj.MeasurementsResults != null)
                                {
                                    int idx_cnt = 0;
                                    foreach (MeasurementResult dt_param in obj.MeasurementsResults.ToArray())
                                    {
                                        if ((obj.TypeMeasurements == MeasurementType.Level) && (obj.Status != "O"))
                                        {
                                            if (dt_param != null)
                                            {
                                                if (dt_param is LevelMeasurementResult)
                                                {
                                                    YXbsLevelmeasres dtrR = new YXbsLevelmeasres(ConnectDB.Connect_Main_);
                                                    dtrR.Format("*");
                                                    dtrR.Filter = "[ID]=-1";
                                                    dtrR.New();
                                                    int ID_DT_params = dtrR.AllocID();
                                                    if ((dt_param as LevelMeasurementResult).Value != null) dtrR.m_value = (dt_param as LevelMeasurementResult).Value.GetValueOrDefault();
                                                    if ((dt_param as LevelMeasurementResult).PMax != null) dtrR.m_pmax = (dt_param as LevelMeasurementResult).PMax.GetValueOrDefault();
                                                    if ((dt_param as LevelMeasurementResult).PMin != null) dtrR.m_pmin = (dt_param as LevelMeasurementResult).PMin.GetValueOrDefault();
                                                    dtrR.m_id_xbs_measurementres = ID;
                                                    dtrR.Save();
                                                    dtrR.Close();
                                                    dtrR.Dispose();

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
                                                                    YXbsFrequencymeas dtr_freq = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                                                                    dtr_freq.Format("*");
                                                                    dtr_freq.Filter = "[ID]=-1";
                                                                    dtr_freq.New();
                                                                    int ID_DT_params_freq = dtr_freq.AllocID();
                                                                    dtr_freq.m_freq = dt_param_freq.Freq;
                                                                    dtr_freq.m_id_xbs_measurementres = ID;
                                                                    dtr_freq.m_num = ID_DT_params;
                                                                    dtr_freq.Save();
                                                                    dt_param_freq.Id = ID_DT_params;
                                                                    dtr_freq.Close();
                                                                    dtr_freq.Dispose();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    }
                                                    dt_param.Id = new MeasurementResultIdentifier();
                                                    dt_param.Id.Value = ID_DT_params;
                                                }
                                            }
                                        }
                                        else if ((obj.TypeMeasurements == MeasurementType.SpectrumOccupation) && (obj.Status == "C"))
                                        {
                                            if (dt_param != null)
                                            {
                                                if (dt_param is SpectrumOccupationMeasurementResult)
                                                {
                                                    YXbsSpectoccupmeas dtr = new YXbsSpectoccupmeas(ConnectDB.Connect_Main_);
                                                    dtr.Format("*");
                                                    dtr.Filter = "[ID]=-1";
                                                    dtr.New();
                                                    int ID_DT_params = dtr.AllocID();
                                                    if ((dt_param as SpectrumOccupationMeasurementResult).Value != null) dtr.m_value = (dt_param as SpectrumOccupationMeasurementResult).Value.GetValueOrDefault();
                                                    if ((dt_param as SpectrumOccupationMeasurementResult).Occupancy != null) dtr.m_occupancy = (dt_param as SpectrumOccupationMeasurementResult).Occupancy.GetValueOrDefault();
                                                    dtr.m_id_xbs_measurementres = ID;
                                                    dtr.Save();
                                                    dtr.Close();
                                                    dtr.Dispose();


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
                                                                    YXbsFrequencymeas dtr_freq = new YXbsFrequencymeas(ConnectDB.Connect_Main_);
                                                                    dtr_freq.Format("*");
                                                                    dtr_freq.Filter = "[ID]=-1";
                                                                    dtr_freq.New();
                                                                    int ID_DT_params_freq = dtr_freq.AllocID();
                                                                    dtr_freq.m_freq = dt_param_freq.Freq;
                                                                    dtr_freq.m_id_xbs_measurementres = ID;
                                                                    dtr_freq.m_num = ID_DT_params;
                                                                    dtr_freq.Save();
                                                                    dt_param_freq.Id = ID_DT_params;
                                                                    dtr_freq.Close();
                                                                    dtr_freq.Dispose();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    }
                                                    dt_param.Id = new MeasurementResultIdentifier();
                                                    dt_param.Id.Value = ID_DT_params;
                                                }
                                            }
                                        }
                                        //else 
                                        {
                                            if (dt_param != null)
                                            {
                                                if (dt_param is LevelMeasurementOnlineResult)
                                                {
                                                    YXbsLevelmeasonlres dtr = new YXbsLevelmeasonlres(ConnectDB.Connect_Main_);
                                                    dtr.Format("*");
                                                    dtr.Filter = "[ID]=-1";
                                                    dtr.New();
                                                    int ID_DT_params = dtr.AllocID();
                                                    if ((dt_param as LevelMeasurementOnlineResult).Value != ConnectDB.NullD) dtr.m_value = (dt_param as LevelMeasurementOnlineResult).Value;
                                                    dtr.m_id_xbs_measurementres = ID;
                                                    dtr.Save();
                                                    dt_param.Id = new MeasurementResultIdentifier();
                                                    dt_param.Id.Value = ID_DT_params;
                                                    dtr.Close();
                                                    dtr.Dispose();
                                                }
                                            }
                                        }
                                        idx_cnt++;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("SaveResultToDB: " + ex.Message);
                    }
                //});
                //tsk.Start();
                //tsk.Wait(30000);
            }
            return ID;
        }
        
    }
}
