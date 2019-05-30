using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{ 

    [Serializable]
    public sealed class Message
    {
        public string Id;
        public string Type;
        public QueueType QueueType;
        public string SpecificQueue;
        public string From;
        public string To;
        public DateTimeOffset Created;
        public string BusName;
        public string ApiVersion;
        public string Application;
        public bool UseEncryption;
        public bool UseCompression;
        public object Body;
        public ContentType ContentType;
        public string MessageTypeAQName;
        public string DeliveryObjectAQName;

        public override string ToString()
        {
            var size = 0;
            if (Body == null)
            {
                size = 0;
            }
            else
            {
                var bytesBody = this.Body as byte[];
                if (bytesBody != null)
                {
                    size = bytesBody.Length;
                }

                var stringBody = this.Body as string;
                if (stringBody != null)
                {
                    size = stringBody.Length;
                }
            }

            return $"Id = '{Id}', Type = '{Type}', From = '{From}', To = '{To}', BodySize = '{size}'";
        }

        public string BuildContentEncoding()
        {
            var encoding = new Stack<string>();
            if (this.UseCompression)
            {
                encoding.Push("compressed");
            }
            if (this.UseEncryption) 
            {
                encoding.Push("encrypted");
            }
            var contentEncoding = string.Join(", ", encoding.ToArray());
            return contentEncoding;
        }
    }
}
