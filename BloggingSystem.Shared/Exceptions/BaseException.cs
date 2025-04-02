using System;
using System.Net;

namespace BloggingSystem.Shared.Exceptions;

public class BaseException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }
    public object? AdditionalData { get; }

    public BaseException(string message) : base(message)
    {
        StatusCode = HttpStatusCode.InternalServerError;
        ErrorCode = "INTERNAL_SERVER_ERROR";
    }

    public BaseException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = statusCode.ToString().ToUpper();
    }

    public BaseException(string message, HttpStatusCode statusCode, string errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public BaseException(string message, HttpStatusCode statusCode, string errorCode, object? additionalData) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        AdditionalData = additionalData;
    }

    public BaseException(string message, Exception innerException) : base(message, innerException)
    {
        StatusCode = HttpStatusCode.InternalServerError;
        ErrorCode = "INTERNAL_SERVER_ERROR";
    }
}