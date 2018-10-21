using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Memoraide_WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Memoraide_WebApp.Controllers
{
    public class CardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CardFront,CardBack")] CardViewModel model)
        {
            if (ModelState.IsValid)
                return RedirectToAction("Create");
            else 
                return View(model);
        }
    }
}