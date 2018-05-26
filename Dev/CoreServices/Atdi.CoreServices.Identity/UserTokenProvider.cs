using Atdi.Common.Extensions;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.Identity;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
