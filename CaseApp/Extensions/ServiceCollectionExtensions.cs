using Azure.Security.KeyVault.Secrets;
using CaseApp.Data;
using CaseApp.Models.Access;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CaseApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder, SecretClient secretClient)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            var cosmosDbConnectionString = builder.Configuration["KeyVault:ConnectionStringSecretName"];
            var secret = secretClient.GetSecretAsync(cosmosDbConnectionString).GetAwaiter().GetResult();
            var connectionString = secret.Value.Value;

            var databaseName = builder.Configuration["CosmosDb:DatabaseName"];
            options.UseCosmos(connectionString.ToString(), databaseName);
        });

        return builder;
    }

    public static WebApplicationBuilder AddIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(2));

        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationJwt(this WebApplicationBuilder builder, SecretClient secretClient)
    {
        var jwtSecretKeyName = builder.Configuration["KeyVault:JwtSecretKeyName"];
        var secret = secretClient.GetSecretAsync(jwtSecretKeyName).GetAwaiter().GetResult();
        var jwtSecretKey = secret.Value.Value;
        var key = Encoding.ASCII.GetBytes(jwtSecretKey);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.IncludeErrorDetails = builder.Environment.IsDevelopment() ? true : false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();

        return builder;
    }

    public static WebApplicationBuilder AddJsonConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.AddSingleton<JsonSerializerSettings>(new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        return builder;
    }
}