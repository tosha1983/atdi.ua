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
using System.Globalization;



namespace Atdi.CoreServices.Identity
{
    public sealed class AuthenticationManager : LoggedObject, IAuthenticationManager
    {
	    private readonly IAuthServiceSite _authServiceSite;
	    private readonly IExternalServiceProvider _externalServiceProvider;
	    private readonly IDataLayer<IcsmDataOrm> _dataLayer;
	    private readonly IServiceTokenProvider _serviceTokenProvider;
	    private readonly IUserTokenProvider _userTokenProvider;
        private readonly IQueryExecutor _queryExecutor;

        public AuthenticationManager(
			IAuthServiceSite authServiceSite,
	        IExternalServiceProvider externalServiceProvider,
	        IDataLayer<IcsmDataOrm> dataLayer,
	        IServiceTokenProvider serviceTokenProvider,
			IUserTokenProvider userTokenProvider, 
	        ILogger logger) : base(logger)
        {
	        _authServiceSite = authServiceSite;
	        _externalServiceProvider = externalServiceProvider;
	        this._dataLayer = dataLayer;
	        _serviceTokenProvider = serviceTokenProvider;
	        this._userTokenProvider = userTokenProvider;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
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

            var userToken = this._userTokenProvider.CreatUserToken(tokenData);

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
                || userData.Password == _userTokenProvider.GetHashPassword(credential.Password))
            {
                var userIdentity = this.CreateUserIdentity(userData);
                return userIdentity;
            }


            throw new InvalidOperationException(Exceptions.InvalidUserPassword.With(credential.UserName));
        }

		public ServiceIdentity AuthenticateService(ServiceCredential credential)
		{
			if (credential == null)
			{
				throw new ArgumentNullException(nameof(credential));
			}
			if (string.IsNullOrEmpty(credential.ServiceId))
			{
				throw new ArgumentNullException(nameof(credential.ServiceId));
			}
			if (string.IsNullOrEmpty(credential.SecretKey))
			{
				throw new ArgumentNullException(nameof(credential.SecretKey));
			}

			var externalService = _externalServiceProvider.GetServiceById(credential.ServiceId);
			if (externalService == null)
			{
				throw new InvalidOperationException(Exceptions.NotFoundService.With(credential.ServiceId));
			}
			if (credential.SecretKey != externalService.SecretKey)
			{
				throw new InvalidOperationException(Exceptions.InvalidServiceSecretKey.With(credential.ServiceId));
			}

			return new ServiceIdentity
			{
				Id = credential.ServiceId,
				Token = _serviceTokenProvider.EncodeToken(new ServiceTokenData
				{
					Id = credential.ServiceId,
					Name = externalService.Name,
					AuthDate = DateTime.Now
				})
			};
		}

		public AuthRedirectionQuery PrepareAuthRedirection(ServiceToken token, AuthRedirectionOptions options)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}
			if (options == null)
			{
				throw new ArgumentNullException(nameof(options));
			}
			if (string.IsNullOrEmpty(options.AuthService))
			{
				throw new ArgumentNullException(nameof(options.AuthService));
			}

			this.VerifyServiceToken(token);

			var authService = DefineAuthService(options.AuthService);
			return authService.PrepareAuthRedirection(options);
		}

		private IAuthService DefineAuthService(string name)
		{
			var authService = _authServiceSite.GetService(name);
			if (authService == null)
			{
				throw new InvalidOperationException($"Unknown authentication service '{name}'");
			}

			return authService;
		}

		public UserIdentity HandleAndAuthenticateUser(ServiceToken token, AuthRedirectionResponse response)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}
			if (response == null)
			{
				throw new ArgumentNullException(nameof(response));
			}
			if (string.IsNullOrEmpty(response.AuthService))
			{
				throw new ArgumentNullException(nameof(response.AuthService));
			}

			this.VerifyServiceToken(token);

			var authService = DefineAuthService(response.AuthService);
			return authService.AuthenticateUser(response, _userTokenProvider);
		}

		private void VerifyServiceToken(ServiceToken token)
		{
			var serviceData = _serviceTokenProvider.DecodeToken(token);
			var externalService = _externalServiceProvider.GetServiceById(serviceData.Id);
			if (externalService == null)
			{
				throw new InvalidOperationException(Exceptions.InvalidServiceToken);
			}
			if (externalService.Name != serviceData.Name)
			{
				throw new InvalidOperationException(Exceptions.InvalidServiceToken);
			}
		}
	}
}
