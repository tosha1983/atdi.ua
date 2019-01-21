using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_station_sys_info : PropertyChangedBase
    {
        /// <summary>
        /// BaseID cdma/evdo
        /// </summary>
        public int base_id
        {
            get { return _base_id; }
            set { _base_id = value; /*OnPropertyChanged("base_id");*/ }
        }
        private int _base_id = -1;

        /// <summary>
        /// gsm
        /// </summary>
        public int bsic
        {
            get { return _bsic; }
            set { _bsic = value; /*OnPropertyChanged("bsic");*/ }
        }
        private int _bsic = -1;

        public int channel_number
        {
            get { return _channel_number; }
            set { _channel_number = value; /*OnPropertyChanged("channel_number");*/ }
        }
        private int _channel_number = -1;

        /// <summary>
        /// gsm... посмотреть
        /// </summary>
        public int cid
        {
            get { return _cid; }
            set { _cid = value; /*OnPropertyChanged("cid");*/ }
        }
        private int _cid = -1;

        public int eci
        {
            get { return _eci; }
            set { _eci = value; /*OnPropertyChanged("eci");*/ }
        }
        private int _eci = -1;

        public int e_node_b_id
        {
            get { return _e_node_b_id; }
            set { _e_node_b_id = value; /*OnPropertyChanged("e_node_b_id");*/ }
        }
        private int _e_node_b_id = -1;

        public int lac
        {
            get { return _lac; }
            set { _lac = value; /*OnPropertyChanged("lac");*/ }
        }
        private int _lac = -1;

        public int mcc
        {
            get { return _mcc; }
            set { _mcc = value; /*OnPropertyChanged("mcc");*/ }
        }
        private int _mcc = -1;

        public int mnc
        {
            get { return _mnc; }
            set { _mnc = value; /*OnPropertyChanged("mnc");*/ }
        }
        private int _mnc = -1;

        public int nid
        {
            get { return _nid; }
            set { _nid = value; /*OnPropertyChanged("nid");*/ }
        }
        private int _nid = -1;

        public int pci
        {
            get { return _pci; }
            set { _pci = value; /*OnPropertyChanged("pci");*/ }
        }
        private int _pci = -1;

        public int pn
        {
            get { return _pn; }
            set { _pn = value; /*OnPropertyChanged("pn");*/ }
        }
        private int _pn = -1;

        public int rnc
        {
            get { return _rnc; }
            set { _rnc = value; /*OnPropertyChanged("rnc");*/ }
        }
        private int _rnc = -1;

        public int sc
        {
            get { return _sc; }
            set { _sc = value; /*OnPropertyChanged("sc");*/ }
        }
        private int _sc = -1;

        public int sid
        {
            get { return _sid; }
            set { _sid = value; /*OnPropertyChanged("sid");*/ }
        }
        private int _sid = -1;

        public int tac
        {
            get { return _tac; }
            set { _tac = value; /*OnPropertyChanged("tac");*/ }
        }
        private int _tac = -1;

        public int ucid
        {
            get { return _ucid; }
            set { _ucid = value; /*OnPropertyChanged("ucid");*/ }
        }
        private int _ucid = -1;




        /// <summary>
        /// Bandwidth_kHz
        /// </summary>
        public decimal bandwidth
        {
            get { return _bandwidth; }
            set { _bandwidth = value; /*OnPropertyChanged("bandwidth");*/ }
        }
        private decimal _bandwidth = -1;

        /// <summary>
        /// Code
        /// </summary>
        public double code_power
        {
            get { return _code_power; }
            set { _code_power = value; /*OnPropertyChanged("code_power");*/ }
        }
        private double _code_power = -1000;

        /// <summary>
        /// CtoI C/I GSM
        /// </summary>
        public double ctoi
        {
            get { return _ctoi; }
            set { _ctoi = value; /*OnPropertyChanged("ctoi");*/ }
        }
        private double _ctoi = -1000;

        public decimal freq
        {
            get { return _freq; }
            set { _freq = value; /*OnPropertyChanged("freq");*/ }
        }
        private decimal _freq = -1;

        public double icio
        {
            get { return _icio; }
            set { _icio = value; /*OnPropertyChanged("icio");*/ }
        }
        private double _icio = -1000;

        public double inband_power
        {
            get { return _inband_power; }
            set { _inband_power = value; /*OnPropertyChanged("inband_power");*/ }
        }
        private double _inband_power = -1000;

        public double iscp
        {
            get { return _iscp; }
            set { _iscp = value; /*OnPropertyChanged("iscp");*/ }
        }
        private double _iscp = -1000;

        public double power
        {
            get { return _power; }
            set { _power = value; /*OnPropertyChanged("power");*/ }
        }
        private double _power = -1000;

        public double ptotal
        {
            get { return _ptotal; }
            set { _ptotal = value; /*OnPropertyChanged("ptotal");*/ }
        }
        private double _ptotal = -1000;

        public double rscp
        {
            get { return _rscp; }
            set { _rscp = value; /*OnPropertyChanged("rscp");*/ }
        }
        private double _rscp = -1000;

        public double rsrp
        {
            get { return _rsrp; }
            set { _rsrp = value; /*OnPropertyChanged("rsrp");*/ }
        }
        private double _rsrp = 0;

        public double rsrq
        {
            get { return _rsrq; }
            set { _rsrq = value; /*OnPropertyChanged("rsrq");*/ }
        }
        private double _rsrq = 0;


        public string type_cdmaevdo
        {
            get { return _type_cdmaevdo; }
            set { _type_cdmaevdo = value; /*OnPropertyChanged("type_cdmaevdo");*/ }
        }
        private string _type_cdmaevdo = "";

        public localatdi_geo_location location
        {
            get { return _location; }
            set { _location = value; /*OnPropertyChanged("location");*/ }
        }
        private localatdi_geo_location _location = new localatdi_geo_location() { };

        public local3GPPSystemInformationBlock[] information_blocks
        {
            get { return _information_blocks; }
            set { _information_blocks = value; OnPropertyChanged("information_blocks"); }
        }
        private local3GPPSystemInformationBlock[] _information_blocks = new local3GPPSystemInformationBlock[] { };

        //public ObservableCollection<local3GPPSystemInformationBlock> information_blocks
        //{
        //    get { return _information_blocks; }
        //    set { _information_blocks = value; OnPropertyChanged("information_blocks"); }
        //}
        //private ObservableCollection<local3GPPSystemInformationBlock> _information_blocks = new ObservableCollection<local3GPPSystemInformationBlock>() { };
    }
}
