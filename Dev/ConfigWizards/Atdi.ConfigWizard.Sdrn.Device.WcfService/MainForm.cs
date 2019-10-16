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

                var rabbitMqHost = GetParameter(doc, "RabbitMQ.Host");
                var rabbitMqPort = GetParameter(doc, "RabbitMQ.Port");
                var rabbitMqVirtualHost = GetParameter(doc, "RabbitMQ.VirtualHost");
                var rabbitMqUser = GetParameter(doc, "RabbitMQ.User");
                var rabbitMqPassword = GetParameter(doc, "RabbitMQ.Password");

                var sdrnServerInstance = GetParameter(doc, "SDRN.Server.Instance");
                var sdrnSensorTechId = GetParameter(doc, "SDRN.Device.SensorTechId");
                var sdrnMessagesExchange = GetParameter(doc, "SDRN.Device.Exchange");
                var sdrnServerQueueNamePart = GetParameter(doc, "SDRN.Server.QueueNamePart");
                var sdrnDeviceQueueNamePart = GetParameter(doc, "SDRN.Device.QueueNamePart");

                var sdrnMessagesBindings = GetParameter(doc, "SDRN.Device.MessagesBindings");
                var sdrnContentType = GetParameter(doc, "DeviceBus.ContentType");
                var sdrnSharedSecretKey = GetParameter(doc, "DeviceBus.SharedSecretKey");

                var inboxPath = GetParameter(doc, "DeviceBus.Inbox.Buffer.Folder");
                var outboxPath = GetParameter(doc, "DeviceBus.Outbox.Buffer.Folder");

                var sdrnUseEncryption = GetParameter(doc, "SDRN.MessageConvertor.UseEncryption");
                var sdrnUseCompression = GetParameter(doc, "SDRN.MessageConvertor.UseCompression");

                var sdrnInboxContentType = GetParameter(doc, "DeviceBus.Inbox.Buffer.ContentType");
                var sdrnOutboxContentType = GetParameter(doc, "DeviceBus.Outbox.Buffer.ContentType");

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
                
                

                txtRabbitMQHost.Text = rabbitMqHost;
                txtRabbitMQPort.Text = rabbitMqPort;
                txtRabbitMQVirtualHost.Text = rabbitMqVirtualHost;
                txtRabbitMQUser.Text = rabbitMqUser;

                try
                {
                    txtRabbitMQPassword.Text = Encryptor.DecryptStringAES(rabbitMqPassword, SharedSecret);
                }
                catch (Exception) { }
                

                txtSdrnServerInstance.Text = sdrnServerInstance;
                txtSensorTechId.Text = sdrnSensorTechId;
                txtSdrnMessagesExchange.Text = sdrnMessagesExchange;
                txtSdrnServerQueueNamePart.Text = sdrnServerQueueNamePart;
                txtSdrnDeviceQueueNamePart.Text = sdrnDeviceQueueNamePart;
                txtMessagesBindings.Text = sdrnMessagesBindings;
                txtContentType.Text = sdrnContentType;
                txtSharedSecretKey.Text = sdrnSharedSecretKey;

                txtInboxPath.Text = inboxPath;
                txtOutboxPath.Text = outboxPath;

                txtUseEncryption.CheckState = "true".Equals(sdrnUseEncryption, StringComparison.OrdinalIgnoreCase)
                    ? CheckState.Checked
                    : CheckState.Unchecked;

                txtUseCompression.CheckState = "true".Equals(sdrnUseCompression, StringComparison.OrdinalIgnoreCase)
                    ? CheckState.Checked
                    : CheckState.Unchecked;

                txtInboxContentType.Text = sdrnInboxContentType;
                txtOutboxContentType.Text = sdrnOutboxContentType;

                txtWcfBindings.Text = wcfBindings;
            }
            catch(Exception)
            {
                throw;
            }
        }

        private static string GetParameter(XmlDocument doc, string paramName)
        {
            var node = doc.SelectSingleNode($"/configuration/atdi.platform/appServer/components/component[@type='SdrnDeviceServices']/parameters/parameter[@name='{paramName}']/@value");

            return node?.InnerText;
        }

        private static void SetParameter(XmlDocument doc, string paramName, string value)
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

            if (parameter.Attributes != null)
            {
                var va = parameter.Attributes["value"];
                if (va == null)
                {
                    va = doc.CreateAttribute("value");
                    parameter.Attributes.Append(va);
                }
                va.Value = value;
            }
        }

        private void SaveControlsDataToConfig(string fileName)
        {
            var licenseFileName = txtLicenseFileName.Text;
            var licenseOwnerId = Encryptor.EncryptStringAES(txtLicenseOwnerId.Text, SharedSecret);
            var licenseProductKey = Encryptor.EncryptStringAES(txtLicenseProductKey.Text, SharedSecret);

            var rabbitMqHost = txtRabbitMQHost.Text;
            var rabbitMqPort = txtRabbitMQPort.Text;
            var rabbitMqVirtualHost = txtRabbitMQVirtualHost.Text;
            var rabbitMqUser = txtRabbitMQUser.Text;
            var rabbitMqPassword = Encryptor.EncryptStringAES(txtRabbitMQPassword.Text, SharedSecret);

            var sdrnServerInstance = txtSdrnServerInstance.Text;
            var sdrnSensorTechId = txtSensorTechId.Text;
            var sdrnMessagesExchange = txtSdrnMessagesExchange.Text;
            var sdrnMessagesBindings = txtMessagesBindings.Text;
            var sdrnServerQueueNamePart = txtSdrnServerQueueNamePart.Text;
            var sdrnDeviceQueueNamePart = txtSdrnDeviceQueueNamePart.Text;
            var sdrnContentType = txtContentType.Text;
            var sdrnSharedSecretKey = txtSharedSecretKey.Text;

            var inboxPath = txtInboxPath.Text;
            var outboxPath = txtOutboxPath.Text;

            var sdrnUseEncryption = txtUseEncryption.CheckState == CheckState.Checked ? "true" : "false";
            var sdrnUseCompression = txtUseCompression.CheckState == CheckState.Checked ? "true" : "false";

            var sdrnInboxContentType = txtInboxContentType.Text;
            var sdrnOutboxContentType = txtOutboxContentType.Text;

            var wcfBindings = txtWcfBindings.Text;

            var path = this.AssemblyDirectory;
            fileName = Path.Combine(path, fileName);

            var doc = new XmlDocument();
            doc.Load(fileName);

            SetParameter(doc, "IMeasTasksBus", wcfBindings);
            SetParameter(doc, "License.FileName", licenseFileName);
            SetParameter(doc, "License.OwnerId", licenseOwnerId);
            SetParameter(doc, "License.ProductKey", licenseProductKey);

            SetParameter(doc, "RabbitMQ.Host", rabbitMqHost);
            SetParameter(doc, "RabbitMQ.Port", rabbitMqPort);
            SetParameter(doc, "RabbitMQ.VirtualHost", rabbitMqVirtualHost);
            SetParameter(doc, "RabbitMQ.User", rabbitMqUser);
            SetParameter(doc, "RabbitMQ.Password", rabbitMqPassword);

            SetParameter(doc, "SDRN.Server.Instance", sdrnServerInstance);
            SetParameter(doc, "SDRN.Device.SensorTechId", sdrnSensorTechId);
            SetParameter(doc, "SDRN.Device.Exchange", sdrnMessagesExchange);
            SetParameter(doc, "SDRN.Server.QueueNamePart", sdrnServerQueueNamePart);
            SetParameter(doc, "SDRN.Device.QueueNamePart", sdrnDeviceQueueNamePart);

            SetParameter(doc, "SDRN.Device.MessagesBindings", sdrnMessagesBindings);

            SetParameter(doc, "DeviceBus.ContentType", sdrnContentType);
            SetParameter(doc, "DeviceBus.SharedSecretKey", sdrnSharedSecretKey);

            SetParameter(doc, "DeviceBus.Outbox.UseBuffer", "FileSystem");
            SetParameter(doc, "DeviceBus.Inbox.UseBuffer", "FileSystem");

            SetParameter(doc, "DeviceBus.Outbox.Buffer.Folder", outboxPath);
            SetParameter(doc, "DeviceBus.Inbox.Buffer.Folder", inboxPath);

            SetParameter(doc, "SDRN.MessageConvertor.UseEncryption", sdrnUseEncryption);
            SetParameter(doc, "SDRN.MessageConvertor.UseCompression", sdrnUseCompression);

            SetParameter(doc, "DeviceBus.Outbox.Buffer.ContentType", sdrnOutboxContentType);
            SetParameter(doc, "DeviceBus.Inbox.Buffer.ContentType", sdrnInboxContentType);

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

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
