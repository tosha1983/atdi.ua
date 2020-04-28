using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.DataModels.Sdrns.Server;


namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.PipelineHandlers
{
    public static class MeasTaskExtend
    {
        public static void SetDefaultSignalMask(ref ReferenceSignal referenceSignal)
        {
            //const 
            double PersentBW30dBForDefaultMask = 1;
            double PersentBW45dBForDefaultMask = 10;
            double PersentBW60dBForDefaultMask = 100;
            // end const
            if (referenceSignal.SignalMask == null)
            {
                referenceSignal.SignalMask = new SignalMask();
                referenceSignal.SignalMask.Freq_kHz = new double[8]{
                    -(PersentBW60dBForDefaultMask/100+1)*(referenceSignal.Bandwidth_kHz/2.0),
                    -(PersentBW45dBForDefaultMask/100+1)*(referenceSignal.Bandwidth_kHz/2.0),
                    -(PersentBW30dBForDefaultMask/100+1)*(referenceSignal.Bandwidth_kHz/2.0),
                    -(referenceSignal.Bandwidth_kHz/2.0),
                    (referenceSignal.Bandwidth_kHz/2.0),
                    (PersentBW30dBForDefaultMask/100+1)*(referenceSignal.Bandwidth_kHz/2.0),
                    (PersentBW45dBForDefaultMask/100+1)*(referenceSignal.Bandwidth_kHz/2.0),
                    (PersentBW60dBForDefaultMask/100+1)*(referenceSignal.Bandwidth_kHz/2.0)
                };
                referenceSignal.SignalMask.Loss_dB = new float[8] {60, 45,30,0,0,30,45,60};

            }
        }

        public static void CreateAllSubTasks(this MeasTask task)
        {
            if ((task.Status == Status.N.ToString()) || (task.Status == Status.S.ToString()))
            {
                var ListMST = new List<MeasSubTask>();
                if (task.TypeMeasurements == MeasurementType.MonitoringStations)
                { // 21_02_2018 в данном случае мы делаем таски исключительно для системы мониторинга станций т.е. один таск на месяц.
                    var MST = new MeasSubTask();
                    if (task.MeasTimeParamList.PerInterval != null) { MST.Interval = (int?)task.MeasTimeParamList.PerInterval; } else { MST.Interval = 3600; };
                    MST.Id = new MeasTaskIdentifier();
                    MST.Id.Value = 1;
                    MST.TimeStart = task.MeasTimeParamList.PerStart;
                    MST.TimeStop = task.MeasTimeParamList.PerStop;
                    MST.Status = task.Status;
                    var ListMSTS = new List<MeasSubTaskSensor>();
                    int j = 0;
                    for (int f=0; f< task.Sensors.Length; f++)
                    {
                        var St = task.Sensors[f];
                        var MSTS = new MeasSubTaskSensor();
                        MSTS.Id = j; j++;
                        MSTS.Status = task.Status;
                        MSTS.SensorId = St.SensorId.Value;
                        ListMSTS.Add(MSTS);
                    }
                    MST.MeasSubTaskSensors = ListMSTS.ToArray();
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
                        var MST = new MeasSubTask();
                        if (task.MeasTimeParamList.PerInterval != null) { MST.Interval = (int?)task.MeasTimeParamList.PerInterval; } else { MST.Interval = 600; };
                        MST.Id = new MeasTaskIdentifier();
                        MST.Id.Value = i; i++;
                        MST.TimeStart = new DateTime(day.Year, day.Month, day.Day, hour_start, min_start, sec_start);
                        MST.TimeStop = new DateTime(day.Year, day.Month, day.Day, hour_stop, min_stop, sec_stop);
                        MST.Status = task.Status;
                        var ListMSTS = new List<MeasSubTaskSensor>();
                        int j = 0;
                        for (int f = 0; f < task.Sensors.Length; f++)
                        {
                            var St = task.Sensors[f];
                            var MSTS = new MeasSubTaskSensor();
                            MSTS.Id = j; j++;
                            MSTS.Status = task.Status;
                            MSTS.SensorId = St.SensorId.Value;
                            ListMSTS.Add(MSTS);
                        }
                        MST.MeasSubTaskSensors = ListMSTS.ToArray();
                        ListMST.Add(MST);

                    }
                }

                task.MeasSubTasks = ListMST.ToArray();
            }
            //task.Status = Status.A.ToString();
        }


        public static void UpdateStatusSubTasks(this MeasTask task, long Id_Sensor, string Type, bool isOnline)
        {
            if (task.MeasSubTasks == null) return;
            var measSubTasks = task.MeasSubTasks.ToArray();
            for (int f = 0; f < measSubTasks.Length; f++)
            {
                var SubTask = measSubTasks[f];
                if (SubTask.MeasSubTaskSensors != null)
                {
                    var measSubTaskSensors = SubTask.MeasSubTaskSensors.ToArray();
                    for (int l = 0; l < measSubTaskSensors.Length; l++)
                    {
                        var SubTaskSensor = measSubTaskSensors[l];
                        if (SubTaskSensor.SensorId == Id_Sensor)
                        {
                            if (Type == MeasTaskMode.Run.ToString())
                            {
                                if (isOnline) SubTaskSensor.Status = Status.O.ToString();
                                else SubTaskSensor.Status = Status.A.ToString();
                            }
                            else if (Type == MeasTaskMode.Stop.ToString())
                            {
                                if (isOnline) SubTaskSensor.Status = Status.P.ToString(); 
                                else SubTaskSensor.Status = Status.F.ToString(); 
                            }
                            else if (Type == MeasTaskMode.Del.ToString())
                            {
                                SubTaskSensor.Status = Status.Z.ToString();
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

            var OperationTransitionRule = new List<RuleStatusTransition>();
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Stop.ToString(), Status.N.ToString(), Status.F.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Stop.ToString(), Status.A.ToString(), Status.F.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Run.ToString(), Status.F.ToString(), Status.A.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Stop.ToString(), Status.E_L.ToString(), Status.F.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Run.ToString(), Status.P.ToString(), Status.O.ToString()));
            OperationTransitionRule.Add(new RuleStatusTransition(MeasTaskMode.Stop.ToString(), Status.O.ToString(), Status.P.ToString()));


            var Descr_MeasSubTaskStation = new List<StatusDescription>();
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

            var Descr_MeasSubTask = new List<StatusDescription>();
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

            var MaxWeightLst = new List<int>();
            if (task.MeasSubTasks == null) return;

            var measSubTasks = task.MeasSubTasks.ToArray();
            for (int l = 0; l < measSubTasks.Length; l++)
            {
                var SubTask = measSubTasks[l];
                string StatusWithMaxWeight = "";
                MaxWeightLst = new List<int>();
                if (SubTask.MeasSubTaskSensors != null)
                {
                    var measSubTaskSensors = SubTask.MeasSubTaskSensors.ToArray();
                    for (int f = 0; f < measSubTaskSensors.Length; f++)
                    {
                        var SubTaskSensor = measSubTaskSensors[f];
                        if (Type == MeasTaskMode.Run.ToString())
                        {
                            if (SubTaskSensor.Status == Status.P.ToString())
                            {
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskSensor.Status && t.ToStatuses == Status.O.ToString())) != null)
                                {
                                    SubTaskSensor.Status = Status.O.ToString();
                                    SubTask.Status = Status.O.ToString();
                                    task.Status = Status.O.ToString();
                                }
                            }
                            if (SubTaskSensor.Status == Status.F.ToString())
                            {
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskSensor.Status && t.ToStatuses == Status.A.ToString())) != null)
                                {
                                    SubTaskSensor.Status = Status.A.ToString();
                                    SubTask.Status = Status.A.ToString();
                                    task.Status = Status.A.ToString();
                                }
                            }
                        }
                        else if (Type == MeasTaskMode.Del.ToString())
                        {
                            SubTaskSensor.Status = Status.Z.ToString();
                            SubTask.Status = Status.Z.ToString();
                            task.Status = Status.Z.ToString();
                        }
                        else if (Type == MeasTaskMode.Stop.ToString())
                        {
                            var DescrStat = Descr_MeasSubTaskStation.Find(t => t.Type != ModeStatus.final && t.NameStatus == SubTaskSensor.Status);
                            if (DescrStat != null)
                            {
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskSensor.Status && t.ToStatuses == Status.P.ToString())) != null)
                                {
                                    SubTaskSensor.Status = Status.P.ToString();
                                    SubTask.Status = Status.P.ToString();
                                    task.Status = Status.P.ToString();
                                }
                                if ((OperationTransitionRule.Find(t => t.NameOperation == Type && t.StartStatus == SubTaskSensor.Status && t.ToStatuses == Status.F.ToString())) != null)
                                {
                                    SubTaskSensor.Status = Status.F.ToString();
                                    SubTask.Status = Status.F.ToString();
                                    task.Status = Status.F.ToString();
                                }
                            }
                        }
                        var val_fnd_status = Descr_MeasSubTaskStation.Find(t => t.NameStatus == SubTaskSensor.Status);
                        if (val_fnd_status != null)
                        {
                            MaxWeightLst.Add(val_fnd_status.Weight);
                        }
                    }
                }
                if (MaxWeightLst.Count > 0)
                {
                    int Max_ = MaxWeightLst.Max();
                    var val_fnd_status_ = Descr_MeasSubTaskStation.Find(t => t.Weight == Max_);
                    if (val_fnd_status_ != null)
                    {
                        StatusWithMaxWeight = val_fnd_status_.NameStatus;
                        SubTask.Status = StatusWithMaxWeight;
                    }
                }
            }
            MaxWeightLst = new List<int>();
            var massMeasSubTasks = task.MeasSubTasks.ToArray();
            for (int r = 0; r < massMeasSubTasks.Length; r++)
            {
                var SubTask = massMeasSubTasks[r];
                var val_fnd_status = Descr_MeasSubTask.Find(t => t.NameStatus == SubTask.Status);
                if (val_fnd_status != null)
                {
                    MaxWeightLst.Add(val_fnd_status.Weight);
                }
            }
            if (MaxWeightLst.Count > 0)
            {
                int Max_ = MaxWeightLst.Max();
                var val_fnd_status_ = Descr_MeasSubTask.Find(t => t.Weight == Max_);
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
