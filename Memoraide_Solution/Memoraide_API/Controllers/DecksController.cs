using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Memoraide_API.Models;
using System.Data.SqlClient;

namespace Memoraide_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly DataContext _context;

        public DecksController(DataContext context)
        {
            _context = context;
        }

        // GET: /Decks
        [HttpGet]
        public async Task<IActionResult> GetDeck()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deck = _context.Deck.FromSql("SELECT * FROM dbo.Decks");
            if (deck == null)
            {
                return NotFound();
            }

            return Ok(deck);
        }

        // GET: /Decks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeck([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deck = _context.Deck.FromSql("EXEC dbo.spGetDeck {0}", id).First();

            if (deck == null)
            {
                return NotFound();
            }

            return Ok(deck);
        }

        // PUT: /Decks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeck([FromRoute] int id, [FromBody] Deck deck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spUpdateDeck {0}, {1}, {2}", id, deck.Name, deck.UserId);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: /Decks
        [HttpPost]
        public async Task<IActionResult> PostDeck([FromBody] Deck deck)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spAddDeck {0}, {1}",  deck.Name, deck.UserId);

            return CreatedAtAction("GetDeck", new { id = deck.Id }, deck);
        }

        // DELETE: /Decks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeck([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deck = _context.Card.FromSql("EXEC dbo.spGetDeck {0}", id);

            if (deck == null)
            {
                return NotFound();
            }

            await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spDeleteDeck {0}", id);

            return Ok(deck);
        }

        //POST: /Decks/1;1
        [HttpPost("{userid};{deckid}")]
        public async Task<IActionResult> PostSubscribeToDeck([FromRoute] int userid, [FromRoute] int deckid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                bool exists;
                using (SqlConnection connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    SqlCommand command = new SqlCommand(String.Format("select count(*) from dbo.userdecks where userid = {0} and deckid = {1}", userid, deckid), connection);
                    connection.Open();
                    exists = (int)command.ExecuteScalar() != 0;
                }

                if (!exists)
                {
                    await _context.Database.ExecuteSqlCommandAsync("INSERT INTO dbo.UserDecks (UserId, DeckId) VALUES ({0}, {1})", userid, deckid);
                    return Ok();
                }
                else
                {
                    return BadRequest("User Already Subscribed");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Server Error");
            }
        }

        // POST: /Decks/rating/1
        [HttpPost("rating/{id}")]
        public async Task<IActionResult> PostDeckRating([FromRoute] int id, [FromBody] int rating)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spAddDeckRating {0}, {1}", id, rating);

            return Ok();
        }

        private bool DeckExists(int id)
        {
            return _context.Deck.Any(e => e.Id == id);
        }

        [HttpGet("UserDecks/{userId}")]
        public async Task<IActionResult> GetUserDecks([FromRoute] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                List<Deck> decks = _context.Deck.FromSql("select d.* from dbo.decks d inner join dbo.UserDecks ud on d.id = ud.deckId where ud.userId = {0}", userId).ToList();

                if (decks == null)
                {
                    return NotFound();
                }

                return Ok(decks);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("Cards/{deckid}")]
        public async Task<IActionResult> GetCardsByDeckId([FromRoute] int deckid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cards = _context.Card.FromSql("SELECT * FROM dbo.Cards WHERE DeckId = {0}", deckid);

            if (cards == null)
                return NotFound();

            return Ok(cards);
        }


        [HttpDelete("UserDecks/{userid};{deckid}")]
        public async Task<IActionResult> DeleteUserDeck([FromRoute] int userid, [FromRoute] int deckid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Database.ExecuteSqlCommandAsync("DELETE FROM dbo.UserDecks WHERE UserId = {0} AND DeckId = {1}", userid, deckid);

            return Ok();
        }
    }
}