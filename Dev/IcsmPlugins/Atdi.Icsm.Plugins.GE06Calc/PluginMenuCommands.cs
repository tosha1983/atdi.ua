using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using System.Windows.Forms;
using VM = Atdi.Icsm.Plugins.GE06Calc.ViewModels;
using Atdi.Platform.Logging;
using Atdi.Icsm.Plugins.Core;
using MD = Atdi.Icsm.Plugins.GE06Calc.Metadata;

namespace Atdi.Icsm.Plugins.GE06Calc
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

        public void OnRunGE06SettingsCommand()
        {
            try
            {
                _viewStarter.Start<VM.GE06Settings.View>(isModal: true);
            }
            catch (Exception e)
            {
                _logger.Exception((EventContext)"PluginMenuCommands", (EventCategory)"OnRunGE06SettingsCommand", e);
                _viewStarter.ShowException(PluginMetadata.Title, e);
            }
        }
        public List<IMQueryMenuNode> BuildStartGE06Menu_FMTV_ASSIGN(string tableName, int nbRecMin)
        {
            var nodes = new List<IMQueryMenuNode>();

            if (MD.FMTV_Assign.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && nbRecMin > 0)
            {
                nodes.AddContextMenuToolForEachRecords(PluginMetadata.ContextMenu.FMTV_Assign.StartGE06Task, OnStartGE06Task_FMTV_ASSIGNCommand);
            }

            return nodes;
        }
        public List<IMQueryMenuNode> BuildStartGE06Menu_ge06_allot_terra(string tableName, int nbRecMin)
        {
            var nodes = new List<IMQueryMenuNode>();

            if (MD.GE06_allot_terra.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && nbRecMin > 0)
            {
                nodes.AddContextMenuToolForEachRecords(PluginMetadata.ContextMenu.GE06_allot_terra.StartGE06Task, OnStartGE06Task_ge06_allot_terraCommand);
            }

            return nodes;
        }
        public List<IMQueryMenuNode> BuildStartGE06Menu_fmtv_terra(string tableName, int nbRecMin)
        {
            var nodes = new List<IMQueryMenuNode>();

            if (MD.FMTV_terra.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) && nbRecMin > 0)
            {
                nodes.AddContextMenuToolForEachRecords(PluginMetadata.ContextMenu.FMTV_terra.StartGE06Task, OnStartGE06Task_fmtv_terraCommand);
            }
            return nodes;
        }
        public bool OnStartGE06Task_FMTV_ASSIGNCommand(IMQueryMenuNode.Context context)
        {
            return OnStartGE06TaskCommand(1, context.TableId, context);
        }
        public bool OnStartGE06Task_ge06_allot_terraCommand(IMQueryMenuNode.Context context)
        {
            return OnStartGE06TaskCommand(2, context.TableId, context);
        }
        public bool OnStartGE06Task_fmtv_terraCommand(IMQueryMenuNode.Context context)
        {
            return OnStartGE06TaskCommand(3, context.TableId, context);
        }
        public bool OnStartGE06TaskCommand(byte startType, int Id, IMQueryMenuNode.Context context)
        {
            try
            {
                _viewStarter.Start<VM.GE06Task.View>(isModal: true, c => c.Context = context);
                return true;
            }
            catch (Exception e)
            {
                _logger.Exception((EventContext)"PluginMenuCommands", (EventCategory)"OnRunGE06SettingsCommand", e);
                _viewStarter.ShowException(PluginMetadata.Title, e);
                return false;
            }
        }
    }
}
