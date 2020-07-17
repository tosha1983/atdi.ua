using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.Contracts.AppServices.WebQuery;
using Atdi.CoreServices.Identity.Models;
using Atdi.DataModels.DataConstraint;
using Atdi.CoreServices.Identity;
using Atdi.DataModels;
using Atdi.DataModels.WebQuery;


namespace Atdi.CoreServices.AuthService.IcsmViisp
{
	public class AuthService : IAuthService
	{
        
        private readonly IWebQuery _webQuery;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly AppServerComponentConfig _config;
		private readonly ILogger _logger;
		public static readonly string Name = "E-Government";

		public AuthService(AppServerComponentConfig config, IDataLayer<IcsmDataOrm> dataLayer, IQueryExecutor queryExecutor, IWebQuery webQuery, ILogger logger)
		{
			_config = config;
			_logger = logger;
            _dataLayer = dataLayer;
            _queryExecutor = queryExecutor;
            _webQuery = webQuery;
        }



        public UserIdentity AuthenticateUser(AuthRedirectionResponse response, IUserTokenProvider tokenProvider)
		{
           
            var userIdentity = new UserIdentity();
            var userTokenData = new UserTokenData();

            var dicParameters = ParseQueryString(response.Url);
            if ((dicParameters!=null) && (dicParameters.Count>0))
            {
                if ((dicParameters.ContainsKey("ticket")) && (dicParameters.ContainsKey("customData")))
                {
                    var xmlRequest = SignData.AuthenticationData(this._config.ViispSecretKey, _config.ViispPublicKey, "#uniqueNodeId", this._config.ViispPID, dicParameters["ticket"]);
                    var responseInformationData = SignData.GetResponseAuthenticationData(this._config.ViispServiceUrl, xmlRequest, out string faultstring);
                    if (string.IsNullOrEmpty(faultstring))
                    {
                        if (responseInformationData != null)
                        {
                            if ((responseInformationData.AuthenticationAttribute.Length > 0) && (responseInformationData.AuthenticationAttribute[0].Attribute == "lt-personal-code"))
                            {
                                //string Role = "Provider";



                                var querySelect = this._dataLayer.Builder
                     .From<USERS>()
                     .Where(c => c.REGIST_NUM, ConditionOperator.Equal, responseInformationData.AuthenticationAttribute[0].Value)
                     .Select(
                         c => c.CODE,
                         c => c.CUST_TXT3,
                         c => c.EMAIL,
                         c => c.ID,
                         c => c.NAME,
                         c => c.REGIST_NUM
                         )
                     .OrderByAsc(c => c.ID)
                     .OnTop(1);

                                var userData = this._queryExecutor
                      .Fetch(querySelect, reader =>
                      {
                          if (reader.Read())
                          {
                              userIdentity.Id = reader.GetValue(x => x.ID);
                              userIdentity.Name = reader.GetValue(x => x.NAME);
                              userTokenData.AuthDate = DateTime.Now;
                              userTokenData.Id = reader.GetValue(x => x.ID);
                              userTokenData.UserCode = reader.GetValue(x => x.CODE);
                              userTokenData.UserId = reader.GetValue(x => x.ID);
                              userTokenData.UserName = reader.GetValue(x => x.NAME);
                              userIdentity.UserToken = tokenProvider.CreatUserToken(userTokenData);

                              return true;
                          }
                          else
                          {
                              return false;
                          }

                      });

                                if (userData == false)
                                {

                                    //this._webQuery.SaveChanges(tokenProvider);

                                    /*
                                    var queryInsert = this._dataLayer.Builder
                         .Insert("USERS")
                         .SetValue(new IntegerColumnValue() { Name = "ID", Value = 1 });
                         */
                                };


                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Response Url not contains parameters 'ticket' or 'customData'"); 
                }
            }


         




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
            return userIdentity;
		}

        public static Dictionary<string, string> ParseQueryString(String query)
        {
            var queryDict = new Dictionary<string, string>();
            var idx = query.TrimStart().LastIndexOf('?');
            if (idx > 0)
            {
                query = query.Remove(0, idx + 1);
                foreach (String token in query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        queryDict[parts[0].Trim()] = parts[1].Trim();
                    }
                    else
                    {
                        queryDict[parts[0].Trim()] = "";
                    }
                }
            }
            return queryDict;
        }

        public AuthRedirectionQuery PrepareAuthRedirection(AuthRedirectionOptions options)
		{
            // Тут начал апроцесса, первая фаза до формирвоания урла для ридеректа
            // его формируем ми перадем в свойствах объекта  результата
            var correlationData = Guid.NewGuid();
            var xmlInit = SignData.InitAuthentication(this._config.ViispSecretKey, _config.ViispPublicKey, "#uniqueNodeId", this._config.ViispPID, options.ReturnUrl, correlationData.ToString());
            var ticketId = SignData.GetResponseinitAuthentication(this._config.ViispServiceUrl, xmlInit, out string faultString);
            if (string.IsNullOrEmpty(faultString))
            {
                return new AuthRedirectionQuery
                {
                    AuthService = options.AuthService,
                    Url = this._config.ViispServiceUrl + string.Format("?ticket={0}", ticketId)
                };
            }
            else
            {
                throw new Exception(faultString);
            }
		}
	}
}
