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
    public class LoadStations
    {
        private ILogger _logger { get; set; }
        private IWebQuery _webQuery { get; set; }
        private UserToken _userToken { get; set; }
        private QueryToken _queryTokenStationsCalcCoverage { get; set; }
        private Condition _condition  { get; set; }


        public LoadStations(IWebQuery webQuery, UserToken userToken, QueryToken queryTokenStationsCalcCoverage, Condition condition, ILogger logger)
        {
            this._webQuery = webQuery;
            this._userToken = userToken;
            this._queryTokenStationsCalcCoverage = queryTokenStationsCalcCoverage;
            this._condition = condition;
            this._logger = logger;
        }

        public  Result<QueryResult> GetStationsFromWebQuery()
        {
            var result = new Result<QueryResult>();
            try
            {
                var fetchOptions = new DataModels.WebQuery.FetchOptions
                {
                    Id = Guid.NewGuid(),
                    Columns = new string[] {
                    "ID",
                    "NAME",
                    "ELEVATION",
                    "Position.ADDRESS",
                    "Position.ASL",
                    "PWR_ANT",
                    "TX_HIGH_FREQ",
                    "BW",
                    "AZIMUTH",
                    "AGL",
                    "GAIN",
                    "TX_LOSSES",
                    "RX_LOSSES",
                    "Position.LONGITUDE",
                    "Position.LATITUDE",
                    "Licence.ID",
                    "Antenna.POLARIZATION",
                    "Antenna.DIAGA",
                    "Antenna.DIAGH",
                    "Antenna.DIAGV",
                    "STATUS",
                    "STANDARD",
                    "Owner.CODE",
                    "Position.PROVINCE",
                    "AssignedFrequencies.RX_FREQ",
                    "AssignedFrequencies.TX_FREQ"
                },
                    ResultStructure = DataSetStructure.TypedRows,
                    Limit = new DataLimit
                    {
                        Type = LimitValueType.Records,
                        Value = 100000000
                    },
                    Condition = this._condition,
                    Orders = new OrderExpression[]
                    {
                    new  OrderExpression { ColumnName = "ID", OrderType = OrderType.Ascending },
                    }
                };
                result = this._webQuery.ExecuteQuery(this._userToken, this._queryTokenStationsCalcCoverage, fetchOptions);
                this._logger.Info(Contexts.CalcCoverages, Events.RequestDBStationsCompletedSuccessfully);
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
            return result;
        }
    }
}
