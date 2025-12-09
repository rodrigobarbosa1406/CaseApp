using CaseApp.Interfaces.Access;
using CaseApp.Interfaces.Invoices;
using CaseApp.Interfaces.Providers;
using CaseApp.Providers;
using CaseApp.Services.Access;
using CaseApp.Services.Invoices;

namespace CaseApp.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IReconciliationService, ReconciliationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISecretProvider>(sp =>
            new KeyVaultSecretProvider(builder.Configuration.GetValue<string>("KeyVault:VaultUri")));

        return builder;
    }
}