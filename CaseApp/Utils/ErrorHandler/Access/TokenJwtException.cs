namespace CaseApp.Utils.ErrorHandler.Access;

public class TokenJwtException : ApplicationException
{
    public TokenJwtException(string message) : base(message) { }
}