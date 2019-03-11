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
            if (result != null)
            {
                try
                {
                    taskContext.SetEvent(result);
                    taskContext.Finish();
                }
                catch (Exception ex)
                {
                    taskContext.SetEvent<ExceptionProcessGPS>(new ExceptionProcessGPS(CommandFailureReason.Exception, ex));
                    taskContext.Abort(ex);
                }
            }
        }
    }
}
