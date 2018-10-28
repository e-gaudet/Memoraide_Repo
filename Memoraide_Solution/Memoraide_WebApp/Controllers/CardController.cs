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
        public async Task<IActionResult> Create([Bind("Question,Answer,DeckId")] CardViewModel model)
        {
            if (ModelState.IsValid)
            {
                string url = "https://localhost:44356/Cards/";

                var response = await client.PostAsJsonAsync(url, model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["message"] = "Successfully added " + model.Answer + " to " + "tempdeckname";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["message"] = "Adding card " + model.Answer + " was unsuccessful.";
                    return View(model);
                }
            }
            else
                return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ViewCard()
        {
            string url = "https://localhost:44356/Cards/";

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                List<CardViewModel> model = JsonConvert.DeserializeObject<List<CardViewModel>>(jsonstring.Result);

                //if (model.Question == null)
                // {
                //     model.Question = "test";
                //     model.Answer = "test";
                // }
                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to grab card data";
                return NotFound();
            }
        }

        //[HttpGet] //This needs to go for routing a post!
        public async Task<IActionResult> ViewCardDetail(int? id)
        {
            string url = "https://localhost:44356/Cards/" + id;

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                CardViewModel model = JsonConvert.DeserializeObject<CardViewModel>(jsonstring.Result);

                if (model.Question == null)
                {
                    model.Question = "test";
                    model.Answer = "test";
                }
                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to grab card data";
                return NotFound();
            }
        }

        //RENAMED from EditCard. viewCardDetail's post will handle the edit. none of the code was changed my me.
        //[HttpPut, Route("{id,:int}")]
        //[HttpPut]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewCardDetail([Bind("ID")] int id, [Bind("ID,Question,Answer,CardTags,DeckId")] CardViewModel model)
        {
            if (ModelState.IsValid)
            {
                string url = "https://localhost:44356/Cards/" + id;

                var response = await client.PutAsJsonAsync(url, model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["message"] = model.Answer + " updated.";
                    return View("ViewCard", model);
                }
                else
                {
                    TempData["message"] = "Updating card " + model.Answer + " was unsuccessful.";
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