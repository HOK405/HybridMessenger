using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using HybridMessenger.Infrastructure.Services;
using HybridMessenger.Infrastructure.UnitOfWork;

namespace HybridMessenger.API.Extensions
{
    public static class ServiceRegistrationExtension
    {
        /// <summary>
        /// This extension is for registration different services like UnitOfWok, UserManager and so on
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserIdentityService, UserIdentityService>();

            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
