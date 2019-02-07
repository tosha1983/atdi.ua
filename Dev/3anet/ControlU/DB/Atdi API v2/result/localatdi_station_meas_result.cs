using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_station_meas_result : PropertyChangedBase
    {
        /// <summary>
        /// TaskGlobalSid
        /// </summary>
        public string task_gcid
        {
            get { return _task_gcid; }
            set { _task_gcid = value; /*OnPropertyChanged("task_gcid");*/ }
        }
        private string _task_gcid = "";

        /// <summary>
        /// с эфира RealGlobalSid
        /// </summary>
        public string real_gcid
        {
            get { return _real_gcid; }
            set { _real_gcid = value; /*OnPropertyChanged("real_gcid");*/ }
        }
        private string _real_gcid = "";

        /// <summary>
        /// GSM/CDMA/UMTS/LTE
        /// </summary>
        public string standard
        {
            get { return _standard; }
            set { _standard = value; /*OnPropertyChanged("standard");*/ }
        }
        private string _standard = "";

        public string station_id
        {
            get { return _station_id; }
            set { _station_id = value; /*OnPropertyChanged("station_id");*/ }
        }
        private string _station_id = "";

        public string sector_id
        {
            get { return _sector_id; }
            set { _sector_id = value; /*OnPropertyChanged("sector_id");*/ }
        }
        private string _sector_id = "";

        public string status
        {
            get { return _status; }
            set { _status = value; /*OnPropertyChanged("status");*/ }
        }
        private string _status = "";

        public localatdi_general_meas_result general_result
        {
            get { return _general_result; }
            set { _general_result = value; /*OnPropertyChanged("general_result");*/ }
        }
        private localatdi_general_meas_result _general_result = new localatdi_general_meas_result() { };

        public localatdi_level_meas_result[] level_results
        {
            get { return _level_results; }
            set { _level_results = value; /*OnPropertyChanged("level_results");*/ }
        }
        private localatdi_level_meas_result[] _level_results = new localatdi_level_meas_result[] { };
    }
}
