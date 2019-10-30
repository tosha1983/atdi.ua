﻿using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Atdi.Platform;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    internal class ResultHandlersHost : IResultHandlersHost
    {
        private readonly IResultHandlerFactory _handlerFactory;
        private readonly IStatistics _statistics;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, IResultHandler> _handlers;
        private readonly Dictionary<string, ResultHandlerDecriptor> _descriptors;
        private readonly object _loker = new object();

        public ResultHandlersHost(IResultHandlerFactory handlerFactory, IStatistics statistics,  ILogger logger)
        {
            this._handlerFactory = handlerFactory;
            this._statistics = statistics;
            this._logger = logger;
            this._handlers = new ConcurrentDictionary<string, IResultHandler>();
            this._descriptors = new Dictionary<string, ResultHandlerDecriptor>();
        }

        public IResultHandler GetHandler(Type commandType, Type resultType, Type taskType, Type processType)
        {
            var key = ResultHandlerDecriptor.BuildKey(commandType, resultType, taskType, processType);
            
            if (this._handlers.TryGetValue(key, out var handler))
            {
                return handler;
            }

            if (!this._descriptors.ContainsKey(key))
            {
                throw new InvalidOperationException($"Not found handler for command of type '{commandType}' and result of type '{resultType}'");
            }
            lock(this._loker)
            {
                var descriptor = this._descriptors[key];
                var instance = this._handlerFactory.Create(descriptor.InstanceType);
                handler = new ResultHandler(descriptor, instance, this._statistics, this._logger);
                this._handlers.TryAdd(key, handler);
                return handler;
            }
        }

        public void Register(Type handlerInstanceType)
        {
            var descriptor = new ResultHandlerDecriptor(handlerInstanceType);
            lock (this._loker)
            {
                if (this._descriptors.ContainsKey(descriptor.Key))
                {
                    throw new InvalidOperationException($"Duplicate result handler: CommandType = '{descriptor.CommandType}', ResultType = '{descriptor.ResultType}'");
                }
                this._descriptors.Add(descriptor.Key, descriptor);
            }
        }
    }
}
