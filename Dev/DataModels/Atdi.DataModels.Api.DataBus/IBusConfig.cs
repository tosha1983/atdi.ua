using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.DataBus
{
    public enum BufferType
    {
        None = 0,
        Filesystem = 1,
        Database
    }

    public enum ContentType
    {
        Json = 0,
        Xml = 1,
        Binary = 2
    }

    public interface IBusConfig
    {
        string Address { get; set; }

        string ApiVersion { get; set; }

        string Name { get; set; }

        string Host { get; set; }

        int? Port { get; set; }

        string VirtualHost { get; set; }

        string User { get; set; }

        string  Password { get; set; } 

        bool? UseEncryption { get; set; }

        bool? UseCompression { get; set; }

        IBusBufferConfig Buffer { get; }

        ContentType ContentType { get; set; }
    }

    public interface IBusBufferConfig
    {
        BufferType Type { get; set; }

        string OutboxFolder { get; set; }

        string ConnectionString { get; set; }

        ContentType ContentType { get; set; }
    }
}
