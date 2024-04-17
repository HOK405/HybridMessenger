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

            builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            string apiBaseAddress;

            if (OperatingSystem.IsAndroid())
            {
                /*apiBaseAddress = "https://192.168.1.11:44314";*/
                apiBaseAddress = "http://10.0.2.2:44314/";
            }
            else
            {
                apiBaseAddress = builder.Configuration["ApiBaseAddress"];
            }
           
            /*var apiBaseAddress = "https://localhost:44314/";*/
            if (string.IsNullOrEmpty(apiBaseAddress))
            {
                throw new InvalidOperationException("API base address is not configured properly.");
            }

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

            builder.Services.AddScoped<IHttpService, HttpService>();
            builder.Services.AddScoped<ChatService>();

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            builder.Services.AddAuthorizationCore();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
