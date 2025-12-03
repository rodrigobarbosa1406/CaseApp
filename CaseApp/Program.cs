using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using CaseApp.Data;
using CaseApp.Endpoints.Access;
using CaseApp.Endpoints.Invoices;
using CaseApp.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

builder.Services.AddValidation();

builder
    .AddServices()
    .AddPersistence(secretClient)
    .AddIdentity()
    .AddAuthenticationJwt(secretClient)
    .AddJsonConfiguration();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();
app.MapReconciliationEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();