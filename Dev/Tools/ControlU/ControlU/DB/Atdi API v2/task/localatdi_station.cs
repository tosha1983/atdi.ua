using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_station : PropertyChangedBase
    {
        /// <summary>
        /// station_id
        /// </summary>
        public string id
        {
            get { return _id; }
            set { _id = value; /*OnPropertyChanged("id");*/ }
        }
        private string _id = "";

        /// <summary>
        /// GlobalSid
        /// </summary>
        public string callsign_db
        {
            get { return _callsign_db; }
            set { _callsign_db = value; OnPropertyChanged("callsign_db"); }
        }
        private string _callsign_db = "";

        #region
        /// <summary>
        /// MCC
        /// </summary>
        public int Callsign_db_S0
        {
            get { return _Callsign_db_S0; }
            set { _Callsign_db_S0 = value; }
        }
        private int _Callsign_db_S0 = 0;
        /// <summary>
        /// MNC
        /// </summary>
        public int Callsign_db_S1
        {
            get { return _Callsign_db_S1; }
            set { _Callsign_db_S1 = value; }
        }
        private int _Callsign_db_S1 = 0;
        /// <summary>
        /// LAC / eNodeBid
        /// </summary>
        public int Callsign_db_S2
        {
            get { return _Callsign_db_S2; }
            set { _Callsign_db_S2 = value; }
        }
        private int _Callsign_db_S2 = 0;
        /// <summary>
        /// CID / PN
        /// </summary>
        public int Callsign_db_S3
        {
            get { return _Callsign_db_S3; }
            set { _Callsign_db_S3 = value; }
        }
        private int _Callsign_db_S3 = 0;
        #endregion

        /// <summary>
        /// global_cid_radio
        /// </summary>
        public string callsign_radio
        {
            get { return _callsign_radio; }
            set { _callsign_radio = value; OnPropertyChanged("callsign_radio"); }
        }
        private string _callsign_radio = "";

        #region
        /// <summary>
        /// MCC
        /// </summary>
        public int Callsign_radio_S0
        {
            get { return _Callsign_radio_S0; }
            set { _Callsign_radio_S0 = value; }
        }
        private int _Callsign_radio_S0 = 0;
        /// <summary>
        /// MNC
        /// </summary>
        public int Callsign_radio_S1
        {
            get { return _Callsign_radio_S1; }
            set { _Callsign_radio_S1 = value; }
        }
        private int _Callsign_radio_S1 = 0;
        /// <summary>
        /// LAC / eNodeBid
        /// </summary>
        public int Callsign_radio_S2
        {
            get { return _Callsign_radio_S2; }
            set { _Callsign_radio_S2 = value; }
        }
        private int _Callsign_radio_S2 = 0;
        /// <summary>
        /// CID / PN
        /// </summary>
        public int Callsign_radio_S3
        {
            get { return _Callsign_radio_S3; }
            set { _Callsign_radio_S3 = value; }
        }
        private int _Callsign_radio_S3 = 0;
        #endregion

        public string standard
        {
            get { return _standard; }
            set { _standard = value; OnPropertyChanged("standard"); }
        }
        private string _standard = "";

        public string status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("status"); }
        }
        private string _status = "";

        public localatdi_license_info license
        {
            get { return _license; }
            set { _license = value; OnPropertyChanged("license"); }
        }
        private localatdi_license_info _license = new localatdi_license_info() { };

        public localatdi_station_owner owner
        {
            get { return _owner; }
            set { _owner = value; OnPropertyChanged("owner"); }
        }
        private localatdi_station_owner _owner = new localatdi_station_owner() { };

        public localatdi_station_site site
        {
            get { return _site; }
            set { _site = value; OnPropertyChanged("site"); }
        }
        private localatdi_station_site _site = new localatdi_station_site() { };

        public ObservableCollection<localatdi_station_sector> sectors
        {
            get { return _sectors; }
            set { _sectors = value; OnPropertyChanged("sectors"); }
        }
        private ObservableCollection<localatdi_station_sector> _sectors = new ObservableCollection<localatdi_station_sector>() { };

        public bool meas_data_exist
        {
            get { return _meas_data_exist; }
            set { _meas_data_exist = value; OnPropertyChanged("meas_data_exist"); }
        }
        private bool _meas_data_exist = false;

        public bool IsIdentified
        {
            get { return _IsIdentified; }
            set { _IsIdentified = value; OnPropertyChanged("IsIdentified"); }
        }
        private bool _IsIdentified = false;
    }
}
