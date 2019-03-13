using System;
using System.Collections.Generic;
using System.IO.Ports;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS
{
    // Code by Aleksandr Dikarev, dikarev-aleksandr@yandex.ru

    #region Utils

    public struct SatelliteData
    {
        #region Properties

        public int PRNNumber;
        public int Elevation;
        public int Azimuth;
        public int SNR;

        #endregion
       
        #region Constructor

        public SatelliteData(int prn, int elevation, int azimuth, int snr)
        {
            PRNNumber = prn;
            Elevation = elevation;
            Azimuth = azimuth;
            SNR = snr;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("PRN: {0:00}, Elevation: {1:00}°, Azimuth: {2:000}°, SNR: {3:00} dB", PRNNumber, Elevation, Azimuth, SNR);
        }

        #endregion
    }

    #endregion

    #region Custom eventArgs

    public class GLLEventArgs : EventArgs
    {
        #region Properties

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public DateTime TimeFix { get; private set; }

        #endregion

        #region Constructor

        public GLLEventArgs(double lat, double lon, DateTime timeFix)
        {
            Latitude = lat;
            Longitude = lon;
            TimeFix = timeFix;
        }

        #endregion
    }

    public class GGAEventArgs : EventArgs
    {
        #region Properties

        public string GPSQualityIndicator { get; private set; }
        public DateTime TimeFix { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public int SatellitesInUse { get; private set; }
        public double PrecisionHorizontalDilution { get; private set; }
        public double AntennaAltitude { get; private set; }
        public double GeoidalSeparation { get; private set; }
        public int DifferentianReferenceStation { get; private set; }

        #endregion

        #region Constructor

        public GGAEventArgs(string gpsQualityIndicator, DateTime timeFix, double lat, double lon, int satsInUse, double dohp, double antennaAlt, double gsep, int drs)
        {
            GPSQualityIndicator = gpsQualityIndicator;
            TimeFix = timeFix;
            Latitude = lat;
            Longitude = lon;
            SatellitesInUse = satsInUse;
            PrecisionHorizontalDilution = dohp;
            AntennaAltitude = antennaAlt;
            GeoidalSeparation = gsep;
            DifferentianReferenceStation = drs;
        }

        #endregion
    }

    public class GSVEventArgs : EventArgs
    {
        #region Properties

        public SatelliteData[] SatellitesData { get; private set; }

        #endregion

        #region Constructor

        public GSVEventArgs(SatelliteData[] satsData)
        {
            SatellitesData = satsData;
        }

        #endregion
    }

    public class VTGEventArgs : EventArgs
    {
        #region Properties

        public double TrackTrue { get; private set; }
        public double TrackMagnetic { get; private set; }
        public double SpeedKmh { get; private set; }

        #endregion

        #region Constructor

        public VTGEventArgs(double trackTrue, double trackMagnetic, double speedKmh)
        {
            TrackTrue = trackTrue;
            TrackMagnetic = TrackMagnetic;
            SpeedKmh = speedKmh;
        }

        #endregion
    }

    public class RMCEventArgs : EventArgs
    {
        #region Properties

        public DateTime TimeFix { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double SpeedKmh { get; private set; }
        public double TrackTrue { get; private set; }
        public double MagneticVariation { get; private set; }

        #endregion

        #region Constructor

        public RMCEventArgs(DateTime timeFix, double lat, double lon, double speedKmh, double trackTrue, double mVar)
        {
            TimeFix = timeFix;
            Latitude = lat;
            Longitude = lon;
            SpeedKmh = speedKmh;
            TrackTrue = trackTrue;
            MagneticVariation = mVar;
        }

        #endregion
    }

    public enum LogLineType
    {
        INFO,
        ERROR,
        CRITICAL
    }

    public class LogEventArgs : EventArgs
    {
        #region Properties

        public LogLineType EventType { get; private set; }
        public string LogString { get; private set; }

        #endregion

        #region Constructor

        public LogEventArgs(LogLineType eventType, Exception ex)
            : this(eventType, ex.StackTrace)
        {
        }

        public LogEventArgs(LogLineType eventType, string logString)
        {
            EventType = eventType;
            LogString = logString;
        }

        #endregion
    }

    #endregion

    public class GNSSReceiverWrapper
    {
        #region Properties

        NMEASerialPort port;
        private delegate void ProcessCommandDelegate(object[] parameters);
        private Dictionary<SentenceIdentifiers, ProcessCommandDelegate> cmdProcessor;

        delegate T NullChecker<T>(object parameter);
        NullChecker<int> intNullChecker = (x => x == null ? -1 : (int)x);
        NullChecker<double> doubleNullChecker = (x => x == null ? double.NaN : (double)x);
        NullChecker<string> stringNullChecker = (x => x == null ? string.Empty : (string)x);


        public SerialPortSettings PortSettings
        {
            get
            {
                return port.PortSettings;
            }
        }

        public bool IsOpen { get { return port.IsOpen; } }

        List<SatelliteData> fullSatellitesData;

        #endregion

        #region Constructor

        public GNSSReceiverWrapper(SerialPortSettings portSettings)
        {
            #region port initialization

            port = new NMEASerialPort(portSettings);

            port.PortError += new EventHandler<System.IO.Ports.SerialErrorReceivedEventArgs>(port_PortError);
            port.NewNMEAMessage += new EventHandler<NewNMEAMessageEventArgs>(port_NewMessage);

            #endregion

            #region parsers initialization

            cmdProcessor = new Dictionary<SentenceIdentifiers, ProcessCommandDelegate>()
            {
                { SentenceIdentifiers.GGA, new ProcessCommandDelegate(ProcessGGA)}//,
                //{ NMEA.SentenceIdentifiers.GSV, new ProcessCommandDelegate(ProcessGSV)},
                //{ NMEA.SentenceIdentifiers.GLL, new ProcessCommandDelegate(ProcessGLL)},
                //{ NMEA.SentenceIdentifiers.RMC, new ProcessCommandDelegate(ProcessRMC)},
                //{ NMEA.SentenceIdentifiers.VTG, new ProcessCommandDelegate(ProcessVTG)}                
            };

            #endregion
        }

        #endregion

        #region Methods

        #region Public

        public void Open()
        {
            port.Open();
        }

        public void Close()
        {
            port.Close();
        }

        public void SendRawCommand(string command)
        {
            port.SendData(command);            
        }

        #endregion

        #region Private

        private void ProcessGGA(object[] parameters)
        {
            if (GGAEvent != null)
            {
                var gpsQualityIndicator = (string)parameters[5];
                if (gpsQualityIndicator != "Fix not availible")
                {
                    try
                    {
                        var timeFix = (DateTime)parameters[0];
                        var lat = doubleNullChecker(parameters[1]);
                        var lon = doubleNullChecker(parameters[3]);
                        var satellitesInUse = intNullChecker(parameters[6]);
                        var precisionHorizontalDilution = doubleNullChecker(parameters[7]);
                        var antennaAltitude = doubleNullChecker(parameters[8]);
                        var antennaAltitudeUnits = (string)parameters[9];
                        var geoidalSeparation = doubleNullChecker(parameters[10]);
                        var geoidalSeparationUnits = (string)parameters[11];                        

                        var differentialReferenceStation = intNullChecker(parameters[12]);

                        GGAEvent(this,
                            new GGAEventArgs(
                                gpsQualityIndicator,
                                timeFix,
                                lat,
                                lon,
                                satellitesInUse,
                                precisionHorizontalDilution,
                                antennaAltitude,
                                geoidalSeparation,
                                differentialReferenceStation));
                    }
                    catch
                    {
                        //
                    }
                }
            }
        }

        private void ProcessGSV(object[] paramters)
        {
            if (GSVEvent != null)
            {
                try
                {
                    List<SatelliteData> satellites = new List<SatelliteData>();

                    int totalMessages = (int)paramters[0];
                    int currentMessageNumber = (int)paramters[1];

                    int satellitesDataItemsCount = (paramters.Length - 3) / 4;

                    for (int i = 0; i < satellitesDataItemsCount; i++)
                    {
                        satellites.Add(
                            new SatelliteData(
                                intNullChecker(paramters[3 + 4 * i]),
                                intNullChecker(paramters[4 + 4 * i]),
                                intNullChecker(paramters[5 + 4 * i]),
                                intNullChecker(paramters[6 + 4 * i])));
                    }

                    if (currentMessageNumber == 1)
                        fullSatellitesData = new List<SatelliteData>();                        

                    fullSatellitesData.AddRange(satellites.ToArray());

                    if (currentMessageNumber == totalMessages)
                        GSVEvent(this, new GSVEventArgs(fullSatellitesData.ToArray()));
                }
                catch
                {
                    //
                }
            }
        }

        private void ProcessGLL(object[] parameters)
        {
            if (GLLEvent != null)
            {
                try
                {
                    if (parameters[5].ToString() == "Valid")
                    {

                        var timeFix = (DateTime)parameters[4];

                        var lat = (double)parameters[0];
                        var latC = (Cardinals)Enum.Parse(typeof(Cardinals), (string)parameters[1]);
                        if (latC == Cardinals.South)
                            lat = -lat;

                        var lon = (double)parameters[2];
                        var lonC = (Cardinals)Enum.Parse(typeof(Cardinals), (string)parameters[3]);
                        if (lonC == Cardinals.West)
                            lon = -lon;

                        GLLEvent(this, new GLLEventArgs(lat, lon, timeFix));
                    }
                }
                catch
                {
                    //
                }
            }
        }

        private void ProcessVTG(object[] parameters)
        {
            if (VTGEvent != null)
            {
                try
                {
                    var trackTrue = doubleNullChecker(parameters[0]);
                    var trackMagnetic = doubleNullChecker(parameters[2]);
                    var speedKnots = doubleNullChecker(parameters[4]);
                    var skUnits = (string)parameters[5];
                    var speedKmh = doubleNullChecker(parameters[6]);
                    var sKmUnits = (string)parameters[7];


                    VTGEvent(this, new VTGEventArgs(trackTrue, trackMagnetic, speedKmh));
                }
                catch
                {
                    //
                }
            }
        }

        private void ProcessRMC(object[] parameters)
        {
            if (RMCEvent != null)
            {
                try
                {
                    if (parameters[1].ToString() == "Valid")
                    {
                        var timeFix = (DateTime)parameters[0];

                        var lat = (double)parameters[2];
                        var latC = (Cardinals)Enum.Parse(typeof(Cardinals), (string)parameters[3]);
                        if (latC == Cardinals.South)
                            lat = -lat;

                        var lon = (double)parameters[4];
                        var lonC = (Cardinals)Enum.Parse(typeof(Cardinals), (string)parameters[5]);
                        if (lonC == Cardinals.West)
                            lon = -lon;

                        var groundSpeed = (double)parameters[6];
                        var courseOverGround = (double)parameters[7];

                        var dateTime = (DateTime)parameters[8];

                        var magneticVariation = doubleNullChecker(parameters[9]);

                        RMCEvent(this, new RMCEventArgs(timeFix, lat, lon, NMEAParser.NM2Km(groundSpeed), courseOverGround, magneticVariation));
                    }
                }
                catch
                {
                    //
                }
            }
        }

        #endregion

        #endregion

        #region Handlers

        #region port

        private void port_PortError(object sender, SerialErrorReceivedEventArgs e)
        {
            if (PortError != null)
                PortError(this, e);

            if (LogEvent != null)
                LogEvent(this, new LogEventArgs(LogLineType.ERROR, string.Format("{0} in port {1}", e.EventType, port.PortSettings.PortName)));
        }

        private void port_NewMessage(object sender, NewNMEAMessageEventArgs e)
        {
            if (LogEvent != null)
                LogEvent(this, new LogEventArgs(LogLineType.INFO, e.Message));

            try
            {
                var result = NMEAParser.Parse(e.Message);

                if (result is NMEAStandartSentence)
                {
                    var sResult = (result as NMEAStandartSentence);
                    if (cmdProcessor.ContainsKey(sResult.SentenceID))
                        cmdProcessor[sResult.SentenceID](sResult.parameters);
                }
            }
            catch (Exception ex)
            {
                if (LogEvent != null)
                    LogEvent(this, new LogEventArgs(LogLineType.ERROR, ex));
            }
        }

        #endregion

        #endregion

        #region Events

        public EventHandler<GLLEventArgs> GLLEvent;
        public EventHandler<GGAEventArgs> GGAEvent;
        public EventHandler<GSVEventArgs> GSVEvent;
        public EventHandler<VTGEventArgs> VTGEvent;
        public EventHandler<RMCEventArgs> RMCEvent;
        public EventHandler<SerialErrorReceivedEventArgs> PortError;
        public EventHandler<LogEventArgs> LogEvent;

        #endregion
    }
}
