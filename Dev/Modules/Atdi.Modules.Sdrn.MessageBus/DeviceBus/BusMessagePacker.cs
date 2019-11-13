using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using Atdi.Modules.Sdrn.MessageBus;


// ReSharper disable once CheckNamespace
namespace Atdi.Modules.Sdrn.DeviceBus
{
    public interface IContentTypeConvertor
    {
        object Serialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null);

        object Deserialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null);
    }

    internal sealed class SdrnContentTypeConvertor : IContentTypeConvertor
    {
        public object Deserialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null)
        {
            if (source == null)
            {
                return null;
            }

            string json;
            switch (source)
            {
                case string sourceAsString:
                    json = sourceAsString;
                    break;
                case byte[] sourceAsByte:
                    json = Encoding.UTF8.GetString(sourceAsByte);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported source type '{source.GetType()}'");
            }

            if (useEncryption)
            {
                json = Atdi.Modules.Sdrn.MessageBus.Encryptor.Decrypt(json);
            }
            if (useCompression)
            {
                json = Atdi.Modules.Sdrn.MessageBus.Compressor.Decompress(json);
            }

            var messageBody = JsonConvert.DeserializeObject<MessageBody>(json);
            var result = new MessageObject
            {
                Type = Type.GetType(messageBody.Type)
            };

            json = messageBody.JsonBody;

            return JsonConvert.DeserializeObject(json, result.Type, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public object Serialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null)
        {
            var messageObject = new MessageObject
            {
                Type = type,
                Object = source
            };
            var messageBody = new MessageBody
            {
                Type = messageObject.Type.AssemblyQualifiedName,
                JsonBody = JsonConvert.SerializeObject(messageObject.Object)
            };

            var json = JsonConvert.SerializeObject(messageBody);

            if (useCompression)
            {
                json = Atdi.Modules.Sdrn.MessageBus.Compressor.Compress(json);
            }
            if (useEncryption)
            {
                json = Atdi.Modules.Sdrn.MessageBus.Encryptor.Encrypt(json);
            }

            return json;
        }
    }

    internal sealed class JsonContentTypeConvertor : IContentTypeConvertor
    {
        public object Deserialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null)
        {
            if (source == null)
            {
                return null;
            }

            string json;
            switch (source)
            {
                case string sourceAsString:
                    json = sourceAsString;
                    break;
                case byte[] sourceAsByte:
                    json = Encoding.UTF8.GetString(sourceAsByte);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported source type '{source.GetType()}'");
            }

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            if (useEncryption)
            {
                json = Encryptor.Decrypt(json, secretKey);
            }
            if (useCompression)
            {
                json = Compressor.Decompress(json);
            }

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(json, type, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public object Serialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null)
        {
            var json = string.Empty;

            if (source != null)
            {
                json = JsonConvert.SerializeObject(source, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All 
                });
            }
            

            if (useCompression)
            {
                json = Compressor.Compress(json);
            }
            if (useEncryption)
            {
                json = Encryptor.Encrypt(json, secretKey);
            }

            return json;
        }
    }

    internal sealed class XmlContentTypeConvertor : IContentTypeConvertor
    {
        public object Deserialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null)
        {
            if (source == null)
            {
                return null;
            }

            string xml;
            switch (source)
            {
                case string sourceAsString:
                    xml = sourceAsString;
                    break;
                case byte[] sourceAsByte:
                    xml = Encoding.UTF8.GetString(sourceAsByte);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported source type '{source.GetType()}'");
            }

            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            if (useEncryption)
            {
                xml = Encryptor.Decrypt(xml, secretKey);
            }
            if (useCompression)
            {
                xml = Compressor.Decompress(xml);
            }
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            var formatter = new XmlSerializer(type);
            using (var reader = new StringReader(xml))
            {
                return formatter.Deserialize(reader);
            }
        }

        public object Serialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null)
        {
            var xml = string.Empty;

            if(source != null)
            {
                var formatter = new XmlSerializer(source.GetType());
                using (var writer = new StringWriter())
                {
                    formatter.Serialize(writer, source);
                    xml = writer.ToString();
                }
            }

            if (useCompression)
            {
                xml = Compressor.Compress(xml);
            }
            if (useEncryption)
            {
                xml = Encryptor.Encrypt(xml, secretKey);
            }

            return xml;
        }
    }

    internal sealed class BinaryContentTypeConvertor : IContentTypeConvertor
    {
        public object Deserialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null)
        {
            if (source == null)
            {
                return null;
            }

            byte[] binary;
            switch (source)
            {
                case byte[] sourceAsByte:
                    binary = sourceAsByte;
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported source type '{source.GetType()}'");
            }
            
            if (binary.Length == 0)
            {
                return null;
            }

            if (useEncryption)
            {
                binary = Encryptor.Decrypt(binary, secretKey);
            }
            if (useCompression)
            {
                binary = Compressor.Decompress(binary);
            }

            if (binary.Length == 0)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream(binary))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Binder = new LocalBinder();
                return formatter.Deserialize(memoryStream);
            }
        }

        public object Serialize(object source, Type type, bool useCompression = false, bool useEncryption = false, string secretKey = null)
        {
            if (source == null)
            {
                return new byte[] { };
            }


            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                var binary = stream.ToArray();

                if (useCompression)
                {
                    binary = Compressor.Compress(binary);
                }
                if (useEncryption)
                {
                    binary = Encryptor.Encrypt(binary, secretKey);
                }
                return binary;
            }
        }
    }

    public class PackerOptions
    {
        public string ApiVersion;
        public string Protocol;
        public string Application;
        public ContentType ContentType;
        public bool UseCompression;
        public bool UseEncryption;
        public string SharedSecretKey;
    }

    public sealed class BusMessagePacker
    {
        private readonly PackerOptions _options;

        public BusMessagePacker(PackerOptions options)
        {
            this._options = options;
        }

        private static SdrnContentTypeConvertor SdrnContentTypeConvertor { get; } = new SdrnContentTypeConvertor();
        private static JsonContentTypeConvertor JsonContentTypeConvertor { get; } = new JsonContentTypeConvertor();
        private static XmlContentTypeConvertor XmlContentTypeConvertor { get; } = new XmlContentTypeConvertor();
        private static BinaryContentTypeConvertor BinaryContentTypeConvertor { get; } = new BinaryContentTypeConvertor();

        public static IContentTypeConvertor GetConvertor(ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.Json:
                    return JsonContentTypeConvertor;
                case ContentType.Xml:
                    return XmlContentTypeConvertor;
                case ContentType.Binary:
                    return BinaryContentTypeConvertor;
                case ContentType.Sdrn:
                    return SdrnContentTypeConvertor;
                default:
                    throw new InvalidOperationException($"Unsupported content type with name '{contentType}'");
            }
        }

        private static IContentTypeConvertor GetConvertor(string contentType)
        {
            switch (contentType)
            {
                case Protocol.ContentType.QualifiedJson:
                    return JsonContentTypeConvertor;
                case Protocol.ContentType.QualifiedXml:
                    return XmlContentTypeConvertor;
                case Protocol.ContentType.QualifiedBinary:
                    return BinaryContentTypeConvertor;
                case Protocol.ContentType.QualifiedSdrn:
                    return SdrnContentTypeConvertor;
                default:
                    throw new InvalidOperationException($"Unsupported content type with name '{contentType}'");
            }
        }

        public BusMessage Pack<TDeliveryObject>(string type,  TDeliveryObject delivery,  string server, string sensor, string techId, string correlationToken)
        {
            var convertor = GetConvertor(_options.ContentType);

            var message = new BusMessage
            {
                Id = Guid.NewGuid().ToString(),
                CorrelationToken = correlationToken,
                Created = DateTimeOffset.Now,
                ContentType = _options.ContentType,
                ApiVersion = _options.ApiVersion,
                Protocol = _options.Protocol,
                Application = _options.Application,
                UseCompression = _options.UseCompression,
                UseEncryption = _options.UseEncryption,
                Type = type,
                Server = server,
                Sensor = sensor,
                TechId = techId,
                BodyAQName = typeof(TDeliveryObject).AssemblyQualifiedName,
            };

            message.Body = convertor.Serialize(delivery, typeof(TDeliveryObject), message.UseCompression, message.UseEncryption, _options.SharedSecretKey);

            return message;
        }

        public static object Unpack(string contentType, string encoding, string protocol, byte[] body, string aqName, string secretKey)
        {
            var encodingDescriptor = Protocol.ContentEncoding.Decode(encoding);

            if (Protocol.Version_3_0.Equals(protocol, StringComparison.OrdinalIgnoreCase))
            {
                var type = Type.GetType(aqName);
                var convertor = GetConvertor(contentType);
                return convertor.Deserialize(body, type, encodingDescriptor.UseCompression, encodingDescriptor.UseEncryption, secretKey);
            }
            
            if (Protocol.ContentType.QualifiedSdrn.Equals(contentType, StringComparison.OrdinalIgnoreCase))
            {
                return SdrnContentTypeConvertor.Deserialize(body, null, encodingDescriptor.UseCompression, encodingDescriptor.UseEncryption);
            }

            throw new InvalidOperationException($"Unsupported content type '{contentType}' and protocol '{protocol}'");
        }


        
        
    }
}
