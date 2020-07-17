using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;

namespace Atdi.CoreServices.AuthService.IcsmViisp
{
	public class AuthService : IAuthService
	{
		private readonly AppServerComponentConfig _config;
		private readonly ILogger _logger;
		public static readonly string Name = "E-Government";

		public AuthService(AppServerComponentConfig config, ILogger logger)
		{
			_config = config;
			_logger = logger;
		}

		public UserIdentity AuthenticateUser(AuthRedirectionResponse response, IUserTokenProvider tokenProvider)
		{
			// тут код работа с E-Government до момента полученого ISCM User ID
			/*
			 *
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

                var userIdentity = this.CreateUserIdentity(userData);
                return userIdentity;
            }


            throw new InvalidOperationException(Exceptions.InvalidUserPassword.With(credential.UserName));

			 *
			 *
			 */
			// обрабатываем пармтеры ответного запроса в нутри которог отикетпользоватлея
			// по тикету берем даннвй
			// находим юзера в БД
			// возвращаем объект
			return new UserIdentity
			{
				UserToken = tokenProvider.CreatUserToken(
					new UserTokenData
					{
						AuthDate =  DateTime.Now,
						
					})
			};
		}

		public AuthRedirectionQuery PrepareAuthRedirection(AuthRedirectionOptions options)
		{
			// Тут начал апроцесса, первая фаза до формирвоания урла для ридеректа
			// его формируем ми перадем в свойствах объекта  результата
			return new AuthRedirectionQuery
			{
				AuthService = options.AuthService,
				Url = ""
			};
		}
	}
}
