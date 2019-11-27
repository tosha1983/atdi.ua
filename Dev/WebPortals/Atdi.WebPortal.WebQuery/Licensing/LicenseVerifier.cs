using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.Licensing
{
    public class LicenseVerifier
    {
        private const string key = "Atdi.Modules.Licensing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79d55ebd8cf97c51";

        private class TypeBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (typeName == "Atdi.Modules.Licensing.LicenseData")
                {
                    return typeof(LicenseData);
                }
                if (typeName == "Atdi.Modules.Licensing.LicenseData2")
                {
                    return typeof(LicenseData2);
                }
                var type = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
                return type;
            }
        }
        public static VerificationResult Verify(VerificationData data, byte[] licenseBody)
        {
            //formatter.Serialize(stream, licenses);
            //var raw = Convert.ToBase64String(stream.ToArray());
            //var key = Assembly.GetAssembly(typeof(LicenseData)).FullName;
            //var encodeVal = Encryptor.EncryptStringAES(raw, key);

            //return new LicenseCrationResult
            //{
            //    Body = Encoding.UTF8.GetBytes(encodeVal)
            //};

             // Assembly.GetAssembly(typeof(LicenseData)).FullName;
            var encodeValue = Encoding.UTF8.GetString(licenseBody);
            var raw = Encryptor.DecryptStringAES(encodeValue, key);
            var licData = Convert.FromBase64String(raw);
            
            using (var memoryStream = new MemoryStream(licData))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Binder = new TypeBinder();
                var lics = (LicenseData[])formatter.Deserialize(memoryStream);

                if (lics != null && lics.Length > 0)
                {
                    for (int i = 0; i < lics.Length; i++)
                    {
                        var lic = lics[i];
                        if (lic.OwnerId.Equals(data.OwnerId, StringComparison.OrdinalIgnoreCase)
                            && lic.LicenseType.Equals(data.LicenseType, StringComparison.OrdinalIgnoreCase)
                            && lic.ProductName.Equals(data.ProductName, StringComparison.OrdinalIgnoreCase)
                            && lic.ProductKey.Equals(data.ProductKey, StringComparison.OrdinalIgnoreCase)
                        )
                        {

                            // Is it the second protocol?
                            if (lic is LicenseData2 license2)
                            {
                                if (!(data is VerificationData2 data2))
                                {
                                    throw new Exception("Invalid verification data.");
                                }

                                if ((license2.LimitationTerms & LicenseLimitationTerms.TimePeriod) != 0)
                                {
                                    if (license2.StartDate > data2.Date)
                                    {
                                        throw new Exception("The time to start using the license has not yet come");
                                    }
                                    if (license2.StopDate <= data2.Date)
                                    {
                                        throw new Exception("License was expired");
                                    }
                                }

                                if ((license2.LimitationTerms & LicenseLimitationTerms.Year) != 0)
                                {
                                    var year = DecodeYear(data2.YearHash);
                                    if (license2.Year < year)
                                    {
                                        throw new Exception("License was expired");
                                    }
                                }

                                if ((license2.LimitationTerms & LicenseLimitationTerms.Version) != 0)
                                {
                                    if (license2.Version.Equals(data2.Version))
                                    {
                                        throw new Exception("Product version does not match license version");
                                    }
                                }

                                if ((license2.LimitationTerms & LicenseLimitationTerms.Assembly) != 0)
                                {
                                    if (license2.AssemblyFullName.Equals(data2.AssemblyFullName))
                                    {
                                        throw new Exception("Assembly does not match license assembly");
                                    }
                                }

                                if ((license2.LimitationTerms & LicenseLimitationTerms.Host) != 0)
                                {
                                    if (license2.HostData.Equals(DefineHostData()))
                                    {
                                        throw new Exception("Host data does not match license data");
                                    }
                                }
                            }
                            else
                            {
                                // This is the first protocol.
                                if (lic.StartDate > data.Date)
                                {
                                    throw new Exception("The time to start using the license has not yet come");
                                }
                                if (lic.StopDate <= data.Date)
                                {
                                    throw new Exception("License was expired");
                                }


                            }

                            return new VerificationResult
                            {
                                LicenseNumber = lic.LicenseNumber,
                                Count = lic.Count,
                                Instance = lic.Instance,
                                OwnerName = lic.OwnerName,
                                StopDate = lic.StopDate,
                                StartDate = lic.StartDate
                            };
                        }
                    }
                }
            }

            throw new Exception("Invalid license data");            
        }


        public static LicenseData GetLicenseInfo(string ownerId, string productKey, byte[] licenseBody)
        {

            var key = Assembly.GetAssembly(typeof(LicenseData)).FullName;
            var encodeValue = Encoding.UTF8.GetString(licenseBody);
            var raw = Encryptor.DecryptStringAES(encodeValue, key);
            var licData = Convert.FromBase64String(raw);

            using (var memoryStream = new MemoryStream(licData))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Binder = new TypeBinder();

                var lics = (LicenseData[])formatter.Deserialize(memoryStream);

                if (lics != null && lics.Length > 0)
                {
                    for (int i = 0; i < lics.Length; i++)
                    {
                        var lic = lics[i];
                        if (lic.OwnerId.Equals(ownerId, StringComparison.OrdinalIgnoreCase)
                        && lic.ProductKey.Equals(productKey, StringComparison.OrdinalIgnoreCase)
                        )
                        {
                            return lic;
                        }
                    }
                }
            }

            throw new Exception("Invalid license data");
        }

        private static ushort DecodeYear(string yearHash)
        {
            if (string.IsNullOrEmpty(yearHash))
            {
                throw new ArgumentNullException(nameof(yearHash));
            }

            return Convert.ToUInt16(Encryptor.DecryptStringAES(yearHash, key));
        }

        private static string DefineHostData()
        {
            return $"H:{Environment.MachineName}";
        }

        public static string EncodeYear(ushort year)
        {
            return Encryptor.EncryptStringAES(year.ToString(), key);
        }
    }
}
