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
    public class CardsController : ControllerBase
    {
        private readonly DataContext _context;

        public CardsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Cards
        [HttpGet]
        public async Task<IActionResult> GetCard()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var card = _context.Card.FromSql("SELECT * FROM dbo.Cards");
            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);
        }

        // GET: api/Cards/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCard([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var card = _context.Card.FromSql("EXEC dbo.spGetCard {0}", id).First();
            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);
        }

        // PUT: api/Cards/5
        [HttpPut]
        public async Task<IActionResult> PutCard([FromBody] Card card)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Card.FromSql("EXEC dbo.spAddCard {0}, {1}, {2}", card.DeckId, card.Question, card.Answer);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Cards
        [HttpPost]
        public async Task<IActionResult> PostCard([FromBody] Card card)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Card.FromSql("EXEC dbo.spAddCard {0}", card);
            //_context.Card.Add(card);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCard", new { id = card.Id }, card);
        }

        // DELETE: api/Cards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var card = await _context.Card.FindAsync(id);
            var card = _context.Card.FromSql("EXEC dbo.spGetCard {0}", id);
            if (card == null)
            {
                return NotFound();
            }

            //_context.Card.Remove(card);
            _context.Card.FromSql("EXEC dbo.spDeleteCard {0}", id);
            await _context.SaveChangesAsync();

            return Ok(card);
        }

        private bool CardExists(int id)
        {
            return _context.Card.Any(e => e.Id == id);
        }
    }
}