using CaseApp.Interfaces.Providers;

namespace CaseApp.Providers;

public class FakeSecretProvider : ISecretProvider
{
    private readonly string _secret;

    public FakeSecretProvider(string secret) =>
        _secret = secret;

    public Task<string> GetSecretAsync(string secretName) =>
        Task.FromResult(secretName);
}