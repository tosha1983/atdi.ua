using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    /// <summary>
    /// работает в рамках одного месяца во все дни
    /// </summary>
    public class localatdi_unknown_result : PropertyChangedBase
    {
        public string id
        {
            get { return _id; }
            set { _id = value; /*OnPropertyChanged("id");*/ }
        }
        private string _id = "";

        /// <summary>
        /// дата месяц год включительно
        /// </summary>
        public DateTime time_start
        {
            get { return _time_start; }
            set { _time_start = value; OnPropertyChanged("time_start"); }
        }
        private DateTime _time_start = DateTime.MinValue;

        /// <summary>
        /// дата месяц год включительно
        /// </summary>
        public DateTime time_stop
        {
            get { return _time_stop; }
            set { _time_stop = value; OnPropertyChanged("time_stop"); }
        }
        private DateTime _time_stop = DateTime.MinValue;

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
        /// хранится в отдельной таблице
        /// </summary>
        public ObservableCollection<localatdi_route_point> routes
        {
            get { return _routes; }
            set { _routes = value; /*OnPropertyChanged("routes");*/ }
        }
        private ObservableCollection<localatdi_route_point> _routes = new ObservableCollection<localatdi_route_point>() { };

        public int new_result_to_send
        {
            get { return _new_result_to_send; }
            set { _new_result_to_send = value; OnPropertyChanged("new_result_to_send"); }
        }
        private int _new_result_to_send = 0;

        public ObservableCollection<localatdi_unknown_result_with_tech> data_from_tech
        {
            get { return _data_from_tech; }
            set { _data_from_tech = value; OnPropertyChanged("data_from_tech"); }
        }
        private ObservableCollection<localatdi_unknown_result_with_tech> _data_from_tech = new ObservableCollection<localatdi_unknown_result_with_tech>() { };

        public DateTime time_last_send_result
        {
            get { return _time_last_send_result; }
            set { _time_last_send_result = value; OnPropertyChanged("time_last_send_result"); }
        }
        private DateTime _time_last_send_result = DateTime.MinValue;

        public int ResultsCount
        {
            get { return _ResultsCount; }
            set { _ResultsCount = value; OnPropertyChanged("ResultsCount"); }
        }
        private int _ResultsCount = 0;

        [NpgsqlTypes.PgName("results_info")]
        public ObservableCollection<localatdi_result_state_data> ResultsInfo
        {
            get { return _ResultsInfo; }
            set { _ResultsInfo = value; OnPropertyChanged("ResultsInfo"); }
        }
        private ObservableCollection<localatdi_result_state_data> _ResultsInfo = new ObservableCollection<localatdi_result_state_data>() { };
    }
    public class localatdi_unknown_result_with_tech : PropertyChangedBase
    {
        public string tech
        {
            get { return _tech; }
            set { _tech = value; OnPropertyChanged("tech"); }
        }
        private string _tech = string.Empty;

        public string result_table_name
        {
            get { return _result_table_name; }
            set { _result_table_name = value; OnPropertyChanged("result_table_name"); }
        }
        private string _result_table_name = string.Empty;

        /// <summary>
        /// хранится в отдельной таблице
        /// </summary>
        public ObservableCollection<localatdi_result_item> ResultItems
        {
            get { return _ResultItems; }
            set { _ResultItems = value; OnPropertyChanged("ResultItems"); }
        }
        private ObservableCollection<localatdi_result_item> _ResultItems = new ObservableCollection<localatdi_result_item>() { };

        public int ResultTask_ToSend
        {
            get { return _ResultTask_ToSend; }
            set { _ResultTask_ToSend = value; OnPropertyChanged("ResultTask_ToSend"); }
        }
        private int _ResultTask_ToSend = 0;
        
        
    }

    
}
