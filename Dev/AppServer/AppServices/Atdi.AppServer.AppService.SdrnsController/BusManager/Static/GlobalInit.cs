using System;
using System.Collections.Generic;
using XMLLibrary;
using Atdi.Oracle.DataAccess;
using System.Configuration;


namespace Atdi.SDNRS.AppServer.BusManager
{
    /// <summary>
    /// 
    /// </summary>
    public static class GlobalInit
    {
        #region Lists_Global_Objects
        //public static ConcurrentDictionary<int,MeasTask> blockingCollectionMeasTask = new ConcurrentDictionary<int, MeasTask>();
        //public static BlockingCollection<MeasurementResults> blockingCollectionMeasurementResults = new BlockingCollection<MeasurementResults>();
        #endregion



        // Список объектов, содержащих сведения о текущем состоянии активности каждого сенсора
        public static List<Mdx> Lst_timers = new List<Mdx>();
        public static string MainRabbitMQServices = "";
        public static string RabbitHostName { get; }
        public static string RabbitUserName { get; }
        public static string RabbitVirtualHost { get; }
        public static string RabbitPassword { get; }
        public static string NameServer { get; }
        public static string ExchangePointFromDevices { get; }
        public static string ExchangePointFromServer { get; }
        public static string StartNameQueueServer { get; }
        public static string StartNameQueueDevice { get; }
        public static string ConcumerDescribe { get; }
        public static bool UseEncryption { get; }
        public static bool UseСompression { get; }
        public static int CheckActivitySensor { get; }
        public static int MaxTimeNotActivateStatusSensor { get; }

    

        public static void Initialization()
        {
            BaseXMLConfiguration sett = new BaseXMLConfiguration();
        }

        static GlobalInit()
        {
            System.Configuration.Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            GlobalInit.RabbitHostName = ConfigurationManager.AppSettings["RabbitHostName"];
            GlobalInit.RabbitUserName = ConfigurationManager.AppSettings["RabbitUserName"];
            GlobalInit.RabbitVirtualHost = ConfigurationManager.AppSettings["RabbitVirtualHostName"];
            GlobalInit.NameServer = ConfigurationManager.AppSettings["ServerInstance"];
            GlobalInit.ExchangePointFromDevices = ConfigurationManager.AppSettings["ExchangePointFromDevices"];
            GlobalInit.ExchangePointFromServer = ConfigurationManager.AppSettings["ExchangePointFromServer"];
            GlobalInit.StartNameQueueServer = ConfigurationManager.AppSettings["StartNameQueueServer"];
            GlobalInit.StartNameQueueDevice = ConfigurationManager.AppSettings["StartNameQueueDevice"];
            GlobalInit.ConcumerDescribe = ConfigurationManager.AppSettings["ConcumerDescribe"];
            GlobalInit.RabbitPassword = ConfigurationManager.AppSettings["RabbitMQ.Password"];
            GlobalInit.UseEncryption = ConfigurationManager.AppSettings["RabbitMQ.UseEncryption"].ToString().ToLower() == "false" ? false : true;
            GlobalInit.UseСompression = ConfigurationManager.AppSettings["RabbitMQ.UseСompression"].ToString().ToLower() == "false" ? false : true;
            GlobalInit.CheckActivitySensor = int.Parse(ConfigurationManager.AppSettings["CheckActivitySensor"].ToString());
            GlobalInit.MaxTimeNotActivateStatusSensor = int.Parse(ConfigurationManager.AppSettings["MaxTimeNotActivateStatusSensor"].ToString());

            if (!string.IsNullOrEmpty(GlobalInit.RabbitVirtualHost))
            {
                MainRabbitMQServices = string.Format("host={0}; username={1}; password={2}; virtualHost={3}", GlobalInit.RabbitHostName, GlobalInit.RabbitUserName, GlobalInit.RabbitPassword, GlobalInit.RabbitVirtualHost);
            }
            else
            {
                MainRabbitMQServices = string.Format("host={0}; username={1}; password={2}", GlobalInit.RabbitHostName, GlobalInit.RabbitUserName, GlobalInit.RabbitPassword);
            }

            BaseXMLConfiguration xml_conf = new BaseXMLConfiguration();
            GlobalInit.Initialization();
        }

    }

    public static class InitConnectionString
    {
        public static string oraDbString { get; set; }
    }
}
