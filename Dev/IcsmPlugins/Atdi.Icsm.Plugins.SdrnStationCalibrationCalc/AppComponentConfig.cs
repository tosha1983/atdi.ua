using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.Modules.Licensing;
using System.IO;
using System.Reflection;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc
{
	public class AppComponentConfig
	{
		private const string LicenseSharedSecret = "A77839F8-5546-41C9-A6D9-3777894D3E41";

		[ComponentConfigProperty("License.FileName")]
		public string LicenseFileName { get; set; }

		[ComponentConfigProperty("License.OwnerId", SharedSecret = LicenseSharedSecret)]
		public string LicenseOwnerId { get; set; }

		[ComponentConfigProperty("License.ProductKey", SharedSecret = LicenseSharedSecret)]
		public string LicenseProductKey { get; set; }

		[ComponentConfigProperty("CalcServer.EntityOrm.Endpoint.BaseAddress")]
		public string CalcServerEntityOrmEndpointBaseAddress { get; set; }

		[ComponentConfigProperty("CalcServer.EntityOrm.Endpoint.ApiUri")]
		public string CalcServerEntityOrmEndpointApiUri { get; set; }

		[ComponentConfigProperty("CalcServer.EntityOrm.DataContext")]
		public string CalcServerEntityOrmDataContext { get; set; }


		[ComponentConfigProperty("Infocenter.EntityOrm.Endpoint.BaseAddress")]
		public string InfocenterEntityOrmEndpointBaseAddress { get; set; }

		[ComponentConfigProperty("Infocenter.EntityOrm.Endpoint.ApiUri")]
		public string InfocenterEntityOrmEndpointApiUri { get; set; }

		[ComponentConfigProperty("Infocenter.EntityOrm.DataContext")]
		public string InfocenterEntityOrmDataContext { get; set; }

		public string Instance { get; set; }

        [ComponentConfigProperty("Threshold.DriveTest.GsmPoints.FetchRows")]
        public int MaximumCountPointsInDriveTestsFor_GSM { get; set; }

        [ComponentConfigProperty("Threshold.DriveTest.UmtsPoints.FetchRows")]
        public int MaximumCountPointsInDriveTestsFor_UMTS { get; set; }

        [ComponentConfigProperty("Threshold.DriveTest.LtePoints.FetchRows")]
        public int MaximumCountPointsInDriveTestsFor_LTE { get; set; }

        [ComponentConfigProperty("Threshold.CdmaPoints.LtePoints.FetchRows")]
        public int MaximumCountPointsInDriveTestsFor_CDMA { get; set; }

        public void VerifyLicense()
		{
			var licenseFileName = this.LicenseFileName;
			var licenseOwnerId = this.LicenseOwnerId;
			var licenseProductKey = this.LicenseProductKey;

			var licenseData = this.VerifyLicense(licenseFileName, licenseOwnerId, licenseProductKey);

			this.Instance = licenseData.Instance;
			//this.LicenseNumber = licenseData.LicenseNumber;
			//this.LicenseStopDate = licenseData.StopDate;
			//this.LicenseStartDate = licenseData.StartDate;
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
					ProductName = "ICSM Plugin - SDRN Station Calibration Calc",
					ProductKey = productKey,
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
				throw new InvalidOperationException($"The license verification failed. {e.Message}");
			}
		}
	}
}
