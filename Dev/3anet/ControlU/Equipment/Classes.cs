

namespace ControlU.Equipment
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using NpgsqlTypes;

    #region Connection
    /// <summary>
    /// Telnet Connection on port 8084 to an instrument
    /// </summary>
    internal class Connection : IDisposable
    {
        TcpClient m_Client;
        public NetworkStream m_Stream;
        bool m_IsOpen = false;
        string m_Hostname;
        int m_Port;
        int m_ReadTimeout = 1; // ms
        public delegate void ConnectionDelegate();
        public event ConnectionDelegate Opened;
        public event ConnectionDelegate Closed;
        public bool IsOpen { get { return m_IsOpen; } }
        public Connection() { }
        public Connection(bool open) : this("localhost", 5025, true) { }
        public Connection(string host, int port, bool open)
        {
            if (open)
                Open(host, port);
        }
        void CheckOpen()
        {
            if (!IsOpen)
                throw new Exception("Connection not open.");
        }
        public string Hostname
        {
            get { return m_Hostname; }
        }
        public int Port
        {
            get { return m_Port; }
        }
        public int ReadTimeout
        {
            set { m_ReadTimeout = value; if (IsOpen) m_Stream.ReadTimeout = value; }
            get { return m_ReadTimeout; }
        }
        public string Query(string str)
        {
            long WaitReadfrom = DateTime.Now.Ticks;
            //Write(@str);
            string t = "";
            Write(str);
            //bool t1 = !m_Stream.DataAvailable; //нет инфы
            //bool t2 = new TimeSpan(DateTime.Now.Ticks - WaitReadfrom) < new TimeSpan(0, 0, 0, 0, 1000);
            //bool t3 = true;
            //if(t1 && t2) t3 = true;
            //else t3 = false;
            while (!m_Stream.DataAvailable && new TimeSpan(DateTime.Now.Ticks - WaitReadfrom) < new TimeSpan(0, 0, 0, 0, 1000))
            {
                System.Threading.Thread.Sleep(5);
                //t1 = !m_Stream.DataAvailable;
                //t2 = new TimeSpan(DateTime.Now.Ticks - WaitReadfrom) < new TimeSpan(0, 0, 0, 0, 1000);
                //if (t1 && t2) t3 = true;
                //else t3 = false;
            }
            //while (!m_Stream.DataAvailable || new TimeSpan(DateTime.Now.Ticks - WaitReadfrom) < new TimeSpan(0, 0, 0, 0, 1000))
            //{
            t = Read().TrimEnd();
            //}
            return t;
        }
        public void Write(string str)
        {
            CheckOpen();
            byte[] bytes = System.Text.UnicodeEncoding.UTF8.GetBytes(@str + "\r\n");


            m_Stream.Write(bytes, 0, bytes.Length);
            m_Stream.Flush();
        }

        public string Read()
        {
            CheckOpen();
            return System.Text.UnicodeEncoding.UTF8.GetString(ReadBytes());
        }

        /// <summary>
        /// Reads bytes from the socket and returns them as a byte[].
        /// </summary>
        /// <returns></returns>       
        public byte[] ReadBytes()
        {
            var bytes = new List<byte>();
            try
            {
                int i = m_Stream.ReadByte();
                byte b = (byte)i;
                int bytesToRead = 0;
                if ((char)b == '#')
                {
                    bytesToRead = ReadLengthHeader();
                    if (bytesToRead > 0)
                    {
                        i = m_Stream.ReadByte();
                        if ((char)i != '\n') // discard carriage return after length header.
                            bytes.Add((byte)i);
                    }
                }
                if (bytesToRead == 0)
                {
                    while (i != -1 && b != (byte)'\n')
                    {
                        bytes.Add(b);
                        i = m_Stream.ReadByte();
                        b = (byte)i;
                    }
                }
                else
                {
                    int bytesRead = 0;
                    while (bytesRead < bytesToRead && i != -1)
                    {
                        i = m_Stream.ReadByte();
                        if (i != -1)
                        {
                            bytesRead++;
                            // record all bytes except \n if it is the last char.
                            if (bytesRead < bytesToRead || (char)i != '\n')
                                bytes.Add((byte)i);
                        }
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "Connection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            return bytes.ToArray();
        }
        int ReadLengthHeader()
        {
            int numDigits = Convert.ToInt32(new string(new char[] { (char)m_Stream.ReadByte() }));
            string bytes = "";
            for (int i = 0; i < numDigits; ++i)
                bytes = bytes + (char)m_Stream.ReadByte();
            return Convert.ToInt32(bytes);
        }
        public void Open(string hostname, int port)
        {
            try
            {
                if (IsOpen)
                    Close();
                m_Hostname = hostname;
                m_Port = port;
                m_Client = new TcpClient(hostname, port);//5025
                m_Client.ReceiveBufferSize = 120000;
                m_Stream = m_Client.GetStream();
                m_Stream.ReadTimeout = ReadTimeout;
                m_IsOpen = true;
                if (Opened != null)
                    Opened();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "Connection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public void Close()
        {
            try
            {
                if (!m_IsOpen)
                    return;
                m_Stream.Close();
                m_Client.Close();
                m_IsOpen = false;
                if (Closed != null)
                    Closed();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "Connection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public void Dispose()
        {
            Close();
        }
    }
    #endregion
    #region TelnetConnection
    /// <summary>
    /// Telnet Connection on port 5025 to an instrument
    /// </summary>
    public class TelnetConnection : IDisposable
    {
        TcpClient m_Client;
        NetworkStream m_Stream;
        bool m_IsOpen = false;
        string m_Hostname;
        int m_Port;
        public delegate void ConnectionDelegate();
        public event ConnectionDelegate Opened;
        public event ConnectionDelegate Closed;
        public bool IsOpen { get { return m_IsOpen; } }
        public TelnetConnection() { }
        public TelnetConnection(bool open) : this("localhost", 5025, true) { }
        public TelnetConnection(string host, int port, bool open)
        {
            if (open)
                Open(host, port);
        }
        void CheckOpen()
        {
            try
            {
                if (!IsOpen)
                    throw new Exception("Connection not open.");
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TelnetConnection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public string Hostname
        {
            get { return m_Hostname; }
        }
        public int Port
        {
            get { return m_Port; }
        }
        public string Query(string str)
        {
            string t = "";
            try
            {
                //long WaitReadfrom = DateTime.Now.Ticks;
                //WriteLine(str);
                //while (!m_Stream.DataAvailable || new TimeSpan(DateTime.Now.Ticks - WaitReadfrom) < new TimeSpan(0, 0, 0, 0, 1000))
                //{
                //    System.Threading.Thread.Sleep(2);
                //}
                //string t = Read();

                //new
                long WaitReadfrom = DateTime.Now.Ticks;
                WriteLine(str);
                while (!m_Stream.DataAvailable && new TimeSpan(DateTime.Now.Ticks - WaitReadfrom) < new TimeSpan(0, 0, 0, 0, 1000))
                {
                    System.Threading.Thread.Sleep(5);
                }
                t = Read();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TelnetConnection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            return t;
        }
        public void Write(string str)
        {
            try
            {
                CheckOpen();
                byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
                m_Stream.Write(bytes, 0, bytes.Length);
                m_Stream.Flush();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TelnetConnection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public void WriteLine(string str)
        {
            try
            {
                CheckOpen();
                byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
                m_Stream.Write(bytes, 0, bytes.Length);
                WriteTerminator();
                m_Stream.Flush();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TelnetConnection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        void WriteTerminator()
        {
            try
            {
                byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes("\n");
                m_Stream.Write(bytes, 0, bytes.Length);
                m_Stream.Flush();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TelnetConnection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public string Read()
        {
            CheckOpen();
            return System.Text.ASCIIEncoding.ASCII.GetString(ReadBytes());
        }
        public byte[] ReadBytes()
        {
            var bytes = new List<byte>();
            try
            {
                int i = m_Stream.ReadByte();
                byte b = (byte)i;
                int bytesToRead = 0;

                if ((char)b == '#')
                {
                    bytesToRead = ReadLengthHeader();
                    if (bytesToRead > 0)
                    {
                        i = m_Stream.ReadByte();
                        if ((char)i != '\n') // discard carriage return after length header.
                            bytes.Add((byte)i);
                    }
                }
                if (bytesToRead == 0)
                {
                    while (i != -1 && b != (byte)'\n')
                    {
                        bytes.Add(b);
                        i = m_Stream.ReadByte();
                        b = (byte)i;
                    }
                }
                else
                {
                    int bytesRead = 0;
                    while (bytesRead < bytesToRead && i != -1)
                    {
                        i = m_Stream.ReadByte();
                        if (i != -1)
                        {
                            bytesRead++;
                            // record all bytes except \n if it is the last char.
                            if (bytesRead < bytesToRead || (char)i != '\n')
                                bytes.Add((byte)i);
                        }
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TelnetConnection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            return bytes.ToArray();
        }
        int ReadLengthHeader()
        {
            int numDigits = Convert.ToInt32(new string(new char[] { (char)m_Stream.ReadByte() }));
            string bytes = "";
            for (int i = 0; i < numDigits; ++i)
                bytes = bytes + (char)m_Stream.ReadByte();
            return Convert.ToInt32(bytes);
        }
        public void Open(string hostname, int port)
        {
            try
            {
                if (IsOpen)
                    Close();
                m_Hostname = hostname;
                m_Port = port;
                m_Client = new TcpClient(hostname, port);//5025
                m_Client.ReceiveBufferSize = 200000;
                m_Stream = m_Client.GetStream();
                m_Stream.ReadTimeout = 1;
                m_IsOpen = true;
                if (Opened != null)
                    Opened();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TelnetConnection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public void Close()
        {
            try
            {
                if (!m_IsOpen)
                    return;
                m_Stream.Close();
                m_Client.Close();
                m_IsOpen = false;
                if (Closed != null)
                    Closed();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "TelnetConnection", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public void Dispose()
        {
            Close();
        }
    }
    #endregion
    #region UdpStreaming
    /// <summary>
    /// UDP Stream Connection
    /// </summary>
    class UdpStreaming : IDisposable
    {
        UdpClient m_Client;
        IPEndPoint RemoteIpEndPoint;
        bool m_IsOpen = false;
        string m_Hostname;
        int m_Port;
        public delegate void ConnectionDelegate();
        public event ConnectionDelegate Opened;
        public event ConnectionDelegate Closed;
        public bool IsOpen { get { return m_IsOpen; } }
        public UdpStreaming() { }
        public UdpStreaming(bool open) : this("localhost", 23023, true) { }
        public UdpStreaming(string host, int port, bool open)
        {
            if (open)
                Open(host, port);
        }
        void CheckOpen()
        {
            if (!IsOpen)
                throw new Exception("Connection not open.");
        }
        public string Hostname
        {
            get { return m_Hostname; }
        }
        public int Port
        {
            get { return m_Port; }
        }
        public string Read()
        {
            CheckOpen();
            string returnData = "";
            try
            {
                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = m_Client.Receive(ref RemoteIpEndPoint);
                returnData = System.Text.ASCIIEncoding.ASCII.GetString(receiveBytes);
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "UdpStreaming", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            return returnData;
        }
        public Byte[] ByteRead()
        {
            CheckOpen();
            Byte[] returnData = new Byte[] { };
            try
            {
                // Blocks until a message returns on this socket from a remote host.                
                returnData = m_Client.Receive(ref RemoteIpEndPoint);
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "UdpStreaming", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            return returnData;
        }
        public double[] ByteRead1()
        {
            CheckOpen();
            Byte[] returnData = new Byte[] { };
            double[] temp = new double[] { };
            try
            {
                // Blocks until a message returns on this socket from a remote host.
                returnData = m_Client.Receive(ref RemoteIpEndPoint);
                double[] t = new double[((returnData.Length - 30) / 2)];

                for (int i = 0; i < (t.Length - 30) / 2; i++)
                {
                    t[i] = ((double)BitConverter.ToInt16(returnData, i * 2 + 29)) / 10;
                }
                temp = t;
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "UdpStreaming", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            return temp;
        }

        public void Open(string hostname, int port)
        {
            try
            {
                if (IsOpen)
                    Close();
                m_Hostname = hostname;
                m_Port = port;
                m_Client = new UdpClient(port);//23023
                                               //Creates an IPEndPoint to record the IP Address and port number of the sender. 
                                               // The IPEndPoint will allow you to read datagrams sent from any source.
                RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);

                m_IsOpen = true;
                if (Opened != null)
                    Opened();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "UdpStreaming", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public void Close()
        {
            try
            {
                if (!m_IsOpen)
                    return;
                m_Client.Close();
                m_IsOpen = false;
                if (Closed != null)
                    Closed();
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "UdpStreaming", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public void Dispose()
        {
            Close();
        }
    }
    #endregion

    public class InstrManufacrures : PropertyChangedBase
    {
        public InstrManufacrures() { }
        public ParamWithUI Unk = new ParamWithUI() { Parameter = "0", UI = "Unknown", UIShort = "Unk" };
        public ParamWithUI RuS = new ParamWithUI() { Parameter = "1", UI = "Rohde&Schwarz", UIShort = "R&S" };
        public ParamWithUI Keysight = new ParamWithUI() { Parameter = "2", UI = "Keysight Technologies", UIShort = "Keysight" };
        public ParamWithUI Anritsu = new ParamWithUI() { Parameter = "3", UI = "Anritsu", UIShort = "Anritsu" };

    }
    /// <summary>
    /// 0 = dBm
    /// 1 = dBmV
    /// 2 = dBµV
    /// 3 = dBµV/m
    /// 4 = mV
    /// 5 = µV
    /// </summary>
    public class AllLevelUnits : PropertyChangedBase
    {
        public AllLevelUnits() { }
        /// <summary>
        /// 0 = dBm
        /// 1 = dBmV
        /// 2 = dBµV
        /// 3 = dBµV/m
        /// 4 = mV
        /// 5 = µV
        /// </summary>
        //public ObservableCollection<LevelUnit> LevelUnits = new ObservableCollection<LevelUnit>()
        //{
        //    new LevelUnit() { ind = 0, IsEnabled = true, AnParameter = "DBM", UI = "dBm"},
        //    new LevelUnit() { ind = 1, IsEnabled = true, AnParameter = "DBMV", UI = "dBmV"},
        //    new LevelUnit() { ind = 2, IsEnabled = true, AnParameter = "DBUV", UI = "dBµV"},
        //    new LevelUnit() { ind = 3, IsEnabled = true, AnParameter = "DBUV/M", UI = "dBµV/m"},
        //    new LevelUnit() { ind = 4, IsEnabled = true, AnParameter = "", UI = "mV"},
        //    new LevelUnit() { ind = 5, IsEnabled = true, AnParameter = "", UI = "µV"},
        //};
        public LevelUnit dBm = new LevelUnit() { ind = 0, IsEnabled = true, AnParameter = "DBM", UI = "dBm" };
        public LevelUnit dBmV = new LevelUnit() { ind = 1, IsEnabled = true, AnParameter = "DBMV", UI = "dBmV" };
        public LevelUnit dBµV = new LevelUnit() { ind = 2, IsEnabled = true, AnParameter = "DBUV", UI = "dBµV" };
        public LevelUnit dBµVm = new LevelUnit() { ind = 3, IsEnabled = true, AnParameter = "DBUV/M", UI = "dBµV/m" };
        public LevelUnit mV = new LevelUnit() { ind = 4, IsEnabled = true, AnParameter = "", UI = "mV" };
        public LevelUnit µV = new LevelUnit() { ind = 5, IsEnabled = true, AnParameter = "", UI = "µV" };

    }
    public class AllTraceTypes : PropertyChangedBase
    {
        public AllTraceTypes() { }
        /// <summary>
        /// 0 = Clear Write
        /// 1 = Average
        /// 2 = Tracking
        /// 3 = Max Hold
        /// 4 = Min Hold
        /// 5 = View
        /// 6 = Blank
        /// </summary>
        public ObservableCollection<ParamWithUI> TraceTypes = new ObservableCollection<ParamWithUI>()
        {
            new ParamWithUI() { Parameter = "0", UI = "Clear Write" },
            new ParamWithUI() { Parameter = "1", UI = "Average" },
            new ParamWithUI() { Parameter = "2", UI = "Tracking" },
            new ParamWithUI() { Parameter = "3", UI = "Max Hold" },
            new ParamWithUI() { Parameter = "4", UI = "Min Hold" },
            new ParamWithUI() { Parameter = "5", UI = "View" },
            new ParamWithUI() { Parameter = "6", UI = "Blank" },
        };
    }
    public class LevelUnit : PropertyChangedBase
    {
        public string UI
        {
            get { return _UI; }
            set { _UI = value; OnPropertyChanged("UI"); }
        }
        private string _UI = "";
        public int ind { get; set; }
        public string AnParameter { get; set; }
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }
        private bool _IsEnabled = false;
    }
    public enum InstrumentType : int
    {
        SpectrumAnalyzer = 1,
        RuSReceiver = 2,
        RuSTSMx = 3,
        SignalHound = 5,
    }
    public class AverageList : PropertyChangedBase
    {
        public void AddTraceToAverade(tracepoint[] NewTrace)
        {
            //сброситрейс если че
            if (TracesToAverage.Count == 0 || NewTrace.Length != TracesToAverage[0].Length || //несовпали длины
                NewTrace[0].freq != TracesToAverage[0][0].freq ||//несовпали начальные частоты
                NewTrace[NewTrace.Length - 1].freq != TracesToAverage[0][TracesToAverage[0].Length - 1].freq)//несовпали конечные частоты
            {
                TracesToAverage.Clear();
                TracePoints = NewTrace.Length;
                AveragedTrace = new tracepoint[TracePoints];
                for (int i = 0; i < TracePoints; i++)
                {
                    AveragedTrace[i] = new tracepoint() { freq = NewTrace[i].freq, level = NewTrace[i].level };
                }
                NumberOfSweeps = 0;
                tracepoint[] tr = new tracepoint[TracePoints];
                for (int i = 0; i < TracePoints; i++)
                {
                    tr[i] = new tracepoint() { freq = NewTrace[i].freq, level = NewTrace[i].level };
                }
                TracesToAverage.Add(tr);
            }
            else if (TracesToAverage.Count < _AveragingCount)//несовпали конечные частоты
            {
                tracepoint[] tr = new tracepoint[TracePoints];
                for (int i = 0; i < TracePoints; i++)
                {
                    tr[i] = new tracepoint() { freq = NewTrace[i].freq, level = NewTrace[i].level };
                }
                TracesToAverage.Add(tr);
                Calc();
            }
            else if (TracesToAverage.Count >= _AveragingCount)
            {
                while (TracesToAverage.Count > _AveragingCount - 1)
                {
                    TracesToAverage.RemoveAt(0);
                }

                tracepoint[] tr = new tracepoint[TracePoints];
                for (int i = 0; i < TracePoints; i++)
                {
                    tr[i] = new tracepoint() { freq = NewTrace[i].freq, level = NewTrace[i].level };
                }
                TracesToAverage.Add(tr);
                Calc();
            }
        }

        public void Reset()
        {
            TracesToAverage.Clear();
        }

        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; OnPropertyChanged("TracePoints"); }
        }
        private int _TracePoints = 1601;

        /// <summary>
        /// Количевство на усреднение
        /// </summary>
        public int AveragingCount
        {
            get { return _AveragingCount; }
            set
            {
                if (value < 2) _AveragingCount = 2;
                else if (value > 500) _AveragingCount = 500;
                else _AveragingCount = value;
                OnPropertyChanged("AveragingCount");
            }
        }
        private int _AveragingCount = 10;

        /// <summary>
        /// текущее значение (сколько трейсов усреднено)
        /// </summary>
        public int NumberOfSweeps
        {
            get { return _NumberOfSweeps; }
            set { if (_NumberOfSweeps != value) { _NumberOfSweeps = value; OnPropertyChanged("NumberOfSweeps"); } }
        }
        private int _NumberOfSweeps = 0;



        public tracepoint[] AveragedTrace;
        private List<tracepoint[]> TracesToAverage = new List<tracepoint[]> { };

        private void Calc()
        {
            NumberOfSweeps = TracesToAverage.Count;

            //int count = TracesToAverage.Count;
            if (NumberOfSweeps > 1)
            {
                for (int x = 0; x < TracePoints; x++)
                {
                    double tl = 0;
                    for (int i = 0; i < NumberOfSweeps; i++)
                    {
                        if (i == 0) tl = TracesToAverage[0][x].level;
                        else tl += TracesToAverage[i][x].level;
                    }
                    AveragedTrace[x].level = tl / NumberOfSweeps;
                }
            }
        }
    }
    public class TrackingList : PropertyChangedBase
    {
        public void AddTraceToTracking(tracepoint[] NewTrace)
        {
            //сброситрейс если че
            if (TracesToTracking.Count == 0 || NewTrace.Length != TracesToTracking[0].Length || //несовпали длины
                NewTrace[0].freq != TracesToTracking[0][0].freq ||//несовпали начальные частоты
                NewTrace[NewTrace.Length - 1].freq != TracesToTracking[0][TracesToTracking[0].Length - 1].freq)//несовпали конечные частоты
            {
                TracesToTracking.Clear();
                TracePoints = NewTrace.Length;
                TrackingTrace = new tracepoint[TracePoints];
                for (int i = 0; i < TracePoints; i++)
                {
                    TrackingTrace[i] = new tracepoint() { freq = NewTrace[i].freq, level = NewTrace[i].level };
                }
                NumberOfSweeps = 0;
                tracepoint[] tr = new tracepoint[TracePoints];
                for (int i = 0; i < TracePoints; i++)
                {
                    tr[i] = new tracepoint() { freq = NewTrace[i].freq, level = NewTrace[i].level };
                }
                TracesToTracking.Add(tr);
            }
            else if (TracesToTracking.Count < _TrackingCount)//несовпали конечные частоты
            {
                tracepoint[] tr = new tracepoint[TracePoints];
                for (int i = 0; i < TracePoints; i++)
                {
                    tr[i] = new tracepoint() { freq = NewTrace[i].freq, level = NewTrace[i].level };
                }
                TracesToTracking.Add(tr);
                Calc();
            }
            else if (TracesToTracking.Count >= _TrackingCount)
            {
                while (TracesToTracking.Count > _TrackingCount - 1)
                {
                    TracesToTracking.RemoveAt(0);
                }

                tracepoint[] tr = new tracepoint[TracePoints];
                for (int i = 0; i < TracePoints; i++)
                {
                    tr[i] = new tracepoint() { freq = NewTrace[i].freq, level = NewTrace[i].level };
                }
                TracesToTracking.Add(tr);
                Calc();
            }
        }
        public void Reset()
        {
            TracesToTracking.Clear();
        }

        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; OnPropertyChanged("TracePoints"); }
        }
        private int _TracePoints = 1601;

        /// <summary>
        /// Количевство на усреднение
        /// </summary>
        public int TrackingCount
        {
            get { return _TrackingCount; }
            set
            {
                if (value < 2) _TrackingCount = 2;
                else if (value > 500) _TrackingCount = 500;
                else _TrackingCount = value;
                OnPropertyChanged("TrackingCount");
            }
        }
        private int _TrackingCount = 10;

        /// <summary>
        /// текущее значение (сколько трейсов усреднено)
        /// </summary>
        public int NumberOfSweeps
        {
            get { return _NumberOfSweeps; }
            set { if (_NumberOfSweeps != value) { _NumberOfSweeps = value; OnPropertyChanged("NumberOfSweeps"); } }
        }
        private int _NumberOfSweeps = 0;



        public tracepoint[] TrackingTrace;
        private List<tracepoint[]> TracesToTracking = new List<tracepoint[]> { };

        private void Calc()
        {
            NumberOfSweeps = TracesToTracking.Count;

            //int count = TracesToAverage.Count;
            if (NumberOfSweeps > 0)
            {
                for (int x = 0; x < TracePoints; x++)
                {
                    for (int i = 0; i < NumberOfSweeps; i++)
                    {
                        if (i == 0) TrackingTrace[x].level = TracesToTracking[0][x].level;
                        if (TracesToTracking[i][x].level > TrackingTrace[x].level) TrackingTrace[x].level = TracesToTracking[i][x].level;
                    }
                }
            }
        }
    }

    public partial class tracepoint
    {
        public decimal freq { get; set; }
        public double level { get; set; }
    }





    public enum GSMBands : int
    {
        GSM450 = 1,
        GSM480 = 2,
        GSM750 = 3,
        P_GSM900 = 4,
        E_GSM900 = 5,
        R_GSM900 = 6,
        ER_GSM900 = 7,
        GSM1800 = 8,
        GSM850 = 9,
        GSM1900 = 10
    }
    enum UMTSBands : int
    {
        Band_1_2100 = 1,
        Band_2_1900 = 2,
        Band_3_1800 = 3,
        Band_4_1700 = 4,
        Band_5_850 = 5,
        Band_6_800 = 6,
        Band_7_2600 = 7,
        Band_8_900 = 8,
        Band_9_1700 = 9,
        Band_10_1700 = 10,
        Band_11_1500 = 11,
        Band_12_700 = 12,
        Band_13_700 = 13,
        Band_14_700 = 14,
        Band_15_Reserved = 15,
        Band_16_Reserved = 16,
        Band_17_Reserved = 17,
        Band_18_Reserved = 18,
        Band_19_800 = 19,
        Band_20_800 = 20,
        Band_21_1500 = 21,
        Band_22_3500 = 22,
        Band_23_Reserved = 23,
        Band_24_Reserved = 24,
        Band_25_1900 = 25,
        Band_26_850 = 26,
        Band_27_Reserved = 27,
        Band_28_Reserved = 28,
        Band_29_Reserved = 29,
        Band_30_Reserved = 30,
        Band_31_Reserved = 31,
        Band_32_1500 = 32
    }
    enum CDMABands : int
    {
        Band_0_800 = 0,
        Band_1_1900_PCS = 1,
        Band_2_TACS = 2,
        Band_3_JTACS = 3,
        Band_4_Korean_PCS = 4,
        Band_5_450 = 5,
        Band_6_2_GHz = 6,
        Band_7_700_Upper = 7,
        Band_8_1800 = 8,
        Band_9_900 = 9,
        Band_10_Sec_800_MHz = 10,
        Band_11_400_MHz_PAMR = 11,
        Band_12_800_MHz_PAMR = 12,
        Band_13_2500_MHz_IMT2000_Ext = 13,
        Band_14_US_PCS_1900_MHz = 14,
        Band_15_AWS = 15,
        Band_16_US_2500_MHz = 16,
        Band_17_2500_MHz = 17,
        Band_18_700_PS = 18,
        Band_19_700_Lower = 19,
        Band_20_1600_L_Band = 20,
        Band_21_2000_S_Band_A = 21,
        Band_22_2000_S_Band_B = 22
    }
    enum WiMaxBands : int
    {
        Band_0_2300_MHz = 0,
        Band_1_2300_MHz = 1,
        Band_2_2300_MHz = 2,
        Band_3_2300_MHz = 3,
        Band_4_2300_MHz = 4,
        Band_5_2300_MHz = 5,
        Band_6_2300_MHz = 6,
        Band_7_2300_MHz = 7,
    }

    public class GSM_Band : INotifyPropertyChanged
    {
        private GSMBands _Band;
        public GSMBands Band
        {
            get { return _Band; }
            set { _Band = value; OnPropertyChanged("Band"); }
        }
        private int _ARFCNStart = 0;
        public int ARFCNStart
        {
            get { return _ARFCNStart; }
            set { _ARFCNStart = value; }
        }
        private int _ARFCNStop = 0;
        public int ARFCNStop
        {
            get { return _ARFCNStop; }
            set { _ARFCNStop = value; }
        }
        private decimal _FreqUpStart = 0;
        public decimal FreqUpStart
        {
            get { return _FreqUpStart; }
            set { _FreqUpStart = value; }
        }
        private decimal _FreqUpStop = 0;
        public decimal FreqUpStop
        {
            get { return _FreqUpStop; }
            set { _FreqUpStop = value; }
        }
        private decimal _FreqDnStart = 0;
        public decimal FreqDnStart
        {
            get { return _FreqDnStart; }
            set { _FreqDnStart = value; }
        }
        private decimal _FreqDnStop = 0;
        public decimal FreqDnStop
        {
            get { return _FreqDnStop; }
            set { _FreqDnStop = value; }
        }
        private List<GSM_BandFreqs> _FreqData;
        public List<GSM_BandFreqs> FreqData
        {
            get { return _FreqData; }
            set { _FreqData = value; OnPropertyChanged("FreqData"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class GSM_BandFreqs
    {
        public int ARFCN { get; set; }
        public decimal FreqUp { get; set; }
        public decimal FreqDn { get; set; }
    }
    public class GSM_Channel : INotifyPropertyChanged
    {
        private int _ARFCN = 0;
        public int ARFCN
        {
            get { return _ARFCN; }
            set { _ARFCN = value; OnPropertyChanged("ARFCN"); }
        }
        private decimal _FreqUp = 0;
        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqDn = 0;
        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private string _StandartSubband;
        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class UMTS_Channel : INotifyPropertyChanged
    {
        private int _UARFCN_DN = 0;
        public int UARFCN_DN
        {
            get { return _UARFCN_DN; }
            set { _UARFCN_DN = value; OnPropertyChanged("UARFCN_DN"); }
        }
        private int _UARFCN_UP = 0;
        public int UARFCN_UP
        {
            get { return _UARFCN_UP; }
            set { _UARFCN_UP = value; OnPropertyChanged("UARFCN_UP"); }
        }
        private decimal _FreqUp = 0;
        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqDn = 0;
        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private string _StandartSubband = "";
        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LTE_Channel : INotifyPropertyChanged
    {
        private int _EARFCN_DN = 0;
        public int EARFCN_DN
        {
            get { return _EARFCN_DN; }
            set { _EARFCN_DN = value; OnPropertyChanged("EARFCN_DN"); }
        }
        private int _EARFCN_UP = 0;
        public int EARFCN_UP
        {
            get { return _EARFCN_UP; }
            set { _EARFCN_UP = value; OnPropertyChanged("EARFCN_UP"); }
        }
        private decimal _FreqUp = 0;
        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqDn = 0;
        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }

        private string _StandartSubband = "";
        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CDMA_Channel : INotifyPropertyChanged
    {
        private int _ChannelN = 0;
        public int ChannelN
        {
            get { return _ChannelN; }
            set { _ChannelN = value; OnPropertyChanged("ChannelN"); }
        }
        private decimal _FreqUp = 0;
        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqDn = 0;
        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private string _StandartSubband = "";
        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class WiMax_Channel : INotifyPropertyChanged
    {
        private int _ChannelN = 0;
        public int ChannelN
        {
            get { return _ChannelN; }
            set { _ChannelN = value; OnPropertyChanged("ChannelN"); }
        }
        private decimal _FreqUp = 0;
        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqDn = 0;
        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private decimal _Bandwidth = 0;
        public decimal Bandwidth
        {
            get { return _Bandwidth; }
            set { _Bandwidth = value; OnPropertyChanged("Bandwidth"); }
        }
        private string _StandartSubband = "";
        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class Marker : PropertyChangedBase
    {
        public bool State
        {
            get { return _State; }
            set { _State = value; OnPropertyChanged("State"); }
        }
        private bool _State = false;
        public bool StateNew = false;
        public string Name
        {
            get
            {
                string str = "";
                if (MarkerType == 0) { str = "M"; }
                else if (MarkerType == 1) { str = "D"; }
                else if (MarkerType == 2) { str = "T"; }
                else if (MarkerType == 3) { str = "M"; }
                else if (MarkerType == 4) { str = "M"; }
                str += Index.ToString();
                return str;
            }
            private set { }
        }

        public int Index
        {
            get { return _Index; }
            set
            {
                _Index = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("Index");
            }
        }
        private int _Index = 0;

        /// <summary>
        /// 0 = M маркер
        /// 1 = D маркер
        /// 2 = T маркер
        /// 3 = M маркер с NDB
        /// 4 = M маркер с OBW
        /// </summary>
        public int MarkerType
        {
            get { return _MarkerType; }
            set
            {
                _MarkerType = value;
                if (_MarkerType == 3)
                {
                    this.TMarkers = new ObservableCollection<Marker>()
                        {
                            new Marker() { State = true, MarkerParent = this, Index = 1, MarkerType = 2, FunctionDataType = 3, IndexOnTrace = IndexOnTrace, TraceNumber = this.TraceNumber, Freq = 1824100000 },
                            new Marker() { State = true, MarkerParent = this, Index = 2, MarkerType = 2, FunctionDataType = 4, IndexOnTrace = IndexOnTrace, TraceNumber = this.TraceNumber, Freq = 1823900000 }
                        };
                }
                else if (_MarkerType == 4)
                {
                    this.TMarkers = new ObservableCollection<Marker>()
                        {
                            new Marker() { State = true, MarkerParent = this, Index = 1, MarkerType = 2, FunctionDataType = 6, IndexOnTrace = IndexOnTrace, TraceNumber = this.TraceNumber, Freq = 1824100000 },
                            new Marker() { State = true, MarkerParent = this, Index = 2, MarkerType = 2, FunctionDataType = 7, IndexOnTrace = IndexOnTrace, TraceNumber = this.TraceNumber, Freq = 1823900000 }
                        };
                }
                else { this.TMarkers = new ObservableCollection<Marker>(); }
                OnPropertyChanged("Name"); OnPropertyChanged("MarkerType");
            }
        }
        private int _MarkerType = 0;
        public int MarkerTypeNew = 0;

        public Marker MarkerParent
        {
            get { return _MarkerParent; }
            set { _MarkerParent = value; OnPropertyChanged("MarkerParent"); }
        }
        private Marker _MarkerParent;

        /// <summary>
        /// индекс в массиве трейса
        /// </summary>
        public int IndexOnTrace
        {
            get { return _IndexOnTrace; }
            set { _IndexOnTrace = value; }
        }
        private int _IndexOnTrace = -1;

        public int TraceNumberIndex
        {
            get { return _TraceNumberIndex; }
            set
            {

                if (_TraceNumberIndex < 0) _TraceNumberIndex = 0;
                else if (_TraceNumberIndex > 2) _TraceNumberIndex = 2;
                else _TraceNumberIndex = value;
                if (_TraceNumberIndex == 0) TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1" };
                else if (_TraceNumberIndex == 1) TraceNumber = new ParamWithUI() { Parameter = "1", UI = "Trace 2" };
                else if (_TraceNumberIndex == 2) TraceNumber = new ParamWithUI() { Parameter = "2", UI = "Trace 3" };
                OnPropertyChanged("TraceNumberIndex");
            }
        }
        private int _TraceNumberIndex = 0;
        /// <summary>
        /// на каком трейсе находится
        /// </summary>
        public ParamWithUI TraceNumber
        {
            get { return _TraceNumber; }
            set { _TraceNumber = value; OnPropertyChanged("TraceNumber"); }
        }
        private ParamWithUI _TraceNumber = new ParamWithUI() { Parameter = "0", UI = "Trace 1" };

        /// <summary>
        /// если ввиду разные шкалы запилить обновление
        /// </summary>
        public string LevelUnit
        {
            get { return _LevelUnit; }
            set { _LevelUnit = value; OnPropertyChanged("LevelUnit"); }
        }
        private string _LevelUnit = "dBm";

        /// <summary>
        /// частота в Гц не меняется от чихов трейса
        /// </summary>
        public decimal Freq
        {
            get { return _Freq; }
            set { _Freq = value; OnPropertyChanged("Freq"); }
        }
        private decimal _Freq = 0;
        public decimal FreqNew = 0;


        public double Level
        {
            get { return _Level; }
            set { _Level = value; OnPropertyChanged("Level"); }
        }
        private double _Level = 0;




        /// <summary>
        /// 0 = ничего
        /// 1 = D маркер
        /// 2 = NDB маркер
        /// 3 = результаты NDB измеренная полоса сигнала
        /// 4 = результаты NDB Q factor
        /// 5 = OBW маркер
        /// 6 = результаты OBW измеренная полоса сигнала
        /// 7 = результаты OBW 
        /// </summary>
        public int FunctionDataType
        {
            get { return _FunctionDataType; }
            set { _FunctionDataType = value; OnPropertyChanged("FunctionDataType"); }
        }
        private int _FunctionDataType = 0;

        public decimal Funk1
        {
            get { return _Funk1; }
            set { _Funk1 = value; OnPropertyChanged("Funk1"); }
        }
        private decimal _Funk1 = 0;

        public decimal Funk2
        {
            get { return _Funk2; }
            set { _Funk2 = value; OnPropertyChanged("Funk2"); }
        }
        private decimal _Funk2 = 0;


        public ObservableCollection<Marker> TMarkers
        {
            get { return _TMarkers; }
            set { _TMarkers = value; OnPropertyChanged("TMarkers"); }
        }
        private ObservableCollection<Marker> _TMarkers;
    }

    /// <summary>
    /// All in dBm Hz
    /// </summary>
    public class spectrum_data : PropertyChangedBase
    {
        [PgName("rbw")]
        public decimal RBW
        {
            get { return _RBW; }
            set { _RBW = value; /*OnPropertyChanged("RBW");*/ }
        }
        private decimal _RBW = 0;

        /// <summary>
        /// не испельзуется то -1
        /// </summary>
        [PgName("vbw")]
        public decimal VBW
        {
            get { return _VBW; }
            set { _VBW = value; /*OnPropertyChanged("VBW");*/ }
        }
        private decimal _VBW = 0;        

        [PgName("freq_start")]
        public decimal FreqStart
        {
            get { return _FreqStart; }
            set { _FreqStart = value; /*OnPropertyChanged("FreqStart");*/ }
        }
        private decimal _FreqStart = 0;

        [PgName("freq_centr")]
        public decimal FreqCentr
        {
            get { return _FreqCentr; }
            set { _FreqCentr = value; OnPropertyChanged("FreqCentr"); }
        }
        private decimal _FreqCentr = 0;

        [PgName("freq_stop")]
        public decimal FreqStop
        {
            get { return _FreqStop; }
            set { _FreqStop = value; /*OnPropertyChanged("FreqStop");*/ }
        }
        private decimal _FreqStop = 0;

        /// <summary>
        /// Полоса просмотра спектра
        /// </summary>
        [PgName("freq_span")]
        public decimal FreqSpan
        {
            get { return _FreqSpan; }
            set { _FreqSpan = value; OnPropertyChanged("FreqSpan"); }
        }
        private decimal _FreqSpan = 0;

        [PgName("meas_duration")]
        public double MeasDuration
        {
            get { return _MeasDuration; }
            set { _MeasDuration = value; OnPropertyChanged("MeasDuration"); }
        }
        private double _MeasDuration = 0;

        [PgName("pre_amp")]
        public int PreAmp
        {
            get { return _PreAmp; }
            set { _PreAmp = value; /*OnPropertyChanged("PreAmp");*/ }
        }
        private int _PreAmp = 0;

        [PgName("att")]
        public int ATT
        {
            get { return _ATT; }
            set { _ATT = value; /*OnPropertyChanged("ATT");*/ }
        }
        private int _ATT = 0;

        [PgName("ref_level")]
        public double RefLevel
        {
            get { return _RefLevel; }
            set { _RefLevel = value; /*OnPropertyChanged("RefLevel");*/ }
        }
        private double _RefLevel = 0;

        /// <summary>
        /// Последние координаты измерения
        /// </summary>
        [PgName("last_meas_latitude")]
        public double LastMeasLatitude
        {
            get { return _LastMeasLatitude; }
            set { _LastMeasLatitude = value; OnPropertyChanged("LastMeasLatitude"); }
        }
        private double _LastMeasLatitude = 0;

        /// <summary>
        /// Последние координаты измерения
        /// </summary>
        [PgName("last_meas_longitude")]
        public double LastMeasLongitude
        {
            get { return _LastMeasLongitude; }
            set { _LastMeasLongitude = value; OnPropertyChanged("LastMeasLongitude"); }
        }
        private double _LastMeasLongitude = 0;

        /// <summary>
        /// Высота последнего измерения
        /// </summary>
        [PgName("last_meas_altitude")]
        public double LastMeasAltitude
        {
            get { return _LastMeasAltitude; }
            set { _LastMeasAltitude = value; OnPropertyChanged("LastMeasAltitude"); }
        }
        private double _LastMeasAltitude = 0;

        [PgName("meas_start")]
        public DateTime MeasStart
        {
            get { return _MeasStart; }
            set { _MeasStart = value; OnPropertyChanged("MeasStart"); }
        }
        private DateTime _MeasStart = DateTime.MinValue;

        [PgName("meas_stop")]
        public DateTime MeasStop
        {
            get { return _MeasStop; }
            set { _MeasStop = value; OnPropertyChanged("MeasStop"); }
        }
        private DateTime _MeasStop = DateTime.MinValue;

        /// <summary>
        /// Количевство учтенных трейсов
        /// </summary>
        [PgName("trace_count")]
        public int TraceCount
        {
            get { return _TraceCount; }
            set { _TraceCount = value; OnPropertyChanged("TraceCount"); }
        }
        private int _TraceCount = 0;

        [PgName("trace_points")]
        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; OnPropertyChanged("TracePoints"); }
        }
        private int _TracePoints = 0;

        [PgName("trace")]
        public tracepoint[] Trace
        {
            get { return _Trace; }
            set { _Trace = value; /*OnPropertyChanged("Trace");*/ }
        }
        private tracepoint[] _Trace = new tracepoint[] { };
        ///// <summary>
        ///// измерение полосы сигнала
        ///// </summary>
        //[PgName("bw_data")]
        //public bandwidth_data BWData
        //{
        //    get { return _BWData; }
        //    set { _BWData = value; /*OnPropertyChanged("BWData");*/ }
        //}
        //private bandwidth_data _BWData = new bandwidth_data() { };

        ///// <summary>
        ///// ChannelPower
        ///// </summary>
        //[PgName("cp_data")]
        //public channelpower_data[] CPData
        //{
        //    get { return _CPData; }
        //    set { _CPData = value; /*OnPropertyChanged("CPData");*/ }
        //}
        //private channelpower_data[] _CPData = new channelpower_data[] { };
    }

    public class bandwidth_data : PropertyChangedBase
    {
        /// <summary>
        /// полоса сигнала с которой начинает измерение NdB
        /// </summary>
        [PgName("bw_meas_min")]
        public decimal BWMeasMin
        {
            get { return _BWMeasMin; }
            set { _BWMeasMin = value; /*OnPropertyChanged("BWMeasMin");*/ }
        }
        private decimal _BWMeasMin = 0;

        /// <summary>
        /// полоса сигнала с которой заканчивается измерение NdB
        /// </summary>
        [PgName("bw_meas_max")]
        public decimal BWMeasMax
        {
            get { return _BWMeasMax; }
            set { _BWMeasMax = value; /*OnPropertyChanged("BWMeasMax");*/ }
        }
        private decimal _BWMeasMax = 0;

        /// <summary>
        /// полоса сигнала с которой ищется максимум сигнала
        /// </summary>
        [PgName("bw_mar_peak")]
        public decimal BWMarPeak
        {
            get { return _BWMarPeak; }
            set { _BWMarPeak = value; /*OnPropertyChanged("BWMarPeak");*/ }
        }
        private decimal _BWMarPeak = 0;

        [PgName("bw_limit")]
        public decimal BWLimit
        {
            get { return _BWLimit; }
            set { _BWLimit = value; BWIdentificationComparison(); BWMeasuredComparison(); /*OnPropertyChanged("BWLimit");*/ }
        }
        private decimal _BWLimit = -1;

        /// <summary>
        /// измеренная полоса сигнала
        /// </summary>
        [PgName("bw_measured")]
        public decimal BWMeasured
        {
            get { return _BWMeasured; }
            set { _BWMeasured = value; BWMeasuredComparison(); OnPropertyChanged("BWMeasured"); }
        }
        private decimal _BWMeasured = -1;
        /// <summary>
        /// результат сравнения полосы сигнала из идентификации и из разрешения
        /// \t 0 = не сравнивалось 1 = не совпадает 2 = совпадает
        /// </summary>
        public decimal BWMeasuredComparisonResult
        {
            get { return _BWMeasuredComparisonResult; }
            set { _BWMeasuredComparisonResult = value; OnPropertyChanged("BWMeasuredComparisonResult"); }
        }
        private decimal _BWMeasuredComparisonResult = -1;

        private void BWMeasuredComparison()
        {
            if (BWLimit != -1 && BWMeasured != -1)
            {
                if (BWMeasured <= BWLimit) BWMeasuredComparisonResult = 2;
                if (BWMeasured > BWLimit) BWMeasuredComparisonResult = 1;
            }
            else BWMeasuredComparisonResult = 0;
        }
        /// <summary>
        /// полоса сигнала из идентификации (с эфира)
        /// </summary>
        [PgName("bw_identification")]
        public decimal BWIdentification
        {
            get { return _BWIdentification; }
            set { _BWIdentification = value; BWIdentificationComparison(); OnPropertyChanged("BWIdentification"); }
        }
        private decimal _BWIdentification = -1;

        /// <summary>
        /// результат сравнения полосы сигнала из идентификации(с эфира) и из разрешения
        /// \t 0 = не сравнивалось 1 = не совпадает (всеплохо) 2 = совпадает (все хорошо)
        /// </summary>
        public decimal BWIdentificationComparisonResult
        {
            get { return _BWIdentificationComparisonResult; }
            set { _BWIdentificationComparisonResult = value; OnPropertyChanged("BWIdentificationComparisonResult"); }
        }
        private decimal _BWIdentificationComparisonResult = -1;

        private void BWIdentificationComparison()
        {
            if (BWLimit != -1 && BWIdentification != -1)
            {
                if (BWIdentification <= BWLimit) BWIdentificationComparisonResult = 2;
                if (BWIdentification > BWLimit) BWIdentificationComparisonResult = 1;
            }
            else BWIdentificationComparisonResult = 0;
        }

        [PgName("ndb_level")]
        public double NdBLevel
        {
            get { return _NdBLevel; }
            set { _NdBLevel = value; /*OnPropertyChanged("NdBLevel");*/ }
        }
        private double _NdBLevel = 0;

        /// <summary>
        /// NdBResult[0] = M1Index, NdBResult[1] = T1Index, NdBResult[2] = T2Index
        /// </summary>
        [PgName("ndb_result")]
        public int[] NdBResult
        {
            get { return _NdBResult; }
            set { _NdBResult = value; /*OnPropertyChanged("NdBResult");*/ }
        }
        private int[] _NdBResult = new int[] { 0, 0, 0 };

        [PgName("obw_percent")]
        public decimal OBWPercent
        {
            get { return _OBWPercent; }
            set { _OBWPercent = value; /*OnPropertyChanged("OBWPercent");*/ }
        }
        private decimal _OBWPercent = 0;

        /// <summary>
        /// OBWResult[0] = M1Index, OBWResult[1] = T1Index, OBWResult[2] = T2Index
        /// </summary>
        [PgName("obw_result")]
        public int[] OBWResult
        {
            get { return _OBWResult; }
            set { _OBWResult = value; /*OnPropertyChanged("OBWResult");*/ }
        }
        private int[] _OBWResult = new int[] { 0, 0, 0 };        
    }

    public class channelpower_data : PropertyChangedBase
    {
        [PgName("freq_centr")]
        public decimal FreqCentr
        {
            get { return _FreqCentr; }
            set { _FreqCentr = value; /*OnPropertyChanged("FreqCentr");*/ }
        }
        private decimal _FreqCentr = 0;

        /// <summary>
        /// Полоса измерения уровня в канале, относительно центральной частоты
        /// </summary>
        [PgName("channel_power_bw")]
        public decimal ChannelPowerBW
        {
            get { return _ChannelPowerBW; }
            set { _ChannelPowerBW = value; /*OnPropertyChanged("ChannelPowerBW");*/ }
        }
        private decimal _ChannelPowerBW = -1;

        /// <summary>
        /// если не измерялось то равно -10000
        /// </summary>
        [PgName("channel_power_result")]
        public double ChannelPowerResult
        {
            get { return _ChannelPowerResult; }
            set { _ChannelPowerResult = value; /*OnPropertyChanged("ChannelPowerResult");*/ }
        }
        private double _ChannelPowerResult = -10000;
    }

    public class ParamWithUI : PropertyChangedBase
    {
        public string UI { get; set; }
        public string UIShort { get; set; }
        public string Parameter { get; set; }
    }

    public partial class GSMBandMeas : PropertyChangedBase
    {
        public bool select
        {
            get { return _select; }
            set { _select = value; OnPropertyChanged("select"); }
        }
        private bool _select = false;
        public int id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("id"); }
        }
        private int _id = 0;
        public int Count
        {
            get { return _Count; }
            set { _Count = value; OnPropertyChanged("Count"); }
        }
        private int _Count = 0;
        public int CountAll
        {
            get { return _CountAll; }
            set { _CountAll = value; OnPropertyChanged("CountAll"); }
        }
        private int _CountAll = 0;
        public decimal Start
        {
            get { return _Start; }
            set { _Start = value; OnPropertyChanged("Start"); }
        }
        private decimal _Start = 0;
        public decimal Stop
        {
            get { return _Stop; }
            set { _Stop = value; OnPropertyChanged("Stop"); }
        }
        private decimal _Stop = 0;
        public decimal Step
        {
            get { return _Step; }
            set { _Step = value; OnPropertyChanged("Step"); }
        }
        private decimal _Step = 0;
        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; OnPropertyChanged("TracePoints"); }
        }
        private int _TracePoints = 0;
        public tracepoint[] Trace
        {
            get { return _Trace; }
            set { _Trace = value; OnPropertyChanged("Trace"); }
        }
        private tracepoint[] _Trace = new tracepoint[] { };
        public TimeSpan Time
        {
            get { return _Time; }
            set { _Time = value; OnPropertyChanged("Time"); }
        }
        private TimeSpan _Time = TimeSpan.MinValue;

        public DateTime MeasTime
        {
            get { return _MeasTime; }
            set { _MeasTime = value; OnPropertyChanged("MeasTime"); }
        }
        private DateTime _MeasTime = DateTime.MinValue;

        public double latitude
        {
            get { return _latitude; }
            set { _latitude = value; OnPropertyChanged("latitude"); }
        }
        private double _latitude = 0;

        public double longitude
        {
            get { return _longitude; }
            set { _longitude = value; OnPropertyChanged("longitude"); }
        }
        private double _longitude = 0;

        public double altitude
        {
            get { return _altitude; }
            set { _altitude = value; OnPropertyChanged("altitude"); }
        }
        private double _altitude = 0;

        public bool saved
        {
            get { return _saved; }
            set { _saved = value; OnPropertyChanged("saved"); }
        }
        private bool _saved = true;

        public DB.localatdi_meas_device device_ident
        {
            get { return _device_ident; }
            set { _device_ident = value; OnPropertyChanged("device_ident"); }
        }
        private DB.localatdi_meas_device _device_ident = new DB.localatdi_meas_device() { };

        public DB.localatdi_meas_device device_meas
        {
            get { return _device_meas; }
            set { _device_meas = value; OnPropertyChanged("device_meas"); }
        }
        private DB.localatdi_meas_device _device_meas = new DB.localatdi_meas_device() { };
    }
}
