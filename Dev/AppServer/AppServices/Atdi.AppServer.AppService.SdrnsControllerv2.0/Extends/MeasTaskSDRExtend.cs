using System;
using System.Linq;
using Atdi.AppServer.Contracts.Sdrns;


namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    public static class MeasTaskSDRExtend
    {
        /// <summary>
        /// функция валидации. Проверка железа без координат
        /// </summary>
        /// <param name="taskSDR"></param>
        /// <returns></returns>
        public static bool ValidationMeas(this MeasSdrTask taskSDR)
        {
            if (taskSDR != null) {
                if (taskSDR.SensorId != null) {
                    /// здесь код проверки
                }
            }
            return true;
        }

        /// <summary>
        /// Если валидация MeasTaskSDR была не успешна, то установка для MeasSubTaskStation.Status="E_E"
        /// </summary>
        /// <param name="task"></param>
        /// <param name="ID_MEasSubTaskStation"></param>
        /// <param name="ToStatus"></param>
        public static void UpdateStatusE_E(this MeasTask task, int ID_MEasSubTaskStation, string ToStatus)
        {
            if (task != null) {
                foreach (MeasSubTask SubTask in task.MeasSubTasks.ToArray()) {
                    foreach (MeasSubTaskStation SubTaskStation in SubTask.MeasSubTaskStations.ToArray()) {
                        if (SubTaskStation.Id == ID_MEasSubTaskStation) {
                            SubTaskStation.Status = ToStatus;
                        }
                    }
                }
            }
        }
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
            double dist = ad* EARTH_RADIUS;
            return dist;
        }
        /// <summary>
        /// Обновление статуса объекта MeasTaskSDR
        /// </summary>
        /// <param name="taskSDR"></param>
        public static void UpdateStatus(this MeasSdrTask taskSDR, ILogger logger)
        {
            if (!CheckDate(taskSDR)) {
                if (taskSDR.status == "N") taskSDR.status = "E_T";
                else if (taskSDR.status == "A") taskSDR.status = "E_T";
                else if (taskSDR.status == "E_L") taskSDR.status = "E_T";
            }
            else {
                //if (CheckLoc(taskSDR, logger)) {
                    //if (taskSDR.status == "N") taskSDR.status = "A";
                    //else if (taskSDR.status == "A") taskSDR.status = "A";
                    //else if (taskSDR.status == "E_L") taskSDR.status = "A";
                //}
                //else 
                {
                    if (taskSDR.status == "N") taskSDR.status = "E_L";
                    else if (taskSDR.status == "A") taskSDR.status = "E_L";
                    else if (taskSDR.status == "E_L") taskSDR.status = "E_L";
                }
            }
        }
        /*
        /// <summary>
        /// Функция CheckLoc  возвращает true если MeasTaskSDR не имеет MeasLocParam 
        /// или хотя бы один из параметров MeasLocParam находится на расстоянии от 
        /// текущих координат сенсора Sensor.SensorLocation  ближе чем MeasLocParam.MaxDist
        /// </summary>
        /// <param name="taskSDR"></param>
        /// <returns></returns>
        public static bool CheckLoc(this MeasSdrTask taskSDR, ILogger logger)
        {
            ClassDBGetSensor gsd = new ClassDBGetSensor(logger);
            bool isChecked = false;
            if (taskSDR!=null) {
                if (taskSDR.MeasLocParam == null)
                    return true;
                if (taskSDR.MeasLocParam != null) {
                    if (taskSDR.MeasLocParam.Count() > 0) {
                        foreach (MeasLocParam locPrm in taskSDR.MeasLocParam.ToArray()) {
                            Atdi.DataModels.Sdrns.Device.Sensor Sensor_fnd = gsd.LoadObjectSensor(taskSDR.SensorId.Value);
                            if (Sensor_fnd != null) {
                                if (Sensor_fnd.Locations != null) {
                                    List<SensorLocation> location_curr = Sensor_fnd.Locations.ToList().FindAll(r => r.Status == "A");
                                    if (location_curr != null) {
                                        if (location_curr.Count > 0) {
                                            location_curr.Sort((a1, a2) => a1.DataFrom.GetValueOrDefault().CompareTo(a2.DataFrom.GetValueOrDefault()));
                                            SensorLocation sloc = location_curr[location_curr.Count - 1];
                                            if (sloc != null) {
                                                double Dist_curr = calculateTheDistance(sloc.Lat.GetValueOrDefault(), sloc.Lon.GetValueOrDefault(), locPrm.Lat.GetValueOrDefault(), locPrm.Lon.GetValueOrDefault());
                                                if (Dist_curr < locPrm.MaxDist) {
                                                    isChecked = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else isChecked = true;
                }
            }
            return isChecked;
        }
        */
        /// <summary>
        /// Функция CheckDate возвращает true если текущая дата находится до MeasTaskSD.TimeStop
        /// </summary>
        /// <param name="taskSDR"></param>
        /// <returns></returns>
        public static bool CheckDate(this MeasSdrTask taskSDR)
        {
            bool isChecked = false;
            if (taskSDR != null) {
                if (DateTime.Now< taskSDR.Time_stop)
                    isChecked = true;
            }
            return isChecked;
        }
    }
}
