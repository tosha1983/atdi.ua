using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atdi.Icsm.Plugins.Core;
using ICSM;
using Atdi.Icsm.Plugins.GE06Calc;
using Atdi.Platform;
using Atdi.Platform.AppServer;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;

namespace XICSM.GE06Calc
{
    /// <summary>
    /// The class of the Control Client Plugin
    /// </summary>
    public class Plugin : PluginBase<PluginMenuCommands>
    {
        public Plugin()
            : base(PluginMetadata.Ident, PluginMetadata.SchemaVersion, PluginMetadata.Title)
        {
        }

        protected override void ConnectToMenu(IMMainMenu mainMenu)
        {
            mainMenu.InsertItem(PluginMetadata.Menu.MainTool, PluginMetadata.Menu.Tools.RunProjectManagerCommand, _menuCommands.OnRunGE06SettingsCommand);
            mainMenu.InsertItem(PluginMetadata.Menu.MainTool, PluginMetadata.Menu.Tools.About, _menuCommands.OnAboutCommand);
        }
    }
}
