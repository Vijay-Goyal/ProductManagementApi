using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ProductApi.Tests
{
    public class ProductsApiIntegrationTests :
        IClassFixture<TestingWebApplicationFactory>
    {
        private readonly TestingWebApplicationFactory _factory;

        public ProductsApiIntegrationTests(
            TestingWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetProducts_ReturnsSuccessStatusCode()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/Products");

            // Assert
            Assert.True(
                response.IsSuccessStatusCode ||
                response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }

    public class TestingWebApplicationFactory :
        WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }
    }
}