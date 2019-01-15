using System;
using GNSSView;


namespace GPS
{
    public  class Runner
    {
        public static SerialPortSettings portSettings;
        public static GNSSReceiverWrapper gnssWrapper;
        public void StartGPS()
        {
            
                SettingsProviderXML<SettingsContainer> settingsProvider = new SettingsProviderXML<SettingsContainer>();
                settingsProvider.isSwallowExceptions = false;
                var settingsFileName = StrUtils.GetExecutableFileNameWithNewExt(AppDomain.CurrentDomain.FriendlyName, "exe.settings");
                try
                {
                    settingsProvider.Load(StrUtils.GetExecutableFileNameWithNewExt(AppDomain.CurrentDomain.FriendlyName, "exe.settings"));
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(string.Format("Unable load settings from {0} properly due to {1}, default settings will be used instead", settingsFileName, ex.Message));
                }
                portSettings = settingsProvider.Data.PortSettings;
                gnssWrapper = new GNSSReceiverWrapper(portSettings);
                gnssWrapper.LogEvent += new EventHandler<LogEventArgs>(gnssWrapper_LogEvent);
                if (gnssWrapper.IsOpen)
                {
                    try
                    {
                        gnssWrapper.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(string.Format("Unable stop GNSSWrapper due to {0}", ex.Message), "Error");
                    }
                }
                else
                {
                    try
                    {
                        gnssWrapper.Open();
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(string.Format("Unable start GNSSWrapper due to {0}", ex.Message), "Error");
                    }
                }
            

        }


        private void gnssWrapper_LogEvent(object sender, LogEventArgs e)
        {
            ClassGetCoordinate.TryGetLocation(e.LogString);
        }


    }
}
