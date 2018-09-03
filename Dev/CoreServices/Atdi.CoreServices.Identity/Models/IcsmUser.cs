using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.Identity.Models
{
    internal class IcsmUser
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string WebLogin { get; set; }

        public string Password { get; set; }

        public string AppUser { get; set; }
    }
}
