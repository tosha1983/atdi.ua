using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public struct EventContext : IEquatable<EventContext>
    {
        public string Name { get; }

        public EventContext(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Name))
                return string.Empty;
            return this.Name;
        }

        public bool Equals(EventContext other)
        {
            return this.Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EventContext))
            {
                return false;
            }
            return this.Equals((EventContext)obj);
        }

        public static bool operator ==(EventContext x, EventContext y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(EventContext x, EventContext y)
        {
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            if (this.Name == null)
                return 0;
            return this.Name.GetHashCode();
        }

        public static implicit operator EventContext(string name)
        {
            return new EventContext(name);
        }

        //public static implicit operator string(LogEventContext value)
        //{
        //    return value.Name;
        //}
    }
}
