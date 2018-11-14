﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Memoraide_API.Models;
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

            var user = _context.User.FromSql("EXEC dbo.spGetUser {0}", id);
            if(user == null)
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
    }
}