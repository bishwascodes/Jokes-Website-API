using Jokes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /admin/reset
        [HttpGet("reset")]
        public async Task<ActionResult<object>> ResetDatabase()
        {
            try
            {
                // Delete all data in order (respect foreign key constraints)
                // First remove many-to-many relationships
                var jokesWithAudiences = await _context.Jokes
                    .Include(j => j.Audiences)
                    .ToListAsync();

                foreach (var joke in jokesWithAudiences)
                {
                    joke.Audiences.Clear();
                }

                // Delete all jokes
                var allJokes = await _context.Jokes.ToListAsync();
                _context.Jokes.RemoveRange(allJokes);

                // Delete all audiences
                var allAudiences = await _context.Audiences.ToListAsync();
                _context.Audiences.RemoveRange(allAudiences);

                // Delete all categories
                var allCategories = await _context.Categories.ToListAsync();
                _context.Categories.RemoveRange(allCategories);

                // Save changes
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Database reset successfully",
                    timestamp = DateTime.UtcNow,
                    deletedItems = new
                    {
                        jokes = allJokes.Count,
                        audiences = allAudiences.Count,
                        categories = allCategories.Count
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error resetting database",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

    }
}