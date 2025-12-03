using CaseApp.Data;
using CaseApp.Models.Access;
using CaseApp.Services.Access;
using CaseApp.Utils.ErrorHandler.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace CaseApp.Test.UnitTests.Services.Access;

public class ApplicationUserServiceUnitTests
{
    private ApplicationUserService CreateService(ApplicationDbContext context)
    {
        var loggerMock = new Mock<ILogger<ApplicationUserService>>();
        return new ApplicationUserService(context, loggerMock.Object);
    }

    private string CreateAuthorizationHeader(Guid appId, string password)
    {
        var plain = $"{appId}:{password}";
        var base64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(plain));
        return $"Basic {base64}";
    }

    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task VerifyHashedPassword_ShouldThrow_WhenAuthorizationIsNull()
    {
        var context = CreateInMemoryContext();
        var service = CreateService(context);

        await Assert.ThrowsAsync<ApplicationUserException>(() => service.VerifyHashedPassword(null));
    }

    [Fact]
    public async Task VerifyHashedPassword_ShouldThrow_WhenAuthorizationDoesNotContainBasic()
    {
        var context = CreateInMemoryContext();
        var service = CreateService(context);

        await Assert.ThrowsAsync<ApplicationUserException>(() => service.VerifyHashedPassword("InvalidHeader"));
    }

    [Fact]
    public async Task VerifyHashedPassword_ShouldThrow_WhenAuthorizationWithoutColon()
    {
        var context = CreateInMemoryContext();
        var service = CreateService(context);

        var base64 = Convert.ToBase64String(Encoding.ASCII.GetBytes("invalidstring"));
        var header = $"Basic {base64}";

        await Assert.ThrowsAsync<ApplicationUserException>(() => service.VerifyHashedPassword(header));
    }

    [Fact]
    public async Task VerifyHashedPassword_ShouldThrow_WhenAppIdNotFound()
    {
        var context = CreateInMemoryContext();
        var service = CreateService(context);

        var header = CreateAuthorizationHeader(Guid.NewGuid(), "password123");

        await Assert.ThrowsAsync<ApplicationUserException>(() => service.VerifyHashedPassword(header));
    }

    [Fact]
    public async Task VerifyHashedPassword_ShouldThrow_WhenPasswordIsInvalid()
    {
        var context = CreateInMemoryContext();
        var appId = Guid.NewGuid();
        var user = new ApplicationUser("TestApp", BCrypt.Net.BCrypt.EnhancedHashPassword("correctPassword"))
        {
            AppId = appId
        };
        context.ApplicationUser.Add(user);
        context.SaveChanges();

        var service = CreateService(context);
        var header = CreateAuthorizationHeader(appId, "wrongPassword");

        await Assert.ThrowsAsync<ApplicationUserException>(() => service.VerifyHashedPassword(header));
    }

    [Fact]
    public async Task VerifyHashedPassword_ShouldReturnTrue_WhenCredentialsAreValid()
    {
        var context = CreateInMemoryContext();
        var appId = Guid.NewGuid();
        var password = "correctPassword";
        var user = new ApplicationUser("TestApp", BCrypt.Net.BCrypt.EnhancedHashPassword(password))
        {
            AppId = appId
        };
        context.ApplicationUser.Add(user);
        context.SaveChanges();

        var service = CreateService(context);
        var header = CreateAuthorizationHeader(appId, password);

        var result = await service.VerifyHashedPassword(header);

        Assert.True(result);
    }
}