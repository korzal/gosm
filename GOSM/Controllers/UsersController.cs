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

namespace GOSM.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly Database _context;
        private IUserService _userService;

        public UsersController(IUserService userService, Database context)
        {
            _userService = userService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model, ipAddress());

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            setTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshToken(refreshToken, ipAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            setTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = _userService.RevokeToken(token, ipAddress());

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
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

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _context.UserTable.Find(id);
            if (user == null) return NotFound();

            return Ok(user.RefreshTokens);
        }

        // helper methods

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
