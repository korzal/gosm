using System;
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
    [Route("api/Users/{UserId}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class FriendRequestsController : ControllerBase
    {
        private readonly Database _context;

        public FriendRequestsController(Database context)
        {
            _context = context;
        }

        // GET: api/Users/{UserId}/FriendRequests
        /// <summary>
        /// Returns a list of active friend requests(not accepted ones) received by a user specified by ID
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns user list</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<FriendRequest>>> GetFriendRequestTable(int UserId)
        {

            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            var friendRequest = (from u in _context.FriendRequestTable
                                 where u.SenderID == UserId || u.RecipientID == UserId
                                 select u).FirstOrDefault();

            if (friendRequest.Sender.Username != username && friendRequest.Recipient.Username != username && username != "admin")
            {
                return Unauthorized("You may only view friend requests you are the sender or recipient of.");
            }

            return await _context.FriendRequestTable
                .Where(u => u.RecipientID == UserId)
                .Where(u => u.IsAccepted == false)
                //.Include(u1 => u1.Recipient)
                .Include(u2 => u2.Sender)
                .ToListAsync();
        }

        // GET: api/Users/{UserId}/FriendRequests/FriendList
        /// <summary>
        /// Returns a list of accepted friend requests of a user specified by ID
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns user list</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpGet, Route("FriendList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<FriendRequest>>> GetFriendRequestList(int UserId)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            var friendRequest = (from u in _context.FriendRequestTable
                                 where u.SenderID == UserId || u.RecipientID == UserId
                                 select u).FirstOrDefault();

            if (friendRequest.Sender.Username != username && friendRequest.Recipient.Username != username && username != "admin")
            {
                return Unauthorized("You may only view friend requests you are the sender or recipient of.");
            }

            return await _context.FriendRequestTable
                .Where(u => u.RecipientID == UserId || u.SenderID == UserId)
                .Where(u => u.IsAccepted == true)
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
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<FriendRequest>> GetFriendRequest(int id)
        {
            var friendRequest = await _context.FriendRequestTable.FindAsync(id);

            if (friendRequest == null)
            {
                return NotFound();
            }

            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            if (friendRequest.Sender.Username != username && friendRequest.Recipient.Username != username && username != "admin")
            {
                return Unauthorized("You may only view friend requests you are the sender or recipient of.");
            }

            return friendRequest;
        }

        // PUT: api/Users/{UserId}/FriendRequests/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Lets user accept a friend request (changing the state of isAccepted)
        /// </summary>
        /// <param name="friendRequest"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">Successfully edited, not returning anything</response>
        /// <response code="400">If any required fields are null</response>
        /// <response code="404">If specified friend request id does not exist</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutFriendRequest(int id, FriendRequest friendRequest)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            if (friendRequest.Sender.Username != username && friendRequest.Recipient.Username != username && username != "admin")
            {
                return Unauthorized("You may only edit friend requests you are the sender or recipient of.");
            }

            if (id != friendRequest.ID)
            {
                return BadRequest();
            }
            friendRequest.Recipient = (from f in _context.FriendRequestTable
                                       where f.ID == friendRequest.ID
                                       select f.Recipient).FirstOrDefault();

            friendRequest.Sender = (from f in _context.FriendRequestTable
                                       where f.ID == friendRequest.ID
                                       select f.Sender).FirstOrDefault();

            friendRequest.RequestDate = (from f in _context.FriendRequestTable
                                    where f.ID == friendRequest.ID
                                    select f.RequestDate).FirstOrDefault();

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
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<FriendRequest>> PostFriendRequest(int UserId, FriendRequest friendRequest)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);
            var requestingUser = (from u in _context.UserTable
                                  where u.Username == username
                                  select u).FirstOrDefault();

            if(friendRequest.Sender.Username != requestingUser.Username && friendRequest.Sender.Username != "admin")
            {
                Unauthorized("You may only post friend requests when you are the sender.");
            }

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
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
             

            friendRequest.IsAccepted = false;
            friendRequest.RequestDate = DateTime.Now;
            friendRequest.Sender = requestingUser;
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
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<FriendRequest>> DeleteFriendRequest(int id)
        {
            var friendRequest = await _context.FriendRequestTable.FindAsync(id);
            if (friendRequest == null)
            {
                return NotFound("Friend request of specified ID could not be found.");
            }

            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            if (friendRequest.Sender.Username != username && friendRequest.Recipient.Username != username && username != "admin")
            {
                return Unauthorized("You may only delete friend requests you are the sender or recipient of.");
            }

            

            _context.FriendRequestTable.Remove(friendRequest);
            await _context.SaveChangesAsync();

            return Ok(friendRequest);
        }

        private bool FriendRequestExists(int id)
        {
            return _context.FriendRequestTable.Any(e => e.ID == id);
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
