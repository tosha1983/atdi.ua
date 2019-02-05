using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using DM = Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppUnits.Sdrn.ControlA.Bus;
using Atdi.AppUnits.Sdrn.ControlA.ManageDB;


namespace Atdi.AppUnits.Sdrn.ControlA.Handlers
{
    class UpdateSensorLocationResultHandler : MessageHandlerBase<Atdi.AppServer.Contracts.Sdrns.Sensor>
    {
        private readonly IBusGate _gate;

        public UpdateSensorLocationResultHandler(IBusGate gate)
            : base("UpdateSensorLocationResult")
        {
            this._gate = gate;
        }

        public override void OnHandle(IReceivedMessage<DM.Sensor> message)
        {
            if (message.Data != null)
            {
                Launcher._logger.Info(Contexts.ThisComponent, Categories.RecievedSensorLocation, string.Format(Events.RecievedSensorLocation.ToString(), message.Data.Id));
                var extDB = new SensorDb();
                var NH = extDB.LoadSensorLocationsFromDB(message.Data.Id.Value);
                if (NH != null)
                {
                    if (NH.Count > 0)
                    {
                        foreach (var z in NH)
                        {
                            if (message.Data.Locations != null)
                            {
                                if (message.Data.Locations.Count() > 0)
                                {
                                    if (message.Data.Locations.ToList().FindAll(t => Math.Abs(t.Lon.GetValueOrDefault() - z.Lon.GetValueOrDefault()) <= ConfigParameters.LonDelta && Math.Abs(t.Lat.GetValueOrDefault() - z.Lat.GetValueOrDefault()) <= ConfigParameters.LatDelta && t.Status != AllStatusLocation.Z.ToString()) != null)
                                        extDB.CloseOldSensorLocation(z);
                                }
                            }
                        }
                    }
                }
                message.Result = MessageHandlingResult.Confirmed;
            }
        }
    }
}
