using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts;
using SD = Atdi.AppServer.Contracts.Sdrns;
using Atdi.AppServer.Common;

namespace Atdi.AppServer.AppService.SdrnsController.ConstraintParsers
{
    public class ShortSensorConstraintParser : StatementConstrainParserBase<SD.ShortSensor, ShortSensorConstraintParser>
    {
        public ShortSensorConstraintParser(IConstraintStatementBuilder builder)
            : base(builder, "SENSORS_TABLE_NAME")
        {
        }

        protected override void OnSetupMapping()
        {
            this.RegistryFieldMapping(c => c.Name, "NAME");
        }
    }
}
