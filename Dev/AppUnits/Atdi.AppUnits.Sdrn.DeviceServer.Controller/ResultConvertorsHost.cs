using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class ResultConvertorsHost : IResultConvertorsHost
    {
        private readonly IResultConvertorFactory _convertorFactory;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, IResultConvertor> _convertors;
        private readonly Dictionary<string, ResultConvertorDecriptor> _descriptors;
        private object _loker = new object();

        public ResultConvertorsHost(IResultConvertorFactory convertorFactory, ILogger logger)
        {
            this._convertorFactory = convertorFactory;
            this._logger = logger;
            this._convertors = new ConcurrentDictionary<string, IResultConvertor>();
            this._descriptors = new Dictionary<string, ResultConvertorDecriptor>();
        }

        public IResultConvertor GetConvertor(Type fromType, Type toType)
        {
            var key = ResultConvertorDecriptor.BuildKey(fromType, toType);

            if (this._convertors.TryGetValue(key, out IResultConvertor convertor))
            {
                return convertor;
            }

            if (!this._descriptors.ContainsKey(key))
            {
                throw new InvalidOperationException($"Not found convertor of from type '{fromType}' to type '{toType}'");
            }
            lock (this._loker)
            {
                var descriptor = this._descriptors[key];
                var instance = this._convertorFactory.Create(descriptor.InstanceType);
                convertor = new ResultConvertor(descriptor, instance, this._logger);
                this._convertors.TryAdd(key, convertor);
                return convertor;
            }
        }

        public void Register(Type convertorType)
        {
            var descriptor = new ResultConvertorDecriptor(convertorType);
            lock (this._loker)
            {
                if (this._descriptors.ContainsKey(descriptor.Key))
                {
                    throw new InvalidOperationException($"Duplicate result convertor: FromType = '{descriptor.FromType}', ResultType = '{descriptor.ResultType}'");
                }
                this._descriptors.Add(descriptor.Key, descriptor);
            }
        }
    }
}
