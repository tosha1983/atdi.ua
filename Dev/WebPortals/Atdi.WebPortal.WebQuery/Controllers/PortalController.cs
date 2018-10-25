using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Atdi.WebPortal.WebQuery.Controllers
{
    public class PortalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}