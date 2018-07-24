using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery.DTO
{
    internal sealed class XUPDATEOBJECTS
    {
        public int ID { get; set; }
        public string OBJTABLE { get; set; }
        public DateTime? DATEMODIFIED { get; set; }
    }
}
