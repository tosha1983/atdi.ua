using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSReceiver
{
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
            // Blocks until a message returns on this socket from a remote host.
            Byte[] receiveBytes = m_Client.Receive(ref RemoteIpEndPoint);
            returnData = System.Text.ASCIIEncoding.ASCII.GetString(receiveBytes);
            return returnData;
        }
        public Byte[] ByteRead()
        {
            CheckOpen();
            Byte[] returnData = new Byte[] { };
            // Blocks until a message returns on this socket from a remote host.                
            returnData = m_Client.Receive(ref RemoteIpEndPoint);
            return returnData;
        }
        public double[] ByteRead1()
        {
            CheckOpen();
            Byte[] returnData = new Byte[] { };
            double[] temp = new double[] { };
            // Blocks until a message returns on this socket from a remote host.
            returnData = m_Client.Receive(ref RemoteIpEndPoint);
            double[] t = new double[((returnData.Length - 30) / 2)];

            for (int i = 0; i < (t.Length - 30) / 2; i++)
            {
                t[i] = ((double)BitConverter.ToInt16(returnData, i * 2 + 29)) / 10;
            }
            temp = t;
            return temp;
        }

        public void Open(string hostname, int port)
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
        public void Close()
        {
            if (!m_IsOpen)
                return;
            m_Client.Close();
            m_IsOpen = false;
            if (Closed != null)
                Closed();
        }
        public void Dispose()
        {
            Close();
        }
    }
}
