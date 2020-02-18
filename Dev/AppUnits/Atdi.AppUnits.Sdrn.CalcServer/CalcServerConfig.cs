using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Modules.Licensing;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	class CalcServerConfig : ICalcServerConfig
	{
		public CalcServerConfig(AppServerComponentConfig componentConfig)
		{
			var licenseFileName = componentConfig.LicenseFileName;
			var licenseOwnerId = componentConfig.LicenseOwnerId;
			var licenseProductKey = componentConfig.LicenseProductKey;

			var licenseData = this.VerifyLicense(licenseFileName, licenseOwnerId, licenseProductKey);

			this.Instance = licenseData.Instance;
			this.LicenseNumber = licenseData.LicenseNumber;
			this.LicenseStopDate = licenseData.StopDate;
			this.LicenseStartDate = licenseData.StartDate;
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
				throw new ArgumentNullException(nameof(licenseFileName));
			}

			if (string.IsNullOrEmpty(ownerId))
			{
				throw new ArgumentNullException(nameof(ownerId));
			}

			if (string.IsNullOrEmpty(productKey))
			{
				throw new ArgumentNullException(nameof(productKey));
			}

			try
			{
				var verificationData = new VerificationData2
				{
					OwnerId = ownerId,
					ProductName = "SDRN Calc Server",
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

		public string Instance { get; set; }
		public string LicenseNumber { get; set; }
		public DateTime LicenseStopDate { get; set; }
		public DateTime LicenseStartDate { get; set; }
	}
}
