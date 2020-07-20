using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Licensing
{
    public class LicenseVerifier
    {
        private static readonly string key = Assembly.GetAssembly(typeof(LicenseData)).FullName;

        class DeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {

                var resultType = Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));

                return resultType;
            }
        }

        //public static VerificationResult Verify(VerificationData data, byte[] licenseBody)
        //{
        //    //formatter.Serialize(stream, licenses);
        //    //var raw = Convert.ToBase64String(stream.ToArray());
        //    //var key = Assembly.GetAssembly(typeof(LicenseData)).FullName;
        //    //var encodeVal = Encryptor.EncryptStringAES(raw, key);

        //    //return new LicenseCrationResult
        //    //{
        //    //    Body = Encoding.UTF8.GetBytes(encodeVal)
        //    //};

        //    var key = Assembly.GetAssembly(typeof(LicenseData)).FullName;
        //    var encodeValue = Encoding.UTF8.GetString(licenseBody);
        //    var raw = Encryptor.DecryptStringAES(encodeValue, key);
        //    var licData = Convert.FromBase64String(raw);
            
        //    using (var memoryStream = new MemoryStream(licData))
        //    {
        //        IFormatter formatter = new BinaryFormatter();
        //        formatter.Binder = new DeserializationBinder();
        //        var lics = (LicenseData[])formatter.Deserialize(memoryStream);

        //        if (lics != null && lics.Length > 0)
        //        {
        //            for (int i = 0; i < lics.Length; i++)
        //            {
        //                var lic = lics[i];
        //                if (lic.OwnerId.Equals(data.OwnerId, StringComparison.OrdinalIgnoreCase)
        //                && lic.LicenseType.Equals(data.LicenseType, StringComparison.OrdinalIgnoreCase)
        //                && lic.ProductName.Equals(data.ProductName, StringComparison.OrdinalIgnoreCase)
        //                && lic.ProductKey.Equals(data.ProductKey, StringComparison.OrdinalIgnoreCase)
        //                )
        //                {
        //                    if (lic.StartDate > data.Date)
        //                    {
        //                        throw new Exception("The time to start using the license has not yet come");
        //                    }
        //                    if (lic.StopDate <= data.Date)
        //                    {
        //                        throw new Exception("License was expired");
        //                    }

        //                    return new VerificationResult
        //                    {
        //                        LicenseNumber = lic.LicenseNumber,
        //                        Count = lic.Count,
        //                        Instance = lic.Instance,
        //                        OwnerName = lic.OwnerName,
        //                        StopDate = lic.StopDate,
        //                        StartDate = lic.StartDate
        //                    };
        //                }
        //            }
        //        }
        //    }

        //    throw new Exception("Invalid license data");            
        //}


        public static VerificationResult Verify(VerificationData data, byte[] licenseBody)
        {
            
            var encodeValue = Encoding.UTF8.GetString(licenseBody);
            var raw = Encryptor.DecryptStringAES(encodeValue, key);
            var licData = Convert.FromBase64String(raw);

            using (var memoryStream = new MemoryStream(licData))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Binder = new DeserializationBinder();

                if (formatter.Deserialize(memoryStream) is LicenseData[] lics && lics.Length > 0)
                {
                    foreach (var lic in lics)
                    {
                        
                        
                        if (lic.OwnerId.Equals(data.OwnerId, StringComparison.OrdinalIgnoreCase)
                            && lic.LicenseType.Equals(data.LicenseType, StringComparison.OrdinalIgnoreCase)
                            && lic.ProductName.Equals(data.ProductName, StringComparison.OrdinalIgnoreCase)
                            && lic.ProductKey.Equals(data.ProductKey, StringComparison.OrdinalIgnoreCase)
                        )
                        {
	                        ExternalServiceDescriptor[] externalServices = null;

                            // Is it the second protocol?
                            if (lic is LicenseData2 license2)
                            {
                                if (!(data is VerificationData2 data2))
                                {
                                    throw new Exception("Invalid verification data.");
                                }

                                if ((license2.LimitationTerms & LicenseLimitationTerms.TimePeriod) != 0)
                                {
                                    if(license2.StartDate > data2.Date)
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

                                if ((license2.LimitationTerms & LicenseLimitationTerms.HardwareBinding) != 0)
                                {
	                                VerifeHardwareBinding(license2 as LicenseData3);
                                }

                                if (lic is LicenseData4 license4)
                                {
	                                externalServices = license4.ExternalServices;
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
                                StartDate = lic.StartDate,
								ExternalServices = externalServices
                            };
                        }
                    }
                }
            }

            throw new Exception("Invalid license data");
        }

        private static void CompareHardwareValue(string value1, string value2)
        {
	        if (!string.IsNullOrEmpty(value1) &&
	            value1.Equals(value2, StringComparison.OrdinalIgnoreCase))
	        {
				return;
	        }
	        if (!string.IsNullOrEmpty(value2) &&
	            value2.Equals(value1, StringComparison.OrdinalIgnoreCase))
	        {
		        return;
	        }

	        if (string.IsNullOrEmpty(value1) && string.IsNullOrEmpty(value2))
	        {
				return;
	        }

			throw new Exception("Hardware data does not match license data");
		}
        private static void VerifeHardwareBinding(LicenseData3 data3)
        {
	        if (data3 == null)
	        {
		        throw new Exception("Invalid license data");
			}

	        var descriptor = DefineHardwareDescriptor();

	        if ((data3.HardwareBinding & LicenseHardwareBinding.HostUUID) != 0)
	        {
		        CompareHardwareValue(descriptor.HostUuid, data3.HardwareDescriptor.HostUuid);
	        }

			if ((data3.HardwareBinding & LicenseHardwareBinding.Cpu) != 0)
			{
				CompareHardwareValue(descriptor.Cpu, data3.HardwareDescriptor.Cpu);
			}

			if ((data3.HardwareBinding & LicenseHardwareBinding.HostName) != 0)
			{
				CompareHardwareValue(descriptor.HostName, data3.HardwareDescriptor.HostName);
			}

			if ((data3.HardwareBinding & LicenseHardwareBinding.OperatingSystem) != 0)
			{
				CompareHardwareValue(descriptor.OperatingSystem, data3.HardwareDescriptor.OperatingSystem);
			}

			if ((data3.HardwareBinding & LicenseHardwareBinding.Motherboard) != 0)
			{
				CompareHardwareValue(descriptor.Motherboard, data3.HardwareDescriptor.Motherboard);
			}

			if ((data3.HardwareBinding & LicenseHardwareBinding.SysDrive) != 0)
			{
				CompareHardwareValue(descriptor.SysDrive, data3.HardwareDescriptor.SysDrive);
			}

			if ((data3.HardwareBinding & LicenseHardwareBinding.NetworkInterfaces) != 0)
			{
				if (data3.HardwareDescriptor.NetworkInterfaces == null
				    || descriptor.NetworkInterfaces != null)
				{
					throw new Exception("Hardware data does not match license data");
				}
				if (data3.HardwareDescriptor.NetworkInterfaces != null
				    || descriptor.NetworkInterfaces == null)
				{
					throw new Exception("Hardware data does not match license data");
				}
				if (data3.HardwareDescriptor.NetworkInterfaces != null
				    && descriptor.NetworkInterfaces != null)
				{
					if (data3.HardwareDescriptor.NetworkInterfaces.Length != descriptor.NetworkInterfaces.Length)
					{
						throw new Exception("Hardware data does not match license data");
					}

					var a1 = data3.HardwareDescriptor.NetworkInterfaces.OrderBy(c => c.Id).ToArray();
					var a2 = descriptor.NetworkInterfaces.OrderBy(c => c.Id).ToArray();
					for (int i = 0; i < a1.Length; i++)
					{
						CompareHardwareValue(a1[i].Id, a2[i].Id);
						CompareHardwareValue(a1[i].Name, a2[i].Name);
						CompareHardwareValue(a1[i].Address, a2[i].Address);
						CompareHardwareValue(a1[i].InterfaceType, a2[i].InterfaceType);
					}
				}
			}
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
                formatter.Binder = new DeserializationBinder();
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

        private static HostHardwareDescriptor DefineHardwareDescriptor()
        {
			var descriptor = new HostHardwareDescriptor();

			ManagementObjectSearcher searcher;

			//процессор
			searcher = new ManagementObjectSearcher("root\\CIMV2",
				"SELECT * FROM Win32_Processor");
			foreach (var queryObj in searcher.Get())
			{
				descriptor.Cpu = queryObj["ProcessorId"].ToString();
			}

			//мать
			searcher = new ManagementObjectSearcher("root\\CIMV2",
				"SELECT * FROM CIM_Card");
			foreach (var queryObj in searcher.Get())
			{
				descriptor.Motherboard = queryObj["SerialNumber"].ToString();
			}

			//ОС
			searcher = new ManagementObjectSearcher("root\\CIMV2",
				"SELECT * FROM CIM_OperatingSystem");
			foreach (var queryObj in searcher.Get())
			{
				descriptor.OperatingSystem = queryObj["SerialNumber"].ToString();
			}

			// Идентифкатор хоста
			searcher = new ManagementObjectSearcher("root\\CIMV2",
				"SELECT UUID FROM Win32_ComputerSystemProduct");
			foreach (var o in searcher.Get())
			{
				var queryObj = (ManagementObject)o;
				descriptor.HostUuid = queryObj["UUID"].ToString();
			}

			// имя хоста
			descriptor.HostName = Environment.MachineName;

			// диск
			var systemDrive = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 1);
			var diskObject = new ManagementObject("win32_logicaldisk.deviceid=\"" + systemDrive + ":\"");
			diskObject.Get();
			descriptor.SysDrive = diskObject["VolumeSerialNumber"].ToString();

			// сетевые интерфнйсы
			var ni = new List<HostNetworkInterface>();
			var nics = NetworkInterface.GetAllNetworkInterfaces();
			foreach (var adapter in nics)
			{
				ni.Add(new HostNetworkInterface()
				{
					Id = adapter.Id,
					Name = adapter.Name,
					InterfaceType = adapter.NetworkInterfaceType.ToString(),
					Address = adapter.GetPhysicalAddress().ToString()
				});
			}
			descriptor.NetworkInterfaces = ni.ToArray();

			return descriptor;
        }

		public static string GetHostKey()
		{
			var descriptor = DefineHardwareDescriptor();
			var result = Convert.ToBase64String(Serialize(descriptor));
			return result;

        }

        private static byte[] Serialize<T>(T value)
        {
	        if (value == null) return new byte[] { };

	        var binaryFormatter = new BinaryFormatter();
	        using (var stream = new MemoryStream())
	        {
		        binaryFormatter.Serialize(stream, value);
		        stream.Position = 0;
		        return stream.ToArray();
	        }
        }
	}
}
