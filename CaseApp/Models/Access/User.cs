using Microsoft.AspNetCore.Identity;

namespace CaseApp.Models.Access;

public class User : IdentityUser<Guid>
{
    public User() : base() { }

    public User(string userName, string passwordHash) : base()
    {
        UserName = userName;
        PasswordHash = passwordHash;
    }
}