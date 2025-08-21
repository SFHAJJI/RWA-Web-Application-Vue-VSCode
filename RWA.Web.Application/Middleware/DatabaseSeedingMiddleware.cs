using Microsoft.EntityFrameworkCore;
using RWA.Web.Application.Models;
using RWA.Web.Application.Services.Seeding;

namespace RWA.Web.Application.Middleware
{
    public class DatabaseSeedingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseSeedingMiddleware> _logger;
        private static bool _hasSeeded = false;
        private static readonly object _seedLock = new object();

        public DatabaseSeedingMiddleware(RequestDelegate next, ILogger<DatabaseSeedingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            // Only seed once and only in development
            if (!_hasSeeded)
            {
                lock (_seedLock)
                {
                    if (!_hasSeeded)
                    {
                        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                        if (env.IsDevelopment())
                        {
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    using var scope = serviceProvider.CreateScope();
                                    var scopedServices = scope.ServiceProvider;
                                    
                                    // Configure in-memory database for development
                                    var dbContextOptions = new DbContextOptionsBuilder<RwaContext>()
                                        .UseInMemoryDatabase(databaseName: "RWA_Development_DB")
                                        .Options;
                                    
                                    using var dbContext = new RwaContext(dbContextOptions);
                                    
                                    _logger.LogInformation("Setting up in-memory database for development...");
                                    
                                    // Ensure the database is created (creates the schema for in-memory)
                                    await dbContext.Database.EnsureCreatedAsync();
                                    
                                    _logger.LogInformation("Starting database seeding...");
                                    
                                    // Use reflection to create the seeder to avoid compilation issues
                                    var seederType = Type.GetType("RWA.Web.Application.Services.Seeding.DatabaseSeederService, RWA.Web.Application");
                                    if (seederType == null)
                                    {
                                        _logger.LogError("Could not find DatabaseSeederService type");
                                        return;
                                    }
                                    
                                    var seeder = Activator.CreateInstance(seederType, 
                                        dbContext,
                                        scopedServices.GetService(typeof(ILogger).MakeGenericType(seederType)),
                                        env);
                                    
                                    if (seeder != null)
                                    {
                                        var seedMethod = seederType.GetMethod("SeedDatabaseAsync");
                                        if (seedMethod != null)
                                        {
                                            var task = (Task)seedMethod.Invoke(seeder, null)!;
                                            await task;
                                        }
                                    }
                                    
                                    _logger.LogInformation("Development database setup and seeding completed successfully.");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error during development database setup and seeding");
                                }
                            });
                        }
                        _hasSeeded = true;
                    }
                }
            }

            await _next(context);
        }
    }

    // Extension method to register the middleware
    public static class DatabaseSeedingMiddlewareExtensions
    {
        public static IApplicationBuilder UseDatabaseSeeding(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DatabaseSeedingMiddleware>();
        }
    }
}
