using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;

namespace XICSM.ICSControlClient.Environment
{
   
    public interface IRepositoryReadedEntity
    {
        string[] GetFieldNames();

        void LoadFromRecordset(IMRecordset source);
    }
    
}
