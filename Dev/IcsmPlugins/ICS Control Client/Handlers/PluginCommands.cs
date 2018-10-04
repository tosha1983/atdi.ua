using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using System.Windows.Forms;
using XICSM.ICSControlClient.Environment;
using XICSM.ICSControlClient.WcfServiceClients;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;


using MD = XICSM.ICSControlClient.Metadata;
using DM = XICSM.ICSControlClient.Models.BuildInspections;
using WCF = XICSM.ICSControlClient.WcfServiceClients;
using SDR = Atdi.AppServer.Contracts.Sdrns;
using AAC = Atdi.AppServer.Contracts;
using FM = XICSM.ICSControlClient.Forms;

namespace XICSM.ICSControlClient
{
    /// <summary>
    /// The commands of the main menu 
    /// </summary>
    public class PluginCommands
    {
        public static void OnAboutCommand()
        {
            var text = new StringBuilder();
            text.AppendLine("Plugin: " + PluginMetadata.Title);
            text.AppendLine("Ident: " + PluginMetadata.Ident );
            text.AppendLine("Schema Version: " + PluginMetadata.SchemaVersion.ToString(System.Globalization.CultureInfo.InvariantCulture));
            text.AppendLine("Assembly: " + typeof(PluginCommands).Assembly.FullName);

            MessageBox.Show(text.ToString(), "About Plugin", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void OnRunCommand()
        {
            try
            {
                var mainForm = new FM.MainForm();
                mainForm.ShowDialog();
                mainForm.Dispose();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public static void OnMeasResultsCommand()
        {
            try
            {
                var mainForm = new FM.MeasResultForm();
                mainForm.ShowDialog();
                mainForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
