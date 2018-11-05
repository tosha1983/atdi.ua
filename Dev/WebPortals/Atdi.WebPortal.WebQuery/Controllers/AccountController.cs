using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly WebQueryClient _webQueryClient;

        public AccountController(IOptions<PortalSettings> options, WebQueryClient webQueryClient)
        {
            this._portalSettings = options.Value;
            this._webQueryClient = webQueryClient;
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
        public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userCredential = new WebApiModels.UserCredential
            {
                UserName = model.Username,
                Password = model.Password
            };

            var userIdentity = await _webQueryClient.AuthenticateUserAsync(userCredential);
            if (userIdentity == null)
            {
                ModelState.AddModelError("", "Incorrect username or password");
                return View(model);
            }

            await this.Authenticate(userIdentity);

            return RedirectToLocal(returnUrl);
        }

        private async Task Authenticate(UserIdentity userIdentity)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userIdentity.Name),
                new Claim("WebQueryUserId", userIdentity.Id.ToString(), ClaimValueTypes.Integer32),
                new Claim("WebQueryUserTokenData", Convert.ToBase64String(userIdentity.UserToken.Data), ClaimValueTypes.Base64Binary)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var principal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
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