namespace CaseApp.Interfaces.Access;

public interface IUserService
{
    Task<dynamic> FindAsync(string userName);
}