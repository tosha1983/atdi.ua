using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ControlU.DB
{
    using Atdi.Modules.Licensing;
    using System.IO;

    internal class ConvertData
    {
        string result = "";
        public ConvertData(string datain1, string datain2)
        {
            // получить с конфигурации, приведенные значения для тестовой лицензии.
            var ownerId = datain1;
            var productKey = datain2;
            var licenseFileName = "license.lic";

            // зашит в код
            var verificationData = new VerificationData
            {
                OwnerId = ownerId,
                ProductName = "ICS Control Device",
                ProductKey = productKey,
                LicenseType = "DeviceLicense",
                Date = DateTime.Now
            };
            if (File.Exists(licenseFileName))
            {
                var licenseBody = File.ReadAllBytes(licenseFileName);
                try
                {
                    if (licenseBody != null)
                    {
                        var verResult = LicenseVerifier.Verify(verificationData, licenseBody);
                        //Console.WriteLine("License OK. Instance name: '" + verResult.Instance + "'");
                        result = verResult.Instance;
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine("License verification failed: " + e.Message);
                    result = "";
                }
            }

           
        }
        public string GetConvertedData()
        {
            return result;
        }
    }
}
