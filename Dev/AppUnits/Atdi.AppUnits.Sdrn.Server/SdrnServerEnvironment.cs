using Atdi.Contracts.Sdrn.Server;
using Atdi.Modules.Licensing;
using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server
{
    public class SdrnServerEnvironment : ISdrnServerEnvironment, ISdrnServerEnvironmentModifier
    {
        private readonly static string _sdatas = "Atdi.AppServer.AppService.SdrnsController";

        public SdrnServerEnvironment(IComponentConfig config)
        {
            var licenseFileName = config.GetParameterAsString("License.FileName");
            var licenseOwnerId = config.GetParameterAsDecodeString("License.OwnerId", _sdatas);
            var licenseProductKey = config.GetParameterAsDecodeString("License.ProductKey", _sdatas);

            var licenseData = this.VerifyLicense(licenseFileName, licenseOwnerId, licenseProductKey);

            this.ServerInstance = licenseData.Instance;
            this.LicenseNumber = licenseData.LicenseNumber;
            this.LicenseStopDate = licenseData.StopDate;
            this.LicenseStartDate = licenseData.StartDate;
            this.ServerRoles = ServerRole.SdrnServer;
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
                var verificationData = new VerificationData
                {
                    OwnerId = ownerId,
                    ProductName = "ICS Control Server",
                    ProductKey = productKey,
                    LicenseType = "ServerLicense",
                    Date = DateTime.Now
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

        public void AddServerRole(ServerRole serverRole)
        {
            this.ServerRoles |= serverRole;
        }

        public string ServerInstance { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime LicenseStopDate { get; set; }
        public DateTime LicenseStartDate { get; set; }

        public ServerRole ServerRoles { get; set; }

        public string MasterServerInstance { get; set; }
    }
}
