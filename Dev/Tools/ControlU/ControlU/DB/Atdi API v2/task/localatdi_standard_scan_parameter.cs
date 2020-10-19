using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_standard_scan_parameter : PropertyChangedBase
    {
        //public string id
        //{
        //    get { return _id; }
        //    set { _id = value; /*OnPropertyChanged("id");*/ }
        //}
        //private string _id = "";

        public double detection_level_dbm
        {
            get { return _detection_level_dbm; }
            set { _detection_level_dbm = value; /*OnPropertyChanged("detection_level_dbm");*/ }
        }
        private double _detection_level_dbm = -10000;

        public decimal max_frequency_relative_offset_mk
        {
            get { return _max_frequency_relative_offset_mk; }
            set { _max_frequency_relative_offset_mk = value; /*OnPropertyChanged("max_frequency_relative_offset_mk");*/ }
        }
        private decimal _max_frequency_relative_offset_mk = 0;

        /// <summary>
        /// MaxPermissionBW_kHz
        /// </summary>
        public decimal max_permission_bw
        {
            get { return _max_permission_bw; }
            set { _max_permission_bw = value; /*OnPropertyChanged("max_permission_bw");*/ }
        }
        private decimal _max_permission_bw = 0;

        public string standard
        {
            get { return _standard; }
            set { _standard = value; /*OnPropertyChanged("standard");*/ }
        }
        private string _standard = "";

        public decimal xdb_level_db
        {
            get { return _xdb_level_db; }
            set { _xdb_level_db = value; /*OnPropertyChanged("xdb_level_db");*/ }
        }
        private decimal _xdb_level_db = 0;

        #region device_param
        /// <summary>
        /// enum Atdi.DataModels.Sdrns.DetectingType
        /// </summary>
        public int detector_type
        {
            get { return _detector_type; }
            set { _detector_type = value; /*OnPropertyChanged("detector_type");*/ }
        }
        private int _detector_type = 0;

        public decimal meas_time_sec
        {
            get { return _meas_time_sec; }
            set { _meas_time_sec = value; /*OnPropertyChanged("meas_time_sec");*/ }
        }
        private decimal _meas_time_sec = 0;

        public int preamplification_db
        {
            get { return _preamplification_db; }
            set { _preamplification_db = value; /*OnPropertyChanged("preamplification_db");*/ }
        }
        private int _preamplification_db = 0;

        /// <summary>
        /// RBW_kHz
        /// </summary>
        public decimal rbw
        {
            get { return _rbw; }
            set { _rbw = value; /*OnPropertyChanged("rbw");*/ }
        }
        private decimal _rbw = 0;

        /// <summary>
        /// VBW_kHz
        /// </summary>
        public decimal vbw
        {
            get { return _vbw; }
            set { _vbw = value; /*OnPropertyChanged("vbw");*/ }
        }
        private decimal _vbw = 0;

        public decimal ref_level_dbm
        {
            get { return _ref_level_dbm; }
            set { _ref_level_dbm = value; /*OnPropertyChanged("ref_level_dbm");*/ }
        }
        private decimal _ref_level_dbm = 0;

        public decimal rf_attenuation_db
        {
            get { return _rf_attenuation_db; }
            set { _rf_attenuation_db = value; /*OnPropertyChanged("rf_attenuation_db");*/ }
        }
        private decimal _rf_attenuation_db = 0;

        /// <summary>
        /// ScanBW_kHz
        /// </summary>
        public decimal meas_span
        {
            get { return _meas_span; }
            set { _meas_span = value; /*OnPropertyChanged("meas_span");*/ }
        }
        private decimal _meas_span = 0;
        #endregion

        //public localatdi_device_meas_param device_param
        //{
        //    get { return _device_param; }
        //    set { _device_param = value; /*OnPropertyChanged("device_param");*/ }
        //}
        //private localatdi_device_meas_param _device_param = new localatdi_device_meas_param() { };
    }
}
