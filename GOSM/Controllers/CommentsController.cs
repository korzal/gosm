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
    [Route("api/Posts/{postId}/[controller]")]
    [ApiController]
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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentTable()
        {
            return await _context.CommentTable.ToListAsync();
        }

        // GET: api/Posts/{postId}/Comments/5
        /// <summary>
        /// Returns a comment on a post specified by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Success, not returning anything</response>
        /// <response code="404">If comment does not exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.CommentTable.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, Comment comment)
        {
            if (id != comment.ID)
            {
                return BadRequest();
            }

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
        /// <response code="400">If all required fields are not filled</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
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
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
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
    }
}
