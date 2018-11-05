using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atdi.WebPortal.WebQuery.WebApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Atdi.WebPortal.WebQuery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        private readonly PortalSettings _portalSettings;

        public EnvironmentController(IOptions<PortalSettings> options)
        {
            this._portalSettings = options.Value;
        }

        // GET api/environment
        [HttpGet]
        [Authorize]
        public PortalEnvironment Get()
        {
            return new PortalEnvironment
            {
                Title = _portalSettings.Title,
                Version = _portalSettings.Version,
                Company = new PortalCompany
                {
                    Title = _portalSettings.CompanyTitle,
                    Site = _portalSettings.CompanySite,
                    Email = _portalSettings.CompanyEmail
                },
                User = new PortalUser
                {
                    Name = this.HttpContext.User.Identity.Name
                }
            };

        }
    }
}