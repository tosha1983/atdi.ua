using Atdi.DataModels.Api.DataBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Atdi.Api.DataBus
{
    internal interface IContentTypeConvertor
    {
        object Serialize(object source, Type type, bool useCompression = false, bool useEncryption = false);

        object Deserialize(object source, Type type, bool UseCompression = false, bool useEncryption = false);
    }

    internal sealed class JsonContentTypeConvertor : IContentTypeConvertor
    {
        public object Deserialize(object source, Type type, bool useCompression = false, bool useEncryption = false)
        {
            if (source == null)
            {
                return null;
            }

            var json = (string)source;
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            if (useEncryption)
            {
                json = Encryptor.Decrypt(json);
            }
            if (useCompression)
            {
                json = Compressor.Decompress(json);
            }

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(json, type);
        }

        public object Serialize(object source, Type type, bool useCompression = false, bool useEncryption = false)
        {
            var json = string.Empty;

            if (source != null)
            {
                json = JsonConvert.SerializeObject(source);
            }
            

            if (useCompression)
            {
                json = Compressor.Compress(json);
            }
            if (useEncryption)
            {
                json = Encryptor.Encrypt(json);
            }

            return json;
        }
    }

    internal sealed class XmlContentTypeConvertor : IContentTypeConvertor
    {
        public object Deserialize(object source, Type type, bool useCompression = false, bool useEncryption = false)
        {
            if (source == null)
            {
                return null;
            }

            var xml = (string)source;
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            if (useEncryption)
            {
                xml = Encryptor.Decrypt(xml);
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

        public object Serialize(object source, Type type, bool useCompression = false, bool useEncryption = false)
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
                xml = Encryptor.Encrypt(xml);
            }

            return xml;
        }
    }

    internal sealed class BinaryContentTypeConvertor : IContentTypeConvertor
    {
        public object Deserialize(object source, Type type, bool useCompression = false, bool useEncryption = false)
        {
            if (source == null)
            {
                return null;
            }

            var binary = (byte[])source;
            if (binary.Length == 0)
            {
                return null;
            }

            if (useEncryption)
            {
                binary = Encryptor.Decrypt(binary);
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

        public object Serialize(object source, Type type, bool useCompression = false, bool useEncryption = false)
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
                    binary = Encryptor.Encrypt(binary);
                }
                return binary;
            }
        }
    }

    internal sealed class MessagePacker
    {
        private readonly IBusConfig _config;

        public MessagePacker(IBusConfig config)
        {
            this._config = config;
        }

        private JsonContentTypeConvertor JsonContentTypeConvertor { get; } = new JsonContentTypeConvertor();
        private XmlContentTypeConvertor XmlContentTypeConvertor { get; } = new XmlContentTypeConvertor();
        private BinaryContentTypeConvertor BinaryContentTypeConvertor { get; } = new BinaryContentTypeConvertor();

        public IContentTypeConvertor GetConvertor(ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.Json:
                    return this.JsonContentTypeConvertor;
                case ContentType.Xml:
                    return this.XmlContentTypeConvertor;
                case ContentType.Binary:
                    return this.BinaryContentTypeConvertor;
                default:
                    throw new InvalidOperationException($"Unsupported content type with name '{contentType}'");
            }
        }

        public Message Pack<TMessageType, TDeliveryObject>(IOutgoingEnvelope<TMessageType, TDeliveryObject> envelope)
            where TMessageType : IMessageType, new()
        {
            var convertor = this.GetConvertor(envelope.ContentType);

            var message = new Message
            {
                ContentType = envelope.ContentType,
                Id = envelope.Token.Id,
                Type = envelope.Token.Type,
                ApiVersion = _config.ApiVersion,
                BusName = _config.Name,
                UseCompression = (envelope.Options & SendingOptions.UseCompression) == SendingOptions.UseCompression,
                UseEncryption = (envelope.Options & SendingOptions.UseEncryption) == SendingOptions.UseEncryption,
                Created = envelope.Created,
                From = _config.Address,
                To = envelope.To,
                Application = this.GetType().Assembly.FullName,
                QueueType = envelope.Type.QueueType,
                SpecificQueue = envelope.Type.SpecificQueue,
                MessageTypeAQName = typeof(TMessageType).AssemblyQualifiedName,
                DeliveryObjectAQName = typeof(TDeliveryObject).AssemblyQualifiedName,
                
            };


            message.Body = convertor.Serialize(envelope.DeliveryObject, typeof(TDeliveryObject), message.UseCompression, message.UseEncryption);

            return message;
        }

        


        
        
    }
}
