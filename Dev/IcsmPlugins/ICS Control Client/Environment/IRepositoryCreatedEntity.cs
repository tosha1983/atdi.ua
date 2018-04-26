using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;

namespace XICSM.ICSControlClient.Environment
{

    public interface IRepositoryCreatedEntity 
    {
        void SetId(int id);

        string[] GetFieldNames();

        void SaveToRecordset(IMRecordset source);
    }
}
