using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using System.Windows.Forms;
using FM = Atdi.Icsm.Plugins.SdrnCalcServerClient.Forms;
using Atdi.Icsm.Plugins.SdrnCalcServerClient;

namespace XICSM.SdrnCalcServerClient
{
    public class PluginCommands
    {
        public static void OnAboutCommand()
        {
            var text = new StringBuilder();
            text.AppendLine("Plugin: " + PluginMetadata.Title);
            text.AppendLine("Ident: " + PluginMetadata.Ident);
            text.AppendLine("Schema Version: " + PluginMetadata.SchemaVersion.ToString(System.Globalization.CultureInfo.InvariantCulture));
            text.AppendLine("Assembly: " + typeof(PluginCommands).Assembly.FullName);

            MessageBox.Show(text.ToString(), "About Plugin", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void OnRunCommand()
        {
            try
            {
                var mainForm = new FM.WpfStandardForm("ProjectManager.xaml", "ProjectManagerViewModel");
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
