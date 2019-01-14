using System.Collections.Generic;
using Atdi.AppServer.Contracts.Sdrns;



namespace Atdi.AppUnits.Sdrn.ControlA.Handlers
{
 
    /// <summary>
    /// Универсвльный класс для привязки данных о сенсорах и времени существования объекта
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
    /// Специальный класс для добавления в коллекцию новых объектов типа MeasSDRResults или 
    /// удаление из коллекции объектов типа Sensor если статус==Z и время существования объекта > заданного TimeOut
    /// </summary>
    public static class LifeTimeObjectMeasSDRResults
    {
        public static List<ExtendClases<MeasSdrResults>> tmLife = new List<ExtendClases<MeasSdrResults>>();
     
        /// <summary>
        /// Удаление из общего списка объектов типа MeasSDRResults  -  таких, которые имеют статус Z
        /// и время их существования превышает время, заданное параметром TimeOut
        /// </summary>
        /// <param name="se"></param>
        /// <param name="TimeOut"></param>
        public static bool FindAndDestroyObject(this MeasSdrResults se, int TimeOut, string Status)
        {
            bool isFind = false;
            lock (tmLife) {
                ExtendClases<MeasSdrResults> rx = tmLife.Find(t => t.Key.Id == se.Id && t.Key.status == Status);
                if (rx != null) {
                    if (rx.Counter > TimeOut) {
                        tmLife.RemoveAll(t => t.Key.Id== se.Id && t.Key.status == Status);
                        isFind = true;
                    }
                }
            }
            return isFind;
        }


    }

    





}
