using AuthC_.Models;
using AuthC_.Services;
using AuthC_.DTOs;
using AuthC_.Data;
using Microsoft.EntityFrameworkCore;
using AuthC_.Helpers;
using Microsoft.Extensions.Configuration;

namespace AuthC_.Tests.Services
{
    public class UserServiceTests
    {
        private static DbContextOptions<AppDbContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        private static IConfiguration CreateConfiguration() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                { "Jwt:Key", "super_secret_key_1234567890_super_secret_key_1234567890" },
                { "Jwt:Issuer", "myauthapp.com" },
                { "Jwt:Audience", "myauthapp.com" },
                { "Jwt:ExpiresInMinutes", "60" }
                })
                .Build();

        private static AuthService CreateAuthService(AppDbContext context)
        {
            var configuration = CreateConfiguration();
            var jwtHelper = new JwtHelper(configuration);
            return new AuthService(context, jwtHelper);
        }

        [Fact]
        public async Task SignUpAsync_Should_Create_User_When_Email_Not_Exists()
        {
            // Arrange
            var options = CreateOptions("TestDb");
            using var context = new AppDbContext(options);
            var service = CreateAuthService(context);

            var userDto = new UserSignupDTO
            {
                Email = "test@example.com",
                Password = "P@ssword123"
            };

            // Act
            var result = await service.SignUpUser(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.Email, result.Email);
            Assert.True(context.Users.Any(u => u.Email == userDto.Email));
        }

        [Fact]
        public async Task SignUpAsync_Should_Throw_When_Email_Exists()
        {
            // Arrange
            var options = CreateOptions("TestDb2");
            using var context = new AppDbContext(options);
            context.Users.Add(new User { Email = "test@example.com", Hash = "hashed" });
            await context.SaveChangesAsync();

            var service = CreateAuthService(context);

            var userDto = new UserSignupDTO
            {
                Email = "test@example.com",
                Password = "P@ssword123"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.SignUpUser(userDto));
        }
    }
}
