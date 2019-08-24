using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.DataModels.Sdrns.Server
{
    [Serializable]
    public class CommandMeasTaskPipebox
    {
        public MeasTask MeasTaskPipeBox;
        public MeasTaskMode MeasTaskModePipeBox;
        public CommonOperation CommonOperationPipeBox;
    }

}
