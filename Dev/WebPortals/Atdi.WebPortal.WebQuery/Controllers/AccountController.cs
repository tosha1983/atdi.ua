using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atdi.WebPortal.WebQuery.Utility;
using Atdi.WebPortal.WebQuery.ViewModels;
using Atdi.WebPortal.WebQuery.WebApiModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Atdi.WebPortal.WebQuery.Controllers
{
    public class AccountController : Controller
    {
        private readonly PortalSettings _portalSettings;

        public AccountController(IOptions<PortalSettings> options)
        {
            this._portalSettings = options.Value;
        }

        //
        // GET: /Account/SignIn
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl)
        {
            //var client = new WebQueryClient( new Uri(this._portalSettings.WebQueryApiUrl));
            //var userCredential = new WebApiModels.UserCredential
            //{
            //    UserName = "Andrey",
            //    Password = ""
            //};

            //var result = client.AuthenticateUserAsync(userCredential);
            ////result.Wait();
            //var identity =  result.GetAwaiter().GetResult();

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/SignIn
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToLocal(returnUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Portal");
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Signin", "Account");
        }
    }
}