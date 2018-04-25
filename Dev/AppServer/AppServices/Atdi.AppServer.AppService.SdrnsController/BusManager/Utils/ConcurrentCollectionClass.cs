using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.BusManager;

namespace Atdi.SDR.Server.Utils
{
 
    /// <summary>
    /// Универсвльный класс для привязки данных объектах  и времени существования объекта
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtendClases<T>
    {
        public  int Counter { get; set; }
        private  System.Timers.Timer tx = new System.Timers.Timer();
        public  T Key { get; set; }
        public ExtendClases(T in_s)
        {
            Key = in_s;
            Counter = 0;
            tx.Interval = 1000;
            tx.Elapsed += Tx_Elapsed;
            tx.Start();
        }
        private void Tx_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Counter++;
        }
    }


    /// <summary>
    /// Специальный класс для добавления в коллекцию новых объектов типа MeasTask или 
    /// удаление из коллекции объектов типа Sensor если статус==Z и время существования объекта > заданного TimeOut
    /// </summary>
    public static class LifeTimeObjectMeasTaskSDR
    {
        public static List<ExtendClases<MeasTask>> tmLife = new List<ExtendClases<MeasTask>>();
        /// <summary>
        /// Добавление нового объекта в список объектов и старт таймера
        /// для отсчитывания времени его существования
        /// </summary>
        /// <param name="se"></param>
        public static void StartLifeTime(this MeasTask se)
        {
            lock (tmLife) {
                ExtendClases<MeasTask> rx = tmLife.Find(t =>  t.Key.Id.Value == se.Id.Value);
                if (rx == null)  {
                    tmLife.Add(new ExtendClases<MeasTask>(se));
                    GlobalInit.LIST_MEAS_TASK.RemoveAll(t => t.Id.Value == se.Id.Value);
                    GlobalInit.LIST_MEAS_TASK.Add(se);
                }
                else {
                    if (tmLife.Find(t => t.Key.Id.Value == se.Id.Value) != null) {
                        tmLife.RemoveAll(t => t.Key.Id.Value == se.Id.Value);
                        tmLife.Add(new ExtendClases<MeasTask>(se));
                        GlobalInit.LIST_MEAS_TASK.RemoveAll(t => t.Id.Value == se.Id.Value);
                        GlobalInit.LIST_MEAS_TASK.Add(se);
                    }
                }
            }
        }
        /// <summary>
        /// Удаление из общего списка объектов типа MeasTask  -  таких, которые имеют статус Z
        /// и время их существования превышает время, заданное параметром TimeOut
        /// </summary>
        /// <param name="se"></param>
        /// <param name="TimeOut"></param>
        public static void FindAndDestroyObject(this MeasTask se, int TimeOut, string Status)
        {
            lock (tmLife) {
                ExtendClases<MeasTask> rx = tmLife.Find(t => t.Key.Id.Value == se.Id.Value && t.Key.Status==Status);
                if (rx != null) {
                    if (rx.Counter > TimeOut) {
                        tmLife.RemoveAll(t => t.Key.Id.Value == se.Id.Value && t.Key.Status == Status);
                        GlobalInit.LIST_MEAS_TASK.RemoveAll(t => t.Id.Value == se.Id.Value && t.Status == Status);
                    }
                }
            }
        }

       
    }

    /// <summary>
    /// Специальный класс для добавления в коллекцию новых объектов типа MeasSDRResults или 
    /// удаление из коллекции объектов типа Sensor если статус==Z и время существования объекта > заданного TimeOut
    /// </summary>
    public static class LifeTimeObjectMeasSDRResults
    {
        public static List<ExtendClases<MeasSdrResults>> tmLife = new List<ExtendClases<MeasSdrResults>>();
        /// <summary>
        /// Добавление нового объекта в список объектов и старт таймера
        /// для отсчитывания времени его существования
        /// </summary>
        /// <param name="se"></param>
        public static void StartLifeTime(this MeasSdrResults se)
        {
            lock (tmLife) {
                ExtendClases<MeasSdrResults> rx = tmLife.Find(t => t.Key.MeasTaskId.Value == se.MeasTaskId.Value && t.Key.MeasSubTaskId.Value == se.MeasSubTaskId.Value && t.Key.MeasSubTaskStationId == se.MeasSubTaskStationId && t.Key.SensorId.Value == se.SensorId.Value);
                if (rx == null) {
                    tmLife.Add(new ExtendClases<MeasSdrResults>(se));
                    GlobalInit.MEAS_SDR_RESULTS.RemoveAll(t => t.MeasTaskId.Value == se.MeasTaskId.Value && t.MeasSubTaskId.Value == se.MeasSubTaskId.Value && t.MeasSubTaskStationId == se.MeasSubTaskStationId && t.SensorId.Value == se.SensorId.Value);
                    GlobalInit.MEAS_SDR_RESULTS.Add(se);
                }
                else {
                    if (tmLife.Find(t => t.Key.MeasTaskId.Value == se.MeasTaskId.Value && t.Key.MeasSubTaskId.Value == se.MeasSubTaskId.Value && t.Key.MeasSubTaskStationId == se.MeasSubTaskStationId && t.Key.SensorId.Value == se.SensorId.Value) != null) {
                        tmLife.RemoveAll(t => t.Key.MeasTaskId.Value == se.MeasTaskId.Value && t.Key.MeasSubTaskId.Value == se.MeasSubTaskId.Value && t.Key.MeasSubTaskStationId == se.MeasSubTaskStationId && t.Key.SensorId.Value == se.SensorId.Value);
                        tmLife.Add(new ExtendClases<MeasSdrResults>(se));
                        GlobalInit.MEAS_SDR_RESULTS.RemoveAll(t => t.MeasTaskId.Value == se.MeasTaskId.Value && t.MeasSubTaskId.Value == se.MeasSubTaskId.Value && t.MeasSubTaskStationId == se.MeasSubTaskStationId && t.SensorId.Value == se.SensorId.Value);
                        GlobalInit.MEAS_SDR_RESULTS.Add(se);
                    }
                }
            }
        }
        /// <summary>
        /// Удаление из общего списка объектов типа MeasSDRResults  -  таких, которые имеют статус Z
        /// и время их существования превышает время, заданное параметром TimeOut
        /// </summary>
        /// <param name="se"></param>
        /// <param name="TimeOut"></param>
        public static void FindAndDestroyObject(this MeasSdrResults se, int TimeOut, string Status)
        {
            lock (tmLife) {
                ExtendClases<MeasSdrResults> rx = tmLife.Find(t => t.Key.MeasTaskId.Value == se.MeasTaskId.Value && t.Key.MeasSubTaskId.Value == se.MeasSubTaskId.Value && t.Key.MeasSubTaskStationId == se.MeasSubTaskStationId && t.Key.SensorId.Value == se.SensorId.Value && t.Key.status == Status);
                if (rx != null) {
                    if (rx.Counter > TimeOut) {
                        tmLife.RemoveAll(t => t.Key.MeasTaskId.Value == se.MeasTaskId.Value && t.Key.MeasSubTaskId.Value == se.MeasSubTaskId.Value && t.Key.MeasSubTaskStationId == se.MeasSubTaskStationId && t.Key.SensorId.Value == se.SensorId.Value && t.Key.status == Status);
                        GlobalInit.MEAS_SDR_RESULTS.RemoveAll(t => t.MeasTaskId.Value == se.MeasTaskId.Value && t.MeasSubTaskId.Value == se.MeasSubTaskId.Value && t.MeasSubTaskStationId == se.MeasSubTaskStationId && t.SensorId.Value == se.SensorId.Value && t.status == Status);
                    }
                }
            }
        }


    }



    /// <summary>
    /// Специальный класс для добавления в коллекцию новых объектов типа MeasurementResults или 
    /// удаление из коллекции объектов типа Sensor если статус==Z и время существования объекта > заданного TimeOut
    /// </summary>
    public static class LifeTimeObjectMeasurementResults
    {
        public static List<ExtendClases<MeasurementResults>> tmLife = new List<ExtendClases<MeasurementResults>>();
        /// <summary>
        /// Добавление нового объекта в список объектов и старт таймера
        /// для отсчитывания времени его существования
        /// </summary>
        /// <param name="se"></param>
        public static void StartLifeTime(this MeasurementResults se)
        {
            lock (tmLife)
            {
                ExtendClases<MeasurementResults> rx = tmLife.Find(t => t.Key.Id.MeasTaskId.Value == se.Id.MeasTaskId.Value && t.Key.StationMeasurements.StationId.Value == se.StationMeasurements.StationId.Value && t.Key.Id.SubMeasTaskId == se.Id.SubMeasTaskId && t.Key.Id.SubMeasTaskStationId == se.Id.SubMeasTaskStationId);
                MeasurementResults res = GlobalInit.LST_MeasurementResults.Find(t => t.Id.MeasTaskId.Value == se.Id.MeasTaskId.Value && t.StationMeasurements.StationId.Value == se.StationMeasurements.StationId.Value && t.Id.SubMeasTaskId == se.Id.SubMeasTaskId && t.Id.SubMeasTaskStationId == se.Id.SubMeasTaskStationId);
                if ((rx == null) && (res == null))
                {
                    tmLife.Add(new ExtendClases<MeasurementResults>(se));
                    GlobalInit.LST_MeasurementResults.RemoveAll(t => t.Id.MeasTaskId.Value == se.Id.MeasTaskId.Value && t.StationMeasurements.StationId.Value == se.StationMeasurements.StationId.Value && t.Id.SubMeasTaskId == se.Id.SubMeasTaskId && t.Id.SubMeasTaskStationId == se.Id.SubMeasTaskStationId);
                    GlobalInit.LST_MeasurementResults.Add(se);
                }
                else {
                    if ((tmLife.Find(t => t.Key.Id.MeasTaskId.Value == se.Id.MeasTaskId.Value && t.Key.StationMeasurements.StationId.Value == se.StationMeasurements.StationId.Value && t.Key.Id.SubMeasTaskId == se.Id.SubMeasTaskId && t.Key.Id.SubMeasTaskStationId == se.Id.SubMeasTaskStationId) != null) || (res != null))
                    {
                        tmLife.RemoveAll(t => t.Key.Id.MeasTaskId.Value == se.Id.MeasTaskId.Value && t.Key.StationMeasurements.StationId.Value == se.StationMeasurements.StationId.Value && t.Key.Id.SubMeasTaskId == se.Id.SubMeasTaskId && t.Key.Id.SubMeasTaskStationId == se.Id.SubMeasTaskStationId);
                        tmLife.Add(new ExtendClases<MeasurementResults>(se));
                        GlobalInit.LST_MeasurementResults.RemoveAll(t => t.Id.MeasTaskId.Value == se.Id.MeasTaskId.Value && t.StationMeasurements.StationId.Value == se.StationMeasurements.StationId.Value && t.Id.SubMeasTaskId == se.Id.SubMeasTaskId && t.Id.SubMeasTaskStationId == se.Id.SubMeasTaskStationId);
                        GlobalInit.LST_MeasurementResults.Add(se);
                    }
                }
            }
        }
        /// <summary>
        /// Удаление из общего списка объектов типа MeasurementResults  -  таких, которые имеют статус Z
        /// и время их существования превышает время, заданное параметром TimeOut
        /// </summary>
        /// <param name="se"></param>
        /// <param name="TimeOut"></param>
        public static bool FindAndDestroyObject(this MeasurementResults se, int TimeOut, string Status)
        {
            bool isSuccessDestroy = false;
            lock (tmLife)
            {
                ExtendClases<MeasurementResults> rx = tmLife.Find(t => t.Key.Id.MeasTaskId.Value == se.Id.MeasTaskId.Value && t.Key.StationMeasurements.StationId.Value == se.StationMeasurements.StationId.Value && t.Key.Id.SubMeasTaskId == se.Id.SubMeasTaskId && t.Key.Id.SubMeasTaskStationId == se.Id.SubMeasTaskStationId && t.Key.Status == Status);
                if (rx != null)
                {
                    if (rx.Counter > TimeOut)
                    {
                        tmLife.RemoveAll(t => t.Key.Id.MeasTaskId.Value == se.Id.MeasTaskId.Value && t.Key.StationMeasurements.StationId.Value == se.StationMeasurements.StationId.Value && t.Key.Id.SubMeasTaskId == se.Id.SubMeasTaskId && t.Key.Id.SubMeasTaskStationId == se.Id.SubMeasTaskStationId && t.Key.Status == Status);
                        GlobalInit.LST_MeasurementResults.RemoveAll(t => t.Id.MeasTaskId.Value == se.Id.MeasTaskId.Value && t.StationMeasurements.StationId.Value == se.StationMeasurements.StationId.Value && t.Id.SubMeasTaskId == se.Id.SubMeasTaskId && t.Id.SubMeasTaskStationId == se.Id.SubMeasTaskStationId && t.Status == Status);
                        isSuccessDestroy = true;
                    }
                }
            }
            return isSuccessDestroy;
        }


    }




}
