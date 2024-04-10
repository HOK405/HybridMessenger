using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace HybridMessenger.Application
{
    public static class ApplicationRegistrations
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(ApplicationRegistrations).Assembly;

            services.AddAutoMapper(assembly);

            services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(assembly));

            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
