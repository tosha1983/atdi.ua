using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.EventSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SubscriptionEventAttribute : Attribute
    {
        public SubscriptionEventAttribute()
        {
        }

        public SubscriptionEventAttribute(string eventName)
        {
            this.EventName = eventName;
        }

        public string EventName { get; set; }

        public string SubscriberName { get; set; }

    }
}
