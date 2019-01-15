using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace GNSSView

    // Code by Aleksandr Dikarev, dikarev-aleksandr@yandex.ru
{
    public sealed class NMEASerialPort : NMEAPort
    {
        #region Properties

        int writeLock;
        int readLock;
        SerialPort serialPort;

        public SerialPortSettings PortSettings { get; private set; }

        #region NMEAPort

        public override bool IsOpen
        {
            get { return serialPort.IsOpen; }
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
                PortSettings.PortStopBits);

            serialPort.Handshake = portSettings.PortHandshake;
            serialPort.Encoding = Encoding.ASCII;

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
