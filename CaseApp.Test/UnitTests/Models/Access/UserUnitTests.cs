using CaseApp.Models.Access;

namespace CaseApp.Test.UnitTests.Models.Access;

public class UserUnitTests
{
    [Fact]
    public void Constructor_ShouldSetUserNameAndPasswordHash()
    {
        var userName = "User";
        var passwordHash = "hashedPassword";

        var user = new User(userName, passwordHash);

        Assert.Equal(userName, user.UserName);
        Assert.Equal(passwordHash, user.PasswordHash);
    }

    [Fact]
    public void DefaultConstructor_ShouldAllowPropertyAssignment()
    {
        var user = new User
        {
            UserName = "ManualUser",
            PasswordHash = "manualHash"
        };

        Assert.Equal("ManualUser", user.UserName);
        Assert.Equal("manualHash", user.PasswordHash);
    }

}