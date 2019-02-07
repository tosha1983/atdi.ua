using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ControlU.Equipment
{
    public class IdentificationData : INotifyPropertyChanged
    {
        public static GSMAllData GSM = new GSMAllData() { };

        public static UMTSAllData UMTS = new UMTSAllData() { };

        public static LTEAllData LTE = new LTEAllData() { };

        public static CDMAAllData CDMA = new CDMAAllData() { };

        public static ACDAllData ACD = new ACDAllData() { };




        #region UHF
        public bool UHFTechIsEnabled
        {
            get { return _UHFTechIsEnabled; }
            set { _UHFTechIsEnabled = value; OnPropertyChanged("UHFTechIsEnabled"); }
        }
        private bool _UHFTechIsEnabled = false;

        public ObservableCollection<UHFBTSData> _UHFBTS = new ObservableCollection<UHFBTSData>() { };
        public ObservableCollection<UHFBTSData> UHFBTS
        {
            get { return _UHFBTS; }
            set { _UHFBTS = value; OnPropertyChanged("UHFBTS"); }
        }

        public int UHFBTSCount
        {
            get { return _UHFBTSCount; }
            set { _UHFBTSCount = value; OnPropertyChanged("UHFBTSCount"); }
        }
        private int _UHFBTSCount = 0;

        public int UHFBTSCountWithGCID
        {
            get { return _UHFBTSCountWithGCID; }
            set { _UHFBTSCountWithGCID = value; OnPropertyChanged("UHFBTSCountWithGCID"); }
        }
        private int _UHFBTSCountWithGCID = 0;
        /// <summary>
        /// Количевство НДП (которые не идентифицировались)
        /// </summary>
        public int UHFBTSCountNDP
        {
            get { return _UHFBTSCountNDP; }
            set { _UHFBTSCountNDP = value; OnPropertyChanged("UHFBTSCountNDP"); }
        }
        private int _UHFBTSCountNDP = 0;

        /// <summary>
        /// Количевство ППЕ (которые идентифицировались но не совпали по частотам)
        /// </summary>
        public int UHFBTSCountNPE
        {
            get { return _UHFBTSCountNPE; }
            set { _UHFBTSCountNPE = value; OnPropertyChanged("UHFBTSCountNPE"); }
        }
        private int _UHFBTSCountNPE = 0;
        #endregion





        public IdentificationData()
        {
            //GSMBTS = new ObservableCollection<GSMBTSData>() { };
            //UMTSBTS = new ObservableCollection<UMTSBTSData>() { };
        }

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class GSMAllData : INotifyPropertyChanged
    {
        public bool TechIsEnabled
        {
            get { return App.Sett.TSMxReceiver_Settings.GSM.TechIsEnabled; }
            set { App.Sett.TSMxReceiver_Settings.GSM.TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; OnPropertyChanged("IsRuning"); }
        }
        private static bool _IsRuning;

        public ObservableCollection<GSMBTSData> _BTS = new ObservableCollection<GSMBTSData>() { };
        public ObservableCollection<GSMBTSData> BTS
        {
            get { return _BTS; }
            set { _BTS = value; OnPropertyChanged("BTS"); }
        }

        /// <summary>
        /// всего БС увиденых
        /// </summary>
        public int BTSCount
        {
            get { return _BTSCount; }
            set { _BTSCount = value; OnPropertyChanged("BTSCount"); }
        }
        private int _BTSCount = 0;

        /// <summary>
        /// Всего БС с GCID
        /// </summary>
        public int BTSCountWithGCID
        {
            get { return _BTSCountWithGCID; }
            set { _BTSCountWithGCID = value; OnPropertyChanged("BTSCountWithGCID"); }
        }
        private int _BTSCountWithGCID = 0;

        
        #region ATDI
        /// <summary>
        /// Всего БС по базе с GCID
        /// </summary>
        public int ATDI_BTSCountWithGCID
        {
            get { return _ATDI_BTSCountWithGCID; }
            set { _ATDI_BTSCountWithGCID = value; OnPropertyChanged("ATDI_BTSCountWithGCID"); }
        }
        private int _ATDI_BTSCountWithGCID = 0;

        /// <summary>
        /// Количевство НДП (которые не идентифицировались по базе)
        /// </summary>
        public int ATDI_BTSCountNDP
        {
            get { return _ATDI_BTSCountNDP; }
            set { _ATDI_BTSCountNDP = value; OnPropertyChanged("ATDI_BTSCountNDP"); }
        }
        private int _ATDI_BTSCountNDP = 0;

        /// <summary>
        /// Количевство ППЕ (которые идентифицировались по базе но не совпали по частотам)
        /// </summary>
        public int ATDI_BTSCountNPE
        {
            get { return _ATDI_BTSCountNPE; }
            set { _ATDI_BTSCountNPE = value; OnPropertyChanged("ATDI_BTSCountNPE"); }
        }
        private int _ATDI_BTSCountNPE = 0;
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //if (PropertyChanged != null)
            //    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class GSMBTSData : INotifyPropertyChanged
    {
        #region 

        public uint FreqIndex
        {
            get { return _FreqIndex; }
            set { _FreqIndex = value; OnPropertyChanged("FreqIndex"); }
        }
        private uint _FreqIndex;

        #region GSM_Channel
        public int ARFCN
        {
            get { return _ARFCN; }
            set { _ARFCN = value; OnPropertyChanged("ARFCN"); }
        }
        private int _ARFCN = 0;

        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqUp = 0;

        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private decimal _FreqDn = 0;

        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        private string _StandartSubband;
        //public GSM_Channel Channel
        //{
        //    get { return _Channel; }
        //    set { _Channel = value; OnPropertyChanged("Channel"); }
        //}
        //private GSM_Channel _Channel;
        #endregion


        public uint IndSCHInfo
        {
            get { return _IndSCHInfo; }
            set { _IndSCHInfo = value; }
        }
        private uint _IndSCHInfo;

        public uint IndFirstSCHInfo
        {
            get { return _IndFirstSCHInfo; }
            set { _IndFirstSCHInfo = value; }
        }
        private uint _IndFirstSCHInfo;

        public double TimeOfSlotInSec
        {
            get { return _TimeOfSlotInSec; }
            set { _TimeOfSlotInSec = value; OnPropertyChanged("TimeOfSlotInSec"); }
        }
        private double _TimeOfSlotInSec;

        public double TimeOfSlotInSecssch
        {
            get { return _TimeOfSlotInSecssch; }
            set { _TimeOfSlotInSecssch = value; OnPropertyChanged("TimeOfSlotInSecssch"); }
        }
        private double _TimeOfSlotInSecssch;

        public uint Detected
        {
            get { return _Detected; }
            set { _Detected = value; OnPropertyChanged("Detected"); }
        }
        private uint _Detected;

        public int BSIC
        {
            get { return _BSIC; }
            set { _BSIC = value; OnPropertyChanged("BSIC"); }
        }
        private int _BSIC = -1;

        public int MCC
        {
            get { return _MCC; }
            set { _MCC = value; OnPropertyChanged("MCC"); }
        }
        private int _MCC = -1;

        public int MNC
        {
            get { return _MNC; }
            set { _MNC = value; OnPropertyChanged("MNC"); }
        }
        private int _MNC = -1;

        public int LAC
        {
            get { return _LAC; }
            set { _LAC = value; OnPropertyChanged("LAC"); }
        }
        private int _LAC = -1;

        public int CID
        {
            get { return _CID; }
            set { _CID = value; OnPropertyChanged("CID"); }
        }
        private int _CID = -1;

        //public int SectorFromCID
        //{
        //    get { return _SectorFromCID; }
        //    set { _SectorFromCID = value; OnPropertyChanged("SectorFromCID"); }
        //}
        //private int _SectorFromCID;

        public string GCID
        {
            get { return _GCID; }
            set { _GCID = value; OnPropertyChanged("GCID"); }
        }
        private string _GCID = "";

        public int CIDToDB
        {
            get { return _CIDToDB; }
            set { _CIDToDB = value; OnPropertyChanged("CIDToDB"); }
        }
        private int _CIDToDB = -1;

        public int SectorIDFromID
        {
            get { return _SectorIDFromID; }
            set { _SectorIDFromID = value; /*OnPropertyChanged("SectorIDFromID");*/ }
        }
        private int _SectorIDFromID = -1;

        public int SectorIDFromIdent
        {
            get { return _SectorIDFromIdent; }
            set { _SectorIDFromIdent = value; OnPropertyChanged("SectorIDFromIdent"); }
        }
        private int _SectorIDFromIdent = -1;

        public int SectorID
        {
            get { return _SectorID; }
            set { _SectorID = value; OnPropertyChanged("SectorID"); }
        }
        private int _SectorID = -1;

        public double Power
        {
            get { return _Power; }
            set { _Power = value; OnPropertyChanged("Power"); }
        }
        private double _Power;

        /// <summary>
        /// напряженность этого сигнала
        /// </summary>
        public double Strenght
        {
            get { return _Strenght; }
            set { _Strenght = value; OnPropertyChanged("Strenght"); }
        }
        private double _Strenght = -1000;

        /// <summary>
        /// Carrier-to-Interference 
        /// </summary>
        public double CarToInt
        {
            get { return _CarToInt; }
            set { _CarToInt = value; OnPropertyChanged("CarToInt"); }
        }
        private double _CarToInt = 0;

        public bool FullData
        {
            get { return _FullData; }
            set { _FullData = value; OnPropertyChanged("FullData"); }
        }
        private bool _FullData;

        public bool Find
        {
            get { return _Find; }
            set { _Find = value; OnPropertyChanged("Find"); }
        }
        private bool _Find;

        public DateTime LastLevelUpdete
        {
            get { return _LastLevelUpdete; }
            set { _LastLevelUpdete = value; OnPropertyChanged("LastLevelUpdete"); }
        }
        private DateTime _LastLevelUpdete = DateTime.MinValue;

        public long LastDetectionLevelUpdete
        {
            get { return _LastDetectionLevelUpdete; }
            set { _LastDetectionLevelUpdete = value; /*OnPropertyChanged("LastDetectionLevelUpdete");*/ }
        }
        private long _LastDetectionLevelUpdete = 0;

        public bool ThisIsMaximumSignalAtThisFrequency
        {
            get { return _ThisIsMaximumSignalAtThisFrequency; }
            set { _ThisIsMaximumSignalAtThisFrequency = value; OnPropertyChanged("ThisIsMaximumSignalAtThisFrequency"); }
        }
        private bool _ThisIsMaximumSignalAtThisFrequency = false;

        public bool DeleteFromMeasMon
        {
            get { return _DeleteFromMeasMon; }
            set { _DeleteFromMeasMon = value; OnPropertyChanged("DeleteFromMeasMon"); }
        }
        private bool _DeleteFromMeasMon = false;
        #endregion


        #region ATDI
        public string ATDI_id_task
        {
            get { return _ATDI_id_task; }
            set { _ATDI_id_task = value; OnPropertyChanged("ATDI_id_task"); }
        }
        private string _ATDI_id_task = "";

        public string ATDI_id_station
        {
            get { return _ATDI_id_station; }
            set { _ATDI_id_station = value; OnPropertyChanged("ATDI_id_station"); }
        }
        private string _ATDI_id_station = "";

        public int ATDI_id_permission
        {
            get { return _ATDI_permission_id; }
            set { _ATDI_permission_id = value; OnPropertyChanged("ATDI_permission_id"); }
        }
        private int _ATDI_permission_id = 0;

        public string ATDI_id_sector
        {
            get { return _ATDI_sector_id; }
            set { _ATDI_sector_id = value; OnPropertyChanged("ATDI_sector_id"); }
        }
        private string _ATDI_sector_id = "";

        public int ATDI_id_frequency
        {
            get { return _ATDI_Frequency_id; }
            set { _ATDI_Frequency_id = value; OnPropertyChanged("ATDI_Frequency_id"); }
        }
        private int _ATDI_Frequency_id = 0;

        public decimal ATDI_FrequencyPermission
        {
            get { return _ATDI_FrequencyPermission; }
            set { _ATDI_FrequencyPermission = value; OnPropertyChanged("ATDI_FrequencyPermission"); }
        }
        private decimal _ATDI_FrequencyPermission = 0;

        /// <summary>
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_Identifier_Find
        {
            get { return _ATDI_Identifier_Find; }
            set { _ATDI_Identifier_Find = value; OnPropertyChanged("ATDI_Identifier_Find"); }
        }
        private int _ATDI_Identifier_Find = 0;

        /// <summary>
        /// если нашлась строка в плане то здесь отметится соответствие частоте
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_FreqCheck_Find
        {
            get { return _ATDI_FreqCheck_Find; }
            set { _ATDI_FreqCheck_Find = value; OnPropertyChanged("ATDI_FreqCheck_Find"); }
        }
        private int _ATDI_FreqCheck_Find = 0;

        public string ATDI_GCID
        {
            get { return _ATDI_GCID; }
            set { _ATDI_GCID = value; OnPropertyChanged("ATDI_GCID"); }
        }
        private string _ATDI_GCID = string.Empty;

        public bool LR_NewDataToSave
        {
            get { return _LR_NewDataToSave; }
            set { _LR_NewDataToSave = value; OnPropertyChanged("LR_NewDataToSave"); }
        }
        private bool _LR_NewDataToSave = false;

        /// <summary>
        /// пишет изфу в station_sys_info 
        /// </summary>
        public DB.localatdi_station_sys_info GetStationInfo()
        {
            return new DB.localatdi_station_sys_info()
            {
                information_blocks = _station_sys_info.information_blocks,
                bandwidth = -1,
                base_id = -1,
                bsic = BSIC,
                channel_number = ARFCN,
                cid = CID,
                code_power = -1000,
                ctoi = CarToInt,
                eci = -1,
                e_node_b_id = -1,
                freq = FreqDn,
                icio = -1000,
                inband_power = -1000,
                iscp = -1000,
                lac = LAC,
                location = new DB.localatdi_geo_location()
                {
                    agl = -100000,
                    asl = MainWindow.gps.Altitude,
                    latitude = (double)MainWindow.gps.LatitudeDecimal,
                    longitude = (double)MainWindow.gps.LongitudeDecimal,
                },// MainWindow.gps.location,
                mcc = MCC,
                mnc = MNC,
                nid = -1,
                pci = -1,
                pn = -1,
                power = Power,
                ptotal = -1000,
                rnc = -1,
                rscp = -1000,
                rsrp = -1000,
                rsrq = -1000,
                sc = -1,
                sid = -1,
                tac = -1,
                type_cdmaevdo = "",
                ucid = -1
            };
            //return ssi;
        }
        public DB.localatdi_station_sys_info station_sys_info
        {
            get { return _station_sys_info; }
            set { _station_sys_info = value; OnPropertyChanged("station_sys_info"); }
        }
        private DB.localatdi_station_sys_info _station_sys_info = new DB.localatdi_station_sys_info() { };

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<DB.localatdi_level_meas_result> level_results
        {
            get { return _level_results; }
            set { _level_results = value; OnPropertyChanged("level_results"); }
        }
        private ObservableCollection<DB.localatdi_level_meas_result> _level_results = new ObservableCollection<DB.localatdi_level_meas_result>() { };

        //public ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar> Atdi_LevelsMeasurementsCar
        //{
        //    get { return _Atdi_LevelsMeasurementsCar; }
        //    set { _Atdi_LevelsMeasurementsCar = value; OnPropertyChanged("Atdi_LevelsMeasurementsCar"); }
        //}
        //private ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar> _Atdi_LevelsMeasurementsCar = new ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar>() { };
        #endregion

        //public ObservableRangeCollection<DB.local3GPPSystemInformationBlock> InformationBlocks
        //{
        //    get { return _InformationBlocks; }
        //    set { _InformationBlocks = value; OnPropertyChanged("InformationBlocks"); }
        //}
        //public ObservableRangeCollection<DB.local3GPPSystemInformationBlock> _InformationBlocks = new ObservableRangeCollection<DB.local3GPPSystemInformationBlock>() { };
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class UMTSAllData : INotifyPropertyChanged
    {
        public bool TechIsEnabled
        {
            get { return App.Sett.TSMxReceiver_Settings.UMTS.TechIsEnabled; }
            set { App.Sett.TSMxReceiver_Settings.UMTS.TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; OnPropertyChanged("IsRuning"); }
        }
        private static bool _IsRuning;

        public ObservableCollection<UMTSBTSData> _BTS = new ObservableCollection<UMTSBTSData>() { };
        public ObservableCollection<UMTSBTSData> BTS
        {
            get { return _BTS; }
            set { _BTS = value; OnPropertyChanged("BTS"); }
        }
        /// <summary>
        /// всего БС увиденых
        /// </summary>
        public int BTSCount
        {
            get { return _BTSCount; }
            set { _BTSCount = value; OnPropertyChanged("BTSCount"); }
        }
        private int _BTSCount = 0;

        /// <summary>
        /// Всего БС с GCID
        /// </summary>
        public int BTSCountWithGCID
        {
            get { return _BTSCountWithGCID; }
            set { _BTSCountWithGCID = value; OnPropertyChanged("BTSCountWithGCID"); }
        }
        private int _BTSCountWithGCID = 0;

        #region RS135
        /// <summary>
        /// Всего БС по базе с GCID
        /// </summary>
        public int RS135_BTSCountWithGCID
        {
            get { return _RS135_BTSCountWithGCID; }
            set { _RS135_BTSCountWithGCID = value; OnPropertyChanged("RS135_BTSCountWithGCID"); }
        }
        private int _RS135_BTSCountWithGCID = 0;
        /// <summary>
        /// Количевство НДП (которые не идентифицировались по базе)
        /// </summary>
        public int RS135_BTSCountNDP
        {
            get { return _RS135_BTSCountNDP; }
            set { _RS135_BTSCountNDP = value; OnPropertyChanged("RS135_BTSCountNDP"); }
        }
        private int _RS135_BTSCountNDP = 0;

        /// <summary>
        /// Количевство ППЕ (которые идентифицировались по базе но не совпали по частотам)
        /// </summary>
        public int RS135_BTSCountNPE
        {
            get { return _RS135_BTSCountNPE; }
            set { _RS135_BTSCountNPE = value; OnPropertyChanged("RS135_BTSCountNPE"); }
        }
        private int _RS135_BTSCountNPE = 0;

        #endregion

        #region ATDI
        /// <summary>
        /// Всего БС по базе с GCID
        /// </summary>
        public int ATDI_BTSCountWithGCID
        {
            get { return _ATDI_BTSCountWithGCID; }
            set { _ATDI_BTSCountWithGCID = value; OnPropertyChanged("ATDI_BTSCountWithGCID"); }
        }
        private int _ATDI_BTSCountWithGCID = 0;

        /// <summary>
        /// Количевство НДП (которые не идентифицировались по базе)
        /// </summary>
        public int ATDI_BTSCountNDP
        {
            get { return _ATDI_BTSCountNDP; }
            set { _ATDI_BTSCountNDP = value; OnPropertyChanged("ATDI_BTSCountNDP"); }
        }
        private int _ATDI_BTSCountNDP = 0;

        /// <summary>
        /// Количевство ППЕ (которые идентифицировались по базе но не совпали по частотам)
        /// </summary>
        public int ATDI_BTSCountNPE
        {
            get { return _ATDI_BTSCountNPE; }
            set { _ATDI_BTSCountNPE = value; OnPropertyChanged("ATDI_BTSCountNPE"); }
        }
        private int _ATDI_BTSCountNPE = 0;
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class UMTSBTSData : INotifyPropertyChanged
    {
        #region 
        public uint FreqIndex
        {
            get { return _FreqIndex; }
            set { _FreqIndex = value; OnPropertyChanged("FreqIndex"); }
        }
        private uint _FreqIndex = uint.MaxValue;

        #region UMTS_Channel
        public int UARFCN_DN
        {
            get { return _UARFCN_DN; }
            set { _UARFCN_DN = value; OnPropertyChanged("UARFCN_DN"); }
        }
        private int _UARFCN_DN = 0;

        public int UARFCN_UP
        {
            get { return _UARFCN_UP; }
            set { _UARFCN_UP = value; OnPropertyChanged("UARFCN_UP"); }
        }
        private int _UARFCN_UP = 0;

        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqUp = 0;

        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private decimal _FreqDn = 0;

        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        private string _StandartSubband = "";

        //public UMTS_Channel Channel
        //{
        //    get { return _Channel; }
        //    set { _Channel = value; OnPropertyChanged("Channel"); }
        //}
        //private UMTS_Channel _Channel = new UMTS_Channel() { };
        #endregion

        public double TimeOfSlotInSec
        {
            get { return _TimeOfSlotInSec; }
            set { _TimeOfSlotInSec = value; OnPropertyChanged("TimeOfSlotInSec"); }
        }
        private double _TimeOfSlotInSec = -1;

        public double TimeOfSlotInSecssch
        {
            get { return _TimeOfSlotInSecssch; }
            set { _TimeOfSlotInSecssch = value; OnPropertyChanged("TimeOfSlotInSecssch"); }
        }
        private double _TimeOfSlotInSecssch = -1;

        public uint Detected
        {
            get { return _Detected; }
            set { _Detected = value; OnPropertyChanged("Detected"); }
        }
        private uint _Detected = uint.MinValue;

        public int SC
        {
            get { return _SC; }
            set { _SC = value; OnPropertyChanged("SC"); }
        }
        private int _SC = -1;

        public int MCC
        {
            get { return _MCC; }
            set { _MCC = value; OnPropertyChanged("MCC"); }
        }
        private int _MCC = -1;

        public int MNC
        {
            get { return _MNC; }
            set { _MNC = value; OnPropertyChanged("MNC"); }
        }
        private int _MNC = -1;

        public int LAC
        {
            get { return _LAC; }
            set { _LAC = value; OnPropertyChanged("LAC"); }
        }
        private int _LAC = -1;

        public int UCID
        {
            get { return _UCID; }
            set { _UCID = value; OnPropertyChanged("UCID"); }
        }
        private int _UCID = -1;

        public int RNC
        {
            get { return _RNC; }
            set { _RNC = value; OnPropertyChanged("RNC"); }
        }
        private int _RNC = -1;

        public int CID
        {
            get { return _CID; }
            set { _CID = value; OnPropertyChanged("CID"); }
        }
        private int _CID = -1;

        //public int RS135CID
        //{
        //    get { return _RS135CID; }
        //    set { _RS135CID = value; OnPropertyChanged("RS135CID"); }
        //}
        //private int _RS135CID = -1;

        public int SectorFromCID
        {
            get { return _SectorFromCID; }
            set { _SectorFromCID = value; OnPropertyChanged("SectorFromCID"); }
        }
        private int _SectorFromCID;

        public int CarrierFromCID
        {
            get { return _CarrierFromCID; }
            set { _CarrierFromCID = value; OnPropertyChanged("CarrierFromCID"); }
        }
        private int _CarrierFromCID;

        public string GCID
        {
            get { return _GCID; }
            set { _GCID = value; OnPropertyChanged("GCID"); }
        }
        private string _GCID = "";

        public int CIDToDB
        {
            get { return _CIDToDB; }
            set { _CIDToDB = value; OnPropertyChanged("CIDToDB"); }
        }
        private int _CIDToDB = -1;

        public int SectorIDToDB
        {
            get { return _SectorIDToDB; }
            set { _SectorIDToDB = value; OnPropertyChanged("SectorIDToDB"); }
        }
        private int _SectorIDToDB = -1;

        public double ISCP
        {
            get { return _ISCP; }
            set { _ISCP = value; OnPropertyChanged("ISCP"); }
        }
        private double _ISCP;

        public double RSCP
        {
            get { return _RSCP; }
            set { _RSCP = value; OnPropertyChanged("RSCP"); }
        }
        private double _RSCP = -1000;

        /// <summary>
        /// напряженность этого сигнала
        /// </summary>
        public double Strenght
        {
            get { return _Strenght; }
            set { _Strenght = value; OnPropertyChanged("Strenght"); }
        }
        private double _Strenght = -1000;

        /// <summary>
        /// P total
        /// </summary>
        public double InbandPower
        {
            get { return _InbandPower; }
            set { _InbandPower = value; OnPropertyChanged("InbandPower"); }
        }
        private double _InbandPower = -1000;

        public double CodePower
        {
            get { return _CodePower; }
            set { _CodePower = value; OnPropertyChanged("CodePower"); }
        }
        private double _CodePower = -1000;

        public double IcIo
        {
            get { return _IcIo; }
            set { _IcIo = value; OnPropertyChanged("IcIo"); }
        }
        private double _IcIo = -1000;

        /// <summary>
        /// определяет есть ли GCID
        /// </summary>
        public bool FullData
        {
            get { return _FullData; }
            set { _FullData = value; OnPropertyChanged("FullData"); }
        }
        private bool _FullData = false;

        public bool Find
        {
            get { return _Find; }
            set { _Find = value; OnPropertyChanged("Find"); }
        }
        private bool _Find = false;

        public DateTime LastLevelUpdete
        {
            get { return _LastLevelUpdete; }
            set { _LastLevelUpdete = value; OnPropertyChanged("LastLevelUpdete"); }
        }
        private DateTime _LastLevelUpdete = DateTime.MinValue;

        public long LastDetectionLevelUpdete
        {
            get { return _LastDetectionLevelUpdete; }
            set { _LastDetectionLevelUpdete = value; /*OnPropertyChanged("LastDetectionLevelUpdete");*/ }
        }
        private long _LastDetectionLevelUpdete = 0;



        public bool ThisIsMaximumSignalAtThisFrequency
        {
            get { return _ThisIsMaximumSignalAtThisFrequency; }
            set { _ThisIsMaximumSignalAtThisFrequency = value; OnPropertyChanged("ThisIsMaximumSignalAtThisFrequency"); }
        }
        private bool _ThisIsMaximumSignalAtThisFrequency = false;

        public bool DeleteFromMeasMon
        {
            get { return _DeleteFromMeasMon; }
            set { _DeleteFromMeasMon = value; OnPropertyChanged("DeleteFromMeasMon"); }
        }
        private bool _DeleteFromMeasMon = false;
        #endregion

        #region ATDI
        public string ATDI_id_task
        {
            get { return _ATDI_id_task; }
            set { _ATDI_id_task = value; OnPropertyChanged("ATDI_id_task"); }
        }
        private string _ATDI_id_task = "";

        public string ATDI_id_station
        {
            get { return _ATDI_id_station; }
            set { _ATDI_id_station = value; OnPropertyChanged("ATDI_id_station"); }
        }
        private string _ATDI_id_station = "";

        public int ATDI_id_permission
        {
            get { return _ATDI_permission_id; }
            set { _ATDI_permission_id = value; OnPropertyChanged("ATDI_permission_id"); }
        }
        private int _ATDI_permission_id = 0;

        public string ATDI_id_sector
        {
            get { return _ATDI_sector_id; }
            set { _ATDI_sector_id = value; OnPropertyChanged("ATDI_sector_id"); }
        }
        private string _ATDI_sector_id = "";

        public int ATDI_id_frequency
        {
            get { return _ATDI_Frequency_id; }
            set { _ATDI_Frequency_id = value; OnPropertyChanged("ATDI_Frequency_id"); }
        }
        private int _ATDI_Frequency_id = 0;

        public decimal ATDI_FrequencyPermission
        {
            get { return _ATDI_FrequencyPermission; }
            set { _ATDI_FrequencyPermission = value; OnPropertyChanged("ATDI_FrequencyPermission"); }
        }
        private decimal _ATDI_FrequencyPermission = 0;

        /// <summary>
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_Identifier_Find
        {
            get { return _ATDI_Identifier_Find; }
            set { _ATDI_Identifier_Find = value; OnPropertyChanged("ATDI_Identifier_Find"); }
        }
        private int _ATDI_Identifier_Find = 0;
        /// <summary>
        /// если нашлась строка в плане то здесь отметится соответствие частоте
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_FreqCheck_Find
        {
            get { return _ATDI_FreqCheck_Find; }
            set { _ATDI_FreqCheck_Find = value; OnPropertyChanged("ATDI_FreqCheck_Find"); }
        }
        private int _ATDI_FreqCheck_Find = 0;

        public string ATDI_GCID
        {
            get { return _ATDI_GCID; }
            set { _ATDI_GCID = value; OnPropertyChanged("ATDI_GCID"); }
        }
        private string _ATDI_GCID = string.Empty;

        public bool LR_NewDataToSave
        {
            get { return _LR_NewDataToSave; }
            set { _LR_NewDataToSave = value; OnPropertyChanged("LR_NewDataToSave"); }
        }
        private bool _LR_NewDataToSave = false;
        #endregion
        /// <summary>
        /// пишет изфу в station_sys_info 
        /// </summary>
        public DB.localatdi_station_sys_info GetStationInfo()
        {
            return new DB.localatdi_station_sys_info()
            {
                information_blocks = _station_sys_info.information_blocks,
                bandwidth = -1,
                base_id = -1,
                bsic = -1,
                channel_number = UARFCN_DN,
                cid = CID,
                code_power = CodePower,
                ctoi = -1000,
                eci = -1,
                e_node_b_id = -1,
                freq = FreqDn,
                icio = IcIo,
                inband_power = InbandPower,
                iscp = ISCP,
                lac = LAC,
                location = new DB.localatdi_geo_location()
                {
                    asl = MainWindow.gps.Altitude,
                    latitude = (double)MainWindow.gps.LatitudeDecimal,
                    longitude = (double)MainWindow.gps.LongitudeDecimal,
                },// MainWindow.gps.location,
                mcc = MCC,
                mnc = MNC,
                nid = -1,
                pci = -1,
                pn = -1,
                power = -1000,
                ptotal = InbandPower,
                rnc = RNC,
                rscp = RSCP,
                rsrp = -1000,
                rsrq = -1000,
                sc = SC,
                sid = -1,
                tac = -1,
                type_cdmaevdo = "",
                ucid = UCID
            };
            //return ssi;
        }
        public DB.localatdi_station_sys_info station_sys_info
        {
            get { return _station_sys_info; }
            set { _station_sys_info = value; OnPropertyChanged("station_sys_info"); }
        }
        private DB.localatdi_station_sys_info _station_sys_info = new DB.localatdi_station_sys_info() { };

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<DB.localatdi_level_meas_result> level_results
        {
            get { return _level_results; }
            set { _level_results = value; OnPropertyChanged("level_results"); }
        }
        private ObservableCollection<DB.localatdi_level_meas_result> _level_results = new ObservableCollection<DB.localatdi_level_meas_result>() { };
        //public ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar> Atdi_LevelsMeasurementsCar
        //{
        //    get { return _Atdi_LevelsMeasurementsCar; }
        //    set { _Atdi_LevelsMeasurementsCar = value; OnPropertyChanged("Atdi_LevelsMeasurementsCar"); }
        //}
        //private ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar> _Atdi_LevelsMeasurementsCar = new ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar>() { };
        //#endregion

        //public ObservableRangeCollection<DB.local3GPPSystemInformationBlock> InformationBlocks
        //{
        //    get { return _InformationBlocks; }
        //    set { _InformationBlocks = value; OnPropertyChanged("InformationBlocks"); }
        //}
        //public ObservableRangeCollection<DB.local3GPPSystemInformationBlock> _InformationBlocks = new ObservableRangeCollection<DB.local3GPPSystemInformationBlock>() { };
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }


    public class CDMAAllData : INotifyPropertyChanged
    {
        public bool TechIsEnabled
        {
            get { return App.Sett.TSMxReceiver_Settings.CDMA.TechIsEnabled; }
            set { App.Sett.TSMxReceiver_Settings.CDMA.TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; OnPropertyChanged("IsRuning"); }
        }
        private static bool _IsRuning;


        public ObservableCollection<CDMABTSData> _BTS = new ObservableCollection<CDMABTSData>() { };
        public ObservableCollection<CDMABTSData> BTS
        {
            get { return _BTS; }
            set { _BTS = value; OnPropertyChanged("BTS"); }
        }

        /// <summary>
        /// всего БС увиденых
        /// </summary>
        public int BTSCount
        {
            get { return _BTSCount; }
            set { _BTSCount = value; OnPropertyChanged("BTSCount"); }
        }
        private int _BTSCount = 0;

        /// <summary>
        /// Всего БС с GCID
        /// </summary>
        public int BTSCountWithGCID
        {
            get { return _BTSCountWithGCID; }
            set { _BTSCountWithGCID = value; OnPropertyChanged("BTSCountWithGCID"); }
        }
        private int _BTSCountWithGCID = 0;
                
        #region ATDI
        /// <summary>
        /// Всего БС по базе с GCID
        /// </summary>
        public int ATDI_BTSCountWithGCID
        {
            get { return _ATDI_BTSCountWithGCID; }
            set { _ATDI_BTSCountWithGCID = value; OnPropertyChanged("ATDI_BTSCountWithGCID"); }
        }
        private int _ATDI_BTSCountWithGCID = 0;

        /// <summary>
        /// Количевство НДП (которые не идентифицировались по базе)
        /// </summary>
        public int ATDI_BTSCountNDP
        {
            get { return _ATDI_BTSCountNDP; }
            set { _ATDI_BTSCountNDP = value; OnPropertyChanged("ATDI_BTSCountNDP"); }
        }
        private int _ATDI_BTSCountNDP = 0;

        /// <summary>
        /// Количевство ППЕ (которые идентифицировались по базе но не совпали по частотам)
        /// </summary>
        public int ATDI_BTSCountNPE
        {
            get { return _ATDI_BTSCountNPE; }
            set { _ATDI_BTSCountNPE = value; OnPropertyChanged("ATDI_BTSCountNPE"); }
        }
        private int _ATDI_BTSCountNPE = 0;
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //if (PropertyChanged != null)
            //    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class CDMABTSData : INotifyPropertyChanged
    {
        #region
        

        private uint _FreqIndex;
        public uint FreqIndex
        {
            get { return _FreqIndex; }
            set { _FreqIndex = value; OnPropertyChanged("FreqIndex"); }
        }
        private uint _Indicator;
        public uint Indicator
        {
            get { return _Indicator; }
            set { _Indicator = value; OnPropertyChanged("Indicator"); }
        }
        #region CDMA_Channel
        public int ChannelN
        {
            get { return _ChannelN; }
            set { _ChannelN = value; OnPropertyChanged("ChannelN"); }
        }
        private int _ChannelN = 0;

        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private decimal _FreqDn = 0;

        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqUp = 0;

        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        private string _StandartSubband = "";
        //private CDMA_Channel _Channel = new CDMA_Channel() { };
        //public CDMA_Channel Channel
        //{
        //    get { return _Channel; }
        //    set { _Channel = value; OnPropertyChanged("Channel"); }
        //}
        #endregion
        private double _TimeOfSlotInSec;
        public double TimeOfSlotInSec
        {
            get { return _TimeOfSlotInSec; }
            set { _TimeOfSlotInSec = value; OnPropertyChanged("TimeOfSlotInSec"); }
        }
        private double _TimeOfSlotInSecssch;
        public double TimeOfSlotInSecssch
        {
            get { return _TimeOfSlotInSecssch; }
            set { _TimeOfSlotInSecssch = value; OnPropertyChanged("TimeOfSlotInSecssch"); }
        }
        private uint _Detected;
        public uint Detected
        {
            get { return _Detected; }
            set { _Detected = value; OnPropertyChanged("Detected"); }
        }

        /// <summary>
        /// true = EVDO false = CDMAO
        /// </summary>
        public bool Type
        {
            get { return _Type; }
            set { _Type = value; OnPropertyChanged("Type"); }
        }
        private bool _Type = false;

        private int _PN;
        public int PN
        {
            get { return _PN; }
            set { _PN = value; OnPropertyChanged("PN"); }
        }
        private int _SID = -1;
        public int SID
        {
            get { return _SID; }
            set { _SID = value; OnPropertyChanged("SID"); }
        }
        private int _NID = -1;
        public int NID
        {
            get { return _NID; }
            set { _NID = value; OnPropertyChanged("NID"); }
        }

        public int MCC
        {
            get { return _MCC; }
            set { _MCC = value; OnPropertyChanged("MCC"); }
        }
        private int _MCC = -1;

        public int MNC
        {
            get { return _MNC; }
            set { _MNC = value; OnPropertyChanged("MNC"); }
        }
        private int _MNC = -1;


        public int BaseID
        {
            get { return _BaseID; }
            set { _BaseID = value; OnPropertyChanged("BaseID"); }
        }
        private int _BaseID = -1;

        public string GCID
        {
            get { return _GCID; }
            set { _GCID = value; OnPropertyChanged("GCID"); }
        }
        private string _GCID = "";

        public int CIDToDB
        {
            get { return _CIDToDB; }
            set { _CIDToDB = value; OnPropertyChanged("CIDToDB"); }
        }
        private int _CIDToDB = -1;

        
        public double RSCP
        {
            get { return _RSCP; }
            set { _RSCP = value; OnPropertyChanged("RSCP"); }
        }
        private double _RSCP = -1000;
        /// <summary>
        /// напряженность этого сигнала
        /// </summary>
        public double Strenght
        {
            get { return _Strenght; }
            set { _Strenght = value; OnPropertyChanged("Strenght"); }
        }
        private double _Strenght = -1000;

        
        public double PTotal
        {
            get { return _PTotal; }
            set { _PTotal = value; OnPropertyChanged("PTotal"); }
        }
        private double _PTotal = -1000;
        
        public double AverageInbandPower
        {
            get { return _AverageInbandPower; }
            set { _AverageInbandPower = value; OnPropertyChanged("AverageInbandPower"); }
        }
        private double _AverageInbandPower = -1000;

        public double IcIo
        {
            get { return _IcIo; }
            set { _IcIo = value; OnPropertyChanged("IcIo"); }
        }
        private double _IcIo = 0;
        //public decimal EcIo
        //{
        //    get { return _EcIo; }
        //    set { _EcIo = value; OnPropertyChanged("EcIo"); }
        //}
        //private decimal _EcIo = -1000;

        private bool _FullData;
        public bool FullData
        {
            get { return _FullData; }
            set { _FullData = value; OnPropertyChanged("FullData"); }
        }
        private bool _Find;
        public bool Find
        {
            get { return _Find; }
            set { _Find = value; OnPropertyChanged("Find"); }
        }

        public DateTime LastLevelUpdete
        {
            get { return _LastLevelUpdete; }
            set { _LastLevelUpdete = value; OnPropertyChanged("LastLevelUpdete"); }
        }
        private DateTime _LastLevelUpdete = DateTime.MinValue;

        public long LastDetectionLevelUpdete
        {
            get { return _LastDetectionLevelUpdete; }
            set { _LastDetectionLevelUpdete = value; /*OnPropertyChanged("LastDetectionLevelUpdete");*/ }
        }
        private long _LastDetectionLevelUpdete = 0;

        public bool ThisIsMaximumSignalAtThisFrequency
        {
            get { return _ThisIsMaximumSignalAtThisFrequency; }
            set { _ThisIsMaximumSignalAtThisFrequency = value; OnPropertyChanged("ThisIsMaximumSignalAtThisFrequency"); }
        }
        private bool _ThisIsMaximumSignalAtThisFrequency = false;

        public bool DeleteFromMeasMon
        {
            get { return _DeleteFromMeasMon; }
            set { _DeleteFromMeasMon = value; OnPropertyChanged("DeleteFromMeasMon"); }
        }
        private bool _DeleteFromMeasMon = false;

        /// <summary>
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int Identifier_Find
        {
            get { return _Identifier_Find; }
            set { _Identifier_Find = value; OnPropertyChanged("Identifier_Find"); }
        }
        private int _Identifier_Find = 0;


        /// <summary>
        /// PlanFreq_ID
        /// -1 не найдено
        /// </summary>
        public int PlanFreq_ID
        {
            get { return _PlanFreq_ID; }
            set { _PlanFreq_ID = value; OnPropertyChanged("PlanFreq_ID"); }
        }
        private int _PlanFreq_ID = -1;
        /// <summary>
        /// Plan_ID
        /// -1 не найдено
        /// </summary>
        public int Plan_ID
        {
            get { return _Plan_ID; }
            set { _Plan_ID = value; OnPropertyChanged("Plan_ID"); }
        }
        private int _Plan_ID = -1;
        /// <summary>
        /// если нашлась строка в плане то здесь отметится соответствие частоте
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int FreqCheck_Find
        {
            get { return _FreqCheck_Find; }
            set { _FreqCheck_Find = value; OnPropertyChanged("FreqCheck_Find"); }
        }
        private int _FreqCheck_Find = 0;
        #endregion
        
        #region ATDI
        public string ATDI_id_task
        {
            get { return _ATDI_id_task; }
            set { _ATDI_id_task = value; OnPropertyChanged("ATDI_id_task"); }
        }
        private string _ATDI_id_task = "";

        public string ATDI_id_station
        {
            get { return _ATDI_id_station; }
            set { _ATDI_id_station = value; OnPropertyChanged("ATDI_id_station"); }
        }
        private string _ATDI_id_station = "";

        public int ATDI_id_permission
        {
            get { return _ATDI_permission_id; }
            set { _ATDI_permission_id = value; OnPropertyChanged("ATDI_permission_id"); }
        }
        private int _ATDI_permission_id = 0;

        public string ATDI_id_sector
        {
            get { return _ATDI_sector_id; }
            set { _ATDI_sector_id = value; OnPropertyChanged("ATDI_sector_id"); }
        }
        private string _ATDI_sector_id = "";

        public int ATDI_id_frequency
        {
            get { return _ATDI_Frequency_id; }
            set { _ATDI_Frequency_id = value; OnPropertyChanged("ATDI_Frequency_id"); }
        }
        private int _ATDI_Frequency_id = 0;

        public decimal ATDI_FrequencyPermission
        {
            get { return _ATDI_FrequencyPermission; }
            set { _ATDI_FrequencyPermission = value; OnPropertyChanged("ATDI_FrequencyPermission"); }
        }
        private decimal _ATDI_FrequencyPermission = 0;

        /// <summary>
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_Identifier_Find
        {
            get { return _ATDI_Identifier_Find; }
            set { _ATDI_Identifier_Find = value; OnPropertyChanged("ATDI_Identifier_Find"); }
        }
        private int _ATDI_Identifier_Find = 0;
        /// <summary>
        /// если нашлась строка в плане то здесь отметится соответствие частоте
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_FreqCheck_Find
        {
            get { return _ATDI_FreqCheck_Find; }
            set { _ATDI_FreqCheck_Find = value; OnPropertyChanged("ATDI_FreqCheck_Find"); }
        }
        private int _ATDI_FreqCheck_Find = 0;

        public string ATDI_GCID
        {
            get { return _ATDI_GCID; }
            set { _ATDI_GCID = value; OnPropertyChanged("ATDI_GCID"); }
        }
        private string _ATDI_GCID = string.Empty;

        public bool LR_NewDataToSave
        {
            get { return _LR_NewDataToSave; }
            set { _LR_NewDataToSave = value; OnPropertyChanged("LR_NewDataToSave"); }
        }
        private bool _LR_NewDataToSave = false;

        //public ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar> Atdi_LevelsMeasurementsCar
        //{
        //    get { return _Atdi_LevelsMeasurementsCar; }
        //    set { _Atdi_LevelsMeasurementsCar = value; OnPropertyChanged("Atdi_LevelsMeasurementsCar"); }
        //}
        //private ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar> _Atdi_LevelsMeasurementsCar = new ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar>() { };
        #endregion
        public string ATDI_IDFrom
        {
            get { return _ATDI_IDFrom; }
            set { _ATDI_IDFrom = value; OnPropertyChanged("ATDI_IDFrom"); }
        }
        private string _ATDI_IDFrom = string.Empty;

        /// <summary>
        /// пишет изфу в station_sys_info 
        /// </summary>
        public DB.localatdi_station_sys_info GetStationInfo()
        {
            DB.localatdi_station_sys_info ssi = new DB.localatdi_station_sys_info()
            {
                information_blocks = _station_sys_info.information_blocks,
                bandwidth = -1,
                base_id = -1,
                bsic = -1,
                channel_number = ChannelN,
                cid = -1,
                code_power = -1000,
                ctoi = -1,
                eci = -1,
                e_node_b_id = -1,
                freq = FreqDn,
                icio = IcIo,
                inband_power = -1000,
                iscp = -1000,
                lac = -1,
                location = new DB.localatdi_geo_location()
                {
                    asl = MainWindow.gps.Altitude,
                    latitude = (double)MainWindow.gps.LatitudeDecimal,
                    longitude = (double)MainWindow.gps.LongitudeDecimal,
                },// MainWindow.gps.location,
                nid = NID,
                pci = -1,
                pn = PN,
                power = -1000,
                ptotal = PTotal,
                rnc = -1,
                rscp = RSCP,
                rsrp = -1000,
                rsrq = -1000,
                sc = -1,
                sid = SID,
                tac = -1,
                type_cdmaevdo = "",
                ucid = -1
            };
            if (Type) { ssi.type_cdmaevdo = "EVDO"; }
            else { ssi.type_cdmaevdo = "CDMA"; }
            if (Type == false)
            {
                if (MCC > -1) ssi.mcc = MCC;
                if (MNC > -1) ssi.mnc = MNC;
            }
            return ssi;
        }

        public DB.localatdi_station_sys_info station_sys_info
        {
            get { return _station_sys_info; }
            set { _station_sys_info = value; OnPropertyChanged("station_sys_info"); }
        }
        private DB.localatdi_station_sys_info _station_sys_info = new DB.localatdi_station_sys_info() { };

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<DB.localatdi_level_meas_result> level_results
        {
            get { return _level_results; }
            set { _level_results = value; OnPropertyChanged("level_results"); }
        }
        private ObservableCollection<DB.localatdi_level_meas_result> _level_results = new ObservableCollection<DB.localatdi_level_meas_result>() { };

        //public ObservableRangeCollection<DB.local3GPPSystemInformationBlock> InformationBlocks
        //{
        //    get { return _InformationBlocks; }
        //    set { _InformationBlocks = value; OnPropertyChanged("InformationBlocks"); }
        //}
        //public ObservableRangeCollection<DB.local3GPPSystemInformationBlock> _InformationBlocks = new ObservableRangeCollection<DB.local3GPPSystemInformationBlock>() { };
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class CDMAFPichData : INotifyPropertyChanged
    {
        #region из елемента Plan
        private uint _FreqIndex;
        public uint FreqIndex
        {
            get { return _FreqIndex; }
            set { _FreqIndex = value; OnPropertyChanged("FreqIndex"); }
        }

        private string _Type;
        /// <summary>
        /// CDMA/EVDO
        /// </summary>
        public string Type
        {
            get { return _Type; }
            set { _Type = value; OnPropertyChanged("Type"); }
        }
        private string _PN;
        public string PN
        {
            get { return _PN; }
            set { _PN = value; OnPropertyChanged("PN"); }
        }
        private decimal _EcIo;
        public decimal EcIo
        {
            get { return _EcIo; }
            set { _EcIo = value; OnPropertyChanged("EcIo"); }
        }
        private decimal _RSCP;
        public decimal RSCP
        {
            get { return _RSCP; }
            set { _RSCP = value; OnPropertyChanged("RSCP"); }
        }
        private decimal _PTotal;
        public decimal PTotal
        {
            get { return _PTotal; }
            set { _PTotal = value; OnPropertyChanged("PTotal"); }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class LTEAllData : INotifyPropertyChanged
    {
        public bool TechIsEnabled
        {
            get { return App.Sett.TSMxReceiver_Settings.LTE.TechIsEnabled; }
            set { App.Sett.TSMxReceiver_Settings.LTE.TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; OnPropertyChanged("IsRuning"); }
        }
        private static bool _IsRuning;

        public ObservableCollection<LTEBTSData> _BTS = new ObservableCollection<LTEBTSData>() { };
        public ObservableCollection<LTEBTSData> BTS
        {
            get { return _BTS; }
            set { _BTS = value; OnPropertyChanged("BTS"); }
        }
        /// <summary>
        /// всего БС увиденых
        /// </summary>
        public int BTSCount
        {
            get { return _BTSCount; }
            set { _BTSCount = value; OnPropertyChanged("BTSCount"); }
        }
        private int _BTSCount = 0;

        /// <summary>
        /// Всего БС с GCID
        /// </summary>
        public int BTSCountWithGCID
        {
            get { return _BTSCountWithGCID; }
            set { _BTSCountWithGCID = value; OnPropertyChanged("BTSCountWithGCID"); }
        }
        private int _BTSCountWithGCID = 0;

        #region ATDI
        /// <summary>
        /// Всего БС по базе с GCID
        /// </summary>
        public int ATDI_BTSCountWithGCID
        {
            get { return _ATDI_BTSCountWithGCID; }
            set { _ATDI_BTSCountWithGCID = value; OnPropertyChanged("ATDI_BTSCountWithGCID"); }
        }
        private int _ATDI_BTSCountWithGCID = 0;

        /// <summary>
        /// Количевство НДП (которые не идентифицировались по базе)
        /// </summary>
        public int ATDI_BTSCountNDP
        {
            get { return _ATDI_BTSCountNDP; }
            set { _ATDI_BTSCountNDP = value; OnPropertyChanged("ATDI_BTSCountNDP"); }
        }
        private int _ATDI_BTSCountNDP = 0;

        /// <summary>
        /// Количевство ППЕ (которые идентифицировались по базе но не совпали по частотам)
        /// </summary>
        public int ATDI_BTSCountNPE
        {
            get { return _ATDI_BTSCountNPE; }
            set { _ATDI_BTSCountNPE = value; OnPropertyChanged("ATDI_BTSCountNPE"); }
        }
        private int _ATDI_BTSCountNPE = 0;
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class LTEBTSData : INotifyPropertyChanged
    {
        #region 
        public uint FreqIndex
        {
            get { return _FreqIndex; }
            set { _FreqIndex = value; OnPropertyChanged("FreqIndex"); }
        }
        private uint _FreqIndex = uint.MaxValue;

        public uint FreqBtsIndex
        {
            get { return _FreqBtsIndex; }
            set { _FreqBtsIndex = value; OnPropertyChanged("FreqBtsIndex"); }
        }
        private uint _FreqBtsIndex = uint.MaxValue;

        #region LTE_Channel
        public int EARFCN_DN
        {
            get { return _EARFCN_DN; }
            set { _EARFCN_DN = value; OnPropertyChanged("EARFCN_DN"); }
        }
        private int _EARFCN_DN = 0;

        public int EARFCN_UP
        {
            get { return _EARFCN_UP; }
            set { _EARFCN_UP = value; OnPropertyChanged("EARFCN_UP"); }
        }
        private int _EARFCN_UP = 0;

        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqUp = 0;

        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private decimal _FreqDn = 0;


        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        private string _StandartSubband = "";

        //public LTE_Channel Channel
        //{
        //    get { return _Channel; }
        //    set { _Channel = value; OnPropertyChanged("Channel"); }
        //}
        //private LTE_Channel _Channel = new LTE_Channel() { };
        #endregion

        public decimal Bandwidth
        {
            get { return _Bandwidth; }
            set { _Bandwidth = value; BandwidthComparison(); OnPropertyChanged("Bandwidth"); }
        }
        private decimal _Bandwidth = -1;

        public int MCC
        {
            get { return _MCC; }
            set { _MCC = value; OnPropertyChanged("MCC"); }
        }
        private int _MCC = -1;

        public int MNC
        {
            get { return _MNC; }
            set { _MNC = value; OnPropertyChanged("MNC"); }
        }
        private int _MNC = -1;

        public int TAC
        {
            get { return _TAC; }
            set { _TAC = value; OnPropertyChanged("TAC"); }
        }
        private int _TAC = -1;

        public int CelId28
        {
            get { return _CelId28; }
            set { _CelId28 = value; OnPropertyChanged("CelId28"); }
        }
        private int _CelId28 = -1;

        public int eNodeBId
        {
            get { return _eNodeBId; }
            set { _eNodeBId = value; OnPropertyChanged("eNodeBId"); }
        }
        private int _eNodeBId = -1;

        public int CID
        {
            get { return _CID; }
            set { _CID = value; OnPropertyChanged("CID"); }
        }
        private int _CID = -1;

        public int PCI
        {
            get { return _PCI; }
            set { _PCI = value; OnPropertyChanged("PCI"); }
        }
        private int _PCI = -1;

        public int CIDToDB
        {
            get { return _CIDToDB; }
            set { _CIDToDB = value; OnPropertyChanged("CIDToDB"); }
        }
        private int _CIDToDB = -1;

        public double Power
        {
            get { return _Power; }
            set { _Power = value; OnPropertyChanged("Power"); }
        }
        private double _Power = -1000;

        /// <summary>
        /// P total
        /// </summary>
        public double InbandPower
        {
            get { return _InbandPower; }
            set { _InbandPower = value; OnPropertyChanged("InbandPower"); }
        }
        private double _InbandPower = -1000;

        //public double WB_RSSI
        //{
        //    get { return _WB_RSSI; }
        //    set { _WB_RSSI = value; OnPropertyChanged("WB_RSSI"); }
        //}
        //private double _WB_RSSI = -1000;

        public double WB_RS_RSSI
        {
            get { return _WB_RS_RSSI; }
            set { _WB_RS_RSSI = value; OnPropertyChanged("WB_RS_RSSI"); }
        }
        private double _WB_RS_RSSI = -1000;

        public double RSRP
        {
            get { return _RSRP; }
            set { _RSRP = value; OnPropertyChanged("RSRP"); }
        }
        private double _RSRP = -1000;

        /// <summary>
        /// напряженность этого сигнала
        /// </summary>
        public double Strenght
        {
            get { return _Strenght; }
            set { _Strenght = value; OnPropertyChanged("Strenght"); }
        }
        private double _Strenght = -1000;

        public double WB_RSRP
        {
            get { return _WB_RSRP; }
            set { _WB_RSRP = value; OnPropertyChanged("WB_RSRP"); }
        }
        private double _WB_RSRP = -1000;

        public double RSRQ
        {
            get { return _RSRQ; }
            set { _RSRQ = value; OnPropertyChanged("RSRQ"); }
        }
        private double _RSRQ = -1000;

        public double WB_RSRQ
        {
            get { return _WB_RSRQ; }
            set { _WB_RSRQ = value; OnPropertyChanged("WB_RSRQ"); }
        }
        private double _WB_RSRQ = -1000;

        //public double RS_SINR
        //{
        //    get { return _RS_SINR; }
        //    set { _RS_SINR = value; OnPropertyChanged("RS_SINR"); }
        //}
        //private double _RS_SINR = -1000;

        //public double WB_RS_SINR
        //{
        //    get { return _WB_RS_SINR; }
        //    set { _WB_RS_SINR = value; OnPropertyChanged("WB_RS_SINR"); }
        //}
        //private double _WB_RS_SINR = -1000;

        //public double SINR
        //{
        //    get { return _SINR; }
        //    set { _SINR = value; OnPropertyChanged("SINR"); }
        //}
        //private double _SINR = -1000;

        public string MIMO_2x2
        {
            get { return _MIMO_2x2; }
            set { _MIMO_2x2 = value; OnPropertyChanged("MIMO_2x2"); }
        }
        private string _MIMO_2x2 = "";

        public string eNodeB_Name
        {
            get { return _eNodeB_Name; }
            set { _eNodeB_Name = value; OnPropertyChanged("eNodeB_Name"); }
        }
        private string _eNodeB_Name = "";

        public decimal Dist
        {
            get { return _Dist; }
            set { _Dist = value; OnPropertyChanged("Dist"); }
        }
        private decimal _Dist = -1000;

        public double CIRofCP
        {
            get { return _CIRofCP; }
            set { _CIRofCP = value; OnPropertyChanged("CIRofCP"); }
        }
        private double _CIRofCP = -1000;

        public string ECI20p8
        {
            get { return _ECI20p8; }
            set { _ECI20p8 = value; OnPropertyChanged("ECI20p8"); }
        }
        private string _ECI20p8 = "";

        public string ECI28
        {
            get { return _ECI28; }
            set { _ECI28 = value; OnPropertyChanged("ECI28"); }
        }
        private string _ECI28 = "";

        //public int EARFCN
        //{
        //    get { return _EARFCN; }
        //    set { _EARFCN = value; OnPropertyChanged("EARFCN"); }
        //}
        //private int _EARFCN = -1000;

        public string ECGI
        {
            get { return _ECGI; }
            set { _ECGI = value; OnPropertyChanged("ECGI"); }
        }
        private string _ECGI = "";





        //public int RS135CID
        //{
        //    get { return _RS135CID; }
        //    set { _RS135CID = value; OnPropertyChanged("RS135CID"); }
        //}
        //private int _RS135CID = -1;

        public int SectorFromCID
        {
            get { return _SectorFromCID; }
            set { _SectorFromCID = value; OnPropertyChanged("SectorFromCID"); }
        }
        private int _SectorFromCID;

        public int CarrierFromCID
        {
            get { return _CarrierFromCID; }
            set { _CarrierFromCID = value; OnPropertyChanged("CarrierFromCID"); }
        }
        private int _CarrierFromCID;

        public string GCID
        {
            get { return _GCID; }
            set { _GCID = value; OnPropertyChanged("GCID"); }
        }
        private string _GCID = "";

        //public decimal ISCP
        //{
        //    get { return _ISCP; }
        //    set { _ISCP = value; OnPropertyChanged("ISCP"); }
        //}
        //private decimal _ISCP;

        //public decimal RSCP
        //{
        //    get { return _RSCP; }
        //    set { _RSCP = value; OnPropertyChanged("RSCP"); }
        //}
        //private decimal _RSCP = -1000;



        //public decimal CodePower
        //{
        //    get { return _CodePower; }
        //    set { _CodePower = value; OnPropertyChanged("CodePower"); }
        //}
        //private decimal _CodePower = -1000;

        //public decimal IcIo
        //{
        //    get { return _IcIo; }
        //    set { _IcIo = value; OnPropertyChanged("IcIo"); }
        //}
        //private decimal _IcIo = 0;

        /// <summary>
        /// определяет есть ли GCID
        /// </summary>
        public bool FullData
        {
            get { return _FullData; }
            set { _FullData = value; OnPropertyChanged("FullData"); }
        }
        private bool _FullData = false;

        public bool Find
        {
            get { return _Find; }
            set { _Find = value; OnPropertyChanged("Find"); }
        }
        private bool _Find = false;

        public DateTime LastLevelUpdete
        {
            get { return _LastLevelUpdete; }
            set { _LastLevelUpdete = value; OnPropertyChanged("LastLevelUpdete"); }
        }
        private DateTime _LastLevelUpdete = DateTime.MinValue;

        public long LastDetectionLevelUpdete
        {
            get { return _LastDetectionLevelUpdete; }
            set { _LastDetectionLevelUpdete = value; /*OnPropertyChanged("LastDetectionLevelUpdete");*/ }
        }
        private long _LastDetectionLevelUpdete = 0;

        public bool ThisIsMaximumSignalAtThisFrequency
        {
            get { return _ThisIsMaximumSignalAtThisFrequency; }
            set { _ThisIsMaximumSignalAtThisFrequency = value; OnPropertyChanged("ThisIsMaximumSignalAtThisFrequency"); }
        }
        private bool _ThisIsMaximumSignalAtThisFrequency = false;

        public bool DeleteFromMeasMon
        {
            get { return _DeleteFromMeasMon; }
            set { _DeleteFromMeasMon = value; OnPropertyChanged("DeleteFromMeasMon"); }
        }
        private bool _DeleteFromMeasMon = false;

        #endregion



        #region ATDI
        public string ATDI_id_task
        {
            get { return _ATDI_id_task; }
            set { _ATDI_id_task = value; OnPropertyChanged("ATDI_id_task"); }
        }
        private string _ATDI_id_task = "";

        public string ATDI_id_station
        {
            get { return _ATDI_id_station; }
            set { _ATDI_id_station = value; OnPropertyChanged("ATDI_id_station"); }
        }
        private string _ATDI_id_station = "";

        public int ATDI_id_permission
        {
            get { return _ATDI_permission_id; }
            set { _ATDI_permission_id = value; OnPropertyChanged("ATDI_permission_id"); }
        }
        private int _ATDI_permission_id = 0;

        public string ATDI_id_sector
        {
            get { return _ATDI_sector_id; }
            set { _ATDI_sector_id = value; OnPropertyChanged("ATDI_sector_id"); }
        }
        private string _ATDI_sector_id = "";

        public int ATDI_id_frequency
        {
            get { return _ATDI_Frequency_id; }
            set { _ATDI_Frequency_id = value; OnPropertyChanged("ATDI_Frequency_id"); }
        }
        private int _ATDI_Frequency_id = 0;

        public decimal ATDI_FrequencyPermission
        {
            get { return _ATDI_FrequencyPermission; }
            set { _ATDI_FrequencyPermission = value; OnPropertyChanged("ATDI_FrequencyPermission"); }
        }
        private decimal _ATDI_FrequencyPermission = 0;


        public decimal ATDI_Bandwidth
        {
            get { return _ATDI_Bandwidth; }
            set { _ATDI_Bandwidth = value; BandwidthComparison(); OnPropertyChanged("ATDI_Bandwidth"); }
        }
        private decimal _ATDI_Bandwidth = -1;

        /// <summary>
        /// результат сравнения полосы сигнала из идентификации и из разрешения
        /// \t 0 = не сравнивалось 1 = не совпадает 2 = совпадает
        /// </summary>
        public int ATDI_BandwidthComparisonResult
        {
            get { return _ATDI_BandwidthComparisonResult; }
            set { _ATDI_BandwidthComparisonResult = value; OnPropertyChanged("ATDI_BandwidthComparisonResult"); }
        }
        private int _ATDI_BandwidthComparisonResult = 0;

        private void BandwidthComparison()
        {
            if (ATDI_Bandwidth != -1 && Bandwidth != -1)
            {
                if (ATDI_Bandwidth == Bandwidth) ATDI_BandwidthComparisonResult = 2;
                if (ATDI_Bandwidth != Bandwidth) ATDI_BandwidthComparisonResult = 1;
            }
            else ATDI_BandwidthComparisonResult = 0;
        }

        /// <summary>
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_Identifier_Find
        {
            get { return _ATDI_Identifier_Find; }
            set { _ATDI_Identifier_Find = value; OnPropertyChanged("ATDI_Identifier_Find"); }
        }
        private int _ATDI_Identifier_Find = 0;
        /// <summary>
        /// если нашлась строка в плане то здесь отметится соответствие частоте
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_FreqCheck_Find
        {
            get { return _ATDI_FreqCheck_Find; }
            set { _ATDI_FreqCheck_Find = value; OnPropertyChanged("ATDI_FreqCheck_Find"); }
        }
        private int _ATDI_FreqCheck_Find = 0;

        public string ATDI_GCID
        {
            get { return _ATDI_GCID; }
            set { _ATDI_GCID = value; OnPropertyChanged("ATDI_GCID"); }
        }
        private string _ATDI_GCID = string.Empty;

        public bool LR_NewDataToSave
        {
            get { return _LR_NewDataToSave; }
            set { _LR_NewDataToSave = value; OnPropertyChanged("LR_NewDataToSave"); }
        }
        private bool _LR_NewDataToSave = false;
        #endregion
        /// <summary>
        /// пишет изфу в station_sys_info 
        /// </summary>
        public DB.localatdi_station_sys_info GetStationInfo()
        {
            return new DB.localatdi_station_sys_info()
            {
                information_blocks = _station_sys_info.information_blocks,
                bandwidth = Bandwidth,
                base_id = -1,
                bsic = -1,
                channel_number = EARFCN_DN,
                cid = -1,
                code_power = -1000,
                ctoi = -1000,
                eci = CelId28,
                e_node_b_id = eNodeBId,
                freq = FreqDn,
                icio = -1000,
                inband_power = InbandPower,
                iscp = -1000,
                lac = -1,
                location = new DB.localatdi_geo_location()
                {
                    asl = MainWindow.gps.Altitude,
                    latitude = (double)MainWindow.gps.LatitudeDecimal,
                    longitude = (double)MainWindow.gps.LongitudeDecimal,
                },// MainWindow.gps.location,
                mcc = MCC,
                mnc = MNC,
                nid = -1,
                pci = PCI,
                pn = -1,
                power = -1000,
                ptotal = -1000,
                rnc = -1,
                rscp = -1000,
                rsrp = RSRP,
                rsrq = RSRQ,
                sc = -1,
                sid = -1,
                tac = TAC,
                type_cdmaevdo = "",
                ucid = -1
            };
            //return ssi;
        }
        public DB.localatdi_station_sys_info station_sys_info
        {
            get { return _station_sys_info; }
            set { _station_sys_info = value; OnPropertyChanged("station_sys_info"); }
        }
        private DB.localatdi_station_sys_info _station_sys_info = new DB.localatdi_station_sys_info() { };

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<DB.localatdi_level_meas_result> level_results
        {
            get { return _level_results; }
            set { _level_results = value; OnPropertyChanged("level_results"); }
        }
        private ObservableCollection<DB.localatdi_level_meas_result> _level_results = new ObservableCollection<DB.localatdi_level_meas_result>() { };

        //public ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar> Atdi_LevelsMeasurementsCar
        //{
        //    get { return _Atdi_LevelsMeasurementsCar; }
        //    set { _Atdi_LevelsMeasurementsCar = value; OnPropertyChanged("Atdi_LevelsMeasurementsCar"); }
        //}
        //private ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar> _Atdi_LevelsMeasurementsCar = new ObservableCollection<DB.LacalAtdi_LevelMeasurementsCar>() { };



        //public ObservableRangeCollection<DB.local3GPPSystemInformationBlock> InformationBlocks
        //{
        //    get { return _InformationBlocks; }
        //    set { _InformationBlocks = value; OnPropertyChanged("InformationBlocks"); }
        //}
        //public ObservableRangeCollection<DB.local3GPPSystemInformationBlock> _InformationBlocks = new ObservableRangeCollection<DB.local3GPPSystemInformationBlock>() { };
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class ACDAllData : INotifyPropertyChanged
    {
        public bool TechIsEnabled
        {
            get { return App.Sett.TSMxReceiver_Settings.ACD.TechIsEnabled; }
            set { App.Sett.TSMxReceiver_Settings.ACD.TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        public bool IsRuning
        {
            get { return _IsRuning; }
            set { _IsRuning = value; OnPropertyChanged("IsRuning"); }
        }
        private static bool _IsRuning;

        public ObservableCollection<ACD_Data> ACDData
        {
            get { return _ACDData; }
            set { _ACDData = value; OnPropertyChanged("ACDData"); }
        }
        private ObservableCollection<ACD_Data> _ACDData = new ObservableCollection<ACD_Data> { };

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ACD_Data : INotifyPropertyChanged
    {
        public int Band
        {
            get { return _Band; }
            set { _Band = value; OnPropertyChanged("Band"); }
        }
        private int _Band = -1;

        public int Tech
        {
            get { return _Tech; }
            set { _Tech = value; OnPropertyChanged("Tech"); }
        }
        private int _Tech = 0;

        public string TechStr
        {
            get { return _TechStr; }
            set { _TechStr = value; OnPropertyChanged("TechStr"); }
        }
        private string _TechStr = "";

        public decimal Freq
        {
            get { return _Freq; }
            set { _Freq = value; OnPropertyChanged("Freq"); }
        }
        private decimal _Freq = 0;

        public decimal BW
        {
            get { return _BW; }
            set { _BW = value; OnPropertyChanged("BW"); }
        }
        private decimal _BW = 0;

        public decimal RSSI
        {
            get { return _RSSI; }
            set { _RSSI = value; OnPropertyChanged("RSSI"); }
        }
        private decimal _RSSI = 0;

        public int MCC
        {
            get { return _MCC; }
            set { _MCC = value; OnPropertyChanged("MCC"); }
        }
        private int _MCC = -1;

        public int MNC
        {
            get { return _MNC; }
            set { _MNC = value; OnPropertyChanged("MNC"); }
        }
        private int _MNC = -1;

        public bool Established
        {
            get { return _Established; }
            set { _Established = value; OnPropertyChanged("Established"); }
        }
        private bool _Established = false;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class UHFBTSData : INotifyPropertyChanged
    {
        #region 

        private decimal _FreqDn = 0;
        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private decimal _FreqUp = 0;
        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }


        private uint _Detected;
        public uint Detected
        {
            get { return _Detected; }
            set { _Detected = value; OnPropertyChanged("Detected"); }
        }

        private string _CallSign = "";
        public string CallSign
        {
            get { return _CallSign; }
            set { _CallSign = value; OnPropertyChanged("CallSign"); }
        }
        private decimal _Power;
        public decimal Power
        {
            get { return _Power; }
            set { _Power = value; OnPropertyChanged("Power"); }
        }
        private bool _FullData;
        public bool FullData
        {
            get { return _FullData; }
            set { _FullData = value; OnPropertyChanged("FullData"); }
        }
        private bool _Find;
        public bool Find
        {
            get { return _Find; }
            set { _Find = value; OnPropertyChanged("Find"); }
        }


        /// <summary>
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int Identifier_Find
        {
            get { return _Identifier_Find; }
            set { _Identifier_Find = value; OnPropertyChanged("Identifier_Find"); }
        }
        private int _Identifier_Find = 0;


        /// <summary>
        /// PlanFreq_ID
        /// -1 не найдено
        /// </summary>
        public int PlanFreq_ID
        {
            get { return _PlanFreq_ID; }
            set { _PlanFreq_ID = value; OnPropertyChanged("PlanFreq_ID"); }
        }
        private int _PlanFreq_ID = -1;
        /// <summary>
        /// Plan_ID
        /// -1 не найдено
        /// </summary>
        public int Plan_ID
        {
            get { return _Plan_ID; }
            set { _Plan_ID = value; OnPropertyChanged("Plan_ID"); }
        }
        private int _Plan_ID = -1;
        /// <summary>
        /// если нашлась строка в плане то здесь отметится соответствие частоте
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int FreqCheck_Find
        {
            get { return _FreqCheck_Find; }
            set { _FreqCheck_Find = value; OnPropertyChanged("FreqCheck_Find"); }
        }
        private int _FreqCheck_Find = 0;
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
