using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.DataBus
{
    public struct MessageToken
    {
        public MessageToken(string id, string type)
        {
            this.Id = id;
            this.Type = type;
        }

        public MessageToken(string type)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Type = type;
        }

        public string Id { get; }

        public string Type { get; }

        public override string ToString()
        {
            return $"{this.Type}({this.Id})"  ;
        }

        public bool Equals(MessageToken other)
        {
            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MessageToken))
            {
                return false;
            }
            return this.Equals((MessageToken)obj);
        }

        public static bool operator ==(MessageToken x, MessageToken y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(MessageToken x, MessageToken y)
        {
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            if (this.Id == null)
                return 0;
            return this.Id.GetHashCode();
        }
    }
}
