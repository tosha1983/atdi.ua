using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Xml;

namespace ControlU.Equipment
{
    public class GNSSNMEA : INotifyPropertyChanged
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        Settings.XMLSettings Sett = App.Sett;
        private Thread gnssThread;
        public System.Timers.Timer tmr = new System.Timers.Timer(1000);

        private SerialPort port;
        private string serBuff = "";


        bool _Run;
        public bool Run
        {
            get { return _Run; }
            set
            {
                _Run = value;
                if (Run) ConnectToGNSS();
                else CloseConGNSS();
                OnPropertyChanged("Run");
            }
        }
        private long LastUpdate;

        Helpers.Helper h = new Helpers.Helper();
        public GNSSNMEA()
        {

        }
        #region get set
        private List<GNSSDataFrom> GNSSData = new List<GNSSDataFrom>() { };
        private GNSSDataFrom SelectedGNSSDataToRead = new GNSSDataFrom();
        private long LastRead = 0;

        public DB.localatdi_geo_location location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged("location"); }
        }
        private DB.localatdi_geo_location _location = new DB.localatdi_geo_location() { };

        private bool _GNSSPortFound = false;
        public bool GNSSPortFound
        {
            get { return _GNSSPortFound; }
            set { _GNSSPortFound = value; OnPropertyChanged("GNSSPortFound"); }
        }

        private DateTime _UTCTime;
        public DateTime UTCTime
        {
            get { return _UTCTime; }
            set { _UTCTime = value; OnPropertyChanged("UTCTime"); }
        }

        private TimeSpan _LocalOffset;
        public DateTime LocalTime
        {
            get { return DateTime.Now - _LocalOffset; ; }
            set { _LocalOffset = DateTime.Now - LocalTime; OnPropertyChanged("LocalTime"); }
        }

        public decimal _LastLatitude = 0;
        private decimal _LatitudeDecimal = 0;//50.407553m;
        public decimal LatitudeDecimal
        {
            get { return _LatitudeDecimal; }
            set { if (_LatitudeDecimal != value) { _LatitudeDecimal = value; OnPropertyChanged("LatitudeDecimal"); } }
        }

        public decimal _LastLongitude = 0;
        private decimal _LongitudeDecimal = 0;//30.613064m;
        public decimal LongitudeDecimal
        {
            get { return _LongitudeDecimal; }
            set { if (_LongitudeDecimal != value) { _LongitudeDecimal = value; OnPropertyChanged("LongitudeDecimal"); } }
        }

        private int _NumbSat = 0;
        public int NumbSat
        {
            get { return _NumbSat; }
            set { _NumbSat = value; OnPropertyChanged("NumbSat"); }
        }

        private double _Horizontaldilution = 0;
        public double Horizontaldilution
        {
            get { return _Horizontaldilution; }
            set { _Horizontaldilution = value; OnPropertyChanged("Horizontaldilution"); }
        }

        private double _Altitude = 0;
        public double Altitude
        {
            get { return _Altitude; }
            set { _Altitude = value; OnPropertyChanged("Altitude"); }
        }

        private double _AngleCourse = 0;
        public double AngleCourse
        {
            get { return _AngleCourse; }
            set
            {
                if (value != null)
                {
                    if (value < 0) _AngleCourse = 0;
                    else if (value > 360) _AngleCourse = 0;
                    else _AngleCourse = value;
                    OnPropertyChanged("AngleCourse");
                }
            }
        }

        private double _Speed = 0;
        public double Speed
        {
            get { return _Speed; }
            set { _Speed = value; OnPropertyChanged("Speed"); OnPropertyChanged("SpeedKmPerH"); }
        }

        private double _SpeedKmPerH = 0;
        public double SpeedKmPerH
        {
            get { return (int)(Speed * 3.6); }
            set { }
        }

        private string _GNSSPosition = "";
        public string GNSSPosition
        {
            get { return _GNSSPosition; }
            set { _GNSSPosition = value; OnPropertyChanged("GNSSPosition"); }
        }

        private int _GSVListNum = 0;
        private int GSVListNum
        {
            get { return _GSVListNum; }
            set { _GSVListNum = value; OnPropertyChanged("GSVListNum"); }
        }

        private string _Sats = "";
        public string Sats
        {
            get { return _Sats; }
            set { _Sats = value; OnPropertyChanged("Sats"); }
        }

        private bool _GNSSIsValid = false;
        public bool GNSSIsValid
        {
            get { return _GNSSIsValid; }
            set { _GNSSIsValid = value; OnPropertyChanged("GNSSIsValid"); }
        }

        private string _GNSSAntennaState = "";
        public string GNSSAntennaState
        {
            get { return _GNSSAntennaState; }
            set { _GNSSAntennaState = value; OnPropertyChanged("GNSSAntennaState"); }
        }

        public ObservableCollection<sat> _Sat = new ObservableCollection<sat>() { };
        public ObservableCollection<sat> Satelite
        {
            get { return _Sat; }
            set { _Sat = value; OnPropertyChanged("Satelite"); }
        }

        private int _SpatialReference = 0;
        public int SpatialReference
        {
            get { return _SpatialReference; }
            set { _SpatialReference = value; OnPropertyChanged("SpatialReference"); }
        }

        private string _Time = "";
        public string Time
        {
            get { return _Time; }
            set { _Time = value; OnPropertyChanged("Time"); }
        }
        #endregion
        //================================================================================================

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        //================================================================================================
        public void ConnectToGNSS()
        {
            GNSSPortFound = false;
            gnssThread = new Thread(GetGNSSData);
            gnssThread.Name = "GNSSThread";
            gnssThread.IsBackground = true;
            gnssThread.Start();

            // создаем таймер
            tmr.AutoReset = true;
            tmr.Enabled = true;
            tmr.Elapsed += WatchDog;
            tmr.Start();
        }
        public void CloseConGNSS()
        {
            try
            {
                port.DataReceived -= new SerialDataReceivedEventHandler(sp_DataReceived);
                Sett.SaveGNSS();
                port.Close();
                gnssThread.Abort();
                tmr.Elapsed -= WatchDog;
                tmr.Stop();
            }
            catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GNSS", AdditionalInformation = "" }; }
        }
        private void WatchDog(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (GNSSPortFound == true && new TimeSpan(DateTime.Now.Ticks - LastUpdate) > new TimeSpan(0, 0, 2))
            { CloseConGNSS(); ConnectToGNSS(); GNSSIsValid = false; }
        }
        private void GetGNSSData()
        {
            try
            {
                try
                {
                    port = new SerialPort();
                    port.BaudRate = Sett.GNSS_Settings.PortBaudRate;
                    port.Parity = Sett.GNSS_Settings.PortParity;//(Parity)Enum.Parse(typeof(Parity), Parity, true); 
                    port.DataBits = Sett.GNSS_Settings.PortDataBits;// DataBits;
                    port.StopBits = Sett.GNSS_Settings.PortStopBits;// (StopBits)Enum.Parse(typeof(StopBits), StopBits, true);
                    port.Handshake = Sett.GNSS_Settings.PortHandshake; // (Handshake)Enum.Parse(typeof(Handshake), Handshake, true);
                }
                catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GNSS", AdditionalInformation = "" }; }
                if (GNSSDetected())
                {
                    port.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "GNSS Подключен";// ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("LicenseFail").ToString();
                    }));
                }
                else
                {
                    GNSSPortFound = false;
                    searchPort();
                    if (GNSSPortFound == true)
                    {
                        try
                        {
                            port.PortName = Sett.GNSS_Settings.PortName;// FMeas.Settings.GPS.Default.GPSSerialPort;
                            port.Open();
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "GNSS Подключен";// ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("LicenseFail").ToString();
                            }));
                            port.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                        }
                        catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GNSS", AdditionalInformation = "" }; }
                    }
                }
            }
            catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GNSS", AdditionalInformation = "" }; }

        }

        private bool GNSSDetected()
        {
            try
            {
                if (Sett.GNSS_Settings.PortName != "")
                {
                    port.PortName = Sett.GNSS_Settings.PortName;// FMeas.Settings.GPS.Default.GPSSerialPort;
                    port.Open();
                    System.Threading.Thread.Sleep(1000);
                    string returnstring = port.ReadExisting();
                    if (returnstring.Contains("$GPGGA"))
                    {
                        GNSSPortFound = true;
                        Sett.SaveGNSS();
                        return true;
                    }
                    else
                    {
                        GNSSPortFound = false;
                        port.Close();
                        return false;
                    }
                }
                else { return false; }
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = exp.Message;// "GNSS Подключен"; //MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GNSS", AdditionalInformation = "" };
                }));
                return false;
            }
        }
        private void searchPort()
        {
            while (GNSSPortFound == false)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Приемник GPS не найден, идет поиск";// ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("LicenseFail").ToString();
                }));
                //MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("Приемник GPS не найден, идет поиск"), ClassName = "GPS", AdditionalInformation = "Search Port" };
                string[] ports = SerialPort.GetPortNames();
                //System.Windows.MessageBox.Show("gps");
                foreach (string p in ports)
                {
                    try
                    {
                        string returnstring = "";
                        try
                        {
                            port.PortName = p;
                            if (port.IsOpen == false)
                            {
                                //System.Windows.MessageBox.Show(p);
                                port.Open();
                                Thread.Sleep(1000);
                                returnstring = port.ReadExisting();
                            }
                        }
                        catch (Exception exp)
                        {
                            //MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" };
                        }
                        if (returnstring != "" && returnstring.Contains("$GPGGA"))
                        {
                            Sett.GNSS_Settings.PortName = p;
                            Sett.SaveGNSS(); //SaveSettings();// FMeas.Settings.GPS.Default.Save();
                            GNSSPortFound = true;
                            port.Close();
                            return;
                        }
                        else
                        {
                            GNSSPortFound = false;
                            port.Close();
                        }
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" };
                        GNSSPortFound = false;
                        port.Close();
                    }
                }
            }
        }
        long beginTiks = 0;
        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            serBuff += port.ReadExisting();
            while (serBuff.Contains("\r\n"))
            {
                int i = serBuff.IndexOf("\r\n");
                string line = serBuff.Substring(0, i);
                ReadNMEAData(line, "SerialPort:" + port.PortName);
                serBuff = serBuff.Substring(i + 2);
            }
        }
        /// <summary>
        /// Read NMEA data
        /// </summary>
        /// <param name="line">NMEA string</param>
        /// <param name="DataFrom">Device:SN ("SerialPort:COM1", "TSME:100219", "FSV:100219" etc)</param>
        public void ReadNMEAData(string line, string DataFrom)
        {
            string[] dev = DataFrom.Split(':');

            #region add/update data 
            if (GNSSData.Count > 0)
            {
                bool find = false;
                for (int i = 0; i < GNSSData.Count; i++)
                {
                    if (GNSSData[i].DataFrom == DataFrom)
                    {
                        find = true;
                        GNSSData[i].DataBlockToRead = line;
                        GNSSData[i].NewData = true;
                        GNSSData[i].Updated = DateTime.Now.Ticks;
                    }
                }
                if (!find)
                {
                    int preor = GetPriority(DataFrom);
                    if (preor >= 0)
                    {
                        GNSSDataFrom dt = new GNSSDataFrom()
                        {
                            DataBlockToRead = line,
                            DataFrom = DataFrom,
                            NewData = true,
                            Priority = preor,
                            Updated = DateTime.Now.Ticks
                        };
                        GNSSData.Add(dt);
                    }
                }
            }
            else
            {
                int preor = GetPriority(DataFrom);
                if (preor >= 0)
                {
                    GNSSDataFrom dt = new GNSSDataFrom()
                    {
                        DataBlockToRead = line,
                        DataFrom = DataFrom,
                        NewData = true,
                        Priority = preor,
                        Updated = DateTime.Now.Ticks
                    };
                    GNSSData.Add(dt);
                }
            }
            #endregion

            #region select data to read
            
            bool find = false;
            //читаем с выбраного источника
            if (SelectedGNSSDataToRead.NewData && LastRead < SelectedGNSSDataToRead.Updated - 10000000)
            {
                ReadNMEAData(SelectedGNSSDataToRead.DataBlockToRead);
                LastRead = DateTime.Now.Ticks;
            }
            else
            {
                int index = -1;
                for (int i = 0; i < GNSSData.Count; i++)
                {
                    if (GNSSData[i].NewData)
                    {
                        find = true;
                        index = i;
                    }
                }
                if (index > -1)
                {
                    ReadNMEAData(SelectedGNSSDataToRead.DataBlockToRead);
                    LastRead = DateTime.Now.Ticks;
                }
            }
            #endregion
        }
        private void ReadNMEAData(string line)
        {
            if ('$' == line[0])
            {
                char[] spl = { ',', '*' };
                string[] tok = line.Split(spl);
                for (int j = 0; j < tok.Length; j++)
                {
                    if (tok[j] == "") { tok[j] = "0"; }
                }
                switch (tok[0])
                {
                    //GPSAntennaState
                    case "$GPAPB":      // Auto Pilot B sentence 
                        break;
                    case "$GPBOD":      // Bearing Origin to Destination 
                        break;
                    case "$GPBWC":      // Bearing using Great Circle route 
                        break;
                    case "$GPGGA":      // Fix information 
                        if (CheckSentence(line))
                        {
                            #region
                            try
                            {
                                LastUpdate = DateTime.Now.Ticks;
                                beginTiks = DateTime.Now.Ticks;
                                if (int.Parse(tok[6]) > 0 && tok[2].Length > 0 && tok[4].Length > 0)
                                {
                                    _LastLatitude = LatitudeDecimal;
                                    _LastLongitude = LongitudeDecimal;

                                    LatitudeDecimal = Math.Round(decimal.Parse(tok[2].Substring(0, 2)) + decimal.Parse(tok[2].Substring(2, tok[2].Length - 2).Replace(".", ",")) / 60, 6); // +" " + tok[3];
                                    LongitudeDecimal = Math.Round(decimal.Parse(tok[4].Substring(0, 3)) + decimal.Parse(tok[4].Substring(3, tok[4].Length - 3).Replace(".", ",")) / 60, 6); // + " " + tok[5];

                                    NumbSat = int.Parse(tok[7]);
                                    Horizontaldilution = double.Parse(tok[8].Replace(".", ","));
                                    Altitude = double.Parse(tok[9].Replace(".", ","));

                                    location.asl = Altitude;
                                    location.latitude = (double)LatitudeDecimal;
                                    location.longitude = (double)LongitudeDecimal;
                                    OnPropertyChanged("location");
                                }
                                else
                                {
                                    //LatitudeStr = "Н/Д";
                                    //LongitudeStr = "Н/Д";
                                }
                                Sats = NumbSat.ToString() + "(" + Satelite.Count.ToString() + ")";
                            }
                            catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
                            finally { GNSSIsValid = true; }
                            #endregion
                        }
                        break;
                    case "$GPGLL":      // Lat/Lon data 
                        break;
                    case "$GPGSA":      //  Overall Satellite data                             
                        break;
                    case "$GPGSV":      // Detailed Satellite data 
                        if (CheckSentence(line))
                        {
                            #region
                            try
                            {
                                App.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    LastUpdate = DateTime.Now.Ticks;
                                    beginTiks = DateTime.Now.Ticks;
                                    GSVListNum = int.Parse(tok[1]);
                                    if (int.Parse(tok[2]) == 1)
                                    { Satelite = new ObservableCollection<sat>() { }; }
                                    for (int j = 0; j < (tok.Length - 5) / 4; j++)
                                    {
                                        try
                                        {
                                            if (7 + j * 4 < tok.Length && int.Parse(tok[7 + j * 4]) > 0)
                                            {

                                                Satelite.Add(
                                                    new sat
                                                    {
                                                        SatNum = int.Parse(tok[4 + j * 4]),
                                                        Elevation = int.Parse(tok[5 + j * 4]),
                                                        Azimuth = int.Parse(tok[6 + j * 4]),
                                                        SatLevel = int.Parse(tok[7 + j * 4])
                                                    });
                                            }
                                        }
                                        catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
                                    }
                                    OnPropertyChanged("Satelite");
                                }));
                            }
                            catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
                            finally { GNSSIsValid = true; }
                            #endregion
                        }
                        break;
                    case "$GPRMB":      // recommended navigation data for gps 
                        break;
                    case "$GPRMC":      // recommended minimum data for gps 
                        if (CheckSentence(line))
                        {
                            #region
                            try
                            {
                                LastUpdate = DateTime.Now.Ticks;
                                beginTiks = DateTime.Now.Ticks;
                                try
                                {
                                    if (tok[9].Length > 1 && tok[1].Length > 1)
                                    {
                                        string time = "20" + tok[9].Substring(4, 2).ToString() + "-" + tok[9].Substring(2, 2).ToString()
                                          + "-" + tok[9].Substring(0, 2).ToString() + " " + tok[1].Substring(0, 2).ToString()
                                          + ":" + tok[1].Substring(2, 2).ToString() + ":" + tok[1].Substring(4, 4).ToString();
                                        UTCTime = DateTime.Parse(time);
                                        LocalTime = UTCTime.ToLocalTime();

                                        GNSSPosition = "   " + LocalTime.ToString();
                                        _LastLatitude = LatitudeDecimal;
                                        _LastLongitude = LongitudeDecimal;
                                    }
                                    if (tok[3] != "0" && tok[3].Length > 1)
                                        LatitudeDecimal = Math.Round(decimal.Parse(tok[3].Substring(0, 2)) + decimal.Parse(tok[3].Substring(2, tok[3].Length - 2).Replace(".", ",")) / 60, 6); // +" " + tok[3];
                                    if (tok[5] != "0" && tok[5].Length > 1)
                                        LongitudeDecimal = Math.Round(decimal.Parse(tok[5].Substring(0, 3)) + decimal.Parse(tok[5].Substring(3, tok[5].Length - 3).Replace(".", ",")) / 60, 6); // + " " + tok[5];
                                    if (tok[7].Length > 1)
                                        Speed = (double.Parse(tok[7].Replace(".", ","))) * 0.5144;

                                }
                                catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }


                                double dist = 0;
                                double ang = 0;
                                if (double.Parse(tok[8].Replace(",", "."), CultureInfo.InvariantCulture) == 0)
                                {
                                    if (_LastLatitude != LatitudeDecimal && _LastLongitude != LongitudeDecimal)
                                    {
                                        h.calcDistance((double)_LastLatitude, (double)_LastLongitude, (double)LatitudeDecimal, (double)LongitudeDecimal, out dist, out ang);
                                        AngleCourse = Math.Round(ang, 1);

                                    }
                                }
                                else { AngleCourse = Math.Round(double.Parse(tok[8].Replace(",", "."), CultureInfo.InvariantCulture), 1); }
                            }
                            catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
                            finally { GNSSIsValid = true; }
                            #endregion
                        }
                        break;
                    case "$GPRTE":      // route message 
                        break;
                    case "$GPVTG":      // Vector track an Speed over the Ground 
                        break;
                    case "$GPXTE":      // measured cross track error 
                        break;
                    case "$PGRME":
                        break;
                    case "$PGRMM":
                        break;
                    case "$PGRMZ":
                        break;
                    default:
                        string un = line;
                        break;
                }
            }
        }
        private bool CheckSentence(string strSentence)
        {
            int iStart = strSentence.IndexOf('$');
            int iEnd = strSentence.IndexOf('*');
            //If start/stop isn't found it probably doesn't contain a checksum,
            //or there is no checksum after *. In such cases just return true.
            if (iStart >= iEnd || iEnd + 3 > strSentence.Length) return true;
            byte result = 0;
            for (int i = iStart + 1; i < iEnd; i++)
            {
                result ^= (byte)strSentence[i];
            }
            return (result.ToString("X") == strSentence.Substring(iEnd + 1, 2));
        }

        private int GetPriority(string DataFrom)
        {
            int res = -1;
            string[] dev = DataFrom.Split(':');
            if (dev.Length == 2)
            {
                if (dev[0].Contains("SerialPort"))
                    res = 2;
                else if (dev[0].Contains("TSME") || dev[0].Contains("TSMW"))
                    res = 0;
                else if (dev[0].Contains("FPH") || dev[0].Contains("FSH") || dev[0].Contains("ZVH") || dev[0].Contains("N99") || dev[0].Contains("NMS27"))
                    res = 1;
                else if (dev[0].Contains("FSW") || dev[0].Contains("FSV") || dev[0].Contains("FSVA") || dev[0].Contains("ESRP"))
                    res = -1;
            }
            return res;
        }
    }
    public partial class sat : INotifyPropertyChanged
    {
        //public int SatNum { get; set; }
        //public int SatLevel { get; set; }
        private int _SatNum = 0;
        public int SatNum
        {
            get { return _SatNum; }
            set { _SatNum = value; OnPropertyChanged("SatNum"); }
        }
        private int _Elevation = 0;
        public int Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; OnPropertyChanged("Elevation"); }
        }
        private int _Azimuth = 0;
        public int Azimuth
        {
            get { return _Azimuth; }
            set { _Azimuth = value; OnPropertyChanged("Azimuth"); }
        }
        private int _SatLevel = 0;
        public int SatLevel
        {
            get { return _SatLevel; }
            set { _SatLevel = value; OnPropertyChanged("SatLevel"); }
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
    public class GNSSDataFrom
    {
        /// <summary>
        /// -1 = такого не бывает, устроить истерику
        /// 0 = Встроенный в прибор приемник (читать без раздумий)
        /// 1 = Внешний приемник у прибора (скорее всего нормальные данные), проприоритарный разьем на приборе, только брендовое устройство
        /// 2 = Внешний приемник RS232/USB (можно мухлевать при желании)
        /// </summary>
        public int Priority { get; set; } = 0;

        public string DataFrom { get; set; } = "";

        public string DataBlockToRead { get; set; } = "";

        /// <summary>
        /// in System.DateTime.Tick
        /// </summary>
        public long Updated { get; set; } = 0;

        public bool NewData { get; set; } = false;
    }
}


