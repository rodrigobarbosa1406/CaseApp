namespace CaseApp.Utils.ErrorHandler.Commom;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message) : base(message) { }
}