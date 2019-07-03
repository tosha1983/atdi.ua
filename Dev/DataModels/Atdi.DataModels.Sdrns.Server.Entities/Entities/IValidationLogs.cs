﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IValidationLogs_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IValidationLogs : IValidationLogs_PK
    {
        string Event { get; set; }
        string TableName { get; set; }
        long? Lcount { get; set; }
        string Info { get; set; }
        string Who { get; set; }
        DateTime When { get; set; }
    }
}