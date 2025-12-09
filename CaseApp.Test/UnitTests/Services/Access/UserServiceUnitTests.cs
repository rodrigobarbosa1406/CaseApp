using CaseApp.Data;
using CaseApp.Models.Access;
using CaseApp.Services.Access;
using CaseApp.Utils.ErrorHandler.Commom;
using Microsoft.EntityFrameworkCore;

namespace CaseApp.Test.UnitTests.Services.Access;

public class UserServiceUnitTests
{
    [Fact]
    public async Task FindAsync_ShouldReturnUser_WhenUserExists()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("UserDb_Test1")
            .Options;

        using var context = new ApplicationDbContext(options);
        context.Users.Add(new User("User", "hash123"));
        context.SaveChanges();

        var service = new UserService(context);

        var result = await service.FindAsync("User");

        Assert.NotNull(result);
        Assert.Equal("User", result.UserName);
    }

    [Fact]
    public async Task FindAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "UserDb_Test2")
            .Options;

        using var context = new ApplicationDbContext(options);
        var service = new UserService(context);

        await Assert.ThrowsAsync<NotFoundException>(() => service.FindAsync("Inexistente"));
    }
}