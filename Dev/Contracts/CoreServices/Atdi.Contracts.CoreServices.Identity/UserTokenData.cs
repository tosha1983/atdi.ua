﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.Identity
{ 
    [Serializable]
    public class UserTokenData
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public DateTime AuthDate { get; set; }
    }
}