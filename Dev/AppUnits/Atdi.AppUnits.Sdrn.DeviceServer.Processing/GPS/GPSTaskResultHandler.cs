using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using DM = Atdi.DataModels.Sdrns.Device;
using System;
using System.Linq;
using System.Collections.Generic;
using Atdi.DataModels.Sdrn.DeviceServer;




namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class GPSTaskResultHandler : IResultHandler<GpsCommand, GpsResult, GPSTask, DispatchProcess>
    {
        public void Handle(GpsCommand command, GpsResult result, DataModels.Sdrn.DeviceServer.ITaskContext<GPSTask, DispatchProcess> taskContext)
        {
            taskContext.Task.Asl = result.Asl;
            taskContext.Task.Lat = result.Lat;
            taskContext.Task.Lon = result.Lon;
            if (result.Asl != null)
            {
                taskContext.Process.Asl = result.Asl.Value;
            }
            if (result.Lat != null)
            {
                taskContext.Process.Lat = result.Lat.Value;
            }
            if (result.Lon != null)
            {
                taskContext.Process.Lon = result.Lon.Value;
            }
        }
    }
}
