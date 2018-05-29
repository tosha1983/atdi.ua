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

namespace Atdi.CoreServices.Identity
{
    public sealed class AuthenticationManager : LoggedObject, IAuthenticationManager
    {
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly IUserTokenProvider _tokenProvider;
        private readonly IQueryExecutor _queryExecutor;

        public AuthenticationManager(IDataLayer<IcsmDataOrm> dataLayer, IUserTokenProvider tokenProvider, ILogger logger) : base(logger)
        {
            this._dataLayer = dataLayer;
            this._tokenProvider = tokenProvider;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
        }

        private UserIdentity CreateUserIdentity(IcsmUser user)
        {
            var tokenData = new UserTokenData
            {
                AuthDate = DateTime.Now,
                Id = user.Id,
                UserName = user.WebLogin,
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
            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            if (string.IsNullOrEmpty(credential.UserName))
            {
                throw new ArgumentNullException(nameof(credential.UserName));
            }

            
            var query = this._dataLayer.Builder
                .From("EMPLOYEE")
                .Where("WEB_LOGIN", credential.UserName)
                //.Where("POSTCODE", "ASD")
                //.Where("LANG", "eu")
                //.Where("City.Province.Names.LEGEN", "s")
                //.Where(new DataModels.DataConstraint.ConditionExpression
                //{
                //    LeftOperand = new DataModels.DataConstraint.ColumnOperand { ColumnName = "REMARK1" },
                //    Operator = DataModels.DataConstraint.ConditionOperator.Like,
                //    RightOperand = new DataModels.DataConstraint.StringValueOperand { Value = "Some Text" }
                //})
                .Select("ID", "WEB_LOGIN", "PWD", "City.NAME")
                .OrderByDesc("APP_USER")
                .OrderByAsc("ID")
                .OrderByDesc("TEL_HOME", "OFFICE", "City.Province.Names.LEGEN")
                .OnTop(1);

            var userData = this._queryExecutor
                .Fetch(query, reader =>
                {
                    if (reader.Read())
                    {
                        return new IcsmUser
                        {
                            Id = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("ID"))),
                            WebLogin = reader.GetString(reader.GetOrdinal("WEB_LOGIN")),
                            Password = reader.GetString(reader.GetOrdinal("PWD"))
                        };
                    }
                    return default(IcsmUser);
                });

            if (userData == null)
            {
                throw new InvalidOperationException(Exceptions.NotFoundUser.With(credential.UserName));
            }
            if ((string.IsNullOrEmpty(userData.Password) && string.IsNullOrEmpty(credential.Password) ) 
                || userData.Password == credential.Password)
            {
                var userIdentity = this.CreateUserIdentity(userData);
                return userIdentity;
            }

            throw new InvalidOperationException(Exceptions.InvalidUserPassword.With(credential.UserName));            
        }
    }
}
