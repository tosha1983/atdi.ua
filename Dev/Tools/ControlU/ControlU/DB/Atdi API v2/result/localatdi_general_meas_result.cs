using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_general_meas_result : PropertyChangedBase
    {
        /// <summary>
        /// CentralFrequency_MHz
        /// </summary>
        public decimal central_frequency
        {
            get { return _central_frequency; }
            set { _central_frequency = value; /*OnPropertyChanged("central_frequency");*/ }
        }
        private decimal _central_frequency = 0;

        /// <summary>
        /// CentralFrequencyMeas_MHz
        /// </summary>
        public decimal central_frequency_meas
        {
            get { return _central_frequency_meas; }
            set { _central_frequency_meas = value; /*OnPropertyChanged("central_frequency_meas");*/ }
        }
        private decimal _central_frequency_meas = 0;

        /// <summary>
        /// OffsetFrequency_mk в 10е-6
        /// </summary>
        public decimal offset_frequency_mk
        {
            get { return _offset_frequency_mk; }
            set { _offset_frequency_mk = value; /*OnPropertyChanged("offset_frequency_mk");*/ }
        }
        private decimal _offset_frequency_mk = 0;

        /// <summary>
        /// LevelsSpectrum_dBm
        /// </summary>
        public Equipment.tracepoint[] trace_dbm
        {
            get { return _trace_dbm; }
            set { _trace_dbm = value; /*OnPropertyChanged("trace_dbm");*/ }
        }
        private Equipment.tracepoint[] _trace_dbm = new Equipment.tracepoint[] { };

        public decimal trace_freq_centr
        {
            get { return _trace_freq_centr; }
            set { _trace_freq_centr = value; /*OnPropertyChanged("trace_freq_centr");*/ }
        }
        private decimal _trace_freq_centr = 0;

        /// <summary>
        /// SpectrumStartFreq_MHz
        /// </summary>
        public decimal trace_freq_start
        {
            get { return _trace_freq_start; }
            set { _trace_freq_start = value; /*OnPropertyChanged("trace_freq_start");*/ }
        }
        private decimal _trace_freq_start = 0;

        public decimal trace_freq_stop
        {
            get { return _trace_freq_stop; }
            set { _trace_freq_stop = value; /*OnPropertyChanged("trace_freq_stop");*/ }
        }
        private decimal _trace_freq_stop = 0;

        /// <summary>
        /// SpectrumSteps_kHz
        /// </summary>
        public decimal trace_freq_step
        {
            get { return _trace_freq_step; }
            set { _trace_freq_step = value; /*OnPropertyChanged("trace_freq_step");*/ }
        }
        private decimal _trace_freq_step = 0;

        /// <summary>
        /// MeasDuration_sec
        /// </summary>
        public decimal meas_duration
        {
            get { return _meas_duration; }
            set { _meas_duration = value; /*OnPropertyChanged("meas_duration");*/ }
        }
        private decimal _meas_duration = 0;

        public DateTime meas_start_time
        {
            get { return _meas_start_time; }
            set { _meas_start_time = value; /*OnPropertyChanged("meas_start_time");*/ }
        }
        private DateTime _meas_start_time = DateTime.MinValue;

        public DateTime meas_finish_time
        {
            get { return _meas_finish_time; }
            set { _meas_finish_time = value; /*OnPropertyChanged("meas_finish_time");*/ }
        }
        private DateTime _meas_finish_time = DateTime.MinValue;

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

        public localatdi_bandwidth_meas_result bandwidth_result
        {
            get { return _bandwidth_result; }
            set { _bandwidth_result = value; /*OnPropertyChanged("bandwidth_result");*/ }
        }
        private localatdi_bandwidth_meas_result _bandwidth_result = new localatdi_bandwidth_meas_result() { };

        public localatdi_elements_mask bw_mask
        {
            get { return _bw_mask; }
            set { _bw_mask = value; /*OnPropertyChanged("bw_mask");*/ }
        }
        private localatdi_elements_mask _bw_mask = new localatdi_elements_mask() { };

        public localatdi_station_sys_info station_sys_info
        {
            get { return _station_sys_info; }
            set { _station_sys_info = value; /*OnPropertyChanged("station_sys_info");*/ }
        }
        private localatdi_station_sys_info _station_sys_info = new localatdi_station_sys_info() { };

    }
}
