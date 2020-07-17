using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.AppServices.WebQuery;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Modules.Licensing;
using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using Microsoft.Win32;

namespace Atdi.AppServices.WebQuery
{
    public sealed class WebQueryComponent : AppServicesComponent<IWebQuery>
    {
	    private const string SharedKey = "Atdi.AppServices.WebQuery";

		private VerificationResult _verificationResult;

        public WebQueryComponent() 
            : base("WebQueryAppServices")
        {
        }

        protected override void OnInstall()
        {
            base.OnInstall();

            var licenseFileName = this.Config.GetParameterAsString("License.FileName");
            var licenseOwnerId = this.Config.GetParameterAsDecodeString("License.OwnerId", SharedKey);
            var licenseProductKey = this.Config.GetParameterAsDecodeString("License.ProductKey", SharedKey);

            this._verificationResult = this.VerifyLicense(licenseFileName, licenseOwnerId, licenseProductKey);

            this.Container.Register<QueriesRepository>(Platform.DependencyInjection.ServiceLifetime.PerThread);
            this.Container.Register<GroupDescriptorsCache>(Platform.DependencyInjection.ServiceLifetime.PerThread);
            this.Container.Register<QueryDescriptorsCache>(Platform.DependencyInjection.ServiceLifetime.PerThread);
            this.Container.Register<UserGroupDescriptorsCache>(Platform.DependencyInjection.ServiceLifetime.PerThread);
        }

        protected override void OnActivate()
        {
	        base.OnActivate();

	        var externalServicesProvider = this.Resolver.Resolve<IExternalServiceProvider>();
	        if (_verificationResult.ExternalServices != null && _verificationResult.ExternalServices.Length > 0)
	        {
		        foreach (var externalServiceDescriptor in _verificationResult.ExternalServices)
		        {
			        try
			        {
				        var service = new ExternalService
				        {
					        Id = externalServiceDescriptor.Id,
					        Name = externalServiceDescriptor.Name,
					        SecretKey = this.Config.GetParameterAsDecodeString($"ExternalServices.{externalServiceDescriptor.Name}.SecretKey", SharedKey)
				        };

				        externalServicesProvider.Register(service);
					}
			        catch (Exception e)
			        {
				        this.Logger.Exception(Contexts.WebQueryAppServices, Categories.Init, $"External service registration error: SID='{externalServiceDescriptor.Id}', Name='{externalServiceDescriptor.Name}'", e, this);
			        }
			        
		        }
			}
	        
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
                var verificationData = new VerificationData2
                {
                    OwnerId = ownerId,
                    ProductName = "WebQuery Application Server",
                    ProductKey = productKey,
                    LicenseType = "ServerLicense",
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
