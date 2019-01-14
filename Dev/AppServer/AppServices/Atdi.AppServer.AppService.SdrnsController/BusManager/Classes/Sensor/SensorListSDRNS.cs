using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLLibrary;
using Atdi.SDNRS.AppServer.ManageDB.Adapters;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer;
using RabbitMQ.Client;

namespace Atdi.SDNRS.AppServer.BusManager
{
    public enum AllStatusSensor
    {
        N,
        A,
        Z,
        E,
        C,
        F
    }
    public enum AllStatusLocation
    {
        A,
        Z
    }
    public class SensorListSDRNS : IDisposable
    {
        public static ILogger logger;
        public SensorListSDRNS(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~SensorListSDRNS()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// формирует список ShortSensor для ответа на запроса клиента.
        /// </summary>
        /// <returns></returns>
        public List<ShortSensor> CreateShortSensorList()
        {
            logger.Trace("Start procedure CreateShortSensorList...");
            List<ShortSensor> val = new List<ShortSensor>();
            //System.Threading.Thread thread = new System.Threading.Thread(() =>
            //{
            ClassDBGetSensor DB = new ClassDBGetSensor(logger);
            List<Sensor> val_all_s = DB.LoadObjectSensor();
            {
                if (val_all_s != null)
                {
                    foreach (Sensor sd in val_all_s.ToArray())
                    {
                        ShortSensor sh = new ShortSensor();
                        sh.Name = sd.Name;
                        sh.Status = sd.Status;
                        sh.Administration = sd.Administration;
                        if (sd.Antenna != null)
                        {
                            sh.AntGainMax = sd.Antenna.GainMax;
                            sh.AntManufacturer = sd.Antenna.Manufacturer;
                            sh.AntName = sd.Antenna.Name;
                        }
                        sh.NetworkId = sd.NetworkId;
                        sh.RxLoss = sd.RxLoss;
                        if (sd.Equipment != null)
                        {
                            sh.UpperFreq = sd.Equipment.UpperFreq;
                            sh.EquipName = sd.Equipment.Name;
                            sh.EquipManufacturer = sd.Equipment.Manufacturer;
                            sh.EquipCode = sd.Equipment.Code;
                            sh.LowerFreq = sd.Equipment.LowerFreq;
                        }
                        sh.Id = sd.Id;
                        sh.EouseDate = sd.EouseDate;
                        sh.DateCreated = sd.DateCreated;
                        sh.CreatedBy = sd.CreatedBy;
                        sh.BiuseDate = sd.BiuseDate;

                        val.Add(sh);
                    }
                }
            }
            //});
            //thread.Start();
            //thread.Join();
            logger.Trace("End procedure CreateShortSensorList.");
            return val;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ShortSensor CreateShortSensorListBySensorId(int Id)
        {
            logger.Trace("Start procedure CreateShortSensorListBySensorId...");
            ShortSensor val = new ShortSensor();
            //System.Threading.Thread thread = new System.Threading.Thread(() =>
            //{
            ClassDBGetSensor DB = new ClassDBGetSensor(logger);
            Sensor sd = DB.LoadObjectSensor(Id);
            if (sd != null)
            {
                ShortSensor sh = new ShortSensor();
                sh.Name = sd.Name;
                sh.Status = sd.Status;
                sh.Administration = sd.Administration;
                if (sd.Antenna != null)
                {
                    sh.AntGainMax = sd.Antenna.GainMax;
                    sh.AntManufacturer = sd.Antenna.Manufacturer;
                    sh.AntName = sd.Antenna.Name;
                }
                sh.NetworkId = sd.NetworkId;
                sh.RxLoss = sd.RxLoss;
                if (sd.Equipment != null)
                {
                    sh.UpperFreq = sd.Equipment.UpperFreq;
                    sh.EquipName = sd.Equipment.Name;
                    sh.EquipManufacturer = sd.Equipment.Manufacturer;
                    sh.EquipCode = sd.Equipment.Code;
                    sh.LowerFreq = sd.Equipment.LowerFreq;
                }
                sh.Id = sd.Id;
                sh.EouseDate = sd.EouseDate;
                sh.DateCreated = sd.DateCreated;
                sh.CreatedBy = sd.CreatedBy;
                sh.BiuseDate = sd.BiuseDate;
                val = sh;
            }
            //});
            //thread.Start();
            //thread.Join();
            logger.Trace("End procedure CreateShortSensorListBySensorId.");
            return val;
        }
    }

    /// <summary>
    /// Специальный класс для хранения временных отрезков, указывающих на активность или недоступность сенсора
    /// </summary>
    public class Mdx
    {
        public Sensor se { get; set; }
        public System.Timers.Timer BZ { get; set; }
        public Int64 Cnt_timer { get; set; }
        public Int64 Cnt_all_time { get; set; }
        public Int64 Cnt_sensor_New { get; set; }
        public Int64 Cnt_sensor_Old { get; set; }
        public Mdx(Sensor v)
        {
            BZ = new System.Timers.Timer();
            BZ.Interval = 1000;
            BZ.Elapsed += BZ_Elapsed;
            BZ.Start();
            Cnt_timer = 0;
            Cnt_all_time = 0;
            Cnt_sensor_New = 0;
            Cnt_sensor_Old = 0;
            se = new Sensor();
            se = v;
        }

        private void BZ_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Cnt_timer++; Cnt_all_time++;
        }
    }
}
