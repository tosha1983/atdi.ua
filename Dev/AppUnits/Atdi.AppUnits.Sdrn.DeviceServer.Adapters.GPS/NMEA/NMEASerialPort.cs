using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.GPS

// Code by Aleksandr Dikarev, dikarev-aleksandr@yandex.ru
{
    public sealed class NMEASerialPort : NMEAPort
    {
        #region Properties

        int writeLock;
        int readLock;
        SerialPort serialPort;

        static bool isRunning = false;

        List<long> OffsetToAvg = new List<long>();
        bool _CDHolding= false;
        long CorrectTime { get; set; }
        public long OffsetToAvged { get; set; }
        

        public SerialPortSettings PortSettings { get; private set; }

        #region NMEAPort

        public override bool IsOpen
        {
            get { return serialPort.IsOpen; }
        }

        public void SetCorrectionTime(long correctTime)
        {
            CorrectTime = correctTime;
            OffsetToAvged = CorrectTime;
            if (isRunning == false)
            {
                StartPPSMonitor();
            }
        }

        #endregion

        #endregion

        #region Constructor

        public NMEASerialPort(SerialPortSettings portSettings)
            : base()
        {
            #region serialPort initialization

            PortSettings = portSettings;
            serialPort = new SerialPort(
                PortSettings.PortName,
                (int)PortSettings.PortBaudRate,
                PortSettings.PortParity,
                (int)PortSettings.PortDataBits,
                PortSettings.PortStopBits)
            {
                Handshake = portSettings.PortHandshake,
                Encoding = Encoding.ASCII
            };

            serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(serialPort_ErrorReceived);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            #endregion
        }
      
        #endregion

        #region Methods

        #region NMEAPort

        public override void Open()
        {
            while (Interlocked.CompareExchange(ref writeLock, 1, 0) != 0)
                Thread.SpinWait(1);

            while (Interlocked.CompareExchange(ref readLock, 1, 0) != 0)
                Thread.SpinWait(1);
                
            try
            {
                OnConnectionOpening();
                serialPort.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Interlocked.Decrement(ref readLock);
                Interlocked.Decrement(ref writeLock);
            }
        }

        public override void Close()
        {
            while (Interlocked.CompareExchange(ref writeLock, 1, 0) != 0)
                Thread.SpinWait(1);

            while (Interlocked.CompareExchange(ref readLock, 1, 0) != 0)
                Thread.SpinWait(1);

            try
            {
                OnConnectionClosing();
                serialPort.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Interlocked.Decrement(ref readLock);
                Interlocked.Decrement(ref writeLock);
            }
        }

        public override void SendData(string message)
        {
            while (Interlocked.CompareExchange(ref writeLock, 1, 0) != 0)
                Thread.SpinWait(1);

            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                serialPort.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Interlocked.Decrement(ref writeLock);
            }
        }

        #endregion

        #endregion

        #region Handlers

        #region serialPort

        private void StartPPSMonitor()
        {
            var PPSThread = new Thread(GetPPSData)
            {
                Name = "PPSThread",
                IsBackground = true
            };
            PPSThread.Start();
            isRunning = true;
        }

        void GetPPSData()
        {
            long PPSStep = 10000000;
            bool b;
            long offtime = 0;
            while (serialPort.IsOpen)
            {
                b = serialPort.CDHolding;
                if (b != _CDHolding)
                {
                    if (b)
                    {
                        offtime = (int)CorrectTime + PPSStep;
                        if ((int)CorrectTime != 0)
                        {
                            if (OffsetToAvg.Count > 599)
                            { OffsetToAvg.RemoveAt(0); }
                            OffsetToAvg.Add(offtime);

                            OffsetToAvged = OffsetToAvg[0];

                            for (int i = 1; i < OffsetToAvg.Count; i++)
                            {
                                OffsetToAvged += OffsetToAvg[i];
                            }

                            OffsetToAvged = OffsetToAvged / OffsetToAvg.Count;
                            CorrectTime = 0;
                        }
                        Thread.Sleep(900);
                    }
                    _CDHolding = b;
                }
            }
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (Interlocked.CompareExchange(ref readLock, 1, 0) != 0)
                Thread.SpinWait(1);
            
            int bytesToRead = serialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            serialPort.Read(buffer, 0, bytesToRead);
            
            Interlocked.Decrement(ref readLock);

            OnIncomingData(Encoding.ASCII.GetString(buffer));
        }

        private void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            if (PortError != null)
                PortError(this, e);
        }

        #endregion

        #endregion

        #region Events

        public EventHandler<SerialErrorReceivedEventArgs> PortError;

        #endregion
    }
}
