namespace HybridMessenger.Presentation
{
    public static class ApiConfiguration
    {
        public static string ApiBaseAddress { get; set; } = "https://hybridmessengerapi20240524013739.azurewebsites.net/";
        public static string HubEndpoint { get; set; } = "/chathub";
        public static string FullHub => $"{ApiBaseAddress.TrimEnd('/')}/{HubEndpoint.TrimStart('/')}";
    }

    /*
    In MauiProgram.cs:
    string apiBaseAddress = builder.Configuration["ApiBaseAddress"];
    */

    /*
    In ChatService:
    string baseAddress = configuration.GetValue<string>("ApiBaseAddress");
    string endpoint = configuration.GetValue<string>("HubEndpoint");
    _url = $"{baseAddress.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    */
}
