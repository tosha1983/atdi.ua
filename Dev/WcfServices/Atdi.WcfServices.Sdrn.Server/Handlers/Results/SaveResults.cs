using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Linq;


namespace Atdi.WcfServices.Sdrn.Server
{
    public class SaveResults 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public SaveResults(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }

        public CommonOperationDataResult<int> DeleteResultFromDB(MeasurementResultsIdentifier measResultsId, string status)
        {
            CommonOperationDataResult<int> result = new CommonOperationDataResult<int>();
            if (measResultsId != null)
            {
                try
                {
                    this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallDeleteResultFromDBMethod.Text);
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        scope.BeginTran();

                        var builderUpdateResMeas = this._dataLayer.GetBuilder<MD.IResMeas>().Update();
                        if (measResultsId.MeasSdrResultsId > 0) builderUpdateResMeas.Where(c => c.Id, ConditionOperator.Equal, measResultsId.MeasSdrResultsId);
                        if (measResultsId.SubMeasTaskStationId > 0) builderUpdateResMeas.Where(c => c.MEAS_SUBTASK_STATION.Id, ConditionOperator.Equal, measResultsId.SubMeasTaskStationId);
                        if (measResultsId.MeasTaskId.Value > 0) builderUpdateResMeas.Where(c => c.MEAS_SUBTASK_STATION.MEAS_SUBTASK.MEAS_TASK.Id, ConditionOperator.Equal, measResultsId.MeasTaskId.Value);
                        if (measResultsId.SubMeasTaskId > 0) builderUpdateResMeas.Where(c => c.MEAS_SUBTASK_STATION.MEAS_SUBTASK.Id, ConditionOperator.Equal, measResultsId.SubMeasTaskId);
                        builderUpdateResMeas.SetValue(c => c.Status, status);
                        if (scope.Executor.Execute(builderUpdateResMeas) > 0)
                        {
                            result.State = CommonOperationState.Success;
                        }
                        else
                        {
                            result.State = CommonOperationState.Fault;
                        }
                        scope.Commit();
                    }
                }
                catch (Exception e)
                {
                    result.State = CommonOperationState.Fault;
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return result;
        }

        public bool AddAssociationStationByEmitting(long[] emittingsId, long AssociatedStationID, string AssociatedStationTableName)
        {
            var isSuccess = false;
            if ((emittingsId != null) && (AssociatedStationID > 0) && (!string.IsNullOrEmpty(AssociatedStationTableName)))
            {
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        scope.BeginTran();

                        long?[] emittingsIdConvert = emittingsId.Select(n => (long?)(n)).ToArray();
                        var builderUpdateEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Update();
                        builderUpdateEmitting.Where(c => c.Id, ConditionOperator.In, emittingsIdConvert);
                        builderUpdateEmitting.SetValue(c => c.StationID, AssociatedStationID);
                        builderUpdateEmitting.SetValue(c => c.StationTableName, AssociatedStationTableName);
                        if (scope.Executor.Execute(builderUpdateEmitting) > 0)
                        {
                            isSuccess = true;
                        }
                        scope.Commit();
                    }
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return isSuccess;
        }

        public bool DeleteEmitting(long[] emittingsId)
        {
            var isSuccess = true;
            if (emittingsId != null)
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCallDeleteResultFromDBMethod.Text);
                try
                {
                    using (var scope = this._dataLayer.CreateScope<SdrnServerDataContext>())
                    {
                        scope.BeginTran();

                        var nullableEmittings = emittingsId.Cast<long?>().ToArray();

                        var builderDeleteWorkTime = this._dataLayer.GetBuilder<MD.IWorkTime>().Delete();
                        builderDeleteWorkTime.Where(c => c.EmittingId, ConditionOperator.In, nullableEmittings);
                        var cntDelIWorkTime = scope.Executor.Execute(builderDeleteWorkTime);

                        //var builderDeleteSignalMask = this._dataLayer.GetBuilder<MD.ISignalMask>().Delete();
                        //builderDeleteSignalMask.Where(c => c.EmittingId, ConditionOperator.In, nullableEmittings);
                        //var cntDelISignalMask = queryExecuter.Execute(builderDeleteSignalMask);

                        var builderDeleteSpectrum = this._dataLayer.GetBuilder<MD.ISpectrum>().Delete();
                        builderDeleteSpectrum.Where(c => c.EMITTING.Id, ConditionOperator.In, nullableEmittings);
                        var cntDelISpectrum = scope.Executor.Execute(builderDeleteSpectrum);

                        var builderDeleteEmitting = this._dataLayer.GetBuilder<MD.IEmitting>().Delete();
                        builderDeleteEmitting.Where(c => c.Id, ConditionOperator.In, nullableEmittings);
                        var cntDelIEmitting = scope.Executor.Execute(builderDeleteEmitting);

                        scope.Commit();
                    }
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    this._logger.Exception(Contexts.ThisComponent, e);
                }
            }
            return isSuccess;
        }
    }
}


