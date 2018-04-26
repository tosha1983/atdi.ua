using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;

namespace XICSM.ICSControlClient.Environment
{
   
    public interface IRepositoryUpdatedEntity
    {
        int GetId();

        string[] GetFieldNames();

        void SaveToRecordset(IMRecordset source);
    }

    
}
