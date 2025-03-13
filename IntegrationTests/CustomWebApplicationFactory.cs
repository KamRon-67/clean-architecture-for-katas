using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // var dbContextDescriptor = services.SingleOrDefault(
                //     d => d.ServiceType ==
                //         typeof(DbContextOptions<SocialDbcontext>));
                //
                // services.Remove(dbContextDescriptor);
                //
                // var dbConnectionDescriptor = services.SingleOrDefault(
                //     d => d.ServiceType ==
                //         typeof(DbConnection));
                //
                // services.Remove(dbConnectionDescriptor);

                services.AddHttpClient();
                // Create open SqliteConnection so EF won't automatically close it.
                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<SocialDbcontext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });
                
                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    //var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

                    // Ensure the database is created.
                    //db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                        //Utilities.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
                    }
                }
            });

            builder.UseEnvironment("Development");
        }

    }
}
