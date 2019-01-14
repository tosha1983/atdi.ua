using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.AppServer.Contracts.Sdrns;



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
                Console.WriteLine($"Recieved sensor location with ID = '{message.Data.Id}'");
                LoadDataMeasTask loadTask = new LoadDataMeasTask();
                SaveMeasTaskSDR svTsk = new SaveMeasTaskSDR();
                DM.Sensor mtSDR = message.Data;
                {
                    SensorDBExtension opt_DB = new SensorDBExtension();
                    List<NH_SensorLocation> NH_L = opt_DB.LoadSensorLocationsFromDB(message.Data.Id.Value);
                    if (NH_L != null)
                    {
                        if (NH_L.Count > 0)
                        {
                            foreach (NH_SensorLocation z_o in NH_L)
                            {
                                if (message.Data.Locations != null)
                                {
                                    if (message.Data.Locations.Count() > 0)
                                    {
                                        if (message.Data.Locations.ToList().FindAll(t => Math.Abs(t.Lon.GetValueOrDefault() - z_o.Lon.GetValueOrDefault()) <= Config._Lon_Delta && Math.Abs(t.Lat.GetValueOrDefault() - z_o.Lat.GetValueOrDefault()) <= Config._Lat_Delta && t.Status != AllStatusLocation.Z.ToString()) != null)
                                            opt_DB.CloseOldSensorLocation(z_o);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
