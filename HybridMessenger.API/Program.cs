using HybridMessenger.API.Extensions;
using HybridMessenger.Application;
using HybridMessenger.Infrastructure;
using HybridMessenger.Infrastructure.Hubs;
using HybridMessenger.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace HybridMessenger.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DevCorsPolicy", builder =>
                {
                    builder.WithOrigins("https://0.0.0.0")  
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });


            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            var connectionString = new KeyVaultService(builder.Configuration).GetDbConnectionString();

            builder.Services.AddDbContext<ApiDbContext>(options =>
            options.UseSqlServer(connectionString));

            // Custom extensions
            builder.Services.AddSwaggerSecuritySetup();
            builder.Services.AddServices();
            builder.Services.AddIdentity();
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // Assembly registration 
            builder.Services.AddApplication();
            builder.Services.AddApplicationValidation();

            builder.Services.AddSignalR();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            app.UseCors("DevCorsPolicy");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.MapHub<ChatHub>("/chathub");
            app.MapHealthChecks("/health");

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }
    }
}
