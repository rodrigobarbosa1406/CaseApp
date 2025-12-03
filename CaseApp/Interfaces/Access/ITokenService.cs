using CaseApp.Models.Access;

namespace CaseApp.Interfaces.Access;

public interface ITokenService
{
    void ValidateTokenAsync(string token);

    string GenerateToken(User user, DateTime utcNow);

    string GenerateRefreshToken(User user, DateTime utcNow, string token);
}