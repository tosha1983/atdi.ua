using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.DeviceBus
{
    public static class Protocol
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once ConvertToConstant.Global
        public static readonly string Version_3_0 = "DeviceBus 3.0";

        public static class Header
        {
            // ReSharper disable ConvertToConstant.Global
            public static readonly string ApiVersion = "ApiVersion";
            public static readonly string Protocol = "Protocol";
            public static readonly string Created = "Created";
            public static readonly string SdrnServer = "SdrnServer";
            public static readonly string SensorName = "SensorName";
            public static readonly string SensorTechId = "SensorTechId";
            public static readonly string BodyAqName = "BodyAQName";
        }

        public static class ContentType
        {
            public const string Application = "application";

            public const string Original = "original";
            public const string Binary = "binary";
            public const string Json = "json";
            public const string Xml = "xml";
            public const string Sdrn = "sdrn";

            public const string QualifiedOriginal = Application + "/" + Original;
            public const string QualifiedBinary = Application + "/" + Binary;
            public const string QualifiedJson = Application + "/" + Json;
            public const string QualifiedXml = Application + "/" + Xml;
            public const string QualifiedSdrn = Application + "/" + Sdrn;

            public static string QualifiedValue(string type)
            {
                return Application + "/" + type;
            }

            public static bool Check(string contentType)
            {
                if (QualifiedBinary.Equals(contentType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (QualifiedJson.Equals(contentType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (QualifiedSdrn.Equals(contentType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (QualifiedXml.Equals(contentType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                if (QualifiedOriginal.Equals(contentType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                return false;
            }
        }

        public static class ContentEncoding
        {
            public struct Descriptor
            {
                public bool UseEncryption;
                public bool UseCompression;
            }

            public static readonly string Encrypted = "encrypted";
            public static readonly string Compressed = "compressed";

            public static Descriptor Decode(string encoding)
            {
                if (string.IsNullOrEmpty(encoding))
                {
                    return new Descriptor();
                }

                return new Descriptor
                {
                    UseCompression = encoding.Contains(Compressed),
                    UseEncryption = encoding.Contains(Encrypted)
                };
            }

        }
    }
}
