using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
using Atdi.DataModels.CoverageCalculation;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.WebQuery;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels;
using System.ServiceModel;
using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels.Identity;
using Atdi.Platform.Logging;
using Atdi.WebQuery.CoverageCalculation;

namespace Atdi.WebQuery.CoverageCalculation
{
    public class InitWebQuery
    {
        private ILogger _logger { get; set; }
        private IWebQuery _webQuery  { get; set; }
        private UserToken _userToken  { get; set; }
        private QueryToken _queryTokenStationsCalcCoverage { get; set; }
        private QueryToken _queryTokenResultCalcCoverage { get; set; }



        public IWebQuery WebQuery => this._webQuery;

        public UserToken UserToken => this._userToken;

        public QueryToken QueryTokenStationsCalcCoverage => this._queryTokenStationsCalcCoverage;

        public QueryToken QueryTokenResultCalcCoverage => this._queryTokenResultCalcCoverage;

        static IWebQuery GetWebQueryByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IWebQuery>(endpointName);
            return f.CreateChannel();
        }

        static IAuthenticationManager GetAuthenticationManagerByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IAuthenticationManager>(endpointName);
            return f.CreateChannel();
        }

        public InitWebQuery(DataConfig dataConfig, ILogger logger)
        {
            this._logger = logger;

            var authManager = GetAuthenticationManagerByEndpoint("HttpAuthenticationManager");

            var userCredential = new UserCredential()
            {
                UserName = dataConfig.IdentityWcfServicesBindingConfig.UserName,
                Password = dataConfig.IdentityWcfServicesBindingConfig.PasswordName
            };

            var authResult = authManager.AuthenticateUser(userCredential);
            if (authResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(Exceptions.ErrorCallMethodAuthenticateUser);
            }

            var userIdentity = authResult.Data;

            this._userToken = userIdentity.UserToken;

            this._webQuery = GetWebQueryByEndpoint("HttpWebQuery");
            
            var groupsResult = this._webQuery.GetQueryGroups(this._userToken);
            if (groupsResult.State == OperationState.Fault)
            {
                throw new InvalidOperationException(Exceptions.ErrorCallMethodGetQueryGroups);
            }

            for (int i = 0; i < groupsResult.Data.Groups.Count(); i++)
            {
                if (groupsResult.Data.Groups[i].QueryTokens != null)
                {
                    for (int j = 0; j < groupsResult.Data.Groups[i].QueryTokens.Count(); j++)
                    {
                        var queryMetadata = this._webQuery.GetQueryMetadata(this._userToken, groupsResult.Data.Groups[i].QueryTokens[j]);
                        if (queryMetadata.State == OperationState.Fault)
                        {
                            throw new InvalidOperationException(Exceptions.ErrorCallMethodGetQueryMetadata);
                        }

                        if (queryMetadata.Data.Code == dataConfig.WebQueryWcfServicesConfig.NameWebQueryCalcCoverage)
                        {
                            this._queryTokenStationsCalcCoverage = groupsResult.Data.Groups[i].QueryTokens[j];
                        }
                        else if (queryMetadata.Data.Code == dataConfig.WebQueryWcfServicesConfig.NameWebQueryResultCalcCoverage)
                        {
                            this._queryTokenResultCalcCoverage = groupsResult.Data.Groups[i].QueryTokens[j];
                        }
                    }
                }
            }
            if (this._queryTokenStationsCalcCoverage == null)
            {
                throw new InvalidOperationException(Exceptions.TokenStationsCalcCoverageNotFound);
            }
            if (this._queryTokenResultCalcCoverage == null)
            {
                throw new InvalidOperationException(Exceptions.TokenResultCalcCoverageNotFound);
            }
        }
    }
}
