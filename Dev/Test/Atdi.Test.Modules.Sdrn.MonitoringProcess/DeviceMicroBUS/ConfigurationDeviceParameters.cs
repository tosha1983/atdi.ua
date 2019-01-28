using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Modules.Sdrn.MonitoringProcess.DeviceMicroBUS
{
    #region input
    class ConfigurationDeviceParameters
    {
        double RBW_Hz; // -1 = auto; -2 отсутсвует, 0.0 до 1000000000
        double VBW_Hz; // -1 = auto; -2 отсутсвует, 0.0 до 1000000000
        decimal FreqStart_Hz; //
        decimal FreqStop_Hz; //
        double SweepTime_s; //-1 = auto
        int Att_dB; //-1 = auto, -2 отсутсвует.
        int PreAmp_dB; //-1 = auto, -2 отсутсвует, 0 = выкл, 1 вкл, больще 1 как есть.
        int RefLevel_dBm; // -1 = default
        DetectorType DetectorType1; // зависит от методики измерений
        DetectorType DetectorType2; 
        DetectorType DetectorType3;
        TraceType traceType1; // зависит от методики измерений
        TraceType traceType2;
        TraceType traceType3;
        TraceMode traceMode;
        int AverageTraceCount; // количество трейсов
        int TracePoint; // количество точек 
        SweepType SweepType; 
        RecieverMode RecieverMode;
        MeasMode MeasMode;
        LevelUnit LevelUnit;
    }
    class ConfigurationDeviceParametersForDF: ConfigurationDeviceParameters
    {
        decimal DFBW_Hz;
        int DFSQUMode; //0 = off, 1 - Normal, 2 - Gate
        double SQULevel_dBm;
        double DFQuality; // 0-100
    }
    class ConfigurationDeviceParametersForTrece : ConfigurationDeviceParameters
    {

    }
    class ConfigurationDeviceParametersForRecievSysData 
    {
        string Technology; // GSM, UMTS, CDMA, EVDO, LTE, WiFi, WiMax, Tetra, NB-Iot
        decimal[] Freqs_Hz;
        string[] bloks; //
        decimal MinBW; // минимальная ширина спестрк сигнала важно для LTE
    }
    class ConfigurationDeviceParametersForAudio
    {
        decimal DemodBW_Hz; // минимальная ширина спестрк сигнала важно для LTE
        string TypeModulatio; //"АМ, ФМ, ....."
    }
    class ConfigurationDeviceParametersForChPowAndBW: ConfigurationDeviceParameters
    {
        decimal FreqCentral;
        decimal BW;
    }
    class ConfigurationDeviceParametersForZeroSpan: ConfigurationDeviceParameters
    {
        double DeltaTime_s;
    }

    enum DetectorType
    {
        Auto, // убрать херня

        AutoSelect, // AN
        AutoPeak, // AN
        MaxPeak, // AN, TSMX
        MinPeak, // AN, TSMX
        Average, // AN, PR
        RMS, // AN, PR, TSMX
        Semple, // AN
        Fast, // PR
        Peak, // PR
        Normal // AN (Kesyte, Anr)
    }
    enum TraceType
    {
        Auto, // убрать херня
        ClearWhrite, //AN, PR
        Average, //AN, PR
        MaxHold, //AN, PR
        MinHold, //AN, PR
        Blank,  //AN, PR
        View, //AN, PR
        Treking // Женя.
    }
    enum TraceMode
    {
        ByDevice,
        BySoft
    }
    enum SweepType
    {
        Auto,
        Sweep,
        FFT
    }
    enum RecieverMode
    {
        FFM,
        PSCAN
    }
    enum MeasMode
    {
        Periodic,
        Continuos
    }
    public enum TypeGSM
    {
        UNKNOWN = -1,
        SITYPE_13 = 0,
        SITYPE_2_BIS = 2,
        SITYPE_2_TER = 3,
        SITYPE_9 = 4,
        SITYPE_2_QUATER = 7,
        SITYPE_8 = 24,
        SITYPE_1 = 25,
        SITYPE_2 = 26,
        SITYPE_3 = 27,
        SITYPE_4 = 28,
        SITYPE_7 = 31,
        SITYPE_16 = 61,
        SITYPE_17 = 62,
        SITYPE_18 = 64,
        SITYPE_19 = 65,
        SITYPE_20 = 66,
        SITYPE_15 = 67,
        SITYPE_13_ALT = 68,
        SITYPE_2_N = 69,
        SITYPE_21 = 70,
        SITYPE_22 = 71
    }
    public enum TypeUMTS
    {
        UNKNOWN = -1,
        UNDEFINED = 0,
        MIB = 14,
        SIB1 = 15,
        SIB2 = 16,
        SIB3 = 17,
        SIB5 = 19,
        SIB7 = 21,
        SIB11 = 25,
        SIB12 = 26,
        SIB13 = 27,
        SIB13_1 = 28,
        SIB13_2 = 29,
        SIB13_3 = 30,
        SIB13_4 = 31,
        SIB15 = 33,
        SIB15_1 = 34,
        SIB15_2 = 35,
        SIB15_3 = 36,
        SIB15_4 = 37,
        SIB15_5 = 38,
        SIB16 = 39,
        SIB18 = 41,
        SB1 = 42,
        SB2 = 43,
        SIB23 = 46,
        SIB24 = 47,
        SIB5bis = 53,
        SIB11ter = 60,
        SIB11bis = 61,
        SIB15_bis = 67,
        SIB15_1bis = 68,
        SIB15_2bis = 69,
        SIB15_2ter = 70,
        SIB15_3bis = 71,
        SIB15_6 = 72,
        SIB15_7 = 73,
        SIB15_8 = 74,
        SIB19 = 75,
        SIB20 = 76
    }
    public enum TypeLTE
    {
        UNDEFINED = 0,
        MIB = 10,
        SIB1 = 11,
        SIB2 = 12,
        SIB3 = 13,
        SIB4 = 14,
        SIB5 = 15,

        SIB6 = 16,
        SIB7 = 17,
        SIB8 = 18,
        SIB9 = 19,
        SIB10 = 20,
        SIB11 = 21,
        SIB12 = 22,
        SIB13 = 23,
        SIB14 = 24,
        SIB15 = 25,
        SIB16 = 26,
        SIB17 = 27,
        SIB18 = 28,
        SIB19 = 29
    }
    public enum TypeCDMAEVDO
    {
        UNKNOWN = -1,
        NONE = 0,
        SYS_PARAMS = 1,
        EXT_SYS_PARAMS = 2,
        CHAN_LIST = 3,
        EXT_CHAN_LIST = 4,
        NEIGHBOR_LIST = 5,
        EXT_NEIGHBOR_LIST = 6,
        GEN_NEIGHBOR_LIST = 7,
        GLOBAL_SERV_DIR = 8,
        EXT_GLOBAL_SERV_RE = 9,
        ACCESS_PARAMETERS = 10,
        ATIM_MESSAGE = 56,
        SYNC_MESSAGE = 101,
        EVDO_QUICK_CONFIG = 201,
        EVDO_SYNC = 202,
        EVDO_SECTOR_PARAMETERS = 203,
        EVDO_ACCESS_PARAMETERS = 204
    }
    enum LevelUnit // Базовый класс ВЕЗДЕ
    {
        dBm,
        dBmkV,
        dBmkVm,
        mkV,
    }

    #endregion
    #region output
    class Trace
    {
        TracePoint[] Points;
        long TimeStamp; //Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
        // Состояние о получении данных ???? 
    }
    class ZeroSpan
    {
        TimePoint[] Points;
        long TimeStamp; //Тики относительно new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
    }
    class ChannelPower
    {
        double Result; // мощность в канале в юнитах
        decimal BW_Hz; // полоса сигнала который мы измерели
        decimal Freq_Hz; // Центральная частота
        // Состояние о получении данных ???? 
    }
    class DeviceBW
    {
        decimal BWResult_Hz;
        int T1; // индекс на trace (левый)
        int T2; // индекс на trace (правый)
        int M1; // индекс на trace (центральный)
        TypeEstimationBW TypeEstimationBW; 
    }
    class SignalMod
    {
        string ModType;
    }
    class Audio
    {
        string AudioData;
    }
    class InndificationData
    {

    }
    enum TypeEstimationBW
    {
        beta,
        NdB
    }
    class TracePoint
    {
        decimal Freq_Hz;
        float Level; 
    }
    class TimePoint
    {
        decimal Time_s;
        float Level;
    }
    public class GSMBTSData
    {
        #region 

        public uint FreqIndex
        {
            get { return _FreqIndex; }
            set { _FreqIndex = value; /*OnPropertyChanged("FreqIndex");*/ }
        }
        private uint _FreqIndex = 0;

        #region GSM_Channel
        public int ARFCN // Номер канала Женя вычисляет
        {
            get { return _ARFCN; }
            set { _ARFCN = value;/* OnPropertyChanged("ARFCN");*/ }
        }
        private int _ARFCN = 0;

        public decimal FreqUp // Частота на канале UL Женя вычисляет
        {
            get { return _FreqUp; }
            set { _FreqUp = value; /*OnPropertyChanged("FreqUp");*/ }
        }
        private decimal _FreqUp = 0;

        public decimal FreqDn // Частота на канале DL Женя вычисляет
        {
            get { return _FreqDn; }
            set { _FreqDn = value; /*OnPropertyChanged("FreqDn");*/ }
        }
        private decimal _FreqDn = 0;

        public string StandartSubband // Вычисляет Женя по частоте
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; /*OnPropertyChanged("StandartSubband");*/ }
        }
        private string _StandartSubband = "";

        #endregion


        public uint IndSCHInfo // c TSMX разделяет потоки данных на 1 частоте.
        {
            get { return _IndSCHInfo; }
            set { _IndSCHInfo = value; }
        }
        private uint _IndSCHInfo;

        public uint IndFirstSCHInfo // c TSMX разделяет потоки данных на 1 частоте.
        {
            get { return _IndFirstSCHInfo; }
            set { _IndFirstSCHInfo = value; }
        }
        private uint _IndFirstSCHInfo;


        public int BSIC // c TSMX код идентификации БС BCC + NCC уникальное в рамках 
        {
            get { return _BSIC; }
            set { _BSIC = value; /*OnPropertyChanged("BSIC");*/ }
        }
        private int _BSIC = -1;

        public int MCC // с TSMX код страны
        {
            get { return _MCC; }
            set { _MCC = value; /*OnPropertyChanged("MCC");*/ }
        }
        private int _MCC = -1;

        public int MNC // с TSMX код оператора
        {
            get { return _MNC; }
            set { _MNC = value; /*OnPropertyChanged("MNC");*/ }
        }
        private int _MNC = -1;

        public int LAC // с TSMX код региона
        {
            get { return _LAC; }
            set { _LAC = value; /*OnPropertyChanged("LAC");*/ }
        }
        private int _LAC = -1;

        public int CID // с TSMX cell ID
        {
            get { return _CID; }
            set { _CID = value; /*OnPropertyChanged("CID");*/ }
        }
        private int _CID = -1;


        public string GCID  // с TSMX Global CID
        {
            get { return _GCID; }
            set { _GCID = value; /*OnPropertyChanged("GCID");*/ }
        }
        private string _GCID = "";

        public int CIDToDB // Женя притягивает к БД ICSM
        {
            get { return _CIDToDB; }
            set { _CIDToDB = value; /*OnPropertyChanged("CIDToDB");*/ }
        }
        private int _CIDToDB = -1;
        #region SectorID
        public int SectorIDFromID 
        {
            get { return _SectorIDFromID; }
            set { _SectorIDFromID = value; /*OnPropertyChanged("SectorIDFromID");*/ }
        }
        private int _SectorIDFromID = -1;

        public int SectorIDFromIdent  
        {
            get { return _SectorIDFromIdent; }
            set { _SectorIDFromIdent = value; /*OnPropertyChanged("SectorIDFromIdent");*/ }
        }
        private int _SectorIDFromIdent = -1;

        public int SectorID 
        {
            get { return _SectorID; }
            set { _SectorID = value; /*OnPropertyChanged("SectorID");*/ }
        }
        private int _SectorID = -1;
        #endregion
        public double Power //dBm -> LevelCar
        {
            get { return _Power; }
            set { _Power = value; /*OnPropertyChanged("Power");*/ }
        }
        private double _Power;


        public double Strenght// напряженность этого сигнала
        {
            get { return _Strenght; }
            set { _Strenght = value; /*OnPropertyChanged("Strenght");*/ }
        }
        private double _Strenght = -1000;

        /// <summary>
        /// Carrier-to-Interference 
        /// </summary>
        public double CarToInt
        {
            get { return _CarToInt; }
            set { _CarToInt = value; /*OnPropertyChanged("CarToInt");*/ }
        }
        private double _CarToInt = 0;

        public bool FullData // наличие всех даных 
        {
            get { return _FullData; }
            set { _FullData = value; /*OnPropertyChanged("FullData");*/ }
        }
        private bool _FullData;

 
        public DateTime LastLevelUpdete // Время последнего уровня.
        {
            get { return _LastLevelUpdete; }
            set { _LastLevelUpdete = value; /*OnPropertyChanged("LastLevelUpdete");*/ }
        }
        private DateTime _LastLevelUpdete = DateTime.MinValue;

        public long LastDetectionLevelUpdete // Время последнего уровня + больше порога обнаружения 
        {
            get { return _LastDetectionLevelUpdete; }
            set { _LastDetectionLevelUpdete = value; /*OnPropertyChanged("LastDetectionLevelUpdete");*/ }
        }
        private long _LastDetectionLevelUpdete = 0;

        public bool ThisIsMaximumSignalAtThisFrequency 
        {
            get { return _ThisIsMaximumSignalAtThisFrequency; }
            set { _ThisIsMaximumSignalAtThisFrequency = value; /*OnPropertyChanged("ThisIsMaximumSignalAtThisFrequency");*/ }
        }
        private bool _ThisIsMaximumSignalAtThisFrequency = false;

        public bool DeleteFromMeasMon // признак прекращения измерения (уехал)
        {
            get { return _DeleteFromMeasMon; }
            set { _DeleteFromMeasMon = value; /*OnPropertyChanged("DeleteFromMeasMon");*/ }
        }
        private bool _DeleteFromMeasMon = false;
        #endregion
        #region ATDI
        public string ATDI_id_task
        {
            get { return _ATDI_id_task; }
            set { _ATDI_id_task = value; /*OnPropertyChanged("ATDI_id_task");*/ }
        }
        private string _ATDI_id_task = "";

        public string ATDI_id_station
        {
            get { return _ATDI_id_station; }
            set { _ATDI_id_station = value; /*OnPropertyChanged("ATDI_id_station");*/ }
        }
        private string _ATDI_id_station = "";

        public int ATDI_id_permission
        {
            get { return _ATDI_permission_id; }
            set { _ATDI_permission_id = value; /*OnPropertyChanged("ATDI_permission_id");*/ }
        }
        private int _ATDI_permission_id = 0;

        public string ATDI_id_sector
        {
            get { return _ATDI_sector_id; }
            set { _ATDI_sector_id = value; /*OnPropertyChanged("ATDI_sector_id");*/ }
        }
        private string _ATDI_sector_id = "";

        public int ATDI_id_frequency
        {
            get { return _ATDI_Frequency_id; }
            set { _ATDI_Frequency_id = value; /*OnPropertyChanged("ATDI_Frequency_id");*/ }
        }
        private int _ATDI_Frequency_id = 0;

        public decimal ATDI_FrequencyPermission
        {
            get { return _ATDI_FrequencyPermission; }
            set { _ATDI_FrequencyPermission = value; /*OnPropertyChanged("ATDI_FrequencyPermission");*/ }
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
            set { _ATDI_Identifier_Find = value; /*OnPropertyChanged("ATDI_Identifier_Find");*/ }
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
            set { _ATDI_FreqCheck_Find = value; /*OnPropertyChanged("ATDI_FreqCheck_Find");*/ }
        }
        private int _ATDI_FreqCheck_Find = 0;

        public string ATDI_GCID
        {
            get { return _ATDI_GCID; }
            set { _ATDI_GCID = value; /*OnPropertyChanged("ATDI_GCID"); */}
        }
        private string _ATDI_GCID = string.Empty;

        public bool LR_NewDataToSave
        {
            get { return _LR_NewDataToSave; }
            set { _LR_NewDataToSave = value; /*OnPropertyChanged("LR_NewDataToSave"); */}
        }
        private bool _LR_NewDataToSave = false;

        /// <summary>
        /// пишет инфу в station_sys_info 
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
            set { _station_sys_info = value; /*OnPropertyChanged("station_sys_info");*/ }
        }
        private DB.localatdi_station_sys_info _station_sys_info = new DB.localatdi_station_sys_info() { };

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<DB.localatdi_level_meas_result> level_results
        {
            get { return _level_results; }
            set { _level_results = value; /*OnPropertyChanged("level_results");*/ }
        }
        private ObservableCollection<DB.localatdi_level_meas_result> _level_results = new ObservableCollection<DB.localatdi_level_meas_result>() { };
        #endregion
    }
    class UMTSBTSData
    {
        #region 
        public uint FreqIndex
        {
            get { return _FreqIndex; }
            set { _FreqIndex = value; /*OnPropertyChanged("FreqIndex");*/ }
        }
        private uint _FreqIndex = uint.MaxValue;

        #region UMTS_Channel
        public int UARFCN_DN
        {
            get { return _UARFCN_DN; }
            set { _UARFCN_DN = value; /*OnPropertyChanged("UARFCN_DN");*/ }
        }
        private int _UARFCN_DN = 0;

        public int UARFCN_UP
        {
            get { return _UARFCN_UP; }
            set { _UARFCN_UP = value; /*OnPropertyChanged("UARFCN_UP");*/ }
        }
        private int _UARFCN_UP = 0;

        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; /*OnPropertyChanged("FreqUp");*/ }
        }
        private decimal _FreqUp = 0;

        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; /*OnPropertyChanged("FreqDn");*/ }
        }
        private decimal _FreqDn = 0;

        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; /*OnPropertyChanged("StandartSubband");*/ }
        }
        private string _StandartSubband = "";
        #endregion

      

        public int SC // сркемлинг кода
        {
            get { return _SC; }
            set { _SC = value; /*OnPropertyChanged("SC");*/ }
        }
        private int _SC = -1;

        public int MCC 
        {
            get { return _MCC; }
            set { _MCC = value; /*OnPropertyChanged("MCC");*/ }
        }
        private int _MCC = -1;

        public int MNC 
        {
            get { return _MNC; }
            set { _MNC = value;/* OnPropertyChanged("MNC");*/ }
        }
        private int _MNC = -1;

        public int LAC
        {
            get { return _LAC; }
            set { _LAC = value; /*OnPropertyChanged("LAC");*/ }
        }
        private int _LAC = -1;

        public int UCID // UTRAN cell ID - > RNC + CID
        {
            get { return _UCID; }
            set { _UCID = value; /*OnPropertyChanged("UCID");*/ }
        }
        private int _UCID = -1;

        public int RNC // Radionetwork Controler ID
        {
            get { return _RNC; }
            set { _RNC = value; /*OnPropertyChanged("RNC");*/ }
        }
        private int _RNC = -1;

        public int CID
        {
            get { return _CID; }
            set { _CID = value; /*OnPropertyChanged("CID");*/ }
        }
        private int _CID = -1;

        public int SectorFromCID
        {
            get { return _SectorFromCID; }
            set { _SectorFromCID = value; /*OnPropertyChanged("SectorFromCID");*/ }
        }
        private int _SectorFromCID;

        public int CarrierFromCID // номер несущей из идентификатора
        {
            get { return _CarrierFromCID; }
            set { _CarrierFromCID = value; /*OnPropertyChanged("CarrierFromCID");*/ }
        }
        private int _CarrierFromCID;

        public string GCID
        {
            get { return _GCID; }
            set { _GCID = value; /*OnPropertyChanged("GCID");*/ }
        }
        private string _GCID = "";

        public int CIDToDB
        {
            get { return _CIDToDB; }
            set { _CIDToDB = value; /*OnPropertyChanged("CIDToDB");*/ }
        }
        private int _CIDToDB = -1;

        public int SectorIDToDB
        {
            get { return _SectorIDToDB; }
            set { _SectorIDToDB = value; /*OnPropertyChanged("SectorIDToDB");*/ }
        }
        private int _SectorIDToDB = -1;

        public double ISCP // c TSMX интегральный уровень сигнала
        {
            get { return _ISCP; }
            set { _ISCP = value; /*OnPropertyChanged("ISCP");*/ }
        }
        private double _ISCP;

        public double RSCP // c TSMX  уровень сигнала -> LevelCar
        {
            get { return _RSCP; }
            set { _RSCP = value; /*OnPropertyChanged("RSCP");*/ }
        }
        private double _RSCP = -1000;

        /// <summary>
        /// напряженность этого сигнала
        /// </summary>
        public double Strenght
        {
            get { return _Strenght; }
            set { _Strenght = value; /*OnPropertyChanged("Strenght");*/ }
        }
        private double _Strenght = -1000;

        public double InbandPower // P total
        {
            get { return _InbandPower; }
            set { _InbandPower = value; /*OnPropertyChanged("InbandPower");*/ }
        }
        private double _InbandPower = -1000;

        public double CodePower //TSMX  уровень сигнала 
        {
            get { return _CodePower; }
            set { _CodePower = value; /*OnPropertyChanged("CodePower");*/ }
        }
        private double _CodePower = -1000;

        public double IcIo // TSMX  C/I 
        {
            get { return _IcIo; }
            set { _IcIo = value; /*OnPropertyChanged("IcIo");*/ }
        }
        private double _IcIo = -1000;

        /// <summary>
        /// определяет есть ли GCID
        /// </summary>
        public bool FullData
        {
            get { return _FullData; }
            set { _FullData = value; /*OnPropertyChanged("FullData");*/ }
        }
        private bool _FullData = false;

        public DateTime LastLevelUpdete
        {
            get { return _LastLevelUpdete; }
            set { _LastLevelUpdete = value; /*OnPropertyChanged("LastLevelUpdete");*/ }
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
            set { _ThisIsMaximumSignalAtThisFrequency = value; /*OnPropertyChanged("ThisIsMaximumSignalAtThisFrequency");*/ }
        }
        private bool _ThisIsMaximumSignalAtThisFrequency = false;

        public bool DeleteFromMeasMon
        {
            get { return _DeleteFromMeasMon; }
            set { _DeleteFromMeasMon = value; /*OnPropertyChanged("DeleteFromMeasMon");*/ }
        }
        private bool _DeleteFromMeasMon = false;
        #endregion

        #region ATDI
        public string ATDI_id_task
        {
            get { return _ATDI_id_task; }
            set { _ATDI_id_task = value; /*OnPropertyChanged("ATDI_id_task");*/ }
        }
        private string _ATDI_id_task = "";

        public string ATDI_id_station
        {
            get { return _ATDI_id_station; }
            set { _ATDI_id_station = value; /*OnPropertyChanged("ATDI_id_station");*/ }
        }
        private string _ATDI_id_station = "";

        public int ATDI_id_permission
        {
            get { return _ATDI_permission_id; }
            set { _ATDI_permission_id = value; /*OnPropertyChanged("ATDI_permission_id");*/ }
        }
        private int _ATDI_permission_id = 0;

        public string ATDI_id_sector
        {
            get { return _ATDI_sector_id; }
            set { _ATDI_sector_id = value; /*OnPropertyChanged("ATDI_sector_id");*/ }
        }
        private string _ATDI_sector_id = "";

        public int ATDI_id_frequency
        {
            get { return _ATDI_Frequency_id; }
            set { _ATDI_Frequency_id = value; /*OnPropertyChanged("ATDI_Frequency_id");*/ }
        }
        private int _ATDI_Frequency_id = 0;

        public decimal ATDI_FrequencyPermission
        {
            get { return _ATDI_FrequencyPermission; }
            set { _ATDI_FrequencyPermission = value; /*OnPropertyChanged("ATDI_FrequencyPermission");*/ }
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
            set { _ATDI_Identifier_Find = value; /*OnPropertyChanged("ATDI_Identifier_Find");*/ }
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
            set { _ATDI_FreqCheck_Find = value; /*OnPropertyChanged("ATDI_FreqCheck_Find");*/ }
        }
        private int _ATDI_FreqCheck_Find = 0;

        public string ATDI_GCID
        {
            get { return _ATDI_GCID; }
            set { _ATDI_GCID = value; /*OnPropertyChanged("ATDI_GCID");*/ }
        }
        private string _ATDI_GCID = string.Empty;

        public bool LR_NewDataToSave
        {
            get { return _LR_NewDataToSave; }
            set { _LR_NewDataToSave = value; /*OnPropertyChanged("LR_NewDataToSave");*/ }
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
            set { _station_sys_info = value; /*OnPropertyChanged("station_sys_info");*/ }
        }
        private DB.localatdi_station_sys_info _station_sys_info = new DB.localatdi_station_sys_info() { };

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<DB.localatdi_level_meas_result> level_results
        {
            get { return _level_results; }
            set { _level_results = value; /*OnPropertyChanged("level_results");*/ }
        }
        private ObservableCollection<DB.localatdi_level_meas_result> _level_results = new ObservableCollection<DB.localatdi_level_meas_result>() { };

    }
    public class CDMABTSData
    {
        #region


        private uint _FreqIndex;
        public uint FreqIndex
        {
            get { return _FreqIndex; }
            set { _FreqIndex = value; /*OnPropertyChanged("FreqIndex");*/ }
        }
        private uint _Indicator;
        public uint Indicator
        {
            get { return _Indicator; }
            set { _Indicator = value; /*OnPropertyChanged("Indicator");*/ }
        }
        #region CDMA_Channel
        public int ChannelN
        {
            get { return _ChannelN; }
            set { _ChannelN = value; /*OnPropertyChanged("ChannelN"); */}
        }
        private int _ChannelN = 0;

        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; /*OnPropertyChanged("FreqDn"); */}
        }
        private decimal _FreqDn = 0;

        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; /*OnPropertyChanged("FreqUp");*/ }
        }
        private decimal _FreqUp = 0;

        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; /*OnPropertyChanged("StandartSubband");*/ }
        }
        private string _StandartSubband = "";

        #endregion




        /// <summary>
        /// true = EVDO false = CDMAO
        /// </summary>
        public bool Type 
        {
            get { return _Type; }
            set { _Type = value;/* OnPropertyChanged("Type");*/ }
        }
        private bool _Type = false;

        private int _PN;
        public int PN // Пилот намбер
        {
            get { return _PN; }
            set { _PN = value; /*OnPropertyChanged("PN");*/ }
        }
        private int _SID = -1;
        public int SID // оператор id 
        {
            get { return _SID; }
            set { _SID = value; /*OnPropertyChanged("SID");*/ }
        }
        private int _NID = -1;
        public int NID // netwokr ID
        {
            get { return _NID; }
            set { _NID = value; /*OnPropertyChanged("NID");*/ }
        }

        public int MCC
        {
            get { return _MCC; }
            set { _MCC = value; /*OnPropertyChanged("MCC");*/ }
        }
        private int _MCC = -1;

        public int MNC
        {
            get { return _MNC; }
            set { _MNC = value; /*OnPropertyChanged("MNC");*/ }
        }
        private int _MNC = -1;


        public int BaseID //Идентификатор БС
        {
            get { return _BaseID; }
            set { _BaseID = value; /*OnPropertyChanged("BaseID");*/ }
        }
        private int _BaseID = -1;

        public string GCID
        {
            get { return _GCID; }
            set { _GCID = value; /*OnPropertyChanged("GCID");*/ }
        }
        private string _GCID = "";

        public int CIDToDB
        {
            get { return _CIDToDB; }
            set { _CIDToDB = value; /*OnPropertyChanged("CIDToDB");*/ }
        }
        private int _CIDToDB = -1;


        public double RSCP // Уровень в LevelCar 
        {
            get { return _RSCP; }
            set { _RSCP = value; /*OnPropertyChanged("RSCP");*/ }
        }
        private double _RSCP = -1000;
        /// <summary>
        /// напряженность этого сигнала
        /// </summary>
        public double Strenght
        {
            get { return _Strenght; }
            set { _Strenght = value; /*OnPropertyChanged("Strenght");*/ }
        }
        private double _Strenght = -1000;


        public double PTotal //
        {
            get { return _PTotal; }
            set { _PTotal = value; /*OnPropertyChanged("PTotal");*/ }
        }
        private double _PTotal = -1000;

        public double AverageInbandPower //
        {
            get { return _AverageInbandPower; }
            set { _AverageInbandPower = value; /*OnPropertyChanged("AverageInbandPower");*/ }
        }
        private double _AverageInbandPower = -1000;

        public double IcIo
        {
            get { return _IcIo; }
            set { _IcIo = value; /*OnPropertyChanged("IcIo");*/ }
        }
        private double _IcIo = 0;


        private bool _FullData;
        public bool FullData
        {
            get { return _FullData; }
            set { _FullData = value; /*OnPropertyChanged("FullData"); */}
        }
        private bool _Find;
        public bool Find
        {
            get { return _Find; }
            set { _Find = value; /*OnPropertyChanged("Find");*/ }
        }

        public DateTime LastLevelUpdete
        {
            get { return _LastLevelUpdete; }
            set { _LastLevelUpdete = value; /*OnPropertyChanged("LastLevelUpdete");*/ }
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
            set { _ThisIsMaximumSignalAtThisFrequency = value; /*OnPropertyChanged("ThisIsMaximumSignalAtThisFrequency");*/ }
        }
        private bool _ThisIsMaximumSignalAtThisFrequency = false;

        public bool DeleteFromMeasMon
        {
            get { return _DeleteFromMeasMon; }
            set { _DeleteFromMeasMon = value; /*OnPropertyChanged("DeleteFromMeasMon");*/ }
        }
        private bool _DeleteFromMeasMon = false;


        #endregion

        #region ATDI
        public string ATDI_id_task
        {
            get { return _ATDI_id_task; }
            set { _ATDI_id_task = value; /*OnPropertyChanged("ATDI_id_task");*/ }
        }
        private string _ATDI_id_task = "";

        public string ATDI_id_station
        {
            get { return _ATDI_id_station; }
            set { _ATDI_id_station = value; /*OnPropertyChanged("ATDI_id_station");*/ }
        }
        private string _ATDI_id_station = "";

        public int ATDI_id_permission
        {
            get { return _ATDI_permission_id; }
            set { _ATDI_permission_id = value; /*OnPropertyChanged("ATDI_permission_id");*/ }
        }
        private int _ATDI_permission_id = 0;

        public string ATDI_id_sector
        {
            get { return _ATDI_sector_id; }
            set { _ATDI_sector_id = value; /*OnPropertyChanged("ATDI_sector_id");*/ }
        }
        private string _ATDI_sector_id = "";

        public int ATDI_id_frequency
        {
            get { return _ATDI_Frequency_id; }
            set { _ATDI_Frequency_id = value; /*OnPropertyChanged("ATDI_Frequency_id");*/ }
        }
        private int _ATDI_Frequency_id = 0;

        public decimal ATDI_FrequencyPermission
        {
            get { return _ATDI_FrequencyPermission; }
            set { _ATDI_FrequencyPermission = value; /*OnPropertyChanged("ATDI_FrequencyPermission");*/ }
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
            set { _ATDI_Identifier_Find = value; /*OnPropertyChanged("ATDI_Identifier_Find");*/ }
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
            set { _ATDI_FreqCheck_Find = value; /*OnPropertyChanged("ATDI_FreqCheck_Find");*/ }
        }
        private int _ATDI_FreqCheck_Find = 0;

        public string ATDI_GCID
        {
            get { return _ATDI_GCID; }
            set { _ATDI_GCID = value; /*OnPropertyChanged("ATDI_GCID");*/ }
        }
        private string _ATDI_GCID = string.Empty;

        public bool LR_NewDataToSave
        {
            get { return _LR_NewDataToSave; }
            set { _LR_NewDataToSave = value; /*OnPropertyChanged("LR_NewDataToSave"); */}
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
            set { _ATDI_IDFrom = value; /*OnPropertyChanged("ATDI_IDFrom");*/ }
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
            set { _station_sys_info = value; /*OnPropertyChanged("station_sys_info");*/ }
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


    }
    public class LTEBTSData
    {
        #region 
        public uint FreqIndex
        {
            get { return _FreqIndex; }
            set { _FreqIndex = value; /*OnPropertyChanged("FreqIndex");*/ }
        }
        private uint _FreqIndex = uint.MaxValue;

        public uint FreqBtsIndex // на одной частоте
        {
            get { return _FreqBtsIndex; }
            set { _FreqBtsIndex = value; /*OnPropertyChanged("FreqBtsIndex");*/ }
        }
        private uint _FreqBtsIndex = uint.MaxValue;

        #region LTE_Channel
        public int EARFCN_DN
        {
            get { return _EARFCN_DN; }
            set { _EARFCN_DN = value; /*OnPropertyChanged("EARFCN_DN");*/ }
        }
        private int _EARFCN_DN = 0;

        public int EARFCN_UP
        {
            get { return _EARFCN_UP; }
            set { _EARFCN_UP = value; /*OnPropertyChanged("EARFCN_UP");*/ }
        }
        private int _EARFCN_UP = 0;

        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; /*OnPropertyChanged("FreqUp");*/ }
        }
        private decimal _FreqUp = 0;

        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; /*OnPropertyChanged("FreqDn");*/ }
        }
        private decimal _FreqDn = 0;


        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; /*OnPropertyChanged("StandartSubband");*/ }
        }
        private string _StandartSubband = "";

        #endregion

        public decimal Bandwidth
        {
            get { return _Bandwidth; }
            set { _Bandwidth = value; BandwidthComparison(); /*OnPropertyChanged("Bandwidth");*/ }
        }
        private decimal _Bandwidth = -1;

        public int MCC
        {
            get { return _MCC; }
            set { _MCC = value; /*OnPropertyChanged("MCC");*/ }
        }
        private int _MCC = -1;

        public int MNC
        {
            get { return _MNC; }
            set { _MNC = value; /*OnPropertyChanged("MNC");*/ }
        }
        private int _MNC = -1;

        public int TAC // trcing area code (LAC)
        {
            get { return _TAC; }
            set { _TAC = value; /*OnPropertyChanged("TAC");*/ }
        }
        private int _TAC = -1;

        public int CelId28 // спец формат 
        {
            get { return _CelId28; }
            set { _CelId28 = value; /*OnPropertyChanged("CelId28");*/ }
        }
        private int _CelId28 = -1;

        public int eNodeBId // Идентификатор БС 20 бит от CelId28
        {
            get { return _eNodeBId; }
            set { _eNodeBId = value; /*OnPropertyChanged("eNodeBId");*/ }
        }
        private int _eNodeBId = -1;

        public int CID // Идентификатор БС 8 бит от CelId28 (последние)
        {
            get { return _CID; }
            set { _CID = value; /*OnPropertyChanged("CID");*/ }
        }
        private int _CID = -1;

        public int PCI // Физикал сел ID
        {
            get { return _PCI; }
            set { _PCI = value; /*OnPropertyChanged("PCI");*/ }
        }
        private int _PCI = -1;

        public int CIDToDB
        {
            get { return _CIDToDB; }
            set { _CIDToDB = value; /*OnPropertyChanged("CIDToDB"); */}
        }
        private int _CIDToDB = -1;

        public double Power // - > Некий уровень
        {
            get { return _Power; }
            set { _Power = value; /*OnPropertyChanged("Power");*/ }
        }
        private double _Power = -1000;

        /// <summary>
        /// P total
        /// </summary>
        public double InbandPower // - > Некий уровень
        {
            get { return _InbandPower; }
            set { _InbandPower = value; /*OnPropertyChanged("InbandPower");*/ }
        }
        private double _InbandPower = -1000;

        public double WB_RS_RSSI // - > Некий уровень
        {
            get { return _WB_RS_RSSI; }
            set { _WB_RS_RSSI = value; /*OnPropertyChanged("WB_RS_RSSI");*/ }
        }
        private double _WB_RS_RSSI = -1000;

        public double RSRP // - > Level CAR
        {
            get { return _RSRP; }
            set { _RSRP = value; /*OnPropertyChanged("RSRP");*/ }
        }
        private double _RSRP = -1000;

        /// <summary>
        /// напряженность этого сигнала
        /// </summary>
        public double Strenght
        {
            get { return _Strenght; }
            set { _Strenght = value; /*OnPropertyChanged("Strenght");*/ }
        }
        private double _Strenght = -1000;

        public double WB_RSRP // - > Некий уровень
        {
            get { return _WB_RSRP; }
            set { _WB_RSRP = value; /*OnPropertyChanged("WB_RSRP");*/ }
        }
        private double _WB_RSRP = -1000;

        public double RSRQ // - > Некий уровень
        {
            get { return _RSRQ; }
            set { _RSRQ = value; /*OnPropertyChanged("RSRQ");*/ }
        }
        private double _RSRQ = -1000;

        public double WB_RSRQ // - > Некий уровень
        {
            get { return _WB_RSRQ; }
            set { _WB_RSRQ = value; /*OnPropertyChanged("WB_RSRQ");*/ }
        }
        private double _WB_RSRQ = -1000;

        public string MIMO_2x2 
        {
            get { return _MIMO_2x2; }
            set { _MIMO_2x2 = value; /*OnPropertyChanged("MIMO_2x2");*/ }
        }
        private string _MIMO_2x2 = "";

        public string eNodeB_Name // ????
        {
            get { return _eNodeB_Name; }
            set { _eNodeB_Name = value; /*OnPropertyChanged("eNodeB_Name");*/ }
        }
        private string _eNodeB_Name = "";


        public double CIRofCP // CtoI
        {
            get { return _CIRofCP; }
            set { _CIRofCP = value; /*OnPropertyChanged("CIRofCP");*/ }
        }
        private double _CIRofCP = -1000;



        public string GCID
        {
            get { return _GCID; }
            set { _GCID = value; /*OnPropertyChanged("GCID");*/ }
        }
        private string _GCID = "";


        /// <summary>
        /// определяет есть ли GCID
        /// </summary>
        public bool FullData
        {
            get { return _FullData; }
            set { _FullData = value; /*OnPropertyChanged("FullData");*/ }
        }
        private bool _FullData = false;


        public DateTime LastLevelUpdete
        {
            get { return _LastLevelUpdete; }
            set { _LastLevelUpdete = value; /*OnPropertyChanged("LastLevelUpdete"); */}
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
            set { _ThisIsMaximumSignalAtThisFrequency = value; /*OnPropertyChanged("ThisIsMaximumSignalAtThisFrequency");*/ }
        }
        private bool _ThisIsMaximumSignalAtThisFrequency = false;

        public bool DeleteFromMeasMon
        {
            get { return _DeleteFromMeasMon; }
            set { _DeleteFromMeasMon = value; /*OnPropertyChanged("DeleteFromMeasMon");*/ }
        }
        private bool _DeleteFromMeasMon = false;

        #endregion



        #region ATDI
        public string ATDI_id_task
        {
            get { return _ATDI_id_task; }
            set { _ATDI_id_task = value; /*OnPropertyChanged("ATDI_id_task");*/ }
        }
        private string _ATDI_id_task = "";

        public string ATDI_id_station
        {
            get { return _ATDI_id_station; }
            set { _ATDI_id_station = value; /*OnPropertyChanged("ATDI_id_station");*/ }
        }
        private string _ATDI_id_station = "";

        public int ATDI_id_permission
        {
            get { return _ATDI_permission_id; }
            set { _ATDI_permission_id = value; /*OnPropertyChanged("ATDI_permission_id");*/ }
        }
        private int _ATDI_permission_id = 0;

        public string ATDI_id_sector
        {
            get { return _ATDI_sector_id; }
            set { _ATDI_sector_id = value; /*OnPropertyChanged("ATDI_sector_id");*/ }
        }
        private string _ATDI_sector_id = "";

        public int ATDI_id_frequency
        {
            get { return _ATDI_Frequency_id; }
            set { _ATDI_Frequency_id = value; /*OnPropertyChanged("ATDI_Frequency_id");*/ }
        }
        private int _ATDI_Frequency_id = 0;

        public decimal ATDI_FrequencyPermission
        {
            get { return _ATDI_FrequencyPermission; }
            set { _ATDI_FrequencyPermission = value; /*OnPropertyChanged("ATDI_FrequencyPermission");*/ }
        }
        private decimal _ATDI_FrequencyPermission = 0;


        public decimal ATDI_Bandwidth
        {
            get { return _ATDI_Bandwidth; }
            set { _ATDI_Bandwidth = value; BandwidthComparison(); /*OnPropertyChanged("ATDI_Bandwidth");*/ }
        }
        private decimal _ATDI_Bandwidth = -1;

        /// <summary>
        /// результат сравнения полосы сигнала из идентификации и из разрешения
        /// \t 0 = не сравнивалось 1 = не совпадает 2 = совпадает
        /// </summary>
        public int ATDI_BandwidthComparisonResult
        {
            get { return _ATDI_BandwidthComparisonResult; }
            set { _ATDI_BandwidthComparisonResult = value; /*OnPropertyChanged("ATDI_BandwidthComparisonResult");*/ }
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
            set { _ATDI_Identifier_Find = value; /*OnPropertyChanged("ATDI_Identifier_Find");*/ }
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
            set { _ATDI_FreqCheck_Find = value; /*OnPropertyChanged("ATDI_FreqCheck_Find");*/ }
        }
        private int _ATDI_FreqCheck_Find = 0;

        public string ATDI_GCID
        {
            get { return _ATDI_GCID; }
            set { _ATDI_GCID = value; /*OnPropertyChanged("ATDI_GCID");*/ }
        }
        private string _ATDI_GCID = string.Empty;

        public bool LR_NewDataToSave
        {
            get { return _LR_NewDataToSave; }
            set { _LR_NewDataToSave = value; /*OnPropertyChanged("LR_NewDataToSave");*/ }
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
            set { _station_sys_info = value; /*OnPropertyChanged("station_sys_info");*/ }
        }
        private DB.localatdi_station_sys_info _station_sys_info = new DB.localatdi_station_sys_info() { };

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<DB.localatdi_level_meas_result> level_results
        {
            get { return _level_results; }
            set { _level_results = value; /*OnPropertyChanged("level_results");*/ }
        }
        private ObservableCollection<DB.localatdi_level_meas_result> _level_results = new ObservableCollection<DB.localatdi_level_meas_result>() { };



    }
    class DFResultOnFreq:Trace
    {
        float Az; // град
        float AzAnt; // град
        float Quality; // проценты
    }
    #endregion

    