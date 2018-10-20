using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.MessageBus
{
    public class MessageConverter
    {
        private readonly MessageConvertSettings _settings;
        private readonly IMessageObjectTypeResolver _typeResolver;

        public MessageConverter(MessageConvertSettings settings, IMessageObjectTypeResolver typeResolver)
        {
            this._settings = settings;
            this._typeResolver = typeResolver;
        }

        public MessageConverter(MessageConvertSettings settings)
        {
            this._settings = settings;
            this._typeResolver = MessageObjectTypeResolver.CreateForApi20(); ;
        }
        public Message Pack<TObject>(string messageType, TObject source)
        {
            var messageObject = new MessageObject
            {
                Type = typeof(TObject),
                Object = source
            };
            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Type = messageType
            };
            this.Serialize(messageObject, message);

            return message;
        }

        public Message Pack(string messageType, Type sourceType, object source)
        {
            var messageObject = new MessageObject
            {
                Type = sourceType,
                Object = source
            };
            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Type = messageType
            };
            this.Serialize(messageObject, message);

            return message;
        }

        public void Serialize(MessageObject source, Message message)
        {
            var messageBody = new MessageBody
            {
                Type = source.Type.AssemblyQualifiedName,
                JsonBody = JsonConvert.SerializeObject(source.Object)
            };

            var json = JsonConvert.SerializeObject(messageBody);

            var encoding = new Stack<string>();
            if (this._settings.UseCompression)
            {
                encoding.Push("compressed");
                json = Compressor.Compress(json);
            }

            if (this._settings.UseEncryption)
            {
                encoding.Push("encrypted");
                json = Encryptor.Encrypt(json);
            }

            message.Body = Encoding.UTF8.GetBytes(json);
            message.ContentType = "application/sdrn";
            message.ContentEncoding = string.Join(", ", encoding.ToArray());
        }

        public MessageObject Deserialize(Message message)
        {
            var result = new MessageObject();

            var json = Encoding.UTF8.GetString(message.Body);

            if ("application/sdrn".Equals(message.ContentType, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(message.ContentEncoding))
                {
                    if (message.ContentEncoding.Contains("encrypted"))
                    {
                        json = Encryptor.Decrypt(json);
                    }
                    if (message.ContentEncoding.Contains("compressed"))
                    {
                        json = Compressor.Decompress(json);
                    }
                }

                var messageBody = JsonConvert.DeserializeObject<MessageBody>(json);
                result.Type = Type.GetType(messageBody.Type);
                json = messageBody.JsonBody;
            }
            else
            {
                result.Type = this._typeResolver.Resolve(message.Type);
            }

            result.Object = JsonConvert.DeserializeObject(json, result.Type);

            return result;
        }

        
    }
}
