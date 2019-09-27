using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public struct EventCategory : IEquatable<EventCategory>
    {
        public string Name { get; }

        public EventCategory(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Name))
                return string.Empty;
            return this.Name;
        }

        public bool Equals(EventCategory other)
        {
            return this.Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EventCategory))
            {
                return false;
            }
            return this.Equals((EventCategory)obj);
        }

        public static bool operator ==(EventCategory x, EventCategory y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(EventCategory x, EventCategory y)
        {
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            if (this.Name == null)
                return 0;
            return this.Name.GetHashCode();
        }

        public static implicit operator EventCategory(string name)
        {
            return new EventCategory(name);
        }

        //public static implicit operator string(LogEventCategory value)
        //{
        //    return value.Name;
        //}
    }
}
