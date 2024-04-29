using HybridMessenger.API;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HybridMessenger.Tests.API.HealthCheck
{
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;

        public HealthCheckTests(WebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            // Arrange & Act
            var response = await _httpClient.GetAsync("/health");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
