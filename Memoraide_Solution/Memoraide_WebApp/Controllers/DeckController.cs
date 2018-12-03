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


        public async Task<IActionResult> ViewDeckDetail(int? id)
        {

            string url = "https://localhost:44356/Decks/" + id;

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

            // Cards /{ deckid}
            string url = "https://localhost:44356/Decks/Cards/" + deckid;

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();
                List<CardViewModel> model = JsonConvert.DeserializeObject<List<CardViewModel>>(jsonstring.Result);

                //viewBag values for doing an initial review / test
                ViewBag.correctAnswer = 0;
                ViewBag.incorrectAnswer = 0;
                ViewBag.cardQuestions = model.Count;
                List<string> answerList = new List<string>();
                List<int> cardIdxList = new List<int>();

                foreach(CardViewModel card in model) {
                    cardIdxList.Add(card.ID);
                }

                ViewBag.answer_list = answerList;
                ViewBag.card_idx_list = cardIdxList;

                //pass deck information via viewBag
                ViewBag.deckName = "";
                string url2 = "https://localhost:44356/Decks/" + deckid;
                var response2 = await client.GetAsync(url2);
                if (response2.IsSuccessStatusCode)
                {
                    var jsonstring2 = response2.Content.ReadAsStringAsync();
                    jsonstring2.Wait();
                    DeckViewModel deckmodel = JsonConvert.DeserializeObject<DeckViewModel>(jsonstring2.Result);

                    ViewBag.deck_name = deckmodel.Name;
                       //other meta information passing can go here.
                }
                else
                {
                    TempData["message"] = "Unable to grab Deck information.";
                    //return NotFound();
                }


               

                return View(model);
            }
            else
            {
                TempData["message"] = "Unable to grab card data";
                return NotFound();
            }


            return View();
            // return View("testing");


            return NotFound();
        }




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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubscribeToDeck(int id, [Bind("ID", "Name", "UserId")] DeckViewModel model)
        {
            string url = "https://localhost:44356/Decks/" + User.FindFirst("UserId").Value + ";" + id;

            var response = await client.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                TempData["message"] = String.Format("User \"{0}\" Subscribed to deck \"{1}\"", User.FindFirst(ClaimTypes.Name).Value, model.Name);

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


        [HttpGet]
        public async  Task<JsonResult> CheckAnswer(string userinput, string questionid)
        {
            string userinput_raw = userinput;
            string questionid_raw = questionid;

            string cardAnswer = "";

            //this should work everytime.
            int card_id;
            Int32.TryParse(questionid_raw, out card_id);

            ///GET CARD DATA
            string url = "https://localhost:44356/Cards/" + card_id;

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonstring = response.Content.ReadAsStringAsync();
                jsonstring.Wait();

                CardViewModel model = JsonConvert.DeserializeObject<CardViewModel>(jsonstring.Result);

                cardAnswer = model.Answer;
                
            }
            else
            {
                TempData["message"] = "Unable to grab card data";
                return null;
            }

            string resultString;


            if (cardAnswer.Equals(userinput_raw))
            {
                resultString = "User entry is correct!";
            }
             else
            {
                resultString = "INCORRECT! answer: " + cardAnswer + " User answer: " + userinput_raw;
            }



            var jsonblob = new { row = userinput, controllerdata = questionid, result = resultString };
            return Json(jsonblob);
        }


    }   
}