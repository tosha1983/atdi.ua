using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppUnits.Sdrn.ControlA.Bus;
using Atdi.AppUnits.Sdrn.ControlA;

namespace Atdi.AppUnits.Sdrn.ControlA.ManageDB
{
    public static class MeasTaskSDRExtend
    {


        /// <summary>
        /// Функция для определения расстояния между заданными координатами
        /// </summary>
        /// <param name="Lat1_"></param>
        /// <param name="Lon1_"></param>
        /// <param name="Lat2_"></param>
        /// <param name="Lon2_"></param>
        /// <returns>расстояние в метрах</returns>
        private static double calculateTheDistance(double Lat1_, double Lon1_, double Lat2_, double Lon2_)
        {
            const double EARTH_RADIUS = 6372795;
            double Lat1 = Lat1_ * Math.PI / 180;
            double Lat2 = Lat2_ * Math.PI / 180;
            double Lon1 = Lon1_ * Math.PI / 180;
            double Lon2 = Lon2_ * Math.PI / 180;
            double cl1 = Math.Cos(Lat1);
            double cl2 = Math.Cos(Lat2);
            double sl1 = Math.Sin(Lat1);
            double sl2 = Math.Sin(Lat2);
            double delta = Lon2 - Lon1;
            double cdelta = Math.Cos(delta);
            double sdelta = Math.Sin(delta);
            double y = Math.Sqrt(Math.Pow(cl2 * sdelta, 2) + Math.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
            double x = sl1 * sl2 + cl1 * cl2 * cdelta;
            double ad = Math.Atan2(y, x);
            double dist = ad * EARTH_RADIUS;
            return dist;
        }
        /// <summary>
        /// Обновление статуса объекта MeasTaskSDR
        /// </summary>
        /// <param name="taskSDR"></param>
        public static void UpdateStatus(this MeasSdrTask taskSDR)
        {
            try
            {
                string OldStatus = taskSDR.status;
                if (!CheckDate(taskSDR))
                {
                    if (taskSDR.status == "N") taskSDR.status = "E_T";
                    else if (taskSDR.status == "A") taskSDR.status = "E_T";
                    else if (taskSDR.status == "E_L") taskSDR.status = "E_T";
                }
                else
                {
                    if (CheckLoc(taskSDR))
                    {
                        if (taskSDR.status == "N") taskSDR.status = "A";
                        else if (taskSDR.status == "A") taskSDR.status = "A";
                        else if (taskSDR.status == "E_L") taskSDR.status = "A";
                    }
                    else
                    {
                        if (taskSDR.status == "N") taskSDR.status = "E_L";
                        else if (taskSDR.status == "A") taskSDR.status = "E_L";
                        else if (taskSDR.status == "E_L") taskSDR.status = "E_L";
                    }
                }
                Launcher._messagePublisher.Send("SendMeasSdrTask", taskSDR);
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.MeasSdrTaskUpdateStatus, Events.UpdateStatus, ex.Message, null);
            }
        }
        /// <summary>
        /// Функция CheckLoc  возвращает true если MeasTaskSDR не имеет MeasLocParam 
        /// или хотя бы один из параметров MeasLocParam находится на расстоянии от 
        /// текущих координат сенсора Sensor.SensorLocation  ближе чем MeasLocParam.MaxDist
        /// </summary>
        /// <param name="taskSDR"></param>
        /// <returns></returns>
        public static bool CheckLoc(this MeasSdrTask taskSDR)
        {
            var listSensor = new List<Sensor>();
            var extension = new SensorDb();
            bool isChecked = false;
            try
            {
                if (taskSDR != null)
                {
                    if (taskSDR.MeasLocParam == null)
                        isChecked = true;
                    if (taskSDR.MeasLocParam != null)
                    {
                        if (taskSDR.MeasLocParam.Count() > 0)
                        {
                            for (int i = 0; i < taskSDR.MeasLocParam.Count(); i++)
                            {
                                var se = extension.GetCurrentSensor();
                                if (se != null)
                                {
                                    if (se.Locations != null)
                                    {
                                        var locationCurr = se.Locations.ToList().FindAll(r => r.Status == AllStatusSensor.A.ToString());
                                        if (locationCurr != null)
                                        {
                                            if (locationCurr.Count > 0)
                                            {
                                                locationCurr.Sort((a1, a2) => a1.DataFrom.GetValueOrDefault().CompareTo(a2.DataFrom.GetValueOrDefault()));
                                                var sloc = locationCurr[locationCurr.Count - 1];
                                                if (sloc != null)
                                                {
                                                    double Dist_curr = calculateTheDistance(sloc.Lat.GetValueOrDefault(), sloc.Lon.GetValueOrDefault(), taskSDR.MeasLocParam[i].Lat.GetValueOrDefault(), taskSDR.MeasLocParam[i].Lon.GetValueOrDefault());
                                                    if (Dist_curr < taskSDR.MeasLocParam[i].MaxDist)
                                                    {
                                                        isChecked = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else isChecked = true;
                            }
                        }
                        else isChecked = true;
                    }
                    else isChecked = true;
                }
            }
            catch (Exception ex)
            {
                Launcher._logger.Error(Contexts.ThisComponent, Categories.MessageCheckLocation, Events.CheckLocation, ex.Message, null);
            }
            return isChecked;
        }

        /// <summary>
        /// Функция CheckDate возвращает true если текущая дата находится до MeasTaskSD.TimeStop
        /// </summary>
        /// <param name="taskSDR"></param>
        /// <returns></returns>
        public static bool CheckDate(this MeasSdrTask taskSDR)
        {
            bool isChecked = false;
            if (taskSDR != null)
            {
                if (DateTime.Now < taskSDR.Time_stop)
                    isChecked = true;
            }
            return isChecked;
        }

        public static object SetNullValue(object v)
        {
            if (v != null)
            {
                Type myType = v.GetType();
                foreach (var propertyInfo in myType.GetProperties())
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
}
