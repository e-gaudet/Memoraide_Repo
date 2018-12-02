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

        public IActionResult Index()
        {
            return View();
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
            UserViewModel user = null;

            var response = await client.PostAsJsonAsync(url, model);
            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                user = JsonConvert.DeserializeObject<UserViewModel>(jsonstring.Result);
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
                new Claim("FullName", user.FirstName + " " + user.LastName),
                new Claim("UserID", user.Id.ToString())
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

            if (returnUrl != null)
                return Redirect(returnUrl);
            else
                return Redirect("/Home/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home/Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            string url = "https://localhost:44356/users/Register";
            RegisterResultModel result;

            var response = await client.PostAsJsonAsync(url, model);
            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                result = JsonConvert.DeserializeObject<RegisterResultModel>(jsonstring.Result);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server error");
                return View(model);
            }

            if (!result.success)
            {
                //Can check for specific error codes here if want to handle unqiuely
                
                ModelState.AddModelError(string.Empty, result.errormessage);
                return View(model);
            }

            return RedirectToAction("login", returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PleaseLogin()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ViewUser()
        {
            string url = "https://localhost:44356/Users/";

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
               
                List<UserViewModel> uvm = JsonConvert.DeserializeObject<List<UserViewModel>>(jsonstring.Result);
                return View(uvm);
            }
            else
            {
                TempData["message"] = "Unable to user card data";
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewUserDecks(int? id)
        {
            string url = "https://localhost:44356/Users/Decks/" + id;
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                List<DeckViewModel> decks = JsonConvert.DeserializeObject<List<DeckViewModel>>(jsonstring.Result);
                return View(decks);
            }
            else
            {
                TempData["message"] = "Unable to grab Deck data";
                return NotFound();
            }
        }
    }
}