using Jokes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JokesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JokesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/jokes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadJokeDto>>> GetJokes()
        {
            return await _context.Jokes
                .Include(j => j.Category)
                .Include(j => j.Audiences)
                .Select(j => new ReadJokeDto
                {
                    Id = j.JokeId,
                    Content = j.Content,
                    CategoryName = j.Category.Name,
                    Audiences = j.Audiences.Select(a => a.Name).ToList()
                })
                .ToListAsync();
        }

        // GET: api/jokes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadJokeDto>> GetJoke(int id)
        {
            var joke = await _context.Jokes
                .Include(j => j.Category)
                .Include(j => j.Audiences)
                .FirstOrDefaultAsync(j => j.JokeId == id);

            if (joke == null)
                return NotFound();

            return new ReadJokeDto
            {
                Id = joke.JokeId,
                Content = joke.Content,
                CategoryName = joke.Category.Name,
                Audiences = joke.Audiences.Select(a => a.Name).ToList()
            };
        }

        // POST: api/jokes
        [HttpPost]
        public async Task<ActionResult<ReadJokeDto>> CreateJoke(CreateJokeDto dto)
        {
            var joke = new DbJoke
            {
                Content = dto.Content,
                CategoryId = dto.CategoryId
            };

            // link audiences
            if (dto.AudienceIds.Any())
            {
                var audiences = await _context.Audiences
                    .Where(a => dto.AudienceIds.Contains(a.AudienceId))
                    .ToListAsync();
                joke.Audiences = audiences;
            }

            _context.Jokes.Add(joke);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJoke), new { id = joke.JokeId }, new ReadJokeDto
            {
                Id = joke.JokeId,
                Content = joke.Content,
                CategoryName = (await _context.Categories.FindAsync(joke.CategoryId))?.Name ?? "",
                Audiences = joke.Audiences.Select(a => a.Name).ToList()
            });
        }
    }
}
