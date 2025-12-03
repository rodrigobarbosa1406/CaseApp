using CaseApp.Models.Access;

namespace CaseApp.Test.UnitTests.Models.Access;

public class ApplicationUserUnitTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        var appName = "TesteApp";
        var passwordHash = "hashedPassword";

        var applicationUser = new ApplicationUser(appName, passwordHash);

        Assert.Equal(appName, applicationUser.AppName);
        Assert.Equal(passwordHash, applicationUser.AppPasswordHash);
        Assert.NotEqual(Guid.Empty, applicationUser.AppId);
        Assert.True(applicationUser.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void DefaultConstructor_ShouldAllowPropertyAssignment()
    {
        var user = new ApplicationUser
        {
            AppName = "ManualApp",
            AppPasswordHash = "manualHash"
        };

        Assert.Equal("ManualApp", user.AppName);
        Assert.Equal("manualHash", user.AppPasswordHash);
    }
}