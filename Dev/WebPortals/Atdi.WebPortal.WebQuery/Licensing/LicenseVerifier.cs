﻿using System;
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
        private class TypeBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (typeName == "Atdi.Modules.Licensing.LicenseData")
                {
                    return typeof(LicenseData);
                }

                var type = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
                return type;
            }
        }
        static public VerificationResult Verify(VerificationData data, byte[] licenseBody)
        {
            //formatter.Serialize(stream, licenses);
            //var raw = Convert.ToBase64String(stream.ToArray());
            //var key = Assembly.GetAssembly(typeof(LicenseData)).FullName;
            //var encodeVal = Encryptor.EncryptStringAES(raw, key);

            //return new LicenseCrationResult
            //{
            //    Body = Encoding.UTF8.GetBytes(encodeVal)
            //};

            var key = "Atdi.Modules.Licensing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=79d55ebd8cf97c51"; // Assembly.GetAssembly(typeof(LicenseData)).FullName;
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
                            if (lic.StartDate > data.Date)
                            {
                                throw new Exception("The time to start using the license has not yet come");
                            }
                            if (lic.StopDate <= data.Date)
                            {
                                throw new Exception("License was expired");
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


        static public LicenseData GetLicenseInfo(string ownerId, string productKey, byte[] licenseBody)
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
    }
}