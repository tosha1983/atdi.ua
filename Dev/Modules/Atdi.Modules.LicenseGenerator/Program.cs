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
            //var path = "C:\\Projects\\Licensing\\UDCR\\WebQuery\\AppServer";
            //WebQueryAppServer_ForUDCR(path);
            //path = "C:\\Projects\\Licensing\\UDCR\\WebQuery\\WebPortal";
            //WebQueryWebPortal_ForUDCR(path);
            UpdatePeriod_ICSControl_ForUDCR(@"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2018");
            Console.WriteLine("Process was finished");
            Console.ReadKey();
        }

        static void UpdatePeriod_ICSControl_ForUDCR(string path)
        {
            var startDate = new DateTime(2018, 12, 25);
            var stopDate = new DateTime(2020, 1, 1);
            var outPath = @"C:\Projects\Licensing\UDCR\Sdrn\Licenses_2019";
            var ownerId = "OID-BD13-G65-N00";
            var ownerKey = "BD13-G65";

            // server
            UpdateLicesePeriod(
                sourcefileName: Path.Combine(path, "ServerLicense\\LIC-SBD13-G65-607.SDRNSV-SBD13-G65-3690.lic"),
                productKey: "MGDM-RD0E-ER0I-6GJR-0DCS",
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate);

            var data = new string[][]
            {
                new string[] { "LIC-DBD13-G65-067.SENSOR-DBD13-G65-3668", "MGIN-1J63-6SAD-EE6P-CDJI" },
                new string[] { "LIC-DBD13-G65-130.SENSOR-DBD13-G65-5854", "FY3I-0CBG-3G1V-DNA1-DBI3" },
                new string[] { "LIC-DBD13-G65-131.SENSOR-DBD13-G65-7613", "J81L-DO1R-EC73-3ELP-8DI0" },
                new string[] { "LIC-DBD13-G65-159.SENSOR-DBD13-G65-4850", "DEOO-TTCL-6S1V-9R3V-S4RN" },
                new string[] { "LIC-DBD13-G65-252.SENSOR-DBD13-G65-1000", "IKBO-SSCS-D2YB-BMLL-ECTF" },
                new string[] { "LIC-DBD13-G65-266.SENSOR-DBD13-G65-3206", "E5MD-35PO-YETY-CIEN-EZ0T" },
                new string[] { "LIC-DBD13-G65-321.SENSOR-DBD13-G65-8938", "H0LZ-DR5E-NZVT-JCVV-DSSS" },
                new string[] { "LIC-DBD13-G65-356.SENSOR-DBD13-G65-9832", "8EIN-KTNZ-UYS8-3NR6-NT09" },
                new string[] { "LIC-DBD13-G65-515.SENSOR-DBD13-G65-4036", "SEE8-ONGD-4VC0-8NS2-HYN0" },
                new string[] { "LIC-DBD13-G65-557.SENSOR-DBD13-G65-2516", "MNOS-7CDE-Q1XM-9Q1H-N3IE" },
                new string[] { "LIC-DBD13-G65-599.SENSOR-DBD13-G65-6781", "DIVV-CWIC-GGOE-D5DD-OT9F" },
                new string[] { "LIC-DBD13-G65-620.SENSOR-DBD13-G65-2314", "BQ35-02C7-ZBC6-DLCE-XG2C" },
                new string[] { "LIC-DBD13-G65-629.SENSOR-DBD13-G65-5768", "CEXS-DL10-D3GD-ETB1-CIEC" },
                new string[] { "LIC-DBD13-G65-680.SENSOR-DBD13-G65-3716", "EC6E-0DGD-5RDN-C6D5-KCCC" },
                new string[] { "LIC-DBD13-G65-786.SENSOR-DBD13-G65-2440", "S715-OC0I-4DLR-E2DG-PIOG" },
                new string[] { "LIC-DBD13-G65-804.SENSOR-DBD13-G65-0561", "EN0O-ISNC-9S1E-CCRZ-QDRD" },
                new string[] { "LIC-DBD13-G65-847.SENSOR-DBD13-G65-6554", "NRD2-C1IS-DEGA-D4RL-ELCO" },
                new string[] { "LIC-DBD13-G65-889.SENSOR-DBD13-G65-8386", "3B53-ELE1-QO02-BI4V-CT8E" },
                new string[] { "LIC-DBD13-G65-898.SENSOR-DBD13-G65-6214", "NDDV-3N2O-L9NC-GTEG-5SCV" },
                new string[] { "LIC-DBD13-G65-973.SENSOR-DBD13-G65-7870", "2JO3-N6I5-SLCG-RI5C-3VH5" },
            };

            foreach (var item in data)
            {
                UpdateLicesePeriod(
                sourcefileName: Path.Combine(path, "DeviceLicense\\" + item[0] +  ".lic"),
                productKey: item[1],
                outPath: outPath,
                ownerId: ownerId,
                ownerKey: ownerKey,
                startDate: startDate,
                stopDate: stopDate);
            }
            // dev 1
            
        }

        static void UpdateLicesePeriod(string sourcefileName, string productKey, string ownerKey, string ownerId, string outPath, DateTime startDate, DateTime stopDate)
        {
            var licBody = File.ReadAllBytes(sourcefileName);
            var lic = LicenseVerifier.GetLicenseInfo(ownerId, productKey, licBody);

            //var licenseIndex = GetUniqueIntegerKey(3);

            lic.Created = DateTime.Now;
            lic.StartDate = startDate;
            lic.StopDate = stopDate;
            //lic.LicenseNumber = $"{licPrefix}{ownerKey}-{licenseIndex}";

            var creator = new LicenseCreator();
            var result = creator.Create(new LicenseData[] { lic });

            var directory = $"{outPath}\\{ownerKey}\\{lic.LicenseType}";
            Directory.CreateDirectory(directory);
            var newfileName = $"{directory}\\{lic.LicenseNumber}.{lic.Instance}.lic";

            File.WriteAllBytes(newfileName, result.Body);

            CreateLicenseDescriptionFile(lic, newfileName);

            var testLicBody = File.ReadAllBytes(newfileName);

            var vd = new VerificationData
            {
                OwnerId = lic.OwnerId,
                ProductName = lic.ProductName,
                ProductKey = lic.ProductKey,
                LicenseType = lic.LicenseType,
                Date = startDate
            };

            var cc = LicenseVerifier.Verify(vd, testLicBody);

            Console.WriteLine($"Update license: '{productKey}' >>> {newfileName}");

        }

        static void WebQueryAppServer_ForUDCR(string path)
        {
            var ownerKey = "BD13-G65";
            var ownerId = "OID-BD13-G65-N00";
            var ownerName = "Державне підприємство «Український державний центр радіочастот»";
            var company = "ТОВ 'Лабораторія інформаційних систем'";

            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Application Server";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQAS";
            var instancePrefix = "APPSRV-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void WebQueryWebPortal_ForUDCR(string path)
        {
            var ownerKey = "BD13-G65";
            var ownerId = "OID-BD13-G65-N00";
            var ownerName = "Державне підприємство «Український державний центр радіочастот»";
            var company = "ТОВ 'Лабораторія інформаційних систем'";

            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Web Portal";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQWP";
            var instancePrefix = "WBP-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void WebQueryAppServer_ForBosny(string path)
        {
            var ownerKey = "CA10-B00";
            var ownerId = "OID-CA10-B00-N00";
            var ownerName = "Regulatorna agencija za komunikacije";
            var company = "ATDI Ukraine";
            
            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Application Server";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQAS";
            var instancePrefix = "APPSRV-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void WebQueryWebPortal_ForBosny(string path)
        {
            var ownerKey = "CA10-B00";
            var ownerId = "OID-CA10-B00-N00";
            var ownerName = "Regulatorna agencija za komunikacije";
            var company = "ATDI Ukraine";
            
            var startDate = new DateTime(2018, 12, 17);
            var stopDate = new DateTime(2020, 1, 1);
            var productName = "WebQuery Web Portal";
            var licenseType = "ServerLicense";

            var srvLicenseIndex = GetUniqueIntegerKey(3);
            var instanceIndex = GetUniqueIntegerKey(4);

            var licPrefix = "LIC-WQWP";
            var instancePrefix = "WBP-WQ";

            MakeLicense(path, licPrefix, instancePrefix, licenseType, productName, srvLicenseIndex, instanceIndex, ownerName, ownerId, ownerKey, company, startDate, stopDate);
        }

        static void WebQueryAppServer_ForTesting(string path)
        {
            var ownerKey = "BD12-A00";
            var ownerId = "OID-BD12-A00-N00";
            var ownerName = "ТОВ 'Лабораторія інформаційних систем'";
            var company = "ТОВ 'Лабораторія інформаційних систем'";
            
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
            CreateLicenseDescriptionFile(l, fileName);

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

        private static void CreateLicenseDescriptionFile(LicenseData l, string fileName)
        {
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
        }
    }
}
