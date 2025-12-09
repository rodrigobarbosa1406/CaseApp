using CaseApp.Data;
using CaseApp.Interfaces.Access;
using CaseApp.Utils.ErrorHandler.Access;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CaseApp.Services.Access;

public class ApplicationUserService(ApplicationDbContext context, ILogger<ApplicationUserService> logger) : IApplicationUserService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ApplicationUserService> _logger = logger;

    public async Task<bool> VerifyHashedPassword(string authorization)
    {
        try
        {
            var exceptionMessage = "Não autorizado.";

            if (string.IsNullOrEmpty(authorization))
            {
                _logger.LogWarning("Acesso de aplicação negado. Dados de autorização ausentes");
                throw new ApplicationUserException(exceptionMessage);
            }

            if (!authorization.Contains("Basic"))
            {
                _logger.LogWarning("Acesso de aplicação negado. Dados de autorização inválidos\n\nHeader:\n{Authorization}", authorization);
                throw new ApplicationUserException(exceptionMessage);
            }

            var secret = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(authorization.Replace("Basic ", string.Empty)));

            if (!secret.Contains(':'))
            {
                _logger.LogWarning("Acesso de aplicação negado. Dados de autorização inválidos\n\nHeader:\n{Authorization}", authorization);
                throw new ApplicationUserException(exceptionMessage);
            }

            var clientCredential = secret.Split(':');

            if (clientCredential.Length != 2)
            {
                _logger.LogWarning("Acesso de aplicação negado. Dados de autorização inválidos\n\nHeader:\n{Authorization}", authorization);
                throw new ApplicationUserException(exceptionMessage);
            }

            var appId = Guid.TryParse(clientCredential[0], out Guid credential) ? credential : Guid.Empty;
            var appPassword = clientCredential[1];

            var applicationUser = await _context.ApplicationUser.FirstOrDefaultAsync(au => au.AppId == appId);

            if (applicationUser is null)
            {
                _logger.LogWarning("Acesso de aplicação negado. O Id de aplicação {AppId} não existe", appId);
                throw new ApplicationUserException(exceptionMessage);
            }

            var passwordVerify = !string.IsNullOrEmpty(appPassword) && BCrypt.Net.BCrypt.EnhancedVerify(appPassword, applicationUser.AppPasswordHash);

            if (!passwordVerify)
            {
                _logger.LogWarning("Acesso de aplicação negado. Senha inválida para o usuário de aplicação {AppId}", appId);
                throw new ApplicationUserException(exceptionMessage);
            }

            _logger.LogInformation("Acesso de aplicação autenticado para o usuário de aplicação {AppId}", appId);
            return passwordVerify;
        }
        catch (ApplicationUserException e)
        {
            throw new ApplicationUserException(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Erro ao autenticar usuário de aplicação.\n\nMensagem de erro: {Message}\nStackTrace: {StackTrace}", e.Message, e.StackTrace);
            throw new ApplicationException($"Erro ao autenticar usuário de aplicação. Mensagem de erro: {e.Message}");
        }
    }
}