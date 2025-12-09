using CaseApp.Interfaces.Access;
using CaseApp.Interfaces.Providers;
using CaseApp.Models.Access;
using CaseApp.Utils.ErrorHandler.Access;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace CaseApp.Services.Access;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ISecretProvider _secretProvider;
    private readonly ILogger<TokenService> _logger;

    private static double s_tokenLifeTime;
    private static string s_issuer;
    private static string s_audience;
    private static string s_jwtSecretKeyName;
    private static string s_secretKey;
    private static double s_refreshTokenLifeTime;
    private const string JwtExceptionMessage = "Token expirado";

    public TokenService(IConfiguration configuration, ISecretProvider secretProvider, ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _secretProvider = secretProvider;
        _logger = logger;

        s_tokenLifeTime = _configuration.GetValue<double>("Jwt:Exp");
        s_issuer = _configuration.GetValue<string>("Jwt:Issuer");
        s_audience = _configuration.GetValue<string>("Jwt:Audience");
        s_jwtSecretKeyName = _configuration.GetValue<string>("KeyVault:JwtSecretKeyName");
        s_refreshTokenLifeTime = _configuration.GetValue<double>("Jwt:RefreshTokenExp");
        s_secretKey = _secretProvider.GetSecretAsync(s_jwtSecretKeyName).GetAwaiter().GetResult();
    }

    public void ValidateTokenAsync(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Falha na validação do token JWT. Token ausente");
                throw new TokenJwtException(JwtExceptionMessage);
            }

            token = token.Replace("Bearer ", string.Empty);

            var handler = new JsonWebTokenHandler();

            if (!handler.CanReadToken(token))
            {
                _logger.LogWarning("Falha na validação do token JWT. Token ilegível:\n\nToken:\n{Token}", token);
                throw new TokenJwtException(JwtExceptionMessage);
            }

            _logger.LogInformation("Token JWT validado com sucesso");
        }
        catch (TokenJwtException e)
        {
            throw new TokenJwtException(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Erro ao tentar validar token JWT.\n\nMensagem de erro: {Message}\nStackTrace: {StackTrace}", e.Message, e.StackTrace);
            throw new ApplicationException($"Erro ao tentar validar token JWT. Mensagem de erro: {e.Message}");
        }
    }

    public string GenerateToken(User user, DateTime utcNow)
    {
        try
        {
            var tokenExpiration = utcNow.AddMinutes(s_tokenLifeTime);
            var tokenJwtId = Guid.NewGuid();

            var claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Jti, tokenJwtId.ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.UserName.ToString()),
                new(JwtRegisteredClaimNames.Amr, "pwd"),
                new(JwtRegisteredClaimNames.AuthTime, utcNow.ToJToken().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = s_issuer,
                Audience = s_audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(s_secretKey)), SecurityAlgorithms.HmacSha256Signature),
                NotBefore = utcNow,
                Expires = tokenExpiration,
                IssuedAt = utcNow,
                TokenType = "JWT",
                Subject = new ClaimsIdentity(claims),
                AdditionalHeaderClaims = new Dictionary<string, object>() { { "fnc", "at" } }
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Token JWT gerado com sucesso para o usuário {UserId}", user.Id);
            return tokenHandler.WriteToken(securityToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Erro ao gerar token JWT.\n\nMensagem de erro: {Message}\nStackTrace: {StackTrace}", e.Message, e.StackTrace);
            throw new ApplicationException($"Erro ao gerar token JWT. Mensagem de erro: {e.Message}");
        }
    }

    public string GenerateRefreshToken(User user, DateTime utcNow, string token)
    {
        try
        {
            var handler = new JsonWebTokenHandler();
            var tokenJwt = handler.ReadJsonWebToken(token);

            var refreshTokenExpiration = tokenJwt.ValidTo.AddMinutes(s_refreshTokenLifeTime);
            var refreshTokenJwtId = Guid.NewGuid();

            var claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Jti, refreshTokenJwtId.ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.UserName.ToString()),
                new(JwtRegisteredClaimNames.AuthTime, utcNow.ToJToken().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = s_issuer,
                Audience = s_audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(s_secretKey)), SecurityAlgorithms.HmacSha256Signature),
                NotBefore = tokenJwt.ValidTo,
                Expires = refreshTokenExpiration,
                IssuedAt = utcNow,
                TokenType = "JWT",
                Subject = new ClaimsIdentity(claims),
                AdditionalHeaderClaims = new Dictionary<string, object>() { { "fnc", "rt" } }
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Refresh Token JWT gerado com sucesso para o usuário {UserId}", user.Id);
            return tokenHandler.WriteToken(securityToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Erro ao gerar refresh token JWT.\n\nMensagem de erro: {Message}\nStackTrace: {StackTrace}", e.Message, e.StackTrace);
            throw new ApplicationException($"Erro ao gerar refresh token JWT. Mensagem de erro: {e.Message}");
        }
    }
}