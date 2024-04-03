using Microsoft.Extensions.DependencyInjection;

namespace HybridMessenger.Application
{
    public static class ApplicationRegistrations
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(ApplicationRegistrations).Assembly;

            services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(assembly));

            return services;
        }
    }
}
