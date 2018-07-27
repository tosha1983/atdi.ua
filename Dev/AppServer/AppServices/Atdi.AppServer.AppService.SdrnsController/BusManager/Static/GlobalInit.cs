using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using System.Collections;
using DAL;
using XMLLibrary;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.Sheduler;
using Atdi.AppServer;




namespace Atdi.SDNRS.AppServer.BusManager
{
    /// <summary>
    /// 
    /// </summary>
    public static class GlobalInit
    {
        public static ConnectDB conn { get; set; }
        #region Lists_Global_Objects
        // List all task (for service1)
        public static List<MeasTask> LIST_MEAS_TASK = new List<MeasTask>();
        // List all task (for service5)
        public static List<MeasSdrResults> MEAS_SDR_RESULTS = new List<MeasSdrResults>();
        // List all MeasurementResults
        public static List<MeasurementResults> LST_MeasurementResults = new List<MeasurementResults>();
        // List all results
        // Список активных объектов SENSOR
        public static List<Sensor> SensorListSDRNS = new List<Sensor>();
        // Перечень всех доступных устройств SDR
        // имя очереди для отправки списка наименований устройств в AppServer
        public static string DevicesAllList = "DevicesAllList";

        #endregion


        public static string MainRabbitMQServices = BaseXMLConfiguration.xml_conf._MainRabbitMQServices;
        // Значение по умолчанию для значения из XML (MEAS_TIME_PARAM_LIST.PER_INTERVAL = 300 сек)
        // Данное значение использувется если MEAS_TIME_PARAM_LIST.PER_INTERVAL = 0 !!!
        public static int DefaultValueMinTimeInterval = BaseXMLConfiguration.xml_conf._DefaultValueMinTimeInterval;

        // Список объектов, содержащих сведения о текущем состоянии активности каждого сенсора
        public static List<Mdx> Lst_timers = new List<Mdx>();

        #region Template_name_queue
        public static string Template_SENSORS_Main_List_APPServer = "SENSORS_Main_List_APPServer_";
        public static string Template_SENSORS_Main_List_SDR = "SENSORS_Main_List_SDR_";
        public static string Template_SENSORS_Main_List_Status_APPServer = "SENSORS_Main_List_Status_APPServer_";
        public static string Template_SENSORS_Stop_List = "SENSORS_Stop_List_";
        public static string Template_MEAS_RESULTS_Main_List_APPServer = "MEAS_RESULTS_Main_List_APPServer_";
        public static string Template_MEAS_RESULTS_Main_List_SDR = "MEAS_RESULTS_Main_List_SDR_";
        public static string Template_MEAS_RESULTS_Stop_List = "MEAS_RESULTS_Stop_List_";
        public static string Template_MEAS_SDR_RESULTS_Main_List_APPServer = "MEAS_SDR_RESULTS_Main_List_APPServer_";
        public static string Template_MEAS_SDR_RESULTS_Main_List_SDR = "MEAS_SDR_RESULTS_Main_List_SDR_";
        public static string Template_MEAS_SDR_RESULTS_Stop_List = "MEAS_SDR_RESULTS_Stop_List_";
        public static string Template_MEAS_TASK_Main_List_APPServer = "MEAS_TASK_Main_List_APPServer_";
        public static string Template_MEAS_TASK_Main_List_SDR = "MEAS_TASK_Main_List_SDR_";
        public static string Template_MEAS_TASK_Stop_List = "MEAS_TASK_Stop_List_";
        public static string Template_MEAS_TASK_SDR_Main_List_APPServer = "MEAS_TASK_SDR_Main_List_APPServer_";
        public static string Template_MEAS_TASK_SDR_Main_List_SDR = "MEAS_TASK_SDR_Main_List_SDR_";
        public static string Template_MEAS_TASK_SDR_Stop_List = "MEAS_TASK_SDR_Stop_List_";
        // имя очереди для отправки списка наименований очередей в заданную SDR
        public static string QueuesAllList = "QueuesAllList_";


        public static string Template_SENSOR_SDRNS_List_Request = "SENSOR_SDRNS_List_Request_";
        public static string Template_SENSOR_SDRNS_List_Response = "SENSOR_SDRNS_List_Response_";

        public static string Template_SENSORS_List_ = "SENSORS_List";
        public static bool BoolTemplate_SENSORS_List_ = false;

        



        //Очередь для приема подтверждений об успешной отправке сенсора с координатами в SDRNS
        public static string Template_Event_Confirm_SENSORS_Send_ = "Event_Confirm_SENSORS_Send_";
        
        //Очередь для отправки запросов в SDR на получение статуса сенсора
        public static string Template_Event_CheckActivitySensor_Req = "Event_CheckActivitySensor_Req_";
        //Очередь на  получение ответов от SDR о текущем состоянии сенсора
        public static string Template_Event_CheckActivitySensor_Resp = "Event_CheckActivitySensor_Resp_";


        //Очередь для отправки запросов в SDRNS на получение сенсора в WCF - сервис
        public static string Template_Event_Req_Sensor_ = "Event_Req_Sensor_";
        //Очередь для получения из SDRNS сенсора по запросу с  WCF - сервиса
        public static string Template_Event_Resp_Sensor_ = "Event_Resp_Sensor_";
        // Очередь хранения объектов MeasTaskSDR
        public static string Template_Event_UpdateStatus_MeasSubTasks_From_MeasTaskSDR = "Event_St_MeasSubTasks_From_MeasTaskSDR_";
        //Очередь для отправки подтверждений об успешном получении результатов со стороны SDR
        public static string Template_Event_Confirm_MeasTaskResults_Send_ = "Event_Confirm_MeasTaskResults_Send_";

        #endregion


        public static void Initialization()
        {
            BaseXMLConfiguration sett = new BaseXMLConfiguration();
        }

        static GlobalInit()
        {
            conn = new ConnectDB();
        }

    }
}
