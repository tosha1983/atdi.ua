using Atdi.Common.Extensions;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.Identity;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.Identity
{
    public sealed class UserTokenProvider : LoggedObject,  IUserTokenProvider
    {
        public UserTokenProvider(ILogger logger) : base(logger)
        {
        }

        public UserToken CreatUserToken(UserTokenData tokenData)
        {
            var token = new UserToken
            {
                Data = tokenData.Serialize()
            };

            return token;
        }

        public string GetHashPassword(string password)
        {
            var passwordAsBytes = UTF8Encoding.UTF8.GetBytes(password);
            var shaProvider = new SHA256CryptoServiceProvider();
            var hash = shaProvider.ComputeHash(passwordAsBytes);

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public UserTokenData UnpackUserToken(UserToken userToken)
        {
            if (userToken == null)
            {
                throw new ArgumentNullException(nameof(userToken));
            }

            if (userToken.Data == null)
            {
                throw new ArgumentNullException(nameof(userToken.Data));
            }

            return userToken.Data.Deserialize<UserTokenData>();
        }
    }
}
