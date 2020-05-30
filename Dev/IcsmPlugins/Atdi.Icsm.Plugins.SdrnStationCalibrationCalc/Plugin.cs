using Atdi.Icsm.Plugins.Core;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc;
using ICSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XICSM.SdrnStationCalibrationCalc
{
	public class Plugin : PluginBase<PluginMenuCommands>
	{
		public Plugin() 
			: base(PluginMetadata.Ident, PluginMetadata.SchemaVersion, PluginMetadata.Title)
		{
		}

		protected override void ConnectToMenu(IMMainMenu mainMenu)
		{
			mainMenu.InsertItem(PluginMetadata.Menu.MainTool, PluginMetadata.Menu.Tools.About, _menuCommands.OnAboutCommand);
			mainMenu.InsertItem(PluginMetadata.Menu.MainTool, "Calibration of stations according measurements", _menuCommands.OnRunCalibrationStationsMeasurementsCommand);
            mainMenu.InsertItem(PluginMetadata.Menu.MainTool, "Results calibration of stations measurements", _menuCommands.OnRunCalibrationStationsResultCommand);
        }
	}
}
