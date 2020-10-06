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
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendRequestsController : ControllerBase
    {
        private readonly Database _context;

        public FriendRequestsController(Database context)
        {
            _context = context;
        }

        // GET: api/FriendRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FriendRequest>>> GetFriendRequestTable()
        {
            return await _context.FriendRequestTable.ToListAsync();
        }

        // GET: api/FriendRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FriendRequest>> GetFriendRequest(int id)
        {
            var friendRequest = await _context.FriendRequestTable.FindAsync(id);

            if (friendRequest == null)
            {
                return NotFound();
            }

            return friendRequest;
        }

        // PUT: api/FriendRequests/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFriendRequest(int id, FriendRequest friendRequest)
        {
            if (id != friendRequest.ID)
            {
                return BadRequest();
            }

            _context.Entry(friendRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FriendRequestExists(id))
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

        // POST: api/FriendRequests
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<FriendRequest>> PostFriendRequest(FriendRequest friendRequest)
        {
            _context.FriendRequestTable.Add(friendRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFriendRequest", new { id = friendRequest.ID }, friendRequest);
        }

        // DELETE: api/FriendRequests/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FriendRequest>> DeleteFriendRequest(int id)
        {
            var friendRequest = await _context.FriendRequestTable.FindAsync(id);
            if (friendRequest == null)
            {
                return NotFound();
            }

            _context.FriendRequestTable.Remove(friendRequest);
            await _context.SaveChangesAsync();

            return friendRequest;
        }

        private bool FriendRequestExists(int id)
        {
            return _context.FriendRequestTable.Any(e => e.ID == id);
        }
    }
}
