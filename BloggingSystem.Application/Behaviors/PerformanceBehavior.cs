using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BloggingSystem.Application.Behaviors;

/// <summary>
/// Performance behavior for MediatR pipeline
/// </summary>
/// <typeparam name="TRequest">Type of request</typeparam>
/// <typeparam name="TResponse">Type of response</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }
        
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();
            
        var response = await next();
            
        _timer.Stop();
            
        var elapsedMilliseconds = _timer.ElapsedMilliseconds;
            
        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
                
            _logger.LogWarning("Long running request: {RequestName} ({ElapsedMilliseconds} milliseconds)",
                requestName, elapsedMilliseconds);
        }
            
        return response;
    }
}