using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    interface IBufferProcessing
    {
        void Start();

        void Stop();

        void Save(Message message);
    }
}
