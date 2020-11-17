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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : ControllerBase
    {
        private readonly Database _context;

        public PostsController(Database context)
        {
            _context = context;
        }

        // GET: api/Posts
        /// <summary>
        /// Returns a list of posts
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns post list</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostTable()
        {
            return await _context.PostTable
                .Include(u => u.User)
                .Include(c => c.CommentList)
                .ToListAsync();
        }

        // GET: api/Posts/PostsByGenre
        /// <summary>
        /// Returns a list of posts filtered by game genre
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns post list</response>
        [HttpGet, Route("PostsByGenre/{genre}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostTableByGenre(GameGenre genre)
        {
            return await _context.PostTable
                .Where(g => g.Tag == genre)
                .Include(u => u.User)
                .Include(c => c.CommentList)
                .ToListAsync();
        }

        // GET: api/Posts/5
        /// <summary>
        /// Returns a post with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Success, not returning anything</response>
        /// <response code="404">If post does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.PostTable.FindAsync(id);

            if (post == null)
            {
                return NotFound("");
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Edits a post's information with the specified ID
        /// </summary>
        /// <param name="post"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">Successfully edited, not returning anything</response>
        /// <response code="400">If any required fields are null</response>
        /// <response code="404">If specified post id does not exist</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);
            if(username != post.User.Username && username != "admin")
            {
                return Unauthorized("Only the user that posted this post is allowed to edit it.");
            }

            var localPost = await _context.PostTable.FindAsync(id);
            if(localPost != null)
            {
                _context.Entry(localPost).State = EntityState.Detached;
            }
            else
            {
                return NotFound("Post with the specified ID does not exist.");
            }
            
            if (id != post.ID)
            {
                return BadRequest("ID provided as parameter does not match the serialized object.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object.");
            }

            post.TimeStamp = _context.PostTable
                .Where(u => u.ID == post.ID)
                .Select(u => u.TimeStamp).FirstOrDefault();

            post.UserID = _context.PostTable
                .Where(u => u.ID == post.ID)
                .Select(u => u.UserID).FirstOrDefault();

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Creates a new post
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        /// <response code="201">If a post is posted successfully</response>
        /// <response code="400">If all required fields are not filled, or non 0 post ID provided</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);
            var user = (from u in _context.UserTable
                        where u.Username == username
                        select u).FirstOrDefault();

            if (post.ID != 0)
            {
                return BadRequest("Post ID should not be provided or left at 0, as it is managed by the database.");
            }
            post.TimeStamp = DateTime.Now;
            post.User = user;

            _context.PostTable.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.ID }, post);
        }

        // DELETE: api/Posts/5
        /// <summary>
        /// Deletes a post with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">If a post is deleted successfully</response>
        /// <response code="404">If a post with the specified id is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> DeletePost(int id)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);
            var user = (from u in _context.UserTable
                        where u.Username == username
                        select u).FirstOrDefault();

            if(username != user.Username && username != "admin")
            {
                return Unauthorized("Only the user that posted this post may delete it.");
            }

            var post = await _context.PostTable.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var relatedComments = (from c in _context.CommentTable
                      where c.PostID == id
                      select c).ToList();

            _context.CommentTable.RemoveRange(relatedComments);
            _context.PostTable.Remove(post);
            await _context.SaveChangesAsync();

            return post;
        }

        private bool PostExists(int id)
        {
            return _context.PostTable.Any(e => e.ID == id);
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
