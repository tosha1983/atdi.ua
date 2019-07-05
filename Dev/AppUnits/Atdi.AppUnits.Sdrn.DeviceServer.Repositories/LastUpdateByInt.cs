using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.DeviceServer.Entities;
using Atdi.DataModels.EntityOrm;
using Atdi.Modules.Sdrn.DeviceServer;
using System.Xml;
using System.Linq;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;


namespace Atdi.AppUnits.Sdrn.DeviceServer.Repositories
{
    public sealed class LastUpdateByInt64 : IRepository<LastUpdate, long?>
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public LastUpdateByInt64(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public long? Create(LastUpdate item)
        {
            long? ID = null;
           
            if (item != null)
            {
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDeviceDataContext>())
                    {
                        scope.BeginTran();

                        var builderInsertILastUpdate = this._dataLayer.GetBuilder<MD.ILastUpdate>().Insert();
                        builderInsertILastUpdate.SetValue(c => c.TableName, item.TableName);
                        builderInsertILastUpdate.SetValue(c => c.LastUpdate, item.LastDateTimeUpdate);
                        builderInsertILastUpdate.SetValue(c => c.Status, item.Status);

                        var lastUpdate = scope.Executor.Execute<MD.ILastUpdate_PK>(builderInsertILastUpdate);
                        ID = lastUpdate.Id;

                        scope.Commit();
                    }
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return ID;
        }

        public bool Delete(long? id)
        {
            bool isSuccessDelete = false;
            
            if (id != null)
            {
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDeviceDataContext>())
                    {
                        scope.BeginTran();
                        var builderDeleteLastUpdate = this._dataLayer.GetBuilder<MD.ILastUpdate>().Delete();
                        builderDeleteLastUpdate.Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, id);
                        if (scope.Executor.Execute(builderDeleteLastUpdate) > 0)
                        {
                            isSuccessDelete = true;
                        }
                        scope.Commit();
                    }
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return isSuccessDelete;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public int GetCountObjectsWithRestrict()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetDictionaryStatusObjects()
        {
            throw new NotImplementedException();
        }

        public LastUpdate[] LoadAllObjects()
        {
            var listLastUpdate = new List<LastUpdate>();
            LastUpdate val = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();
            var builderSelectLastUpdate = this._dataLayer.GetBuilder<MD.ILastUpdate>().From();
            builderSelectLastUpdate.Select(c => c.LastUpdate);
            builderSelectLastUpdate.Select(c => c.TableName);
            builderSelectLastUpdate.Select(c => c.Status);
            builderSelectLastUpdate.Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.GreaterThan, 0);
            builderSelectLastUpdate.OrderByDesc(c => c.Id);
            queryExecuter.Fetch(builderSelectLastUpdate, reader =>
            {
                while (reader.Read())
                {
                    val = new LastUpdate();
                    val.TableName = reader.GetValue(c => c.TableName);
                    val.LastDateTimeUpdate = reader.GetValue(c => c.LastUpdate);
                    val.Status = reader.GetValue(c => c.Status);
                    listLastUpdate.Add(val);
                }
                return true;
            });
            return listLastUpdate.ToArray();
        }
            

        public LastUpdate LoadObject(long? id)
        {
            LastUpdate val = null;
            var queryExecuter = this._dataLayer.Executor<SdrnServerDeviceDataContext>();
            var builderSelectLastUpdate = this._dataLayer.GetBuilder<MD.ILastUpdate>().From();
            builderSelectLastUpdate.Select(c => c.LastUpdate);
            builderSelectLastUpdate.Select(c => c.TableName);
            builderSelectLastUpdate.Select(c => c.Status);
            builderSelectLastUpdate.Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.Equal, id);
            builderSelectLastUpdate.OrderByDesc(c => c.Id);
            queryExecuter.Fetch(builderSelectLastUpdate, reader =>
            {
                while (reader.Read())
                {
                    val = new LastUpdate();
                    val.TableName = reader.GetValue(c => c.TableName);
                    val.LastDateTimeUpdate = reader.GetValue(c => c.LastUpdate);
                    val.Status = reader.GetValue(c => c.Status);
                }
                return true;
            });
            return val;
        }

        public LastUpdate[] LoadObjectsWithRestrict()
        {
            throw new NotImplementedException();
        }

        public bool Update(LastUpdate item)
        {
            bool isSuccessUpdate = false;
            if (item != null)
            {
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDeviceDataContext>())
                    {
                        scope.BeginTran();

                        var builderDeleteLastUpdate = this._dataLayer.GetBuilder<MD.ILastUpdate>().Update();
                        builderDeleteLastUpdate.Where(c => c.TableName, DataModels.DataConstraint.ConditionOperator.Equal, item.TableName);
                        builderDeleteLastUpdate.SetValue(c => c.LastUpdate, item.LastDateTimeUpdate);
                        builderDeleteLastUpdate.SetValue(c => c.Status, item.Status);
                        if (scope.Executor.Execute(builderDeleteLastUpdate) > 0)
                        {
                            isSuccessUpdate = true;
                        }
                        
                        scope.Commit();
                    }
                }
                catch (Exception e)
                {
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return isSuccessUpdate;
        }
    }
}



