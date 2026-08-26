using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestTask.Api.Integration;

/// <summary>
/// Minimal CRM auth: a shared API key in the X-Api-Key header.
/// Enough to show how an external system would be gated without a full identity stack.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    public const string HeaderName = "X-Api-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var expected = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["Integration:ApiKey"];

        context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided);

        if (!IsValid(expected, provided.ToString()))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "Нужен заголовок X-Api-Key с ключом интеграции."
            });
            return;
        }

        await next();
    }

    private static bool IsValid(string? expected, string? provided)
    {
        if (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(provided))
        {
            return false;
        }

        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        return expectedBytes.Length == providedBytes.Length
               && CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }
}
