using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    public struct EventText : IEquatable<EventText>
    {
        public string Text { get; }

        public object[] Args { get; }

        public EventText(string text)
        {
            this.Text = text;
            this.Args = null;
        }

        public EventText(string text, object[] args)
        {
            this.Text = text;
            this.Args = args;
        }


        public EventText With(params object[] args)
        {
            if (args != null && args.Length > 0)
                return new EventText(this.Text, args);
            return this;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Text))
                return string.Empty;

            if (this.Args != null && this.Args.Length > 0)
                return string.Format(this.Text, this.Args);

            return this.Text;
        }

        public bool Equals(EventText other)
        {
            return this.Text == other.Text;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EventText))
            {
                return false;
            }
            return this.Equals((EventText)obj);
        }

        public static bool operator ==(EventText x, EventText y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(EventText x, EventText y)
        {
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            if (this.Text == null)
                return 0;
            return this.Text.GetHashCode();
        }

        public static implicit operator EventText(string text)
        {
            return new EventText(text);
        }

        
    }
}
