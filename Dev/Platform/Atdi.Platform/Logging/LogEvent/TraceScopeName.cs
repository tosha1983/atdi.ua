using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public struct TraceScopeName : IEquatable<TraceScopeName>
    {
        public string Name { get; private set; }

        public object[] Args { get; private set; }

        public TraceScopeName(string name)
        {
            this.Name = name;
            this.Args = null;
        }

        public TraceScopeName(string name, object[] args)
        {
            this.Name = name;
            this.Args = args;
        }


        public TraceScopeName With(params object[] args)
        {
            if (args != null && args.Length > 0)
                return new TraceScopeName(this.Name, args);
            return this;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Name))
                return string.Empty;

            if (this.Args != null && this.Args.Length > 0)
                return string.Format(this.Name, this.Args);

            return this.Name;
        }

        public bool Equals(TraceScopeName other)
        {
            return this.Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TraceScopeName))
            {
                return false;
            }
            return this.Equals((TraceScopeName)obj);
        }

        public static bool operator ==(TraceScopeName x, TraceScopeName y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(TraceScopeName x, TraceScopeName y)
        {
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            if (this.Name == null)
                return 0;
            return this.Name.GetHashCode();
        }

        public static implicit operator TraceScopeName(string name)
        {
            return new TraceScopeName(name);
        }


    }
}
