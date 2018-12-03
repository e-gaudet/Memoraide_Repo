using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Memoraide_API.Models;
using System.Collections.Generic;
using System;

namespace Memoraide_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // GET: /Users
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.User.FromSql("SELECT * FROM dbo.Users");
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // GET/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.User.FromSql("EXEC dbo.spGetUser {0}", id).FirstAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("by_name/{username}")]
        public async Task<IActionResult> GetUser([FromRoute] string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.User.FromSql("EXEC dbo.spGetUserByUsername {0}", username);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: /Users/user
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spUpdateUser {0}, {1}, {2}, {3}, {4}", id, user.FirstName, user.LastName, user.Username, user.Email);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: /Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            byte[] data = System.Text.Encoding.ASCII.GetBytes(user.Password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            string hashPassword = System.Text.Encoding.ASCII.GetString(data);

            await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spAddUser {0}, {1}, {2}, {3}, {4}", user.FirstName, user.LastName, user.Username, user.Email, hashPassword);

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // POST: /Users/Answer
        [HttpPost("Answer")]
        public async Task<IActionResult> PostAnswer([FromBody] CardAnswer carda)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spAddCardAnswer {0}, {1}, {2}, {3}", carda.UserId, carda.CardId, carda.CorrectAnswer, carda.NextReviewDate);

            return CreatedAtAction("GetCardAnswer", new { id = carda.Id }, carda);
        }

        // PUT: /Users/ban/user
        [HttpPut("ban/{id}")]
        public async Task<IActionResult> PutUserBan([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spBanUser {0}", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("Decks/{userId}")]
        public async Task<IActionResult> GetUserDecks([FromRoute] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var decks = _context.Deck.FromSql("SELECT * FROM dbo.Decks WHERE UserId = {0}", userId);

                if (decks != null)
                    return CreatedAtAction("GetUserDecks", decks);
                else
                    return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Login login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = _context.User.FromSql("EXEC dbo.spGetUserByUsername {0}", login.Username).SingleOrDefaultAsync().Result;

            byte[] data = System.Text.Encoding.ASCII.GetBytes(login.Password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            string hashPassword = System.Text.Encoding.ASCII.GetString(data);

            if(user != null && hashPassword.Equals(user.Password))
            {
                return Ok(user);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            User user = _context.User.FromSql("EXEC dbo.spGetUserByUsername {0}", register.Username).SingleOrDefaultAsync().Result;

            if (user != null)
            {
                return Ok(new RegisterResponse { success = false, errorcode = 1, errormessage = "User Already Exists" });
            }
            try
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(register.Password);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                string hashPassword = System.Text.Encoding.ASCII.GetString(data);

                await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spAddUser {0}, {1}, {2}, {3}, {4}", "", "", register.Username, "", hashPassword);

                return Ok(new RegisterResponse { success = true});
            }
            catch
            {
                return Ok(new RegisterResponse { success = false, errorcode = -1, errormessage = "Unexpected exception" });
            }
        }
    }
}