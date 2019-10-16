using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Atdi.Modules.Sdrn.DeviceBus;

namespace Atdi.Api.Sdrn.Device.BusController
{
    public class MessageToken : IMessageToken
    {
        public MessageToken(string id, string type)
        {
            this.Id = id;
            this.Type = type;
        }

        public MessageToken()
        {

        }

        public string Id { get; set; }

        public string Type { get; set; }

        public static IMessageToken FromBytes(byte[] source)
        {
            if (source == null)
                return null;

            using (var memoryStream = new MemoryStream(source))
            {
                IFormatter formatter = new BinaryFormatter();
                return (IMessageToken)formatter.Deserialize(memoryStream);
            }
        }
        public static byte[] ToBytes(IMessageToken source)
        {
            if (source == null)
                return null;

            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                var binary = stream.ToArray();
                return binary;
            }
        }
    }
}
