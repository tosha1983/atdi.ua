using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.DataModels.Sdrns.Server
{
    [Serializable]
    public class ClientMeasTaskPipebox
    {
        public MeasTask[] MeasTasksWithAggregationServerPipeBox;
        public string[] AggregationServerInstancesPipeBox;
        public MeasTask MeasTaskPipeBox;
        public PrepareSendEvent[] PrepareSendEvents;
        public MeasTaskMode MeasTaskModePipeBox;
        public CommonOperation CommonOperationPipeBox;
    }

    [Serializable]
    public class ClientMeasTaskPiperesult
    {
        public long MeasTaskIdPipeResult;
        public CommonOperation CommonOperationPipeBoxResult;
    }
}
