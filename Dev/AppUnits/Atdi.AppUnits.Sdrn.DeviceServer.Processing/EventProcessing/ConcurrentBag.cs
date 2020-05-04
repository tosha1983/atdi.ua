using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using System.Linq;
using Atdi.DataModels.EntityOrm;
using DM = Atdi.DataModels.Sdrns.Device;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public static class ConcurrentBag
    {
        static Object locker = new object();

        public static ConcurrentBag<T> Remove<T>(this ConcurrentBag<T> bagSource, List<T> itemlist)
        {
            lock (locker)
            {
                var removelist = bagSource.ToList();

                Parallel.ForEach(itemlist, currentitem =>
                {
                    removelist.Remove(currentitem);
                });

                bagSource = new ConcurrentBag<T>();

                Parallel.ForEach(removelist, currentitem =>
                {
                    bagSource.Add(currentitem);
                });

                return bagSource;
            }
        }

        public static ConcurrentBag<T> Remove<T>(this ConcurrentBag<T> bagSource, T removeitem)
        {
            lock (locker)
            {
                var removelist = bagSource.ToList();
                removelist.Remove(removeitem);

                bagSource = new ConcurrentBag<T>();

                Parallel.ForEach(removelist, currentitem =>
                {
                    bagSource.Add(currentitem);
                });

                return bagSource;
            }
        }
    }
}
