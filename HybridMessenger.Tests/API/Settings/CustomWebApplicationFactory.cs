using HybridMessenger.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.Tests.API.Settings
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApiDbContext>)); // Find specific service 

                if (descriptor != null)
                {
                    services.Remove(descriptor); // Remove service
                }

                services.AddDbContext<ApiDbContext>(options =>
                    options.UseSqlServer("Server=localhost;Database=HybridMessenger.TestDatabase;Trusted_Connection=True;TrustServerCertificate=True;")); // Add new context
            });
        }
    }
}
