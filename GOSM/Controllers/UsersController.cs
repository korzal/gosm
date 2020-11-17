using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GOSM.Models;
using GOSM.Controllers;
using GOSM.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GOSM.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly Database _context;
        //private IUserService _userService;

        public UsersController(Database context)//IUserService userService, Database context)
        {
            //_userService = userService;
            _context = context;
        }


        // GET: api/Users
        /// <summary>
        /// Returns a list of users
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns user list</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.UserTable.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.UserRelevantGamesList = _context.UserTable
                                .SelectMany(g => g.UserRelevantGamesList)
                                .Include(ge => ge.RelevantGames)
                                .ToList();
            
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
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            //var checkUser = await _context.UserTable.FindAsync(user.ID);
            if (id != user.ID)
            {
                return BadRequest("ID provided as parameter does not match the serialized object.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object.");
            }

            if (username != user.Username && username != "admin")
            {
                return Unauthorized("Only the user can edit his own info.");
            }

            var queryExisting = _context.UserTable
                .Where(u => EF.Functions.Like(u.Username, user.Username)).FirstOrDefault();

            if(queryExisting != null && queryExisting.Username != user.Username)
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
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        // POST: api/Users/{id}/AddRelevantGame
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Adds a relevant game to user specified by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relevantGame"></param>
        /// <returns></returns>
        /// <response code="201">If a relevant game is added successfully</response>
        /// <response code="400">If all required fields are not filled</response>
        /// <response code="409">If relevant game is already added to provided account</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpPost, Route("{id}/AddRelevantGame")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserRelevantGames>> PostRelevantGame(int id, UserRelevantGames relevantGame)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            var checkUser = await _context.UserTable.FindAsync(id);
            if (checkUser == null)
            {
                return NotFound("User with specified UserId was not found.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object.");
            }

            if (username != checkUser.Username && username != "admin")
            {
                return Unauthorized("Only the user can add his own relevant games.");
            }

            var queryExisting = _context.UserRelevantGamesTable
                .Where(u => u.RelevantGames == relevantGame.RelevantGames).FirstOrDefault();

            if (queryExisting != null)
            {
                return Conflict("Relevant game is already added to this account.");
            }

            _context.UserRelevantGamesTable.Add(relevantGame);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRelevantGame", relevantGame);

        }

        // DELETE: api/Users/{id}/DeleteRelevantGame/{gameId}
        /// <summary>
        /// Deletes a relevant game specified by gameId from a user specified by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="gameId"></param>
        /// <returns></returns>
        /// <response code="200">If an account is deleted successfully</response>
        /// <response code="404">If an account with the specified id is not found</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpDelete, Route("{id}/DeleteRelevantGame/{gameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserRelevantGames>> DeleteRelevantGame(int id, int gameId)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);
            
            var user = await _context.UserTable.FindAsync(id);
            if (user == null)
            {
                return NotFound("User with the specified id does not exist.");
            }

            if (username != user.Username && username != "admin")
            {
                return Unauthorized("Only the user can delete his own relevant games.");
            }

            var game = (from g in _context.UserRelevantGamesTable
                        where g.UserID == id && g.RelevantGamesID == gameId
                        select g).FirstOrDefault();

            if(game == null)
            {
                return NotFound("Game with the specified gameId does not belong to this user.");
            }

            _context.UserRelevantGamesTable.Remove(game);
            await _context.SaveChangesAsync();

            return Ok(game);
        }

        // DELETE: api/Users/5
        /// <summary>
        /// Deletes a user with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">If an account is deleted successfully</response>
        /// <response code="404">If an account with the specified id is not found</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);

            var user = await _context.UserTable.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (username != "admin" && username != user.Username)
            {
                return Unauthorized("Only the admin and the user can delete his own account.");
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
