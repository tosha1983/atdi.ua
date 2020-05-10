using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using Atdi.Icsm.Plugins.SdrnCalcServerClient;


namespace XICSM.SdrnCalcServerClient
{
    /// <summary>
    /// The class of the Control Client Plugin
    /// </summary>
    public class Plugin : IPlugin
    {
        public double SchemaVersion => PluginMetadata.SchemaVersion;

        public string Description => PluginMetadata.Title;

        public string Ident => PluginMetadata.Ident;

        public void GetMainMenu(IMMainMenu mainMenu)
        {
            mainMenu.SetLocation();
            mainMenu.InsertItem(PluginMetadata.Menu.MainTool, PluginMetadata.Menu.Tools.RunProjectManagerCommand, PluginCommands.OnRunProjectManagerCommand);
            mainMenu.InsertItem(PluginMetadata.Menu.MainTool, PluginMetadata.Menu.Tools.About, PluginCommands.OnAboutCommand);
        }

        public bool OtherMessage(string message, object inParam, ref object outParam)
        {
            return false;
        }
        public void RegisterBoard(IMBoard b)
        {
            //b.RegisterQueryMenuBuilder(MD.Tours.TableName, ToursContextMenuBuilder.Build);
            //b.RegisterQueryMenuBuilder(MD.Allotments.TableName, AllotmentsContextMenuBuilder.Build);
            //b.RegisterQueryMenuBuilder(MD.Inspection.TableName, InspectionsContextMenuBuilder.Build);
            //b.RegisterQueryMenuBuilder(MD.MobStations.TableName, OtherTerrestrialStationsContextMenuBuilder.Build);
            //b.RegisterQueryMenuBuilder(MD.MobStations2.TableName, YetOtherTerrestrialStationsContextMenuBuilder.Build);
        }

        public void RegisterSchema(IMSchema s)
        {

        }
        public bool UpgradeDatabase(IMSchema s, double dbCurVersion)
        {
            if (dbCurVersion < this.SchemaVersion)
            {
                s.SetDatabaseVersion(this.SchemaVersion);
            }
            return true;
        }
    }
}
