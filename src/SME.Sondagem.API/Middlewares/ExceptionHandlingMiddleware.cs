using System.Net;
using System.Text.Json;
using SME.Sondagem.Dominio;

namespace SME.Sondagem.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            RegraNegocioException regraNegocio => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = regraNegocio.Message,
                Type = "RegraNegocio"
            },
            ErroNaoEncontradoException erroNaoEncontrado => new
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = erroNaoEncontrado.Message,
                Type = "ErroNaoEncontrado"
            },
            ErroInternoException erroInterno => new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = erroInterno.Message,
                Type = "ErroInterno"
            },
            _ => new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.",
                Type = "ErroDesconhecido"
            }
        };

        context.Response.StatusCode = response.StatusCode;

        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(exception, "Erro capturado: {Message}", exception.Message);
        }

        var jsonResponse = JsonSerializer.Serialize(response, JsonOptions);

        await context.Response.WriteAsync(jsonResponse);
    }
}