using Atdi.Modules.Licensing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.LicenseGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new LicenseCreator();

            var l = new LicenseData
            {
                LicenseNumber = "LCS-2018-TEST",
                LicenseType = "DeviceLicense",
                Company = "ATDI",
                Copyright = "ATDI",
                OwnerId = "OWNID-01181765",
                OwnerName = "Державне підприємство «Український державний центр радіочастот»",
                Created = DateTime.Now,
                StopDate = new DateTime(2019, 1, 1),
                ProductKey = "TEST-TEST-TEST-TEST-TEST",
                ProductName = "ICS Control Device",
                Count = 1,
                Instance = "INS-DV-2018-TEST"
            };

            var result = c.Create(new LicenseData[] { l });
            var fileName = $"C:\\Temp\\{l.LicenseNumber} {l.Instance}.lic";
            File.WriteAllBytes(fileName, result.Body);

            var ownerId = Encryptor.EncryptStringAES(l.OwnerId, "Atdi.WcfServices.Sdrn.Device");
            var productKey = Encryptor.EncryptStringAES(l.ProductKey, "Atdi.WcfServices.Sdrn.Device");

            var verFileData = new StringBuilder();
            verFileData.AppendLine($"Encoded Owner Id = '{ownerId}'");
            verFileData.AppendLine($"Encoded Product Key = '{productKey}'");
            verFileData.AppendLine();
            verFileData.AppendLine("-- Verification Data --");
            verFileData.AppendLine($"Owner Id = '{l.OwnerId}'");
            verFileData.AppendLine($"Product Name = '{l.ProductName}'");
            verFileData.AppendLine($"Product Key = '{l.ProductKey}'");
            verFileData.AppendLine($"License Type = '{l.LicenseType}'");

            File.WriteAllText(fileName + ".txt", verFileData.ToString(), Encoding.UTF8);

            var licBody = File.ReadAllBytes(fileName);

            var vd = new VerificationData
            {
                OwnerId = l.OwnerId,
                ProductName = l.ProductName,
                ProductKey = l.ProductKey,
                LicenseType = l.LicenseType,
                Date = DateTime.Now
            };

            var cc = LicenseVerifier.Verify(vd, licBody);
        }
    }
}
