﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HybridMessenger.API.Extensions
{
    public static class AuthenticationExtension
    {
        /// <summary>
        /// This extension is for adding JWT Authentication. It defines key from the configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure JWT Authentication
            ValidateJwtSettings(configuration);

            var key = Encoding.ASCII.GetBytes(configuration["JwtSettings:Key"]);
            var audience = configuration["JwtSettings:Audience"];
            var issuer = configuration["JwtSettings:Issuer"];

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();

            return services;
        }

        private static void ValidateJwtSettings(IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(configuration["JwtSettings:Key"]);
            var audience = configuration["JwtSettings:Audience"];
            var issuer = configuration["JwtSettings:Issuer"];

            if (key is null || key.Length == 0)
            {
                throw new ArgumentNullException("JwtSettings:Key", "JWT Key must not be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(audience))
            {
                throw new ArgumentNullException("JwtSettings:Audience", "Audience must not be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentNullException("JwtSettings:Issuer", "Issuer must not be null or empty.");
            }
        }
    }
}
