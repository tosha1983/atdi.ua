using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.WebQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery
{
    public sealed class UserGroupDescriptors
    {
        private readonly Dictionary<int, QueryTokenDescriptor> _tokensById;
        private readonly Dictionary<string, QueryTokenDescriptor> _tokensByCode;

        public UserGroupDescriptors(UserTokenData token, GroupDescriptor[] descriptors)
        {
            this._tokensByCode = new Dictionary<string, QueryTokenDescriptor>();
            this._tokensById = new Dictionary<int, QueryTokenDescriptor>();
            this.Token = token;
            this.Descriptors = descriptors;

            for (int i = 0; i < descriptors.Length; i++)
            {
                var descriptor = descriptors[i];
                var queryTokens = descriptor.QueryTokens;
                for (int j = 0; j < queryTokens.Length; j++)
                {
                    var queryToken = queryTokens[j];
                    this._tokensById.Add(queryToken.Token.Id, queryToken);

                    if (!string.IsNullOrEmpty(queryToken.Code))
                    {
                        this._tokensByCode.Add(queryToken.Code, queryToken);
                    }
                }
            }
        }
        public UserTokenData Token { get; private set; }

        public GroupDescriptor[] Descriptors { get; private set; }

        public bool HasQuery(QueryToken queryToken, out QueryTokenDescriptor queryTokenDescriptor)
        {
            if (_tokensById.TryGetValue(queryToken.Id, out queryTokenDescriptor))
            {
                return true;
            }
            queryTokenDescriptor = null;
            return false;
        }

        public bool HasQuery(string code, out QueryTokenDescriptor queryTokenDescriptor)
        {
            if (_tokensByCode.TryGetValue(code, out queryTokenDescriptor))
            {
                return true;
            }
            queryTokenDescriptor = null;
            return false;
        }
    }
}
