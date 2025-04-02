using System;
using System.Collections.Generic;
using System.Net;

namespace BloggingSystem.Shared.Exceptions;

// Resource không tìm thấy
public class NotFoundException : BaseException
{
    public NotFoundException(string message) 
        : base(message, HttpStatusCode.NotFound, "RESOURCE_NOT_FOUND")
    {
    }
    
    public NotFoundException(string resourceName, object resourceId) 
        : base($"{resourceName} với id {resourceId} không tìm thấy", HttpStatusCode.NotFound, "RESOURCE_NOT_FOUND")
    {
    }
}

// Không có quyền truy cập
public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message) 
        : base(message, HttpStatusCode.Unauthorized, "UNAUTHORIZED_ACCESS")
    {
    }
    
    public UnauthorizedException() 
        : base("Bạn không có quyền truy cập tài nguyên này", HttpStatusCode.Unauthorized, "UNAUTHORIZED_ACCESS")
    {
    }
}

// Lỗi xác thực
public class AuthenticationException : BaseException
{
    public AuthenticationException(string message) 
        : base(message, HttpStatusCode.Unauthorized, "AUTHENTICATION_FAILED")
    {
    }
    
    public AuthenticationException() 
        : base("Xác thực không thành công", HttpStatusCode.Unauthorized, "AUTHENTICATION_FAILED")
    {
    }
}

// Lỗi xác nhận dữ liệu
public class ValidationException : BaseException
{
    public ValidationException(string message, object validationErrors) 
        : base(message, HttpStatusCode.BadRequest, "VALIDATION_ERROR", validationErrors)
    {
    }
    
    public ValidationException(object validationErrors) 
        : base("Dữ liệu không hợp lệ", HttpStatusCode.BadRequest, "VALIDATION_ERROR", validationErrors)
    {
    }
}

// Lỗi nghiệp vụ
public class BusinessRuleException : BaseException
{
    public BusinessRuleException(string message) 
        : base(message, HttpStatusCode.BadRequest, "BUSINESS_RULE_VIOLATION")
    {
    }
}

// Lỗi trùng lặp dữ liệu
public class DuplicateResourceException : BaseException
{
    public DuplicateResourceException(string message) 
        : base(message, HttpStatusCode.Conflict, "DUPLICATE_RESOURCE")
    {
    }
    
    public DuplicateResourceException(string resourceName, string fieldName, object fieldValue) 
        : base($"{resourceName} với {fieldName} là '{fieldValue}' đã tồn tại", HttpStatusCode.Conflict, "DUPLICATE_RESOURCE")
    {
    }
}

// Lỗi phụ thuộc
public class DependencyException : BaseException
{
    public DependencyException(string message) 
        : base(message, HttpStatusCode.Conflict, "DEPENDENCY_ERROR")
    {
    }
}

// Lỗi giới hạn truy cập
public class RateLimitException : BaseException
{
    public RateLimitException(string message, int retryAfterSeconds) 
        : base(message, HttpStatusCode.TooManyRequests, "RATE_LIMIT_EXCEEDED",
            new { RetryAfterSeconds = retryAfterSeconds })
    {
    }
}

// Lỗi quyền truy cập
public class ForbiddenException : BaseException
{
    public ForbiddenException(string message) 
        : base(message, HttpStatusCode.Forbidden, "ACCESS_FORBIDDEN")
    {
    }
    
    public ForbiddenException() 
        : base("Bạn không có quyền thực hiện hành động này", HttpStatusCode.Forbidden, "ACCESS_FORBIDDEN")
    {
    }
}

// Lỗi hệ thống file
public class FileSystemException : BaseException
{
    public FileSystemException(string message, Exception innerException) 
        : base(message, HttpStatusCode.InternalServerError, "FILE_SYSTEM_ERROR")
    {
    }
}

// Lỗi cấu hình
public class ConfigurationException : BaseException
{
    public ConfigurationException(string message) 
        : base(message, HttpStatusCode.InternalServerError, "CONFIGURATION_ERROR")
    {
    }
}

// Lỗi dịch vụ bên ngoài
public class ExternalServiceException : BaseException
{
    public ExternalServiceException(string message, string serviceName) 
        : base(message, HttpStatusCode.ServiceUnavailable, "EXTERNAL_SERVICE_ERROR", 
            new { ServiceName = serviceName })
    {
    }
}

// Lỗi giao dịch database
public class DatabaseException : BaseException
{
    public DatabaseException(string message, Exception innerException) 
        : base(message, HttpStatusCode.InternalServerError, "DATABASE_ERROR", innerException)
    {
    }
}

// Lỗi timeout
public class TimeoutException : BaseException
{
    public TimeoutException(string operation, int timeoutSeconds) 
        : base($"Thao tác '{operation}' đã vượt quá thời gian chờ ({timeoutSeconds}s)", 
            HttpStatusCode.RequestTimeout, "OPERATION_TIMEOUT")
    {
    }
}

// Lỗi tạm khóa tài khoản
public class AccountLockedException : BaseException
{
    public AccountLockedException(string message, DateTime unlockTime) 
        : base(message, HttpStatusCode.Forbidden, "ACCOUNT_LOCKED", 
            new { UnlockTime = unlockTime })
    {
    }
}

// Lỗi tính năng bị vô hiệu hóa
public class FeatureDisabledException : BaseException
{
    public FeatureDisabledException(string featureName) 
        : base($"Tính năng '{featureName}' hiện tại đang bị vô hiệu hóa", 
            HttpStatusCode.ServiceUnavailable, "FEATURE_DISABLED")
    {
    }
}