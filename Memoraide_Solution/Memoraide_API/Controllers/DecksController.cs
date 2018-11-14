using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Memoraide_API.Models;

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

            await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spSubscribeToDeck {0}, {1}", userid, deckid);

            return NoContent();
        }

        private bool DeckExists(int id)
        {
            return _context.Deck.Any(e => e.Id == id);
        }
    }
}