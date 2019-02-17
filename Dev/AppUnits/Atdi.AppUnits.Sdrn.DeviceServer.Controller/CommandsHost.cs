using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Controller
{
    class CommandsHost : ICommandsHost
    {
        private readonly Dictionary<CommandType, IDevice[]> _enumDevices;
        private readonly Dictionary<Type, IDevice[]> _typeDevices;

        public CommandsHost()
        {
            this._enumDevices = new Dictionary<CommandType, IDevice[]>();
            this._typeDevices = new Dictionary<Type, IDevice[]>();
        }

        public IDevice[] GetDevices(Type instanceType)
        {
            return this._typeDevices[instanceType];
        }

        public IDevice[] GetDevices(CommandType commandType)
        {
            return this._enumDevices[commandType];
        }

        public void Register(IDevice device, CommandType commandType, Type instanceType)
        {
            var devices = new IDevice[] { device };
            if (_enumDevices.ContainsKey(commandType))
            {
                var list = _enumDevices[commandType];
                _enumDevices[commandType] = list.Union(devices).ToArray();
            }
            else
            {
                _enumDevices.Add(commandType, devices);
            }

            if (_typeDevices.ContainsKey(instanceType))
            {
                var list = _typeDevices[instanceType];
                _typeDevices[instanceType] = list.Union(devices).ToArray();
            }
            else
            {
                _typeDevices.Add(instanceType, devices);
            }
        }
    }
}
