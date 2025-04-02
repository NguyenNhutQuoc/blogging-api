using System.Net;
using BloggingSystem.Domain.Exceptions;
using BloggingSystem.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BloggingSystem.API.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case BaseException baseException:
                HandleBaseException(context, baseException);
                break;
            
            case EntityNotFoundException notFoundException:
                HandleNotFoundException(context, notFoundException);
                break;
            
            case DomainException domainException:
                HandleDomainException(context, domainException);
                break;

            default:
                HandleUnknownException(context);
                break;
        }

        context.ExceptionHandled = true;
    }

    private void HandleBaseException(ExceptionContext context, BaseException exception)
    {
        var statusCode = (int)exception.StatusCode;
        var errorResponse = new ErrorResponse
        {
            StatusCode = statusCode,
            ErrorCode = exception.ErrorCode,
            Message = exception.Message,
            Details = exception.AdditionalData
        };

        LogException(exception, statusCode);
        SetContextResult(context, statusCode, errorResponse);
    }

    private void HandleNotFoundException(ExceptionContext context, EntityNotFoundException exception)
    {
        const int statusCode = (int)HttpStatusCode.NotFound;
        var errorResponse = new ErrorResponse
        {
            StatusCode = statusCode,
            ErrorCode = "RESOURCE_NOT_FOUND",
            Message = exception.Message
        };

        LogException(exception, statusCode);
        SetContextResult(context, statusCode, errorResponse);
    }

    private void HandleDomainException(ExceptionContext context, DomainException exception)
    {
        const int statusCode = (int)HttpStatusCode.BadRequest;
        var errorResponse = new ErrorResponse
        {
            StatusCode = statusCode,
            ErrorCode = "VALIDATION_ERROR",
            Message = exception.Message
        };

        _logger.LogWarning(exception, "Domain validation error: {Message}", exception.Message);
        SetContextResult(context, statusCode, errorResponse);
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        const int statusCode = (int)HttpStatusCode.InternalServerError;
        var errorResponse = new ErrorResponse
        {
            StatusCode = statusCode,
            ErrorCode = "INTERNAL_SERVER_ERROR",
            Message = "An unexpected error occurred while processing the request."
        };

        // For development environment, include the exception details
        if (_environment.IsDevelopment())
        {
            errorResponse.Details = new
            {
                ExceptionMessage = context.Exception.Message,
                StackTrace = context.Exception.StackTrace
            };
        }

        _logger.LogError(context.Exception, "Unhandled exception occurred");
        SetContextResult(context, statusCode, errorResponse);
    }

    private void LogException(Exception exception, int statusCode)
    {
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "{Message}", exception.Message);
        }
        else if (statusCode >= 400)
        {
            _logger.LogWarning(exception, "{Message}", exception.Message);
        }
        else
        {
            _logger.LogInformation(exception, "{Message}", exception.Message);
        }
    }

    private static void SetContextResult(ExceptionContext context, int statusCode, ErrorResponse errorResponse)
    {
        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = statusCode
        };
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string ErrorCode { get; set; }
    public string Message { get; set; }
    public object Details { get; set; }
}