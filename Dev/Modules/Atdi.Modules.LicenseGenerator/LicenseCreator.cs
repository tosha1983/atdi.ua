using Atdi.Modules.Licensing;
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

namespace Atdi.Modules.LicenseGenerator
{
    public class LicenseCreator
    {
        public LicenseCrationResult Create(LicenseData[] licenses)
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, licenses);
                var raw = Convert.ToBase64String(stream.ToArray());
                var key = Assembly.GetAssembly(typeof(LicenseData)).FullName;
                var encodeVal = Encryptor.EncryptStringAES(raw, key);

                return new LicenseCrationResult
                {
                    Body = Encoding.UTF8.GetBytes(encodeVal)
                };
            }

            //using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            //{
            //    aes.GenerateIV();
            //    byte[] iv = aes.IV;
            //    ivAsBase64 = Convert.ToBase64String(iv);

            //    var key = Assembly.GetAssembly(typeof(LicenseData)).FullName;
            //    keySize = aes.Key.Length;
            //    var keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, aes.Key.Length));
            //    aes.Key = keyBytes;

            //    keyAsBase64 = Convert.ToBase64String(aes.Key);

            //    var cryptor = aes.CreateEncryptor();
            //    result = cryptor.TransformFinalBlock(result, 0, result.Length);
            //}

            //return new LicenseCrationResult
            //{
            //    Body = result
            //};

        }
    }
}
