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
    public class GPSNMEA : INotifyPropertyChanged
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        Settings.XMLSettings Sett = App.Sett;
        private Thread gpsThread;
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
                if (Run) ConnectToGPS();
                else CloseConGPS();
                OnPropertyChanged("Run");
            }
        }
        //private string _ExceptionText;
        //public string ExceptionText
        //{
        //    get { return _ExceptionText; }
        //    set { _ExceptionText = value; OnPropertyChanged("ExceptionText"); }
        //}
        private long LastUpdate;

        Helpers.Helper h = new Helpers.Helper();
        //public ObservableCollection<GPSSat> sat { get; set; }
        public GPSNMEA()
        {
            //this.sat = new ObservableCollection<GPSSat> { };
        }
        #region get set


        public DB.localatdi_geo_location location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged("location"); }
        }
        private DB.localatdi_geo_location _location = new DB.localatdi_geo_location() { };

        private bool _GPSPortFound = false;
        public bool GPSPortFound
        {
            get { return _GPSPortFound; }
            set { _GPSPortFound = value; OnPropertyChanged("GPSPortFound"); }
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

        private string _LatitudeStr = "";
        public string LatitudeStr
        {
            get { return _LatitudeStr; }
            set { _LatitudeStr = value; OnPropertyChanged("LatitudeStr"); }
        }

        private string _LongitudeStr = "";
        public string LongitudeStr
        {
            get { return _LongitudeStr; }
            set { _LongitudeStr = value; OnPropertyChanged("LongitudeStr"); }
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

        private string _GPSPosition = "";
        public string GPSPosition
        {
            get { return _GPSPosition; }
            set { _GPSPosition = value; OnPropertyChanged("GPSPosition"); }
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

        private bool _GPSIsValid = false;
        public bool GPSIsValid
        {
            get { return _GPSIsValid; }
            set { _GPSIsValid = value; OnPropertyChanged("GPSIsValid"); }
        }

        private string _GPSAntennaState = "";
        public string GPSAntennaState
        {
            get { return _GPSAntennaState; }
            set { _GPSAntennaState = value; OnPropertyChanged("GPSAntennaState"); }
        }

        public ObservableCollection<sat> _Sat = new ObservableCollection<sat>() { };
        public ObservableCollection<sat> Satelite
        {
            get { return _Sat; }
            set
            {
                _Sat = value;
                
                OnPropertyChanged("Satelite");
            }
        }
        private bool _ShowSatelite = false;
        public bool ShowSatelite
        {
            get { return _ShowSatelite; }
            set { _ShowSatelite = value; OnPropertyChanged("ShowSatelite"); }
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
        public void ConnectToGPS()
        {
            // создаем таймер
            tmr.AutoReset = true;
            tmr.Enabled = true;
            tmr.Elapsed += WatchDog;
            tmr.Start();

            GPSPortFound = false;
            gpsThread = new Thread(GetGPSData);
            gpsThread.Name = "GPSThread";
            gpsThread.IsBackground = true;
            gpsThread.Start();
        }
        public void CloseConGPS()
        {
            try
            {
                port.DataReceived -= new SerialDataReceivedEventHandler(sp_DataReceived);
                Sett.SaveGNSS();
                port.Close();
                gpsThread.Abort();
                tmr.Elapsed -= WatchDog;
                tmr.Stop();
            }
            catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
        }
        private void WatchDog(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (GPSPortFound == true && new TimeSpan(DateTime.Now.Ticks - LastUpdate) > new TimeSpan(0, 0, 2))
            { CloseConGPS(); ConnectToGPS(); GPSIsValid = false; }
        }
        private void GetGPSData()
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
                catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
                if (GPSDetected())
                {
                    port.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                }
                else
                {
                    GPSPortFound = false;
                    searchPort();
                    if (GPSPortFound == true)
                    {
                        try
                        {
                            port.PortName = Sett.GNSS_Settings.PortName;// FMeas.Settings.GPS.Default.GPSSerialPort;
                            port.Open();
                            port.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                        }
                        catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
                    }
                }
            }
            catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
            MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("GPS Подключен"), ClassName = "GPS", AdditionalInformation = "" };
        }

        private bool GPSDetected()
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
                        GPSPortFound = true;
                        Sett.SaveGNSS();
                        return true;
                    }
                    else
                    {
                        GPSPortFound = false;
                        port.Close();
                        return false;
                    }
                }
                else { return false; }
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" };
                return false;
            }
        }
        private void searchPort()
        {
            while (GPSPortFound == false)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = new Exception("Приемник GPS не найден, идет поиск"), ClassName = "GPS", AdditionalInformation = "Search Port" };
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
                        catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
                        if (returnstring != "" && returnstring.Contains("$GPGGA"))
                        {
                            Sett.GNSS_Settings.PortName = p;
                            Sett.SaveGNSS(); //SaveSettings();// FMeas.Settings.GPS.Default.Save();
                            GPSPortFound = true;
                            port.Close();
                            return;
                        }
                        else
                        {
                            GPSPortFound = false;
                            port.Close();
                        }
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" };
                        GPSPortFound = false;
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
                ReadNMEAData(line);
                serBuff = serBuff.Substring(i + 2);
                //if (DateTime.Now.Ticks - beginTiks > 30000000)
                //{
                //    Time = new TimeSpan(DateTime.Now.Ticks - beginTiks).ToString();
                //    System.Windows.Forms.MessageBox.Show("Приемник GPS потерян");

                //}
            }
        }
        public void ReadNMEAData(string line)
        {
            if ('$' == line[0])
            {
                char[] spl = { ',', '*' };
                string[] tok = line.Split(spl);
                for (int j = 0; j < tok.Length; j++)
                {
                    if (tok[j] == "") { tok[j] = "0"; }
                }
                if (CheckSentence(line))
                {
                    if (tok[0].Contains("GGA")) // Fix information 
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
                                LatitudeStr = (int)double.Parse(tok[2].Substring(0, 2)) + "° " + (int)double.Parse(tok[2].Substring(2, tok[2].Length - 2).Replace(".", ",")) + "' " +
                                    Math.Round((double.Parse(tok[2].Substring(2, tok[2].Length - 2).Replace(".", ",")) - (int)double.Parse(tok[2].Substring(2, tok[2].Length - 2).Replace(".", ","))) * 60, 1) + "\" " + tok[3];

                                LongitudeStr = (int)double.Parse(tok[4].Substring(0, 3)) + "° " + (int)double.Parse(tok[4].Substring(3, tok[4].Length - 3).Replace(".", ",")) + "' " +
                                    Math.Round((double.Parse(tok[4].Substring(3, tok[4].Length - 3).Replace(".", ",")) - (int)double.Parse(tok[4].Substring(3, tok[4].Length - 3).Replace(".", ","))) * 60, 1) + "\" " + tok[5];
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
                                LatitudeStr = "Н/Д";
                                LongitudeStr = "Н/Д";
                            }
                            Sats = NumbSat.ToString() + "(" + Satelite.Count.ToString() + ")";
                        }
                        catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
                        finally { GPSIsValid = true; }
                        #endregion
                    }
                    else if (tok[0].Contains("GSV")) // Detailed Satellite data
                    {
                        #region
                        try
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                LastUpdate = DateTime.Now.Ticks;
                                beginTiks = DateTime.Now.Ticks;
                                GSVListNum = int.Parse(tok[1]);

                                //ObservableCollection<sat>  sat = new ObservableCollection<sat>() { };
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
                                //Satelite = sat;
                                ShowSatelite = _Sat.Count > 0 ? true : false;
                                OnPropertyChanged("Satelite");
                            }));
                        }
                        catch (Exception exp) { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "GPS", AdditionalInformation = "" }; }
                        finally { GPSIsValid = true; }
                        #endregion
                    }
                    else if (tok[0].Contains("RMC")) // recommended minimum data for gps 
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
                                    //string time = "20" + tok[9].Substring(4, 2).ToString() + "-" + tok[9].Substring(2, 2).ToString()
                                    //  + "-" + tok[9].Substring(0, 2).ToString() + " " + tok[1].Substring(0, 2).ToString()
                                    //  + ":" + tok[1].Substring(2, 2).ToString() + ":" + tok[1].Substring(4, 4).ToString();
                                    //UTCTime = DateTime.Parse(time);
                                    //LocalTime = UTCTime.ToLocalTime();

                                    GPSPosition = "   " + LocalTime.ToString();
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
                        finally { GPSIsValid = true; }
                        #endregion
                    }
                    else if (tok[0].Contains("APB")) // Auto Pilot B sentence 
                    {

                    }
                    else if (tok[0].Contains("BOD")) // Bearing Origin to Destination  
                    {

                    }
                    else if (tok[0].Contains("BWC")) // Bearing using Great Circle route
                    {

                    }
                    else if (tok[0].Contains("GLL")) // Lat/Lon data
                    {

                    }
                    else if (tok[0].Contains("GSA")) // Overall Satellite data
                    {

                    }
                    else if (tok[0].Contains("RMB")) // recommended navigation data for gps 
                    {

                    }
                    else if (tok[0].Contains("RTE")) // route message 
                    {

                    }
                    else if (tok[0].Contains("VTG")) // Vector track an Speed over the Ground 
                    {

                    }
                    else if (tok[0].Contains("XTE")) // measured cross track error 
                    {

                    }
                    else if (tok[0].Contains("RME")) // 
                    {

                    }
                    else if (tok[0].Contains("RMM")) // 
                    {

                    }
                    else if (tok[0].Contains("RMZ")) // 
                    {

                    }
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
            string s1 = result.ToString("X");
            string s2 = strSentence.Substring(iEnd + 1, 2);
            bool b = (s1 == s2);
            return b;
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
}


