using HybridMessenger.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            // Retrieve and validate JWT settings
            var jwtSettings = GetJwtSettings(configuration);
            ValidateJwtSettings(jwtSettings);

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
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.Key),
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

        private static (byte[] Key, string Audience, string Issuer) GetJwtSettings(IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(new KeyVaultService(configuration).GetJwtKey());
            var audience = configuration["JwtAudience"];
            var issuer = configuration["JwtIssuer"];

            return (key, audience, issuer);
        }

        private static void ValidateJwtSettings((byte[] Key, string Audience, string Issuer) jwtSettings)
        {
            if (jwtSettings.Key is null || jwtSettings.Key.Length == 0)
            {
                throw new ArgumentNullException("JwtKey", "JWT Key must not be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
            {
                throw new ArgumentNullException("JwtAudience", "Audience must not be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
            {
                throw new ArgumentNullException("JwtIssuer", "Issuer must not be null or empty.");
            }
        }
    }
}
