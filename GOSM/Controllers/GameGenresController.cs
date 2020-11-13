using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GOSM.Models;

namespace GOSM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameGenresController : ControllerBase
    {
        private readonly Database _context;

        public GameGenresController(Database context)
        {
            _context = context;
        }

        // GET: api/GameGenres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameGenre>>> GetGameGenreTable()
        {
            return await _context.GameGenreTable.ToListAsync();
        }

        // GET: api/GameGenres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameGenre>> GetGameGenre(int id)
        {
            var gameGenre = await _context.GameGenreTable.FindAsync(id);

            if (gameGenre == null)
            {
                return NotFound();
            }

            return gameGenre;
        }

        // PUT: api/GameGenres/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameGenre(int id, GameGenre gameGenre)
        {
            if (id != gameGenre.ID)
            {
                return BadRequest();
            }

            _context.Entry(gameGenre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameGenreExists(id))
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

        // POST: api/GameGenres
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<GameGenre>> PostGameGenre(GameGenre gameGenre)
        {
            _context.GameGenreTable.Add(gameGenre);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGameGenre", new { id = gameGenre.ID }, gameGenre);
        }

        // DELETE: api/GameGenres/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GameGenre>> DeleteGameGenre(int id)
        {
            var gameGenre = await _context.GameGenreTable.FindAsync(id);
            if (gameGenre == null)
            {
                return NotFound();
            }

            _context.GameGenreTable.Remove(gameGenre);
            await _context.SaveChangesAsync();

            return gameGenre;
        }

        private bool GameGenreExists(int id)
        {
            return _context.GameGenreTable.Any(e => e.ID == id);
        }
    }
}
