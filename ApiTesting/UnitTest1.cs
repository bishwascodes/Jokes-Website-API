using System.Net;
using System.Net.Http.Json;
using ApiTesting;
using Jokes.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Shouldly;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace Jokes.IntegrationTests
{
    // Checking all the catregories return
    public class CategoriesGetAllTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory;
        private readonly PostgreSqlContainer _dbContainer;
        private readonly ITestOutputHelper _outputHelper;

        public CategoriesGetAllTest(WebApplicationFactory<Program> webAppFactory, ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres")
                .WithPassword("Strong_password_123!")
                .Build();

            _factory = webAppFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<AppDbContext>();
                    services.RemoveAll<DbContextOptions>();
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_dbContainer.GetConnectionString()));
                });

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddProvider(new XUnitLoggerProvider(_outputHelper));
                    logging.SetMinimumLevel(LogLevel.Information);
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            _factory?.Dispose();
            await _dbContainer.StopAsync();
        }
        // actual test
        [Fact]
        public async Task GET_Categories_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/categories");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryReadDto>>();
            categories.ShouldNotBeNull();
            categories.ShouldBeEmpty();
        }
    }

    // Creating a category test
    public class CategoriesCreateTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory;
        private readonly PostgreSqlContainer _dbContainer;
        private readonly ITestOutputHelper _outputHelper;
        public CategoriesCreateTest(WebApplicationFactory<Program> webAppFactory, ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres")
                .WithPassword("Strong_password_123!")
                .Build();

            _factory = webAppFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<AppDbContext>();
                    services.RemoveAll<DbContextOptions>();
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_dbContainer.GetConnectionString()));
                });

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddProvider(new XUnitLoggerProvider(_outputHelper));
                    logging.SetMinimumLevel(LogLevel.Information);
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            _factory?.Dispose();
            await _dbContainer.StopAsync();
        }
        // actual test
        [Fact]
        public async Task POST_Categories_CreatesCategory_ReturnsCreatedResult()
        {
            // Arrange
            var client = _factory.CreateClient();
            var newCategory = new CategoryCreateDto { Name = "Dad Jokes" };

            // Act
            var response = await client.PostAsJsonAsync("/api/categories", newCategory);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var createdCategory = await response.Content.ReadFromJsonAsync<CategoryReadDto>();
            createdCategory.ShouldNotBeNull();
            createdCategory.Name.ShouldBe("Dad Jokes");
            createdCategory.Id.ShouldBeGreaterThan(0);

            // Verify it exists in database
            var getResponse = await client.GetAsync($"/api/categories/{createdCategory.Id}");
            getResponse.EnsureSuccessStatusCode();
        }
    }

    // Get all the audiences
    public class AudiencesGetAllTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory;
        private readonly PostgreSqlContainer _dbContainer;
        private readonly ITestOutputHelper _outputHelper;

        public AudiencesGetAllTest(WebApplicationFactory<Program> webAppFactory, ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres")
                .WithPassword("Strong_password_123!")
                .Build();

            _factory = webAppFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<AppDbContext>();
                    services.RemoveAll<DbContextOptions>();
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_dbContainer.GetConnectionString()));
                });

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddProvider(new XUnitLoggerProvider(_outputHelper));
                    logging.SetMinimumLevel(LogLevel.Information);
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            _factory?.Dispose();
            await _dbContainer.StopAsync();
        }
        //  actual test
        [Fact]
        public async Task GET_Audiences_ReturnsEmptyList_WhenNoAudiencesExist()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/audiences");

            // Assert
            response.EnsureSuccessStatusCode();
            var audiences = await response.Content.ReadFromJsonAsync<List<ReadAudienceDto>>();
            audiences.ShouldNotBeNull();
            audiences.ShouldBeEmpty();
        }
    }
    // Creating an audience test
    public class AudiencesCreateTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory;
        private readonly PostgreSqlContainer _dbContainer;
        private readonly ITestOutputHelper _outputHelper;

        public AudiencesCreateTest(WebApplicationFactory<Program> webAppFactory, ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres")
                .WithPassword("Strong_password_123!")
                .Build();

            _factory = webAppFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<AppDbContext>();
                    services.RemoveAll<DbContextOptions>();
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_dbContainer.GetConnectionString()));
                });

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddProvider(new XUnitLoggerProvider(_outputHelper));
                    logging.SetMinimumLevel(LogLevel.Information);
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            _factory?.Dispose();
            await _dbContainer.StopAsync();
        }
        // actual test
        [Fact]
        public async Task POST_Audiences_CreatesAudience_ReturnsCreatedResult()
        {
            // Arrange
            var client = _factory.CreateClient();
            var newAudience = new CreateAudienceDto { Name = "Kids", Age = 8 };

            // Act
            var response = await client.PostAsJsonAsync("/api/audiences", newAudience);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var createdAudience = await response.Content.ReadFromJsonAsync<ReadAudienceDto>();
            createdAudience.ShouldNotBeNull();
            createdAudience.Name.ShouldBe("Kids");
            createdAudience.Age.ShouldBe(8);
            createdAudience.Id.ShouldBeGreaterThan(0);

            // Verify it exists in database
            var getResponse = await client.GetAsync($"/api/audiences/{createdAudience.Id}");
            getResponse.EnsureSuccessStatusCode();
        }
    }

    // Get all the jokes - test
    public class JokesGetAllTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory;
        private readonly PostgreSqlContainer _dbContainer;
        private readonly ITestOutputHelper _outputHelper;

        public JokesGetAllTest(WebApplicationFactory<Program> webAppFactory, ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres")
                .WithPassword("Strong_password_123!")
                .Build();

            _factory = webAppFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<AppDbContext>();
                    services.RemoveAll<DbContextOptions>();
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_dbContainer.GetConnectionString()));
                });

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddProvider(new XUnitLoggerProvider(_outputHelper));
                    logging.SetMinimumLevel(LogLevel.Information);
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            _factory?.Dispose();
            await _dbContainer.StopAsync();
        }
        // actual test
        [Fact]
        public async Task GET_Jokes_ReturnsEmptyList_WhenNoJokesExist()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/jokes");

            // Assert
            response.EnsureSuccessStatusCode();
            var jokes = await response.Content.ReadFromJsonAsync<List<ReadJokeDto>>();
            jokes.ShouldNotBeNull();
            jokes.ShouldBeEmpty();
        }
    }
    // Creating a joke - test
    public class JokesCreateTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private WebApplicationFactory<Program> _factory;
        private readonly PostgreSqlContainer _dbContainer;
        private readonly ITestOutputHelper _outputHelper;

        public JokesCreateTest(WebApplicationFactory<Program> webAppFactory, ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres")
                .WithPassword("Strong_password_123!")
                .Build();

            _factory = webAppFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<AppDbContext>();
                    services.RemoveAll<DbContextOptions>();
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_dbContainer.GetConnectionString()));
                });

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddProvider(new XUnitLoggerProvider(_outputHelper));
                    logging.SetMinimumLevel(LogLevel.Information);
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            _factory?.Dispose();
            await _dbContainer.StopAsync();
        }
        // actual test
        [Fact]
        public async Task POST_Jokes_CreatesJoke_ReturnsCreatedResult()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Create prerequisite data first
            var categoryDto = new CategoryCreateDto { Name = "Programming" };
            var categoryResponse = await client.PostAsJsonAsync("/api/categories", categoryDto);
            var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryReadDto>();

            var audienceDto = new CreateAudienceDto { Name = "Developers", Age = 28 };
            var audienceResponse = await client.PostAsJsonAsync("/api/audiences", audienceDto);
            var audience = await audienceResponse.Content.ReadFromJsonAsync<ReadAudienceDto>();

            var newJoke = new CreateJokeDto
            {
                Content = "Why do programmers prefer dark mode? Because light attracts bugs!",
                CategoryId = category!.Id,
                AudienceIds = new List<int> { audience!.Id }
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/jokes", newJoke);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var createdJoke = await response.Content.ReadFromJsonAsync<ReadJokeDto>();
            createdJoke.ShouldNotBeNull();
            createdJoke.Content.ShouldBe(newJoke.Content);
            createdJoke.CategoryName.ShouldBe("Programming");
            createdJoke.Audiences.ShouldContain("Developers");
            createdJoke.Id.ShouldBeGreaterThan(0);

            // Verify it exists in database
            var getResponse = await client.GetAsync($"/api/jokes/{createdJoke.Id}");
            getResponse.EnsureSuccessStatusCode();
        }
    }
}
