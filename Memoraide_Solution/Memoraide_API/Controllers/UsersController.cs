using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Memoraide_API.Models;
using System.Collections.Generic;
using System.Security;
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
                await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spUpdateUser {0}, {1}, {2}, {3}, {4}", id, user.FirstName, user.LastName, user.Username, user.EmailAddress);
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
            String hashPassword = System.Text.Encoding.ASCII.GetString(data);

            await _context.Database.ExecuteSqlCommandAsync("EXEC dbo.spAddUser {0}, {1}, {2}, {3}, {4}", user.FirstName, user.LastName, user.Username, user.EmailAddress, hashPassword);

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }
    }
}