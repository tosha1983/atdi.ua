using System;
using System.Collections.Generic;
using System.Text;

namespace Atdi.Modules.AmqpBroker
{
    public interface IDeliveryMessage
    {
        string Id { get; set; }

        string AppId { get; set; }

        string Type { get; set; }

        string ContentType { get; set; }

        string ContentEncoding { get; set; }

        string CorrelationId { get; set; }

        IDictionary<string, object> Headers { get; set; } 

        byte[] Body { get; set; }
    }

    public static class DeliveryMessageExtension
    {
        public static string GetHeaderValue(this IDeliveryMessage message, string key)
        {
            if (message?.Headers == null)
            {
                return null;
            }
            var headers = message.Headers;
            if (!headers.ContainsKey(key))
            {
                return null;
            }
            return Convert.ToString(Encoding.UTF8.GetString(((byte[])headers[key])));
        }
    }
}
