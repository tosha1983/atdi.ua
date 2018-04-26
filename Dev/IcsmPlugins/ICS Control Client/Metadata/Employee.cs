using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;

namespace XICSM.ICSControlClient.Metadata
{
    public class Employee
    {
        public static readonly string TableName = "EMPLOYEE";

        public static class Fields
        {
            public static readonly string Id = "ID";
            public static readonly string AppUser = "APP_USER";
        }

        public static int GetCurrentUserId()
        {
            var result = IM.NullI;
            string userName = IM.ConnectedUser();

            var employeeRs = new IMRecordset(Employee.TableName, IMRecordset.Mode.ReadOnly);
            employeeRs.Select(Employee.Fields.Id);
            employeeRs.SetWhere(Employee.Fields.AppUser, IMRecordset.Operation.Eq, userName);

            using (employeeRs.OpenWithScope())
            {
                if (!employeeRs.IsEOF())
                {
                    result = employeeRs.GetI(Employee.Fields.Id);
                }    
            }

            return result;
        }
    }
}
