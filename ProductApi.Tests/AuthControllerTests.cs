using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductApi.Controllers;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public void Login_ReturnsToken_WhenCredentialsAreCorrect()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "LoginTestDatabase")
                .Options;

            using var context = new AppDbContext(options);

            context.Users.Add(new User
            {
                Id = 1,
                Username = "testuser",
                Password = "password123"
            });

            context.SaveChanges();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "ThisIsASecretKeyForTesting123456789",
                    ["Jwt:Issuer"] = "ProductApi",
                    ["Jwt:Audience"] = "ProductApiUsers"
                })
                .Build();

            var controller = new AuthController(context, configuration);

            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            // Act
            var result = controller.Login(loginRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public void Login_ReturnsUnauthorized_WhenPasswordIsWrong()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "WrongPasswordTestDatabase")
                .Options;

            using var context = new AppDbContext(options);

            context.Users.Add(new User
            {
                Id = 1,
                Username = "testuser",
                Password = "password123"
            });

            context.SaveChanges();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "ThisIsASecretKeyForTesting123456789",
                    ["Jwt:Issuer"] = "ProductApi",
                    ["Jwt:Audience"] = "ProductApiUsers"
                })
                .Build();

            var controller = new AuthController(context, configuration);

            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            // Act
            var result = controller.Login(loginRequest);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
        [Fact]
        public void Refresh_ReturnsToken_WhenRefreshTokenIsValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "RefreshTestDatabase")
                .Options;

            using var context = new AppDbContext(options);

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Password = "password123"
            };

            context.Users.Add(user);

            context.RefreshTokens.Add(new RefreshToken
            {
                Token = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                UserId = 1
            });

            context.SaveChanges();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "ThisIsASecretKeyForTesting123456789",
                    ["Jwt:Issuer"] = "ProductApi",
                    ["Jwt:Audience"] = "ProductApiUsers"
                })
                .Build();

            var controller = new AuthController(context, configuration);

            // Act
            var result = controller.Refresh("valid-refresh-token");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public void Refresh_ReturnsUnauthorized_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "InvalidRefreshTestDatabase")
                .Options;

            using var context = new AppDbContext(options);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "ThisIsASecretKeyForTesting123456789",
                    ["Jwt:Issuer"] = "ProductApi",
                    ["Jwt:Audience"] = "ProductApiUsers"
                })
                .Build();

            var controller = new AuthController(context, configuration);

            // Act
            var result = controller.Refresh("invalid-refresh-token");

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
        [Fact]
        public void Refresh_ReturnsUnauthorized_WhenRefreshTokenIsExpired()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ExpiredRefreshTestDatabase")
                .Options;

            using var context = new AppDbContext(options);

            context.Users.Add(new User
            {
                Id = 1,
                Username = "testuser",
                Password = "password123"
            });

            context.RefreshTokens.Add(new RefreshToken
            {
                Token = "expired-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                IsRevoked = false,
                UserId = 1
            });

            context.SaveChanges();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "ThisIsASecretKeyForTesting123456789",
                    ["Jwt:Issuer"] = "ProductApi",
                    ["Jwt:Audience"] = "ProductApiUsers"
                })
                .Build();

            var controller = new AuthController(context, configuration);

            // Act
            var result = controller.Refresh("expired-refresh-token");

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}