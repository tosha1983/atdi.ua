using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using System.Windows.Forms;
using Atdi.Platform.Logging;
using Atdi.Icsm.Plugins.Core;
using VM = Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc
{
    public class PluginMenuCommands
    {
	    private readonly ViewStarter _viewStarter;
	    private readonly ILogger _logger;

	    public PluginMenuCommands(ViewStarter viewStarter, ILogger logger)
	    {
		    _viewStarter = viewStarter;
		    _logger = logger;
	    }

		public void OnAboutCommand()
        {
            var text = new StringBuilder();
            text.AppendLine("Plugin: " + PluginMetadata.Title);
            text.AppendLine("Ident: " + PluginMetadata.Ident);
            text.AppendLine("Schema Version: " + PluginMetadata.SchemaVersion.ToString(System.Globalization.CultureInfo.InvariantCulture));
            text.AppendLine("Assembly: " + typeof(PluginMenuCommands).Assembly.FullName);

            MessageBox.Show(text.ToString(), @"About Plugin", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void OnRunCalibrationStationsMeasurementsCommand()
        {
	        try
	        {
		        _viewStarter.Start<VM.StationCalibrationManager.View>(isModal: true);
	        }
	        catch (Exception e)
	        {
		        _logger.Exception((EventContext)"PluginMenuCommands", (EventCategory)"OnRunCalibrationStationsMeasurementsCommand", e);
		        MessageBox.Show(e.ToString());
	        }
        }
	}
}
