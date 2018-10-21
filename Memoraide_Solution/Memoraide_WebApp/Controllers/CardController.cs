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
        public IActionResult Create([Bind("CardFront,CardBack,CardTags")] CardViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["message"] = "Successfully added " + model.CardBack + " to " + "tempdeckname";
                return RedirectToAction("Create");
            }
            else
                return View(model);
        }

        [HttpGet]
        public IActionResult ViewCard(int id)
        {
            //TODO: CALL TO API HERE
            CardViewModel model = new CardViewModel();
            model.CardFront = "This is a test";
            model.CardBack = "This is a test";
            model.CardTags = "Test, test";
            return View(model);
        }
    }
}