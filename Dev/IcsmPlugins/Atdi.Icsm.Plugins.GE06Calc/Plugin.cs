using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atdi.Icsm.Plugins.Core;
using ICSM;
using Atdi.Icsm.Plugins.GE06Calc;
using MD = Atdi.Icsm.Plugins.GE06Calc.Metadata;
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
            mainMenu.InsertItem(PluginMetadata.Menu.MainTool, PluginMetadata.Menu.Tools.RunSettingsCommand, _menuCommands.OnRunGE06SettingsCommand);
            mainMenu.InsertItem(PluginMetadata.Menu.MainTool, PluginMetadata.Menu.Tools.About, _menuCommands.OnAboutCommand);
        }
        protected override void AddBoard(IMBoard b)
        {
            b.RegisterQueryMenuBuilder(MD.FMTV_Assign.TableName, _menuCommands.BuildStartGE06Menu_FMTV_ASSIGN);
            b.RegisterQueryMenuBuilder(MD.GE06_allot_terra.TableName, _menuCommands.BuildStartGE06Menu_ge06_allot_terra);
            b.RegisterQueryMenuBuilder(MD.FMTV_terra.TableName, _menuCommands.BuildStartGE06Menu_fmtv_terra);
        }
    }
}
