using System;
using System.Collections.Generic;
using System.Text;

namespace Atdi.AppUnits.Sdrn.DeviceServer.GPS
{
    // Code by Aleksandr Dikarev, dikarev-aleksandr@yandex.ru

    [Serializable]
    public class SettingsContainer
    {
        #region Properties

        public SerialPortSettings PortSettings;

        #endregion

        #region Constructor

        public SettingsContainer()
        {
            SetDefaults();
        }

        #endregion

        #region Methods

        public void SetDefaults()
        {
            PortSettings = new SerialPortSettings("COM1", BaudRate.baudRate9600, System.IO.Ports.Parity.None, DataBits.dataBits8, System.IO.Ports.StopBits.One, System.IO.Ports.Handshake.None);
        }

        #endregion
    }
}
