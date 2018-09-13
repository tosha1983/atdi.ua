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
        // Значение по умолчанию для значения из XML (MEAS_TIME_PARAM_LIST.PER_INTERVAL = 300 сек)
        // Данное значение использувется если MEAS_TIME_PARAM_LIST.PER_INTERVAL = 0 !!!
        public static int DefaultValueMinTimeInterval = BaseXMLConfiguration.xml_conf._DefaultValueMinTimeInterval;
        #region Template_name_queue
        public static string DevicesAllList = "";
        public static string Template_SENSORS_Main_List_APPServer = "";
        public static string Template_SENSORS_Main_List_SDR = "";
        public static string Template_SENSORS_Main_List_Status_APPServer = "";
        public static string Template_SENSORS_Stop_List = "";
        public static string Template_MEAS_RESULTS_Main_List_APPServer = "";
        public static string Template_MEAS_RESULTS_Main_List_SDR = "";
        public static string Template_MEAS_RESULTS_Stop_List = "";
        public static string Template_MEAS_SDR_RESULTS_Main_List_APPServer = "";
        public static string Template_MEAS_SDR_RESULTS_Main_List_SDR = "";
        public static string Template_MEAS_SDR_RESULTS_Stop_List = "";
        public static string Template_MEAS_TASK_Main_List_APPServer = "";
        public static string Template_MEAS_TASK_Main_List_SDR = "";
        public static string Template_MEAS_TASK_Stop_List = "";
        public static string Template_MEAS_TASK_SDR_Main_List_APPServer = "";
        public static string Template_MEAS_TASK_SDR_Main_List_SDR = "";
        public static string Template_MEAS_TASK_SDR_Stop_List = "";
        // имя очереди для отправки списка наименований очередей в заданную SDR
        public static string QueuesAllList = "";


        public static string Template_SENSOR_SDRNS_List_Request = "";
        public static string Template_SENSOR_SDRNS_List_Response = "";

        public static string Template_SENSORS_List_ = "";
        public static bool BoolTemplate_SENSORS_List_ = false;


        //Очередь для приема подтверждений об успешной отправке сенсора с координатами в SDRNS
        public static string Template_Event_Confirm_SENSORS_Send_ = "";

        //Очередь для отправки запросов в SDR на получение статуса сенсора
        public static string Template_Event_CheckActivitySensor_Req = "";
        //Очередь на  получение ответов от SDR о текущем состоянии сенсора
        public static string Template_Event_CheckActivitySensor_Resp = "";


        //Очередь для отправки запросов в SDRNS на получение сенсора в WCF - сервис
        public static string Template_Event_Req_Sensor_ = "";
        //Очередь для получения из SDRNS сенсора по запросу с  WCF - сервиса
        public static string Template_Event_Resp_Sensor_ = "";
        // Очередь хранения объектов MeasTaskSDR
        public static string Template_Event_UpdateStatus_MeasSubTasks_From_MeasTaskSDR = "";
        //Очередь для отправки подтверждений об успешном получении результатов со стороны SDR
        public static string Template_Event_Confirm_MeasTaskResults_Send_ = "";

        public static string RabbitHostName { get; }
        public static string RabbitUserName { get; }
        public static string RabbitPassword { get; }
        public static string NameServer { get; }
        public static string ExchangePointFromDevices { get; }
        public static string ExchangePointFromServer { get; }
        public static string StartNameQueueServer { get; }
        public static string StartNameQueueDevice { get; }
        public static string ConcumerDescribe { get; }

        #endregion

        public static void Initialization()
        {
            BaseXMLConfiguration sett = new BaseXMLConfiguration();
        }

        static GlobalInit()
        {
            System.Configuration.Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            GlobalInit.DevicesAllList = ConfigurationManager.AppSettings["DevicesAllList"];
            GlobalInit.Template_SENSORS_Main_List_APPServer = ConfigurationManager.AppSettings["Template_SENSORS_Main_List_APPServer"];
            GlobalInit.Template_SENSORS_Main_List_SDR = ConfigurationManager.AppSettings["Template_SENSORS_Main_List_SDR"];
            GlobalInit.Template_SENSORS_Main_List_Status_APPServer = ConfigurationManager.AppSettings["Template_SENSORS_Main_List_Status_APPServer"];
            GlobalInit.Template_SENSORS_Stop_List = ConfigurationManager.AppSettings["Template_SENSORS_Stop_List"];
            GlobalInit.Template_MEAS_RESULTS_Main_List_APPServer = ConfigurationManager.AppSettings["Template_MEAS_RESULTS_Main_List_APPServer"];
            GlobalInit.Template_MEAS_RESULTS_Main_List_SDR = ConfigurationManager.AppSettings["Template_MEAS_RESULTS_Main_List_SDR"];
            GlobalInit.Template_MEAS_RESULTS_Stop_List = ConfigurationManager.AppSettings["Template_MEAS_RESULTS_Stop_List"];
            GlobalInit.Template_MEAS_SDR_RESULTS_Main_List_APPServer = ConfigurationManager.AppSettings["Template_MEAS_SDR_RESULTS_Main_List_APPServer"];
            GlobalInit.Template_MEAS_SDR_RESULTS_Main_List_SDR = ConfigurationManager.AppSettings["Template_MEAS_SDR_RESULTS_Main_List_SDR"];
            GlobalInit.Template_MEAS_SDR_RESULTS_Stop_List = ConfigurationManager.AppSettings["Template_MEAS_SDR_RESULTS_Stop_List"];
            GlobalInit.Template_MEAS_TASK_Main_List_APPServer = ConfigurationManager.AppSettings["Template_MEAS_TASK_Main_List_APPServer"];
            GlobalInit.Template_MEAS_TASK_Main_List_SDR = ConfigurationManager.AppSettings["Template_MEAS_TASK_Main_List_SDR"];
            GlobalInit.Template_MEAS_TASK_Stop_List = ConfigurationManager.AppSettings["Template_MEAS_TASK_Stop_List"];
            GlobalInit.Template_MEAS_TASK_SDR_Main_List_APPServer = ConfigurationManager.AppSettings["Template_MEAS_TASK_SDR_Main_List_APPServer"];
            GlobalInit.Template_MEAS_TASK_SDR_Main_List_SDR = ConfigurationManager.AppSettings["Template_MEAS_TASK_SDR_Main_List_SDR"];
            GlobalInit.Template_MEAS_TASK_SDR_Stop_List = ConfigurationManager.AppSettings["Template_MEAS_TASK_SDR_Stop_List"];
            GlobalInit.QueuesAllList = ConfigurationManager.AppSettings["QueuesAllList"];
            GlobalInit.Template_SENSOR_SDRNS_List_Request = ConfigurationManager.AppSettings["Template_SENSOR_SDRNS_List_Request"];
            GlobalInit.Template_SENSOR_SDRNS_List_Response = ConfigurationManager.AppSettings["Template_SENSOR_SDRNS_List_Response"];
            GlobalInit.Template_SENSORS_List_ = ConfigurationManager.AppSettings["Template_SENSORS_List_"];
            GlobalInit.Template_Event_Confirm_SENSORS_Send_ = ConfigurationManager.AppSettings["Template_Event_Confirm_SENSORS_Send_"];
            GlobalInit.Template_Event_CheckActivitySensor_Req = ConfigurationManager.AppSettings["Template_Event_CheckActivitySensor_Req"];
            GlobalInit.Template_Event_CheckActivitySensor_Resp = ConfigurationManager.AppSettings["Template_Event_CheckActivitySensor_Resp"];
            GlobalInit.Template_Event_Req_Sensor_ = ConfigurationManager.AppSettings["Template_Event_Req_Sensor_"];
            GlobalInit.Template_Event_Resp_Sensor_ = ConfigurationManager.AppSettings["Template_Event_Resp_Sensor_"];
            GlobalInit.Template_Event_UpdateStatus_MeasSubTasks_From_MeasTaskSDR = ConfigurationManager.AppSettings["Template_Event_UpdateStatus_MeasSubTasks_From_MeasTaskSDR"];
            GlobalInit.Template_Event_Confirm_MeasTaskResults_Send_ = ConfigurationManager.AppSettings["Template_Event_Confirm_MeasTaskResults_Send_"];
            GlobalInit.RabbitHostName = ConfigurationManager.AppSettings["RabbitHostName"];
            GlobalInit.RabbitUserName = ConfigurationManager.AppSettings["RabbitUserName"];
            GlobalInit.NameServer = ConfigurationManager.AppSettings["ServerInstance"];
            GlobalInit.ExchangePointFromDevices = ConfigurationManager.AppSettings["ExchangePointFromDevices"];
            GlobalInit.ExchangePointFromServer = ConfigurationManager.AppSettings["ExchangePointFromServer"];
            GlobalInit.StartNameQueueServer = ConfigurationManager.AppSettings["StartNameQueueServer"];
            GlobalInit.StartNameQueueDevice = ConfigurationManager.AppSettings["StartNameQueueDevice"];
            GlobalInit.ConcumerDescribe = ConfigurationManager.AppSettings["ConcumerDescribe"];
            GlobalInit.RabbitPassword = ConfigurationManager.AppSettings["RabbitMQ.Password"];
            MainRabbitMQServices = string.Format("host={0}; username={1}; password={2}", GlobalInit.RabbitHostName, GlobalInit.RabbitUserName, GlobalInit.RabbitPassword);
            BaseXMLConfiguration xml_conf = new BaseXMLConfiguration();
            GlobalInit.Initialization();
            Atdi.Oracle.DataAccess.OracleDataAccess oracleDataAccess = new OracleDataAccess();
            try
            {
                oracleDataAccess.OpenConnection(InitConnectionString.oraDbString);
            }
            catch (Exception) { }
        }

    }

    public static class InitConnectionString
    {
        public static string oraDbString { get; set; }
    }
}
