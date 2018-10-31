using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.DataModels.Identity;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.CoreServices.Identity.Models;
using System.Security.Cryptography;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.CoreServices.Netkey;
using System.Globalization;



namespace Atdi.CoreServices.Identity
{
    public sealed class AuthenticationManager : LoggedObject, IAuthenticationManager
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly IUserTokenProvider _tokenProvider;
        private readonly IQueryExecutor _queryExecutor;
        private readonly INetKeyValidator _netKey;

        public AuthenticationManager(IDataLayer<IcsmDataOrm> dataLayer, IUserTokenProvider tokenProvider, INetKeyValidator netKey, ILogger logger) : base(logger)
        {
            this._dataLayer = dataLayer;
            this._tokenProvider = tokenProvider;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._netKey = netKey;
        }

        private UserIdentity CreateUserIdentity(IcsmUser user)
        {
            var tokenData = new UserTokenData
            {
                AuthDate = DateTime.Now,
                Id = user.Id,
                UserName = user.WebLogin,
                UserCode = user.AppUser,
                UserId = user.UserId
            };

            var userToken = this._tokenProvider.CreatUserToken(tokenData);

            return new UserIdentity
            {
                Id = user.Id,
                Name = user.WebLogin,
                UserToken = userToken
            };
        }


        public UserIdentity AuthenticateUser(UserCredential credential)
        {
            //string dateFormat = DateTime.Now.ToString("MMM dd yyyy", CultureInfo.CreateSpecificCulture("en-US"));

            //int val = _netKey.GetTokenValue("ICS Manager", dateFormat);
            //if (val <= 0)
            //{
            //    throw new InvalidOperationException(Exceptions.InvalidKey);
            //}


            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            if (string.IsNullOrEmpty(credential.UserName))
            {
                throw new ArgumentNullException(nameof(credential.UserName));
            }

            var query = this._dataLayer.Builder
                .From<EMPLOYEE>()
                .Where(c => c.WEB_LOGIN, ConditionOperator.Equal, credential.UserName)
                .Select(
                    c => c.ID,
                    c => c.APP_USER,
                    c => c.WEB_LOGIN,
                    c => c.PWD,
                    c => c.USER_ID)
                .OrderByAsc(c => c.ID)
                .OnTop(1);


            var userData = this._queryExecutor
                .Fetch(query, reader =>
                {
                    if (reader.Read())
                    {
                        return new IcsmUser
                        {
                            Id = reader.GetValue(c => c.ID),
                            WebLogin = reader.GetValue(c => c.WEB_LOGIN),
                            Password = reader.GetValue(c => c.PWD),
                            AppUser = reader.GetValue(c => c.APP_USER),
                            UserId = reader.GetValue(c => c.USER_ID)
                        };
                    }
                    return default(IcsmUser);
                });

            if (userData == null)
            {
                throw new InvalidOperationException(Exceptions.NotFoundUser.With(credential.UserName));
            }



            if ((string.IsNullOrEmpty(userData.Password) && string.IsNullOrEmpty(credential.Password))
                || userData.Password == _tokenProvider.GetHashPassword(credential.Password))
            {
                var userIdentity = this.CreateUserIdentity(userData);
                return userIdentity;
            }


            throw new InvalidOperationException(Exceptions.InvalidUserPassword.With(credential.UserName));
        }
    }
}
