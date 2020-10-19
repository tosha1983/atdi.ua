using System;
using System.Collections.ObjectModel;

namespace ControlU.DB
{
    public class localatdi_meas_task : PropertyChangedBase
    {
        public string task_id
        {
            get { return _task_id; }
            set { _task_id = value; OnPropertyChanged("task_id"); }
        }
        private string _task_id = "";

        public string equipment_tech_id
        {
            get { return _equipment_tech_id; }
            set { _equipment_tech_id = value; /*OnPropertyChanged("equipment_tech_id");*/ }
        }
        private string _equipment_tech_id = "";

        /// <summary>
        /// mobEqipmentMeasurements = enum Atdi.DataModels.Sdrns.MeasurementType 
        /// </summary>
        public int[] xms_eqipment
        {
            get { return _xms_eqipment; }
            set { _xms_eqipment = value; /*OnPropertyChanged("xms_eqipment");*/ }
        }
        private int[] _xms_eqipment = new int[] { };

        public int priority
        {
            get { return _priority; }
            set { _priority = value; /*OnPropertyChanged("priority");*/ }
        }
        private int _priority = 0;

        public int scan_per_task_number
        {
            get { return _scan_per_task_number; }
            set { _scan_per_task_number = value; /*OnPropertyChanged("scan_per_task_number");*/ }
        }
        private int _scan_per_task_number = 0;

        public string sdrn_server
        {
            get { return _sdrn_server; }
            set { _sdrn_server = value; /*OnPropertyChanged("sdrn_server");*/ }
        }
        private string _sdrn_server = "";

        public string sensor_name
        {
            get { return _sensor_name; }
            set { _sensor_name = value; /*OnPropertyChanged("sensor_name");*/ }
        }
        private string _sensor_name = "";

        public string status
        {
            get { return _status; }
            set { _status = value; /*OnPropertyChanged("status");*/ }
        }
        private string _status = "";

        public DateTime time_start
        {
            get { return _time_start; }
            set { _time_start = value; OnPropertyChanged("time_start"); }
        }
        private DateTime _time_start = DateTime.MinValue;

        public DateTime time_stop
        {
            get { return _time_stop; }
            set { _time_stop = value; OnPropertyChanged("time_stop"); }
        }
        private DateTime _time_stop = DateTime.MinValue;

        public DateTime time_save
        {
            get { return _time_save; }
            set { _time_save = value; /*OnPropertyChanged("time_save");*/ }
        }
        private DateTime _time_save = DateTime.MinValue;

        public DateTime time_last_send_result
        {
            get { return _time_last_send_result; }
            set { _time_last_send_result = value; OnPropertyChanged("time_last_send_result"); }
        }
        private DateTime _time_last_send_result = DateTime.MinValue;

        
        public int with_results_measured
        {
            get { return _with_results_measured; }
            set { _with_results_measured = value; OnPropertyChanged("with_results_measured"); }
        }
        private int _with_results_measured = 0;

        public int without_results_measured
        {
            get { return _without_results_measured; }
            set { _without_results_measured = value; OnPropertyChanged("without_results_measured"); }
        }
        private int _without_results_measured = 0;

        public int with_ppe_results_measured
        {
            get { return _with_ppe_results_measured; }
            set { _with_ppe_results_measured = value; OnPropertyChanged("with_ppe_results_measured"); }
        }
        private int _with_ppe_results_measured = 0;

        public int with_ndp_results_measured
        {
            get { return _with_ndp_results_measured; }
            set { _with_ndp_results_measured = value; OnPropertyChanged("with_ndp_results_measured"); }
        }
        private int _with_ndp_results_measured = 0;

        public int task_station_count
        {
            get { return _task_station_count; }
            set { _task_station_count = value; OnPropertyChanged("task_station_count"); }
        }
        private int _task_station_count = 0;

        /// <summary>
        /// 0 = ожидание, 1 = выкл, 2 = вкл
        /// </summary>
        public int task_state
        {
            get { return _task_state; }
            set { _task_state = value; OnPropertyChanged("task_state"); }
        }
        private int _task_state = 0;

        public int new_result_to_send
        {
            get { return _new_result_to_send; }
            set { _new_result_to_send = value; OnPropertyChanged("new_result_to_send"); }
        }
        private int _new_result_to_send = 0;

        public ObservableCollection<localatdi_task_with_tech> data_from_tech
        {
            get { return _data_from_tech; }
            set { _data_from_tech = value; OnPropertyChanged("data_from_tech"); }
        }
        private ObservableCollection<localatdi_task_with_tech> _data_from_tech = new ObservableCollection<localatdi_task_with_tech>() { };

        public string routes_tb_name
        {
            get { return _routes_tb_name; }
            set { _routes_tb_name = value; /*OnPropertyChanged("routes_tb_name");*/ }
        }
        private string _routes_tb_name = "";

        public int routes_id
        {
            get { return _routes_id; }
            set { _routes_id = value; /*OnPropertyChanged("routes_tb_name");*/ }
        }
        private int _routes_id = -1;

        /// <summary>
        /// отдельная таблица для каждого таска
        /// </summary>
        public ObservableCollection<localatdi_route_point> routes
        {
            get { return _routes; }
            set { _routes = value; /*OnPropertyChanged("routes");*/ }
        }
        private ObservableCollection<localatdi_route_point> _routes = new ObservableCollection<localatdi_route_point>() { };

        [NpgsqlTypes.PgName("results_info")]
        public ObservableCollection<localatdi_result_state_data> ResultsInfo
        {
            get { return _ResultsInfo; }
            set { _ResultsInfo = value; OnPropertyChanged("ResultsInfo"); }
        }
        private ObservableCollection<localatdi_result_state_data> _ResultsInfo = new ObservableCollection<localatdi_result_state_data>() { };
    }
}
