using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Memoraide_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Memoraide_WebApp.Controllers
{
    [Authorize]
    public class DeckController : Controller
    {
        HttpClient client;
        public DeckController()
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
        public async Task<IActionResult> Create([Bind("Name,UserId")] DeckViewModel model)
        {
            if (ModelState.IsValid)
            {
                string url = "https://localhost:44356/Decks/";

                var response = await client.PostAsJsonAsync(url, model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["message"] = "Successfully created " +  model.Name;
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["message"] = "Adding deck " + model.Name + " was unsuccessful.";
                    return View(model);
                }
            }
            else
                return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ViewDeck()
        {
            string url = "https://localhost:44356/Decks/";

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                List<DeckViewModel> model = JsonConvert.DeserializeObject<List<DeckViewModel>>(jsonstring.Result);

                 if (model[0].Name == null)
                 {
                     model[0].Name = "test";
                 }
                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to grab deck data";
                return NotFound();
            }
        }

        //[HttpPut]
        //public async Task<IActionResult> EditDeck([Bind("ID")] int id, [Bind("ID,DeckName")] DeckViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string url = "https://localhost:44356/Decks/" + id;

        //        var response = await client.PutAsJsonAsync(url, model);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            TempData["message"] = model.Name + " updated.";
        //            return View("ViewDeck", model);
        //        }
        //        else
        //        {
        //            TempData["message"] = "Updating deck " + model.Name + " was unsuccessful.";
        //            TempData["edit"] = true;
        //            return NoContent();
        //        }
        //    }
        //    else
        //    {
        //        TempData["edit"] = true;
        //        return NoContent();
        //    }
        //}


        //[HttpGet] //This needs to go for routing a post!
        public async Task<IActionResult> ViewDeckDetail(int? id)
        {

            string url = "https://localhost:44356/DEcks/" + id;

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                DeckViewModel model = JsonConvert.DeserializeObject<DeckViewModel>(jsonstring.Result);
                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to grab card data";
                return NotFound();
            }


        }

        //todo: non-retarded way of getting all cards in deck.
        public async Task<IActionResult> ReviewDeck(int? deckid)
        {
            return View();
            // return View("testing");

            //testing to get to other page.
            string url = "https://localhost:44356/DEcks/" + deckid;

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                DeckViewModel model = JsonConvert.DeserializeObject<DeckViewModel>(jsonstring.Result);
                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to grab card data";
                return NotFound();
            }




            return NotFound();
        }

        //testing: jump to deck detail outside of the deck page.
        //[HttpGet]
        //public async Task<IActionResult> ViewDeck_JumpToDetail(int? id)
        //{
        //    string url = "https://localhost:44356/Decks/";

        //    var response = await client.GetAsync(url);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var jsonstring = response.Content.ReadAsStringAsync();
        //        jsonstring.Wait();
        //        List<DeckViewModel> model = JsonConvert.DeserializeObject<List<DeckViewModel>>(jsonstring.Result);

        //        if (model[0].Name == null)
        //        {
        //            model[0].Name = "test";
        //        }
        //        await ViewDeckDetail(id);
        //        return View(model);
        //    }
        //    else
        //    {
        //        TempData["message"] = "Unable to grab deck data";
        //        return NotFound();
        //    }
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewDeckDetail([Bind("ID")] int id, [Bind("ID", "Name", "UserId")] DeckViewModel model)
        {

            if (ModelState.IsValid)
            {
                string url = "https://localhost:44356/Decks/" + id;

                //if no {get; set;} on model ID, this is needed.
                model.ID = id;

                var response = await client.PutAsJsonAsync(url, model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["message"] = model.Name + " updated.";

                    return NoContent();
                }
                else
                {
                    TempData["message"] = "Updating deck " + model.Name + " was unsuccessful.";
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