using Atdi.Modules.Licensing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.LicenseGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = "C:\\Projects\\Licensing\\WebQuery\\AppServer";
            WebQueryAppServer_ForTesting(path);
            path = "C:\\Projects\\Licensing\\WebQuery\\WebPortal";
            WebQueryWebPortal_ForTesting(path);
        }

        static void WebQueryAppServer_ForTesting(string path)
        {
            var ownerId = "OID-BD12-A00-N00";
            var ownerName = "ТОВ 'Лабораторія інформаційних систем'";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            var ownerKey = "BD12-A00";
            var startDate = new DateTime(2018, 12, 5);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Application Server";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQAS";
            var instancePrefix = "APPSRV-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void WebQueryWebPortal_ForTesting(string path)
        {
            var ownerId = "OID-BD12-A00-N00";
            var ownerName = "ТОВ 'Лабораторія інформаційних систем'";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            var ownerKey = "BD12-A00";
            var startDate = new DateTime(2018, 12, 5);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Web Portal";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQWP";
            var instancePrefix = "WBP-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void ICSControl_ForTesting(string path)
        {
            var ownerId = "OID-BD12-A00-N00";
            var ownerName = "ТОВ 'Лабораторія інформаційних систем'";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            var ownerKey = "BD12-A00";
            var startDate = new DateTime(2018, 9, 12);
            var stopDate = new DateTime(2019, 1, 1);

            //MakeServerLicense();
            for (int i = 0; i < 10; i++)
            {
                var licenseIndex = GetUniqueIntegerKey(3);
                var deviceIndex = GetUniqueIntegerKey(4);
                var licPrefix = "LIC-D";
                var instancePrefix = "SENSOR-D";
                MakeLicense(path, licPrefix, instancePrefix, "DeviceLicense", "ICS Control Device", licenseIndex, deviceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
            }

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);
            var srvLicPrefix = "LIC-S";
            var srvInstancePrefix = "SDRNSV-S";

            MakeLicense(path, srvLicPrefix, srvInstancePrefix, "ServerLicense", "ICS Control Server", srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void ICSControl_ForUDCR(string path)
        {
            var ownerId = "OID-BD13-G65-N00";
            var ownerName = "Державне підприємство «Український державний центр радіочастот»";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            var ownerKey = "BD13-G65";
            var startDate = new DateTime(2018, 9, 17);
            var stopDate = new DateTime(2019, 1, 1);

            //MakeServerLicense();
            for (int i = 0; i < 20; i++)
            {
                var licenseIndex = GetUniqueIntegerKey(3);
                var deviceIndex = GetUniqueIntegerKey(4);
                var licPrefix = "LIC-D";
                var instancePrefix = "SENSOR-D";
                MakeLicense(path, licPrefix, instancePrefix, "DeviceLicense", "ICS Control Device", licenseIndex, deviceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
            }

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);
            var srvLicPrefix = "LIC-S";
            var srvInstancePrefix = "SDRNSV-S";

            MakeLicense(path, srvLicPrefix, srvInstancePrefix, "ServerLicense", "ICS Control Server", srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        public static string GetProductKey(string productName, string licenseType, string instance, string ownerId, string number)
        {
            var source = productName + "ZXCV158BNM" + licenseType + "ASD290FGHJKL" + instance + "QWE346RTYU7IOP" + ownerId + number;
            var data = new Stack<string>();
            for (int i = 0; i < 5; i++)
            {
                data.Push(GetUniqueKey(source, 4));
            }

            return string.Join("-", data.ToArray());
        }

        public static string GetUniqueKey(int maxSize)
        {
            var chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ2451678390".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GetUniqueKey(string source, int maxSize)
        {
            var chars =
           source.ToUpper().Replace(" ", "").Replace("-", "").ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GetUniqueIntegerKey(int maxSize)
        {
            var chars =
            "2146389507".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        

        private static string MakeLicense(string path, string licPrefix, string instancePrefix, string licenseType, string productName, string licenseIndex, string instanceIndex, string ownerName, string ownerId, string ownerKey, string company, DateTime startDate, DateTime stopDate)
        {
            var productKey = string.Empty;

            var c = new LicenseCreator();

            var l = new LicenseData
            {
                //LicenseNumber = $"LIC-D{ownerKey}-{licenseIndex}",
                LicenseType = licenseType, //"DeviceLicense",
                Company = company,
                Copyright = "",
                OwnerId = ownerId,
                OwnerName = ownerName,
                Created = DateTime.Now,
                StartDate = startDate,
                StopDate = stopDate,
                ProductKey = productKey,
                ProductName = productName,
                Count = 1,
                //Instance = $"SENSOR-D{ownerKey}-{deviceIndex}"
            };

            if ("DeviceLicense".Equals(licenseType))
            {
                l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
                l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
            }
            else if ("ServerLicense".Equals(licenseType))
            {
                l.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";
                l.Instance = $"{instancePrefix}{ownerKey}-{instanceIndex}";
            }

            productKey = GetProductKey(l.ProductName, l.LicenseType, l.Instance, l.OwnerId, l.LicenseNumber);
            l.ProductKey = productKey;

            var result = c.Create(new LicenseData[] { l });

            var directory = $"{path}\\{ownerKey}\\{licenseType}";
            Directory.CreateDirectory(directory);

            var fileName = $"{directory}\\{l.LicenseNumber}.{l.Instance}.lic";

            File.WriteAllBytes(fileName, result.Body);

            var verFileData = new StringBuilder();

            verFileData.AppendLine();
            verFileData.AppendLine("  -- License Data -- ");
            verFileData.AppendLine($"License Number    : '{l.LicenseNumber}'");
            verFileData.AppendLine($"License Type      : {l.LicenseType}");
            verFileData.AppendLine($"Issued by Company : {l.Company}");
            verFileData.AppendLine("  ------------------ ");
            verFileData.AppendLine();
            verFileData.AppendLine($"Owner Id     : '{l.OwnerId}'");
            verFileData.AppendLine($"Owner Name   : '{l.OwnerName}'");
            verFileData.AppendLine($"Product Name : '{l.ProductName}'");
            verFileData.AppendLine($"Product Key  : '{l.ProductKey}'");
            verFileData.AppendLine("  ------------------ ");
            verFileData.AppendLine();
            verFileData.AppendLine($"Created    : {l.Created}");
            verFileData.AppendLine($"Start Date : {l.StartDate}");
            verFileData.AppendLine($"Stop Date  : {l.StopDate}");
            verFileData.AppendLine("  ------------------ ");
            verFileData.AppendLine();
            verFileData.AppendLine($"Instance : '{l.Instance}'");
            verFileData.AppendLine("  ------------------ ");


            File.WriteAllText(fileName + ".txt", verFileData.ToString(), Encoding.UTF8);

            var licBody = File.ReadAllBytes(fileName);

            var vd = new VerificationData
            {
                OwnerId = l.OwnerId,
                ProductName = l.ProductName,
                ProductKey = l.ProductKey,
                LicenseType = l.LicenseType,
                Date = startDate
            };

            var cc = LicenseVerifier.Verify(vd, licBody);

            Console.WriteLine($"Made license: '{productKey}' >>> {fileName}");
            return productKey;
        }
    }
}
