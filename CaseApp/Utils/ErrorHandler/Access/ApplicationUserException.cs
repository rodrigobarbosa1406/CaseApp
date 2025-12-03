namespace CaseApp.Utils.ErrorHandler.Access;

public class ApplicationUserException : ApplicationException
{
    public ApplicationUserException(string message) : base(message) { }
}