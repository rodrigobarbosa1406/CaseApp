using CaseApp.DataTransferObjects.Access;
using CaseApp.Interfaces.Access;
using CaseApp.Models.Access;
using CaseApp.Utils.ErrorHandler.Access;
using CaseApp.Utils.ErrorHandler.Commom;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace CaseApp.Endpoints.Access;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/api/v1/UserLogin", async (
            UserLoginPostDTO userLoginPostDTO,
            HttpContext httpContext,
            IUserService userService,
            IApplicationUserService applicationUserService,
            ITokenService tokenService,
            ILogger<object> logger) =>
        {
            try
            {
                httpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
                await applicationUserService.VerifyHashedPassword(headerValue);

                if (userLoginPostDTO == null)
                {
                    logger.LogInformation("Solicitação de login inválida. Corpo da solicitação nulo");
                    return Results.BadRequest("Dados inválidos");
                }

                var user = await userService.FindAsync(userLoginPostDTO.UserName);

                if (user == null)
                {
                    logger.LogInformation("Login não autorizado. O usuário {UserName} não existe", userLoginPostDTO.UserName);
                    return Results.BadRequest("Usuário e/ou senha inválidos");
                }

                var passwordHasher = new PasswordHasher<User>();

                if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userLoginPostDTO.PasswordHash) == PasswordVerificationResult.Failed)
                {
                    logger.LogWarning("Login não autorizado para o usuário {UserName}. Senha incorreta", userLoginPostDTO.UserName);
                    return Results.Json(
                            new { Message = "Usuário e/ou senha inválidos" },
                            statusCode: StatusCodes.Status401Unauthorized
                        );
                }

                var tokenJwt = string.Empty;
                var refreshTokenJwt = string.Empty;
                var utcNow = DateTime.UtcNow;

                tokenJwt = tokenService.GenerateToken(user, utcNow);
                refreshTokenJwt = tokenService.GenerateRefreshToken(user, utcNow, tokenJwt);

                logger.LogInformation("Login realizado pelo usuário {UserName}", userLoginPostDTO.UserName);

                var authenticatedUserDTO = new AuthenticatedUserGetDTO(tokenJwt, refreshTokenJwt);

                return Results.Ok(authenticatedUserDTO);
            }
            catch (ApplicationUserException e)
            {
                return Results.Json(
                        new { e.Message },
                        statusCode: StatusCodes.Status401Unauthorized
                    );
            }
            catch (NotFoundException)
            {
                logger.LogWarning("Login não autorizado para o usuário {UserName}. O status do usuário não existe", userLoginPostDTO.UserName);
                return Results.Json(
                            new { Message = "Usuário e/ou senha inválidos" },
                            statusCode: StatusCodes.Status401Unauthorized
                        );
            }
            catch (ArgumentException e)
            {
                logger.LogError("Mensagem de erro: {Message}\nStackTrace: {StackTrace}", e.Message, e.StackTrace);
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
            catch (ApplicationException e)
            {
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                logger.LogError("Falha na tentativa de login do usuário {UserName}\nMensagem de erro: {Message}\nStackTrace: {StackTrace}", userLoginPostDTO.UserName, e.Message, e.StackTrace);
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("UserLogin");
    }
}