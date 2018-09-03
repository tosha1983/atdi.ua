using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts.Sdrns;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{
    public static class MeasTaskExtend
    {
        // Создание MeasSubTask и MeasSubTaskStation
        // Создание MeasSubTask и MeasSubTaskStation
        public static void CreateAllSubTasksApi1_0(this MeasTask task)
        {
            if (task.Status == "N")
            {
                List<MeasSubTask> ListMST = new List<MeasSubTask>();
                if (task.MeasDtParam == null) { task.MeasDtParam = new MeasDtParam(); };
                if (task.MeasDtParam.TypeMeasurements == MeasurementType.MonitoringStations)
                { // 21_02_2018 в данном случае мы делаем таски исключительно для системы мониторинга станций т.е. один таск на месяц.
                    MeasSubTask MST = new MeasSubTask();
                    if (task.MeasTimeParamList.PerInterval != null) { MST.Interval = (int?)task.MeasTimeParamList.PerInterval; } else { MST.Interval = 3600; };
                    MST.Id = new MeasTaskIdentifier();
                    MST.Id.Value = 1;
                    MST.TimeStart = task.MeasTimeParamList.PerStart;
                    MST.TimeStop = task.MeasTimeParamList.PerStop;
                    MST.Status = "A";
                    List<MeasSubTaskStation> ListMSTS = new List<MeasSubTaskStation>();
                    int j = 0;
                    foreach (MeasStation St in task.Stations)
                    {
                        MeasSubTaskStation MSTS = new MeasSubTaskStation();
                        MSTS.Id = j; j++;
                        MSTS.Status = "N";
                        MSTS.StationId = new SensorIdentifier();
                        MSTS.StationId.Value = St.StationId.Value;
                        ListMSTS.Add(MSTS);
                    }
                    MST.MeasSubTaskStations = ListMSTS.ToArray();
                    ListMST.Add(MST);
                }
                else
                {
                    int i = 0;
                    int hour_start = 0; int hour_stop = 24;
                    int min_start = 0; int min_stop = 0;
                    int sec_start = 0; int sec_stop = 0;
                    if (task.MeasTimeParamList.TimeStart != null) { hour_start = task.MeasTimeParamList.TimeStart.GetValueOrDefault().Hour; min_start = task.MeasTimeParamList.TimeStart.GetValueOrDefault().Minute; sec_start = task.MeasTimeParamList.TimeStart.GetValueOrDefault().Second; }
                    if (task.MeasTimeParamList.TimeStop != null) { hour_stop = task.MeasTimeParamList.TimeStop.GetValueOrDefault().Hour; min_stop = task.MeasTimeParamList.TimeStop.GetValueOrDefault().Minute; sec_stop = task.MeasTimeParamList.TimeStop.GetValueOrDefault().Second; }
                    for (var day = task.MeasTimeParamList.PerStart; day.Date <= task.MeasTimeParamList.PerStop; day = day.AddDays(1))
                    {
                        MeasSubTask MST = new MeasSubTask();
                        MST.Id = new MeasTaskIdentifier();
                        MST.Id.Value = i; i++;
                        MST.TimeStart = new DateTime(day.Year, day.Month, day.Day, hour_start, min_start, sec_start);
                        MST.TimeStop = new DateTime(day.Year, day.Month, day.Day, hour_stop, min_stop, sec_stop);
                        MST.Status = "A";
                        List<MeasSubTaskStation> ListMSTS = new List<MeasSubTaskStation>();
                        int j = 0;
                        foreach (MeasStation St in task.Stations)
                        {
                            MeasSubTaskStation MSTS = new MeasSubTaskStation();
                            MSTS.Id = j; j++;
                            MSTS.Status = "N";
                            MSTS.StationId = new SensorIdentifier();
                            MSTS.StationId.Value = St.StationId.Value;
                            ListMSTS.Add(MSTS);
                        }
                        MST.MeasSubTaskStations = ListMSTS.ToArray();
                        ListMST.Add(MST);

                    }
                }

                task.MeasSubTasks = ListMST.ToArray();
            }
            task.Status = "A";
        }


        public static void UpdateStatusSubTasks(this MeasTask task, int Id_Sensor, string Type, bool isOnline)
        {
            if (task.MeasSubTasks == null) return;
            foreach (MeasSubTask SubTask in task.MeasSubTasks.ToArray()) {
                foreach (MeasSubTaskStation SubTaskStation in SubTask.MeasSubTaskStations.ToArray()) {
                    if (SubTaskStation.StationId.Value == Id_Sensor) {
                        if (Type == "Run") {
                            if (isOnline) SubTaskStation.Status = "O";
                            else SubTaskStation.Status = "A";
                        }
                        else if (Type == "Stop")
                        {
                            if (isOnline) SubTaskStation.Status = "P";
                            else SubTaskStation.Status = "F";
                        }
                        else if (Type == "Del")
                        {
                            SubTaskStation.Status = "Z";
                        }
                    }
                }
            }
        }

        // Создание MeasTaskSDRs 26.12.2017 обновление функции Максим 27.12.2017
        public static List<MeasSdrTask> CreateeasTaskSDRsApi1_0(this MeasTask task, string Type = "New")
        {
            List<MeasSdrTask> ListMTSDR = new List<MeasSdrTask>();
            if (task.MeasSubTasks == null) return ListMTSDR;
            foreach (MeasSubTask SubTask in task.MeasSubTasks)
            {
                foreach (MeasSubTaskStation SubTaskStation in SubTask.MeasSubTaskStations)
                {
                    if ((Type == "New") || ((Type == "Stop") && ((SubTaskStation.Status == "F") || (SubTaskStation.Status == "P"))) || ((Type == "Run") && ((SubTaskStation.Status == "O") || (SubTaskStation.Status == "A"))) ||
                        ((Type == "Del") && (SubTaskStation.Status == "Z")))

                    {
                        MeasSdrTask MTSDR = new MeasSdrTask();
                        MTSDR.MeasSubTaskId = SubTask.Id;
                        MTSDR.MeasSubTaskStationId = SubTaskStation.Id;
                        MTSDR.MeasTaskId = task.Id;
                        MTSDR.status = SubTaskStation.Status;
                        MTSDR.SensorId = SubTaskStation.StationId;
                        if (Type == "New")
                        {
                            if (task.Id == null) task.Id = new MeasTaskIdentifier();
                            if (task.MeasOther == null) task.MeasOther = new MeasOther();
                            if (task.MeasDtParam == null) { task.MeasDtParam = new MeasDtParam(); }
                            MTSDR.MeasDataType = task.MeasDtParam.TypeMeasurements;
                            MTSDR.MeasFreqParam = task.MeasFreqParam;
                            if (task.MeasLocParams != null) { MTSDR.MeasLocParam = task.MeasLocParams; }
                            MTSDR.MeasSDRParam = new MeasSdrParam();
                            MTSDR.MeasSDRSOParam = new MeasSdrSOParam();
                            MTSDR.MeasSDRParam.DetectTypeSDR = task.MeasDtParam.DetectType;
                            if (task.MeasDtParam.MeasTime != null) { MTSDR.MeasSDRParam.MeasTime = task.MeasDtParam.MeasTime.GetValueOrDefault(); } else { MTSDR.MeasSDRParam.MeasTime = 0.001; }
                            MTSDR.MeasSDRParam.PreamplificationSDR = task.MeasDtParam.Preamplification;
                            if (task.MeasDtParam.RBW != null) { MTSDR.MeasSDRParam.RBW = task.MeasDtParam.RBW.GetValueOrDefault(); } else { MTSDR.MeasSDRParam.RBW = 10; }
                            if (task.MeasOther.LevelMinOccup != null) { MTSDR.MeasSDRSOParam.LevelMinOccup = task.MeasOther.LevelMinOccup.GetValueOrDefault(); } else { MTSDR.MeasSDRSOParam.LevelMinOccup = -70; }
                            MTSDR.MeasSDRParam.RfAttenuationSDR = (int)task.MeasDtParam.RfAttenuation;
                            if (task.MeasDtParam.VBW != null) { MTSDR.MeasSDRParam.VBW = task.MeasDtParam.VBW.GetValueOrDefault(); } else { MTSDR.MeasSDRParam.VBW = 10; }
                            // MTSDR.MeasSDRParam.ref_level_dbm = 70; // Для SDR пока не присваевается
                            MTSDR.MeasSDRParam.ref_level_dbm = -30;
                            if (task.MeasOther.NChenal != null) { MTSDR.MeasSDRSOParam.NChenal = task.MeasOther.NChenal.GetValueOrDefault(); } else { MTSDR.MeasSDRSOParam.NChenal = 10; }
                            MTSDR.MeasSDRSOParam.TypeSO = task.MeasOther.TypeSpectrumOccupation;
                            if (task.Prio != null) { MTSDR.prio = task.Prio.GetValueOrDefault(); } else { MTSDR.prio = 10; }
                            MTSDR.status = "N";
                            if (task.MeasOther.SwNumber != null) { MTSDR.SwNumber = task.MeasOther.SwNumber.GetValueOrDefault(); } //else { MTSDR.SwNumber = 10; }
                            MTSDR.Time_start = SubTask.TimeStart;
                            MTSDR.Time_stop = SubTask.TimeStop;
                            if (SubTask.Interval != null) { MTSDR.PerInterval = SubTask.Interval.GetValueOrDefault(); } //else { MTSDR.SwNumber = 10; }
                            MTSDR.TypeM = task.MeasOther.TypeSpectrumScan;
                            if (task.MeasDtParam.TypeMeasurements == MeasurementType.MonitoringStations)
                            { // 21_02_2018 в данном случае мы передаем станции  исключительно для системы мониторинга станций т.е. один таск на месяц Надо проверить.
                                if (task.StationsForMeasurements != null)
                                {
                                    MTSDR.StationsForMeasurements = task.StationsForMeasurements;
                                    // далее сформируем переменную GlobalSID 
                                    for (int i = 0; i < MTSDR.StationsForMeasurements.Count(); i++)
                                    {
                                        string CodeOwener = "0";
                                        if (MTSDR.StationsForMeasurements[i].Owner.OKPO == "14333937") { CodeOwener = "1"; };
                                        if (MTSDR.StationsForMeasurements[i].Owner.OKPO == "22859846") { CodeOwener = "6"; };
                                        if (MTSDR.StationsForMeasurements[i].Owner.OKPO == "21673832") { CodeOwener = "3"; };
                                        if (MTSDR.StationsForMeasurements[i].Owner.OKPO == "37815221") { CodeOwener = "7"; };
                                        MTSDR.StationsForMeasurements[i].GlobalSID = "255 " + CodeOwener + " 00000 " + string.Format("{0:00000}", MTSDR.StationsForMeasurements[i].IdStation);
                                    }
                                }
                            }
                        }
                        ListMTSDR.Add(MTSDR);
                    }
                }
            }
            return ListMTSDR;
        }


        public static List<Atdi.DataModels.Sdrns.Device.MeasTask> CreateeasTaskSDRsApi2_0(this MeasTask task, string SensorName, string SdrnServer, string EquipmentTechId, string Type = "New")
        {
            List<Atdi.DataModels.Sdrns.Device.MeasTask> ListMTSDR = new List<Atdi.DataModels.Sdrns.Device.MeasTask>();
            if (task.MeasSubTasks == null) return ListMTSDR;
            foreach (MeasSubTask SubTask in task.MeasSubTasks)
            {
                foreach (MeasSubTaskStation SubTaskStation in SubTask.MeasSubTaskStations)
                {
                    if ((Type == "New") || ((Type == "Stop") && ((SubTaskStation.Status == "F") || (SubTaskStation.Status == "P"))) || ((Type == "Run") && ((SubTaskStation.Status == "O") || (SubTaskStation.Status == "A"))) ||
                        ((Type == "Del") && (SubTaskStation.Status == "Z")))

                    {
                        Atdi.DataModels.Sdrns.Device.MeasTask MTSDR = new Atdi.DataModels.Sdrns.Device.MeasTask();
                        int? IdentValueTaskSDR = ClassesDBGetTasks.SaveTaskSDRToDB(SubTask.Id.Value, SubTaskStation.Id, task.Id.Value, SubTaskStation.StationId.Value);
                        MTSDR.TaskId = IdentValueTaskSDR.GetValueOrDefault().ToString();
                        if (task.Id == null) task.Id = new MeasTaskIdentifier();
                        if (task.MeasOther == null) task.MeasOther = new MeasOther();
                        if (task.MeasDtParam == null) { task.MeasDtParam = new MeasDtParam(); }
                        if (task.Prio != null) { MTSDR.Priority = task.Prio.GetValueOrDefault(); } else { MTSDR.Priority = 10; }
                        MTSDR.SensorName = SensorName;
                        MTSDR.SdrnServer = SdrnServer;
                        MTSDR.EquipmentTechId = EquipmentTechId;
                        if (Type == "New")
                        {
                            MTSDR.ScanParameters = new DataModels.Sdrns.Device.StandardScanParameter[] { };
                            MTSDR.StartTime= SubTask.TimeStart;
                            MTSDR.StopTime = SubTask.TimeStop;
                            MTSDR.Status = SubTask.Status;
                            MTSDR.MobEqipmentMeasurements = new DataModels.Sdrns.MeasurementType[5];
                            MTSDR.MobEqipmentMeasurements[0] = DataModels.Sdrns.MeasurementType.MonitoringStations;
                            MTSDR.MobEqipmentMeasurements[1] = DataModels.Sdrns.MeasurementType.BandwidthMeas;
                            MTSDR.MobEqipmentMeasurements[2] = DataModels.Sdrns.MeasurementType.Offset;
                            MTSDR.MobEqipmentMeasurements[3] = DataModels.Sdrns.MeasurementType.Frequency;
                            MTSDR.MobEqipmentMeasurements[4] = DataModels.Sdrns.MeasurementType.Level;
                            if (task.MeasOther.SwNumber != null) { MTSDR.ScanPerTaskNumber = task.MeasOther.SwNumber.GetValueOrDefault(); }
                            if (task.StationsForMeasurements != null)
                            {
                                MTSDR.Stations = new DataModels.Sdrns.Device.MeasuredStation[task.StationsForMeasurements.Count()];
                                if (task.MeasDtParam.TypeMeasurements == MeasurementType.MonitoringStations)
                                { // 21_02_2018 в данном случае мы передаем станции  исключительно для системы мониторинга станций т.е. один таск на месяц Надо проверить.
                                    if (task.StationsForMeasurements != null)
                                    {
                                        ///MTSDR.StationsForMeasurements = task.StationsForMeasurements;
                                        // далее сформируем переменную GlobalSID 
                                        for (int i = 0; i < task.StationsForMeasurements.Count(); i++)
                                        {
                                            MTSDR.Stations[i] = new DataModels.Sdrns.Device.MeasuredStation();
                                            string CodeOwener = "0";
                                            MTSDR.Stations[i].Owner = new DataModels.Sdrns.Device.StationOwner();
                                            if (task.StationsForMeasurements[i].Owner != null)
                                            {
                                                MTSDR.Stations[i].Owner.Address = task.StationsForMeasurements[i].Owner.Addres;
                                                MTSDR.Stations[i].Owner.Code = task.StationsForMeasurements[i].Owner.Code;
                                                MTSDR.Stations[i].Owner.Id = task.StationsForMeasurements[i].Owner.Id;
                                                MTSDR.Stations[i].Owner.OKPO = task.StationsForMeasurements[i].Owner.OKPO;
                                                MTSDR.Stations[i].Owner.OwnerName = task.StationsForMeasurements[i].Owner.OwnerName;
                                                MTSDR.Stations[i].Owner.Zip = task.StationsForMeasurements[i].Owner.Zip;


                                                if (MTSDR.Stations[i].Owner.OKPO == "14333937") { CodeOwener = "1"; };
                                                if (MTSDR.Stations[i].Owner.OKPO == "22859846") { CodeOwener = "6"; };
                                                if (MTSDR.Stations[i].Owner.OKPO == "21673832") { CodeOwener = "3"; };
                                                if (MTSDR.Stations[i].Owner.OKPO == "37815221") { CodeOwener = "7"; };
                                            }
                                            MTSDR.Stations[i].GlobalSid = "255 " + CodeOwener + " 00000 " + string.Format("{0:00000}", task.StationsForMeasurements[i].IdStation);


                                            MTSDR.Stations[i].OwnerGlobalSid = task.StationsForMeasurements[i].GlobalSID;//работать с таблицей (доп. создасть в БД по GlobalSID и Standard)
                                                                                                                         //
                                            MTSDR.Stations[i].License = new DataModels.Sdrns.Device.StationLicenseInfo();
                                            if (task.StationsForMeasurements[i].LicenseParameter != null)
                                            {
                                                MTSDR.Stations[i].License.CloseDate = task.StationsForMeasurements[i].LicenseParameter.CloseDate;
                                                MTSDR.Stations[i].License.EndDate = task.StationsForMeasurements[i].LicenseParameter.EndDate;
                                                MTSDR.Stations[i].License.IcsmId = task.StationsForMeasurements[i].LicenseParameter.Id;
                                                MTSDR.Stations[i].License.Name = task.StationsForMeasurements[i].LicenseParameter.DozvilName;
                                                MTSDR.Stations[i].License.StartDate = task.StationsForMeasurements[i].LicenseParameter.StartDate;
                                            }

                                            MTSDR.Stations[i].Site = new DataModels.Sdrns.Device.StationSite();
                                            if (task.StationsForMeasurements[i].Site != null)
                                            {
                                                MTSDR.Stations[i].Site.Adress = task.StationsForMeasurements[i].Site.Adress;
                                                MTSDR.Stations[i].Site.Lat = task.StationsForMeasurements[i].Site.Lat;
                                                MTSDR.Stations[i].Site.Lon = task.StationsForMeasurements[i].Site.Lon;
                                                MTSDR.Stations[i].Site.Region = task.StationsForMeasurements[i].Site.Region;
                                            }
                                            MTSDR.Stations[i].Standard = task.StationsForMeasurements[i].Standart;
                                            MTSDR.Stations[i].StationId = task.StationsForMeasurements[i].IdStation.ToString();
                                            MTSDR.Stations[i].Status = task.StationsForMeasurements[i].Status;


                                            if (task.StationsForMeasurements[i].Sectors != null)
                                            {
                                                MTSDR.Stations[i].Sectors = new DataModels.Sdrns.Device.StationSector[task.StationsForMeasurements[i].Sectors.Length];
                                                for (int j = 0; j < task.StationsForMeasurements[i].Sectors.Length; j++)
                                                {
                                                    MTSDR.Stations[i].Sectors[j] = new DataModels.Sdrns.Device.StationSector();
                                                    MTSDR.Stations[i].Sectors[j].AGL = task.StationsForMeasurements[i].Sectors[j].AGL;
                                                    MTSDR.Stations[i].Sectors[j].Azimuth = task.StationsForMeasurements[i].Sectors[j].Azimut;

                                                    if (task.StationsForMeasurements[i].Sectors[j].MaskBW != null)
                                                    {
                                                        MTSDR.Stations[i].Sectors[j].BWMask = new DataModels.Sdrns.Device.ElementsMask[task.StationsForMeasurements[i].Sectors[j].MaskBW.Length];
                                                        for (int k = 0; k < task.StationsForMeasurements[i].Sectors[j].MaskBW.Length; k++)
                                                        {
                                                            MTSDR.Stations[i].Sectors[j].BWMask[k] = new DataModels.Sdrns.Device.ElementsMask();
                                                            MTSDR.Stations[i].Sectors[j].BWMask[k].BW_kHz = task.StationsForMeasurements[i].Sectors[j].MaskBW[k].BW;
                                                            MTSDR.Stations[i].Sectors[j].BWMask[k].Level_dB = task.StationsForMeasurements[i].Sectors[j].MaskBW[k].level;
                                                        }
                                                    }
                                                    MTSDR.Stations[i].Sectors[j].BW_kHz = task.StationsForMeasurements[i].Sectors[j].BW;
                                                    MTSDR.Stations[i].Sectors[j].ClassEmission = task.StationsForMeasurements[i].Sectors[j].ClassEmission;
                                                    MTSDR.Stations[i].Sectors[j].EIRP_dBm = task.StationsForMeasurements[i].Sectors[j].EIRP;

                                                    if (task.StationsForMeasurements[i].Sectors[j].Frequencies != null)
                                                    {
                                                        MTSDR.Stations[i].Sectors[j].Frequencies = new DataModels.Sdrns.Device.SectorFrequency[task.StationsForMeasurements[i].Sectors[j].Frequencies.Length];
                                                        for (int k = 0; k < task.StationsForMeasurements[i].Sectors[j].Frequencies.Length; k++)
                                                        {
                                                            MTSDR.Stations[i].Sectors[j].Frequencies[k] = new DataModels.Sdrns.Device.SectorFrequency();
                                                            MTSDR.Stations[i].Sectors[j].Frequencies[k].ChannelNumber = task.StationsForMeasurements[i].Sectors[j].Frequencies[k].ChannalNumber;
                                                            MTSDR.Stations[i].Sectors[j].Frequencies[k].Frequency_MHz = task.StationsForMeasurements[i].Sectors[j].Frequencies[k].Frequency;
                                                            MTSDR.Stations[i].Sectors[j].Frequencies[k].Id = task.StationsForMeasurements[i].Sectors[j].Frequencies[k].Id;
                                                            MTSDR.Stations[i].Sectors[j].Frequencies[k].PlanId = task.StationsForMeasurements[i].Sectors[j].Frequencies[k].IdPlan;
                                                        }
                                                    }
                                                    MTSDR.Stations[i].Sectors[j].SectorId = task.StationsForMeasurements[i].Sectors[j].IdSector.ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        ListMTSDR.Add(MTSDR);
                    }
                }
            }
            return ListMTSDR;
        }

        /// <summary>
        /// Функция обновления статуса MeasTask,MeasSubTask,MeasSubTaskStation
        /// </summary>
        /// <param name="task"></param>
        /// <param name="Type"></param>
        public static void UpdateStatus(this MeasTask task, string Type = "New")
        {
            //правила переходов из статуса в статусы при заданной операции

                List<RuleStatusTransition> OperationTransitionRule = new List<RuleStatusTransition>();
                OperationTransitionRule.Add(new RuleStatusTransition("Stop", "N", "F"));
                OperationTransitionRule.Add(new RuleStatusTransition("Stop", "A", "F"));
                OperationTransitionRule.Add(new RuleStatusTransition("Run", "F", "A"));
                OperationTransitionRule.Add(new RuleStatusTransition("Stop", "E_L", "F"));
                OperationTransitionRule.Add(new RuleStatusTransition("Run", "P", "O"));
                OperationTransitionRule.Add(new RuleStatusTransition("Stop", "O", "P"));


                List<StatusDescription> Descr_MeasSubTaskStation = new List<StatusDescription>();
                Descr_MeasSubTaskStation.Add(new StatusDescription("", 0, ModeStatus.cur));
                Descr_MeasSubTaskStation.Add(new StatusDescription("N", 2, ModeStatus.cur));
                Descr_MeasSubTaskStation.Add(new StatusDescription("A", 9, ModeStatus.cur));
                Descr_MeasSubTaskStation.Add(new StatusDescription("C", 4, ModeStatus.final));
                Descr_MeasSubTaskStation.Add(new StatusDescription("T", 3, ModeStatus.final));
                Descr_MeasSubTaskStation.Add(new StatusDescription("F", 8, ModeStatus.cur));
                Descr_MeasSubTaskStation.Add(new StatusDescription("Z", 1, ModeStatus.final));
                Descr_MeasSubTaskStation.Add(new StatusDescription("E_L", 5, ModeStatus.cur));
                Descr_MeasSubTaskStation.Add(new StatusDescription("E_T", 6, ModeStatus.final));
                Descr_MeasSubTaskStation.Add(new StatusDescription("E_E", 7, ModeStatus.final));
                Descr_MeasSubTaskStation.Add(new StatusDescription("P", 10, ModeStatus.cur));
                Descr_MeasSubTaskStation.Add(new StatusDescription("O", 11, ModeStatus.cur));

                List<StatusDescription> Descr_MeasSubTask = new List<StatusDescription>();
                Descr_MeasSubTask.Add(new StatusDescription("", 0, ModeStatus.cur));
                Descr_MeasSubTask.Add(new StatusDescription("N", 2, ModeStatus.cur));
                Descr_MeasSubTask.Add(new StatusDescription("A", 9, ModeStatus.cur));
                Descr_MeasSubTask.Add(new StatusDescription("C", 4, ModeStatus.final));
                Descr_MeasSubTask.Add(new StatusDescription("T", 3, ModeStatus.final));
                Descr_MeasSubTask.Add(new StatusDescription("F", 8, ModeStatus.cur));
                Descr_MeasSubTask.Add(new StatusDescription("Z", 1, ModeStatus.final));
                Descr_MeasSubTask.Add(new StatusDescription("E_L", 5, ModeStatus.cur));
                Descr_MeasSubTask.Add(new StatusDescription("E_T", 6, ModeStatus.final));
                Descr_MeasSubTask.Add(new StatusDescription("E_E", 7, ModeStatus.final));
                Descr_MeasSubTask.Add(new StatusDescription("P", 10, ModeStatus.cur));
                Descr_MeasSubTask.Add(new StatusDescription("O", 11, ModeStatus.cur));

                List<int> MaxWeightLst = new List<int>();
                if (task.MeasSubTasks == null) return;

                foreach (MeasSubTask SubTask in task.MeasSubTasks.ToArray())
                {
                    string StatusWithMaxWeight = "";
                    MaxWeightLst = new List<int>();
                    foreach (MeasSubTaskStation SubTaskStation in SubTask.MeasSubTaskStations.ToArray())
                    {
                        if (Type == "Run")
                        {
                            if (SubTaskStation.Status == "P")
                            {
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskStation.Status && t.ToStatuses == "O")) != null)
                                {
                                    SubTaskStation.Status = "O";
                                    SubTask.Status = "O";
                                    task.Status = "O";
                                }
                            }
                            if (SubTaskStation.Status == "F")
                            {
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskStation.Status && t.ToStatuses == "A")) != null)
                                {
                                    SubTaskStation.Status = "A";
                                    SubTask.Status = "A";
                                    task.Status = "A";
                                }
                            }
                        }
                        else if (Type == "Del")
                        {
                            SubTaskStation.Status = "Z";
                            SubTask.Status = "Z";
                            task.Status = "Z";
                        }
                        else if (Type == "Stop")
                        {
                            StatusDescription DescrStat = Descr_MeasSubTaskStation.Find(t => t.Type != ModeStatus.final && t.NameStatus == SubTaskStation.Status);
                            if (DescrStat != null)
                            {
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskStation.Status && t.ToStatuses == "P")) != null)
                                {
                                    SubTaskStation.Status = "P";
                                    SubTask.Status = "P";
                                    task.Status = "P";
                                }
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskStation.Status && t.ToStatuses == "F")) != null)
                                {
                                    SubTaskStation.Status = "F";
                                    SubTask.Status = "F";
                                    task.Status = "F";
                                }
                            }
                        }
                        StatusDescription val_fnd_status = Descr_MeasSubTaskStation.Find(t => t.NameStatus == SubTaskStation.Status);
                        if (val_fnd_status != null)
                        {
                            MaxWeightLst.Add(val_fnd_status.Weight);
                        }
                    }
                    if (MaxWeightLst.Count > 0)
                    {
                        int Max_ = MaxWeightLst.Max();
                        StatusDescription val_fnd_status_ = Descr_MeasSubTaskStation.Find(t => t.Weight == Max_);
                        if (val_fnd_status_ != null)
                        {
                            StatusWithMaxWeight = val_fnd_status_.NameStatus;
                            SubTask.Status = StatusWithMaxWeight;
                        }
                    }
                }
                MaxWeightLst = new List<int>();
                foreach (MeasSubTask SubTask in task.MeasSubTasks.ToArray())
                {
                    StatusDescription val_fnd_status = Descr_MeasSubTask.Find(t => t.NameStatus == SubTask.Status);
                    if (val_fnd_status != null)
                    {
                        MaxWeightLst.Add(val_fnd_status.Weight);
                    }
                }
                if (MaxWeightLst.Count > 0)
                {
                    int Max_ = MaxWeightLst.Max();
                    StatusDescription val_fnd_status_ = Descr_MeasSubTask.Find(t => t.Weight == Max_);
                    if (val_fnd_status_ != null)
                    {
                        task.Status = val_fnd_status_.NameStatus;
                    }
                }
                OperationTransitionRule.Clear();
                Descr_MeasSubTaskStation.Clear();
                Descr_MeasSubTask.Clear();
                MaxWeightLst.Clear();
            
        }
}


    public class RuleStatusTransition
    {
        public string NameOperation { get; set; }
        public string StartStatus { get; set; }
        public string ToStatuses { get; set; }
        public RuleStatusTransition(string NameOperation_, string StartStatus_, string ToStatuses_)
        {
            NameOperation = NameOperation_;
            StartStatus = StartStatus_;
            ToStatuses = ToStatuses_;
        }
    }

    public class StatusDescription
    {
        public string NameStatus { get; set; }
        public int Weight { get; set; }
        public ModeStatus Type { get; set; }
        public StatusDescription(string NameStatus_, int Weight_, ModeStatus type_)
        {
            NameStatus = NameStatus_;
            Weight = Weight_;
            Type = type_;
        }
    }

    public enum ModeStatus
    {
        final,
        cur
    }

}
