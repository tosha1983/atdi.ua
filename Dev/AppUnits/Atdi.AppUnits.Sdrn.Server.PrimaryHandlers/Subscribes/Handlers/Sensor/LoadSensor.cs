using Atdi.Contracts.Api.EventSystem;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.Server;
using MSG = Atdi.DataModels.Sdrns.BusMessages;
using DEV = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.DataConstraint;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;




namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Subscribes
{

    public class LoadSensor
    {
        private readonly ILogger _logger;
        private readonly IDataLayer<EntityDataOrm> _dataLayer;

        public LoadSensor(IDataLayer<EntityDataOrm> dataLayer,  ILogger logger)
        {
            this._logger = logger;
            this._dataLayer = dataLayer;
        }


        public int? ReadSensor(string Name, string TechId)
        {
            int? id = null;

            var query = this._dataLayer.GetBuilder<MD.ISensor>()
                .From()
                .Select(c => c.Name)
                .Select(c => c.Id)
                .Where(c => c.Name, ConditionOperator.Equal, Name)
                .Where(c => c.TechId, ConditionOperator.Equal, TechId)
                .OrderByAsc(c => c.Id)
                ;

            var sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
            .Fetch(query, reader =>
            {
                while (reader.Read())
                {
                    id = reader.GetValue(c => c.Id);
                }
                return true;
            });

            return id;
        }

        public DateTime? GetLastActivity(int id)
        {
            DateTime? lastActivity = null;
            var query = this._dataLayer.GetBuilder<MD.ISensor>()
                .From()
                .Select(c => c.Id)
                .Select(c => c.LastActivity)
                .Where(c => c.Id, ConditionOperator.Equal, id)
                .OrderByAsc(c => c.Id)
                ;

            var sensorExistsInDb = this._dataLayer.Executor<SdrnServerDataContext>()
            .Fetch(query, reader =>
            {
                while (reader.Read())
                {
                    lastActivity = reader.GetValue(c => c.LastActivity);
                }
                return true;
            });

            return lastActivity;
        }
    }
}

