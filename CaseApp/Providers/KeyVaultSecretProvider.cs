using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CaseApp.Interfaces.Providers;

namespace CaseApp.Providers;

public class KeyVaultSecretProvider : ISecretProvider
{
    private readonly SecretClient _secretClient;

    public KeyVaultSecretProvider(string vaultUri) =>
        _secretClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());

    public async Task<string> GetSecretAsync(string secretName)
    {
        var secret = await _secretClient.GetSecretAsync(secretName);
        return secret.Value.Value;
    }
}