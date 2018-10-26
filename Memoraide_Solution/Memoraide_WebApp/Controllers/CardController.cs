using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Memoraide_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using Newtonsoft.Json;
namespace Memoraide_WebApp.Controllers
{
    public class CardController : Controller
    {
        HttpClient client;

        public CardController()
        {
            client = new HttpClient();
        }

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
        public async Task<IActionResult> Create([Bind("CardFront,CardBack,CardTags,DeckId")] CardViewModel model)
        {
            if (ModelState.IsValid)
            {
                string url = "https://localhost:44356/Cards/";

                var response = await client.PostAsJsonAsync(url, model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["message"] = "Successfully added " + model.CardBack + " to " + "tempdeckname";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["message"] = "Adding card " + model.CardBack + " was unsuccessful.";
                    return View(model);
                }
            }
            else
                return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ViewCard(int id)
        {
            string url = "https://localhost:44356/Cards/" + id;

            var response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                CardViewModel model = JsonConvert.DeserializeObject<CardViewModel>(jsonstring.Result);

                if (model.CardFront == null)
                {
                    model.CardFront = "test";
                    model.CardBack = "test";
                }
                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to grab card data";
                return NotFound();
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditCard([Bind("ID")] int id, [Bind("ID,CardFront,CardBack,CardTags,DeckId")] CardViewModel model)
        {
            if (ModelState.IsValid)
            {
                string url = "https://localhost:44356/Cards/" + id;

                var response = await client.PutAsJsonAsync(url, model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["message"] = model.CardBack + " updated.";
                    return View("ViewCard", model);
                }
                else
                {
                    TempData["message"] = "Updating card " + model.CardBack + " was unsuccessful.";
                    TempData["edit"] = true;
                    return NoContent();
                }
            }
            else
            {
                TempData["edit"] = true;
                return NoContent();
            }
        }
    }
}