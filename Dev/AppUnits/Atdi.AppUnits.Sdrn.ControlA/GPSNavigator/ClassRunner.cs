using System;
using GNSSView;
using Atdi.AppUnits.Sdrn.ControlA.Bus;
using Atdi.Platform.Logging;
using Atdi.AppUnits.Sdrn.ControlA;

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
                Launcher._logger.Error(Contexts.ThisComponent, Categories.StartGPS, string.Format(Events.UnableLoadSettings.ToString(), settingsFileName, ex.Message));
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
                    Launcher._logger.Error(Contexts.ThisComponent, Categories.StartGPS, string.Format(Events.UnableStopGNSSWrapper.ToString(), ex.Message));
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
                    Launcher._logger.Error(Contexts.ThisComponent, Categories.StartGPS, string.Format(Events.UnableStartGNSSWrapper.ToString(), ex.Message));
                }
            }
        }


        private void gnssWrapper_LogEvent(object sender, LogEventArgs e)
        {
            ClassGetCoordinate.TryGetLocation(e.LogString);
        }


    }
}
