using System.Collections.ObjectModel;

namespace ControlU.DB
{
    public class localatdi_task_with_tech : PropertyChangedBase
    {
        public string tech
        {
            get { return _tech; }
            set { _tech = value; OnPropertyChanged("tech"); }
        }
        private string _tech = string.Empty;

        public string task_table_name
        {
            get { return _task_table_name; }
            set { _task_table_name = value; OnPropertyChanged("task_table_name"); }
        }
        private string _task_table_name = string.Empty;

        public string result_table_name
        {
            get { return _result_table_name; }
            set { _result_table_name = value; OnPropertyChanged("result_table_name"); }
        }
        private string _result_table_name = string.Empty;

        public localatdi_standard_scan_parameter[] scan_parameters
        {
            get { return _scan_parameters; }
            set { _scan_parameters = value; /*OnPropertyChanged("scan_parameters");*/ }
        }
        private localatdi_standard_scan_parameter[] _scan_parameters = new localatdi_standard_scan_parameter[] { };

        public ObservableCollection<localatdi_station> TaskItems
        {
            get { return _TaskItems; }
            set { _TaskItems = value; OnPropertyChanged("TaskItems"); }
        }
        private ObservableCollection<localatdi_station> _TaskItems = new ObservableCollection<localatdi_station>() { };

        public ObservableCollection<localatdi_result_item> ResultItems
        {
            get { return _ResultItems; }
            set { _ResultItems = value; OnPropertyChanged("ResultItems"); }
        }
        private ObservableCollection<localatdi_result_item> _ResultItems = new ObservableCollection<localatdi_result_item>() { };


        public int ResultTask_WithMeasurement
        {
            get { return _ResultTask_WithMeasurement; }
            set { _ResultTask_WithMeasurement = value; OnPropertyChanged("ResultTask_WithMeasurement"); }
        }
        private int _ResultTask_WithMeasurement = 0;

        public int ResultTask_PPEWithMeasurement
        {
            get { return _ResultTask_PPEWithMeasurement; }
            set { _ResultTask_PPEWithMeasurement = value; OnPropertyChanged("ResultTask_PPEWithMeasurement"); }
        }
        private int _ResultTask_PPEWithMeasurement = 0;

        public int ResultTask_NDPWithMeasurement
        {
            get { return _ResultTask_NDPWithMeasurement; }
            set { _ResultTask_NDPWithMeasurement = value; OnPropertyChanged("ResultTask_NDPWithMeasurement"); }
        }
        private int _ResultTask_NDPWithMeasurement = 0;

        public int ResultTask_ToSend
        {
            get { return _ResultTask_ToSend; }
            set { _ResultTask_ToSend = value; OnPropertyChanged("ResultTask_ToSend"); }
        }
        private int _ResultTask_ToSend = 0;

        public int SectorsCount
        {
            get { return _SectorsCount; }
            set { _SectorsCount = value; OnPropertyChanged("SectorsCount"); }
        }
        private int _SectorsCount = 0;        
    }
}
