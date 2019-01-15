using System;
using System.Text;
using NMEA;

namespace GNSSView
{
    // Code by Aleksandr Dikarev, dikarev-aleksandr@yandex.ru

    #region Custom EventArgs

    public class NewNMEAMessageEventArgs : EventArgs
    {
        #region Properties

        public string Message { get; private set; }

        #endregion

        #region Constructor

        public NewNMEAMessageEventArgs(string message)
        {
            Message = message;
        }

        #endregion
    }

    #endregion

    public abstract class NMEAPort
    {
        #region Properties

        public abstract bool IsOpen { get; }

        StringBuilder buffer;

        #endregion

        #region Methods

        #region Abstract

        public abstract void SendData(string message);
        public abstract void Open();
        public abstract void Close();

        #endregion

        #region Protected

        protected void OnIncomingData(string data)
        {
            buffer.Append(data);
            var temp = buffer.ToString();

            int lIndex = temp.LastIndexOf(NMEAParser.SentenceEndDelimiter);
            if (lIndex >= 0)
            {
                buffer = buffer.Remove(0, lIndex + 2);
                if (lIndex + 2 < temp.Length)
                    temp = temp.Remove(lIndex + 2);

                temp = temp.Trim(new char[] { '\0' });

                var lines = temp.Split(NMEAParser.SentenceEndDelimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (NewNMEAMessage != null)
                {
                    for (int i = 0; i < lines.Length; i++)
                        NewNMEAMessage(this, new NewNMEAMessageEventArgs(string.Format("{0}{1}", lines[i], NMEAParser.SentenceEndDelimiter)));
                }
            }

            if (buffer.Length >= ushort.MaxValue)
                buffer.Remove(0, short.MaxValue);
        }

        protected void OnConnectionOpening()
        {
            buffer = new StringBuilder();
        }

        protected void OnConnectionClosing()
        {
            buffer = new StringBuilder();
        }

        #endregion        

        #endregion

        #region Events

        public EventHandler<NewNMEAMessageEventArgs> NewNMEAMessage;

        #endregion
    }
}
