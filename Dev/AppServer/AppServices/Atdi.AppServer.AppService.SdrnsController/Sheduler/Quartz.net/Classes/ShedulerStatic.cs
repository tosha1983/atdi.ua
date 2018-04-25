using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.SDNRS.AppServer.Sheduler
{
    public class ShedulerStatic
    {
        /// <summary>
        /// For sensor sheduler load from database
        /// </summary>
        public static string Name_job_sensor_load = "job_sensor_load";
        public static string Name_trigger_sensor_load = "trigger_sensor_load";
        public static string Name_group_sensor_load = "group_sensor_load";
       
        /// <summary>
        /// For sensor sheduler submit to the SDR server
        /// </summary>
        public static string Name_job_sensor_submit = "job_sensor_submit";
        public static string Name_trigger_sensor_submit = "trigger_sensor_submit";
        public static string Name_group_sensor_submit = "group_sensor_submit";

        /// <summary>
        /// For meas_task sheduler submit to the SDR server
        /// </summary>
        public static string Name_job_meas_task_submit = "job_meas_task_submit";
        public static string Name_trigger_meas_task_submit = "trigger_meas_task_submit";
        public static string Name_group_meas_task_submit = "group_meas_task_submit";

        /// <summary>
        /// Переменные для планировщика, который выполняет периодическое сканирование списка
        /// доступных устройств
        /// </summary>
        public static string Name_job_device_submit = "job_device_submit";
        public static string Name_trigger_device_submit = "trigger_device_submit";
        public static string Name_group_device_submit = "group_device_submit";


        /// <summary>
        /// Переменные для планировщика, который выполняет периодическую отправку списков очередей, которыми может манипулировать SDR
        /// доступных устройств
        /// </summary>
        public static string Name_job_submit_list_queus_submit = "job_submit_list_queus_submit";
        public static string Name_trigger_submit_list_queus_submit = "trigger_submit_list_queus_submit";
        public static string Name_group_submit_list_queus_submit = "group_submit_list_queus_submit";




        /// <summary>
        /// Переменные для планировщика, который выполняет периодическое обновление списка тасков 
        /// доступных устройств
        /// </summary>
        public static string Name_job_meas_task_scan = "job_meas_task_scan";
        public static string Name_trigger_meas_task_scan = "trigger_meas_task_scan";
        public static string Name_group_meas_task_scan = "group_meas_task_scan";

        /// <summary>
        /// Переменные для планировщика, который выполняет периодическое получение результатов измерений 
        /// </summary>
        public static string Name_job_meas_sdr_results = "job_meas_sdr_results";
        public static string Name_trigger_meas_sdr_results = "trigger_meas_sdr_results";
        public static string Name_group_meas_sdr_results = "group_meas_sdr_results";

        public static string Name_job_CheckActivitySensor = "job_meas_CheckActivitySensor";
        public static string Name_trigger_CheckActivitySensor = "trigger_CheckActivitySensor";
        public static string Name_group_CheckActivitySensor = "group_CheckActivitySensor";


        public static string Name_job_GetSensorList = "job_meas_GetSensorList";
        public static string Name_trigger_GetSensorList = "trigger_GetSensorList";
        public static string Name_group_GetSensorList = "group_GetSensorList";


        public static string Name_job_CheckStatusF = "job_meas_CheckStatusF";
        public static string Name_trigger_CheckStatusF = "trigger_CheckStatusF";
        public static string Name_group_CheckStatusF = "group_CheckStatusF";

        public static string Name_job_meas_sdr_results_arch = "job_meas_sdr_results_arch";
        public static string Name_trigger_meas_sdr_results_arch = "trigger_meas_sdr_results_arch";
        public static string Name_group_meas_sdr_results_arch = "group_meas_sdr_results_arch";



    }
}
