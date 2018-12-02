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
using System.Security.Claims;

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
        public async Task<IActionResult> Index()
        {
            string url = "https://localhost:44356/Decks/UserDecks/" + User.FindFirst("UserId").Value;

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                List<DeckViewModel> model = JsonConvert.DeserializeObject<List<DeckViewModel>>(jsonstring.Result);
                return View(model);
            }

            return View(new List<DeckViewModel>());

        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] DeckViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst("UserId").Value;
                int id = Convert.ToInt32(userId);
                model.UserId = id;
                string url = "https://localhost:44356/Decks/";

                var response = await client.PostAsJsonAsync(url, model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["message"] = "Successfully created " + model.Name;
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


        public async Task<IActionResult> ViewDeckDetail(int? id)
        {

            string url = "https://localhost:44356/Decks/" + id;

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                DeckViewModel model = JsonConvert.DeserializeObject<DeckViewModel>(jsonstring.Result);

                string url2 = "https://localhost:44356/Decks/issubbed/" + User.FindFirstValue("UserId") + ";" + model.ID;
                var response2 = await client.GetAsync(url2);

                if (response2.IsSuccessStatusCode)
                {
                    var jsonstring2 = response2.Content.ReadAsStringAsync();
                    jsonstring2.Wait();
                    bool exists = JsonConvert.DeserializeObject<bool>(jsonstring2.Result);

                    model.isSubbed = exists;
                }
                else
                {
                    model.isSubbed = false;
                }
                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to grab deck data";
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
        public async Task<IActionResult> ViewDeckDetail([Bind("ID")] int id, [Bind("ID", "Name", "UserId", "isSubbed")] DeckViewModel model)
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

                    return View(model);
                }
                else
                {
                    TempData["message"] = "Updating deck " + model.Name + " was unsuccessful.";
                    TempData["edit"] = true;
                    return View(model);
                }
            }
            else
            {
                TempData["edit"] = true;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubscribeToDeck(int id, [Bind("ID", "Name", "UserId", "isSubbed")] DeckViewModel model)
        {
            string url = "https://localhost:44356/Decks/" + User.FindFirst("UserId").Value + ";" + id;

            var response = await client.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                TempData["message"] = String.Format("User \"{0}\" Subscribed to deck \"{1}\"", User.FindFirst(ClaimTypes.Name).Value, model.Name);
                model.isSubbed = true;
                return View("ViewDeckDetail", model);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["message"] = "User already subscribed"; //TODO: Make this check bad request properly
                return View("ViewDeckDetail", model);
            }
            else
            {
                TempData["message"] = "Issue subscribing to deck";
                return View("ViewDeckDetail", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnsubscribeFromDeck(int id, [Bind("ID", "Name", "UserId", "isSubbed")] DeckViewModel model)
        {
            string url = "https://localhost:44356/Decks/UserDecks/" + User.FindFirst("UserId").Value + ";" + id;

            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                TempData["message"] = String.Format("User \"{0}\" Unsubscribed from deck \"{1}\"", User.FindFirst(ClaimTypes.Name).Value, model.Name);
                model.isSubbed = false;
                return View("ViewDeckDetail", model);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["message"] = "User is not subscribed"; //TODO: Make this check bad request properly
                return View("ViewDeckDetail", model);
            }
            else
            {
                TempData["message"] = "Issue unsubscribing from deck";
                return View("ViewDeckDetail", model);
            }
        }

    }   
}