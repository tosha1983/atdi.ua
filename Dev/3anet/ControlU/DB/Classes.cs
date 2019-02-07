 
namespace ControlU.DB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using System.Collections.ObjectModel;
    using System.IO;
    public class MyFileInfo1 : System.ComponentModel.INotifyPropertyChanged
    {

        public System.IO.FileInfo FileInfo
        {
            get { return _FileInfo; }
            set { _FileInfo = value; OnPropertyChanged("FileInfo"); }
        }
        private System.IO.FileInfo _FileInfo;

        public int FileNameType
        {
            get { return _FileNameType; }
            set { _FileNameType = value; OnPropertyChanged("FileNameType"); }
        }
        private int _FileNameType = -1;

        public string InFileErrore
        {
            get { return _InFileErrore; }
            set { _InFileErrore = value; OnPropertyChanged("InFileErrore"); }
        }
        private string _InFileErrore = string.Empty;
        // Событие, которое нужно вызывать при изменении
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
    
    [Serializable]
    public class ALLSTATIONS : INotifyPropertyChanged
    {
        public ObservableCollection<ALLSTATION> _Loaded = new ObservableCollection<ALLSTATION>() { };
        [XmlElement]
        public ObservableCollection<ALLSTATION> Loaded
        {
            get { return _Loaded; }
            set { _Loaded = value; OnPropertyChanged("Loaded"); }
        }
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Serialize(string FileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ALLSTATIONS));
            TextWriter writer = new StreamWriter(FileName, false);
            ser.Serialize(writer, this);
            writer.Close();
        }
    }
    [Serializable]
    public class ALLSTATION : INotifyPropertyChanged
    {
        private string _ID = "";
        [XmlElement]
        public string ID
        {
            get { return _ID; }
            set { _ID = value; OnPropertyChanged("ID"); }
        }
        private string _TABLE_ID = "";
        [XmlElement]
        public string TABLE_ID
        {
            get { return _TABLE_ID; }
            set { _TABLE_ID = value; OnPropertyChanged("TABLE_ID"); }
        }
        private string _TABLE_NAME = "";
        [XmlElement]
        public string TABLE_NAME
        {
            get { return _TABLE_NAME; }
            set { _TABLE_NAME = value; OnPropertyChanged("TABLE_NAME"); }
        }
        private string _STANDARD = "";
        [XmlElement]
        public string STANDARD
        {
            get { return _STANDARD; }
            set { _STANDARD = value; OnPropertyChanged("STANDARD"); }
        }
        private string _STATUS = "";
        [XmlElement]
        public string STATUS
        {
            get { return _STATUS; }
            set { _STATUS = value; STATUS_Str = MainWindow.help.helpStatusFromXXToSTR(value); OnPropertyChanged("STATUS"); }
        }
        private string _STATUS_Str = "";
        public string STATUS_Str
        {
            get { return _STATUS_Str; }
            set { _STATUS_Str = value; OnPropertyChanged("STATUS_Str"); }
        }
        private string _OWNER_NAME = "";
        [XmlElement]
        public string OWNER_NAME
        {
            get { return _OWNER_NAME; }
            set { _OWNER_NAME = MainWindow.help.helpOwnerNameAbbreviation(value); OnPropertyChanged("OWNER_NAME"); }
        }
        private string _OWNER_ID = "";
        [XmlElement]
        public string OWNER_ID
        {
            get { return _OWNER_ID; }
            set { _OWNER_ID = value; OnPropertyChanged("OWNER_ID"); }
        }
        private string _PAY_OWNER_ID = "";
        [XmlElement]
        public string PAY_OWNER_ID
        {
            get { return _PAY_OWNER_ID; }
            set { _PAY_OWNER_ID = value; OnPropertyChanged("PAY_OWNER_ID"); }
        }
        private string _CODE_EDRPU = "";
        [XmlElement]
        public string CODE_EDRPU
        {
            get { return _CODE_EDRPU; }
            set { _CODE_EDRPU = value; OnPropertyChanged("CODE_EDRPU"); }
        }
        private string _ADDRESS = "";
        [XmlElement]
        public string ADDRESS
        {
            get { return _ADDRESS; }
            set { _ADDRESS = value; OnPropertyChanged("ADDRESS"); }
        }
        private string _CITY = "";
        [XmlElement]
        public string CITY
        {
            get { return _CITY; }
            set { _CITY = value; OnPropertyChanged("ADDRESS"); }
        }
        private string _PROVINCE = "";
        [XmlElement]
        public string PROVINCE
        {
            get { return _PROVINCE; }
            set { _PROVINCE = value; OnPropertyChanged("PROVINCE"); }
        }
        private double _LATITUDE = 0;
        [XmlElement]
        public double LATITUDE
        {
            get { return _LATITUDE; }
            set { _LATITUDE = value; LATITUDE_Str = MainWindow.help.DDDtoDDMMSS(value); OnPropertyChanged("LATITUDE"); }
        }
        private string _LATITUDE_Str = string.Empty;
        public string LATITUDE_Str
        {
            get { return _LATITUDE_Str; }
            set { _LATITUDE_Str = value; OnPropertyChanged("LATITUDE_Str"); }
        }
        private double _LONGITUDE = 0;
        [XmlElement]
        public double LONGITUDE
        {
            get { return _LONGITUDE; }
            set { _LONGITUDE = value; LONGITUDE_Str = MainWindow.help.DDDtoDDMMSS(value); OnPropertyChanged("LONGITUDE"); }
        }
        private string _LONGITUDE_Str = string.Empty;
        public string LONGITUDE_Str
        {
            get { return _LONGITUDE_Str; }
            set { _LONGITUDE_Str = value; OnPropertyChanged("LONGITUDE_Str"); }
        }
        private string _EQUIP_NAME = "";
        [XmlElement]
        public string EQUIP_NAME
        {
            get { return _EQUIP_NAME; }
            set { _EQUIP_NAME = value; OnPropertyChanged("EQUIP_NAME"); }
        }
        private double _POWER = 0;
        [XmlElement]
        public double POWER
        {
            get { return _POWER; }
            set { _POWER = value; OnPropertyChanged("POWER"); }
        }
        private string _DES_EM = "";
        [XmlElement]
        public string DES_EM
        {
            get { return _DES_EM; }
            set { _DES_EM = value; OnPropertyChanged("DES_EM"); }
        }
        private double _ALTITUDE = 0;
        [XmlElement]
        public double ALTITUDE
        {
            get { return _ALTITUDE; }
            set { _ALTITUDE = value; OnPropertyChanged("ALTITUDE"); }
        }
        private string _POLARIZATION = "";
        [XmlElement]
        public string POLARIZATION
        {
            get { return _POLARIZATION; }
            set { _POLARIZATION = value; OnPropertyChanged("POLARIZATION"); }
        }
        private double _TX_FREQ_TXT = 0;
        [XmlElement]
        public double TX_FREQ_TXT
        {
            get { return _TX_FREQ_TXT; }
            set { _TX_FREQ_TXT = value; OnPropertyChanged("TX_FREQ_TXT"); }
        }
        private double _TX_LOW_FREQ = 0;
        [XmlElement]
        public double TX_LOW_FREQ
        {
            get { return _TX_LOW_FREQ; }
            set { _TX_LOW_FREQ = value; OnPropertyChanged("TX_LOW_FREQ"); }
        }
        private double _TX_HIGH_FREQ = 0;
        [XmlElement]
        public double TX_HIGH_FREQ
        {
            get { return _TX_HIGH_FREQ; }
            set { _TX_HIGH_FREQ = value; OnPropertyChanged("TX_HIGH_FREQ"); }
        }
        private string _CONCLUS_NUM = "";
        [XmlElement]
        public string CONCLUS_NUM
        {
            get { return _CONCLUS_NUM; }
            set { _CONCLUS_NUM = value; OnPropertyChanged("CONCLUS_NUM"); }
        }
        private DateTime _CONCLUS_DATE = new DateTime(1900, 01, 01, 00, 00, 00);
        [XmlElement]
        public DateTime CONCLUS_DATE
        {
            get { return _CONCLUS_DATE; }
            set { _CONCLUS_DATE = value; OnPropertyChanged("CONCLUS_DATE"); }
        }
        private DateTime _CONCLUS_DATE_STOP = new DateTime(1900, 01, 01, 00, 00, 00);
        [XmlElement]
        public DateTime CONCLUS_DATE_STOP
        {
            get { return _CONCLUS_DATE_STOP; }
            set { _CONCLUS_DATE_STOP = value; OnPropertyChanged("CONCLUS_DATE_STOP"); }
        }
        private string _PERM_NUM = "";
        [XmlElement]
        public string PERM_NUM
        {
            get { return _PERM_NUM; }
            set { _PERM_NUM = value; OnPropertyChanged("PERM_NUM"); }
        }
        private DateTime _PERM_DATE = new DateTime(1900, 01, 01, 00, 00, 00);
        [XmlElement]
        public DateTime PERM_DATE
        {
            get { return _PERM_DATE; }
            set { _PERM_DATE = value; OnPropertyChanged("PERM_DATE"); }
        }
        private DateTime _PERM_DATE_STOP = new DateTime(1900, 01, 01, 00, 00, 00);
        [XmlElement]
        public DateTime PERM_DATE_STOP
        {
            get { return _PERM_DATE_STOP; }
            set { _PERM_DATE_STOP = value; OnPropertyChanged("PERM_DATE_STOP"); }
        }
        private string _PERM_NUM_OLD = "";
        [XmlElement]
        public string PERM_NUM_OLD
        {
            get { return _PERM_NUM_OLD; }
            set { _PERM_NUM_OLD = value; OnPropertyChanged("PERM_NUM_OLD"); }
        }
        private DateTime _PERM_DATE_OLD = new DateTime(1900, 01, 01, 00, 00, 00);
        [XmlElement]
        public DateTime PERM_DATE_OLD
        {
            get { return _PERM_DATE_OLD; }
            set { _PERM_DATE_OLD = value; OnPropertyChanged("PERM_DATE_OLD"); }
        }
        private DateTime _PERM_DATE_STOP_OLD = new DateTime(1900, 01, 01, 00, 00, 00);
        [XmlElement]
        public DateTime PERM_DATE_STOP_OLD
        {
            get { return _PERM_DATE_STOP_OLD; }
            set { _PERM_DATE_STOP_OLD = value; OnPropertyChanged("PERM_DATE_STOP_OLD"); }
        }
        private string _LICENCE_NUM = "";
        [XmlElement]
        public string LICENCE_NUM
        {
            get { return _LICENCE_NUM; }
            set { _LICENCE_NUM = value; OnPropertyChanged("LICENCE_NUM"); }
        }
        private string _APPL_ID = "";
        [XmlElement]
        public string APPL_ID
        {
            get { return _APPL_ID; }
            set { _APPL_ID = value; OnPropertyChanged("APPL_ID"); }
        }


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    #region Enums   
    public enum Technologys : int
    {
        Unknown = 0,
        GSM = 1,
        CDMA = 2,
        UMTS = 3,
        WIMAX = 4,
        LTE = 5,
        WRLS = 6,
        RRS = 7,
        UHF = 8,
    }    
    #endregion
}
