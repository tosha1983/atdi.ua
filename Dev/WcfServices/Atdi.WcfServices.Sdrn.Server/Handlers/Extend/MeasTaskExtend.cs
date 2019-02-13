using System;
using System.Collections.Generic;
using System.Linq;


namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    public static class MeasTaskExtend
    {
        public static void CreateAllSubTasks(this MeasTask task)
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
                        if (task.MeasTimeParamList.PerInterval != null) { MST.Interval = (int?)task.MeasTimeParamList.PerInterval; } else { MST.Interval = 3600; };
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
                if (SubTask.MeasSubTaskStations!=null)
                {
                    foreach (MeasSubTaskStation SubTaskStation in SubTask.MeasSubTaskStations.ToArray())
                    {
                        if (SubTaskStation.StationId.Value == Id_Sensor)
                        {
                            if (Type == "Run")
                            {
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
                if (SubTask.MeasSubTaskStations != null)
                {
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
