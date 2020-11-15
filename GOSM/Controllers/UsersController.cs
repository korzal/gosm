using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GOSM.Models;
using GOSM.Controllers;

namespace GOSM.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Database _context;

        public UsersController(Database context)
        {
            _context = context;
        }

        // GET: api/Users
        /// <summary>
        /// Returns a list of users
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns user list</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetUserTable()
        {
            return await _context.UserTable
                .Include(g => g.UserRelevantGamesList)
                .ThenInclude(ge => ge.RelevantGames)
                //.ThenInclude(ge => ge.GameGenre)
                .ToListAsync();
        }

        // GET: api/Users/5
        /// <summary>
        /// Returns a user with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Success, return requested user</response>
        /// <response code="404">If user does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.UserTable.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.UserRelevantGamesList = await _context.UserTable
                .SelectMany(g => g.UserRelevantGamesList)
                .Include(ge => ge.RelevantGames)
                .ToListAsync();

            return Ok(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Edits a user's information with the specified ID
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">Successfully edited, not returning anything</response>
        /// <response code="400">If any required fields are null</response>
        /// <response code="404">If specified user id does not exist</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            //var checkUser = await _context.UserTable.FindAsync(user.ID);
            if (id != user.ID)
            {
                return BadRequest("ID provided as parameter does not match the serialized object.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object.");
            }

            var queryExisting = _context.UserTable
                .Where(u => EF.Functions.Like(u.Username, user.Username)).FirstOrDefault();

            if(queryExisting != null)
            {
                return Conflict("Username already exists.");
            }

            user.CreationDate = _context.UserTable
                .Where(u => u.ID == user.ID)
                .Select(u => u.CreationDate).FirstOrDefault();

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <response code="201">If an account is created successfully</response>
        /// <response code="400">If all required fields are not filled, or non 0 user ID is provided</response>
        /// <response code="409">If username is already taken</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (user.ID != 0)
            {
                return BadRequest("User ID should not be provided or left at 0, as it is managed by the database.");
            }
            var queryExisting = _context.UserTable
                .Where(u => EF.Functions.Like(u.Username, user.Username)).FirstOrDefault();

            if(queryExisting != null)
            {
                return Conflict("Username already exists.");
            }

            user.CreationDate = DateTime.Now;
            _context.UserTable.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.ID }, user);
        }

        // DELETE: api/Users/5
        /// <summary>
        /// Deletes a user with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">If an account is deleted successfully</response>
        /// <response code="404">If an account with the specified id is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.UserTable.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            PostsController posts = new PostsController(_context);
            FriendRequestsController friendRequests = new FriendRequestsController(_context);
            
            var relatedPosts = (from p in _context.PostTable
                                where p.UserID == id
                                select p).ToList();

            var relatedFriendRequests = (from r in _context.FriendRequestTable
                                         where r.RecipientID == id || r.SenderID == id
                                         select r).ToList();

            foreach(var post in relatedPosts)
            {
                await posts.DeletePost(post.ID);
            }

            foreach(var request in relatedFriendRequests)
            {
                await friendRequests.DeleteFriendRequest(request.ID);
            }

            _context.UserTable.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(int id)
        {
            return _context.UserTable.Any(e => e.ID == id);
        }
    }
}
