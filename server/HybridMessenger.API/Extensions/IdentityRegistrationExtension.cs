﻿using HybridMessenger.Domain.Entities;
using HybridMessenger.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace HybridMessenger.API.Extensions
{
    public static class IdentityRegistrationExtension
    {
        /// <summary>
        /// This extension is for registration identity and user properties (Unique email, password and so on).
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            // Configure Identity
            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
             .AddEntityFrameworkStores<ApiDbContext>()
             .AddDefaultTokenProviders();

            return services;
        }
    }
}
