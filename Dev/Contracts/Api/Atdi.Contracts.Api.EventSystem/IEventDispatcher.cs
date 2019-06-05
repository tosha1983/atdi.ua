using Atdi.DataModels.Api.EventSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Api.EventSystem
{
    public interface IEventDispatcher : IDisposable
    {
        void Subscribe(string eventName, Type type, string subscriberName = null);

        void Unsubscribe(string eventName, Type type, string subscriberName = null);
    }

    public static class EventDispatcherExtantion
    {
        public static void Subscribe(this IEventDispatcher dispatcher, Type type, string subscriberName = null)
        {
            var eventNames = type.GetAttributesValues((SubscriptionEventAttribute a) => new { a.EventName, a.SubscriberName });
            if (eventNames != null && eventNames.Length > 0)
            {
                for (int i = 0; i < eventNames.Length; i++)
                {
                    dispatcher.Subscribe(eventNames[i].EventName, type, eventNames[i].SubscriberName);
                }
            }
            else
            {
                // register by class name
                dispatcher.Subscribe(type.Name, type);
            }
        }
    }

    public static class AttributeExtensions
    {
        public static TAttribute[] GetAttributesByType<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return type.GetCustomAttributes(
                typeof(TAttribute), true
            ).ToArray() as TAttribute[];
        }
        
        public static TValue[] GetAttributesValues<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            return (type.GetCustomAttributes(
                typeof(TAttribute), true
            ) as TAttribute[]).Select(att => valueSelector(att)).ToArray();

        }
        
    }
}
