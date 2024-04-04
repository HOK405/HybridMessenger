using HybridMessenger.Presentation.Services;
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

            var apiBaseAddress = builder.Configuration["ApiBaseAddress"];
            if (string.IsNullOrEmpty(apiBaseAddress))
            {
                throw new InvalidOperationException("API base address is not configured properly.");
            }

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

            builder.Services.AddScoped<IHttpService, HttpService>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
