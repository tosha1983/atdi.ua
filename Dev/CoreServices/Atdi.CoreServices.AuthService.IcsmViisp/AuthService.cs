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
using Atdi.DataModels.DataConstraint;
using Atdi.CoreServices.Identity;
using Atdi.DataModels;



namespace Atdi.CoreServices.AuthService.IcsmViisp
{
	public class AuthService : IAuthService
	{
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private readonly AppServerComponentConfig _config;
		private readonly ILogger _logger;
		public static readonly string Name = "E-Government";
        public static readonly string TicketName = "ticket";
        public static readonly string CorrelationName = "customData";
        public static readonly string UriUniqueNode = "#uniqueNodeId";

        public AuthService(AppServerComponentConfig config, IDataLayer<IcsmDataOrm> dataLayer,  ILogger logger)
		{
			_config = config;
			_logger = logger;
            _dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
        }

        /// <summary>
        /// По полученной от E-Government PostBackUrl строки делаем запрос к E-Government и получаем идентификационные данные о пользователе
        /// </summary>
        /// <param name="response"></param>
        /// <param name="tokenProvider"></param>
        /// <returns></returns>
        public UserIdentity AuthenticateUser(AuthRedirectionResponse response, IUserTokenProvider tokenProvider)
		{
            UserTokenData userTokenData = null;
            // парсим URL
            var dicParameters = Helpers.ParseQueryString(response.Url);
            if ((dicParameters!=null) && (dicParameters.Count>0))
            {
                // если обнаружены параметры ticket и customData
                if ((dicParameters.ContainsKey(TicketName)) && (dicParameters.ContainsKey(CorrelationName)))
                {
                    // формирование запроса к E-Government
                    var xmlRequest = SignData.AuthenticationData(this._config.ViispSecretKey, _config.ViispPublicKey, UriUniqueNode, this._config.ViispPID, dicParameters[TicketName]);
                    // пробуем получить ответ с идентификационными данными пользователя
                    var responseInformationData = SignData.GetResponseAuthenticationData(this._config.ViispRequestUrl, xmlRequest, out string faultstring);
                    // если ответ не содержит ошибок
                    if (string.IsNullOrEmpty(faultstring))
                    {
                        if (responseInformationData != null)
                        {
                            if (responseInformationData.AuthenticationAttribute.Length > 0)
                            {
                                var userInformation = responseInformationData.UserInformation;
                                var authenticationAttribute = responseInformationData.AuthenticationAttribute[0];

                                this._logger.Info(Contexts.ThisComponent, Categories.Handling, Events.InformationAboutAuthenticatedUser.With(authenticationAttribute.Value, string.Format("{0} {1}", userInformation.firstName, userInformation.lastName), userInformation.email));

                                // поиск по базе данных пользователя по полю REGIST_NUM 
                                // здесь работа с БД ICSM
                                var queryUsers = this._dataLayer.Builder
                                .From<USERS>()
                                .Where(c => c.REGIST_NUM, ConditionOperator.Equal, authenticationAttribute.Value)
                                .Select(
                                c => c.ID,
                                c => c.NAME,
                                c => c.EMAIL,
                                c => c.CODE,
                                c => c.CUST_TXT3)
                                .OrderByAsc(c => c.ID)
                                .OnTop(1);


                                userTokenData = this._queryExecutor
                                   .Fetch(queryUsers, reader =>
                                   {
                                       if (reader.Read())
                                       {
                                           // если пользователь обнаружен - формируем токен 
                                           return new UserTokenData
                                           {
                                               AuthDate = DateTime.Now,
                                               Id = reader.GetValue(c => c.ID),
                                               UserCode = reader.GetValue(c => c.CODE),
                                               UserId = reader.GetValue(c => c.ID),
                                               UserName = reader.GetValue(c => c.NAME)
                                           };
                                       }
                                       return default(UserTokenData);
                                   });

                                // пользователь с заданным  RegistNum не найден
                                // в этом случае создаем нового пользователя в БД ICSM
                                if (userTokenData == null)
                                {
                                    // select maxId
                                    var queryMaxId = this._dataLayer.Builder
                                       .From<USERS>()
                                       .Where(c => c.ID, ConditionOperator.GreaterThan, 0)
                                       .Select(c => c.ID)
                                       .OrderByDesc(c => c.ID)
                                       .OnTop(1);

                                    // поиск максимального значения по полю Id
                                    var userMaxId = this._queryExecutor
                                     .Fetch(queryMaxId, reader =>
                                     {
                                         int Id = -1;
                                         if (reader.Read())
                                         {
                                             Id = reader.GetValue(c => c.ID);
                                         }
                                         return Id;
                                     });

                                    // если нет записей в таблице USERS, в этом случае идентификатору присваиваем 0
                                    if (userMaxId == -1)
                                    {
                                        userMaxId = 0;
                                    }
                                    // увеличиваем индекс на 1
                                    var newId = ++userMaxId;
                                    // формируем поле CODE на основе сгенерированного ранее значения идентификатора
                                    string code = "I#" + newId.ToString().PadLeft(9, '0');
                                    // вставка новой записи в таблицу USERS
                                    var insertQuery = _dataLayer.Builder
                                       .Insert("USERS")
                                       .SetValue("ID", new IntegerValueOperand() { DataType = DataModels.DataType.Integer, Value = newId })
                                       .SetValue("CODE", new StringValueOperand() { DataType = DataModels.DataType.String, Value = code })
                                       .SetValue("NAME", new StringValueOperand() { DataType = DataModels.DataType.String, Value = string.Format("{0} {1}", userInformation.firstName, userInformation.lastName) })
                                       .SetValue("REGIST_NUM", new StringValueOperand() { DataType = DataModels.DataType.String, Value = authenticationAttribute.Value })
                                       .SetValue("CUST_TXT3", new StringValueOperand() { DataType = DataModels.DataType.String, Value = "Provider" })
                                       .SetValue("EMAIL", new StringValueOperand() { DataType = DataModels.DataType.String, Value = userInformation.email });
                                    if (this._queryExecutor.Execute(insertQuery) > 0)
                                    {
                                        this._logger.Info(Contexts.ThisComponent, Categories.Handling, Events.CreatedNewUser.With(newId, authenticationAttribute.Value, string.Format("{0} {1}", userInformation.firstName, userInformation.lastName), userInformation.email));
                                        // формируем токен
                                        userTokenData = new UserTokenData
                                        {
                                            AuthDate = DateTime.Now,
                                            Id = newId,
                                            UserCode = code,
                                            UserId = newId,
                                            UserName = string.Format("{0} {1}", userInformation.firstName, userInformation.lastName)
                                        };
                                    }
                                }
                            }
                            else
                            {
                                this._logger.Exception(Contexts.ThisComponent, new InvalidOperationException(Exceptions.ResponseInformationDataAuthenticationAttributeZero));
                                throw new InvalidOperationException(Exceptions.ResponseInformationDataAuthenticationAttributeZero);
                            }
                        }
                        else
                        {
                            this._logger.Exception(Contexts.ThisComponent, new InvalidOperationException(Exceptions.ResponseInformationDataIsNull));
                            throw new InvalidOperationException(Exceptions.ResponseInformationDataIsNull);
                        }
                    }
                    else
                    {
                        this._logger.Exception(Contexts.ThisComponent, new InvalidOperationException(Exceptions.GetResponseAuthenticationDataFaultString.With(faultstring)));
                        throw new InvalidOperationException(Exceptions.GetResponseAuthenticationDataFaultString.With(faultstring));
                    }
                }
                else
                {
                    this._logger.Exception(Contexts.ThisComponent, new InvalidOperationException(Exceptions.ResponseUrlNotContainsParametersTicketOrCustomData));
                    throw new InvalidOperationException(Exceptions.ResponseUrlNotContainsParametersTicketOrCustomData); 
                }
            }
            if (userTokenData != null)
            {
                return new UserIdentity
                {
                    UserToken = tokenProvider.CreatUserToken(userTokenData),
                    Id = userTokenData.Id,
                    Name = userTokenData.UserName
                };
            }
            else
            {
                this._logger.Exception(Contexts.ThisComponent, new InvalidOperationException(Exceptions.UnexpectedErrorOccurredDuringMethodExecution));
                throw new InvalidOperationException(Exceptions.UnexpectedErrorOccurredDuringMethodExecution);
            }
        }


        /// <summary>
        /// Тут начало процесса, первая фаза до формирования URL для ридеректа
        /// его формируем и передаем в свойствах объекта результата
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public AuthRedirectionQuery PrepareAuthRedirection(AuthRedirectionOptions options)
		{
            var correlationData = Guid.NewGuid();
            var xmlInit = SignData.InitAuthentication(this._config.ViispSecretKey, _config.ViispPublicKey, UriUniqueNode, this._config.ViispPID, options.ReturnUrl, correlationData.ToString());
            var ticketId = SignData.GetResponseinitAuthentication(this._config.ViispRequestUrl, xmlInit, out string faultString);
            if (string.IsNullOrEmpty(faultString))
            {
                var url = this._config.ViispServiceUrl + $"?{TicketName}={ticketId}";
                this._logger.Info(Contexts.ThisComponent, Categories.Handling, Events.RedirectionQuery.With(url));
                return new AuthRedirectionQuery
                {
                    AuthService = options.AuthService,
                    Url = url
                };
            }
            else
            {
                this._logger.Exception(Contexts.ThisComponent, new InvalidOperationException(Exceptions.PrepareAuthRedirectionFaultString.With(faultString)));
                throw new InvalidOperationException(Exceptions.PrepareAuthRedirectionFaultString.With(faultString));
            }
		}
	}
}
