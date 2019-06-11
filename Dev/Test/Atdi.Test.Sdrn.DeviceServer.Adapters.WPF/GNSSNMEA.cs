using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Adapters;
//using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Atdi.Test.Sdrn.DeviceServer.Adapters.WPF
{
    public class GNSSNMEA
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private readonly ITimeService _timeService;
        private Thread gpsThread;
        private Thread PPSThread;
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
            }
        }
        //private string _ExceptionText;
        //public string ExceptionText
        //{
        //    get { return _ExceptionText; }
        //    set { _ExceptionText = value; OnPropertyChanged("ExceptionText"); }
        //}
        private long LastUpdate;

        //Helpers.Helper h = new Helpers.Helper();
        //public ObservableCollection<GPSSat> sat { get; set; }
        public GNSSNMEA(ITimeService timeService)
        {
            _timeService = timeService;
            //this.sat = new ObservableCollection<GPSSat> { };
        }
        bool CDHolding = false;
        private long CDUpdate = 0;
        #region get set


        private bool _GPSPortFound = false;
        public bool GPSPortFound
        {
            get { return _GPSPortFound; }
            set { _GPSPortFound = value; }
        }

        private DateTime _UTCTime;
        public DateTime UTCTime
        {
            get { return _UTCTime; }
            set { _UTCTime = value; }
        }

        public TimeSpan _LocalOffset;
        public DateTime LocalTime
        {
            get { return DateTime.Now + _LocalOffset; }
            set { }
        }

        private string _LatitudeStr = "";
        public string LatitudeStr
        {
            get { return _LatitudeStr; }
            set { _LatitudeStr = value; }
        }

        private string _LongitudeStr = "";
        public string LongitudeStr
        {
            get { return _LongitudeStr; }
            set { _LongitudeStr = value; }
        }

        public decimal _LastLatitude = 0;
        private decimal _LatitudeDecimal = 0;//50.407553m;
        public decimal LatitudeDecimal
        {
            get { return _LatitudeDecimal; }
            set { if (_LatitudeDecimal != value) { _LatitudeDecimal = value; } }
        }

        public decimal _LastLongitude = 0;
        private decimal _LongitudeDecimal = 0;//30.613064m;
        public decimal LongitudeDecimal
        {
            get { return _LongitudeDecimal; }
            set { if (_LongitudeDecimal != value) { _LongitudeDecimal = value; } }
        }

        private int _NumbSat = 0;
        public int NumbSat
        {
            get { return _NumbSat; }
            set { _NumbSat = value; }
        }

        private double _Horizontaldilution = 0;
        public double Horizontaldilution
        {
            get { return _Horizontaldilution; }
            set { _Horizontaldilution = value; }
        }

        private double _Altitude = 0;
        public double Altitude
        {
            get { return _Altitude; }
            set { _Altitude = value; }
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
                }
            }
        }

        private double _Speed = 0;
        public double Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
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
            set { _GPSPosition = value; }
        }

        private int _GSVListNum = 0;
        private int GSVListNum
        {
            get { return _GSVListNum; }
            set { _GSVListNum = value; }
        }

        private string _Sats = "";
        public string Sats
        {
            get { return _Sats; }
            set { _Sats = value; }
        }

        private bool _GPSIsValid = false;
        public bool GPSIsValid
        {
            get { return _GPSIsValid; }
            set { _GPSIsValid = value; }
        }

        private string _GPSAntennaState = "";
        public string GPSAntennaState
        {
            get { return _GPSAntennaState; }
            set { _GPSAntennaState = value; }
        }

        public ObservableCollection<sat> _Sat = new ObservableCollection<sat>() { };
        public ObservableCollection<sat> Satelite
        {
            get { return _Sat; }
            set { _Sat = value; }
        }

        private int _SpatialReference = 0;
        public int SpatialReference
        {
            get { return _SpatialReference; }
            set { _SpatialReference = value; }
        }

        private string _Time = "";
        public string Time
        {
            get { return _Time; }
            set { _Time = value; }
        }
        #endregion
        //================================================================================================


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
                port.Close();
                gpsThread.Abort();
                PPSThread.Abort();
                tmr.Elapsed -= WatchDog;
                tmr.Stop();
            }
            catch (Exception exp) { }
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
                    port.BaudRate = 115200;// 115200;// 9600;
                    port.Parity = Parity.None;//(Parity)Enum.Parse(typeof(Parity), Parity, true); 
                    port.DataBits = 8;// Sett.GNSS_Settings.PortDataBits;// DataBits;
                    port.StopBits = StopBits.One;// Sett.GNSS_Settings.PortStopBits;// (StopBits)Enum.Parse(typeof(StopBits), StopBits, true);
                    port.Handshake = Handshake.None;// Sett.GNSS_Settings.PortHandshake; // (Handshake)Enum.Parse(typeof(Handshake), Handshake, true);
                }
                catch (Exception exp) { Debug.WriteLine(exp); }
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
                            port.PortName = "COM21";// FMeas.Settings.GPS.Default.GPSSerialPort;
                            port.Open();
                            port.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                        }
                        catch (Exception exp) { Debug.WriteLine(exp); }
                    }
                }
            }
            catch (Exception exp) { Debug.WriteLine(exp); }
            Debug.WriteLine("GPS Подключен");

            PPSThread = new Thread(GetPPSData);
            PPSThread.Name = "PPSThread";
            PPSThread.IsBackground = true;
            PPSThread.Start();
        }



        private void GetPPSData()
        {
            try
            {
                long PPSStep = 10000000;//10000000;//2000000
                long offset = 0;
                bool b;
                long offtime = 0;
                while (GPSPortFound)
                {
                    b = port.CDHolding;
                    if (b != CDHolding)
                    {
                        if (b)
                        {
                            offtime = UTCTime.Ticks - Atdi.Common.WinAPITime.GetTimeStamp() + PPSStep;
                            //offtime = UTCTime.Ticks;// - PPSStep;
                            // offtime = MyTime.GetTimeStamp() - 504911232000000000;//621355968000000000;// - UTCTime.Ticks - PPSStep;
                            if (UTCTime.Ticks != 0)
                            {
                                if (OffsetToAvg.Count > 599)
                                { OffsetToAvg.RemoveAt(0); }
                                OffsetToAvg.Add(offtime);

                                long OffsetToAvged = OffsetToAvg[0];

                                for (int i = 1; i < OffsetToAvg.Count; i++)
                                {
                                    OffsetToAvged += OffsetToAvg[i];
                                }

                                OffsetToAvged = OffsetToAvged / OffsetToAvg.Count;
                                _LocalOffset = new TimeSpan(OffsetToAvged);


                                //////////offset = _timeService.TimeStamp.Ticks - CDUpdate;
                                //////////CDUpdate = _timeService.TimeStamp.Ticks;
                                ////////////////UTCTime;

                                ////////////////_LocalOffset = new TimeSpan(DateTime.Now.Ticks - UTCTime.ToLocalTime().Ticks - 10000000);

                                //////////Debug.WriteLine("This " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                                //////////Debug.WriteLine("ofs  " + _LocalOffset.ToString());
                                //////////Debug.WriteLine(Thread.CurrentThread.Name + "  " + _timeService.TimeStamp.Ticks);
                                //////////Debug.WriteLine("UTCG " + UTCTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                                //////////Debug.WriteLine("UTC " + _UTCTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                                ////////Debug.WriteLine("Ofse " + _LocalOffset + "  " + offtime + "  " + OffsetToAvged + "  " + OffsetToAvg.Count);
                                ////////Debug.WriteLine("LocS " + LocalTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                                //////////Debug.WriteLine("LocA " + (DateTime.Now + new TimeSpan(OffsetToAvged)).ToString("yyyy-MM-dd HH:mm:ss.fffffffK") + "  " + OffsetToAvged + "  " + OffsetToAvg.Count);
                                ////////Debug.WriteLine("Time " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                               

                                //////////Debug.WriteLine("time  " + MyTime.GetTimeStamp());
                                //////////Debug.WriteLine("Yes  " + offset);
                               
                            }
                            Thread.Sleep(900);

                        }
                        else
                        {
                            //Debug.WriteLine("No");
                        }
                        CDHolding = b;
                    }
                }
            }
            catch (Exception exp) { Debug.WriteLine(exp); }
        }
        List<long> OffsetToAvg = new List<long>();

        private bool GPSDetected()
        {
            try
            {
                if ("COM21" != "")
                {
                    port.PortName = "COM21";// FMeas.Settings.GPS.Default.GPSSerialPort;
                    port.Open();
                    System.Threading.Thread.Sleep(1000);
                    string returnstring = port.ReadExisting();
                    if (returnstring.Contains("$GPGGA"))
                    {
                        GPSPortFound = true;
                        //Sett.SaveGNSS();
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
                Debug.WriteLine(exp);
                return false;
            }
        }
        private void searchPort()
        {
            while (GPSPortFound == false)
            {
                Debug.WriteLine("Приемник GPS не найден, идет поиск");
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
                        catch (Exception exp) { Debug.WriteLine(exp); }
                        if (returnstring != "" && returnstring.Contains("$GPGGA"))
                        {
                            //Sett.GNSS_Settings.PortName = p;
                            //Sett.SaveGNSS(); //SaveSettings();// FMeas.Settings.GPS.Default.Save();
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
                        Debug.WriteLine(exp);
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
                                //Debug.WriteLine("GPS GGA " + tok[1]);
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
                                }
                                else
                                {
                                    LatitudeStr = "Н/Д";
                                    LongitudeStr = "Н/Д";
                                }
                                Sats = NumbSat.ToString() + "(" + Satelite.Count.ToString() + ")";
                            }
                            catch (Exception exp) { Debug.WriteLine(exp); }
                            finally { GPSIsValid = true; }
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
                                        catch (Exception exp) { Debug.WriteLine(exp); }
                                    }
                                }));
                            }
                            catch (Exception exp) { Debug.WriteLine(exp); }
                            finally { GPSIsValid = true; }
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

                                        //Thread.Sleep(500);
                                        //Debug.WriteLine(Thread.CurrentThread.Name + "  " + _timeService.TimeStamp.Ticks);
                                        string time = "20" + tok[9].Substring(4, 2).ToString() + "-" + tok[9].Substring(2, 2).ToString()
                                          + "-" + tok[9].Substring(0, 2).ToString() + " " + tok[1].Substring(0, 2).ToString()
                                          + ":" + tok[1].Substring(2, 2).ToString() + ":" + tok[1].Substring(4, tok[1].Length - 4).ToString();
                                        UTCTime = DateTime.Parse(time);
                                        ////////////_LocalOffset = new TimeSpan(DateTime.Now.Ticks - UTCTime.ToLocalTime().Ticks);
                                        //////////////LocalTime = UTCTime.ToLocalTime();
                                        //////////Debug.WriteLine("GPSR " + time);
                                        //////////Debug.WriteLine("");
                                        ////////////Debug.WriteLine("ofs  " + _LocalOffset.ToString());
                                        ////////////Debug.WriteLine("UTCG " + UTCTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                                        ////////////Debug.WriteLine("LocS " + LocalTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));

                                        ////////////GPSPosition = "   " + LocalTime.ToString();
                                        _LastLatitude = LatitudeDecimal;
                                        _LastLongitude = LongitudeDecimal;
                                    }
                                    if (tok[3] != "0" && tok[3].Length > 1)
                                        LatitudeDecimal = Math.Round(decimal.Parse(tok[3].Substring(0, 2)) + decimal.Parse(tok[3].Substring(2, tok[3].Length - 2).Replace(".", ",")) / 60, 6); // +" " + tok[3];
                                    if (tok[5] != "0" && tok[5].Length > 1)
                                        LongitudeDecimal = Math.Round(decimal.Parse(tok[5].Substring(0, 3)) + decimal.Parse(tok[5].Substring(3, tok[5].Length - 3).Replace(".", ",")) / 60, 6); // + " " + tok[5];
                                    if (tok[7].Length > 1)
                                        Speed = (double.Parse(tok[7].Replace(".", ","))) * 0.5144;
                                    //Debug.WriteLine(LocalTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK"));
                                }
                                catch (Exception exp)
                                {
                                    Debug.WriteLine(exp);
                                }


                                double dist = 0;
                                double ang = 0;
                                if (double.Parse(tok[8].Replace(",", "."), CultureInfo.InvariantCulture) == 0)
                                {
                                    if (_LastLatitude != LatitudeDecimal && _LastLongitude != LongitudeDecimal)
                                    {
                                        calcDistance((double)_LastLatitude, (double)_LastLongitude, (double)LatitudeDecimal, (double)LongitudeDecimal, out dist, out ang);
                                        AngleCourse = Math.Round(ang, 1);

                                    }
                                }
                                else { AngleCourse = Math.Round(double.Parse(tok[8].Replace(",", "."), CultureInfo.InvariantCulture), 1); }
                            }
                            catch (Exception exp) { Debug.WriteLine(exp); }
                            finally { GPSIsValid = true; }
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
        public void calcDistance(double Lat1, double Lon1, double Lat2, double Lon2, out double distance, out double angledeg)
        {
            int rad = 6372795; // 6378137;// 6372795; // радиус земли

            //получение координат точек в радианах
            double lat1r = Lat1 * Math.PI / 180; //широта
            double lat2r = Lat2 * Math.PI / 180; //широта
            double lon1r = Lon1 * Math.PI / 180; //долгота
            double lon2r = Lon2 * Math.PI / 180; //долгота

            //косинусы и синусы широт и разницы долгот
            double cl1 = Math.Cos(lat1r);
            double cl2 = Math.Cos(lat2r);
            double sl1 = Math.Sin(lat1r);
            double sl2 = Math.Sin(lat2r);
            double delta = lon2r - lon1r;
            double cdelta = Math.Cos(delta);
            double sdelta = Math.Sin(delta);

            //вычисления длины большого круга
            double yd = Math.Sqrt(Math.Pow(cl2 * sdelta, 2) + Math.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
            double xd = sl1 * sl2 + cl1 * cl2 * cdelta;
            double ad = Math.Atan2(yd, xd);
            distance = ad * rad;

            //вычисление начального азимута
            double xa = (cl1 * sl2) - (sl1 * cl2 * cdelta);
            double ya = sdelta * cl2;
            double za = Math.Atan(-ya / xa) * 180 / Math.PI;

            if (xa < 0)
            {
                za = za + 180;
            }
            double za2 = (za + 180) % 360 - 180;
            za2 = -za2 * Math.PI / 180;
            double anglerad2 = za2 - ((2 * Math.PI) * Math.Floor((za2 / (2 * Math.PI))));
            angledeg = (anglerad2 * 180) / Math.PI;
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

    }
    public partial class sat
    {
        //public int SatNum { get; set; }
        //public int SatLevel { get; set; }
        private int _SatNum = 0;
        public int SatNum
        {
            get { return _SatNum; }
            set { _SatNum = value; }
        }
        private int _Elevation = 0;
        public int Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; }
        }
        private int _Azimuth = 0;
        public int Azimuth
        {
            get { return _Azimuth; }
            set { _Azimuth = value; }
        }
        private int _SatLevel = 0;
        public int SatLevel
        {
            get { return _SatLevel; }
            set { _SatLevel = value; }
        }
    }
   
}
