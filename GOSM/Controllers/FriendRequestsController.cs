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
    [Route("api/Users/{UserId}/[controller]")]
    [ApiController]
    public class FriendRequestsController : ControllerBase
    {
        private readonly Database _context;

        public FriendRequestsController(Database context)
        {
            _context = context;
        }

        // GET: api/Users/{UserId}/FriendRequests
        /// <summary>
        /// Returns a list of friend requests received by a user specified by ID
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns user list</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FriendRequest>>> GetFriendRequestTable(int UserId)
        {
            return await _context.FriendRequestTable
                .Where(u => u.RecipientID == UserId)
                //.Include(u1 => u1.Recipient)
                .Include(u2 => u2.Sender)
                .ToListAsync();
        }

        // GET: api/Users/{UserId}/FriendRequests/5
        /// <summary>
        /// Returns a friend request specified by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Success, not returning anything</response>
        /// <response code="404">If user does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FriendRequest>> GetFriendRequest(int id)
        {
            var friendRequest = await _context.FriendRequestTable.FindAsync(id);

            if (friendRequest == null)
            {
                return NotFound();
            }

            return friendRequest;
        }

        // PUT: api/Users/{UserId}/FriendRequests/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// (possibly unnecessary)
        /// </summary>
        /// <param name="friendRequest"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">Successfully edited, not returning anything</response>
        /// <response code="400">If any required fields are null</response>
        /// <response code="404">If specified friend request id does not exist</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        // POST: api/Users/{UserId}/FriendRequests
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Sends a new friend request
        /// </summary>
        /// <param name="friendRequest"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <response code="200">If a friend request had already been sent by the recipient</response>
        /// <response code="201">If a friend request is sent successfully</response>
        /// <response code="400">If all required fields are not filled, or non 0 ID for friend request is provided</response>
        /// <response code="404">If sender or recipient of provided IDs do not exist</response>
        /// <response code="409">If a friend request had already been sent to the same user by the sender</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<FriendRequest>> PostFriendRequest(int UserId, FriendRequest friendRequest)
        {
            if (friendRequest.ID != 0)
            {
                return BadRequest("Friend request ID should not be provided or left at 0, as it is managed by the database.");
            }

            var localSender = await _context.UserTable.FindAsync(UserId);
            if (localSender != null)
            {
                _context.Entry(localSender).State = EntityState.Detached;
            }
            else
            {
                return NotFound("Sender with the provided ID does not exist.");
            }

            var localReceiver = await _context.UserTable.FindAsync(friendRequest.RecipientID);
            if(localReceiver != null)
            {
                _context.Entry(localReceiver).State = EntityState.Detached;
            }
            else
            {
                return NotFound("Recipient with the provided ID does not exist.");
            }

            if(localSender.ID == localReceiver.ID)
            {
                return Conflict("Cannot send friend request to self.");
            }

            var compareRequest = (from f in _context.FriendRequestTable
                                    where f.SenderID == localSender.ID
                                    where f.RecipientID == localReceiver.ID
                                    select f).FirstOrDefault();

            if(compareRequest != null)
            {
                return Conflict("Friend request has already been sent to specified user.");
            }
            else
            {
                compareRequest = (from f in _context.FriendRequestTable
                                  where f.SenderID == localReceiver.ID
                                  where f.RecipientID == localSender.ID
                                  select f).FirstOrDefault();

                if(compareRequest != null)
                {
                    _context.FriendRequestTable.Find(compareRequest.ID).IsAccepted = true;
                    return Ok();
                }
            }
             

            friendRequest.IsAccepted = false;
            friendRequest.RequestDate = DateTime.Now;
            friendRequest.SenderID = UserId;
            _context.FriendRequestTable.Add(friendRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFriendRequest", new { id = friendRequest.ID }, friendRequest);
        }

        // DELETE: api/Users/{UserId}/FriendRequests/5
        /// <summary>
        /// Deletes a friend request with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">If a friend request is deleted successfully</response>
        /// <response code="404">If a friend request with the specified id is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FriendRequest>> DeleteFriendRequest(int id)
        {
            var friendRequest = await _context.FriendRequestTable.FindAsync(id);
            if (friendRequest == null)
            {
                return NotFound("Friend request of specified ID could not be found.");
            }

            _context.FriendRequestTable.Remove(friendRequest);
            await _context.SaveChangesAsync();

            return Ok(friendRequest);
        }

        private bool FriendRequestExists(int id)
        {
            return _context.FriendRequestTable.Any(e => e.ID == id);
        }
    }
}
