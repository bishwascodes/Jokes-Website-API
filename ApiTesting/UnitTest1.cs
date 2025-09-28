using ApiTesting;
using Jokes.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;


namespace TestProject1;

public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private WebApplicationFactory<Program> customWebAppFactory;
    private readonly PostgreSqlContainer _dbContainer;

    public UnitTest1(WebApplicationFactory<Program> webAppFactory, ITestOutputHelper outputHelper)
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres")
            .WithPassword("Strong_password_123!")
            .Build();

        customWebAppFactory = webAppFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<AppDbContext>();
                services.RemoveAll<DbContextOptions>();
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.AddDbContext<AppDbContext>(options => options.UseNpgsql(_dbContainer.GetConnectionString()));
            });

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddProvider(new XUnitLoggerProvider(outputHelper));
                logging.SetMinimumLevel(LogLevel.Information);
            });
        });
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        //You can run database migrations if you have them
        using var scope = customWebAppFactory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    [Fact]
    public async Task Database_ShouldHaveCorrectSchema()
    {
        // Arrange
        var client = customWebAppFactory.CreateClient();

        // Act - try to access your endpoints
        var categoriesResponse = await client.GetAsync("/api/categories");

        // Assert - should not get database errors
        categoriesResponse.EnsureSuccessStatusCode();
    }
}