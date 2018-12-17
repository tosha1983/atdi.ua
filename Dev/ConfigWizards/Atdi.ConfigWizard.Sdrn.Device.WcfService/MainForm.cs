using Atdi.Platform.Cryptography;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Atdi.ConfigWizard.Sdrn.Device.WcfService
{
    public partial class MainForm : Form
    {
        private readonly string SvcConfigFileName = "Atdi.AppServer.Svchost.exe.config";
        private readonly string ConsoleConfigFileName = "Atdi.AppServer.Console.exe.config";
        private readonly string SharedSecret = "Atdi.WcfServices.Sdrn.Device";

        public MainForm()
        {
            InitializeComponent();
            LoadConfigDataToControls();

        }

        public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetAssembly(this.GetType()).CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private void LoadConfigDataToControls()
        {
            try
            {
                var path = this.AssemblyDirectory;
                var fileName = Path.Combine(path, SvcConfigFileName);
                var doc = new XmlDocument();
                doc.Load(fileName);

                var wcfBindings = GetParameter(doc, "IMeasTasksBus");
                var licenseFileName = GetParameter(doc, "License.FileName");
                var licenseOwnerId = GetParameter(doc, "License.OwnerId");
                var licenseProductKey = GetParameter(doc, "License.ProductKey");

                var rabbitMQHost = GetParameter(doc, "RabbitMQ.Host");
                var rabbitMQVirtualHost = GetParameter(doc, "RabbitMQ.VirtualHost");
                var rabbitMQUser = GetParameter(doc, "RabbitMQ.User");
                var rabbitMQPassword = GetParameter(doc, "RabbitMQ.Password");

                var sdrnServerInstance = GetParameter(doc, "SDRN.ServerInstance");
                var sdrnMessagesExchange = GetParameter(doc, "SDRN.MessagesExchange");
                var sdrnServerQueueNamePart = GetParameter(doc, "SDRN.ServerQueueNamePart");
                var sdrnDeviceQueueNamePart = GetParameter(doc, "SDRN.DeviceQueueNamePart");

                var inboxPath = GetParameter(doc, "Messages.InboxFolder");
                var outboxPath = GetParameter(doc, "Messages.OutboxFolder");

                txtLicenseFileName.Text = licenseFileName;
                try
                {
                    txtLicenseOwnerId.Text = Encryptor.DecryptStringAES(licenseOwnerId, SharedSecret);
                }
                catch (Exception) { }
                try
                {
                    txtLicenseProductKey.Text = Encryptor.DecryptStringAES(licenseProductKey, SharedSecret);
                }
                catch (Exception) { }
                
                

                txtRabbitMQHost.Text = rabbitMQHost;
                txtRabbitMQVirtualHost.Text = rabbitMQVirtualHost;
                txtRabbitMQUser.Text = rabbitMQUser;

                try
                {
                    txtRabbitMQPassword.Text = Encryptor.DecryptStringAES(rabbitMQPassword, SharedSecret);
                }
                catch (Exception) { }
                

                txtSdrnServerInstance.Text = sdrnServerInstance;
                txtSdrnMessagesExchange.Text = sdrnMessagesExchange;
                txtSdrnServerQueueNamePart.Text = sdrnServerQueueNamePart;
                txtSdrnDeviceQueueNamePart.Text = sdrnDeviceQueueNamePart;

                txtInboxPath.Text = inboxPath;
                txtOutboxPath.Text = outboxPath;

                txtWcfBindings.Text = wcfBindings;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private string GetParameter(XmlDocument doc, string paramName)
        {
            var node = doc.SelectSingleNode($"/configuration/atdi.platform/appServer/components/component[@type='SdrnDeviceServices']/parameters/parameter[@name='{paramName}']/@value");
            if (node == null)
            {
                return null;
            }

            return node.InnerText;
        }

        private void SetParameter(XmlDocument doc, string paramName, string value)
        {
            var parameters = doc.SelectSingleNode("/configuration/atdi.platform/appServer/components/component[@type='SdrnDeviceServices']/parameters");
            var parameter = parameters.SelectSingleNode($"parameter[@name='{paramName}']");
            // /@value");
            if (parameter == null)
            {
                var element = doc.CreateElement("parameter");
                var n = doc.CreateAttribute("name");
                n.Value = paramName;
                element.Attributes.Append(n);
                var v = doc.CreateAttribute("value");
                v.Value = value;
                element.Attributes.Append(v);
                parameters.AppendChild(element);
                return;
            }

            var va = parameter.Attributes["value"];
            if (va == null)
            {
                va = doc.CreateAttribute("value");
                parameter.Attributes.Append(va);
            }
            va.Value = value;
        }

        private void SaveControlsDataToConfig(string fileName)
        {
            var licenseFileName = txtLicenseFileName.Text;
            var licenseOwnerId = Encryptor.EncryptStringAES(txtLicenseOwnerId.Text, SharedSecret);
            var licenseProductKey = Encryptor.EncryptStringAES(txtLicenseProductKey.Text, SharedSecret);

            var rabbitMQHost = txtRabbitMQHost.Text;
            var rabbitMQVirtualHost = txtRabbitMQVirtualHost.Text;
            var rabbitMQUser = txtRabbitMQUser.Text;
            var rabbitMQPassword = Encryptor.EncryptStringAES(txtRabbitMQPassword.Text, SharedSecret);

            var sdrnServerInstance = txtSdrnServerInstance.Text;
            var sdrnMessagesExchange = txtSdrnMessagesExchange.Text;
            var sdrnServerQueueNamePart = txtSdrnServerQueueNamePart.Text;
            var sdrnDeviceQueueNamePart = txtSdrnDeviceQueueNamePart.Text;

            var inboxPath = txtInboxPath.Text;
            var outboxPath = txtOutboxPath.Text;

            var wcfBindings = txtWcfBindings.Text;

            var path = this.AssemblyDirectory;
            fileName = Path.Combine(path, fileName);

            var doc = new XmlDocument();
            doc.Load(fileName);

            SetParameter(doc, "IMeasTasksBus", wcfBindings);
            SetParameter(doc, "License.FileName", licenseFileName);
            SetParameter(doc, "License.OwnerId", licenseOwnerId);
            SetParameter(doc, "License.ProductKey", licenseProductKey);

            SetParameter(doc, "RabbitMQ.Host", rabbitMQHost);
            SetParameter(doc, "RabbitMQ.VirtualHost", rabbitMQVirtualHost);
            SetParameter(doc, "RabbitMQ.User", rabbitMQUser);
            SetParameter(doc, "RabbitMQ.Password", rabbitMQPassword);

            SetParameter(doc, "SDRN.ServerInstance", sdrnServerInstance);
            SetParameter(doc, "SDRN.MessagesExchange", sdrnMessagesExchange);
            SetParameter(doc, "SDRN.ServerQueueNamePart", sdrnServerQueueNamePart);
            SetParameter(doc, "SDRN.DeviceQueueNamePart", sdrnDeviceQueueNamePart);

            SetParameter(doc, "Messages.OutboxFolder", outboxPath);
            SetParameter(doc, "Messages.InboxFolder", inboxPath);

            doc.Save(fileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveControlsDataToConfig(SvcConfigFileName);
            SaveControlsDataToConfig(ConsoleConfigFileName);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
