using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Atdi.Common;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.RSReceiver
{
    class CommandConnection : IDisposable
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
        public CommandConnection() { }
        public CommandConnection(bool open) : this("localhost", 5025, true) { }
        public CommandConnection(string host, int port, bool open)
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
        public string Query(string str)
        {
            string t = "";
            long WaitReadfrom = WinAPITime.GetTimeStamp();// DateTime.Now.Ticks;
            WriteLine(str);
            while (!m_Stream.DataAvailable && WinAPITime.GetTimeStamp() - WaitReadfrom < 20000000)
            {
                System.Threading.Thread.Sleep(5);
            }
            t = Read();
            return t;
        }
        public void Write(string str)
        {
            CheckOpen();
            byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
            m_Stream.Write(bytes, 0, bytes.Length);
            m_Stream.Flush();
        }
        public void WriteLine(string str)
        {
            CheckOpen();
            byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
            m_Stream.Write(bytes, 0, bytes.Length);
            WriteTerminator();
            m_Stream.Flush();
        }
        void WriteTerminator()
        {
            byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes("\n");
            m_Stream.Write(bytes, 0, bytes.Length);
            m_Stream.Flush();
        }
        public string Read()
        {
            CheckOpen();
            return System.Text.ASCIIEncoding.ASCII.GetString(ReadBytes());
        }
        public byte[] ReadBytes()
        {
            var bytes = new List<byte>();
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
        public void Close()
        {
            if (!m_IsOpen)
                return;
            m_Stream.Close();
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
