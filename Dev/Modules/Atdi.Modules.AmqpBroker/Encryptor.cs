using System;
using System.Security.Cryptography;
using System.IO;

namespace Atdi.Modules.AmqpBroker
{
    internal class Encryptor
    {
        private static readonly byte[] Salt = new byte[] { 173, 73, 19, 75, 245, 39, 233, 214, 8, 77, 6, 33, 17, 93, 2, 67, 51 };
        private static readonly string SharedSecret = typeof(Encryptor).AssemblyQualifiedName;

        public static string Encrypt(string source)
        {
            string result; 
            RijndaelManaged aesAlg = null;
            try
            {
                var key = new Rfc2898DeriveBytes(SharedSecret, Salt);
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(source);
                        }
                    }
                    result = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                aesAlg?.Clear();
            }
            return result;
        }

        public static string Decrypt(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }
            RijndaelManaged aesAlg = null;
            string result;
            try
            {
                var key = new Rfc2898DeriveBytes(SharedSecret, Salt);
                var bytes = Convert.FromBase64String(source);
                using (var msDecrypt = new MemoryStream(bytes))
                {
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                aesAlg?.Clear();
            }
            return result;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}

