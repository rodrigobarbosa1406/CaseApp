namespace CaseApp.Interfaces.Providers;

public interface ISecretProvider
{
    Task<string> GetSecretAsync(string secretName);
}