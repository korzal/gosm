﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GOSM.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GOSM.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class RelevantGamesController : ControllerBase
    {
        private readonly Database _context;

        public RelevantGamesController(Database context)
        {
            _context = context;
        }

        // GET: api/RelevantGames
        /// <summary>
        /// Returns a list of games
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns game list</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RelevantGames>>> GetRelevantGamesTable()
        {
            //var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);
            //var sth = User.Identity.IsAuthenticated;
            //if (username != "admin")
            //{
            //    return Unauthorized("Only the admin may add relevant games to the list.");
            //}
            return await _context.RelevantGamesTable
                //.Include(u => u.UserRelevantGamesList)
                .ToListAsync();
        }

        // GET: api/RelevantGames/5
        /// <summary>
        /// Returns a game with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Successfully edited, not returning anything</response>
        /// <response code="404">If game does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RelevantGames>> GetRelevantGames(int id)
        {
            var relevantGames = await _context.RelevantGamesTable.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
            //if(relevantGames != null)
            //{
            //    _context.Entry(relevantGames).State = EntityState.Detached;
            //}

            if (relevantGames == null)
            {
                return NotFound();
            }

            return relevantGames;
        }

        // PUT: api/RelevantGames/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Edits a game's information with the specified ID
        /// </summary>
        /// <param name="relevantGames"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">Successfully edited, not returning anything</response>
        /// <response code="400">If any required fields are null</response>
        /// <response code="404">If specified game id does not exist</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutRelevantGames(int id, RelevantGames relevantGames)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            if (username != "admin")
            {
                return Unauthorized("Only the admin may edit the relevant games list.");
            }

            if (id != relevantGames.ID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object");
            }

            var queryExisting = _context.RelevantGamesTable
                .Where(g => EF.Functions.Like(g.Title, relevantGames.Title)).FirstOrDefault();

            if (queryExisting != null && queryExisting.Title != relevantGames.Title)
            {
                return Conflict("The title already exists.");
            }
            if(queryExisting != null)
            {
                _context.Entry(queryExisting).State = EntityState.Detached;
            }
            if (relevantGames != null)
            {
                _context.Entry(relevantGames).State = EntityState.Detached;
            }

            _context.Entry(relevantGames).State = EntityState.Modified;
            //_context.RelevantGamesTable.Update(relevantGames);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RelevantGamesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RelevantGames
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Adds a new game to the table
        /// </summary>
        /// <param name="relevantGames"></param>
        /// <returns></returns>
        /// <response code="201">If a new game is added successfully</response>
        /// <response code="400">If all required fields are not filled, or non 0 relevant games ID is provided</response>
        /// <response code="409">If the client is attempting to post a duplicate entry</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RelevantGames>> PostRelevantGames(RelevantGames relevantGames)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            if (username != "admin")
            {
                return Unauthorized("Only the admin may add relevant games to the list.");
            }

            if (relevantGames.ID != 0)
            {
                return BadRequest("relevantGames ID should not be provided or left at 0, as it is managed by the database.");
            }
            var queryExisting = _context.RelevantGamesTable
                .Where(g => EF.Functions.Like(g.Title, relevantGames.Title)).FirstOrDefault();

            if(queryExisting != null)
            {
                return Conflict("The title already exists.");
            }

            _context.RelevantGamesTable.Add(relevantGames);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRelevantGames", new { id = relevantGames.ID }, relevantGames);
        }

        // DELETE: api/RelevantGames/5
        /// <summary>
        /// Deletes a user with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">If a game is removed from the table successfully</response>
        /// <response code="404">If a game with the specified id is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RelevantGames>> DeleteRelevantGames(int id)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            if (username != "admin")
            {
                return Unauthorized("Only the admin may delete relevant games from the list.");
            }

            var relevantGames = await _context.RelevantGamesTable.FindAsync(id);
            if (relevantGames == null)
            {
                return NotFound();
            }

            _context.RelevantGamesTable.Remove(relevantGames);
            await _context.SaveChangesAsync();

            return Ok(relevantGames);
        }

        private bool RelevantGamesExists(int id)
        {
            return _context.RelevantGamesTable.Any(e => e.ID == id);
        }

        private string GetUsernameFromClaims(ClaimsIdentity claimsIdentity)
        {
            var claims = claimsIdentity.Claims;
            var usernameClaim = claims.Where(c => c.Type == "Username").FirstOrDefault();
            if (usernameClaim == null)
            {
                throw new NullReferenceException("Authorized user provided no valid username claim. Definitely internal error.");
            }
            return usernameClaim.Value;
        }
    }
}
