﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.Netkey
{
    public interface INetKeyValidator
    {
        int GetTokenValue(string softname, string exedate);
    }
}
