using HybridMessenger.API.Extensions;
using HybridMessenger.Application;
using HybridMessenger.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddDbContext<ApiDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Custom extensions
            builder.Services.AddSwaggerSecuritySetup();
            builder.Services.AddServices();
            builder.Services.AddIdentity();
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // Assembly registration
            builder.Services.AddApplication();
            builder.Services.AddApplicationValidation();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
