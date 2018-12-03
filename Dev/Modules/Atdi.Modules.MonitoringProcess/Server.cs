using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Xml.Serialization;
using BB60C_;
using System.Windows.Forms;

namespace ServerTCP
{
    public class StatusChangedEventArgs : EventArgs
    {
        private string EventMsg;

        public string EventMessage
        {
            get
            {
                return EventMsg;
            }
            set
            {
                EventMsg = value;
            }
        }

        public StatusChangedEventArgs(string strEventMsg)
        {
            EventMsg = strEventMsg;
        }
    }

    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);

    public class Server
    {
        public static Hashtable htUsers = new Hashtable(300); // 30 users at one time limit
        public static Hashtable htConnections = new Hashtable(300); // 30 users at one time limit
        private IPAddress ipAddress;
        private TcpClient tcpClient;
        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;
        private AsyncCallback callbackRead;
        public delegate void DataReceivedDelegate(byte[] data, int len);
        public event DataReceivedDelegate OnDataReceived;
        private NetworkStream swReader;
        public bool isAbort = false;

        byte[] _readBuffer;
        String data = "";

        public Server(IPAddress address)
        {
            ipAddress = address;
            callbackRead = new AsyncCallback(this.EndReadData);
        }

        private Thread thrListener;
        private TcpListener tlsClient;
        bool ServRunning = false;


        public static void AddUser(TcpClient tcpUser, string strUsername)
        {
            Server.htUsers.Add(strUsername, tcpUser);
            Server.htConnections.Add(tcpUser, strUsername);
            //SendAdminMessage(htConnections[tcpUser] + " has joined us");
        }

        public static void RemoveUser(TcpClient tcpUser)
        {
            if (htConnections[tcpUser] != null)
            {
                //SendAdminMessage(htConnections[tcpUser] + " has left us");
                Server.htUsers.Remove(Server.htConnections[tcpUser]);
                Server.htConnections.Remove(tcpUser);
            }
        }

        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                statusHandler(null, e);
            }
        }

        public static void SendAdminMessage(string Message)
        {
            StreamWriter swSenderSender;
            //e = new StatusChangedEventArgs(Message);
            //OnStatusChanged(e);

            TcpClient[] tcpClients = new TcpClient[Server.htUsers.Count];
            Server.htUsers.Values.CopyTo(tcpClients, 0);
            for (int i = 0; i < tcpClients.Length; i++)
            {
                try
                {
                    if (Message.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());
                    swSenderSender.Write(Message);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }




        void EndReadData(IAsyncResult iar)
        {
            try
            {
                swReader = (NetworkStream)iar.AsyncState;
                int bytesRead;
                bytesRead = swReader.EndRead(iar);
                data = String.Concat(data, Encoding.UTF8.GetString(_readBuffer, 0, bytesRead));

                if (swReader.DataAvailable)
                    swReader.BeginRead(_readBuffer, 0, _readBuffer.Length, new AsyncCallback(EndReadData), swReader);
                isAbort = true;
            }
            catch (Exception)
            { isAbort = true; }

        }


        public bool isAbt()
        {

            bool isAbrt = false;
            /*
            try
            {
                string AllText = "";
                swReader = tcpClient.GetStream();
                if (swReader.CanRead)
                {
                    _readBuffer = new byte[1024];
                    swReader.BeginRead(_readBuffer, 0, _readBuffer.Length,
                                                                 new AsyncCallback(EndReadData),
                                                                 swReader);
                    isAbrt = isAbort;
                    if (!tcpClient.Connected) isAbrt = true;

                    List<byte> r = new List<byte>();
                    byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                    swReader.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);
                    bool isAllNull = true;
                    foreach (byte b in bytes)
                    {
                        if (b != 0)
                            isAllNull = false;
                    }

                    if (isAllNull==false)
                        AllText = Encoding.UTF8.GetString(bytes);

                }

                if (!isAbrt)
                {
                    AllText = AllText.Replace("\0", "");
                    if (!string.IsNullOrEmpty(AllText))
                    {

                        AllText = AllText.Replace("</SDR_BB60C_test>", "");
                        int index = AllText.LastIndexOf("<F_semples />") + 13;
                        string tmp = AllText.Substring(0, index);
                        AllText = tmp;
                        AllText += "\r\n" + "</SDR_BB60C_test>";
                        String tempPath_answer = System.IO.Path.GetTempPath();
                        string xml_file = string.Format(tempPath_answer + "\\{0}", Guid.NewGuid().ToString()) + ".xml";
                        File.WriteAllText(xml_file, AllText, Encoding.UTF8);



                        XmlSerializer ser = new XmlSerializer(typeof(SDR_BB60C_test));
                        TextReader reader = new System.IO.StreamReader(xml_file, false);
                        object vb = ser.Deserialize(reader);
                        SDR_BB60C_test test_une_sw = (SDR_BB60C_test)vb;
                        reader.Close();
                        isAbort = test_une_sw.isAbort;
                    }
                }
            }
            catch (Exception)
            {
                isAbort = true;
            }
            */
            return isAbort;
        }


        public static void SendMessage(string From, string Message)
        {
            StreamWriter swSenderSender;
            e = new StatusChangedEventArgs(Message);
            OnStatusChanged(e);

            TcpClient[] tcpClients = new TcpClient[Server.htUsers.Count];
            Server.htUsers.Values.CopyTo(tcpClients, 0);
            for (int i = 0; i < tcpClients.Length; i++)
            {
                try
                {
                    if (Message.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());
                    swSenderSender.Write(Message);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }

        public void StartListening()
        {

            IPAddress ipaLocal = ipAddress;
            tlsClient = new TcpListener(1986);
            tlsClient.Start();
            ServRunning = true;
            thrListener = new Thread(KeepListening);
            thrListener.Start();

        }

        public void Close()
        {
            thrListener.Abort();
            tlsClient.Stop();
        }
        private void KeepListening()
        {
            while (ServRunning == true)
            {
                tcpClient = tlsClient.AcceptTcpClient();
                Connection newConnection = new Connection(tcpClient);
            }
        }
    }

    class Connection
    {
        TcpClient tcpClient;
        private Thread thrSender;
        private StreamReader srReceiver;
        private StreamWriter swSender;
        private string currUser;
        private string strResponse;

        public Connection(TcpClient tcpCon)
        {
            tcpClient = tcpCon;
            thrSender = new Thread(AcceptClient);
            thrSender.Start();
        }

        private void CloseConnection()
        {
            tcpClient.Close();
            srReceiver.Close();
            swSender.Close();
        }

        private void AcceptClient()
        {
            srReceiver = new System.IO.StreamReader(tcpClient.GetStream());
            swSender = new System.IO.StreamWriter(tcpClient.GetStream());
            currUser = srReceiver.ReadLine();

            if (currUser != "")
            {
                if (Server.htUsers.Contains(currUser) == true)
                {
                    swSender.WriteLine("0|This username already exists.");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else if (currUser == "Administrator")
                {
                    swSender.WriteLine("0|This username is reserved.");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else
                {
                    swSender.WriteLine("1");
                    swSender.Flush();
                    Server.AddUser(tcpClient, currUser);
                }
            }
            else
            {
                CloseConnection();
                return;
            }

            try
            {
                while ((strResponse = srReceiver.ReadLine()) != "")
                {
                    if (strResponse == null)
                    {
                        Server.RemoveUser(tcpClient);
                    }
                    else
                    {
                        Server.SendMessage(currUser, strResponse);
                    }
                }
            }
            catch
            {
                Server.RemoveUser(tcpClient);
                CloseConnection();
                thrSender.Interrupt();
                return;
            }
        }
    }
}
