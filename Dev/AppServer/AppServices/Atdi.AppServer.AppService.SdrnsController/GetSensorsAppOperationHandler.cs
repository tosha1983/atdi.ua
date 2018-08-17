using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Models.AppServices;
using Atdi.AppServer.Models.AppServices.SdrnsController;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.SDNRS.AppServer.BusManager;
using Atdi.SDNRS.AppServer;


namespace Atdi.AppServer.AppServices.SdrnsController
{
    public class GetSensorsAppOperationHandler
        : AppOperationHandlerBase
        <
            SdrnsControllerAppService,
            SdrnsControllerAppService.GetSensorsAppOperation,
            GetSensorsAppOperationOptions,
            Sensor[]
        >
    {

        public GetSensorsAppOperationHandler(IAppServerContext serverContext, ILogger logger) : base(serverContext, logger)
        {

        }

        /*
        public void CheckCondition(DataConstraint constraint)
        {
            if (constraint is DataConstraintExpression)
            {
                var operand = (constraint as DataConstraintExpression).LeftOperand;
                if (operand is DataConstraintColumnOperand)
                {
                    
                }
                if (operand is DataConstraintValueOperand)
                {

                }
            }
            else if (constraint is DataConstraintGroup)
            {
                if (((constraint as DataConstraintGroup).Constraints != null) && (((constraint as DataConstraintGroup).Constraints.Length > 0)))
                {
                    for (int i = 0; i < ((constraint as DataConstraintGroup).Constraints).Length; i++)
                    {
                        if (((constraint as DataConstraintGroup).Constraints)[i] is DataConstraintExpression)
                        {
                            CheckCondition(((constraint as DataConstraintGroup).Constraints)[i] as DataConstraintExpression);
                        }
                        else if (((constraint as DataConstraintGroup).Constraints)[i] is DataConstraintGroup)
                        {
                            CheckCondition(((constraint as DataConstraintGroup).Constraints)[i]);
                        }
                    }
                }

            }
        }
        */

        public override Sensor[] Handle(GetSensorsAppOperationOptions options, IAppOperationContext operationContext)
        {
            Logger.Trace(this, options, operationContext);
            List<Sensor> val = new List<Sensor>();
            val = GlobalInit.SensorListSDRNS;
            return val.ToArray();
        }
    }
     
}

