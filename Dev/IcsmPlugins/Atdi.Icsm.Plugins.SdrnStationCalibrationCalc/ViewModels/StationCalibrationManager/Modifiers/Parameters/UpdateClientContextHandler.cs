using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Events;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Modifiers
{
    public class UpdateClientContextHandler : ICommandHandler<UpdateClientContext>
    {
        private readonly AppComponentConfig _config;
        private readonly CalcServerDataLayer _dataLayer;
        private readonly IEventBus _eventBus;

        public UpdateClientContextHandler(AppComponentConfig config, CalcServerDataLayer dataLayer, IEventBus eventBus)
        {
            _config = config;
            _dataLayer = dataLayer;
            _eventBus = eventBus;
        }

        public void Handle(UpdateClientContext command)
        {

            var updQuery = _dataLayer.GetBuilder<IClientContext>()
               .Update()
               .SetValue(c => c.StatusCode, (byte)ClientContextStatusCode.Pending)
               .SetValue(c => c.StatusName, "Pending")
               .SetValue(c => c.StatusNote, "")
               .Filter(c => c.Id, command.ClientContextId);

            _dataLayer.Executor.Execute(updQuery);

            var cancel = false;
            while (!cancel)
            {
                System.Threading.Thread.Sleep(5 * 1000);

                var checkQuery = _dataLayer.GetBuilder<IClientContext>()
                    .Read()
                    .Select(c => c.StatusCode)
                    .Select(c => c.StatusNote)
                    .Filter(c => c.Id, command.ClientContextId);

                cancel = _dataLayer.Executor.ExecuteAndFetch(checkQuery, reader =>
                {
                    if (reader.Count == 0 || !reader.Read())
                    {
                        throw new InvalidOperationException($"A client context not found by ID #{command.ClientContextId}");
                    }

                    var status = (ClientContextStatusCode)reader.GetValue(c => c.StatusCode);
                    var statusNote = reader.GetValue(c => c.StatusNote);


                    if (status == ClientContextStatusCode.Failed)
                    {
                        throw new InvalidOperationException($"Error preparing client context with ID #{command.ClientContextId}: {statusNote}");
                    }
                    return status == ClientContextStatusCode.Prepared;
                });
            }
        }
    }
}
