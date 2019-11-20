using Atdi.Modules.Licensing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Atdi.Tools.Sdrn.Monitoring
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            try
            {
                var licenseFileName = ConfigurationManager.AppSettings["License.FileName"];
                var licenseOwnerId = ConfigurationManager.AppSettings["License.OwnerId"];
                var licenseProductKey = ConfigurationManager.AppSettings["License.ProductKey"];

                var licenseData = this.VerifyLicense(licenseFileName, licenseOwnerId, licenseProductKey);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Incorrect loading: {ex.Message}", "Loading Application", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
            }
        }

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

        private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetAssembly(this.GetType()).CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private VerificationResult VerifyLicense(string licenseFileName, string ownerId, string productKey)
        {
            if (string.IsNullOrEmpty(licenseFileName))
            {
                throw new ArgumentException("message", nameof(licenseFileName));
            }

            if (string.IsNullOrEmpty(ownerId))
            {
                throw new ArgumentException("message", nameof(ownerId));
            }

            if (string.IsNullOrEmpty(productKey))
            {
                throw new ArgumentException("message", nameof(productKey));
            }

            try
            {
                var sharedSecret = "Atdi.Tools.Sdrn.Client";

                var verificationData = new VerificationData2
                {
                    OwnerId = Platform.Cryptography.Encryptor.DecryptStringAES(ownerId, sharedSecret), 
                    ProductName = "ICS Control Monitoring Client",
                    ProductKey = Platform.Cryptography.Encryptor.DecryptStringAES(productKey, sharedSecret),
                    LicenseType = "ClientLicense",
                    Date = DateTime.Now,
                    YearHash = LicenseVerifier.EncodeYear(2020)
                };

                licenseFileName = Path.Combine(this.AssemblyDirectory, licenseFileName);
                var licenseBody = File.ReadAllBytes(licenseFileName);

                var verResult = LicenseVerifier.Verify(verificationData, licenseBody);

                return verResult;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The license verification failed", e);
            }
        }
    }
}
