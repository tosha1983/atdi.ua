using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Modules.MonitoringProcess;
using Atdi.Sdrn.Modules.MonitoringProcess;
using NHibernate;
using NHibernate.Criterion;
using Atdi.AppUnits.Sdrn.ControlA;

namespace Atdi.AppUnits.Sdrn.ControlA.Handlers
{
    /// <summary>
    /// Класс для валидации тасков.
    /// </summary>
    public class CheckMeasTask
    {
        // Проверка на координаты (пока что заглушка)
        public static bool CheckCoordinate(NH_MeasTaskSDR M, double LAT, double LON, double ASL)
        {
            bool isCheck = true;

            return isCheck;
        }

        /// <summary>
        /// Проверка на совместимость параметров оборудования (пока что заглушка)
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public static bool CheckTechnicalParams(NH_MeasTaskSDR M)
        {
            bool isCheck = true;

            return isCheck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public static bool CheckValidate(MeasSdrTask M)
        {
            bool Check = true;
            //if ((M.status == "A") || (M.status == "O") || (M.status == "P") || (M.status == "E_L"))
            {
                if (M.Time_stop < DateTime.Now)
                {
                    Check = false;
                    //M.status = "E_T";
                }
                else
                {
                    Check = true;
                }
            }
            //else Check = false;
            return Check;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoadDataMeasTask
    {

        /// <summary>
        /// Основной цикл обработки тасков
        /// </summary>
        public void ProcessBB60C()
        {
            SensorDBExtension sensorDBExtension = new SensorDBExtension();
            MeasurementProcessing MeasProcessing = new MeasurementProcessing();
            Sensor se_ = sensorDBExtension.GetCurrentSensor();
            if (se_ != null)
            {
                LoadDataMeasTask ld = new LoadDataMeasTask();
                MeasSdrResults Res = new MeasSdrResults();
                List<MeasSdrResults> Res_History = new List<MeasSdrResults>();
                SaveMeasSDRResults sv_SdrRes = new SaveMeasSDRResults();
                SaveMeasTaskSDR svTaskSDR = new SaveMeasTaskSDR();
                Action task_ssb = new Action(() =>
                {
                    while (true)
                    {
                        try
                        {
                            MeasSdrTask M_DR = new MeasSdrTask();
                            {
                                var out_list = ld.GetAllMeasTaskSDR();
                                if (out_list != null)
                                {
                                    List<KeyValuePair<MeasSdrTask, DateTime>> asd = new List<KeyValuePair<MeasSdrTask, DateTime>>();
                                    List<KeyValuePair<MeasSdrTask, int>> asi = new List<KeyValuePair<MeasSdrTask, int>>();
                                    List<DateTime> IDS = new List<DateTime>();
                                    {
                                        foreach (MeasSdrTask tsk_prio in out_list.ToArray())
                                        {
                                            if (tsk_prio != null)
                                            {
                                                if ((tsk_prio.status == "O") && ((tsk_prio.Time_start < DateTime.Now) && (tsk_prio.Time_stop > DateTime.Now)))
                                                {
                                                    if (tsk_prio.MeasTaskId != null)
                                                    {
                                                        var DataMeas = sv_SdrRes.LoadDataMeasByTaskId(tsk_prio.MeasTaskId.Value);
                                                        if (DataMeas == null)
                                                        {
                                                            M_DR = tsk_prio;
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            asd.Add(new KeyValuePair<MeasSdrTask, DateTime>(tsk_prio, DataMeas.Value));
                                                            IDS.Add(DataMeas.Value);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (asd.Count > 0)
                                        {
                                            IDS.Sort();
                                            if (IDS.Count > 0)
                                            {
                                                KeyValuePair<MeasSdrTask, DateTime> vv = asd.Find(c => c.Value == IDS[0]);
                                                if (vv.Key != null)
                                                {
                                                    M_DR = vv.Key;
                                                }
                                            }
                                        }
                                        if (M_DR.MeasTaskId == null)
                                        {
                                            List<int> IR = new List<int>();
                                            foreach (MeasSdrTask tsk_prio in out_list.ToArray())
                                            {
                                                if (tsk_prio != null)
                                                {
                                                    if ((tsk_prio.status == "A") && ((tsk_prio.Time_start < DateTime.Now) && (tsk_prio.Time_stop > DateTime.Now)))
                                                    {
                                                        asi.Add(new KeyValuePair<MeasSdrTask, int>(tsk_prio, tsk_prio.prio));
                                                        IR.Add(tsk_prio.prio);
                                                    }
                                                }
                                            }
                                            IR.Sort();
                                            IR.Reverse();
                                            if (IR.Count > 0)
                                            {
                                                KeyValuePair<MeasSdrTask, int> vv = asi.Find(c => c.Value == IR[0]);
                                                if (vv.Key != null)
                                                {
                                                    M_DR = vv.Key;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (M_DR == null) continue;
                                if (M_DR.MeasTaskId != null)
                                {
                                    if ((M_DR.Time_start < DateTime.Now) && (M_DR.Time_stop > DateTime.Now))
                                    {
                                        MeasSdrResults Res_H = Res_History.Find(t => t.MeasSubTaskId.Value == M_DR.MeasSubTaskId.Value && t.MeasSubTaskStationId == M_DR.MeasSubTaskStationId && t.MeasTaskId.Value == M_DR.MeasTaskId.Value && t.SensorId.Value == M_DR.SensorId.Value);
                                        if (Res_H != null)
                                        {
                                            System.Console.WriteLine(string.Format("Res_H.TaskId = {0}, Res_H.NN = {1}", Res_H.MeasTaskId.Value, Res_H.NN));
                                            CirculatingData circulatingData = new CirculatingData();
                                            Res = MeasProcessing.TaskProcessing(BusManager.SDR, M_DR, se_, ref circulatingData, Res_H) as MeasSdrResults;
                                            Res_History.RemoveAll(t => t.MeasSubTaskId.Value == M_DR.MeasSubTaskId.Value && t.MeasSubTaskStationId == M_DR.MeasSubTaskStationId && t.MeasTaskId.Value == M_DR.MeasTaskId.Value && t.SensorId.Value == M_DR.SensorId.Value);
                                            if (Res.MeasSubTaskId != null)
                                                Res_History.Add(Res);
                                        }
                                        else
                                        {
                                            CirculatingData circulatingData = new CirculatingData();
                                            Res = MeasProcessing.TaskProcessing(BusManager.SDR, M_DR, se_, ref circulatingData, Res_H) as MeasSdrResults;
                                            Res_History.RemoveAll(t => t.MeasSubTaskId.Value == M_DR.MeasSubTaskId.Value && t.MeasSubTaskStationId == M_DR.MeasSubTaskStationId && t.MeasTaskId.Value == M_DR.MeasTaskId.Value && t.SensorId.Value == M_DR.SensorId.Value);
                                            if (Res.MeasSubTaskId != null)
                                            {
                                                Res_History.Add(Res);
                                                System.Console.WriteLine(string.Format("Res.TaskId = {0}, Res.NN = {1}", Res.MeasTaskId.Value, Res.NN));
                                            }
                                        }
                                        if (Res != null)
                                        {
                                            int idx_max = 1;
                                            int maxValue = sv_SdrRes.GetMaxIdFromResults();
                                            if (maxValue >= 0)
                                            {
                                                idx_max++;
                                            }
                                            Res.Id = idx_max;
                                            if (Res.FSemples != null)
                                            {
                                                if (Res.FSemples.Count() > 0)
                                                {
                                                    if (M_DR.MeasDataType != MeasurementType.SpectrumOccupation)
                                                    {
                                                        sv_SdrRes.SaveMeasResultSDR(Res, "C");
                                                        BusManager._messagePublisher.Send("SendMeasSdrResults", Res);
                                                        if (SaveMeasSDRResults.SaveStatusMeasTaskSDRResults(Res, "C"))
                                                        {
                                                            SaveMeasSDRResults.SaveisSendMeasTaskSDRResults(Res);
                                                        }
                                                    }
                                                    else if (M_DR.MeasDataType == MeasurementType.SpectrumOccupation)
                                                    {
                                                        sv_SdrRes.SaveMeasResultSDR(Res, "C");
                                                        BusManager._messagePublisher.Send("SendMeasSdrResults", Res);
                                                        BusManager.Counter_Online = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if ((Res.Freqs != null) || (Res.Level != null))
                                                    {
                                                        BusManager.Counter_Online++;
                                                        Res.Id = BusManager.Counter_Online;
                                                        Res.status = M_DR.status;
                                                        // Отправка данных в шину сразу после записи в БД
                                                        BusManager._messagePublisher.Send("SendMeasSdrResults", Res);
                                                        System.Console.WriteLine("Online ...");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((Res.Freqs != null) || (Res.Level != null))
                                                {
                                                    BusManager.Counter_Online++;
                                                    Res.Id = BusManager.Counter_Online;
                                                    Res.status = M_DR.status;
                                                    // Отправка данных в шину сразу после записи в БД
                                                    BusManager._messagePublisher.Send("SendMeasSdrResults", Res);
                                                    System.Console.WriteLine("Online ...");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (M_DR.Time_stop < DateTime.Now)
                                        {
                                            M_DR.status = "C";
                                            svTaskSDR.ArchiveMeasTaskSDR(M_DR, "C");
                                            if (M_DR.MeasDataType == MeasurementType.SpectrumOccupation)
                                            {
                                                sv_SdrRes.SaveMeasResultSDR(Res, "C");
                                            }
                                            
                                            var ResDX = FindMeasTaskSDR(Res.SensorId.Value, Res.MeasTaskId.Value, Res.MeasSubTaskStationId, Res.MeasSubTaskId.Value);
                                            if ((ResDX != null) && (ResDX.Count>0))
                                            {
                                                ResDX[0].status = Res.status;
                                                SaveMeasSDRResults.SaveStatusMeasTaskSDR(ResDX[0]);
                                            }
                                        }
                                    }
                                    if (M_DR != null)
                                    {
                                        if (M_DR.MeasSubTaskId != null)
                                        {
                                            SaveMeasSDRResults.SaveStatusMeasTaskSDR(M_DR);
                                        }
                                    }


                                }
                            }
                        }
                        catch (Exception ex)
                        { /*GlobalInit.log.Trace("Error in ProcessWorkBB60C: " + ex.Message); */ }
                    }
                });
                Task task_swwsb = new Task(task_ssb);
                task_swwsb.Start();
            }

        }



        public List<MeasSdrTask> FindMeasTaskSDR(int SensorId, int MeasTaskId, int MeasSubTaskStationId, int MeasSubTaskId)
        {
            List<MeasSdrTask> listMeasSdrTask = new List<MeasSdrTask>();
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    #region NH_Meas_Task_SDR
                    {
                        session.Clear();
                        var s_l_NH_Meas_Task_SDR = session.QueryOver<NH_MeasTaskSDR>().Fetch(x => x.ID).Eager;
                        if (s_l_NH_Meas_Task_SDR.RowCount() > 0)
                        {
                            List<NH_MeasTaskSDR> F = (List<NH_MeasTaskSDR>)s_l_NH_Meas_Task_SDR.Where(t => t.MeasSubTaskId == MeasSubTaskId && t.MeasSubTaskStationId == MeasSubTaskStationId && t.MeasTaskId == MeasTaskId && t.SensorId == SensorId).List();
                            foreach (NH_MeasTaskSDR s in F)
                            {
                                MeasSdrTask M_DR = new MeasSdrTask();
                                M_DR.MeasTaskId = new MeasTaskIdentifier();
                                if (s.MeasTaskId != null) M_DR.MeasTaskId.Value = s.MeasTaskId.GetValueOrDefault();
                                M_DR.MeasSubTaskId = new MeasTaskIdentifier();
                                if (s.MeasSubTaskId != null) M_DR.MeasSubTaskId.Value = s.MeasSubTaskId.GetValueOrDefault();
                                if (s.ID != null) M_DR.Id = s.ID.GetValueOrDefault();
                                if (s.MeasSubTaskStationId != null) M_DR.MeasSubTaskStationId = s.MeasSubTaskStationId.GetValueOrDefault();
                                MeasurementType out_MeasurementType;
                                if (Enum.TryParse<MeasurementType>(s.MeasDataType, out out_MeasurementType))
                                    M_DR.MeasDataType = out_MeasurementType;
                                M_DR.status = s.status;
                                M_DR.SensorId = new SensorIdentifier();
                                if (s.SensorId != null) M_DR.SensorId.Value = s.SensorId.GetValueOrDefault();
                                if (s.SwNumber != null) M_DR.SwNumber = s.SwNumber.GetValueOrDefault();
                                if (s.Time_start != null) M_DR.Time_start = s.Time_start.GetValueOrDefault();
                                if (s.Time_stop != null) M_DR.Time_stop = s.Time_stop.GetValueOrDefault();
                                M_DR.NumberScanPerTask = -999;
                                if (s.PerInterval != null) M_DR.PerInterval = s.PerInterval.GetValueOrDefault();

                                SpectrumScanType out_SpectrumScanType;
                                if (Enum.TryParse<SpectrumScanType>(s.TypeM, out out_SpectrumScanType))
                                    M_DR.TypeM = out_SpectrumScanType;


                                M_DR.MeasSDRSOParam = new MeasSdrSOParam();
                                var s_l_NH_SDR_So_Param = session.QueryOver<NH_MeasSDRSOParam>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_SDR_So_Param.RowCount() > 0)
                                {
                                    List<NH_MeasSDRSOParam> F_SDR_So_Param = (List<NH_MeasSDRSOParam>)s_l_NH_SDR_So_Param.Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                    foreach (NH_MeasSDRSOParam s_so_param in F_SDR_So_Param)
                                    {
                                        if (s_so_param.LevelMinOccup != null) M_DR.MeasSDRSOParam.LevelMinOccup = s_so_param.LevelMinOccup.GetValueOrDefault();
                                        if (s_so_param.NChenal != null) M_DR.MeasSDRSOParam.NChenal = s_so_param.NChenal.GetValueOrDefault();

                                        SpectrumOccupationType out_SpectrumOccupationType;
                                        if (Enum.TryParse<SpectrumOccupationType>(s_so_param.TypeSO, out out_SpectrumOccupationType))
                                            M_DR.MeasSDRSOParam.TypeSO = out_SpectrumOccupationType;
                                        break;
                                    }
                                }
                                //--------------------------------------------------
                                M_DR.MeasSDRParam = new MeasSdrParam();
                                var s_l_NH_MEAS_SDR_PARAM = session.QueryOver<NH_MeasSDRParam>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_MEAS_SDR_PARAM.RowCount() > 0)
                                {
                                    List<NH_MeasSDRParam> F_SDR_Sdr_Param = (List<NH_MeasSDRParam>)s_l_NH_MEAS_SDR_PARAM.Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                    foreach (NH_MeasSDRParam s_sdr_param in F_SDR_Sdr_Param)
                                    {
                                        DetectingType out_DetectingType;
                                        if (Enum.TryParse<DetectingType>(s_sdr_param.DetectTypeSDR, out out_DetectingType))
                                            M_DR.MeasSDRParam.DetectTypeSDR = out_DetectingType;

                                        //M_DR.MeasSDRParam. ID = s_sdr_param.ID.GetValueOrDefault();
                                        if (s_sdr_param.PreamplificationSDR != null) M_DR.MeasSDRParam.PreamplificationSDR = s_sdr_param.PreamplificationSDR.GetValueOrDefault();
                                        if (s_sdr_param.VBW != null) M_DR.MeasSDRParam.VBW = s_sdr_param.VBW.GetValueOrDefault();
                                        if (s_sdr_param.RBW != null) M_DR.MeasSDRParam.RBW = s_sdr_param.RBW.GetValueOrDefault();
                                        if (s_sdr_param.ref_level_dbm != null) M_DR.MeasSDRParam.ref_level_dbm = s_sdr_param.ref_level_dbm.GetValueOrDefault();
                                        if (s_sdr_param.RfAttenuationSDR != null) M_DR.MeasSDRParam.RfAttenuationSDR = s_sdr_param.RfAttenuationSDR.GetValueOrDefault();
                                        if (s_sdr_param.MeasTime != null) M_DR.MeasSDRParam.MeasTime = s_sdr_param.MeasTime.GetValueOrDefault();
                                        break;
                                    }
                                }
                                //--------------------------------------------------
                                List<MeasLocParam> LMSZ = new List<MeasLocParam>();
                                M_DR.MeasLocParam = (new List<MeasLocParam>()).ToArray();
                                var s_l_NH_MEAS_SDR_LOC_PARAM = session.QueryOver<NH_MeasSDRLoc>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_MEAS_SDR_LOC_PARAM.RowCount() > 0)
                                {
                                    List<NH_MeasSDRLoc> F_SDR_Sdr_Loc_Param = (List<NH_MeasSDRLoc>)s_l_NH_MEAS_SDR_LOC_PARAM.Where(t => t.ID_MeasSDRID == s.ID).List();
                                    foreach (NH_MeasSDRLoc s_sdr_loc_param in F_SDR_Sdr_Loc_Param)
                                    {
                                        MeasLocParam cr_sdr_loc = new MeasLocParam();
                                        if (s_sdr_loc_param.ASL != null) cr_sdr_loc.ASL = s_sdr_loc_param.ASL.GetValueOrDefault();
                                        if (s_sdr_loc_param.Lat != null) cr_sdr_loc.Lat = s_sdr_loc_param.Lat.GetValueOrDefault();
                                        if (s_sdr_loc_param.Lon != null) cr_sdr_loc.Lon = s_sdr_loc_param.Lon.GetValueOrDefault();
                                        //cr_sdr_loc.MAX_DIST = s_sdr_loc_param.MAX_DIST.GetValueOrDefault();
                                        LMSZ.Add(cr_sdr_loc);
                                    }
                                }
                                M_DR.MeasLocParam = LMSZ.ToArray();
                                //--------------------------------------------------
                                M_DR.MeasFreqParam = new MeasFreqParam();
                                var s_l_NH_MEAS_SDR_FREQ_PARAM = session.QueryOver<NH_MeasSDRFreqParam>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_MEAS_SDR_FREQ_PARAM.RowCount() > 0)
                                {
                                    List<NH_MeasSDRFreqParam> F_SDR_MEAS_SDR_FREQ_PARAM = (List<NH_MeasSDRFreqParam>)s_l_NH_MEAS_SDR_FREQ_PARAM.Where(t => t.id_meas_task_sdr == s.ID).List();
                                    foreach (NH_MeasSDRFreqParam s_sdr_freq_param in F_SDR_MEAS_SDR_FREQ_PARAM)
                                    {
                                        //M_DR.MeasFreqParam.BW_CH = s_sdr_freq_param.BW_CH.GetValueOrDefault();
                                        if (s_sdr_freq_param.RgU != null) M_DR.MeasFreqParam.RgU = s_sdr_freq_param.RgU.GetValueOrDefault();
                                        if (s_sdr_freq_param.RgL != null) M_DR.MeasFreqParam.RgL = s_sdr_freq_param.RgL.GetValueOrDefault();
                                        if (s_sdr_freq_param.Step != null) M_DR.MeasFreqParam.Step = s_sdr_freq_param.Step.GetValueOrDefault();
                                        FrequencyMode out_FrequencyMode;
                                        if (Enum.TryParse<FrequencyMode>(s_sdr_freq_param.Mode, out out_FrequencyMode))
                                            M_DR.MeasFreqParam.Mode = out_FrequencyMode;

                                        List<MeasFreq> FRL = new List<MeasFreq>();
                                        var s_l_NH_MEAS_FREQ_LST = session.QueryOver<NH_MeasSDRFreq>().Fetch(x => x.ID).Eager;
                                        if (s_l_NH_MEAS_FREQ_LST.RowCount() > 0)
                                        {
                                            List<NH_MeasSDRFreq> F_SDR_MEAS_FREQ_LST = (List<NH_MeasSDRFreq>)s_l_NH_MEAS_FREQ_LST.Where(t => t.ID_MeasSDRFreqParam == s_sdr_freq_param.ID).List();
                                            foreach (NH_MeasSDRFreq s_sdr_freq_lst in F_SDR_MEAS_FREQ_LST)
                                            {
                                                MeasFreq FR_L = new MeasFreq();
                                                if (s_sdr_freq_lst.Freq != null) FR_L.Freq = s_sdr_freq_lst.Freq.GetValueOrDefault();
                                                FRL.Add(FR_L);
                                            }
                                        }
                                        M_DR.MeasFreqParam.MeasFreqs = FRL.ToArray();
                                        break;
                                    }
                                }
                                listMeasSdrTask.Add(M_DR);
                            }
                        }

                    }
                    #endregion
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in ProcessMultyMeas: " + ex.Message);*/ }
            return listMeasSdrTask;
        }


        public List<MeasSdrTask> GetAllMeasTaskSDR()
        {
            List<MeasSdrTask> listMeasSdrTask = new List<MeasSdrTask>();
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    #region NH_Meas_Task_SDR
                    {
                        session.Clear();
                        var s_l_NH_Meas_Task_SDR = session.QueryOver<NH_MeasTaskSDR>().Fetch(x => x.ID).Eager;
                        if (s_l_NH_Meas_Task_SDR.RowCount() > 0)
                        {
                            List<NH_MeasTaskSDR> F = (List<NH_MeasTaskSDR>)s_l_NH_Meas_Task_SDR.Where(t => t.status!="Z" && t.status != "F" && t.status != "P").List();
                            foreach (NH_MeasTaskSDR s in F)
                            {
                                MeasSdrTask M_DR = new MeasSdrTask();

                                M_DR.MeasTaskId = new MeasTaskIdentifier();
                                if (s.MeasTaskId != null) M_DR.MeasTaskId.Value = s.MeasTaskId.GetValueOrDefault();
                                M_DR.MeasSubTaskId = new MeasTaskIdentifier();
                                if (s.MeasSubTaskId != null) M_DR.MeasSubTaskId.Value = s.MeasSubTaskId.GetValueOrDefault();
                                if (s.ID != null) M_DR.Id = s.ID.GetValueOrDefault();
                                if (s.MeasSubTaskStationId != null) M_DR.MeasSubTaskStationId = s.MeasSubTaskStationId.GetValueOrDefault();
                                MeasurementType out_MeasurementType;
                                if (Enum.TryParse<MeasurementType>(s.MeasDataType, out out_MeasurementType))
                                    M_DR.MeasDataType = out_MeasurementType;
                                M_DR.status = s.status;
                                M_DR.SensorId = new SensorIdentifier();
                                if (s.SensorId != null) M_DR.SensorId.Value = s.SensorId.GetValueOrDefault();
                                if (s.SwNumber != null) M_DR.SwNumber = s.SwNumber.GetValueOrDefault();
                                if (s.Time_start != null) M_DR.Time_start = s.Time_start.GetValueOrDefault();
                                if (s.Time_stop != null) M_DR.Time_stop = s.Time_stop.GetValueOrDefault();
                                M_DR.NumberScanPerTask = -999;
                                if (s.PerInterval != null) M_DR.PerInterval = s.PerInterval.GetValueOrDefault();

                                SpectrumScanType out_SpectrumScanType;
                                if (Enum.TryParse<SpectrumScanType>(s.TypeM, out out_SpectrumScanType))
                                    M_DR.TypeM = out_SpectrumScanType;


                                M_DR.MeasSDRSOParam = new MeasSdrSOParam();
                                var s_l_NH_SDR_So_Param = session.QueryOver<NH_MeasSDRSOParam>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_SDR_So_Param.RowCount() > 0)
                                {
                                    List<NH_MeasSDRSOParam> F_SDR_So_Param = (List<NH_MeasSDRSOParam>)s_l_NH_SDR_So_Param.Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                    foreach (NH_MeasSDRSOParam s_so_param in F_SDR_So_Param)
                                    {
                                        if (s_so_param.LevelMinOccup != null) M_DR.MeasSDRSOParam.LevelMinOccup = s_so_param.LevelMinOccup.GetValueOrDefault();
                                        if (s_so_param.NChenal != null) M_DR.MeasSDRSOParam.NChenal = s_so_param.NChenal.GetValueOrDefault();

                                        SpectrumOccupationType out_SpectrumOccupationType;
                                        if (Enum.TryParse<SpectrumOccupationType>(s_so_param.TypeSO, out out_SpectrumOccupationType))
                                            M_DR.MeasSDRSOParam.TypeSO = out_SpectrumOccupationType;
                                        break;
                                    }
                                }
                                //--------------------------------------------------
                                M_DR.MeasSDRParam = new MeasSdrParam();
                                var s_l_NH_MEAS_SDR_PARAM = session.QueryOver<NH_MeasSDRParam>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_MEAS_SDR_PARAM.RowCount() > 0)
                                {
                                    List<NH_MeasSDRParam> F_SDR_Sdr_Param = (List<NH_MeasSDRParam>)s_l_NH_MEAS_SDR_PARAM.Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                    foreach (NH_MeasSDRParam s_sdr_param in F_SDR_Sdr_Param)
                                    {
                                        DetectingType out_DetectingType;
                                        if (Enum.TryParse<DetectingType>(s_sdr_param.DetectTypeSDR, out out_DetectingType))
                                            M_DR.MeasSDRParam.DetectTypeSDR = out_DetectingType;

                                        //M_DR.MeasSDRParam. ID = s_sdr_param.ID.GetValueOrDefault();
                                        if (s_sdr_param.PreamplificationSDR != null) M_DR.MeasSDRParam.PreamplificationSDR = s_sdr_param.PreamplificationSDR.GetValueOrDefault();
                                        if (s_sdr_param.VBW != null) M_DR.MeasSDRParam.VBW = s_sdr_param.VBW.GetValueOrDefault();
                                        if (s_sdr_param.RBW != null) M_DR.MeasSDRParam.RBW = s_sdr_param.RBW.GetValueOrDefault();
                                        if (s_sdr_param.ref_level_dbm != null) M_DR.MeasSDRParam.ref_level_dbm = s_sdr_param.ref_level_dbm.GetValueOrDefault();
                                        if (s_sdr_param.RfAttenuationSDR != null) M_DR.MeasSDRParam.RfAttenuationSDR = s_sdr_param.RfAttenuationSDR.GetValueOrDefault();
                                        if (s_sdr_param.MeasTime != null) M_DR.MeasSDRParam.MeasTime = s_sdr_param.MeasTime.GetValueOrDefault();
                                        break;
                                    }
                                }
                                //--------------------------------------------------
                                List<MeasLocParam> LMSZ = new List<MeasLocParam>();
                                M_DR.MeasLocParam = (new List<MeasLocParam>()).ToArray();
                                var s_l_NH_MEAS_SDR_LOC_PARAM = session.QueryOver<NH_MeasSDRLoc>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_MEAS_SDR_LOC_PARAM.RowCount() > 0)
                                {
                                    List<NH_MeasSDRLoc> F_SDR_Sdr_Loc_Param = (List<NH_MeasSDRLoc>)s_l_NH_MEAS_SDR_LOC_PARAM.Where(t => t.ID_MeasSDRID == s.ID).List();
                                    foreach (NH_MeasSDRLoc s_sdr_loc_param in F_SDR_Sdr_Loc_Param)
                                    {
                                        MeasLocParam cr_sdr_loc = new MeasLocParam();
                                        if (s_sdr_loc_param.ASL != null) cr_sdr_loc.ASL = s_sdr_loc_param.ASL.GetValueOrDefault();
                                        if (s_sdr_loc_param.Lat != null) cr_sdr_loc.Lat = s_sdr_loc_param.Lat.GetValueOrDefault();
                                        if (s_sdr_loc_param.Lon != null) cr_sdr_loc.Lon = s_sdr_loc_param.Lon.GetValueOrDefault();
                                        //cr_sdr_loc.MAX_DIST = s_sdr_loc_param.MAX_DIST.GetValueOrDefault();
                                        LMSZ.Add(cr_sdr_loc);
                                    }
                                }
                                M_DR.MeasLocParam = LMSZ.ToArray();
                                //--------------------------------------------------
                                M_DR.MeasFreqParam = new MeasFreqParam();
                                var s_l_NH_MEAS_SDR_FREQ_PARAM = session.QueryOver<NH_MeasSDRFreqParam>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_MEAS_SDR_FREQ_PARAM.RowCount() > 0)
                                {
                                    List<NH_MeasSDRFreqParam> F_SDR_MEAS_SDR_FREQ_PARAM = (List<NH_MeasSDRFreqParam>)s_l_NH_MEAS_SDR_FREQ_PARAM.Where(t => t.id_meas_task_sdr == s.ID).List();
                                    foreach (NH_MeasSDRFreqParam s_sdr_freq_param in F_SDR_MEAS_SDR_FREQ_PARAM)
                                    {
                                        //M_DR.MeasFreqParam.BW_CH = s_sdr_freq_param.BW_CH.GetValueOrDefault();
                                        if (s_sdr_freq_param.RgU != null) M_DR.MeasFreqParam.RgU = s_sdr_freq_param.RgU.GetValueOrDefault();
                                        if (s_sdr_freq_param.RgL != null) M_DR.MeasFreqParam.RgL = s_sdr_freq_param.RgL.GetValueOrDefault();
                                        if (s_sdr_freq_param.Step != null) M_DR.MeasFreqParam.Step = s_sdr_freq_param.Step.GetValueOrDefault();
                                        FrequencyMode out_FrequencyMode;
                                        if (Enum.TryParse<FrequencyMode>(s_sdr_freq_param.Mode, out out_FrequencyMode))
                                            M_DR.MeasFreqParam.Mode = out_FrequencyMode;

                                        List<MeasFreq> FRL = new List<MeasFreq>();
                                        var s_l_NH_MEAS_FREQ_LST = session.QueryOver<NH_MeasSDRFreq>().Fetch(x => x.ID).Eager;
                                        if (s_l_NH_MEAS_FREQ_LST.RowCount() > 0)
                                        {
                                            List<NH_MeasSDRFreq> F_SDR_MEAS_FREQ_LST = (List<NH_MeasSDRFreq>)s_l_NH_MEAS_FREQ_LST.Where(t => t.ID_MeasSDRFreqParam == s_sdr_freq_param.ID).List();
                                            foreach (NH_MeasSDRFreq s_sdr_freq_lst in F_SDR_MEAS_FREQ_LST)
                                            {
                                                MeasFreq FR_L = new MeasFreq();
                                                if (s_sdr_freq_lst.Freq != null) FR_L.Freq = s_sdr_freq_lst.Freq.GetValueOrDefault();
                                                FRL.Add(FR_L);
                                            }
                                        }
                                        M_DR.MeasFreqParam.MeasFreqs = FRL.ToArray();
                                        break;
                                    }
                                }
                                listMeasSdrTask.Add(M_DR);
                            }
                        }

                    }
                    #endregion
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in ProcessMultyMeas: " + ex.Message);*/ }
            return listMeasSdrTask;
        }


        /// <summary>
        ///Получить список всех тасков в БД 
        /// </summary>
        /// <param name="CountMeasTask">Количество тасков в БД</param>
        /// <param name="OnlyGetCount">Признак, который указаывает режим работы метода, если  OnlyGetCount = false - то объекты в памяти могут обновляться, OnlyGetCount = true - то объекты в памяти обновляться не могут </param>
        /// <returns></returns>
        public bool LoadMeasTaskSDR(out int CountMeasTask, bool OnlyGetCount = false)
        {
            CountMeasTask = 0;
            bool isSuccess = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    #region NH_Meas_Task_SDR
                    {
                        session.Clear();
                        var s_l_NH_Meas_Task_SDR = session.QueryOver<NH_MeasTaskSDR>().Fetch(x => x.ID).Eager;
                        if (s_l_NH_Meas_Task_SDR.RowCount() > 0)
                        {
                            // Проверка по статусам A,E_L,E_B,O
                            List<NH_MeasTaskSDR> F = (List<NH_MeasTaskSDR>)s_l_NH_Meas_Task_SDR.Where(t => t.status != "Z").List();//.OrderBy(u=>u.TypeM);
                            foreach (NH_MeasTaskSDR s in F)
                            {
                                ///Предварительная проверка перед постановкой задачи устройству



                                MeasSdrTask M_DR = new MeasSdrTask();

                                M_DR.MeasTaskId = new MeasTaskIdentifier();
                                if (s.MeasTaskId != null) M_DR.MeasTaskId.Value = s.MeasTaskId.GetValueOrDefault();
                                M_DR.MeasSubTaskId = new MeasTaskIdentifier();
                                if (s.MeasSubTaskId != null) M_DR.MeasSubTaskId.Value = s.MeasSubTaskId.GetValueOrDefault();
                                if (s.ID != null) M_DR.Id = s.ID.GetValueOrDefault();
                                if (s.MeasSubTaskStationId != null) M_DR.MeasSubTaskStationId = s.MeasSubTaskStationId.GetValueOrDefault();
                                MeasurementType out_MeasurementType;
                                if (Enum.TryParse<MeasurementType>(s.MeasDataType, out out_MeasurementType))
                                    M_DR.MeasDataType = out_MeasurementType;
                                M_DR.status = s.status;
                                M_DR.SensorId = new SensorIdentifier();
                                if (s.SensorId != null) M_DR.SensorId.Value = s.SensorId.GetValueOrDefault();
                                if (s.SwNumber != null) M_DR.SwNumber = s.SwNumber.GetValueOrDefault();
                                if (s.Time_start != null) M_DR.Time_start = s.Time_start.GetValueOrDefault();
                                if (s.Time_stop != null) M_DR.Time_stop = s.Time_stop.GetValueOrDefault();
                                M_DR.NumberScanPerTask = -999;
                                if (s.PerInterval != null) M_DR.PerInterval = s.PerInterval.GetValueOrDefault();

                                SpectrumScanType out_SpectrumScanType;
                                if (Enum.TryParse<SpectrumScanType>(s.TypeM, out out_SpectrumScanType))
                                    M_DR.TypeM = out_SpectrumScanType;


                                M_DR.MeasSDRSOParam = new MeasSdrSOParam();
                                var s_l_NH_SDR_So_Param = session.QueryOver<NH_MeasSDRSOParam>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_SDR_So_Param.RowCount() > 0)
                                {
                                    List<NH_MeasSDRSOParam> F_SDR_So_Param = (List<NH_MeasSDRSOParam>)s_l_NH_SDR_So_Param.Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                    foreach (NH_MeasSDRSOParam s_so_param in F_SDR_So_Param)
                                    {
                                        if (s_so_param.LevelMinOccup != null) M_DR.MeasSDRSOParam.LevelMinOccup = s_so_param.LevelMinOccup.GetValueOrDefault();
                                        if (s_so_param.NChenal != null) M_DR.MeasSDRSOParam.NChenal = s_so_param.NChenal.GetValueOrDefault();

                                        SpectrumOccupationType out_SpectrumOccupationType;
                                        if (Enum.TryParse<SpectrumOccupationType>(s_so_param.TypeSO, out out_SpectrumOccupationType))
                                            M_DR.MeasSDRSOParam.TypeSO = out_SpectrumOccupationType;
                                        break;
                                    }
                                }
                                //--------------------------------------------------
                                M_DR.MeasSDRParam = new MeasSdrParam();
                                var s_l_NH_MEAS_SDR_PARAM = session.QueryOver<NH_MeasSDRParam>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_MEAS_SDR_PARAM.RowCount() > 0)
                                {
                                    List<NH_MeasSDRParam> F_SDR_Sdr_Param = (List<NH_MeasSDRParam>)s_l_NH_MEAS_SDR_PARAM.Where(t => t.ID_MeasTaskSDR == s.ID).List();
                                    foreach (NH_MeasSDRParam s_sdr_param in F_SDR_Sdr_Param)
                                    {
                                        DetectingType out_DetectingType;
                                        if (Enum.TryParse<DetectingType>(s_sdr_param.DetectTypeSDR, out out_DetectingType))
                                            M_DR.MeasSDRParam.DetectTypeSDR = out_DetectingType;

                                        //M_DR.MeasSDRParam. ID = s_sdr_param.ID.GetValueOrDefault();
                                        if (s_sdr_param.PreamplificationSDR != null) M_DR.MeasSDRParam.PreamplificationSDR = s_sdr_param.PreamplificationSDR.GetValueOrDefault();
                                        if (s_sdr_param.VBW != null) M_DR.MeasSDRParam.VBW = s_sdr_param.VBW.GetValueOrDefault();
                                        if (s_sdr_param.RBW != null) M_DR.MeasSDRParam.RBW = s_sdr_param.RBW.GetValueOrDefault();
                                        if (s_sdr_param.ref_level_dbm != null) M_DR.MeasSDRParam.ref_level_dbm = s_sdr_param.ref_level_dbm.GetValueOrDefault();
                                        if (s_sdr_param.RfAttenuationSDR != null) M_DR.MeasSDRParam.RfAttenuationSDR = s_sdr_param.RfAttenuationSDR.GetValueOrDefault();
                                        if (s_sdr_param.MeasTime != null) M_DR.MeasSDRParam.MeasTime = s_sdr_param.MeasTime.GetValueOrDefault();
                                        break;
                                    }
                                }
                                //--------------------------------------------------
                                List<MeasLocParam> LMSZ = new List<MeasLocParam>();
                                M_DR.MeasLocParam = (new List<MeasLocParam>()).ToArray();
                                var s_l_NH_MEAS_SDR_LOC_PARAM = session.QueryOver<NH_MeasSDRLoc>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_MEAS_SDR_LOC_PARAM.RowCount() > 0)
                                {
                                    List<NH_MeasSDRLoc> F_SDR_Sdr_Loc_Param = (List<NH_MeasSDRLoc>)s_l_NH_MEAS_SDR_LOC_PARAM.Where(t => t.ID_MeasSDRID == s.ID).List();
                                    foreach (NH_MeasSDRLoc s_sdr_loc_param in F_SDR_Sdr_Loc_Param)
                                    {
                                        MeasLocParam cr_sdr_loc = new MeasLocParam();
                                        if (s_sdr_loc_param.ASL != null) cr_sdr_loc.ASL = s_sdr_loc_param.ASL.GetValueOrDefault();
                                        if (s_sdr_loc_param.Lat != null) cr_sdr_loc.Lat = s_sdr_loc_param.Lat.GetValueOrDefault();
                                        if (s_sdr_loc_param.Lon != null) cr_sdr_loc.Lon = s_sdr_loc_param.Lon.GetValueOrDefault();
                                        //cr_sdr_loc.MAX_DIST = s_sdr_loc_param.MAX_DIST.GetValueOrDefault();
                                        LMSZ.Add(cr_sdr_loc);
                                    }
                                }
                                M_DR.MeasLocParam = LMSZ.ToArray();
                                //--------------------------------------------------
                                M_DR.MeasFreqParam = new MeasFreqParam();
                                var s_l_NH_MEAS_SDR_FREQ_PARAM = session.QueryOver<NH_MeasSDRFreqParam>().Fetch(x => x.ID).Eager;
                                if (s_l_NH_MEAS_SDR_FREQ_PARAM.RowCount() > 0)
                                {
                                    List<NH_MeasSDRFreqParam> F_SDR_MEAS_SDR_FREQ_PARAM = (List<NH_MeasSDRFreqParam>)s_l_NH_MEAS_SDR_FREQ_PARAM.Where(t => t.id_meas_task_sdr == s.ID).List();
                                    foreach (NH_MeasSDRFreqParam s_sdr_freq_param in F_SDR_MEAS_SDR_FREQ_PARAM)
                                    {
                                        //M_DR.MeasFreqParam.BW_CH = s_sdr_freq_param.BW_CH.GetValueOrDefault();
                                        if (s_sdr_freq_param.RgU != null) M_DR.MeasFreqParam.RgU = s_sdr_freq_param.RgU.GetValueOrDefault();
                                        if (s_sdr_freq_param.RgL != null) M_DR.MeasFreqParam.RgL = s_sdr_freq_param.RgL.GetValueOrDefault();
                                        if (s_sdr_freq_param.Step != null) M_DR.MeasFreqParam.Step = s_sdr_freq_param.Step.GetValueOrDefault();
                                        FrequencyMode out_FrequencyMode;
                                        if (Enum.TryParse<FrequencyMode>(s_sdr_freq_param.Mode, out out_FrequencyMode))
                                            M_DR.MeasFreqParam.Mode = out_FrequencyMode;

                                        List<MeasFreq> FRL = new List<MeasFreq>();
                                        var s_l_NH_MEAS_FREQ_LST = session.QueryOver<NH_MeasSDRFreq>().Fetch(x => x.ID).Eager;
                                        if (s_l_NH_MEAS_FREQ_LST.RowCount() > 0)
                                        {
                                            List<NH_MeasSDRFreq> F_SDR_MEAS_FREQ_LST = (List<NH_MeasSDRFreq>)s_l_NH_MEAS_FREQ_LST.Where(t => t.ID_MeasSDRFreqParam == s_sdr_freq_param.ID).List();
                                            foreach (NH_MeasSDRFreq s_sdr_freq_lst in F_SDR_MEAS_FREQ_LST)
                                            {
                                                MeasFreq FR_L = new MeasFreq();
                                                if (s_sdr_freq_lst.Freq != null) FR_L.Freq = s_sdr_freq_lst.Freq.GetValueOrDefault();
                                                FRL.Add(FR_L);
                                            }
                                        }
                                        M_DR.MeasFreqParam.MeasFreqs = FRL.ToArray();
                                        break;
                                    }
                                }
                                CountMeasTask++;
                                isSuccess = true;
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in ProcessMultyMeas: " + ex.Message); */ isSuccess = false; }
            return isSuccess;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SaveMeasSDRResults
    {
        public int GetMaxIdFromResults()
        {
            int maxId = 0;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var max = session.QueryOver<NH_MeasSDRResults>().Select(Projections.Max<NH_MeasSDRResults>(x => x.ID)).SingleOrDefault<object>();
                    maxId = (max == null ? 0 : Convert.ToInt32(max));
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in GetMaxIdFromResults: " + ex.Message); */ }
            return maxId;
        }

        public DateTime? LoadDataMeasByTaskId(int TaskId)
        {
            DateTime? DataMeas = null;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var s_l_NH_Meas_Task_SDR = session.QueryOver<NH_MeasSDRResults>().Fetch(x => x.ID).Eager;
                    if (s_l_NH_Meas_Task_SDR.RowCount() > 0)
                    {
                        // Проверка по статусам A,E_L,E_B,O
                        List<NH_MeasSDRResults> F = (List<NH_MeasSDRResults>)s_l_NH_Meas_Task_SDR.Where(t => t.status != "Z" && t.MeasTaskId== TaskId).List();//.OrderBy(u=>u.TypeM);
                        foreach (NH_MeasSDRResults s in F)
                        {
                            DataMeas = s.DataMeas;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in LoadDataMeasByTaskId: " + ex.Message);*/ }
            return DataMeas;
        }


        public static bool SaveisSendMeasTaskSDRResults(MeasSdrResults sdrRes)
        {
            bool isSuccess = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var s_l_NH_MeasSDRResults = session.QueryOver<NH_MeasSDRResults>().Fetch(x => x.ID).Eager;
                    if (s_l_NH_MeasSDRResults.RowCount() > 0) {
                        List<NH_MeasSDRResults> F = (List<NH_MeasSDRResults>)s_l_NH_MeasSDRResults.Where(t => t.ID == sdrRes.Id).List();
                        if (F != null) {
                            ITransaction tr_1 = session.BeginTransaction();
                            foreach (NH_MeasSDRResults s in F) {
                                ClassObjectsSensorOnSDR cl = new ClassObjectsSensorOnSDR();
                                s.isSend = 1;
                                cl.UpdateObject<NH_MeasSDRResults>(s.ID.GetValueOrDefault(), s);
                            }
                            tr_1.Commit();
                            session.Flush();
                            tr_1.Dispose();
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in Save_Status_Meas_Task_SDRResults: " + ex.Message);*/ }
            return isSuccess;
        }

        public static bool SaveStatusMeasTaskSDRResults(MeasSdrResults sdrRes, string Status)
        {
            bool isSuccess = false;
            try {
            if (Domain.sessionFactory == null) Domain.Init();
            ISession session = Domain.CurrentSession;
            {
                session.Clear();
                var s_l_NH_MeasSDRResults = session.QueryOver<NH_MeasSDRResults>().Fetch(x => x.ID).Eager;
                if (s_l_NH_MeasSDRResults.RowCount() > 0) {
                    List<NH_MeasSDRResults> F = (List<NH_MeasSDRResults>)s_l_NH_MeasSDRResults.Where(t => t.ID == sdrRes.Id).List();
                    if (F != null) {
                        ITransaction tr_1 = session.BeginTransaction();
                        foreach (NH_MeasSDRResults s in F) {
                            ClassObjectsSensorOnSDR cl = new ClassObjectsSensorOnSDR();
                            s.status = Status;
                            cl.UpdateObject<NH_MeasSDRResults>(s.ID.GetValueOrDefault(), s);
                        }
                        tr_1.Commit();
                        session.Flush();
                        tr_1.Dispose();
                        isSuccess = true;
                    }
                }
            }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in SaveStatusMeasTaskSDRResults: " + ex.Message);*/ }
            return isSuccess;
        }

        public static bool SaveStatusMeasTaskSDR(MeasSdrTask sdrM)
        {
            bool isCorrect = false;
            try {
            if (Domain.sessionFactory == null) Domain.Init();
            ISession session = Domain.CurrentSession;
            {
                session.Clear();
                var s_l_NH_Meas_Task_SDR = session.QueryOver<NH_MeasTaskSDR>().Fetch(x => x.ID).Eager;
                if (s_l_NH_Meas_Task_SDR.RowCount() > 0) {
                    //sdrM.UpdateStatus();
                    List<NH_MeasTaskSDR> F = (List<NH_MeasTaskSDR>)s_l_NH_Meas_Task_SDR.Where(t => t.SensorId.Value == sdrM.SensorId.Value && t.MeasTaskId.Value == sdrM.MeasTaskId.Value && t.MeasSubTaskStationId == sdrM.MeasSubTaskStationId && t.MeasSubTaskId.Value == sdrM.MeasSubTaskId.Value).List();
                    if (F != null) {
                        ITransaction tr_1 = session.BeginTransaction();
                        foreach (NH_MeasTaskSDR s in F) {
                            ClassObjectsSensorOnSDR cl = new ClassObjectsSensorOnSDR();
                            s.status = sdrM.status;
                            cl.UpdateObject<NH_MeasTaskSDR>(s.ID.GetValueOrDefault(), s);
                            //break;
                        }
                        tr_1.Commit();
                        session.Flush();
                        tr_1.Dispose();
                        isCorrect = true;
                    }
                }
            }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in Save_Status_Meas_Task_SDR: " + ex.Message);*/ }
            return isCorrect;
        }



        public static void Save_Status_Meas_Task_SDR(NH_MeasTaskSDR nh_Task_SDR, string newStatus)
        {
            try
            {
                ClassObjectsSensorOnSDR cl = new ClassObjectsSensorOnSDR();
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    ITransaction tr_1 = session.BeginTransaction();
                    nh_Task_SDR.status = newStatus;
                    cl.UpdateObject<NH_MeasTaskSDR>(nh_Task_SDR.ID.GetValueOrDefault(), nh_Task_SDR);
                    tr_1.Commit();
                    session.Flush();
                    tr_1.Dispose();
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in Save_Status_Meas_Task_SDR: " + ex.Message);*/ }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="M_SDR_RES"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public bool SaveStatusResultSDR(MeasSdrResults M_SDR_RES, string Status)
        {
            bool isCorrect = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    var s_l_NH_Meas_Task_SDR = session.QueryOver<NH_MeasSDRResults>().Fetch(x => x.ID).Eager;
                    if (s_l_NH_Meas_Task_SDR.RowCount() > 0)
                    {
                        //sdrM.UpdateStatus();
                        //List<NH_MeasSDRResults> F = (List<NH_MeasSDRResults>)s_l_NH_Meas_Task_SDR.Where(t => t.SensorId.Value == M_SDR_RES.SensorId.Value && t.MeasTaskId.Value == M_SDR_RES.MeasTaskId.Value && t.MeasSubTaskStationId == M_SDR_RES.MeasSubTaskStationId && t.MeasSubTaskId.Value == M_SDR_RES.MeasSubTaskId.Value).List();
                        List<NH_MeasSDRResults> F = (List<NH_MeasSDRResults>)s_l_NH_Meas_Task_SDR.Where(t => t.ID == M_SDR_RES.Id).List();
                        if (F != null)
                        {
                            ITransaction tr_1 = session.BeginTransaction();
                            foreach (NH_MeasSDRResults s in F)
                            {
                                ClassObjectsSensorOnSDR cl = new ClassObjectsSensorOnSDR();
                                s.status = Status;
                                cl.UpdateObject<NH_MeasSDRResults>(s.ID.GetValueOrDefault(), s);
                                Console.WriteLine("Successfully saved into table - NH_MeasSDRResults");
                                //break;
                            }
                            tr_1.Commit();
                            session.Flush();
                            tr_1.Dispose();
                            isCorrect = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in SaveStatusResultSDR: " + ex.Message);*/ isCorrect = false; }
            return isCorrect;
        }

        /// <summary>
        /// Создать новую запись в таблице [NH_Meas_Task_SDR]
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool SaveMeasResultSDR(MeasSdrResults M_SDR_RES, string Status)
        {
            bool isCorrect = false;
            try
            {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    ITransaction tr = session.BeginTransaction();
                    #region NH_Meas_SDR_Results
                    NH_MeasSDRResults nh_Meas_SDR_Results = new NH_MeasSDRResults();
                    nh_Meas_SDR_Results.NN = M_SDR_RES.NN;
                    nh_Meas_SDR_Results.DataMeas = M_SDR_RES.DataMeas;
                    if (M_SDR_RES.MeasDataType != null) nh_Meas_SDR_Results.MeasDataType = M_SDR_RES.MeasDataType.ToString();
                    if (M_SDR_RES.MeasSubTaskId != null) nh_Meas_SDR_Results.MeasSubTaskId = M_SDR_RES.MeasSubTaskId.Value;
                    nh_Meas_SDR_Results.MeasSubTaskStationId = M_SDR_RES.MeasSubTaskStationId;
                    if (M_SDR_RES.SensorId != null) nh_Meas_SDR_Results.SensorId = M_SDR_RES.SensorId.Value;
                    if (M_SDR_RES.MeasTaskId != null) nh_Meas_SDR_Results.MeasTaskId = M_SDR_RES.MeasTaskId.Value;
                    //nh_Meas_SDR_Results.status = M_SDR_RES.status;
                    nh_Meas_SDR_Results.status = Status;
                    nh_Meas_SDR_Results.SwNumber = M_SDR_RES.SwNumber;
                    nh_Meas_SDR_Results = (NH_MeasSDRResults)SetNullValue(nh_Meas_SDR_Results);
                    object ID = session.Save(nh_Meas_SDR_Results);
                    M_SDR_RES.Id = Convert.ToInt32(ID);
                    Console.WriteLine("Successfully saved into table - NH_MeasSDRResults");
                    if (M_SDR_RES.FSemples != null) {
                        int idx_cnt = 0;
                        foreach (FSemples f_s in M_SDR_RES.FSemples) {
                            //if (M_SDR_RES.MeasDataType == MeasurementType.SpectrumOccupation) {
                                //if (idx_cnt != M_SDR_RES.FSemples.Count() - 1) continue;
                            //}
                                if (f_s.Freq != 0) {
                                NH_FSemples nh_F_SEMPLES = new NH_FSemples();
                                nh_F_SEMPLES.Freq = f_s.Freq;
                                nh_F_SEMPLES.LeveldBm = f_s.LeveldBm;
                                nh_F_SEMPLES.LeveldBmkVm = f_s.LeveldBmkVm;
                                nh_F_SEMPLES.LevelMaxdBm = f_s.LevelMaxdBm;
                                nh_F_SEMPLES.LevelMindBm = f_s.LevelMindBm;
                                nh_F_SEMPLES.OcupationPt = f_s.OcupationPt;
                                nh_F_SEMPLES.ID_MeasSDRResults = Convert.ToInt32(ID);
                                object ID_SEMPL = session.Save(nh_F_SEMPLES);
                                Console.WriteLine("Successfully saved into table - NH_FSemples");
                            }
                            idx_cnt++;
                        }
                    }
                    if (M_SDR_RES.Level != null)
                    {
                        foreach (float f_s in M_SDR_RES.Level)
                        {
                              NH_MeasResultsLevel  NH_NH_MeasResultsLevel  = new NH_MeasResultsLevel();
                              NH_NH_MeasResultsLevel.Level = f_s;
                              NH_NH_MeasResultsLevel.ID_NH_MeasSDRResults = Convert.ToInt32(ID);
                              object ID_SEMPL = session.Save(NH_NH_MeasResultsLevel);
                              Console.WriteLine("Successfully saved into table - _NH_MeasResultsLevel");
                        }
                    }
                    if (M_SDR_RES.Freqs != null)
                    {
                        int idx_cnt = 0;
                        foreach (float f_s in M_SDR_RES.Freqs) {
                            //if (M_SDR_RES.MeasDataType == MeasurementType.SpectrumOccupation) {
                                //if (idx_cnt != M_SDR_RES.Freqs.Count() - 1) continue;
                            //}
                            NH_MeasResultsFreq NH_NH_MeasResultsFreq = new NH_MeasResultsFreq();
                                NH_NH_MeasResultsFreq.Freq = f_s;
                                NH_NH_MeasResultsFreq.ID_NH_MeasSDRResults = Convert.ToInt32(ID);
                                object ID_SEMPL = session.Save(NH_NH_MeasResultsFreq);
                                Console.WriteLine("Successfully saved into table - NH_MeasResultsFreq");
                            idx_cnt++;
                        }
                    }
                    if (M_SDR_RES.MeasSDRLoc != null)  {
                        LocationSensorMeasurement f_s_ = M_SDR_RES.MeasSDRLoc; {
                            NH_LocationSensor nh_F_MeasSDRLoc = new NH_LocationSensor();
                            nh_F_MeasSDRLoc.ASL = f_s_.ASL;
                            nh_F_MeasSDRLoc.Lat = f_s_.Lat;
                            nh_F_MeasSDRLoc.Lon = f_s_.Lon;
                            nh_F_MeasSDRLoc.ID_MeasSDRResults = Convert.ToInt32(ID);
                            object ID_SEMPL = session.Save(nh_F_MeasSDRLoc);
                            Console.WriteLine("Successfully saved into table - NH_MeasSDRLoc");
                        }
                    }

                    
                    tr.Commit();
                    session.Flush();
                    tr.Dispose();
                    isCorrect = true;
                    #endregion
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in SaveMeasResultSDR: " + ex.Message);*/ isCorrect = false; }
            return isCorrect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static object SetNullValue(object v)
        {
            if (v != null)
            {
                Type myType = v.GetType();
                foreach (System.Reflection.PropertyInfo propertyInfo in myType.GetProperties())
                {
                    string name = propertyInfo.Name;
                    object value = propertyInfo.GetValue(v, null);
                    if (value is int)
                    {
                        if ((int)value == Constants.NullI)
                            propertyInfo.SetValue(v, new Nullable<int>());
                    }
                    else if (value is DateTime)
                    {
                        if (((DateTime)value) == Constants.NullT)
                            propertyInfo.SetValue(v, new Nullable<DateTime>());
                    }
                    else if (value is double)
                    {
                        if (((double)value) == Constants.NullD)
                            propertyInfo.SetValue(v, new Nullable<double>());
                    }
                }
            }
            return v;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public class SaveMeasTaskSDR
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static object SetNullValue(object v)
        {
            if (v != null)
            {
                Type myType = v.GetType();
                foreach (System.Reflection.PropertyInfo propertyInfo in myType.GetProperties())
                {
                    string name = propertyInfo.Name;
                    object value = propertyInfo.GetValue(v, null);
                    if (value is int)
                    {
                        if ((int)value == Constants.NullI)
                            propertyInfo.SetValue(v, new Nullable<int>());
                    }
                    else if (value is DateTime)
                    {
                        if (((DateTime)value) == Constants.NullT)
                            propertyInfo.SetValue(v, new Nullable<DateTime>());
                    }
                    else if (value is double)
                    {
                        if (((double)value) == Constants.NullD)
                            propertyInfo.SetValue(v, new Nullable<double>());
                    }
                }
            }
            return v;
        }

        /// <summary>
        /// Создать новую запись в таблице [NH_Meas_Task_SDR]
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool CreateNewMeasTaskSDR(MeasSdrTask M_SDR)
        {
            bool isCorrect = false;
            try
            {
                M_SDR.UpdateStatus();
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    ITransaction tr = session.BeginTransaction();
                    #region NH_Meas_Task_SDR
                    {
                        NH_MeasTaskSDR nh_Meas_Task_SDR = new NH_MeasTaskSDR();
                        nh_Meas_Task_SDR.MeasSubTaskStationId = M_SDR.MeasSubTaskStationId;
                        nh_Meas_Task_SDR.MeasSubTaskId = M_SDR.MeasSubTaskId.Value;
                        nh_Meas_Task_SDR.prio = M_SDR.prio.ToString();
                        nh_Meas_Task_SDR.status = M_SDR.status;
                        nh_Meas_Task_SDR.SwNumber = M_SDR.SwNumber;
                        nh_Meas_Task_SDR.PerInterval = (int)M_SDR.PerInterval;
                        nh_Meas_Task_SDR.Time_start = M_SDR.Time_start;
                        nh_Meas_Task_SDR.Time_stop = M_SDR.Time_stop;
                        nh_Meas_Task_SDR.TypeM = M_SDR.TypeM.ToString();
                        nh_Meas_Task_SDR.SensorId = M_SDR.SensorId.Value;
                        nh_Meas_Task_SDR.MeasTaskId = M_SDR.MeasTaskId.Value;
                        nh_Meas_Task_SDR.MeasDataType = M_SDR.MeasDataType.ToString();
                        nh_Meas_Task_SDR = (NH_MeasTaskSDR)SetNullValue(nh_Meas_Task_SDR);
                        object ID = session.Save(nh_Meas_Task_SDR);
                        M_SDR.Id = (int)ID;
                        Console.WriteLine("Successfully saved into table - NH_MeasTaskSDR");

                        NH_MeasSDRFreqParam n__Meas_SDR_FREQ_PARAM = new NH_MeasSDRFreqParam();
                        if (M_SDR.MeasFreqParam != null) {
                            n__Meas_SDR_FREQ_PARAM.RgU = M_SDR.MeasFreqParam.RgU;
                            n__Meas_SDR_FREQ_PARAM.RgL = M_SDR.MeasFreqParam.RgL;
                            n__Meas_SDR_FREQ_PARAM.Mode = M_SDR.MeasFreqParam.Mode.ToString();
                            n__Meas_SDR_FREQ_PARAM.Step = M_SDR.MeasFreqParam.Step;
                        }
                        n__Meas_SDR_FREQ_PARAM.id_meas_task_sdr = Convert.ToInt32(ID);
                        n__Meas_SDR_FREQ_PARAM = (NH_MeasSDRFreqParam)SetNullValue(n__Meas_SDR_FREQ_PARAM);
                        object _ID_MEAS_SDR_F_PARAM = session.Save(n__Meas_SDR_FREQ_PARAM);
                        Console.WriteLine("Successfully saved into table - NH_MeasSDRFreqParam");

                        if (M_SDR.MeasFreqParam != null) {
                            if (M_SDR.MeasFreqParam.MeasFreqs != null) {
                                foreach (MeasFreq FR in M_SDR.MeasFreqParam.MeasFreqs) {
                                    NH_MeasSDRFreq n__NH_Meas_SDR_Freq_LST = new NH_MeasSDRFreq();
                                    n__NH_Meas_SDR_Freq_LST.Freq = FR.Freq;
                                    n__NH_Meas_SDR_Freq_LST.ID_MeasSDRFreqParam = Convert.ToInt32(_ID_MEAS_SDR_F_PARAM);
                                    n__NH_Meas_SDR_Freq_LST = (NH_MeasSDRFreq)SetNullValue(n__NH_Meas_SDR_Freq_LST);
                                    session.Save(n__NH_Meas_SDR_Freq_LST);
                                    Console.WriteLine("Successfully saved into table - NH_MeasSDRFreq");
                                }
                            }
                        }

                        if (M_SDR.MeasLocParam != null){
                            foreach (MeasLocParam LOC in M_SDR.MeasLocParam) {
                                NH_MeasSDRLoc n__Meas_SDR_Loc_Param = new NH_MeasSDRLoc();
                                n__Meas_SDR_Loc_Param.ASL = LOC.ASL;
                                n__Meas_SDR_Loc_Param.Lat = LOC.Lat;
                                n__Meas_SDR_Loc_Param.Lon = LOC.Lon;
                                //n__Meas_SDR_Loc_Param.ID_MeasSDRResults = Convert.ToInt32(ID);
                                n__Meas_SDR_Loc_Param = (NH_MeasSDRLoc)SetNullValue(n__Meas_SDR_Loc_Param);
                                session.Save(n__Meas_SDR_Loc_Param);
                                Console.WriteLine("Successfully saved into table - NH_MeasSDRLoc");
                            }
                        }


                        NH_MeasSDRParam n__NH_Meas_SDR_Param = new NH_MeasSDRParam();
                        if (M_SDR.MeasSDRParam != null) {
                            n__NH_Meas_SDR_Param.DetectTypeSDR = M_SDR.MeasSDRParam.DetectTypeSDR.ToString();
                            n__NH_Meas_SDR_Param.PreamplificationSDR = M_SDR.MeasSDRParam.PreamplificationSDR;
                            n__NH_Meas_SDR_Param.RBW = M_SDR.MeasSDRParam.RBW;
                            n__NH_Meas_SDR_Param.ref_level_dbm = M_SDR.MeasSDRParam.ref_level_dbm;
                            n__NH_Meas_SDR_Param.RfAttenuationSDR = M_SDR.MeasSDRParam.RfAttenuationSDR;
                            n__NH_Meas_SDR_Param.MeasTime = M_SDR.MeasSDRParam.MeasTime;
                            n__NH_Meas_SDR_Param.VBW = M_SDR.MeasSDRParam.VBW;
                        }
                        n__NH_Meas_SDR_Param.ID_MeasTaskSDR = Convert.ToInt32(ID);
                        n__NH_Meas_SDR_Param = (NH_MeasSDRParam)SetNullValue(n__NH_Meas_SDR_Param);
                        session.Save(n__NH_Meas_SDR_Param);



                        NH_MeasSDRSOParam n__NH_Meas_SDR_So_Param = new NH_MeasSDRSOParam();
                        if (M_SDR.MeasSDRSOParam != null)
                        {
                            n__NH_Meas_SDR_So_Param.LevelMinOccup = M_SDR.MeasSDRSOParam.LevelMinOccup;
                            n__NH_Meas_SDR_So_Param.NChenal = M_SDR.MeasSDRSOParam.NChenal;
                            n__NH_Meas_SDR_So_Param.TypeSO = M_SDR.MeasSDRSOParam.TypeSO.ToString();
                        }
                        n__NH_Meas_SDR_So_Param.ID_MeasTaskSDR = Convert.ToInt32(ID);
                        n__NH_Meas_SDR_So_Param = (NH_MeasSDRSOParam)SetNullValue(n__NH_Meas_SDR_So_Param);
                        session.Save(n__NH_Meas_SDR_So_Param);
                        Console.WriteLine("Successfully saved into table - NH_MeasSDRSOParam");
                    }

                    #endregion
                    tr.Commit();
                    session.Flush();
                    tr.Dispose();
                    isCorrect = true;
                }

            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in CreateNewMeasTaskSDR: " + ex.Message);*/
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
                    ITransaction tr = session.BeginTransaction();
                    var s_l_NH_Meas_Task_SDR = session.QueryOver<NH_MeasTaskSDR>().Fetch(x => x.ID).Eager;
                    if (s_l_NH_Meas_Task_SDR.RowCount() > 0) {
                        List<NH_MeasTaskSDR> F = (List<NH_MeasTaskSDR>)s_l_NH_Meas_Task_SDR.Where(t => t.SensorId.Value == sdrM.SensorId.Value && t.MeasTaskId.Value == sdrM.MeasTaskId.Value && t.MeasSubTaskStationId == sdrM.MeasSubTaskStationId).List();
                        if (F != null) {
                            ITransaction tr_1 = session.BeginTransaction();
                            foreach (NH_MeasTaskSDR s in F) {
                                session.Delete(s);
                                Console.WriteLine("Remove record from table - NH_MeasTaskSDR");
                            }
                        }
                    }
                    tr.Commit();
                    session.Flush();
                    tr.Dispose();
                    isCorrect = true;
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in DeleteMeasTaskSDR: " + ex.Message); */  isCorrect = false;}
            
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
            try {
                if (Domain.sessionFactory == null) Domain.Init();
                ISession session = Domain.CurrentSession;
                {
                    session.Clear();
                    ITransaction tr = session.BeginTransaction();
                    var s_l_NH_Meas_Task_SDR = session.QueryOver<NH_MeasTaskSDR>().Fetch(x => x.ID).Eager;
                    if (s_l_NH_Meas_Task_SDR.RowCount() > 0) {
                        List<NH_MeasTaskSDR> F = (List<NH_MeasTaskSDR>)s_l_NH_Meas_Task_SDR.Where(t => t.SensorId.Value == sdrM.SensorId.Value && t.MeasTaskId.Value == sdrM.MeasTaskId.Value && t.MeasSubTaskStationId == sdrM.MeasSubTaskStationId).List();
                        if (F != null) {
                            ITransaction tr_1 = session.BeginTransaction();
                            foreach (NH_MeasTaskSDR s in F) {
                                s.status = Status;
                                session.Update(s);
                                Console.WriteLine("Archive record  table - NH_MeasTaskSDR");
                            }
                        }
                    }
                    tr.Commit();
                    session.Flush();
                    tr.Dispose();
                    isCorrect = true;
                }
            }
            catch (Exception ex)
            { /*GlobalInit.log.Trace("Error in ArchiveMeasTaskSDR: " + ex.Message); */ isCorrect = false; }
            return isCorrect;
        }
    }
}
