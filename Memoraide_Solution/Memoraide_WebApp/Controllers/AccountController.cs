using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Memoraide_WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Memoraide_WebApp.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ILogger logger;
        private readonly HttpClient client;

        public AccountController(ILogger<AccountController> logger)
        {
            this.logger = logger;
            client = new HttpClient();
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            string url = "https://localhost:44356/users/Authenticate";
            User user = null;

            var response = await client.PostAsJsonAsync(url, model);
            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                user = JsonConvert.DeserializeObject<User>(jsonstring.Result);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error");
                return View(model);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FullName", user.FirstName + " " + user.LastName)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //None for now
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PleaseLogin()
        {
            return View();
        }
    }
}