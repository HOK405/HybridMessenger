using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using HybridMessenger.Infrastructure.Repositories;
using HybridMessenger.Infrastructure.Services;
using HybridMessenger.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace HybridMessenger.API.Extensions
{
    public static class ServiceRegistrationExtension
    {
        /// <summary>
        /// This extension is for registration different services like UnitOfWok, UserManager and so on.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IChatMemberRepository, ChatMemberRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();

            services.AddScoped<IUserIdentityService, UserIdentityService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IDynamicProjectionService, DynamicProjectionService>();
            services.AddScoped<IUserClaimsService, UserClaimsService>();
            services.AddScoped<IBlobStorageService, BlobStorageService>();

            return services;
        }
    }
}
