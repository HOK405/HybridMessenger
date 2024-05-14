using HybridMessenger.Presentation.Auth;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HybridMessenger.Presentation
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

            /*string apiBaseAddress = "https://hybridmessenger-2024.azurewebsites.net";*/
            string apiBaseAddress = "https://localhost:44314";
            if (string.IsNullOrEmpty(apiBaseAddress))
            {
                throw new InvalidOperationException("API base address is not configured properly.");
            }

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

            builder.Services.AddScoped<IHttpService, HttpService>();
            builder.Services.AddScoped<ChatService>();

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationService>();

            builder.Services.AddAuthorizationCore();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
