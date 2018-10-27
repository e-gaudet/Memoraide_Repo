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

        // PUT: api/Cards/card
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCard([FromRoute] int id,[FromBody] Card card)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                _context.Card.FromSql("EXEC dbo.spUpdateCard {0}, {1}, {2}, {3}, {4}", id, card.DeckId, card.Question, card.Answer, card.IsDeleted);

                //await _context.SaveChangesAsync();
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
            var res =_context.Card.FromSql("EXEC dbo.spAddCard {0}, {1}, {2}", card.DeckId, card.Question, card.Answer);

            //await _context.SaveChangesAsync();

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
            
            var card = _context.Card.FromSql("EXEC dbo.spGetCard {0}", id);
            if (card == null)
            {
                return NotFound();
            }
            
            _context.Card.FromSql("EXEC dbo.spDeleteCard {0}", id);
            //await _context.SaveChangesAsync();

            return Ok(card);
        }

        private bool CardExists(int id)
        {
            return _context.Card.Any(e => e.Id == id);
        }
    }
}