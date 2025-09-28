using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jokes.Models
{

    // ============================
    // CATEGORY
    // ============================
    public class DbCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<DbJoke> Jokes { get; set; } = new List<DbJoke>();
    }

    // ============================
    // JOKE
    // ============================
    public class DbJoke
    {
        [Key]
        public int JokeId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public DbCategory Category { get; set; } = null!;

        // Many-to-many with Audience
        public ICollection<DbAudience> Audiences { get; set; } = new List<DbAudience>();
    }

    // ============================
    // AUDIENCE
    // ============================
    public class DbAudience
    {
        [Key]
        public int AudienceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        // Many-to-many with Jokes
        public ICollection<DbJoke> Jokes { get; set; } = new List<DbJoke>();
    }

    // ============================
    // CATEGORY DTOs
    // ============================
    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class CategoryReadDto
    {
        public int Id { get; set; }              // Different name than DbCategory.CategoryId
        public string Name { get; set; } = string.Empty;
    }


    // ============================
    // JOKE DTOs
    // ============================
    // For creating a joke
    public class CreateJokeDto
    {
        public string Content { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public List<int> AudienceIds { get; set; } = new();  // link audiences
    }

    // For reading a joke
    public class ReadJokeDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public List<string> Audiences { get; set; } = new(); // audience names
    }

    // ============================
    // AUDIENCE DTOs
    // ============================
    // For creating an audience
    public class CreateAudienceDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    // For reading an audience
    public class ReadAudienceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public List<string> Jokes { get; set; } = new(); // joke contents
    }


}
