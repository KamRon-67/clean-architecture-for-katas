using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Microsoft.Extensions.Logging; // Keep if you uncomment logging later
using System.Linq; // Needed for SingleOrDefault/Remove

namespace IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // --- Step 1: Remove the original DbContext registration ---
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SocialDbcontext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Optional: Remove DbConnection if Program.cs also registers one (unlikely based on provided code)
                // var dbConnectionDescriptor = services.SingleOrDefault(
                //     d => d.ServiceType == typeof(DbConnection));
                // if (dbConnectionDescriptor != null)
                // {
                //     services.Remove(dbConnectionDescriptor);
                // }
                // ---------------------------------------------------------

                // AddHttpClient is likely fine if needed elsewhere, but remove if unused.
                // services.AddHttpClient();

                // --- Step 2: Add the test-specific database setup ---
                // Create open SqliteConnection so EF won't automatically close it.
                // Use AddSingleton for DbConnection ONLY if you need to share the SAME connection instance across multiple DbContext instances (within the test run).
                // Often, AddDbContext handles connection management sufficiently. Let's keep the singleton for the shared in-memory DB pattern.
                services.AddSingleton<DbConnection>(container =>
                {
                    // Using "DataSource=:memory:;Cache=Shared" is often more robust for shared in-memory
                    var connection = new SqliteConnection("DataSource=:memory:;Cache=Shared");
                    connection.Open();
                    return connection;
                });

                services.AddDbContext<SocialDbcontext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                    // Consider adding logging for EF Core in tests if needed
                    // options.UseLoggerFactory(container.GetRequiredService<ILoggerFactory>())
                    //        .EnableSensitiveDataLogging();
                }, ServiceLifetime.Scoped); // Ensure DbContext has the correct lifetime (usually Scoped)
                // -----------------------------------------------------


                // --- Step 3: Remove the intermediate ServiceProvider build and usage ---
                // var sp = services.BuildServiceProvider(); // REMOVE THIS LINE
                // using (var scope = sp.CreateScope()) // REMOVE THIS BLOCK
                // {
                //     // ... commented out seeding logic ...
                // }
                // -------------------------------------------------------------------
            });

            // Setting the environment is good practice for tests
            builder.UseEnvironment("Development");
        }

        // Optional: Implement IAsyncLifetime if you need to dispose the connection
        // public override async ValueTask DisposeAsync()
        // {
        //     var connection = this.Services.GetService<DbConnection>();
        //     if (connection != null)
        //     {
        //         await connection.DisposeAsync();
        //     }
        //     await base.DisposeAsync();
        // }
    }
}