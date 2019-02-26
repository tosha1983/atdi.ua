﻿using System;
using System.Collections.Generic;
using System.Linq;


namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    public static class MeasTaskExtend
    {
        public static void CreateAllSubTasks(this MeasTask task)
        {
            if (task.Status == Status.N.ToString())
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
                    MST.Status = Status.A.ToString();
                    List<MeasSubTaskStation> ListMSTS = new List<MeasSubTaskStation>();
                    int j = 0;
                    foreach (MeasStation St in task.Stations)
                    {
                        MeasSubTaskStation MSTS = new MeasSubTaskStation();
                        MSTS.Id = j; j++;
                        MSTS.Status = Status.N.ToString();
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
                        MST.Status = Status.A.ToString();
                        List<MeasSubTaskStation> ListMSTS = new List<MeasSubTaskStation>();
                        int j = 0;
                        foreach (MeasStation St in task.Stations)
                        {
                            MeasSubTaskStation MSTS = new MeasSubTaskStation();
                            MSTS.Id = j; j++;
                            MSTS.Status = Status.N.ToString();
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
            task.Status = Status.A.ToString();
        }


        public static void UpdateStatusSubTasks(this MeasTask task, int Id_Sensor, string Type, bool isOnline)
        {
            if (task.MeasSubTasks == null) return;
            foreach (MeasSubTask SubTask in task.MeasSubTasks.ToArray())
            {
                if (SubTask.MeasSubTaskStations != null)
                {
                    foreach (MeasSubTaskStation SubTaskStation in SubTask.MeasSubTaskStations.ToArray())
                    {
                        if (SubTaskStation.StationId.Value == Id_Sensor)
                        {
                            if (Type == MeasTaskMode.Run.ToString())
                            {
                                if (isOnline) SubTaskStation.Status = Status.O.ToString();
                                else SubTaskStation.Status = Status.A.ToString();
                            }
                            else if (Type == MeasTaskMode.Stop.ToString())
                            {
                                if (isOnline) SubTaskStation.Status = Status.P.ToString(); 
                                else SubTaskStation.Status = Status.F.ToString(); 
                            }
                            else if (Type == MeasTaskMode.Del.ToString())
                            {
                                SubTaskStation.Status = Status.Z.ToString();
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
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Stop.ToString(), Status.N.ToString(), Status.F.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Stop.ToString(), Status.A.ToString(), Status.F.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Run.ToString(), Status.F.ToString(), Status.A.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Stop.ToString(), Status.E_L.ToString(), Status.F.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Run.ToString(), Status.P.ToString(), Status.O.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Stop.ToString(), Status.O.ToString(), Status.P.ToString()));


            List<StatusDescription> Descr_MeasSubTaskStation = new List<StatusDescription>();
            Descr_MeasSubTaskStation.Add(new StatusDescription("", 0, ModeStatus.cur));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.N.ToString(), 2, ModeStatus.cur));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.A.ToString(), 9, ModeStatus.cur));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.C.ToString(), 4, ModeStatus.final));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.T.ToString(), 3, ModeStatus.final));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.F.ToString(), 8, ModeStatus.cur));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.Z.ToString(), 1, ModeStatus.final));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.E_L.ToString(), 5, ModeStatus.cur));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.E_T.ToString(), 6, ModeStatus.final));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.E_E.ToString(), 7, ModeStatus.final));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.P.ToString(), 10, ModeStatus.cur));
            Descr_MeasSubTaskStation.Add(new StatusDescription(Status.O.ToString(), 11, ModeStatus.cur));

            List<StatusDescription> Descr_MeasSubTask = new List<StatusDescription>();
            Descr_MeasSubTask.Add(new StatusDescription("", 0, ModeStatus.cur));
            Descr_MeasSubTask.Add(new StatusDescription(Status.N.ToString(), 2, ModeStatus.cur));
            Descr_MeasSubTask.Add(new StatusDescription(Status.A.ToString(), 9, ModeStatus.cur));
            Descr_MeasSubTask.Add(new StatusDescription(Status.C.ToString(), 4, ModeStatus.final));
            Descr_MeasSubTask.Add(new StatusDescription(Status.T.ToString(), 3, ModeStatus.final));
            Descr_MeasSubTask.Add(new StatusDescription(Status.F.ToString(), 8, ModeStatus.cur));
            Descr_MeasSubTask.Add(new StatusDescription(Status.Z.ToString(), 1, ModeStatus.final));
            Descr_MeasSubTask.Add(new StatusDescription(Status.E_L.ToString(), 5, ModeStatus.cur));
            Descr_MeasSubTask.Add(new StatusDescription(Status.E_T.ToString(), 6, ModeStatus.final));
            Descr_MeasSubTask.Add(new StatusDescription(Status.E_E.ToString(), 7, ModeStatus.final));
            Descr_MeasSubTask.Add(new StatusDescription(Status.P.ToString(), 10, ModeStatus.cur));
            Descr_MeasSubTask.Add(new StatusDescription(Status.O.ToString(), 11, ModeStatus.cur));

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
                        if (Type == MeasTaskMode.Run.ToString())
                        {
                            if (SubTaskStation.Status == Status.P.ToString())
                            {
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskStation.Status && t.ToStatuses == Status.O.ToString())) != null)
                                {
                                    SubTaskStation.Status = Status.O.ToString();
                                    SubTask.Status = Status.O.ToString();
                                    task.Status = Status.O.ToString();
                                }
                            }
                            if (SubTaskStation.Status == Status.F.ToString())
                            {
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskStation.Status && t.ToStatuses == Status.A.ToString())) != null)
                                {
                                    SubTaskStation.Status = Status.A.ToString();
                                    SubTask.Status = Status.A.ToString();
                                    task.Status = Status.A.ToString();
                                }
                            }
                        }
                        else if (Type == MeasTaskMode.Del.ToString())
                        {
                            SubTaskStation.Status = Status.Z.ToString();
                            SubTask.Status = Status.Z.ToString();
                            task.Status = Status.Z.ToString();
                        }
                        else if (Type == MeasTaskMode.Stop.ToString())
                        {
                            StatusDescription DescrStat = Descr_MeasSubTaskStation.Find(t => t.Type != ModeStatus.final && t.NameStatus == SubTaskStation.Status);
                            if (DescrStat != null)
                            {
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskStation.Status && t.ToStatuses == Status.P.ToString())) != null)
                                {
                                    SubTaskStation.Status = Status.P.ToString();
                                    SubTask.Status = Status.P.ToString();
                                    task.Status = Status.P.ToString();
                                }
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskStation.Status && t.ToStatuses == Status.F.ToString())) != null)
                                {
                                    SubTaskStation.Status = Status.F.ToString();
                                    SubTask.Status = Status.F.ToString();
                                    task.Status = Status.F.ToString();
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

}