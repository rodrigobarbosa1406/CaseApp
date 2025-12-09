using CaseApp.Enums.Invoices;
using CaseApp.Interfaces.Access;
using CaseApp.Interfaces.Invoices;
using CaseApp.Utils.Converters;
using CaseApp.Utils.ErrorHandler.Access;
using CaseApp.Utils.ErrorHandler.Commom;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace CaseApp.Endpoints.Invoices;

public static class ReconciliationEndpoints
{
    public static void MapReconciliationEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/Reconciliations", async (
            int? skip,
            int? take,
            HttpContext httpContext,
            IReconciliationService reconciliationService,
            ITokenService tokenService,
            ILogger<object> logger) =>
        {
            try
            {
                httpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
                tokenService.ValidateTokenAsync(headerValue);

                if (!skip.HasValue || !take.HasValue)
                    return Results.BadRequest("Parâmetros de paginação inválidos.");

                if (skip.Value < 0 || take.Value <= 0)
                    return Results.BadRequest("Parâmetros de paginação inválidos.");

                var reconciliations = await reconciliationService.GetReconciliationsAsync(skip.Value, take.Value);

                logger.LogInformation("Consulta de conciliações realizada");

                return Results.Ok(reconciliations);
            }
            catch (TokenJwtException e)
            {
                return Results.Json(
                        new { e.Message },
                        statusCode: StatusCodes.Status401Unauthorized
                    );
            }
            catch (ApplicationException e)
            {
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                logger.LogError("Falha ao consultar clientes. Mensagem de erro: {Message}. StackTrace: {StackTrace}", e.Message, e.StackTrace);
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("Reconciliations");

        app.MapGet("/api/v1/ReconciliationById/{id:Guid}", async (
            Guid id,
            HttpContext httpContext,
            IReconciliationService reconciliationService,
            ITokenService tokenService,
            ILogger<object> logger) =>
        {
            try
            {
                httpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
                tokenService.ValidateTokenAsync(headerValue);

                var reconciliation = await reconciliationService.GetReconciliationByIdAsync(id);

                logger.LogInformation("Consulta da conciliações id {Id} realizada", id);

                return Results.Ok(reconciliation);
            }
            catch (TokenJwtException e)
            {
                return Results.Json(
                        new { e.Message },
                        statusCode: StatusCodes.Status401Unauthorized
                    );
            }
            catch (NotFoundException e)
            {
                return Results.Json(
                        new { e.Message },
                        statusCode: StatusCodes.Status404NotFound
                    );
            }
            catch (ApplicationException e)
            {
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                logger.LogError("Falha ao consultar clientes. Mensagem de erro: {Message}. StackTrace: {StackTrace}", e.Message, e.StackTrace);
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("ReconciliationById");

        app.MapGet("/api/v1/ReconciliationsByCustomerId", async (
            int? customerId,
            int? skip,
            int? take,
            HttpContext httpContext,
            IReconciliationService reconciliationService,
            ITokenService tokenService,
            ILogger<object> logger) =>
        {
            try
            {
                httpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
                tokenService.ValidateTokenAsync(headerValue);

                if (!customerId.HasValue)
                    return Results.BadRequest("Parâmetro customerId é obrigatório.");

                if (!skip.HasValue || !take.HasValue)
                    return Results.BadRequest("Parâmetros de paginação inválidos.");

                if (skip.Value < 0 || take.Value <= 0)
                    return Results.BadRequest("Parâmetros de paginação inválidos.");

                var reconciliations = await reconciliationService.GetReconciliationsByCustomerIdAsync(customerId.Value, skip.Value, take.Value);

                logger.LogInformation("Consulta de conciliações por cliente realizada");

                return Results.Ok(reconciliations);
            }
            catch (TokenJwtException e)
            {
                return Results.Json(
                        new { e.Message },
                        statusCode: StatusCodes.Status401Unauthorized
                    );
            }
            catch (ApplicationException e)
            {
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                logger.LogError("Falha ao consultar clientes. Mensagem de erro: {Message}. StackTrace: {StackTrace}", e.Message, e.StackTrace);
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("ReconciliationsByCustomerId");

        app.MapGet("/api/v1/ReconciliationsByInvoiceId", async (
            string invoiceId,
            int? skip,
            int? take,
            HttpContext httpContext,
            IReconciliationService reconciliationService,
            ITokenService tokenService,
            ILogger<object> logger) =>
        {
            try
            {
                httpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
                tokenService.ValidateTokenAsync(headerValue);

                if (string.IsNullOrEmpty(invoiceId))
                    return Results.BadRequest("Parâmetro invoiceId é obrigatório.");

                if (!skip.HasValue || !take.HasValue)
                    return Results.BadRequest("Parâmetros de paginação inválidos.");

                if (skip.Value < 0 || take.Value <= 0)
                    return Results.BadRequest("Parâmetros de paginação inválidos.");

                var reconciliations = await reconciliationService.GetReconciliationsByInvoiceIdAsync(invoiceId, skip.Value, take.Value);

                logger.LogInformation("Consulta de conciliações por fatura realizada");

                return Results.Ok(reconciliations);
            }
            catch (TokenJwtException e)
            {
                return Results.Json(
                        new { e.Message },
                        statusCode: StatusCodes.Status401Unauthorized
                    );
            }
            catch (ApplicationException e)
            {
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                logger.LogError("Falha ao consultar clientes. Mensagem de erro: {Message}. StackTrace: {StackTrace}", e.Message, e.StackTrace);
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("ReconciliationsByInvoiceId");

        app.MapGet("/api/v1/ReconciliationsByStatusId", async (
            string reconciliationStatus,
            int? skip,
            int? take,
            HttpContext httpContext,
            IReconciliationService reconciliationService,
            ITokenService tokenService,
            ILogger<object> logger) =>
        {
            try
            {
                httpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValue);
                tokenService.ValidateTokenAsync(headerValue);

                if (string.IsNullOrEmpty(reconciliationStatus))
                    return Results.BadRequest("Parâmetro reconciliationStatus é obrigatório.");

                if (!skip.HasValue || !take.HasValue)
                    return Results.BadRequest("Parâmetros de paginação inválidos.");

                if (skip.Value < 0 || take.Value <= 0)
                    return Results.BadRequest("Parâmetros de paginação inválidos.");

                var reconciliationStatusParsed = CustomConverter.GetEnumValueFromDescription<ReconciliationStatus>(reconciliationStatus);

                var reconciliations = await reconciliationService.GetReconciliationsByStatusAsync(reconciliationStatusParsed, skip.Value, take.Value);

                logger.LogInformation("Consulta de conciliações por fatura realizada");

                return Results.Ok(reconciliations);
            }
            catch (TokenJwtException e)
            {
                return Results.Json(
                        new { e.Message },
                        statusCode: StatusCodes.Status401Unauthorized
                    );
            }
            catch (ApplicationException e)
            {
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                logger.LogError("Falha ao consultar clientes. Mensagem de erro: {Message}. StackTrace: {StackTrace}", e.Message, e.StackTrace);
                return Results.Problem(title: e.Message, statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("ReconciliationsByStatusId");
    }
}