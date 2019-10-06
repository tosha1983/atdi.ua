using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Sdrn.DeviceBus
{
    public enum BufferType
    {
        None = 0,
        Filesystem = 1,
        Database
    }

    public enum ContentType
    {
        Sdrn = 0,
        Json = 1,
        Xml = 2,
        Binary = 3
    }
    [Serializable]
    public sealed class BusMessage
    {
        public string Id;

        public string Type;

        public string Server;

        public string Sensor;

        public string TechId;

        public DateTimeOffset Created;

        public string ApiVersion;

        public string Protocol;

        public string Application;

        public bool UseEncryption;

        public bool UseCompression;

        public object Body;

        public ContentType ContentType;

        public string BodyAQName;

        public string CorrelationToken;

        public override string ToString()
        {
            var size = 0;
            if (Body == null)
            {
                size = 0;
            }
            else
            {
                if (this.Body is byte[] bytesBody)
                {
                    size = bytesBody.Length;
                }
                else if (this.Body is string stringBody)
                {
                    size = stringBody.Length;
                }
            }

            return $"Type='{Type}', Server='{Server}', Sensor='{Sensor}', BodySize=#{size}, Id='{Id}'";
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

        public byte[] AsBytes()
        {
            switch (this.ContentType)
            {
                case ContentType.Json:
                    return Encoding.UTF8.GetBytes((string)this.Body);
                case ContentType.Xml:
                    return Encoding.UTF8.GetBytes((string)this.Body);
                case ContentType.Binary:
                    return (byte[])this.Body;
                case ContentType.Sdrn:
                    return Encoding.UTF8.GetBytes((string)this.Body);
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported content type '{ContentType}'");
            }
        }
    }
}
