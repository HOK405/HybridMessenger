using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

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

            return services;
        }

        public static IServiceCollection AddApplicationValidation(this IServiceCollection services)
        {
            var assembly = typeof(ApplicationRegistrations).Assembly;

            services.AddValidatorsFromAssembly(assembly); 
            services.AddFluentValidationAutoValidation(configuration =>
            {
                configuration.DisableBuiltInModelValidation = true;
            });

            ValidatorOptions.Global.CascadeMode = CascadeMode.StopOnFirstFailure;

            return services;
        }
    }
}
