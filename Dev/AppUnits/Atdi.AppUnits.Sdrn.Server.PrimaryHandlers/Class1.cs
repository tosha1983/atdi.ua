using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Platform.Logging;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server.Entities.Types;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers
{
    public class TestHandler
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly IQueryExecutor _executor;

        public TestHandler(ILogger logger, IDataLayer<EntityDataOrm> dataLayer)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
            this._executor = dataLayer.Executor<SdrnServerDataContext>();
        }

        void OnMessage()
        {
            var queryTypes = this._dataLayer.Builder
                .From<IAntennaType>()
                .Select(
                    c => c.Id,
                    c => c.Name
                ).Where(c => c.Name, DataModels.DataConstraint.ConditionOperator.Like, "Mge");

            var resultTypes = this._executor.Fetch(queryTypes, reader =>
            {
                string name = reader.GetValue(c => c.Name);
                return name;
            });

            var insert = this._dataLayer.GetBuilder<IAntennaType>()
                .Insert()
                .SetValue(c => c.Name, "Новый тип");

            this._executor.Execute(insert);


            //var query = this._dataLayer.Builder
            //    .From<IAntennaExten1>()
            //    .Select(
            //        c => c.EXTENDED.Name
            //    ).Where(c => c.EXT1.ShortName, DataModels.DataConstraint.ConditionOperator.Equal, "");

            //this._dataLayer.GetBuilder<IAntenna>().Update()
            //    .SetValue(c => c.TYPE.Id, 25);

            //var result = this._executor.Fetch(query, reader =>
            //{
            //    string fullName = reader.GetValue(c => c.EXT1.FullName);
            //    reader.GetValue(c => c.TYPE.Id);   ANTENNNA
            //    reader.GetValue(c => c.TYPE.Name); ANENTA_TYPE


            //    return string.Empty;
            //});

            /// select 
            ///  AT.NAME as [TYPE.Name],
            ///  A.TYPE_ID as [TYPE.Id],
            ///  A.TYPE_TABLE_NAME AS [TYPE.TableName]
            /// from ANTENNA A left join ANTENNA_TYPE AT on (A.TYPE_ID = AT.ID AND A.TYPE_TABLE_NAME = AT.TABLE_NAME)
            /// ID TABLE_NAME
            /// 1  AnetnaBase 
            //  23 AntneaBase
        }
    }
}
