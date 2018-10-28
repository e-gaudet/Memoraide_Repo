using System.Linq;
using System.Threading.Tasks;
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