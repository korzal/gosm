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
    [Route("api/Posts/{postId}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class CommentsController : ControllerBase
    {
        private readonly Database _context;

        public CommentsController(Database context)
        {
            _context = context;
        }

        // GET: api/Posts/{postId}/Comments
        /// <summary>
        /// Returns a list of comments on a post specified by ID
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns comment list</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentTable(int postId)
        {
            var retrievePost = (from p in _context.PostTable
                                where p.ID == postId
                                select p).FirstOrDefault();
            if(retrievePost == null)
            {
                return BadRequest("A post with the specified ID does not exist.");
            }

            return Ok(await _context.CommentTable
                .Where(p => p.PostID == postId)
                .Include(u => u.User)
                //.Include(p => p.Post)
                .ToListAsync());
        }

        // GET: api/Posts/{postId}/Comments/5
        /// <summary>
        /// Returns a comment on a post specified by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Success, not returning anything</response>
        /// <response code="404">If comment does not exist</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.CommentTable.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // PUT: api/Posts/{postId}/Comments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Edits a comment with the specified ID
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">Comment edited successfully, not returning anything</response>
        /// <response code="400">If any required fields are null</response>
        /// <response code="404">If specified comment id does not exist</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);
            if (username != comment.User.Username && username != "admin")
            {
                return Unauthorized("Only the user that posted this comment can edit it.");
            }

            var localComment = await _context.CommentTable.FindAsync(id);
            if (localComment != null)
            {
                _context.Entry(localComment).State = EntityState.Detached;
            }
            else
            {
                return NotFound("Post with the specified ID does not exist.");
            }

            if (id != comment.ID)
            {
                return BadRequest("ID provided as parameter does not match the serialized object.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model object.");
            }

            comment.TimeStamp = _context.CommentTable
                .Where(u => u.ID == comment.ID)
                .Select(u => u.TimeStamp).FirstOrDefault();

            comment.UserID = _context.CommentTable
                .Where(u => u.ID == comment.ID)
                .Select(u => u.UserID).FirstOrDefault();

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Posts/{postId}/Comments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Posts a new comment on a post with a specified ID
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        /// <response code="201">If a comment is posted successfully</response>
        /// <response code="400">If all required fields are not filled, or non 0 comment ID is provided</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);
            var user = (from u in _context.UserTable
                        where u.Username == username
                        select u).FirstOrDefault();

            if (comment.ID != 0)
            {
                return BadRequest("Comment ID should not be provided or left at 0, as it is managed by the database.");
            }

            comment.TimeStamp = DateTime.Now;
            comment.User = user;

            _context.CommentTable.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.ID }, comment);
        }

        // DELETE: api/Posts/{postId}/Comments/5
        /// <summary>
        /// Deletes the comment with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">If a comment is deleted successfully</response>
        /// <response code="404">If the comment with the specified id is not found</response>
        /// <response code="401">Client isn't authorized to perform this action</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
            var username = GetUsernameFromClaims(HttpContext.User.Identity as ClaimsIdentity);
            var requestingUser = (from u in _context.UserTable
                                  where u.Username == username
                                  select u).FirstOrDefault();

            var deletionUser = (from u in _context.UserTable
                                where u.ID == id
                                select u).FirstOrDefault();

            if(deletionUser.Username != requestingUser.Username && username != "admin")
            {
                return Unauthorized("Only the user that posted this comment may delete it.");
            }

            var comment = await _context.CommentTable.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.CommentTable.Remove(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        private bool CommentExists(int id)
        {
            return _context.CommentTable.Any(e => e.ID == id);
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
