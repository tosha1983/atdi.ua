using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public struct ExceptionText : IEquatable<ExceptionText>
    {
        public string Text { get; private set; }

        public object[] Args { get; private set; }

        public ExceptionText(string text)
        {
            this.Text = text;
            this.Args = null;
        }

        public ExceptionText(string text, object[] args)
        {
            this.Text = text;
            this.Args = args;
        }


        public ExceptionText With(params object[] args)
        {
            if (args != null && args.Length > 0)
                return new ExceptionText(this.Text, args);
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

        public bool Equals(ExceptionText other)
        {
            return this.Text == other.Text;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExceptionText))
            {
                return false;
            }
            return this.Equals((ExceptionText)obj);
        }

        public static bool operator ==(ExceptionText x, ExceptionText y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ExceptionText x, ExceptionText y)
        {
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            if (this.Text == null)
                return 0;
            return this.Text.GetHashCode();
        }

        public static implicit operator ExceptionText(string text)
        {
            return new ExceptionText(text);
        }

        public static implicit operator string(ExceptionText text)
        {
            return text.ToString();
        }
    }
}
