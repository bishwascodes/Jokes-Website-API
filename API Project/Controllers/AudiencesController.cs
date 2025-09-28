using Jokes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AudiencesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AudiencesController(AppDbContext context)
        {
            //Correctly scaffold your DBContext and associated classes, appropriately inject DBContext into your classes
            _context = context;
        }

        // GET: api/audiences
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadAudienceDto>>> GetAudiences()
        {
            return await _context.Audiences
                .Include(a => a.Jokes)
                .Select(a => new ReadAudienceDto
                {
                    Id = a.AudienceId,
                    Name = a.Name,
                    Age = a.Age,
                    Jokes = a.Jokes.Select(j => j.Content).ToList()
                })
                .ToListAsync();
        }

        // GET: api/audiences/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadAudienceDto>> GetAudience(int id)
        {
            var audience = await _context.Audiences
                .Include(a => a.Jokes)
                .FirstOrDefaultAsync(a => a.AudienceId == id);

            if (audience == null)
                return NotFound();

            return new ReadAudienceDto
            {
                Id = audience.AudienceId,
                Name = audience.Name,
                Age = audience.Age,
                Jokes = audience.Jokes.Select(j => j.Content).ToList()
            };
        }

        // POST: api/audiences
        [HttpPost]
        public async Task<ActionResult<ReadAudienceDto>> CreateAudience(CreateAudienceDto dto)
        {
            var audience = new DbAudience
            {
                Name = dto.Name,
                Age = dto.Age
            };

            _context.Audiences.Add(audience);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAudience), new { id = audience.AudienceId }, new ReadAudienceDto
            {
                Id = audience.AudienceId,
                Name = audience.Name,
                Age = audience.Age,
                Jokes = new List<string>()
            });
        }
    }
}
