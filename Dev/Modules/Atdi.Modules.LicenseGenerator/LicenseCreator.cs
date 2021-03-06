﻿using Atdi.Modules.Licensing;
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
        }

        public LicenseCrationResult Create(LicenseData2[] licenses)
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, licenses);
                var raw = Convert.ToBase64String(stream.ToArray());
                var key = Assembly.GetAssembly(typeof(LicenseData2)).FullName;
                var encodeVal = Encryptor.EncryptStringAES(raw, key);

                return new LicenseCrationResult
                {
                    Body = Encoding.UTF8.GetBytes(encodeVal)
                };
            }
        }

        public LicenseCrationResult Create(LicenseData3[] licenses)
        {
	        IFormatter formatter = new BinaryFormatter();
	        using (MemoryStream stream = new MemoryStream())
	        {
		        formatter.Serialize(stream, licenses);
		        var raw = Convert.ToBase64String(stream.ToArray());
		        var key = Assembly.GetAssembly(typeof(LicenseData3)).FullName;
		        var encodeVal = Encryptor.EncryptStringAES(raw, key);

		        return new LicenseCrationResult
		        {
			        Body = Encoding.UTF8.GetBytes(encodeVal)
		        };
	        }
        }

        public LicenseCrationResult Create(LicenseData4[] licenses)
        {
	        IFormatter formatter = new BinaryFormatter();
	        using (MemoryStream stream = new MemoryStream())
	        {
		        formatter.Serialize(stream, licenses);
		        var raw = Convert.ToBase64String(stream.ToArray());
		        var key = Assembly.GetAssembly(typeof(LicenseData4)).FullName;
		        var encodeVal = Encryptor.EncryptStringAES(raw, key);

		        return new LicenseCrationResult
		        {
			        Body = Encoding.UTF8.GetBytes(encodeVal)
		        };
	        }
        }
	}
}
