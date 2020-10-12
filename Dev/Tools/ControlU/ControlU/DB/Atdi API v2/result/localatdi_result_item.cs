using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    /// <summary>
    /// to Atdi.DataModels.Sdrns.Device.StationMeasResult
    /// </summary>
    public class localatdi_result_item : PropertyChangedBase
    {
        /// <summary>
        /// non
        /// </summary>
        public int id_permission
        {
            get { return _id_permission; }
            set { _id_permission = value; OnPropertyChanged("id_permission"); }
        }
        private int _id_permission = -1;

        /// <summary>
        /// StationId
        /// </summary>
        public string id_station
        {
            get { return _id_station; }
            set { _id_station = value; OnPropertyChanged("id_station"); }
        }
        private string _id_station = "";

        /// <summary>
        /// SectorId
        /// </summary>
        public string id_sector
        {
            get { return _id_sector; }
            set { _id_sector = value; OnPropertyChanged("id_sector"); }
        }
        private string _id_sector = "";

        /// <summary>
        /// non
        /// </summary>
        public int id_frequency
        {
            get { return _id_frequency; }
            set { _id_frequency = value; OnPropertyChanged("id_frequency"); }
        }
        private int _id_frequency = 0;

        /// <summary>
        /// non
        /// </summary>
        public string id_task
        {
            get { return _id_task; }
            set { _id_task = value; OnPropertyChanged("id_task"); }
        }
        private string _id_task = "";

        /// <summary>
        /// GeneralResult.CentralFrequency_MHz если нет в разрешении то свою
        /// </summary>
        public decimal freq_centr_perm
        {
            get { return _freq_centr_perm; }
            set { _freq_centr_perm = value; OnPropertyChanged("freq_centr_perm"); }
        }
        private decimal _freq_centr_perm = -1;
      

        /// <summary>
        /// в полосе сигнала
        /// </summary>
        public decimal meas_strength
        {
            get { return _meas_strength; }
            set { _meas_strength = value; OnPropertyChanged("meas_strength"); }
        }
        private decimal _meas_strength = 0; 

        /// <summary>
        /// GeneralResult.BWMask
        /// </summary>
        public localatdi_elements_mask[] meas_mask
        {
            get { return _meas_mask; }
            set { _meas_mask = value; OnPropertyChanged("meas_mask"); }
        }
        private localatdi_elements_mask[] _meas_mask = new localatdi_elements_mask[] { };

        /// <summary>
        /// 0 = не меряли(нет маски) 1 = плохо 2 =хорошо 
        /// </summary>
        public int mask_result
        {
            get { return _mask_result; }
            set { _mask_result = value; OnPropertyChanged("mask_result"); }
        }
        private int _mask_result = 0;

        /// <summary>
        /// GeneralResult.BandwidthResult.СorrectnessEstimations
        /// </summary>
        public bool meas_correctness
        {
            get { return _meas_correctness; }
            set { _meas_correctness = value; OnPropertyChanged("meas_correctness"); }
        }
        private bool _meas_correctness = false;

        public Equipment.spectrum_data spec_data
        {
            get { return _spec_data; }
            set { _spec_data = value; OnPropertyChanged("spec_data"); }
        }
        private Equipment.spectrum_data _spec_data = new Equipment.spectrum_data() { };

        public Equipment.bandwidth_data bw_data
        {
            get { return _bw_data; }
            set { _bw_data = value; OnPropertyChanged("bw_data"); }
        }
        private Equipment.bandwidth_data _bw_data = new Equipment.bandwidth_data() { };

        public Equipment.channelpower_data[] cp_data
        {
            get { return _cp_data; }
            set { _cp_data = value; OnPropertyChanged("cp_data"); }
        }
        private Equipment.channelpower_data[] _cp_data = new Equipment.channelpower_data[] { };
      
        /// <summary>
        /// RealGlobalSid
        /// </summary>
        public string station_identifier_from_radio
        {
            get { return _station_identifier_from_radio; }
            set { _station_identifier_from_radio = value; OnPropertyChanged("station_identifier_from_radio"); }
        }
        private string _station_identifier_from_radio = string.Empty;
        #region station_identifier_from_radio
        public int station_identifier_from_radio_s0
        {
            get { return _station_identifier_from_radio_s0; }
            set { _station_identifier_from_radio_s0 = value; }
        }
        private int _station_identifier_from_radio_s0 = 0;
        public int station_identifier_from_radio_s1
        {
            get { return _station_identifier_from_radio_s1; }
            set { _station_identifier_from_radio_s1 = value; }
        }
        private int _station_identifier_from_radio_s1 = 0;
        public int station_identifier_from_radio_s2
        {
            get { return _station_identifier_from_radio_s2; }
            set { _station_identifier_from_radio_s2 = value; }
        }
        private int _station_identifier_from_radio_s2 = 0;
        public int station_identifier_from_radio_s3
        {
            get { return _station_identifier_from_radio_s3; }
            set { _station_identifier_from_radio_s3 = value; }
        }
        private int _station_identifier_from_radio_s3 = 0;
        #endregion

        public int station_identifier_from_radio_tech_sub_ind
        {
            get { return _station_identifier_from_radio_tech_sub_ind; }
            set { _station_identifier_from_radio_tech_sub_ind = value; OnPropertyChanged("station_identifier_from_radio_tech_sub_ind"); }
        }
        private int _station_identifier_from_radio_tech_sub_ind = 0;

        /// <summary>
        /// TaskGlobalSid
        /// </summary>
        public string station_identifier_atdi
        {
            get { return _station_identifier_atdi; }
            set { _station_identifier_atdi = value; OnPropertyChanged("station_identifier_atdi"); }
        }
        private string _station_identifier_atdi = string.Empty;

        #region station_identifier_atdi
        public int station_identifier_atdi_s0
        {
            get { return _station_identifier_atdi_s0; }
            set { _station_identifier_atdi_s0 = value; }
        }
        private int _station_identifier_atdi_s0 = 0;
        public int station_identifier_atdi_s1
        {
            get { return _station_identifier_atdi_s1; }
            set { _station_identifier_atdi_s1 = value; }
        }
        private int _station_identifier_atdi_s1 = 0;
        public int station_identifier_atdi_s2
        {
            get { return _station_identifier_atdi_s2; }
            set { _station_identifier_atdi_s2 = value; }
        }
        private int _station_identifier_atdi_s2 = 0;
        public int station_identifier_atdi_s3
        {
            get { return _station_identifier_atdi_s3; }
            set { _station_identifier_atdi_s3 = value; }
        }
        private int _station_identifier_atdi_s3 = 0;
        #endregion

        /// <summary>
        /// Status = “E” в случае НДП, “A” – если это не НДП, “I” – порушенная правил эксплуатации
        /// </summary>
        public string status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("status"); }
        }
        private string _status = string.Empty;

        /// <summary>
        /// non
        /// </summary>
        public string user_id
        {
            get { return _user_id; }
            set { _user_id = value; /*OnPropertyChanged("user_id");*/ }
        }
        private string _user_id = "";

        /// <summary>
        /// non
        /// </summary>
        public string user_name
        {
            get { return _user_name; }
            set { _user_name = value; OnPropertyChanged("user_name"); }
        }
        private string _user_name = string.Empty;

        /// <summary>
        /// есть новые результаты нужно отправить
        /// </summary>
        public bool new_meas_data_to_send
        {
            get { return _new_meas_data_to_send; }
            set { _new_meas_data_to_send = value; OnPropertyChanged("new_meas_data_to_send"); }
        }
        private bool _new_meas_data_to_send = false;

        /// <summary>
        /// отправлено ли в ICSC
        /// </summary>
        public bool level_results_sended
        {
            get { return _level_results_sended; }
            set { _level_results_sended = value; OnPropertyChanged("level_results_sended"); }
        }
        private bool _level_results_sended = false;

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<localatdi_level_meas_result> level_results
        {
            get { return _level_results; }
            set { _level_results = value; OnPropertyChanged("level_results"); }
        }
        private ObservableCollection<localatdi_level_meas_result> _level_results = new ObservableCollection<localatdi_level_meas_result>() { };

        /// <summary>
        /// GeneralResult.StationSysInfo
        /// </summary>
        public localatdi_station_sys_info station_sys_info
        {
            get { return _station_sys_info; }
            set { _station_sys_info = value; OnPropertyChanged("station_sys_info"); }
        }
        private localatdi_station_sys_info _station_sys_info = new localatdi_station_sys_info() { };

        

        public localatdi_meas_device device_ident
        {
            get { return _device_ident; }
            set { _device_ident = value; OnPropertyChanged("device_ident"); }
        }
        private localatdi_meas_device _device_ident = new localatdi_meas_device() { };

        public localatdi_meas_device device_meas
        {
            get { return _device_meas; }
            set { _device_meas = value; OnPropertyChanged("device_meas"); }
        }
        private localatdi_meas_device _device_meas = new localatdi_meas_device() { };
    }
}
