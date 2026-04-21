using System.Net;
using System.Text.Json;
using FluentValidation;
using Jobrythm.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Middleware;

public static class GlobalExceptionHandler
{
    public static async Task Handle(HttpContext context)
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception == null) return;

        var (statusCode, title) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Not Found"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),
            Application.Exceptions.ValidationException => (HttpStatusCode.UnprocessableEntity, "Validation Error"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict"),
            BadRequestException => (HttpStatusCode.BadRequest, "Bad Request"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        if (exception is Application.Exceptions.ValidationException validationException)
        {
            response.Extensions["errors"] = validationException.Errors;
        }

        response.Extensions["traceId"] = context.TraceIdentifier;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }
}
