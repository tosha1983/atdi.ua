﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IEntity
    {
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string ParentId { get; set; }
        string ParentType { get; set; }
        string ContentType { get; set; }
        string HashAlgoritm { get; set; }
        string HashCode { get; set; }
    }
}
