using CaseApp.Models.Access;
using CaseApp.Providers;
using CaseApp.Services.Access;
using CaseApp.Utils.ErrorHandler.Access;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace CaseApp.Test.UnitTests.Services.Access;

public class TokenServiceUnitTests
{
    private TokenService CreateService(string secretKey = "zcwaHc9vs5#4vv&P8k7!6P&u&sPyxADmb^gNk^fVtq3T!kWxYbNXFBM82tN7avEeAQVJAp2hMeBvTyLM")
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:Exp", "30"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"},
            {"KeyVault:JwtSecretKeyName", "TestSecret"},
            {"KeyVault:VaultUri", "https://fake.vault"},
            {"Jwt:RefreshTokenExp", "60"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var fakeSecretProvider = new FakeSecretProvider("zcwaHc9vs5#4vv&P8k7!6P&u&sPyxADmb^gNk^fVtq3T!kWxYbNXFBM82tN7avEeAQVJAp2hMeBvTyLM");
        var loggerMock = new Mock<ILogger<TokenService>>();

        var service = new TokenService(configuration, fakeSecretProvider, loggerMock.Object);

        typeof(TokenService)
            .GetField("s_secretKey", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            .SetValue(null, secretKey);

        return service;
    }

    [Fact]
    public void ValidateTokenAsync_ShouldThrowTokenJwtException_WhenTokenIsNullOrEmpty()
    {
        var service = CreateService();

        Assert.Throws<TokenJwtException>(() => service.ValidateTokenAsync(""));
    }

    [Fact]
    public void ValidateTokenAsync_ShouldThrowTokenJwtException_WhenTokenIsInvalid()
    {
        var service = CreateService();

        Assert.Throws<TokenJwtException>(() => service.ValidateTokenAsync("Bearer invalid_token"));
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwt()
    {
        var service = CreateService();
        var user = new User("testuser@test.com", "hash123") { Id = Guid.NewGuid() };

        var token = service.GenerateToken(user, DateTime.UtcNow);

        Assert.NotNull(token);
        Assert.Contains("eyJ", token);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidJwt()
    {
        var service = CreateService();
        var user = new User("testuser@test.com", "hash123") { Id = Guid.NewGuid() };

        var accessToken = service.GenerateToken(user, DateTime.UtcNow);

        var refreshToken = service.GenerateRefreshToken(user, DateTime.UtcNow, accessToken);

        Assert.NotNull(refreshToken);
        Assert.Contains("eyJ", refreshToken);
    }

}