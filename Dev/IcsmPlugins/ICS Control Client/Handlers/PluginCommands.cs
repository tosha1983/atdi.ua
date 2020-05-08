using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using System.Windows.Forms;
using XICSM.ICSControlClient.Environment;
using XICSM.ICSControlClient.WcfServiceClients;
using MD = XICSM.ICSControlClient.Metadata;
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
        public static void OnAnalyzeEmissionsCommand()
        {
            try
            {
                var mainForm = new FM.SignalizationSensorsForm(2, null, null);
                mainForm.ShowDialog();
                mainForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public static void OnGroupeSynchronizationEmissionsWithStationICSM()
        {
            try
            {
                var mainForm = new FM.GroupeEmissionWithStationForm();
                mainForm.ShowDialog();
                mainForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public static void ExportSODatatoCSV()
        {
            try
            {
                var mainForm = new FM.ExportSODatatoCSVForm();
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
