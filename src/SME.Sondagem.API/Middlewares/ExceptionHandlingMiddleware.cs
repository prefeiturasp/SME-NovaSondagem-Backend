using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using FluentValidation;
using SME.Sondagem.Dominio;
using SME.Sondagem.Dominio.Constantes.MensagensNegocio;

namespace SME.Sondagem.API.Middlewares;

[ExcludeFromCodeCoverage]
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
        {
            await WriteResponseAsync(
                context,
                StatusCodes.Status499ClientClosedRequest,
                MensagemNegocioComuns.REQUISICAO_CANCELADA,
                "RequestCanceled");
        }
        catch (ValidationException ex)
        {
            await WriteResponseAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Erro de validação",
                "ValidationError",
                ex.Errors.Select(e => new
                {
                    campo = e.PropertyName,
                    mensagem = e.ErrorMessage
                }));
        }
        catch (RegraNegocioException ex)
        {
            await WriteResponseAsync(
                context,
                ex.StatusCode,
                ex.Message,
                "RegraNegocio");
        }
        catch (ErroNaoEncontradoException ex)
        {
            await WriteResponseAsync(
                context,
                StatusCodes.Status404NotFound,
                ex.Message,
                "NotFound");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado");

            await WriteResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.",
                "InternalServerError");
        }
    }

    private async Task WriteResponseAsync(
        HttpContext context,
        int statusCode,
        string message,
        string type,
        object? details = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            statusCode,
            message,
            type,
            details
        };

        if (logger.IsEnabled(LogLevel.Error) && statusCode >= 500)
        {
            logger.LogError("Erro {Type}: {Message}", type, message);
        }

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions));
    }
}
