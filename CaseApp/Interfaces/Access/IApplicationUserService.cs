namespace CaseApp.Interfaces.Access;

public interface IApplicationUserService
{
    Task<bool> VerifyHashedPassword(string authorization);
}