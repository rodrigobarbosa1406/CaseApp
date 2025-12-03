namespace CaseApp.DataTransferObjects.Access;

public class AuthenticatedUserGetDTO
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }

    public AuthenticatedUserGetDTO() { }

    public AuthenticatedUserGetDTO(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }
}