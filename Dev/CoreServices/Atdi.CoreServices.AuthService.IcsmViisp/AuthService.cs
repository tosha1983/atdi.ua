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
using Atdi.CoreServices.Identity.Models;
using Atdi.DataModels.DataConstraint;
using Atdi.CoreServices.Identity;
using Atdi.DataModels;
using Atdi.DataModels.WebQuery;


namespace Atdi.CoreServices.AuthService.IcsmViisp
{
	public class AuthService : IAuthService
	{
        
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly AppServerComponentConfig _config;
		private readonly ILogger _logger;
		public static readonly string Name = "E-Government";

		public AuthService(AppServerComponentConfig config, IDataLayer<IcsmDataOrm> dataLayer,  ILogger logger)
		{
			_config = config;
			_logger = logger;
            _dataLayer = dataLayer;
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
                    var responseInformationData = SignData.GetResponseAuthenticationData(this._config.ViispRequestUrl, xmlRequest, out string faultstring);
                    if (string.IsNullOrEmpty(faultstring))
                    {
                        if (responseInformationData != null)
                        {
                            if (responseInformationData.AuthenticationAttribute.Length > 0)
                            {
                                // здесь работа с БД ICSM
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(faultstring);
                    }
                }
                else
                {
                    throw new Exception("Response Url not contains parameters 'ticket' or 'customData'"); 
                }
            }

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
            var ticketId = SignData.GetResponseinitAuthentication(this._config.ViispRequestUrl, xmlInit, out string faultString);
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
