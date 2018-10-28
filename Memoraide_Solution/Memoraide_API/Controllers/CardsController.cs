using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Memoraide_API.Models;
using System.Collections.Generic;

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

        // GET: /Cards
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

        // GET: /Cards/5
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

        // GET: /Cards/Tag 5,Tag 2
        [HttpGet("search/{tags}")]
        public async Task<IActionResult> GetCard([FromRoute] string tags)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string query = "SELECT c.* FROM Cards c INNER JOIN CardTags ct ON c.Id = ct.CardId WHERE";
            List<string> tagsList = tags.Split(',').ToList<string>(); ;


            foreach (string tag in tagsList)
            {
                if (tag == tagsList[0])
                    query += " ct.Tag = '" + tag + "'";
                else
                    query += " OR ct.Tag = '" + tag + "'";
            }

            if (tagsList.Count() > 1)
            {
                query += " GROUP BY c.Id, c.DeckId, c.Question, c.Answer, c.DateCreated, c.DateDeleted, c.IsDeleted HAVING Count(c.Id) > 1";
            }

            var cards = _context.Card.FromSql(query);
            if (cards == null)
            {
                return NotFound();
            }

            return Ok(cards);
        }

        // PUT: /Cards/card
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCard([FromRoute] int id,[FromBody] Card card)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spUpdateCard {0}, {1}, {2}, {3}", id, card.DeckId, card.Question, card.Answer);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: /Cards
        [HttpPost]
        public async Task<IActionResult> PostCard([FromBody] Card card)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spAddCard {0}, {1}, {2}", card.DeckId, card.Question, card.Answer);

            return CreatedAtAction("GetCard", new { id = card.Id }, card);
        }

        // DELETE: /Cards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
             
            var card = _context.Card.FromSql("EXEC dbo.spGetCard {0}", id);
            if (card == null)
            {
                return NotFound();
            }

            await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spDeleteCard {0}", id);

            return Ok(card);
        }

        private bool CardExists(int id)
        {
            return _context.Card.Any(e => e.Id == id);
        }
    }
}