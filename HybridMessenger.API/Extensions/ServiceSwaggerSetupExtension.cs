using Autofac.Core;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace HybridMessenger.API.Extensions
{
    public static class ServiceSwaggerSetupExtension
    {
        /// <summary>
        /// This extension is for set up swaager for input jwt token
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerSecuritySetup(this IServiceCollection services)
        {       
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Please provide a valid token",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            return services;
        }
    }
}
