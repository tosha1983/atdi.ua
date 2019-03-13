using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
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


    public class CalcStationLevelsByTask 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;



        public CalcStationLevelsByTask(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }




        public StationLevelsByTask[] GetStationLevelsByTask(LevelsByTaskParams paramsStationLevelsByTask)
        {
            var lOUT = new List<StationLevelsByTask>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerGetStationLevelsByTaskMethod.Text);
                double ANT_VAL = 0; // єто костыль
                var listLevelMeas2 = new List<StationLevelsByTask>();
                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderResStLevelCar = this._dataLayer.GetBuilder<MD.IResStLevelCar>().From();
                builderResStLevelCar.Select(c => c.RESSTATION.ResMeasId);
                builderResStLevelCar.Select(c => c.RESSTATION.SectorId);
                builderResStLevelCar.Select(c => c.Agl);
                builderResStLevelCar.Select(c => c.Altitude);
                builderResStLevelCar.Select(c => c.Bw);
                builderResStLevelCar.Select(c => c.CentralFrequency);
                builderResStLevelCar.Select(c => c.DifferenceTimeStamp);
                builderResStLevelCar.Select(c => c.Id);
                builderResStLevelCar.Select(c => c.Lat);
                builderResStLevelCar.Select(c => c.LevelDbm);
                builderResStLevelCar.Select(c => c.LevelDbmkvm);
                builderResStLevelCar.Select(c => c.Lon);
                builderResStLevelCar.Select(c => c.ResStationId);
                builderResStLevelCar.Select(c => c.TimeOfMeasurements);
                builderResStLevelCar.Select(c => c.RESSTATION.RESMEAS.MeasTaskId);
                builderResStLevelCar.Where(c => c.RESSTATION.RESMEAS.Id, ConditionOperator.In, paramsStationLevelsByTask.MeasResultID.ToArray());
                builderResStLevelCar.Where(c => c.RESSTATION.RESMEAS.MeasTaskId, ConditionOperator.Equal, paramsStationLevelsByTask.MeasTaskId.ToString());
                builderResStLevelCar.Where(c => c.RESSTATION.SectorId, ConditionOperator.Equal, paramsStationLevelsByTask.SectorId);
                queryExecuter.Fetch(builderResStLevelCar, readerResStLevelCar =>
                {
                    while (readerResStLevelCar.Read())
                    {
                        var tx = new StationLevelsByTask();
                        tx.Lon = readerResStLevelCar.GetValue(c => c.Lon);
                        tx.Lat = readerResStLevelCar.GetValue(c => c.Lat);
                        if (((readerResStLevelCar.GetValue(c => c.LevelDbmkvm) != 0) && (readerResStLevelCar.GetValue(c => c.LevelDbmkvm) != -1)) && (readerResStLevelCar.GetValue(c => c.LevelDbmkvm) > -30) && (readerResStLevelCar.GetValue(c => c.LevelDbmkvm) < 200))
                        {
                            tx.Level_dBmkVm = Math.Round(readerResStLevelCar.GetValue(c => c.LevelDbmkvm).Value, 2);
                            lOUT.Add(tx);
                        }
                        else
                        {
                            if ((readerResStLevelCar.GetValue(c => c.CentralFrequency).Value > 0.01) && (readerResStLevelCar.GetValue(c => c.LevelDbm) > -300) && (readerResStLevelCar.GetValue(c => c.LevelDbm) < -10))
                            {
                                tx.Level_dBmkVm = Math.Round((float)(77.2 + 20 * Math.Log10(readerResStLevelCar.GetValue(c => c.CentralFrequency).Value) + readerResStLevelCar.GetValue(c => c.LevelDbm) - ANT_VAL), 2);
                                lOUT.Add(tx);
                            }
                        }

                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return lOUT.ToArray();
        }
    }
    
}

